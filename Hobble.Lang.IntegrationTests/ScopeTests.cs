using Hobble.Lang.Interface;
using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace Hobble.Lang.IntegrationTests;

public class ScopeTests
{
    [Fact]
    public void VariableSameNameAsEnclosingScope_CorrectlyOutputsAssociatedValues()
    {
        const string source = """
                              var a = "outer";

                              {
                                 var a = "inner";
                                 print a;            // expect: inner
                              }

                              print a;               // expect: outer
                              """;

        var reporter = Substitute.For<IReporter>();
        var driver = new Driver(reporter);

        var success = driver.Run(source);
        
        Assert.True(success);
        reporter.Received(1).Output(Arg.Is("outer"));
        reporter.Received(1).Output(Arg.Is("inner"));
    }
}