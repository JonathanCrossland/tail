using Tail.Tests;

public abstract class TestRunner 
{
    private static int _testCaseCounter = 0;
    internal static void AssertEqual(string expected, string actual, string message = "")
    {
        _testCaseCounter++; // Increment the counter each time AssertEqual is called

        if (expected == actual)
        {
            Console.WriteLine($"[OK] Test case {_testCaseCounter}: {message}");
        }
        else
        {
            Console.WriteLine($"[FAIL] Test case {_testCaseCounter}: {message}");
            Console.WriteLine($"\tExpected: '{expected}'");
            Console.WriteLine($"\tActual:   '{actual}'");
        }
    }

    public abstract void Run();
}

