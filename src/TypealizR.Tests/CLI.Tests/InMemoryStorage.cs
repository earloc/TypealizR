using TypealizR.CLI.Abstractions;

namespace TypealizR.Tests.CLI.Tests;
internal class InMemoryStorage : IStorage
{
    private Dictionary<string, string> files = new();
    public Task AddAsync(string fileName, string content)
    {
        files.Add(fileName, content);
        return Task.CompletedTask;
    }

    public IReadOnlyDictionary<string, string> Files => files;
}