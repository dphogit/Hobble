using NSubstitute;

namespace Hobble.Lang.IntegrationTests;

public class LoopTests : BaseIntegrationTests
{
    [Fact]
    public void While_ExecutesCorrectNumberIterations()
    {
        const string source = """
                              var sum = 0;
                              var i = 0;
                              
                              while (i < 3) {
                                sum = sum + i;
                                i = i + 1; 
                              }
                              
                              print sum;    // expect: 3
                              """;

        ReporterActionTest(source, reporter => reporter.Received(1).Output("3"));
    }

    [Fact]
    public void While_AssignmentInCondition_ExecutesCorrectNumberIterations()
    {
        const string source = """
                              var sum = 0;
                              var i = 0;
                              
                              while ((i = i + 1) < 3)
                                sum = sum + i;
                              
                              print sum;    // expect: 3
                              """;
        
        ReporterActionTest(source, reporter => reporter.Received(1).Output("3"));
    }
}