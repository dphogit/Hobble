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
    public void Get_DefinedInParentEnvironment_ReturnsStoredValue()
    {
        var parentEnvironment = new VariableEnvironment();
        parentEnvironment.Define("age", HobbleValue.Number(67));
        
        var environment = new VariableEnvironment(parentEnvironment);
        
        var age = environment.Get("age");
        
        Assert.Equal(HobbleValue.Number(67), age);
    }

    [Fact]
    public void Get_VariableShadowsParent_ReturnsShadowedValue()
    {
        var parentEnvironment = new VariableEnvironment();
        parentEnvironment.Define("age", HobbleValue.Number(67));
        
        var environment = new VariableEnvironment(parentEnvironment);
        environment.Define("age", HobbleValue.Number(69));
        
        var age = environment.Get("age");
        
        Assert.Equal(HobbleValue.Number(69), age);
    }

    [Fact]
    public void Get_UndefinedVariable_ThrowsUndefinedVariableError()
    {
        var environment = new VariableEnvironment();
        
        Assert.Throws<UndefinedVariableError>(() => environment.Get("age"));
    }

    [Fact]
    public void Get_UndefinedVariableWithParent_ThrowsUndefinedVariableError()
    {
        var parentEnvironment = new VariableEnvironment();
        var environment = new VariableEnvironment(parentEnvironment);
        
        Assert.Throws<UndefinedVariableError>(() => environment.Get("age"));
    }

    [Fact]
    public void Assign_ExistingVariable_UpdatesVariableValue()
    {
        const string identifier = "age";
        var environment = new VariableEnvironment();
        environment.Define(identifier, HobbleValue.Number(67));
        
        environment.Assign(identifier, HobbleValue.Number(69));
        
        var age = environment.Get(identifier);
        Assert.Equal(HobbleValue.Number(69), age);
    }

    [Fact]
    public void Assign_VariableInParent_UpdatesVariableValue()
    {
        var parentEnvironment = new VariableEnvironment();
        parentEnvironment.Define("age", HobbleValue.Number(67));
        
        var environment = new VariableEnvironment(parentEnvironment);
        
        environment.Assign("age", HobbleValue.Number(69));
        
        var age = parentEnvironment.Get("age");
        Assert.Equal(HobbleValue.Number(69), age);
    }

    [Fact]
    public void Assign_UndefinedVariable_ThrowsUndefinedVariableError()
    {
        var environment = new VariableEnvironment();
        
        Assert.Throws<UndefinedVariableError>(() => environment.Assign("age", HobbleValue.Number(67)));
    }
    
    [Fact]
    public void Assign_UndefinedVariableWithParent_ThrowsUndefinedVariableError()
    {
        var parentEnvironment = new VariableEnvironment();
        var environment = new VariableEnvironment(parentEnvironment);
        
        Assert.Throws<UndefinedVariableError>(() => environment.Assign("age", HobbleValue.Number(67)));
    }
}