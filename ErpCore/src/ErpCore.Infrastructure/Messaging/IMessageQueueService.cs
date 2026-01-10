namespace ErpCore.Infrastructure.Messaging;

/// <summary>
/// 訊息佇列服務介面
/// 提供訊息發送、接收等功能
/// </summary>
public interface IMessageQueueService
{
    /// <summary>
    /// 發送訊息到佇列
    /// </summary>
    /// <typeparam name="T">訊息類型</typeparam>
    /// <param name="queueName">佇列名稱</param>
    /// <param name="message">訊息內容</param>
    /// <returns>是否發送成功</returns>
    Task<bool> SendAsync<T>(string queueName, T message) where T : class;

    /// <summary>
    /// 接收訊息
    /// </summary>
    /// <typeparam name="T">訊息類型</typeparam>
    /// <param name="queueName">佇列名稱</param>
    /// <param name="handler">訊息處理器</param>
    /// <returns>是否接收成功</returns>
    Task<bool> ReceiveAsync<T>(string queueName, Func<T, Task> handler) where T : class;

    /// <summary>
    /// 訂閱訊息
    /// </summary>
    /// <typeparam name="T">訊息類型</typeparam>
    /// <param name="queueName">佇列名稱</param>
    /// <param name="handler">訊息處理器</param>
    /// <returns>是否訂閱成功</returns>
    Task<bool> SubscribeAsync<T>(string queueName, Func<T, Task> handler) where T : class;

    /// <summary>
    /// 取消訂閱
    /// </summary>
    /// <param name="queueName">佇列名稱</param>
    /// <returns>是否取消成功</returns>
    Task<bool> UnsubscribeAsync(string queueName);
}

