using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypealizR.CLI.Abstractions;
internal class FileStorage : IStorage
{
    public Task AddAsync(string fileName, string content)
    {
        throw new NotImplementedException();
    }
}
