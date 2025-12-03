using Hobble.Lang.Interpreter;
using Hobble.Lang.Representation;

namespace Hobble.Lang.UnitTests.Interpreter;

public class VariableEnvironmentTests
{
    [Fact]
    public void Define_NewVariable_StoresValue()
    {
        var environment = new VariableEnvironment();
        
        environment.Define("age", HobbleValue.Number(67));

        var age = environment.Get("age");
        Assert.Equal(age, HobbleValue.Number(67));
    }

    [Fact]
    public void Define_ExistingDefinedVariable_ThrowsVariableAlreadyDefinedError()
    {
        var environment = new VariableEnvironment();
        environment.Define("age", HobbleValue.Number(67));
        
        Assert.Throws<VariableAlreadyDefinedError>(() => environment.Define("age", HobbleValue.Number(69)));
    }

    [Fact]
    public void Get_ExistingDefinedVariable_ReturnsStoredValue()
    {
        var environment = new VariableEnvironment();
        environment.Define("age", HobbleValue.Number(67));
        
        var age = environment.Get("age");
        
        Assert.Equal(HobbleValue.Number(67), age);
    }

    [Fact]
    public void Get_UndefinedVariable_ThrowsUndefinedVariableError()
    {
        var environment = new VariableEnvironment();
        
        Assert.Throws<UndefinedVariableError>(() => environment.Get("age"));
    }
}