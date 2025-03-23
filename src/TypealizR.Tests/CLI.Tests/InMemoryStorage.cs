using TypealizR.CLI.Abstractions;

namespace TypealizR.Tests.CLI.Tests;
internal sealed class InMemoryStorage : IStorage
{
    private readonly Dictionary<string, string> files = [];
    public Task AddAsync(string fileName, string content)
    {
        files.Add(fileName, content);
        return Task.CompletedTask;
    }

    public IReadOnlyDictionary<string, string> Files => files;
}