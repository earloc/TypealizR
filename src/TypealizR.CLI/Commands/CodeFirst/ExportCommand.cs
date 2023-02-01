using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using TypealizR.CLI.Abstractions;
using TypealizR.CLI.Resources;

namespace TypealizR.CLI.Commands.CodeFirst;
internal class ExportCommand : Command
{
    public ExportCommand() : base("export", "exports typealized interfaces to a resource-file")
    {
        AddAlias("ex");
        var projectArgument = new Argument<FileInfo>("--project");
        AddArgument(projectArgument);
    }

    public class Implementation : ICommandHandler
    {
        private readonly IConsole console;
        private readonly IStorage storage;
        public FileInfo? Project { get; set; }

        public Implementation(IConsole console, IStorage storage)
        {
            this.console = console;
            this.storage = storage;
        }

        public int Invoke(InvocationContext context)
        {
            throw new NotImplementedException();
        }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            if (Project is null)
            {
                throw new InvalidOperationException($"{nameof(Project)} is missing");
            }

            var cancellationToken = context.GetCancellationToken();

            if (!MSBuildLocator.IsRegistered)
            {
                MSBuildLocator.RegisterDefaults();
            }
            using var w = MSBuildWorkspace.Create();


            var project = await w.OpenProjectAsync(Project.FullName, cancellationToken: cancellationToken);

            await ExportAsync(console, project, storage, cancellationToken);

            return 0;
        }

        private const string MarkerAttributeName = "CodeFirstTypealized";

        private static async Task ExportAsync(IConsole console, Project project, IStorage storage, CancellationToken cancellationToken)
        {
            console.WriteLine($"🚀 building {project.FilePath}");

            var compilation = await project.GetCompilationAsync(cancellationToken);

            if (compilation is null)
            {
                return;
            }

            var diagnostics = compilation.GetDiagnostics(cancellationToken).Where(x => x.Severity == DiagnosticSeverity.Error);

            if (diagnostics.Any())
            {
                throw new Exception("💥 project contains errors");
            }

            await ExportAsync(console, project, storage, compilation, cancellationToken);
        }

        private static async Task ExportAsync(IConsole console, Project project, IStorage storage, Compilation compilation, CancellationToken cancellationToken)
        {
            console.WriteLine($"🔍 scanning {project.FilePath}");

            var allNamespaces = FindNamespaces(compilation, cancellationToken).ToArray();
            var markedInterfaces = FindInterfaces(compilation, allNamespaces, cancellationToken).ToArray();

            var typesImplementingMarkedInterfaces = FindClasses(compilation, allNamespaces, markedInterfaces, cancellationToken).ToArray();

            foreach (var type in typesImplementingMarkedInterfaces)
            {
                var interfaceFile = type.ImplementingInterface.Declaration.SyntaxTree.FilePath;
                var interfacePath = Path.GetDirectoryName(interfaceFile) ?? "";

                var resourcefileName = Path.Combine(interfacePath, $"{type.Declaration.Identifier.Text}.resx");

                console.WriteLine($"  👀   {interfaceFile}");
                console.WriteLine($"    -> {resourcefileName}");

                var builder = new ResxBuilder();

                foreach (var property in FindProperties(type))
                {
                    var key = FindKeyOf(type, property);
                    if (key?.Initializer?.Value is not null)
                    {
                        builder.Add(property.Syntax.Identifier.Text, key?.Initializer?.Value.ToString().Trim('@', '$', '"') ?? "");
                    }
                }

                foreach (var method in FindMethods(type))
                {
                    var key = FindKeyOf(type, method);
                    if (key?.Initializer?.Value is not null)
                    {
                        builder.Add(method.Syntax.Identifier.Text, key?.Initializer?.Value.ToString().Trim('@', '$', '"') ?? "");
                    }
                }

                var content = builder.Build();
                await storage.AddAsync(resourcefileName, content);
            }
        }

        private static IEnumerable<BaseNamespaceDeclarationSyntax> FindNamespaces(Compilation compilation, CancellationToken cancellationToken)
            => compilation.SyntaxTrees
                .Where(x => x.GetRoot() is CompilationUnitSyntax)
                .Select(x => (CompilationUnitSyntax)x.GetRoot(cancellationToken))
                .SelectMany(x => x.Members.OfType<BaseNamespaceDeclarationSyntax>())
        ;

