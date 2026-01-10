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
                // 實作POS資料同步邏輯
                // 1. 從外部POS系統取得交易資料（這裡使用模擬邏輯，實際應呼叫外部API）
                var pendingTransactions = await _transactionRepository.GetPendingTransactionsAsync(
                    request.StoreId, 
                    request.StartDate, 
                    request.EndDate);

                int processedCount = 0;
                int successCount = 0;
                int failedCount = 0;

                foreach (var transaction in pendingTransactions)
                {
                    try
                    {
                        processedCount++;

                        // 模擬外部API呼叫（實際應使用 HttpClient 呼叫外部POS系統API）
                        // 這裡簡化處理，實際應該：
                        // 1. 呼叫外部POS系統API取得交易資料
                        // 2. 驗證資料完整性
                        // 3. 轉換資料格式
                        // 4. 儲存到資料庫

                        // 更新交易狀態為已同步
                        await _transactionRepository.UpdateSyncStatusAsync(
                            transaction.TransactionId, 
                            "Synced", 
                            DateTime.Now, 
                            null);

                        successCount++;
                        _logger.LogInfo($"同步POS交易成功: {transaction.TransactionId}");
                    }
                    catch (Exception ex)
                    {
                        failedCount++;
                        await _transactionRepository.UpdateSyncStatusAsync(
                            transaction.TransactionId, 
                            "Failed", 
                            null, 
                            ex.Message);
                        _logger.LogError($"同步POS交易失敗: {transaction.TransactionId}", ex);
                    }
                }

                // 更新同步記錄
                syncLog.RecordCount = processedCount;
                syncLog.SuccessCount = successCount;
                syncLog.FailedCount = failedCount;
                syncLog.Status = failedCount == 0 ? "Completed" : "Partial";
                syncLog.EndTime = DateTime.Now;
                await _syncLogRepository.UpdateAsync(syncLog);

                _logger.LogInfo($"POS資料同步完成: 處理={processedCount}, 成功={successCount}, 失敗={failedCount}");

                return new PosSyncResultDto
                {
                    SyncLogId = syncLog.Id,
                    ProcessedCount = processedCount,
                    SuccessCount = successCount,
                    FailedCount = failedCount,
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

