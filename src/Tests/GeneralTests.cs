using Tail;

namespace Tail.Tests;

internal class GeneralTests : TestRunner
{
    public GeneralTests()
    {
    }
    public override void Run()
    {

        List<Definition> definitions = new List<Definition>()
        {
            new Definition("[t]", "the"),
            new Definition("[ t]", " the"),
            new Definition("[ c]", " cat"),
            new Definition("[ i]", " in"),
            new Definition("[ h]", " hat"),
            //new Definition("[.]", "."),
            new Definition("[ .]", "."),


        };

        TailLang tail = new TailLang(definitions);
        tail.ParseDefinitions();

        TestRunner.AssertEqual("the cat in the hat.", tail.Parse("t c i t h ."));
     

    }
}

