using System.CommandLine;
using System.CommandLine.Invocation;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using TypealizR.CLI.Abstractions;
using TypealizR.CLI.Resources;
using TypealizR.Core.Writer;

namespace TypealizR.CLI.Commands.CodeFirst;
internal class ExportCommand : Command
{
    public ExportCommand() : base("export", "exports typealized interfaces to a resource-file")
    {
        AddAlias("ex");
        var projectArgument = new Argument<FileInfo>("--project");
        AddArgument(projectArgument);
    }

    internal class Implementation : ICommandHandler
    {
        private readonly IndentableWriter writer;
        private readonly IStorage storage;
        public FileInfo? Project { get; set; }

        public Implementation(IConsole console, IStorage storage)
        {
            writer = new(console.WriteLine);
            this.storage = storage;
        }

        public int Invoke(InvocationContext context) => throw new NotImplementedException();

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

            writer.WriteLine($"📖 opening {Project.FullName}");
            var project = await w.OpenProjectAsync(Project.FullName, cancellationToken: cancellationToken);

            using (writer.Indent())
            {
                await ExportAsync(writer, project, storage, cancellationToken);
            }

            return 0;
        }

        private const string MarkerAttributeName = "CodeFirstTypealized";

        private static async Task ExportAsync(IndentableWriter writer, Project project, IStorage storage, CancellationToken cancellationToken)
        {
            writer.WriteLine($"🚀 building");

            var compilation = await project.GetCompilationAsync(cancellationToken);

            if (compilation is null)
            {
                return;
            }

            var directory = Directory.GetParent(project.FilePath ?? "")?.FullName ?? "";

            writer.WriteLine($"🔍 scanning");

            var allNamespaces = FindNamespaces(compilation, cancellationToken).ToArray();
            if (allNamespaces.Length == 0)
            {
                writer.WriteLine("⚠️ no namespaces found");
            }

            var markedInterfaces = FindInterfaces(compilation, allNamespaces, cancellationToken).ToArray();
            if (markedInterfaces.Length == 0)
            {
                writer.WriteLine("⚠️ no typealized interfaces found");
            }

            var typesImplementingMarkedInterfaces = FindClasses(compilation, allNamespaces, markedInterfaces, cancellationToken).ToArray();
            if (typesImplementingMarkedInterfaces.Length == 0)
            {
                writer.WriteLine("⚠️ no classes implementing typealized interfaces found");
            }

            using (writer.Indent())
            {
                await ExportAsync(writer, directory, typesImplementingMarkedInterfaces, storage);
            }
        }

