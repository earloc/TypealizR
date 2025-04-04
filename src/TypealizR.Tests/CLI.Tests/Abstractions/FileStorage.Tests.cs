using TypealizR.CLI.Abstractions;

namespace TypealizR.Tests.CLI.Tests.Abstractions;
internal class FileStorage_Tests
{

    [Fact]
    public async Task CanAdd_File_With_Content()
    {
        var sut = new FileStorage();

        var fileName = $"{Guid.NewGuid()}.txt";
        File.Exists(fileName).ShouldBeFalse();

        var expected = Guid.NewGuid().ToString();
        await sut.AddAsync(fileName, expected);

        File.Exists(fileName).ShouldBeTrue();

        var actual = await File.ReadAllTextAsync(fileName);
        actual.ShouldBe(expected);
    }

}
