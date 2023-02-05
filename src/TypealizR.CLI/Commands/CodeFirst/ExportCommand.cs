using System.CommandLine;
using System.CommandLine.Invocation;
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
            if (!MSBuildLocator.IsRegistered)
            {
                MSBuildLocator.RegisterDefaults();
            }

            if (Project is null)
            {
                throw new InvalidOperationException($"{nameof(Project)} is missing");
            }

            var cancellationToken = context.GetCancellationToken();

            using var w = MSBuildWorkspace.Create();

            console.WriteLine($"📖 opening {Project.FullName}");

            var project = await w.OpenProjectAsync(Project.FullName, cancellationToken: cancellationToken);

            await ExportAsync(console, project, storage, cancellationToken);

            return 0;
        }

        private const string MarkerAttributeName = "CodeFirstTypealized";

        private static async Task ExportAsync(IConsole console, Project project, IStorage storage, CancellationToken cancellationToken)
        {
            console.WriteLine($"  🚀 building");

            var compilation = await project.GetCompilationAsync(cancellationToken);

            if (compilation is null)
            {
                return;
            }

            await ExportAsync(console, project, storage, compilation, cancellationToken);
        }

        private static async Task ExportAsync(IConsole console, Project project, IStorage storage, Compilation compilation, CancellationToken cancellationToken)
        {
            var directory = Directory.GetParent(project.FilePath ?? "")?.FullName ?? "";

            console.WriteLine($"  🔍 scanning");

            var allNamespaces = FindNamespaces(compilation, cancellationToken).ToArray();
            if (!allNamespaces.Any())
            {
                console.WriteLine("  ⚠️ no namespaces found");
            }

            var markedInterfaces = FindInterfaces(compilation, allNamespaces, cancellationToken).ToArray();
            if (!markedInterfaces.Any())
            {
                console.WriteLine("  ⚠️ no typealized interfaces found");
            }

            var typesImplementingMarkedInterfaces = FindClasses(compilation, allNamespaces, markedInterfaces, cancellationToken).ToArray();
            if (!typesImplementingMarkedInterfaces.Any())
            {
                console.WriteLine("  ⚠️ no classes implementing typealized interfaces found");
            }
            foreach (var type in typesImplementingMarkedInterfaces)
            {
                var interfaceFile = type.ImplementingInterface.Declaration.SyntaxTree.FilePath;
                var interfacePath = Path.GetDirectoryName(interfaceFile) ?? "";

                var resourcefileName = Path.Combine(interfacePath, $"{type.ImplementingInterface.Declaration.Identifier.Text}.resx");

                console.WriteLine($"    👀 found        {interfaceFile.Replace(directory, "")}");
                console.WriteLine($"      🆕 generating {resourcefileName.Replace(directory, "")}");

                var builder = new ResxBuilder();

                foreach (var property in type.Declaration.Members
                    .OfType<PropertyDeclarationSyntax>()
                    .Where(x => !x.Identifier.Text.EndsWith("_Raw"))
                ) {
                    var key = FindKeyOf(type, property);
                    var sanitizedValue = key?.Initializer?.Value?.ToResourceKey() ?? "";
                    
                    if (key is not null && !string.IsNullOrEmpty(sanitizedValue))
                    {
                        builder.Add(property.Identifier.Text, sanitizedValue);
                        console.WriteLine($"        ✔️ {property.Identifier.Text}");
                    }
                    else
                    {
                        console.WriteLine($"        ⚠️ {property.Identifier.Text} - invalid key");
                    }
                }

                foreach (var method in type.Declaration.Members.OfType<MethodDeclarationSyntax>())
                {
                    var key = FindKeyOf(type, method);
                    var sanitizedValue = key?.Initializer?.Value?.ToResourceKey() ?? "";
                    if (key is not null && !string.IsNullOrEmpty(sanitizedValue))
                    {
                        //TODO: emit diagnostics, otherwise
                        builder.Add(method.Identifier.Text, sanitizedValue);
                        console.WriteLine($"        ✔️ {method.Identifier.Text}()");
                    }
                    else
                    {
                        console.WriteLine($"        ⚠️ {method.Identifier.Text} - invalid key");
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
                    .Any(x => x.AttributeClass is not null && x.AttributeClass!.Name.StartsWith(MarkerAttributeName))
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
                .Select(x => new TypeInfo(x.Declaration, interfaces[x.Interface!]))
            ;
        }

        private static VariableDeclaratorSyntax? FindKeyOf(TypeInfo type, PropertyDeclarationSyntax propertySyntax)
            => type.Declaration.Members
                .OfType<FieldDeclarationSyntax>()
                .Where(x => x.Modifiers.Any(y => y.Text == "const"))
                .Select(x => x.Declaration.Variables.SingleOrDefault())
                .Where(x => x is not null).Select(x => x!)
                .FirstOrDefault(x => x.Identifier.Text == $"{propertySyntax.Identifier.Text}{TypealizR._.FallBackKeySuffix}")
        ;

        private static VariableDeclaratorSyntax? FindKeyOf(TypeInfo type, MethodDeclarationSyntax methodSyntax)
            => type.Declaration.Members
                .OfType<FieldDeclarationSyntax>()
                .Where(x => x.Modifiers.Any(y => y.Text == "const"))
                .Select(x => x.Declaration.Variables.SingleOrDefault())
                .Where(x => x is not null).Select(x => x!)
                .FirstOrDefault(x => x.Identifier.Text == $"{methodSyntax.Identifier.Text}{TypealizR._.FallBackKeySuffix}")
        ;
    }
}
