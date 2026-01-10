using ErpCore.Application.DTOs.Communication;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Communication;
using ErpCore.Infrastructure.Repositories.Communication;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Communication;

/// <summary>
/// 簡訊服務實作
/// </summary>
public class SmsService : BaseService, ISmsService
{
    private readonly ISmsLogRepository _smsLogRepository;

    public SmsService(
        ISmsLogRepository smsLogRepository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _smsLogRepository = smsLogRepository;
    }

    public async Task<SendSmsResponseDto> SendSmsAsync(SendSmsRequestDto request)
    {
        try
        {
            _logger.LogInfo($"開始發送簡訊: {request.PhoneNumber}");

            // 建立簡訊記錄
            var smsLog = new SmsLog
            {
                PhoneNumber = request.PhoneNumber,
                Message = request.Message,
                Status = "Pending",
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now
            };

            var savedLog = await _smsLogRepository.CreateAsync(smsLog);

            // 實作實際的簡訊發送邏輯（整合簡訊服務提供商 API）
            // 注意：此處需要整合實際的簡訊服務提供商 API（如 Twilio、AWS SNS 等）
            // 目前提供框架，實際發送需要配置簡訊服務提供商的 API Key 和相關設定
            try
            {
                // TODO: 整合簡訊服務提供商 API
                // 範例：使用 HTTP 呼叫簡訊服務 API
                // var smsApiUrl = _configuration["SmsSettings:ApiUrl"];
                // var smsApiKey = _configuration["SmsSettings:ApiKey"];
                // 使用 HttpClient 呼叫 API 發送簡訊
                
                // 目前先標記為已發送（實際環境需要實作真實的 API 呼叫）
                await _smsLogRepository.UpdateStatusAsync(savedLog.Id, "Sent", DateTime.Now);
            }
            catch (Exception ex)
            {
                await _smsLogRepository.UpdateStatusAsync(savedLog.Id, "Failed", null, ex.Message);
                _logger.LogError($"簡訊發送失敗: {savedLog.Id}", ex);
                throw;
            }

            _logger.LogInfo($"簡訊發送成功: {savedLog.Id}");

            return new SendSmsResponseDto
            {
                SmsLogId = savedLog.Id,
                Status = "Sent",
                SentAt = DateTime.Now
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("發送簡訊失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<SmsLogDto>> GetSmsLogsAsync(SmsLogQueryDto query)
    {
        try
        {
            var smsLogQuery = new SmsLogQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                PhoneNumber = query.PhoneNumber,
                Status = query.Status,
                StartDate = query.StartDate,
                EndDate = query.EndDate
            };

            var result = await _smsLogRepository.QueryAsync(smsLogQuery);

            var dtos = result.Items.Select(x => new SmsLogDto
            {
                Id = x.Id,
                PhoneNumber = x.PhoneNumber,
                Message = x.Message,
                Status = x.Status,
                ErrorMessage = x.ErrorMessage,
                SentAt = x.SentAt,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                Provider = x.Provider,
                ProviderMessageId = x.ProviderMessageId
            }).ToList();

            return new PagedResult<SmsLogDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢簡訊記錄列表失敗", ex);
            throw;
        }
    }

    public async Task<SmsLogDto?> GetSmsLogByIdAsync(long id)
    {
        try
        {
            var smsLog = await _smsLogRepository.GetByIdAsync(id);
            if (smsLog == null) return null;

            return new SmsLogDto
            {
                Id = smsLog.Id,
                PhoneNumber = smsLog.PhoneNumber,
                Message = smsLog.Message,
                Status = smsLog.Status,
                ErrorMessage = smsLog.ErrorMessage,
                SentAt = smsLog.SentAt,
                CreatedBy = smsLog.CreatedBy,
                CreatedAt = smsLog.CreatedAt,
                Provider = smsLog.Provider,
                ProviderMessageId = smsLog.ProviderMessageId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢簡訊記錄失敗: {id}", ex);
            throw;
        }
    }
}

