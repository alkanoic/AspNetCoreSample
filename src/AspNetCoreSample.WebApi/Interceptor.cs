// Attribute should be "registered" by adding as module or assembly custom attribute
using System.Reflection;
using System.Xml;

using AspNetCoreSample.WebApi;

using MethodDecorator.Fody.Interfaces;

[module: Interceptor]

namespace AspNetCoreSample.WebApi;

// Any attribute which provides OnEntry/OnExit/OnException with proper args
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Assembly | AttributeTargets.Module)]
public class InterceptorAttribute : Attribute, IMethodDecorator
{
    // instance, method and args can be captured here and stored in attribute instance fields
    // for future usage in OnEntry/OnExit/OnException
    public void Init(object instance, MethodBase method, object[] args)
    {
        var declaringType = method.DeclaringType?.FullName ?? "UnknownType";
        var methodName = method.Name ?? "UnknownMethod";
        var argsJson = System.Text.Json.JsonSerializer.Serialize(args);
        Console.WriteLine($"Init: {declaringType}.{methodName} {argsJson}");
    }

    public void OnEntry()
    {
        Console.WriteLine("OnEntry");
    }

    public void OnExit()
    {
        Console.WriteLine("OnExit");
    }

    public void OnException(Exception exception)
    {
        Console.WriteLine($"OnException: {exception.GetType()}: {exception.Message}");
    }
}
