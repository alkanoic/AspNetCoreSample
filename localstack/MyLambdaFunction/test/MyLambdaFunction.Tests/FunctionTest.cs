using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

namespace MyLambdaFunction.Tests;

public class FunctionTest
{
    [Test]
    public async Task TestToUpperFunction()
    {

        // Invoke the lambda function and confirm the string was upper cased.
        var function = new Function();
        var context = new TestLambdaContext();
        var upperCase = function.FunctionHandler(new MyRequest { Input = "hello world" }, context);

        await Assert.That(upperCase).IsEqualTo("HELLO WORLD");
    }
}
