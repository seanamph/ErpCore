using System.Data;
using System.Linq;
using Dapper;
using ErpCore.Domain.Entities.ReportManagement;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.ReportManagement;

/// <summary>
/// 收款沖帳傳票 Repository 實作 (SYSR310-SYSR450)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class ReceiptVoucherTransferRepository : BaseRepository, IReceiptVoucherTransferRepository
{
    public ReceiptVoucherTransferRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<ReceiptVoucherTransfer?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM ReceiptVoucherTransfer 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<ReceiptVoucherTransfer>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢收款沖帳傳票失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<ReceiptVoucherTransfer>> GetAllAsync()
    {
        try
        {
            const string sql = @"SELECT * FROM ReceiptVoucherTransfer ORDER BY ReceiptDate DESC, TKey DESC";

            return await QueryAsync<ReceiptVoucherTransfer>(sql);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢收款沖帳傳票列表失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<ReceiptVoucherTransfer>> QueryAsync(ReceiptVoucherTransferQuery query)
    {
        try
        {
            var sql = @"SELECT * FROM ReceiptVoucherTransfer WHERE 1=1";
            var countSql = @"SELECT COUNT(1) FROM ReceiptVoucherTransfer WHERE 1=1";
            var parameters = new DynamicParameters();

            // 查詢條件
            if (!string.IsNullOrEmpty(query.ReceiptNo))
            {
                sql += " AND ReceiptNo LIKE @ReceiptNo";
                countSql += " AND ReceiptNo LIKE @ReceiptNo";
                parameters.Add("ReceiptNo", $"%{query.ReceiptNo}%");
            }

            if (!string.IsNullOrEmpty(query.VoucherNo))
            {
                sql += " AND VoucherNo LIKE @VoucherNo";
                countSql += " AND VoucherNo LIKE @VoucherNo";
                parameters.Add("VoucherNo", $"%{query.VoucherNo}%");
            }

            if (query.ReceiptDateFrom.HasValue)
            {
                sql += " AND ReceiptDate >= @ReceiptDateFrom";
                countSql += " AND ReceiptDate >= @ReceiptDateFrom";
                parameters.Add("ReceiptDateFrom", query.ReceiptDateFrom.Value);
            }

            if (query.ReceiptDateTo.HasValue)
            {
                sql += " AND ReceiptDate <= @ReceiptDateTo";
                countSql += " AND ReceiptDate <= @ReceiptDateTo";
                parameters.Add("ReceiptDateTo", query.ReceiptDateTo.Value);
            }

            if (!string.IsNullOrEmpty(query.TransferStatus))
            {
                sql += " AND TransferStatus = @TransferStatus";
                countSql += " AND TransferStatus = @TransferStatus";
                parameters.Add("TransferStatus", query.TransferStatus);
            }

            if (!string.IsNullOrEmpty(query.ObjectId))
            {
                sql += " AND ObjectId = @ObjectId";
                countSql += " AND ObjectId = @ObjectId";
                parameters.Add("ObjectId", query.ObjectId);
            }

            if (!string.IsNullOrEmpty(query.ShopId))
            {
                sql += " AND ShopId = @ShopId";
                countSql += " AND ShopId = @ShopId";
                parameters.Add("ShopId", query.ShopId);
            }

            if (!string.IsNullOrEmpty(query.SiteId))
            {
                sql += " AND SiteId = @SiteId";
                countSql += " AND SiteId = @SiteId";
                parameters.Add("SiteId", query.SiteId);
            }

            // 排序
            var sortBy = query.SortBy ?? "ReceiptDate";
            var sortOrder = query.SortOrder ?? "DESC";
            sql += $" ORDER BY {sortBy} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<ReceiptVoucherTransfer>(sql, parameters);
            var totalCount = await ExecuteScalarAsync<int>(countSql, parameters);

            return new PagedResult<ReceiptVoucherTransfer>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢收款沖帳傳票列表失敗", ex);
            throw;
        }
    }

    public async Task<IEnumerable<ReceiptVoucherTransfer>> GetByReceiptNoAsync(string receiptNo)
    {
        try
        {
            const string sql = @"
                SELECT * FROM ReceiptVoucherTransfer 
                WHERE ReceiptNo = @ReceiptNo 
                ORDER BY ReceiptDate DESC";

            return await QueryAsync<ReceiptVoucherTransfer>(sql, new { ReceiptNo = receiptNo });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢收款沖帳傳票列表失敗: ReceiptNo={receiptNo}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<ReceiptVoucherTransfer>> GetByVoucherNoAsync(string voucherNo)
    {
        try
        {
            const string sql = @"
                SELECT * FROM ReceiptVoucherTransfer 
                WHERE VoucherNo = @VoucherNo 
                ORDER BY ReceiptDate DESC";

            return await QueryAsync<ReceiptVoucherTransfer>(sql, new { VoucherNo = voucherNo });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢收款沖帳傳票列表失敗: VoucherNo={voucherNo}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<ReceiptVoucherTransfer>> GetByTransferStatusAsync(string transferStatus)
    {
        try
        {
            const string sql = @"
                SELECT * FROM ReceiptVoucherTransfer 
                WHERE TransferStatus = @TransferStatus 
                ORDER BY ReceiptDate DESC";

            return await QueryAsync<ReceiptVoucherTransfer>(sql, new { TransferStatus = transferStatus });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢收款沖帳傳票列表失敗: TransferStatus={transferStatus}", ex);
            throw;
        }
    }

    public async Task<ReceiptVoucherTransfer> CreateAsync(ReceiptVoucherTransfer entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO ReceiptVoucherTransfer (
                    ReceiptNo, ReceiptDate, ObjectId, AcctKey, ReceiptAmount, AritemId,
                    VoucherNo, VoucherM_TKey, TransferStatus, TransferDate, TransferUser, ErrorMessage,
                    ShopId, SiteId, OrgId, Notes, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                )
                VALUES (
                    @ReceiptNo, @ReceiptDate, @ObjectId, @AcctKey, @ReceiptAmount, @AritemId,
                    @VoucherNo, @VoucherM_TKey, @TransferStatus, @TransferDate, @TransferUser, @ErrorMessage,
                    @ShopId, @SiteId, @OrgId, @Notes, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, new
            {
                entity.ReceiptNo,
                entity.ReceiptDate,
                entity.ObjectId,
                entity.AcctKey,
                entity.ReceiptAmount,
                entity.AritemId,
                entity.VoucherNo,
                entity.VoucherM_TKey,
                entity.TransferStatus,
                entity.TransferDate,
                entity.TransferUser,
                entity.ErrorMessage,
                entity.ShopId,
                entity.SiteId,
                entity.OrgId,
                entity.Notes,
                entity.CreatedBy,
                entity.CreatedAt,
                entity.UpdatedBy,
                entity.UpdatedAt
            });

            entity.TKey = tKey;
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增收款沖帳傳票失敗", ex);
            throw;
        }
    }

    public async Task<ReceiptVoucherTransfer> UpdateAsync(ReceiptVoucherTransfer entity)
    {
        try
        {
            const string sql = @"
                UPDATE ReceiptVoucherTransfer 
                SET ReceiptNo = @ReceiptNo,
                    ReceiptDate = @ReceiptDate,
                    ObjectId = @ObjectId,
                    AcctKey = @AcctKey,
                    ReceiptAmount = @ReceiptAmount,
                    AritemId = @AritemId,
                    VoucherNo = @VoucherNo,
                    VoucherM_TKey = @VoucherM_TKey,
                    TransferStatus = @TransferStatus,
                    TransferDate = @TransferDate,
                    TransferUser = @TransferUser,
                    ErrorMessage = @ErrorMessage,
                    ShopId = @ShopId,
                    SiteId = @SiteId,
                    OrgId = @OrgId,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new
            {
                entity.TKey,
                entity.ReceiptNo,
                entity.ReceiptDate,
                entity.ObjectId,
                entity.AcctKey,
                entity.ReceiptAmount,
                entity.AritemId,
                entity.VoucherNo,
                entity.VoucherM_TKey,
                entity.TransferStatus,
                entity.TransferDate,
                entity.TransferUser,
                entity.ErrorMessage,
                entity.ShopId,
                entity.SiteId,
                entity.OrgId,
                entity.Notes,
                entity.UpdatedBy,
                entity.UpdatedAt
            });

            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新收款沖帳傳票失敗: {entity.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"DELETE FROM ReceiptVoucherTransfer WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除收款沖帳傳票失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<int> BatchUpdateTransferStatusAsync(List<long> tKeys, string transferStatus, string transferUser, DateTime transferDate)
    {
        try
        {
            if (tKeys == null || !tKeys.Any())
                return 0;

            const string sql = @"
                UPDATE ReceiptVoucherTransfer 
                SET TransferStatus = @TransferStatus,
                    TransferDate = @TransferDate,
                    TransferUser = @TransferUser,
                    UpdatedAt = GETDATE()
                WHERE TKey IN @TKeys";

            var parameters = new
            {
                TransferStatus = transferStatus,
                TransferDate = transferDate,
                TransferUser = transferUser,
                TKeys = tKeys
            };

            return await ExecuteAsync(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError($"批次更新收款沖帳傳票狀態失敗", ex);
            throw;
        }
    }
}

