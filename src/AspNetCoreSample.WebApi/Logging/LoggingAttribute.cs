using MethodDecorator.Fody.Interfaces;

using System.Collections;
using System.Reflection;
using System.Text.Json;

namespace AspNetCoreSample.WebApi.Logging;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
public class LoggingAttribute : Attribute, IMethodDecorator
{
    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        WriteIndented = false,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    private ILogger? _logger;
    private string? _methodName;
    private object[]? _args;
    private MethodBase? _method;
    private DateTime _startTime;

    /// <summary>
    /// 対象のメソッドの開始時のロギング
    /// </summary>
    public void Init(object instance, MethodBase method, object[] args)
    {
        _startTime = DateTime.UtcNow;
        _method = method;
        _methodName = $"{method.DeclaringType!.Name}.{method.Name}";
        _args = args;

        // ASP.NET Core のDIコンテナからILoggerを取得
        var serviceProvider = ServiceProviderAccessor.ServiceProvider;
        if (serviceProvider != null)
        {
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            if (instance != null)
            {
                _logger = loggerFactory?.CreateLogger(instance.GetType());
            }
            else
            {
                _logger = loggerFactory?.CreateLogger(method.DeclaringType);
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
                    .WithProperty("Arguments", _args ?? Array.Empty<object>()),
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
                    .WithProperty("Arguments", _args ?? Array.Empty<object>())
                    .WithProperty("ExceptionType", exception.GetType().Name)
                    .WithProperty("ErrorMessage", exception.Message)
                    .WithProperty("StackTrace", exception.StackTrace ?? string.Empty),
                exception,
                MyLogEvent.Formatter);
        }
    }

    public void OnEntry()
    {
        // Init で既に処理済み
    }

    public void OnExit()
    {
        // OnException が呼ばれた後に OnExit が呼ばれるため、何もしない
    }

    /// <summary>
    /// メソッド内でExceptionが発生した時のロギング
    /// </summary>
    public void OnException(Exception exception)
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
                .WithProperty("Arguments", _args ?? Array.Empty<object>())
                .WithProperty("ExecutionTime", executionTime)
                .WithProperty("ExceptionType", exception.GetType().Name)
                .WithProperty("ErrorMessage", exception.Message)
                .WithProperty("StackTrace", exception.StackTrace ?? string.Empty),
            exception,
            MyLogEvent.Formatter);
    }

    /// <summary>
    /// 対象のメソッドの終了時のロギング
    /// </summary>
    public void OnExit(object returnValue)
    {
        if (_logger == null) return;

        // Mvcの場合、OnExceptionが呼び出されないため、returnValueで例外を判定する
        var exceptionProperty = returnValue?.GetType().GetProperty("Exception");
        if (exceptionProperty != null)
        {
            var exception = exceptionProperty.GetValue(returnValue) as Exception;
            if (exception != null)
            {
                OnException(exception);
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
                    .WithProperty("Arguments", _args ?? Array.Empty<object>())
                    .WithProperty("ReturnValue", returnValue)
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
                    .WithProperty("Arguments", _args ?? Array.Empty<object>())
                    .WithProperty("ReturnValue", returnValue)
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
