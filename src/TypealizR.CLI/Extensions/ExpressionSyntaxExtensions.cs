using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CodeAnalysis.CSharp.Syntax;
internal static class ExpressionSyntaxExtensions
{
    public static string ToResourceKey(this ExpressionSyntax that)
        => that.ToString().Trim('@', '$', '"');
}
