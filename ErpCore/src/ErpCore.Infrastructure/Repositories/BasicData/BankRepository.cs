using Dapper;
using ErpCore.Domain.Entities.BasicData;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.BasicData;

/// <summary>
/// 銀行 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class BankRepository : BaseRepository, IBankRepository
{
    public BankRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Bank?> GetByIdAsync(string bankId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Banks 
                WHERE BankId = @BankId";

            return await QueryFirstOrDefaultAsync<Bank>(sql, new { BankId = bankId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢銀行失敗: {bankId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<Bank>> QueryAsync(BankQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Banks
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.BankId))
            {
                sql += " AND BankId LIKE @BankId";
                parameters.Add("BankId", $"%{query.BankId}%");
            }

            if (!string.IsNullOrEmpty(query.BankName))
            {
                sql += " AND BankName LIKE @BankName";
                parameters.Add("BankName", $"%{query.BankName}%");
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (!string.IsNullOrEmpty(query.BankKind))
            {
                sql += " AND BankKind = @BankKind";
                parameters.Add("BankKind", query.BankKind);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "BankId" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<Bank>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM Banks
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.BankId))
            {
                countSql += " AND BankId LIKE @BankId";
                countParameters.Add("BankId", $"%{query.BankId}%");
            }
            if (!string.IsNullOrEmpty(query.BankName))
            {
                countSql += " AND BankName LIKE @BankName";
                countParameters.Add("BankName", $"%{query.BankName}%");
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND Status = @Status";
                countParameters.Add("Status", query.Status);
            }
            if (!string.IsNullOrEmpty(query.BankKind))
            {
                countSql += " AND BankKind = @BankKind";
                countParameters.Add("BankKind", query.BankKind);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<Bank>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢銀行列表失敗", ex);
            throw;
        }
    }

    public async Task<Bank> CreateAsync(Bank bank)
    {
        try
        {
            const string sql = @"
                INSERT INTO Banks (
                    BankId, BankName, AcctLen, AcctLenMax, Status, BankKind, SeqNo, Notes,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup
                )
                OUTPUT INSERTED.*
                VALUES (
                    @BankId, @BankName, @AcctLen, @AcctLenMax, @Status, @BankKind, @SeqNo, @Notes,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup
                )";

            var result = await QueryFirstOrDefaultAsync<Bank>(sql, bank);
            if (result == null)
            {
                throw new InvalidOperationException("新增銀行失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增銀行失敗: {bank.BankId}", ex);
            throw;
        }
    }

    public async Task<Bank> UpdateAsync(Bank bank)
    {
        try
        {
            const string sql = @"
                UPDATE Banks SET
                    BankName = @BankName,
                    AcctLen = @AcctLen,
                    AcctLenMax = @AcctLenMax,
                    Status = @Status,
                    BankKind = @BankKind,
                    SeqNo = @SeqNo,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                OUTPUT INSERTED.*
                WHERE BankId = @BankId";

            var result = await QueryFirstOrDefaultAsync<Bank>(sql, bank);
            if (result == null)
            {
                throw new InvalidOperationException($"銀行不存在: {bank.BankId}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改銀行失敗: {bank.BankId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string bankId)
    {
        try
        {
            const string sql = @"
                DELETE FROM Banks 
                WHERE BankId = @BankId";

            await ExecuteAsync(sql, new { BankId = bankId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除銀行失敗: {bankId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string bankId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM Banks 
                WHERE BankId = @BankId";

            var count = await QuerySingleAsync<int>(sql, new { BankId = bankId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查銀行是否存在失敗: {bankId}", ex);
            throw;
        }
    }
}
