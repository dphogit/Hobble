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
}