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

            console.WriteLine($"🔍 scanning {project.FilePath}");

            var allNamespaces = FindNamespaces(compilation, cancellationToken);

            var markedInterfaces = allNamespaces
                .SelectMany(x => x.Members.OfType<InterfaceDeclarationSyntax>())
                .Select(x => new { Declaration = x, Model = compilation.GetSemanticModel(x.SyntaxTree) })
                .Select(x => new { x.Declaration, Symbol = x.Model.GetDeclaredSymbol(x.Declaration, cancellationToken) })
                .Where(x => x.Symbol is not null)
                .Select(x => new { x.Declaration, Symbol = x.Symbol! })
                .Where(x => x.Symbol
                    .GetAttributes()
                    .Any(x => x.AttributeClass?.Name.StartsWith(MarkerAttributeName) ?? false)
                )
                .ToArray()
            ;

            var allClasses = allNamespaces
                .SelectMany(x => x.Members.OfType<ClassDeclarationSyntax>())
                .Select(x => new { Declaration = x, Model = compilation.GetSemanticModel(x.SyntaxTree) })
                .Select(x => new { x.Declaration, x.Model, Symbol = x.Model.GetDeclaredSymbol(x.Declaration, cancellationToken) as INamedTypeSymbol })
                .Where(x => x.Symbol is not null)
                .Select(x => new { x.Declaration, x.Model, Symbol = x.Symbol! })
                .ToArray()
            ;

            var markedInterfacesIdentifier = markedInterfaces.Select(x => x.Symbol).ToArray();

            var typesImplementingMarkedInterfaces =
                allClasses
                .Where(x => x.Symbol.AllInterfaces
                    .Any(y => markedInterfacesIdentifier.Contains(y, SymbolEqualityComparer.Default))
                )
            ;

            foreach (var type in typesImplementingMarkedInterfaces)
            {
                var fileName = $"{type.Declaration.Identifier.Text}.resx";

                console.WriteLine($"  👀 {type.Declaration.Identifier.Text} -> {fileName}");

                var properties = type.Declaration.Members
                    .OfType<PropertyDeclarationSyntax>()
                    .Select(x => new { Syntax = x, ReturnType = x.Type as IdentifierNameSyntax })
                    .Where(x => x.ReturnType is not null)
                    .Select(x => new { x.Syntax, ReturnType = x.ReturnType! })
                    .ToArray()
                ;

                var methods = type.Declaration.Members
                    .OfType<MethodDeclarationSyntax>()
                    .Select(x => new { Syntax = x, ReturnType = x.ReturnType as IdentifierNameSyntax })
                    .Where(x => x.ReturnType is not null)
                    .Select(x => new { x.Syntax, ReturnType = x.ReturnType! })
                    .ToArray()
                ;

                var builder = new StringBuilder();

                foreach (var property in properties)
                {
                    var key = type.Declaration.Members
                        .OfType<FieldDeclarationSyntax>()
                        .Where(x => x.Modifiers.Any(y => y.Text == "const"))
                        .Select(x => x.Declaration.Variables.SingleOrDefault())
                        .Where(x => x is not null).Select(x => x!)
                        .Where(x => x.Identifier.Text == $"{property.Syntax.Identifier.Text}_Key")
                        .FirstOrDefault()
                    ;
                    builder.AppendLine($"{property.Syntax.Identifier.Text} = {key?.Initializer?.Value}");
                }

                foreach (var method in methods)
                {
                    var key = type.Declaration.Members
                        .OfType<FieldDeclarationSyntax>()
                        .Where(x => x.Modifiers.Any(y => y.Text == "const"))
                        .Select(x => x.Declaration.Variables.SingleOrDefault())
                        .Where(x => x is not null).Select(x => x!)
                        .Where(x => x.Identifier.Text == $"{method.Syntax.Identifier.Text}_Key")
                        .FirstOrDefault();
                    builder.AppendLine($"{method.Syntax.Identifier.Text} = {key?.Initializer?.Value}");
                }

                await storage.AddAsync(fileName, builder.ToString());
            }
        }

        private static IEnumerable<BaseNamespaceDeclarationSyntax> FindNamespaces(Compilation compilation, CancellationToken cancellationToken)
            => compilation.SyntaxTrees
                .Where(x => x.GetRoot() is CompilationUnitSyntax)
                .Select(x => (CompilationUnitSyntax)x.GetRoot(cancellationToken))
                .SelectMany(x => x.Members.OfType<BaseNamespaceDeclarationSyntax>())
                .ToArray()
            ;
    }
}
