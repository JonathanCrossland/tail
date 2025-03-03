using Tail;

namespace Tail.Tests;

internal class TailTests : TestRunner
{
    public TailTests()
    {
    }
    public override void Run()
    {

        List<Definition> definitions = new List<Definition>()
        {
            new Definition("[X]", "$1"), // Self matching
            new Definition("[Y](Z)", "$1Z"),// Circular dependency
        };

        TailLang tail = new TailLang(definitions);
        tail.ParseDefinitions();

        TestRunner.AssertEqual("", tail.Parse("X X X"));
        TestRunner.AssertEqual("Z", tail.Parse("YZ"));
        

    }
}

