using MethodBoundaryAspect.Fody.Attributes;

using System.Collections;
using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreSample.Util.Logging;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
public sealed class LoggingAttribute : OnMethodBoundaryAspect
{
    private ILogger? _logger;
    private string? _methodName;
    private MethodExecutionArgs? _args;
    private DateTime _startTime;

    /// <summary>
    /// メソッド開始時の引数ログを取得するかどうか
    /// </summary>
    public bool LogOnStartArgs { get; set; } = true;

    /// <summary>
    /// メソッド終了時の返却値ログを取得するかどうか
    /// </summary>
    public bool LogOnEndArgs { get; set; } = true;

    /// <summary>
    /// 対象のメソッドの開始時のロギング
    /// </summary>
    public override void OnEntry(MethodExecutionArgs arg)
    {
        _startTime = DateTime.UtcNow;
        _methodName = $"{arg.Method.DeclaringType!.Name}.{arg.Method.Name}";
        _args = arg;

        // ASP.NET Core のDIコンテナからILoggerを取得
        var serviceProvider = ServiceProviderAccessor.ServiceProvider;
        if (serviceProvider != null)
        {
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            if (arg.Instance != null)
            {
                _logger = loggerFactory?.CreateLogger(arg.Instance.GetType());
            }
            else
            {
                _logger = loggerFactory?.CreateLogger(arg.Method.DeclaringType);
            }
        }

        if (_logger == null) return;

        // メソッド開始時のログ
        try
        {
            _logger.Log(LogLevel.Warning,
                new EventId(1, nameof(LoggingAttribute)),
                new MyLogEvent($"Method entry warning")
                    .WithProperty("EventType", "MethodEntry")
                    .WithProperty("MethodName", _methodName ?? "UnknownMethod")
                    .WithProperty("Arguments", LogOnStartArgs ? _args?.Arguments ?? Array.Empty<object>() : "Off"),
                    null,
                MyLogEvent.Formatter);
        }
        catch (Exception exception)
        {
            _logger.Log(LogLevel.Critical,
                new EventId(4, nameof(LoggingAttribute)),
                new MyLogEvent($"Critical logging method entry")
                    .WithProperty("EventType", "MethodExit")
                    .WithProperty("MethodName", _methodName ?? "UnknownMethod")
                    .WithProperty("Arguments", _args?.Arguments ?? Array.Empty<object>())
                    .WithProperty("ExceptionType", exception.GetType().Name)
                    .WithProperty("ErrorMessage", exception.Message)
                    .WithProperty("StackTrace", exception.StackTrace ?? string.Empty),
                exception,
                MyLogEvent.Formatter);
        }
    }

    /// <summary>
    /// メソッド内でExceptionが発生した時のロギング
    /// </summary>
    public override void OnException(MethodExecutionArgs arg)
    {
        if (_logger == null) return;

        // 構造化された例外ログデータを作成
        var methodName = _methodName ?? "UnknownMethod";
        var executionTime = (DateTime.UtcNow - _startTime).TotalMilliseconds;
        _logger.Log(LogLevel.Critical,
            new EventId(3, nameof(LoggingAttribute)),
            new MyLogEvent($"Critical Unhandled Exception")
                .WithProperty("EventType", "MethodException")
                .WithProperty("MethodName", methodName)
                .WithProperty("Arguments", _args?.Arguments ?? Array.Empty<object>())
                .WithProperty("ExecutionTime", executionTime)
                .WithProperty("ExceptionType", arg.Exception.GetType().Name)
                .WithProperty("ErrorMessage", arg.Exception.Message)
                .WithProperty("StackTrace", arg.Exception.StackTrace ?? string.Empty),
            arg.Exception,
            MyLogEvent.Formatter);
    }

    /// <summary>
    /// 対象のメソッドの終了時のロギング
    /// </summary>
    ///
    public override void OnExit(MethodExecutionArgs arg)
    {
        if (_logger == null) return;

        // Mvcの場合、OnExceptionが呼び出されないため、returnValueで例外を判定する
        var exceptionProperty = arg.ReturnValue?.GetType().GetProperty("Exception");
        if (exceptionProperty != null)
        {
            var exception = exceptionProperty.GetValue(arg.ReturnValue) as Exception;
            if (exception != null)
            {
                OnException(arg);
                return;
            }
        }

        try
        {
            var executionTime = (DateTime.UtcNow - _startTime).TotalMilliseconds;
            _logger.Log(LogLevel.Warning,
                new EventId(2, nameof(LoggingAttribute)),
                new MyLogEvent($"Method exit warning")
                    .WithProperty("EventType", "MethodExit")
                    .WithProperty("MethodName", _methodName ?? "UnknownMethod")
                    .WithProperty("Arguments", LogOnStartArgs ? _args?.Arguments ?? Array.Empty<object>() : "Off")
                    .WithProperty("ReturnValue", LogOnEndArgs ? arg.ReturnValue ?? Array.Empty<object>() : "Off")
                    .WithProperty("ExecutionTime", executionTime),
                null,
                MyLogEvent.Formatter);
        }
        catch (Exception exception)
        {
            var executionTime = (DateTime.UtcNow - _startTime).TotalMilliseconds;
            _logger.Log(LogLevel.Warning,
                new EventId(2, nameof(LoggingAttribute)),
                new MyLogEvent($"Critical logging method exit")
                    .WithProperty("EventType", "MethodExit")
                    .WithProperty("MethodName", _methodName ?? "UnknownMethod")
                    .WithProperty("Arguments", _args?.Arguments ?? Array.Empty<object>())
                    .WithProperty("ReturnValue", arg.ReturnValue ?? Array.Empty<object>())
                    .WithProperty("ExecutionTime", executionTime)
                    .WithProperty("ExceptionType", exception.GetType().Name)
                    .WithProperty("ErrorMessage", exception.Message)
                    .WithProperty("StackTrace", exception.StackTrace ?? string.Empty),
                exception,
                MyLogEvent.Formatter);
        }
    }
}

public class MyLogEvent : IEnumerable<KeyValuePair<string, object>>
{
    readonly List<KeyValuePair<string, object>> _properties = new List<KeyValuePair<string, object>>();

    public string Message { get; }

    public MyLogEvent(string message)
    {
        Message = message;
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        return _properties.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

    public MyLogEvent WithProperty(string name, object value)
    {
        _properties.Add(new KeyValuePair<string, object>(name, value));
        return this;
    }

    public static Func<MyLogEvent, Exception?, string> Formatter { get; } = (l, e) => l.Message;
}
