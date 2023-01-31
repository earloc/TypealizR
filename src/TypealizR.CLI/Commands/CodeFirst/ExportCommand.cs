using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;

namespace TypealizR.CLI.Commands.CodeFirst;
internal class ExportCommand
{
    public static async Task Handle(string baseDirectory, CancellationToken cancellationToken)
    {
        var directory = new DirectoryInfo(baseDirectory);

        if (!MSBuildLocator.IsRegistered)
        {
            MSBuildLocator.RegisterDefaults();
        }

        foreach (var projectFile in directory.GetFiles("*.csproj"))
        {
            await ExportAsync(projectFile, cancellationToken);
        }
    }

    private static async Task ExportAsync(FileInfo projectFile, CancellationToken cancellationToken)
    {
        using var w = MSBuildWorkspace.Create();

        var project = await w.OpenProjectAsync(projectFile.FullName, cancellationToken: cancellationToken);

        await ExportAsync(project, cancellationToken);

    }
    private const string MarkerAttributeName = "CodeFirstTypealized";

    private static async Task ExportAsync(Project project, CancellationToken cancellationToken)
    {
        Console.WriteLine($"🚀 building {project.FilePath}");

        var compilation = await project.GetCompilationAsync(cancellationToken);

        if (compilation is null)
        {
            return;
        }

        Console.WriteLine($"🔍 scanning {project.FilePath}");

        var allNameSpaces = compilation.SyntaxTrees
            .Where(x => x.GetRoot() is CompilationUnitSyntax)
            .Select(x => (CompilationUnitSyntax)x.GetRoot())
            .SelectMany(x => x.Members.OfType<BaseNamespaceDeclarationSyntax>())
            .ToArray()
        ;

        var allInterfaces = allNameSpaces
            .SelectMany(x => x.Members.OfType<InterfaceDeclarationSyntax>())
            .Select(x => new { Declaration = x, Model = compilation.GetSemanticModel(x.SyntaxTree) })
            .ToArray()
        ;

        var markedInterfaces = allInterfaces
            .Select((x, cancel) => new { x.Declaration, Symbol = x.Model.GetDeclaredSymbol(x.Declaration) })
            .Where(x => x.Symbol is not null)
            .Select((x, cancel) => new { x.Declaration, Symbol = x.Symbol! })
            .Where(x => x.Symbol
                .GetAttributes()
                .Any(x => x.AttributeClass?.Name.StartsWith(MarkerAttributeName) ?? false)
            )
            .ToArray()
        ;

        var allClasses = allNameSpaces
            .SelectMany(x => x.Members.OfType<ClassDeclarationSyntax>())
            .Select(x => new { Declaration = x, Model = compilation.GetSemanticModel(x.SyntaxTree) })
            .Select(x => new { x.Declaration, x.Model, Symbol = x.Model.GetDeclaredSymbol(x.Declaration) as INamedTypeSymbol })
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
            Console.WriteLine($"  👀 {type.Declaration.Identifier.Text} -> {type.Declaration.Identifier.Text}.resx");

            foreach (var member in type.Declaration.Members)
            {
                Console.WriteLine($"    ✅ {member}");
                Console.WriteLine($"    ❌ {member}");
                Console.WriteLine($"    ⚠️ {member}");
            }
        }


    }
}
