namespace TypealizR.CLI.Abstractions;
internal interface IStorage
{
    public Task AddAsync(string fileName, string content);
}
