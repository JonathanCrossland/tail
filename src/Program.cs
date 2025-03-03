public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Tail---__---");   
    }

    static void AssertEqual(string expected, string actual, string message)
    {
        if (expected == actual)
        {
            Console.WriteLine($"✅ Assertion passed: {message}");
        }
        else
        {
            Console.WriteLine($"❌ Assertion failed: {message}");
            Console.WriteLine($"   Expected: '{expected}'");
            Console.WriteLine($"   Actual:   '{actual}'");
        }
    }
}