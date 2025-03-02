// Attribute should be "registered" by adding as module or assembly custom attribute
using System.Reflection;

using MethodDecorator.Fody.Interfaces;

[module: Interceptor]

// Any attribute which provides OnEntry/OnExit/OnException with proper args
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Assembly | AttributeTargets.Module)]
public class InterceptorAttribute : Attribute, IMethodDecorator
{
    // instance, method and args can be captured here and stored in attribute instance fields
    // for future usage in OnEntry/OnExit/OnException
    public void Init(object instance, MethodBase method, object[] args)
    {
        Console.Write(string.Format("Init: {0} [{1}]", method.DeclaringType.FullName + "." + method.Name, args.Length));
    }

    public void OnEntry()
    {
        Console.Write("OnEntry");
    }

    public void OnExit()
    {
        Console.Write("OnExit");
    }

    public void OnException(Exception exception)
    {
        Console.Write(string.Format("OnException: {0}: {1}", exception.GetType(), exception.Message));
    }
}
