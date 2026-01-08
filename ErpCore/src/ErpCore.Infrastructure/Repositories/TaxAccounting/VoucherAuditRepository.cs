using Dapper;
using ErpCore.Domain.Entities.TaxAccounting;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.TaxAccounting;

/// <summary>
/// 暫存傳票審核 Repository 實作 (SYSTA00-SYSTA70)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class VoucherAuditRepository : BaseRepository, IVoucherAuditRepository
{
    public VoucherAuditRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<TmpVoucherM?> GetTmpVoucherByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM TmpVoucherM 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<TmpVoucherM>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢暫存傳票失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<PagedResult<TmpVoucherM>> GetTmpVouchersPagedAsync(TmpVoucherQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM TmpVoucherM
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.TypeId))
            {
                sql += " AND TypeId = @TypeId";
                parameters.Add("TypeId", query.TypeId);
            }

            if (!string.IsNullOrEmpty(query.SysId))
            {
                sql += " AND SysId = @SysId";
                parameters.Add("SysId", query.SysId);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (query.VoucherDateFrom.HasValue)
            {
                sql += " AND VoucherDate >= @VoucherDateFrom";
                parameters.Add("VoucherDateFrom", query.VoucherDateFrom.Value);
            }

            if (query.VoucherDateTo.HasValue)
            {
                sql += " AND VoucherDate <= @VoucherDateTo";
                parameters.Add("VoucherDateTo", query.VoucherDateTo.Value);
            }

            if (!string.IsNullOrEmpty(query.SlipType))
            {
                sql += " AND SlipType = @SlipType";
                parameters.Add("SlipType", query.SlipType);
            }

            if (!string.IsNullOrEmpty(query.VendorId))
            {
                sql += " AND VendorId = @VendorId";
                parameters.Add("VendorId", query.VendorId);
            }

            if (!string.IsNullOrEmpty(query.StoreId))
            {
                sql += " AND StoreId = @StoreId";
                parameters.Add("StoreId", query.StoreId);
            }

            if (!string.IsNullOrEmpty(query.SiteId))
            {
                sql += " AND SiteId = @SiteId";
                parameters.Add("SiteId", query.SiteId);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "VoucherDate" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 總數查詢
            var countSql = sql.Replace("SELECT *", "SELECT COUNT(*)");
            var totalCount = await QuerySingleAsync<int>(countSql, parameters);

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<TmpVoucherM>(sql, parameters);

            return new PagedResult<TmpVoucherM>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢暫存傳票列表失敗", ex);
            throw;
        }
    }

    public async Task<TmpVoucherM> UpdateTmpVoucherAsync(TmpVoucherM voucher)
    {
        try
        {
            const string sql = @"
                UPDATE TmpVoucherM SET
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                OUTPUT INSERTED.*
                WHERE TKey = @TKey";

            return await QuerySingleAsync<TmpVoucherM>(sql, voucher);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改暫存傳票失敗: {voucher.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteTmpVoucherAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM TmpVoucherM 
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除暫存傳票失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<bool> TmpVoucherExistsAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM TmpVoucherM 
                WHERE TKey = @TKey";

            var count = await QuerySingleAsync<int>(sql, new { TKey = tKey });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查暫存傳票是否存在失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<List<TmpVoucherD>> GetTmpVoucherDetailsAsync(long voucherTKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM TmpVoucherD 
                WHERE VoucherTKey = @VoucherTKey
                ORDER BY Sn";

            return (await QueryAsync<TmpVoucherD>(sql, new { VoucherTKey = voucherTKey })).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢暫存傳票明細失敗: {voucherTKey}", ex);
            throw;
        }
    }

    public async Task CreateTmpVoucherDetailAsync(TmpVoucherD detail)
    {
        try
        {
            const string sql = @"
                INSERT INTO TmpVoucherD (
                    VoucherTKey, Sn, Dc, SubN, OrgId, ActId, Notes,
                    Val0, Val1, VendorId, AbatId, ObjectId,
                    CreatedBy, CreatedAt
                )
                VALUES (
                    @VoucherTKey, @Sn, @Dc, @SubN, @OrgId, @ActId, @Notes,
                    @Val0, @Val1, @VendorId, @AbatId, @ObjectId,
                    @CreatedBy, @CreatedAt
                )";

            await ExecuteAsync(sql, detail);
        }
        catch (Exception ex)
        {
            _logger.LogError("新增暫存傳票明細失敗", ex);
            throw;
        }
    }

    public async Task UpdateTmpVoucherDetailAsync(TmpVoucherD detail)
    {
        try
        {
            const string sql = @"
                UPDATE TmpVoucherD SET
                    Sn = @Sn, Dc = @Dc, SubN = @SubN, OrgId = @OrgId, ActId = @ActId, Notes = @Notes,
                    Val0 = @Val0, Val1 = @Val1, VendorId = @VendorId, AbatId = @AbatId, ObjectId = @ObjectId
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, detail);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改暫存傳票明細失敗: {detail.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteTmpVoucherDetailsAsync(long voucherTKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM TmpVoucherD 
                WHERE VoucherTKey = @VoucherTKey";

            await ExecuteAsync(sql, new { VoucherTKey = voucherTKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除暫存傳票明細失敗: {voucherTKey}", ex);
            throw;
        }
    }

    public async Task<int> GetUnreviewedCountAsync(string? typeId = null, string? sysId = null)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM TmpVoucherM 
                WHERE Status = '1'";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(typeId))
            {
                sql += " AND TypeId = @TypeId";
                parameters.Add("TypeId", typeId);
            }

            if (!string.IsNullOrEmpty(sysId))
            {
                sql += " AND SysId = @SysId";
                parameters.Add("SysId", sysId);
            }

            return await QuerySingleAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢未審核筆數失敗", ex);
            throw;
        }
    }

    public async Task<List<SystemVoucherCountDto>> GetSystemVoucherCountsAsync()
    {
        try
        {
            const string sql = @"
                SELECT 
                    SysId,
                    COUNT(*) AS UnreviewedCount
                FROM TmpVoucherM 
                WHERE Status = '1'
                GROUP BY SysId";

            var results = await QueryAsync<dynamic>(sql);

            return results.Select(x => new SystemVoucherCountDto
            {
                SysId = x.SysId ?? string.Empty,
                SysName = string.Empty, // 需要從系統主檔查詢
                ProgId = string.Empty, // 需要從系統主檔查詢
                UnreviewedCount = (int)x.UnreviewedCount
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢系統傳票統計失敗", ex);
            throw;
        }
    }

    public async Task<string> GenerateVoucherIdAsync(DateTime voucherDate)
    {
        try
        {
            // 產生傳票編號邏輯（需要根據實際需求實作）
            var year = voucherDate.Year;
            var month = voucherDate.Month.ToString("D2");
            var day = voucherDate.Day.ToString("D2");

            const string sql = @"
                SELECT COUNT(*) FROM Vouchers 
                WHERE VoucherId LIKE @Pattern";

            var pattern = $"{year}{month}{day}%";
            var count = await QuerySingleAsync<int>(sql, new { Pattern = pattern });

            return $"{year}{month}{day}{(count + 1):D4}";
        }
        catch (Exception ex)
        {
            _logger.LogError("產生傳票編號失敗", ex);
            throw;
        }
    }

    public async Task<bool> IsVoucherDateValidAsync(DateTime voucherDate)
    {
        try
        {
            // 檢查傳票日期是否大於關帳年月（需要根據實際需求實作）
            // 這裡先返回 true，實際需要查詢關帳年月設定
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("檢查傳票日期有效性失敗", ex);
            throw;
        }
    }
}

