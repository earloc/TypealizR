using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using TypealizR.CLI.Abstractions;

namespace TypealizR.Tests.CLI.Tests.Abstractions;
public class FileStorage_Tests
{

    [Fact]
    public async Task CanAdd_File_With_Content()
    {
        var sut = new FileStorage();

        var fileName = $"{Guid.NewGuid()}.txt";
        File.Exists(fileName).Should().BeFalse();

        var expected = Guid.NewGuid().ToString();
        await sut.AddAsync(fileName, expected);

        File.Exists(fileName).Should().BeTrue();

        var actual = File.ReadAllText(fileName);
        actual.Should().Be(expected);
    }

}
