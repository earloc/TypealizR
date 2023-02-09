namespace TypealizR.CLI.Abstractions;
internal class FileStorage : IStorage
{
    public async Task AddAsync(string fileName, string content)
    {
        await File.WriteAllTextAsync(fileName, content);
    }
}
