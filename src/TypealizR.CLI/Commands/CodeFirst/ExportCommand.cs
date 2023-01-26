using System;
using System.Collections.Generic;
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
        
        var project = await w.OpenProjectAsync (projectFile.FullName, cancellationToken:cancellationToken);

        await ExportAsync(project, cancellationToken);

    }
    private const string MarkerAttributeName = "CodeFirstTypealized";

    private static async Task ExportAsync(Project project, CancellationToken cancellationToken)
    {
        var compilation = await project.GetCompilationAsync(cancellationToken);

        if (compilation is null)
        {
            return;
        }

        var allInterfaces = compilation.SyntaxTrees
            .Where(x => x.GetRoot() is CompilationUnitSyntax)
            .Select(x => (CompilationUnitSyntax)x.GetRoot())
            .SelectMany(x => x.Members.OfType<BaseNamespaceDeclarationSyntax>())
            .SelectMany(x => x.Members.OfType<InterfaceDeclarationSyntax>())
            .Select(x => new { Declaration = x, Model = compilation.GetSemanticModel(x.SyntaxTree) })
        ;

        var markedInterfaces = allInterfaces
            .Select((x, cancel) => new { x.Declaration, Model = x.Model.GetDeclaredSymbol(x.Declaration) })
            .Where(x => x.Model is not null)
            .Select((x, cancel) => new { x.Declaration, Model = x.Model! })
            .Where(x => x.Model
                .GetAttributes()
                .Any(x => x.AttributeClass?.Name.StartsWith(MarkerAttributeName) ?? false)
            )
        ;

        foreach (var markedInterface in markedInterfaces)
        {
            Console.WriteLine(markedInterface.Model.ToDisplayString());

            foreach (var member in markedInterface.Declaration.Members)
            {
                Console.WriteLine(member.ToString());
            }
        }


    }
}
