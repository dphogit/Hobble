using NSubstitute;

namespace Hobble.Lang.IntegrationTests;

public class LoopTests : BaseIntegrationTests
{
    [Fact]
    public void While()
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

        ReporterTest(source, reporter => reporter.Received(1).Output("3"));
    }

    [Fact]
    public void While_AssignmentInCondition()
    {
        const string source = """
                              var sum = 0;
                              var i = 0;
                              
                              while ((i = i + 1) < 3)
                                sum = sum + i;
                              
                              print sum;    // expect: 3
                              """;
        
        ReporterTest(source, reporter => reporter.Received(1).Output("3"));
    }

    [Fact]
    public void For()
    {
        const string source = """
                              var sum = 0;
                              
                              for (var i = 0; i < 3; i = i + 1) {
                                sum = sum + i;
                              }
                              
                              print sum;    // expect: 3
                              """;
        
        ReporterTest(source, reporter => reporter.Received(1).Output("3"));
    }

    [Fact]
    public void For_NoInitializer()
    {
        const string source = """
                              var i = 0;
                              var sum = 0;
                              
                              for (; i < 3; i = i + 1) {
                                sum = sum + i;
                              }
                              
                              print sum;    // expect: 3
                              """;
        
        ReporterTest(source, reporter => reporter.Received(1).Output("3"));
    }

    [Fact]
    public void For_NoIncrement()
    {
        const string source = """
                              var sum = 0;

                              for (var i = 0; i < 3; ) {
                                sum = sum + i;
                                i = i + 1;
                              }
                              
                              print sum;    // expect: 3
                              """;
        
        ReporterTest(source, reporter => reporter.Received(1).Output("3"));
    }
}