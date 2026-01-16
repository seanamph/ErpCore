using System.Data;
using Dapper;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Transfer;

/// <summary>
/// 調撥單 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class TransferOrderRepository : BaseRepository, ITransferOrderRepository
{
    public TransferOrderRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    /// <summary>
    /// 更新調撥單明細已退數量
    /// 注意: 假設有 TransferOrderDetails 資料表，包含 DetailId, ReturnQty 欄位
    /// </summary>
    public async Task UpdateReturnQtyAsync(Guid transferDetailId, decimal returnQty, global::System.Data.IDbTransaction? transaction = null)
    {
        try
        {
            const string sql = @"
                UPDATE TransferOrderDetails 
                SET ReturnQty = ISNULL(ReturnQty, 0) + @ReturnQty,
                    UpdatedAt = GETDATE()
                WHERE DetailId = @TransferDetailId";

            var parameters = new { TransferDetailId = transferDetailId, ReturnQty = returnQty };

            if (transaction != null)
            {
                await transaction.Connection!.ExecuteAsync(sql, parameters, transaction);
            }
            else
            {
                await ExecuteAsync(sql, parameters);
            }

            _logger.LogInfo($"更新調撥單已退數量成功: TransferDetailId={transferDetailId}, ReturnQty={returnQty}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新調撥單已退數量失敗: TransferDetailId={transferDetailId}, ReturnQty={returnQty}", ex);
            throw;
        }
    }

    /// <summary>
    /// 取得調撥單資料
    /// </summary>
    public async Task<TransferOrderInfo?> GetTransferOrderAsync(string transferId)
    {
        try
        {
            const string sql = @"
                SELECT TransferId, TransferDate, FromShopId, ToShopId, Status
                FROM TransferOrders
                WHERE TransferId = @TransferId";

            return await QueryFirstOrDefaultAsync<TransferOrderInfo>(sql, new { TransferId = transferId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"取得調撥單失敗: TransferId={transferId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 取得調撥單明細資料
    /// </summary>
    public async Task<IEnumerable<TransferOrderDetailInfo>> GetTransferOrderDetailsAsync(string transferId)
    {
        try
        {
            const string sql = @"
                SELECT DetailId, TransferId, GoodsId, TransferQty, ReturnQty, ReceiptQty
                FROM TransferOrderDetails
                WHERE TransferId = @TransferId
                ORDER BY LineNum";

            return await QueryAsync<TransferOrderDetailInfo>(sql, new { TransferId = transferId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"取得調撥單明細失敗: TransferId={transferId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 更新調撥單明細已收數量
    /// 注意: 假設有 TransferOrderDetails 資料表，包含 DetailId, ReceiptQty 欄位
    /// </summary>
    public async Task UpdateReceiptQtyAsync(Guid transferDetailId, decimal receiptQty, global::System.Data.IDbTransaction? transaction = null)
    {
        try
        {
            const string sql = @"
                UPDATE TransferOrderDetails 
                SET ReceiptQty = ISNULL(ReceiptQty, 0) + @ReceiptQty,
                    UpdatedAt = GETDATE()
                WHERE DetailId = @TransferDetailId";

            var parameters = new { TransferDetailId = transferDetailId, ReceiptQty = receiptQty };

            if (transaction != null)
            {
                await transaction.Connection!.ExecuteAsync(sql, parameters, transaction);
            }
            else
            {
                await ExecuteAsync(sql, parameters);
            }

            _logger.LogInfo($"更新調撥單已收數量成功: TransferDetailId={transferDetailId}, ReceiptQty={receiptQty}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新調撥單已收數量失敗: TransferDetailId={transferDetailId}, ReceiptQty={receiptQty}", ex);
            throw;
        }
    }
}

