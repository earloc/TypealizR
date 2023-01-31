using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using TypealizR.CLI.Abstractions;

namespace TypealizR.CLI.Commands.CodeFirst;
internal class ExportCommand
{
    public static async Task Handle(FileInfo projectFile, IStorage storage, CancellationToken cancellationToken)
    {
        if (!MSBuildLocator.IsRegistered)
        {
            MSBuildLocator.RegisterDefaults();
        }
        using var w = MSBuildWorkspace.Create();

        var project = await w.OpenProjectAsync(projectFile.FullName, cancellationToken: cancellationToken);

        await ExportAsync(project, storage, cancellationToken);
    }

    private const string MarkerAttributeName = "CodeFirstTypealized";

    private static async Task ExportAsync(Project project, IStorage storage, CancellationToken cancellationToken)
    {
        Console.WriteLine($"🚀 building {project.FilePath}");

        var compilation = await project.GetCompilationAsync(cancellationToken);

        if (compilation is null)
        {
            return;
        }

        Console.WriteLine($"🔍 scanning {project.FilePath}");

        var allNamespaces = FindNamespaces(compilation, cancellationToken);

        var allInterfaces = allNamespaces
            .SelectMany(x => x.Members.OfType<InterfaceDeclarationSyntax>())
            .Select(x => new { Declaration = x, Model = compilation.GetSemanticModel(x.SyntaxTree) })
            .ToArray()
        ;

        var markedInterfaces = allInterfaces
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

            Console.WriteLine($"  👀 {type.Declaration.Identifier.Text} -> {fileName}");

            var builder = new StringBuilder();
            foreach (var member in type.Declaration.Members)
            {
                builder.AppendLine(member.ToString());

                //Console.WriteLine($"    ✅ {member}");
                //Console.WriteLine($"    ❌ {member}");
                //Console.WriteLine($"    ⚠️ {member}");
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