        private static async Task ExportAsync(IndentableWriter writer, string baseDirectory, IEnumerable<TypeInfo> types, IStorage storage)
        {
            foreach (var type in types)
            {
                var interfaceFile = type.ImplementingInterface.Declaration.SyntaxTree.FilePath;
                var interfacePath = Path.GetDirectoryName(interfaceFile) ?? "";

                var fileName = type.ImplementingInterface.Declaration.Identifier.Text;
                var containingTypes = type.ImplementingInterface.Symbol.ContainingType.GetContainingTypesRecursive().Join("+");
                if (!string.IsNullOrEmpty(containingTypes))
                {
                    fileName = $"{containingTypes}+{fileName}";
                }

                var resourcefileName = Path.Combine(interfacePath, $"{fileName}.resx");

                writer.WriteLine($"👀 found        {interfaceFile.Replace(baseDirectory, "", StringComparison.Ordinal)}");
                using (writer.Indent())
                {
                    writer.WriteLine($"🆕 generating {resourcefileName.Replace(baseDirectory, "", StringComparison.Ordinal)}");
                }

                var builder = new ResxBuilder();
                using (writer.Indent())
                {
                    foreach (var property in type.Declaration.Members
                    .OfType<PropertyDeclarationSyntax>()
                    .Where(x => !x.Identifier.Text.EndsWith("_Raw", StringComparison.Ordinal))
                    )
                    {
                        AddProperty(writer, type, builder, property);
                    }

                    foreach (var method in type.Declaration.Members.OfType<MethodDeclarationSyntax>())
                    {
                        AddMethod(writer, type, builder, method);
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

        private static IEnumerable<InterfaceInfo> FindInterfaces
        (
            Compilation compilation,
            IEnumerable<BaseNamespaceDeclarationSyntax> allNamespaces,
            CancellationToken cancellationToken
        )
            => allNamespaces
                .SelectMany(x => GetAllInterfaceDeclarations(x.Members))
                .Select(x => new { Declaration = x, Model = compilation.GetSemanticModel(x.SyntaxTree) })
                .Select(x => new { x.Declaration, Symbol = x.Model.GetDeclaredSymbol(x.Declaration, cancellationToken) })
                .Where(x => x.Symbol is not null)
                .Select(x => new InterfaceInfo(x.Declaration, x.Symbol!))
                .Where(x => x.Symbol
                    .GetAttributes()
                    .Any(x => x.AttributeClass is not null && x.AttributeClass!.Name.StartsWith(MarkerAttributeName, StringComparison.Ordinal))
                )
        ;

        private static IEnumerable<InterfaceDeclarationSyntax> GetAllInterfaceDeclarations(SyntaxList<MemberDeclarationSyntax> members)
        {
            foreach (var member in members)
            {
                if (member is InterfaceDeclarationSyntax interfaceDeclaration)
                {
                    yield return interfaceDeclaration;
                }
                else if (member is ClassDeclarationSyntax classDeclaration)
                {
                    foreach (var innerInterface in GetAllInterfaceDeclarations(classDeclaration.Members))
                    {
                        yield return innerInterface;
                    }
                }
            }
        }

        private static IEnumerable<TypeInfo> FindClasses(
                Compilation compilation,
                IEnumerable<BaseNamespaceDeclarationSyntax> allNamespaces,
                IEnumerable<InterfaceInfo> markedInterfacesIdentifier,
                CancellationToken cancellationToken
        )
        {
            var interfaces = markedInterfacesIdentifier.ToDictionary(x => x.Symbol, SymbolEqualityComparer.Default);

            return allNamespaces
                .SelectMany(x => GetAllClassDeclarations(x.Members))
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

        private static IEnumerable<ClassDeclarationSyntax> GetAllClassDeclarations(SyntaxList<MemberDeclarationSyntax> members)
        {
            foreach (var member in members)
            {
                if (member is ClassDeclarationSyntax classDeclaration)
                {
                    yield return classDeclaration;

                    // Recursively search for inner classes
                    foreach (var innerClass in GetAllClassDeclarations(classDeclaration.Members))
                    {
                        yield return innerClass;
                    }
                }
            }
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

        private static void AddProperty(IndentableWriter writer, TypeInfo type, ResxBuilder builder, PropertyDeclarationSyntax property)
        {
            var key = FindKeyOf(type, property);
            var sanitizedValue = key?.Initializer?.Value?.ToResourceKey() ?? "";
            
            if (key is not null && !string.IsNullOrEmpty(sanitizedValue))
            {
                var commentTrivia = property.Identifier.GetAllTrivia().FirstOrDefault(t => t.IsKind(SyntaxKind.SingleLineCommentTrivia));
                var commentText = commentTrivia.ToFullString()?.Replace("//", "", StringComparison.InvariantCulture);
                
                builder.Add(property.Identifier.Text, sanitizedValue, commentText);
                writer.WriteLine($"✔️ {property.Identifier.Text}");
            }
            else
            {
                writer.WriteLine($"⚠️ {property.Identifier.Text} - invalid key");
            }
        }

        private static void AddMethod(IndentableWriter writer, TypeInfo type, ResxBuilder builder, MethodDeclarationSyntax method)
        {
            var key = FindKeyOf(type, method);
            var sanitizedValue = key?.Initializer?.Value?.ToResourceKey() ?? "";
            
            if (key is not null && !string.IsNullOrEmpty(sanitizedValue))
            {
                var commentTrivia = method.DescendantTrivia(null, true).FirstOrDefault(t => t.IsKind(SyntaxKind.SingleLineCommentTrivia));
                var commentText = commentTrivia.ToFullString()?.Replace("//", "", StringComparison.InvariantCulture);
                
                builder.Add(method.Identifier.Text, sanitizedValue, commentText);
                writer.WriteLine($"✔️ {method.Identifier.Text}()");
            }
            else
            {
                writer.WriteLine($"⚠️ {method.Identifier.Text} - invalid key");
            }
        }
    }
}
