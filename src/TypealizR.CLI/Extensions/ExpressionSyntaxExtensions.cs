namespace Microsoft.CodeAnalysis.CSharp.Syntax;
internal static class ExpressionSyntaxExtensions
{
    public static string ToResourceKey(this string that) => that.Trim('@', '$', '"');
}
