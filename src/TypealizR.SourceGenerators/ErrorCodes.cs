using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace TypealizR.SourceGenerators;
internal static class ErrorCodes
{
    internal static DiagnosticDescriptor TargetProjectRootDirectoryNotFound_000010() =>
        new(id: "TYPEALIZR000010", 
            title:"TargetProjectRootDirectoryNotFound", 
            messageFormat: "The code generator could not determine the projects root-directory", 
            category:"Project", 
            defaultSeverity: DiagnosticSeverity.Error, 
            isEnabledByDefault: true,
            description: "The code generator could not determine the projects root-directory"
        );

}
