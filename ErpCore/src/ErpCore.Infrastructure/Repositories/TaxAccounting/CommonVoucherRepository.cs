using Dapper;
using ErpCore.Domain.Entities.TaxAccounting;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.TaxAccounting;

/// <summary>
/// 常用傳票 Repository 實作 (SYST123)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class CommonVoucherRepository : BaseRepository, ICommonVoucherRepository
{
    public CommonVoucherRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<CommonVoucher?> GetByTKeyAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM CommonVouchers 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<CommonVoucher>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢常用傳票失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<CommonVoucher?> GetByIdAsync(string voucherId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM CommonVouchers 
                WHERE CommonVoucherId = @VoucherId";

            return await QueryFirstOrDefaultAsync<CommonVoucher>(sql, new { VoucherId = voucherId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢常用傳票失敗: {voucherId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<CommonVoucher>> QueryAsync(CommonVoucherQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM CommonVouchers
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.VoucherId))
            {
                sql += " AND CommonVoucherId LIKE @VoucherId";
                parameters.Add("VoucherId", $"%{query.VoucherId}%");
            }

            if (!string.IsNullOrEmpty(query.VoucherName))
            {
                sql += " AND CommonVoucherName LIKE @VoucherName";
                parameters.Add("VoucherName", $"%{query.VoucherName}%");
            }

            if (!string.IsNullOrEmpty(query.VoucherType))
            {
                sql += " AND VoucherTypeId = @VoucherType";
                parameters.Add("VoucherType", query.VoucherType);
            }

            if (!string.IsNullOrEmpty(query.SiteId))
            {
                sql += " AND SiteId = @SiteId";
                parameters.Add("SiteId", query.SiteId);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "CommonVoucherId" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<CommonVoucher>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM CommonVouchers
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.VoucherId))
            {
                countSql += " AND CommonVoucherId LIKE @VoucherId";
                countParameters.Add("VoucherId", $"%{query.VoucherId}%");
            }
            if (!string.IsNullOrEmpty(query.VoucherName))
            {
                countSql += " AND CommonVoucherName LIKE @VoucherName";
                countParameters.Add("VoucherName", $"%{query.VoucherName}%");
            }
            if (!string.IsNullOrEmpty(query.VoucherType))
            {
                countSql += " AND VoucherTypeId = @VoucherType";
                countParameters.Add("VoucherType", query.VoucherType);
            }
            if (!string.IsNullOrEmpty(query.SiteId))
            {
                countSql += " AND SiteId = @SiteId";
                countParameters.Add("SiteId", query.SiteId);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<CommonVoucher>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢常用傳票列表失敗", ex);
            throw;
        }
    }

    public async Task<CommonVoucher> CreateAsync(CommonVoucher commonVoucher)
    {
        try
        {
            const string sql = @"
                INSERT INTO CommonVouchers (
                    CommonVoucherId, CommonVoucherName, VoucherTypeId, Description, Status, SortOrder,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup
                ) VALUES (
                    @CommonVoucherId, @CommonVoucherName, @VoucherTypeId, @Description, @Status, @SortOrder,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup
                );
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await QuerySingleAsync<long>(sql, new
            {
                CommonVoucherId = commonVoucher.VoucherId,
                CommonVoucherName = commonVoucher.VoucherName,
                VoucherTypeId = commonVoucher.VoucherType,
                Description = commonVoucher.Notes,
                Status = "A",
                SortOrder = (int?)null,
                commonVoucher.SiteId,
                commonVoucher.VendorId,
                commonVoucher.VendorName,
                commonVoucher.Notes,
                commonVoucher.CustomField1,
                commonVoucher.CreatedBy,
                commonVoucher.CreatedAt,
                commonVoucher.UpdatedBy,
                commonVoucher.UpdatedAt,
                commonVoucher.CreatedPriority,
                commonVoucher.CreatedGroup
            });

            commonVoucher.TKey = tKey;
            return commonVoucher;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增常用傳票失敗: {commonVoucher.VoucherId}", ex);
            throw;
        }
    }

    public async Task<CommonVoucher> UpdateAsync(CommonVoucher commonVoucher)
    {
        try
        {
            const string sql = @"
                UPDATE CommonVouchers SET
                    CommonVoucherName = @CommonVoucherName,
                    VoucherTypeId = @VoucherTypeId,
                    Description = @Description,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new
            {
                commonVoucher.TKey,
                CommonVoucherName = commonVoucher.VoucherName,
                VoucherTypeId = commonVoucher.VoucherType,
                Description = commonVoucher.Notes,
                commonVoucher.UpdatedBy,
                commonVoucher.UpdatedAt
            });

            return commonVoucher;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改常用傳票失敗: {commonVoucher.VoucherId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            // 先刪除明細（級聯刪除）
            await DeleteDetailsAsync(tKey);

            const string sql = @"
                DELETE FROM CommonVouchers
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除常用傳票失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string voucherId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM CommonVouchers
                WHERE CommonVoucherId = @VoucherId";

            var count = await QuerySingleAsync<int>(sql, new { VoucherId = voucherId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查常用傳票是否存在失敗: {voucherId}", ex);
            throw;
        }
    }

    public async Task<List<CommonVoucherDetail>> GetDetailsAsync(long voucherTKey)
    {
        try
        {
            // 先取得 CommonVoucherId
            var voucher = await GetByTKeyAsync(voucherTKey);
            if (voucher == null)
            {
                return new List<CommonVoucherDetail>();
            }

            const string sql = @"
                SELECT * FROM CommonVoucherDetails
                WHERE CommonVoucherId = @CommonVoucherId
                ORDER BY SeqNo";

            var items = await QueryAsync<CommonVoucherDetail>(sql, new { CommonVoucherId = voucher.VoucherId });
            return items.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢常用傳票明細失敗: {voucherTKey}", ex);
            throw;
        }
    }

    public async Task CreateDetailAsync(CommonVoucherDetail detail)
    {
        try
        {
            // 先取得 CommonVoucherId
            var voucher = await GetByTKeyAsync(detail.VoucherTKey);
            if (voucher == null)
            {
                throw new InvalidOperationException($"常用傳票不存在: {detail.VoucherTKey}");
            }

            const string sql = @"
                INSERT INTO CommonVoucherDetails (
                    CommonVoucherId, SeqNo, StypeId, Dc, Amount, Description,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                ) VALUES (
                    @CommonVoucherId, @SeqNo, @StypeId, @Dc, @Amount, @Description,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            // 根據借貸金額決定 Dc 和 Amount
            var dc = detail.DebitAmount > 0 ? "D" : "C";
            var amount = detail.DebitAmount > 0 ? detail.DebitAmount : detail.CreditAmount;

            var tKey = await QuerySingleAsync<long>(sql, new
            {
                CommonVoucherId = voucher.VoucherId,
                detail.SeqNo,
                detail.StypeId,
                detail.DebitAmount,
                detail.CreditAmount,
                detail.OrgId,
                detail.Notes,
                detail.VendorId,
                detail.CustomField1,
                detail.CreatedBy,
                detail.CreatedAt,
                detail.UpdatedBy,
                detail.UpdatedAt
            });

            detail.TKey = tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增常用傳票明細失敗", ex);
            throw;
        }
    }

    public async Task UpdateDetailAsync(CommonVoucherDetail detail)
    {
        try
        {
            // 根據借貸金額決定 Dc 和 Amount
            var dc = detail.DebitAmount > 0 ? "D" : "C";
            var amount = detail.DebitAmount > 0 ? detail.DebitAmount : detail.CreditAmount;

            const string sql = @"
                UPDATE CommonVoucherDetails SET
                    SeqNo = @SeqNo,
                    StypeId = @StypeId,
                    Dc = @Dc,
                    Amount = @Amount,
                    Description = @Description,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new
            {
                detail.TKey,
                detail.SeqNo,
                detail.StypeId,
                Dc = dc,
                Amount = amount,
                Description = detail.Notes,
                detail.UpdatedBy,
                detail.UpdatedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改常用傳票明細失敗: {detail.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteDetailsAsync(long voucherTKey)
    {
        try
        {
            // 先取得 CommonVoucherId
            var voucher = await GetByTKeyAsync(voucherTKey);
            if (voucher == null)
            {
                return;
            }

            const string sql = @"
                DELETE FROM CommonVoucherDetails
                WHERE CommonVoucherId = @CommonVoucherId";

            await ExecuteAsync(sql, new { CommonVoucherId = voucher.VoucherId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除常用傳票明細失敗: {voucherTKey}", ex);
            throw;
        }
    }
}

