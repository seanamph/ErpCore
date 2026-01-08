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

            // TODO: 實作實際的簡訊發送邏輯（整合簡訊服務提供商 API）
            // 目前先標記為已發送
            await _smsLogRepository.UpdateStatusAsync(savedLog.Id, "Sent", DateTime.Now);

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

