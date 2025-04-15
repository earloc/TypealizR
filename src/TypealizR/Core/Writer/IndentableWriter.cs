using System;
using System.Text;

namespace TypealizR.Core.Writer;

public class IndentableWriter (Action<string> writeLine, int indentSize = 2)
{
    private sealed class Scope : IDisposable
    {
        private readonly IndentableWriter writer;
        private readonly int indentLevel;

        internal Scope(IndentableWriter writer)
        {
            this.writer = writer;
            indentLevel = writer.whiteSpaces.Length;
            writer.SetIndentLevel(indentLevel + writer.indentSize);
        }

        public void Dispose() => Dispose(true);

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                writer.SetIndentLevel(indentLevel);
            }
        }
    }
    
    string whiteSpaces = string.Empty;
    private readonly Action<string> writeLine = writeLine;
    private readonly int indentSize = indentSize;

    public IDisposable Indent() => new Scope(this);

    private void SetIndentLevel(int level) => whiteSpaces = new string(' ', level * 4);

    public void WriteLine(string line) => writeLine($"{whiteSpaces}{line}");
}
