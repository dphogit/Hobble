using NSubstitute;

namespace Hobble.Lang.IntegrationTests;

public class FunctionTests : BaseIntegrationTests
{
    [Fact]
    public void Call()
    {
        const string source = """
                              fn printSum(a, b) {
                                print a + b;
                              }
                              
                              printSum(1, 2);   // expect: 3
                              """;
        
        ReporterTest(source, reporter => reporter.Received(1).Output("3"));
    }

    [Fact]
    public void NoReturnStatement_ReturnsImplicitNull()
    {
        const string source = """
                              fn a() {}
                              
                              print a();    // expect: null
                              """;
        
        ReporterTest(source, reporter => reporter.Received(1).Output("null"));
    }

    [Fact]
    public void ReturnStatement_ReturnsValue()
    {
        const string source = """
                              fn sum(a, b) {
                                return a + b;
                              }
                              
                              print sum(1, 2);   // expect: 3
                              """;
        
        ReporterTest(source, reporter => reporter.Received(1).Output("3"));
    }

    [Fact]
    public void ArgumentToPrint_FormatsCorrectly()
    {
        const string source = """
                              fn hello() {}
                              
                              print hello;  // expect: "<fn hello>"
                              """;
        
        ReporterTest(source, reporter => reporter.Received(1).Output("<fn hello>"));
    }

    [Fact]
    public void EarlyReturn()
    {
        const string source = """
                              fn a() {
                                if (true) {
                                  print "pass";
                                  return;
                                }
                                
                                print "fail";
                              }
                              
                              a();  // expect: "pass"
                              """;
        
        ReporterTest(source, reporter =>
        {
            reporter.Received(1).Output("pass");
            reporter.DidNotReceive().Output("fail");
        });
    }

    [Fact]
    public void Recursion()
    {
        const string source = """
                              fn fib(n) {
                                if (n <= 1) return n;
                                return fib(n - 1) + fib(n - 2);
                              }
                              
                              print fib(10);    // expect: 55
                              """;
        
        ReporterTest(source, reporter => reporter.Received(1).Output("55"));
    }
}