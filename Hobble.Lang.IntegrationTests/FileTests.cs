using Hobble.Lang.Interface;
using NSubstitute;

namespace Hobble.Lang.IntegrationTests;

public class FileTests
{
    [Fact]
    public void RunFile()
    {
        var reporter = Substitute.For<IReporter>();
        var driver = new Driver(reporter);

        var success = driver.RunFile("./TestPrograms/fib.hob");
        
        Assert.True(success);
        reporter.Received(1).Output("55");
    }

    [Fact]
    public void RunFile_NotExists_ReportsError()
    {
        const string path = "./TestPrograms/hello.hob";
        var reporter = Substitute.For<IReporter>();
        var driver = new Driver(reporter);

        var success = driver.RunFile(path);
        
        Assert.False(success);
        reporter.Received(1).Error($"File '{path}' not found.");
    }

    [Fact]
    public void RunFile_NoHobExtension_ReportsError()
    {
        var reporter = Substitute.For<IReporter>();
        var driver = new Driver(reporter);
        
        var success = driver.RunFile("./TestPrograms/fib.js");
        
        Assert.False(success);
        reporter.Received(1).Error("Not a .hob file.");
    }
}