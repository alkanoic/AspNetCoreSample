using System;
using System.Reflection;

using MethodDecorator.Fody.Interfaces;

// using Microsoft.Extensions.Logging;

[module: LogMethod] // Attribute should be "registered" by adding as module or assembly custom attribute

// namespace AspNetCoreSample.Util;

// Any attribute which provides OnEntry/OnExit/OnException with proper args
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Assembly | AttributeTargets.Module)]
public class LogMethodAttribute : Attribute, IMethodDecorator
{
    // static ILogger<T> _logger => new LoggerFactory().CreateLogger<T>();

    private MethodBase? _method;
    // instance, method and args can be captured here and stored in attribute instance fields
    // for future usage in OnEntry/OnExit/OnException
    public void Init(object instance, MethodBase method, object[] args)
    {
        _method = method;
    }
    public void OnEntry()
    {
        Console.Write("aaaa");
        // _logger.LogTrace("Entering into {0}", _method.Name);
    }
    public void OnExit()
    {
        // _logger.LogTrace("Exiting into {0}", _method.Name);
    }
    public void OnException(Exception exception)
    {
        // _logger.LogTrace(exception, "Exception {0}", _method.Name);
    }
}
