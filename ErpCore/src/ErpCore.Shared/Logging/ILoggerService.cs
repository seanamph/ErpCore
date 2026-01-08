namespace ErpCore.Shared.Logging;

/// <summary>
/// 日誌服務介面
/// </summary>
public interface ILoggerService
{
    /// <summary>
    /// 記錄追蹤訊息
    /// </summary>
    void LogTrace(string message);

    /// <summary>
    /// 記錄除錯訊息
    /// </summary>
    void LogDebug(string message);

    /// <summary>
    /// 記錄資訊訊息
    /// </summary>
    void LogInfo(string message);

    /// <summary>
    /// 記錄警告訊息
    /// </summary>
    void LogWarning(string message);

    /// <summary>
    /// 記錄錯誤訊息
    /// </summary>
    void LogError(string message, Exception? exception = null);

    /// <summary>
    /// 記錄嚴重錯誤訊息
    /// </summary>
    void LogFatal(string message, Exception? exception = null);
}

