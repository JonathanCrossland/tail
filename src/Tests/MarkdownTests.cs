using Tail;

namespace Tail.Tests;

public class MarkdownTests : TestRunner
{
    public MarkdownTests()
    {
    }

    public override void Run()
    {
        List<Definition> definitions = new List<Definition>()
            {
                new Definition("![---]", "<hr>"),
                new Definition("![***]", "<hr>"),
                new Definition("![___]", "<hr>"),
                new Definition("![----]", "<hr>"),
                new Definition("![****]", "<hr>"),
                new Definition("![____]", "<hr>"),
                new Definition("[**](**)", "<b>$1</b>"),
                new Definition("[*](*)", "<i>$1</i>"),
                new Definition("[__](__)", "<u>$1</u>"),
                new Definition("[~~](~~)", "<strike>$1</strike>"),
                new Definition("[```](```)", "<pre><code>$1</code></pre>"),
                new Definition("[`](`)", "<code>$1</code>"),
                new Definition("[###### ]", "<h6>$1</h6>"),
                new Definition("[##### ]", "<h5>$1</h5>"),
                new Definition("[#### ]", "<h4>$1</h4>"),
                new Definition("[### ]", "<h3>$1</h3>"),
                new Definition("[## ]", "<h2>$1</h2>"),
                new Definition("[# ]", "<h1>$1</h1>"),
                new Definition("[> ]", "<blockquote>$1</blockquote>"),
                new Definition("[\\!\\[](\\])[\\(](\\))", "<img src=\"$1\" alt=\"$2\">"),
                new Definition("[\\[](\\])[\\(](\\))", "<a href=\"$1\">$2</a>"),
                new Definition("[\\!\\[](\\])", "<img src=\"$1\" alt=\"$1\">"),
                new Definition("[\\[](\\])", "<a href=\"$1\">$1</a>"),
                new Definition("![* ]", "<ul><li>$1</li></ul>"),
                new Definition("![- ]", "<ul><li>$1</li></ul>"),
                new Definition("![+ ]", "<ul><li>$1</li></ul>"),
                new Definition("![1. ]", "<ol><li>$1</li></ol>"),
                new Definition("![2. ]", "<ol><li>$1</li></ol>"),
                new Definition("![3. ]", "<ol><li>$1</li></ol>"),
                new Definition("![4. ]", "<ol><li>$1</li></ol>"),
                new Definition("![5. ]", "<ol><li>$1</li></ol>"),
                new Definition("![6. ]", "<ol><li>$1</li></ol>"),
                new Definition("![7. ]", "<ol><li>$1</li></ol>"),
                new Definition("![8. ]", "<ol><li>$1</li></ol>"),
                new Definition("![9. ]", "<ol><li>$1</li></ol>"),
                new Definition("{|}", "<table>$1</table>"),
                new Definition("[\\n|]", "<tr>$1</tr>"),
                new Definition("[|]", "<td>$1</td>")
            };

        TailLang tail = new TailLang(definitions);
        tail.ParseDefinitions();


        TestRunner.AssertEqual("<b>Hello</b>", tail.Parse("**Hello**"));
        TestRunner.AssertEqual("<i>Hello</i>", tail.Parse("*Hello*"));
        TestRunner.AssertEqual("<u>Hello</u>", tail.Parse("__Hello__"));
        TestRunner.AssertEqual("<strike>Hello</strike>", tail.Parse("~~Hello~~"));
        TestRunner.AssertEqual("<code>Hello</code>", tail.Parse("`Hello`"));
        TestRunner.AssertEqual("<pre><code>Hello</code></pre>", tail.Parse("```Hello```"));
        TestRunner.AssertEqual("<h1>Hello</h1>", tail.Parse("# Hello"));
        TestRunner.AssertEqual("<h2>Hello</h2>", tail.Parse("## Hello"));
        TestRunner.AssertEqual("<h3>Hello</h3>", tail.Parse("### Hello"));
        TestRunner.AssertEqual("<h4>Hello</h4>", tail.Parse("#### Hello"));
        TestRunner.AssertEqual("<h5>Hello</h5>", tail.Parse("##### Hello"));
        TestRunner.AssertEqual("<h6>Hello</h6>", tail.Parse("###### Hello"));
        TestRunner.AssertEqual("<blockquote>Hello</blockquote>", tail.Parse("> Hello"));
        TestRunner.AssertEqual("<img src=\"image.jpg\" alt=\"image.jpg\">", tail.Parse("![image.jpg]"));
        TestRunner.AssertEqual("<a href=\"link.com\">link.com</a>", tail.Parse("[link.com]"));
        TestRunner.AssertEqual("<hr>", tail.Parse("---"));
        TestRunner.AssertEqual("<hr>", tail.Parse("***"));
        TestRunner.AssertEqual("<hr>", tail.Parse("___"));
        TestRunner.AssertEqual("<h1><b>Hello</b></h1>", tail.Parse("# **Hello**"));
        TestRunner.AssertEqual("Plain text", tail.Parse("Plain text"));
        TestRunner.AssertEqual("", tail.Parse(""));
        TestRunner.AssertEqual("<img src=\"image.png\" alt=\"Alt Text\">", tail.Parse("![image.png](Alt Text)"));
        TestRunner.AssertEqual("<a href=\"link.net\">Link Text</a>", tail.Parse("[link.net](Link Text)"));
        TestRunner.AssertEqual("<hr>", tail.Parse("----"));
        TestRunner.AssertEqual("<hr>", tail.Parse("****"));
        TestRunner.AssertEqual("<hr>", tail.Parse("____"));
        TestRunner.AssertEqual("<ul><li>Item 1</li></ul>", tail.Parse("* Item 1"));
        TestRunner.AssertEqual("<ul><li>Item 2</li></ul>", tail.Parse("- Item 2"));
        TestRunner.AssertEqual("<ul><li>Item 3</li></ul>", tail.Parse("+ Item 3"));
        TestRunner.AssertEqual("<ol><li>Item 1</li></ol>", tail.Parse("1. Item 1"));
        TestRunner.AssertEqual("<ol><li>Item 2</li></ol>", tail.Parse("2. Item 2"));
        TestRunner.AssertEqual("<ol><li>Item 3</li></ol>", tail.Parse("3. Item 3"));
        
        TestRunner.AssertEqual("<h1><i>Hello</i></h1>", tail.Parse("# *Hello*"));
        TestRunner.AssertEqual("<h2><code>Code</code></h2>", tail.Parse("## `Code`"));
        TestRunner.AssertEqual("<h3><b><u><strike>Strike</strike></u></b></h3>", tail.Parse("### **__~~Strike~~__**"));
        //TestRunner.AssertEqual("<table><tr><td>Cell 1</td></tr></table>", tail.Parse("| Cell 1 |"));
        //TestRunner.AssertEqual("<td>cell 1 | cell 2</td>", tail.Parse("| cell 1 | cell 2 |"));
    }
}

