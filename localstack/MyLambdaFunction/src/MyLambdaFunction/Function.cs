using Amazon.Lambda.Core;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace MyLambdaFunction;

public class MyRequest
{
    public string Input { get; set; }
}

public class Function
{

    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public string FunctionHandler(MyRequest request, ILambdaContext context)
    {
        try
        {
            return request.Input.ToUpper();
        }
        catch (Exception ex)
        {
            return "error";
        }
    }
}
