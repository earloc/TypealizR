using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypealizR.CLI.Abstractions;
internal class FileStorage : IStorage
{
    public async Task AddAsync(string fileName, string content)
    {
        await File.WriteAllTextAsync(fileName, content);
    }
}
