using NLog;

namespace ErpCore.Shared.Logging;

/// <summary>
/// NLog 日誌服務實作
/// </summary>
public class LoggerService : ILoggerService
{
    private readonly Logger _logger;

    public LoggerService(string loggerName = "Default")
    {
        _logger = LogManager.GetLogger(loggerName);
    }

    public void LogTrace(string message)
    {
        _logger.Trace(message);
    }

    public void LogDebug(string message)
    {
        _logger.Debug(message);
    }

    public void LogInfo(string message)
    {
        _logger.Info(message);
    }

    public void LogWarning(string message)
    {
        _logger.Warn(message);
    }

    public void LogError(string message, Exception? exception = null)
    {
        if (exception != null)
        {
            _logger.Error(exception, message);
        }
        else
        {
            _logger.Error(message);
        }
    }

    public void LogFatal(string message, Exception? exception = null)
    {
        if (exception != null)
        {
            _logger.Fatal(exception, message);
        }
        else
        {
            _logger.Fatal(message);
        }
    }
}

