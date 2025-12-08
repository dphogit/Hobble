namespace Hobble.Lang.Interpreter;

public record HobbleValue
{
    private readonly object? _value;
    private readonly HobbleValueType _type;

    private HobbleValue(object? value, HobbleValueType type)
    {
        _value = value;
        _type = type;
    }

    public static HobbleValue Number(decimal value) => new(value, HobbleValueType.Number);
    public static HobbleValue String(string value) => new(value, HobbleValueType.String);
    public static HobbleValue Bool(bool value) => new(value, HobbleValueType.Bool);
    public static HobbleValue Function(HobbleFunction value) => new(value, HobbleValueType.Function);
    public static HobbleValue Null() => new(null, HobbleValueType.Null);

    public bool IsNumber() => _type == HobbleValueType.Number;
    public bool IsString() => _type == HobbleValueType.String;
    public bool IsBool() => _type == HobbleValueType.Bool;
    public bool IsFunction() => _type == HobbleValueType.Function;
    public bool IsNull() => _type == HobbleValueType.Null;

    public decimal AsNumber() => IsNumber()
        ? Convert.ToDecimal(_value)
        : throw new InvalidOperationException("Value is not of type Number.");

    public string AsString() => IsString()
        ? Convert.ToString(_value)!
        : throw new InvalidOperationException("Value is not of type String.");

    public bool AsBool() => IsBool()
        ? (bool)_value!
        : throw new InvalidOperationException("Value is not of type Bool.");
    
    public HobbleFunction AsFunction() => IsFunction()
        ? (HobbleFunction)_value!
        : throw new InvalidOperationException("Value is not of type Function.");

    public override string ToString()
    {
        return _type switch
        {
            HobbleValueType.Number or HobbleValueType.String => _value!.ToString()!,
            HobbleValueType.Bool => (bool)_value! ? "true" : "false",
            HobbleValueType.Function => AsFunction().ToString()!,
            HobbleValueType.Null => "null",
            _ => throw new InvalidOperationException($"Cannot convert '{_type.ToString()}' to string.")
        };
    }
}