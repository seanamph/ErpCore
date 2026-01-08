using ErpCore.Application.DTOs.Pos;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Pos;
using ErpCore.Infrastructure.Repositories.Pos;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Pos;

/// <summary>
/// POS同步服務實作
/// </summary>
public class PosSyncService : BaseService, IPosSyncService
{
    private readonly IPosSyncLogRepository _syncLogRepository;
    private readonly IPosTransactionRepository _transactionRepository;

    public PosSyncService(
        IPosSyncLogRepository syncLogRepository,
        IPosTransactionRepository transactionRepository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _syncLogRepository = syncLogRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<PosSyncResultDto> SyncTransactionsAsync(PosSyncRequestDto request)
    {
        try
        {
            _logger.LogInfo($"開始同步POS交易資料: StoreId={request.StoreId}, StartDate={request.StartDate}, EndDate={request.EndDate}");

            // 建立同步記錄
            var syncLog = new PosSyncLog
            {
                SyncType = "Transaction",
                SyncDirection = "ToIMS",
                RecordCount = 0,
                SuccessCount = 0,
                FailedCount = 0,
                Status = "Running",
                StartTime = DateTime.Now,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now
            };

            syncLog = await _syncLogRepository.CreateAsync(syncLog);

            try
            {
                // 這裡應該實作實際的同步邏輯
                // 目前僅返回基本結構
                syncLog.Status = "Completed";
                syncLog.EndTime = DateTime.Now;
                await _syncLogRepository.UpdateAsync(syncLog);

                return new PosSyncResultDto
                {
                    SyncLogId = syncLog.Id,
                    ProcessedCount = syncLog.RecordCount,
                    SuccessCount = syncLog.SuccessCount,
                    FailedCount = syncLog.FailedCount,
                    Status = syncLog.Status
                };
            }
            catch (Exception ex)
            {
                syncLog.Status = "Failed";
                syncLog.EndTime = DateTime.Now;
                syncLog.ErrorMessage = ex.Message;
                await _syncLogRepository.UpdateAsync(syncLog);

                _logger.LogError("同步POS交易資料失敗", ex);
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("同步POS交易資料失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<PosSyncLogDto>> GetSyncLogsAsync(PosSyncLogQueryDto query)
    {
        try
        {
            var repositoryQuery = new PosSyncLogQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                SyncType = query.SyncType,
                SyncDirection = query.SyncDirection,
                Status = query.Status,
                StartDate = query.StartDate,
                EndDate = query.EndDate
            };

            var result = await _syncLogRepository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => new PosSyncLogDto
            {
                Id = x.Id,
                SyncType = x.SyncType,
                SyncDirection = x.SyncDirection,
                RecordCount = x.RecordCount,
                SuccessCount = x.SuccessCount,
                FailedCount = x.FailedCount,
                Status = x.Status,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                ErrorMessage = x.ErrorMessage,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt
            }).ToList();

            return new PagedResult<PosSyncLogDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢POS同步記錄列表失敗", ex);
            throw;
        }
    }

    public async Task<PosSyncLogDto?> GetSyncLogByIdAsync(long id)
    {
        try
        {
            var log = await _syncLogRepository.GetByIdAsync(id);
            if (log == null)
            {
                return null;
            }

            return new PosSyncLogDto
            {
                Id = log.Id,
                SyncType = log.SyncType,
                SyncDirection = log.SyncDirection,
                RecordCount = log.RecordCount,
                SuccessCount = log.SuccessCount,
                FailedCount = log.FailedCount,
                Status = log.Status,
                StartTime = log.StartTime,
                EndTime = log.EndTime,
                ErrorMessage = log.ErrorMessage,
                CreatedBy = log.CreatedBy,
                CreatedAt = log.CreatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢POS同步記錄失敗: {id}", ex);
            throw;
        }
    }
}

