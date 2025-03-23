namespace Microsoft.CodeAnalysis.CSharp.Syntax;
internal static class ExpressionSyntaxExtensions
{
    public static string ToResourceKey(this ExpressionSyntax that) => that.ToString().Trim('@', '$', '"');
}
