using Tail;
using Tail.Tests;

public class Program
{
  
    public static void Main(string[] args)
    {
        Console.WriteLine("Tail---__---");

        if (args.Length > 0 && args[0].StartsWith("--test"))
        {
            var split = args[0].Split("=");

            switch (split[1].ToLower())
            {
                case "general":
                    new GeneralTests().Run();
                    break;
                case "tail":
                    new TailTests().Run();
                    break;
                case "markdown":
                    new MarkdownTests().Run();
                    break;
                default:
                    break;
            }
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

            TailLang tail = new TailLang(definitions);
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