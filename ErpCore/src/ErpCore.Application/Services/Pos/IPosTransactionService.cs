using ErpCore.Application.DTOs.Pos;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Pos;

/// <summary>
/// POS交易服務介面
/// </summary>
public interface IPosTransactionService
{
    /// <summary>
    /// 查詢POS交易列表
    /// </summary>
    Task<PagedResult<PosTransactionDto>> GetPosTransactionsAsync(PosTransactionQueryDto query);

    /// <summary>
    /// 根據交易編號查詢POS交易
    /// </summary>
    Task<PosTransactionDto?> GetPosTransactionByIdAsync(string transactionId);

    /// <summary>
    /// 同步POS交易資料
    /// </summary>
    Task<PosSyncResultDto> SyncTransactionsAsync(PosSyncRequestDto request);
}

/// <summary>
/// POS同步服務介面
/// </summary>
public interface IPosSyncService
{
    /// <summary>
    /// 同步POS交易資料
    /// </summary>
    Task<PosSyncResultDto> SyncTransactionsAsync(PosSyncRequestDto request);

    /// <summary>
    /// 查詢POS同步記錄列表
    /// </summary>
    Task<PagedResult<PosSyncLogDto>> GetSyncLogsAsync(PosSyncLogQueryDto query);

    /// <summary>
    /// 根據ID查詢POS同步記錄
    /// </summary>
    Task<PosSyncLogDto?> GetSyncLogByIdAsync(long id);
}

