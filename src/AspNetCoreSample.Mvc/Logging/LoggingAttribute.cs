using MethodDecorator.Fody.Interfaces;

using System.Reflection;
using System.Text.Json;

namespace AspNetCoreSample.Mvc.Logging;

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
    private static readonly Action<ILogger, Dictionary<string, object>, Exception?> _logMethodEntry =
        LoggerMessage.Define<Dictionary<string, object>>(
            LogLevel.Warning,
            new EventId(1, nameof(LoggingAttribute)),
            "{@LogData}");

    /// <summary>
    /// 対象のメソッドの終了時のロギング
    /// </summary>
    private static readonly Action<ILogger, Dictionary<string, object>, Exception?> _logMethodExit =
        LoggerMessage.Define<Dictionary<string, object>>(
            LogLevel.Warning,
            new EventId(2, nameof(LoggingAttribute)),
            "{@LogData}");

    /// <summary>
    /// 対象のメソッドで例外が発生し、キャッチされなかった時のロギング
    /// </summary>
    private static readonly Action<ILogger, Dictionary<string, object>, Exception?> _logMethodException =
        LoggerMessage.Define<Dictionary<string, object>>(
            LogLevel.Critical,
            new EventId(3, nameof(LoggingAttribute)),
            "Critical Unhandled Exception for {@LogData}");

    /// <summary>
    /// メソッド開始時のロギングでExceptionが発生した時のロギング
    /// </summary>
    private static readonly Action<ILogger, string, Exception?> _logMethodEntryError =
        LoggerMessage.Define<string>(
            LogLevel.Critical,
            new EventId(4, nameof(LoggingAttribute)),
            "Critical logging method entry for {@MethodName}");

    /// <summary>
    /// メソッド終了時のロギングでExceptionが発生した時のロギング
    /// </summary>
    private static readonly Action<ILogger, string, Exception?> _logMethodExitError =
        LoggerMessage.Define<string>(
            LogLevel.Critical,
            new EventId(5, nameof(LoggingAttribute)),
            "Critical logging method exit for {@MethodName}");

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

        // メソッド開始時のログ
        if (_logger != null)
        {
            try
            {
                var parameters = method.GetParameters();
                var parameterInfo = new Dictionary<string, object>();

                for (int i = 0; i < parameters.Length && i < args.Length; i++)
                {
                    parameterInfo.Add(parameters[i].Name!, args[i]);
                }

                var serializedArgs = JsonSerializer.Serialize(parameterInfo, _jsonOptions);

                // 構造化されたログデータを作成
                var logData = new Dictionary<string, object>
                {
                    ["EventType"] = "MethodEntry",
                    ["MethodName"] = _methodName ?? "UnknownMethod",
                    ["Arguments"] = parameterInfo,
                    ["Timestamp"] = _startTime
                };

                // 構造化ログを出力（NLogのJsonLayoutがこれを処理）
                _logMethodEntry(_logger, logData, null);
            }
            catch (Exception ex)
            {
                _logMethodEntryError(_logger, _methodName, ex);
            }
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

    public void OnException(Exception exception)
    {
        if (_logger != null)
        {
            // 構造化された例外ログデータを作成
            var methodName = _methodName ?? "UnknownMethod";
            var logData = new Dictionary<string, object>
            {
                ["EventType"] = "MethodException",
                ["MethodName"] = methodName,
                ["ErrorMessage"] = exception.Message,
                ["StackTrace"] = exception.StackTrace ?? string.Empty,
                ["ExceptionType"] = exception.GetType().Name,
                ["Timestamp"] = DateTime.UtcNow,
                ["ExecutionTime"] = (DateTime.UtcNow - _startTime).TotalMilliseconds
            };
            _logMethodException(_logger, logData, exception);
        }
    }

    public void OnExit(object returnValue)
    {
        if (_logger != null)
        {
            try
            {
                var executionTime = (DateTime.UtcNow - _startTime).TotalMilliseconds;

                // 構造化された終了ログデータを作成
                var logData = new Dictionary<string, object>
                {
                    ["EventType"] = "MethodExit",
                    ["MethodName"] = _methodName ?? "UnknownMethod",
                    ["ReturnValue"] = returnValue,
                    ["Timestamp"] = DateTime.UtcNow,
                    ["ExecutionTime"] = executionTime
                };

                _logMethodExit(_logger, logData, null);
            }
            catch (Exception ex)
            {
                _logMethodExitError(_logger, _methodName ?? "UnknownMethod", ex);
            }
        }
    }
}
