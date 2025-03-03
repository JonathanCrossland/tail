public class Program
{
    private static int _testCaseCounter = 0; // Static counter to keep track of test cases

  
    public static void Main(string[] args)
    {
        Console.WriteLine("Tail---__---");

        if (args.Length > 0 && args[0] == "--test")
        {
            TestRunner.RunTests();
        }
        else if (args.Length >= 3 && args[0] == "--def" && args[1] == "--template")
        {
            string pattern = args[2];
            string replacement = args[3];
            string input = args[4];

            List<Definition> definitions = new List<Definition>
            {
                new Definition(pattern, replacement)
            };

            Tail tail = new Tail(definitions);
            tail.ParseDefinitions();

            string result = tail.Parse(input);
            Console.WriteLine($"Result: {result}");
        }
        else
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  --test : Run test cases");
            Console.WriteLine("  --def <pattern> --template <replacement> <input> : Process a single definition");
        }
    }
}