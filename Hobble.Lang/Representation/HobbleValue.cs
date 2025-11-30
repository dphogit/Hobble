namespace Hobble.Lang.Representation;

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

    public bool IsNumber() => _type == HobbleValueType.Number;
    public bool IsString() => _type == HobbleValueType.String;

    public decimal AsNumber() => IsNumber()
        ? Convert.ToDecimal(_value)
        : throw new InvalidOperationException("Value is not of type Number.");

    public string AsString() => IsString()
        ? Convert.ToString(_value)!
        : throw new InvalidOperationException("Value is not of type String.");

    public override string ToString()
    {
        switch (_type)
        {
            case HobbleValueType.Number:
            case HobbleValueType.String:
                return _value!.ToString()!;
            default:
                throw new InvalidOperationException($"Cannot convert '{_type.ToString()}' to string.");
        }
    }
}