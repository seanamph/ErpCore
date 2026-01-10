namespace ErpCore.Infrastructure.Services.Notification;

/// <summary>
/// 通知服務介面
/// 提供簡訊、推播通知等功能
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// 發送簡訊
    /// </summary>
    /// <param name="phoneNumber">手機號碼</param>
    /// <param name="message">訊息內容</param>
    /// <returns>是否發送成功</returns>
    Task<bool> SendSmsAsync(string phoneNumber, string message);

    /// <summary>
    /// 發送推播通知
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <param name="title">通知標題</param>
    /// <param name="body">通知內容</param>
    /// <returns>是否發送成功</returns>
    Task<bool> SendPushNotificationAsync(string userId, string title, string body);

    /// <summary>
    /// 批次發送簡訊
    /// </summary>
    /// <param name="phoneNumbers">手機號碼列表</param>
    /// <param name="message">訊息內容</param>
    /// <returns>發送結果列表</returns>
    Task<Dictionary<string, bool>> SendBulkSmsAsync(List<string> phoneNumbers, string message);

    /// <summary>
    /// 批次發送推播通知
    /// </summary>
    /// <param name="userIds">使用者ID列表</param>
    /// <param name="title">通知標題</param>
    /// <param name="body">通知內容</param>
    /// <returns>發送結果列表</returns>
    Task<Dictionary<string, bool>> SendBulkPushNotificationAsync(List<string> userIds, string title, string body);
}

