using ErpCore.Shared.Logging;
using System.Net.Http.Json;

namespace ErpCore.Infrastructure.Services.Notification;

/// <summary>
/// 簡訊通知服務實作
/// 提供簡訊發送功能
/// </summary>
public class SmsNotificationService : INotificationService
{
    private readonly ILoggerService _logger;
    private readonly string _apiUrl;
    private readonly string _apiKey;

    public SmsNotificationService(ILoggerService logger)
    {
        _logger = logger;
        // 從設定檔讀取 API 設定（可在 appsettings.json 中設定）
        _apiUrl = Environment.GetEnvironmentVariable("SMS_API_URL") ?? "https://api.sms.example.com";
        _apiKey = Environment.GetEnvironmentVariable("SMS_API_KEY") ?? string.Empty;
    }

    public async Task<bool> SendSmsAsync(string phoneNumber, string message)
    {
        try
        {
            _logger.LogInfo($"發送簡訊: {phoneNumber}, 訊息長度: {message.Length}");

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                _logger.LogWarning("手機號碼為空，無法發送簡訊");
                return false;
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                _logger.LogWarning("訊息內容為空，無法發送簡訊");
                return false;
            }

            // 實作簡訊發送邏輯（可整合第三方簡訊服務 API）
            // 這裡僅為範例，實際需根據使用的簡訊服務商 API 進行實作
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

            var requestBody = new
            {
                PhoneNumber = phoneNumber,
                Message = message
            };

            var response = await httpClient.PostAsJsonAsync($"{_apiUrl}/send", requestBody);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInfo($"簡訊發送成功: {phoneNumber}");
                return true;
            }
            else
            {
                _logger.LogError($"簡訊發送失敗: {phoneNumber}, 狀態碼: {response.StatusCode}");
                return false;
            }
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
            
            // 簡訊服務不支援推播通知，返回 false
            _logger.LogWarning($"簡訊服務不支援推播通知功能");
            return await Task.FromResult(false);
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

