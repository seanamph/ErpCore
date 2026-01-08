using Dapper;
using ErpCore.Domain.Entities.TaxAccounting;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.TaxAccounting;

/// <summary>
/// 傳票型態 Repository 實作 (SYST121-SYST122)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class VoucherTypeRepository : BaseRepository, IVoucherTypeRepository
{
    public VoucherTypeRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<VoucherType?> GetByIdAsync(string voucherId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM VoucherTypes 
                WHERE VoucherTypeId = @VoucherId";

            return await QueryFirstOrDefaultAsync<VoucherType>(sql, new { VoucherId = voucherId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢傳票型態失敗: {voucherId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<VoucherType>> QueryAsync(VoucherTypeQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM VoucherTypes
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.VoucherId))
            {
                sql += " AND VoucherTypeId LIKE @VoucherId";
                parameters.Add("VoucherId", $"%{query.VoucherId}%");
            }

            if (!string.IsNullOrEmpty(query.VoucherName))
            {
                sql += " AND VoucherTypeName LIKE @VoucherName";
                parameters.Add("VoucherName", $"%{query.VoucherName}%");
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "VoucherTypeId" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<VoucherType>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM VoucherTypes
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.VoucherId))
            {
                countSql += " AND VoucherTypeId LIKE @VoucherId";
                countParameters.Add("VoucherId", $"%{query.VoucherId}%");
            }
            if (!string.IsNullOrEmpty(query.VoucherName))
            {
                countSql += " AND VoucherTypeName LIKE @VoucherName";
                countParameters.Add("VoucherName", $"%{query.VoucherName}%");
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND Status = @Status";
                countParameters.Add("Status", query.Status);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<VoucherType>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢傳票型態列表失敗", ex);
            throw;
        }
    }

    public async Task<VoucherType> CreateAsync(VoucherType voucherType)
    {
        try
        {
            const string sql = @"
                INSERT INTO VoucherTypes (
                    VoucherTypeId, VoucherTypeName, Status, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup
                ) VALUES (
                    @VoucherTypeId, @VoucherTypeName, @Status, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup
                );
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await QuerySingleAsync<long>(sql, new
            {
                VoucherTypeId = voucherType.VoucherId,
                VoucherTypeName = voucherType.VoucherName,
                voucherType.Status,
                voucherType.CreatedBy,
                voucherType.CreatedAt,
                voucherType.UpdatedBy,
                voucherType.UpdatedAt,
                voucherType.CreatedPriority,
                voucherType.CreatedGroup
            });

            voucherType.TKey = tKey;
            return voucherType;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增傳票型態失敗: {voucherType.VoucherId}", ex);
            throw;
        }
    }

    public async Task<VoucherType> UpdateAsync(VoucherType voucherType)
    {
        try
        {
            const string sql = @"
                UPDATE VoucherTypes SET
                    VoucherTypeName = @VoucherTypeName,
                    Status = @Status,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE VoucherTypeId = @VoucherTypeId";

            await ExecuteAsync(sql, new
            {
                VoucherTypeId = voucherType.VoucherId,
                VoucherTypeName = voucherType.VoucherName,
                voucherType.Status,
                voucherType.UpdatedBy,
                voucherType.UpdatedAt
            });

            return voucherType;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改傳票型態失敗: {voucherType.VoucherId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string voucherId)
    {
        try
        {
            const string sql = @"
                DELETE FROM VoucherTypes
                WHERE VoucherTypeId = @VoucherId";

            await ExecuteAsync(sql, new { VoucherId = voucherId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除傳票型態失敗: {voucherId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string voucherId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM VoucherTypes
                WHERE VoucherTypeId = @VoucherId";

            var count = await QuerySingleAsync<int>(sql, new { VoucherId = voucherId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查傳票型態是否存在失敗: {voucherId}", ex);
            throw;
        }
    }

    public async Task<bool> IsInUseAsync(string voucherId)
    {
        try
        {
            // 檢查是否有使用此型態的傳票或常用傳票
            const string sql = @"
                SELECT COUNT(*) FROM (
                    SELECT VoucherTypeId FROM Vouchers WHERE VoucherTypeId = @VoucherId
                    UNION
                    SELECT VoucherTypeId FROM CommonVouchers WHERE VoucherTypeId = @VoucherId
                ) AS Used";

            var count = await QuerySingleAsync<int>(sql, new { VoucherId = voucherId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查傳票型態是否使用中失敗: {voucherId}", ex);
            throw;
        }
    }
}

