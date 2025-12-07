using Hobble.Lang.Interface;
using NSubstitute;

namespace Hobble.Lang.IntegrationTests;

public abstract class BaseIntegrationTests
{
    protected static void ReporterActionTest(string source, Action<IReporter>? reporterAction = null)
    {
        var reporter = Substitute.For<IReporter>();
        var driver = new Driver(reporter);

        var success = driver.Run(source);
        
        Assert.True(success);
        reporterAction?.Invoke(reporter);
    }
}