        private static IEnumerable<InterfaceInfo> FindInterfaces(Compilation compilation, IEnumerable<BaseNamespaceDeclarationSyntax> allNamespaces, CancellationToken cancellationToken)
            => allNamespaces
                .SelectMany(x => x.Members.OfType<InterfaceDeclarationSyntax>())
                .Select(x => new { Declaration = x, Model = compilation.GetSemanticModel(x.SyntaxTree) })
                .Select(x => new { x.Declaration, Symbol = x.Model.GetDeclaredSymbol(x.Declaration, cancellationToken) })
                .Where(x => x.Symbol is not null)
                .Select(x => new InterfaceInfo(x.Declaration, x.Symbol!))
                .Where(x => x.Symbol
                    .GetAttributes()
                    .Any(x => x.AttributeClass?.Name.StartsWith(MarkerAttributeName) ?? false)
                )
        ;

        private static IEnumerable<TypeInfo> FindClasses(Compilation compilation, IEnumerable<BaseNamespaceDeclarationSyntax> allNamespaces, IEnumerable<InterfaceInfo> markedInterfacesIdentifier, CancellationToken cancellationToken)
        {
            var interfaces = markedInterfacesIdentifier.ToDictionary(x => x.Symbol, SymbolEqualityComparer.Default);

            return allNamespaces
                .SelectMany(x => x.Members.OfType<ClassDeclarationSyntax>())
                .Select(x => new { Declaration = x, Model = compilation.GetSemanticModel(x.SyntaxTree) })
                .Select(x => new { x.Declaration, x.Model, Symbol = x.Model.GetDeclaredSymbol(x.Declaration, cancellationToken) as INamedTypeSymbol })
                .Where(x => x.Symbol is not null)
                .Select(x => new
                {
                    x.Declaration,
                    x.Model,
                    Symbol = x.Symbol!,
                    Interface = x.Symbol!.AllInterfaces
                        .FirstOrDefault(y => interfaces.ContainsKey(y))
                })
                .Where(x => x.Interface is not null)
                .Select(x => new TypeInfo(x.Declaration, x.Model, x.Symbol, interfaces[x.Interface!]))
            ;
        }

        private static IEnumerable<PropertyInfo> FindProperties(TypeInfo type)
            => type.Declaration.Members
                .OfType<PropertyDeclarationSyntax>()
                .Select(x => new { Syntax = x, ReturnType = x.Type as IdentifierNameSyntax })
                .Where(x => x.ReturnType is not null)
                .Select(x => new PropertyInfo(x.Syntax, x.ReturnType!))
        ;
        private static IEnumerable<MethodInfo> FindMethods(TypeInfo type)
            => type.Declaration.Members
                .OfType<MethodDeclarationSyntax>()
                .Select(x => new { Syntax = x, ReturnType = x.ReturnType as IdentifierNameSyntax })
                .Where(x => x.ReturnType is not null)
                .Select(x => new MethodInfo(x.Syntax, x.ReturnType!))
        ;

        private static VariableDeclaratorSyntax? FindKeyOf(TypeInfo type, PropertyInfo property)
            => type.Declaration.Members
                .OfType<FieldDeclarationSyntax>()
                .Where(x => x.Modifiers.Any(y => y.Text == "const"))
                .Select(x => x.Declaration.Variables.SingleOrDefault())
                .Where(x => x is not null).Select(x => x!)
                .FirstOrDefault(x => x.Identifier.Text == $"{property.Syntax.Identifier.Text}_Key")
        ;

        private static VariableDeclaratorSyntax? FindKeyOf(TypeInfo type, MethodInfo method)
            => type.Declaration.Members
                .OfType<FieldDeclarationSyntax>()
                .Where(x => x.Modifiers.Any(y => y.Text == "const"))
                .Select(x => x.Declaration.Variables.SingleOrDefault())
                .Where(x => x is not null).Select(x => x!)
                .FirstOrDefault(x => x.Identifier.Text == $"{method.Syntax.Identifier.Text}_Key")
        ;
    }
}
