using ErpCore.Shared.Logging;
using System.Net.Http.Json;

namespace ErpCore.Infrastructure.Services.Notification;

/// <summary>
/// 推播通知服務實作
/// 提供推播通知發送功能
/// </summary>
public class PushNotificationService : INotificationService
{
    private readonly ILoggerService _logger;
    private readonly string _apiUrl;
    private readonly string _apiKey;

    public PushNotificationService(ILoggerService logger)
    {
        _logger = logger;
        // 從設定檔讀取 API 設定（可在 appsettings.json 中設定）
        _apiUrl = Environment.GetEnvironmentVariable("PUSH_API_URL") ?? "https://api.push.example.com";
        _apiKey = Environment.GetEnvironmentVariable("PUSH_API_KEY") ?? string.Empty;
    }

    public async Task<bool> SendSmsAsync(string phoneNumber, string message)
    {
        try
        {
            _logger.LogInfo($"推播服務不支援簡訊功能");
            
            // 推播服務不支援簡訊，返回 false
            return await Task.FromResult(false);
        }
        catch (Exception ex)
        {
            _logger.LogError($"發送簡訊時發生錯誤: {phoneNumber}", ex);
            return false;
        }
    }

    public async Task<bool> SendPushNotificationAsync(string userId, string title, string body)
    {
        try
        {
            _logger.LogInfo($"發送推播通知: {userId}, 標題: {title}");

            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogWarning("使用者ID為空，無法發送推播通知");
                return false;
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                _logger.LogWarning("通知標題為空，無法發送推播通知");
                return false;
            }

            // 實作推播通知發送邏輯（可整合第三方推播服務 API，如 Firebase Cloud Messaging）
            // 這裡僅為範例，實際需根據使用的推播服務商 API 進行實作
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

            var requestBody = new
            {
                UserId = userId,
                Title = title,
                Body = body,
                Timestamp = DateTime.UtcNow
            };

            var response = await httpClient.PostAsJsonAsync($"{_apiUrl}/send", requestBody);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInfo($"推播通知發送成功: {userId}");
                return true;
            }
            else
            {
                _logger.LogError($"推播通知發送失敗: {userId}, 狀態碼: {response.StatusCode}");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"發送推播通知時發生錯誤: {userId}", ex);
            return false;
        }
    }

    public async Task<Dictionary<string, bool>> SendBulkSmsAsync(List<string> phoneNumbers, string message)
    {
        var results = new Dictionary<string, bool>();

        try
        {
            _logger.LogInfo($"批次發送簡訊: {phoneNumbers.Count} 筆");

            foreach (var phoneNumber in phoneNumbers)
            {
                var result = await SendSmsAsync(phoneNumber, message);
                results[phoneNumber] = result;
            }

            var successCount = results.Count(r => r.Value);
            _logger.LogInfo($"批次發送簡訊完成: 成功 {successCount}/{phoneNumbers.Count}");
        }
        catch (Exception ex)
        {
            _logger.LogError("批次發送簡訊時發生錯誤", ex);
        }

        return results;
    }

    public async Task<Dictionary<string, bool>> SendBulkPushNotificationAsync(List<string> userIds, string title, string body)
    {
        var results = new Dictionary<string, bool>();

        try
        {
            _logger.LogInfo($"批次發送推播通知: {userIds.Count} 筆");

            foreach (var userId in userIds)
            {
                var result = await SendPushNotificationAsync(userId, title, body);
                results[userId] = result;
            }

            var successCount = results.Count(r => r.Value);
            _logger.LogInfo($"批次發送推播通知完成: 成功 {successCount}/{userIds.Count}");
        }
        catch (Exception ex)
        {
            _logger.LogError("批次發送推播通知時發生錯誤", ex);
        }

        return results;
    }
}

