namespace Hobble.Lang.Interpreter;

public class ReturnValue : Exception
{
   private ReturnValue(HobbleValue value)
   {
      Value = value;
   }
   
   public HobbleValue Value { get; init; }

   public static void Throw(HobbleValue value) => throw new ReturnValue(value);
}