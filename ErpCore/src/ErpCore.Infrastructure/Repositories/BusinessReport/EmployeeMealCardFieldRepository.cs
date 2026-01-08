using Dapper;
using ErpCore.Domain.Entities.BusinessReport;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.BusinessReport;

/// <summary>
/// 員餐卡欄位 Repository 實作 (SYSL206/SYSL207)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class EmployeeMealCardFieldRepository : BaseRepository, IEmployeeMealCardFieldRepository
{
    public EmployeeMealCardFieldRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PagedResult<EmployeeMealCardField>> QueryAsync(EmployeeMealCardFieldQuery query)
    {
        try
        {
            var sql = @"
                SELECT 
                    emcf.TKey,
                    emcf.FieldId,
                    emcf.FieldName,
                    emcf.CardType,
                    ct.CardName AS CardTypeName,
                    emcf.ActionType,
                    at.ActionName AS ActionTypeName,
                    emcf.OtherType,
                    ot.OtherName AS OtherTypeName,
                    emcf.MustKeyinYn,
                    emcf.ReadonlyYn,
                    emcf.BtnEtekYn,
                    emcf.SeqNo,
                    emcf.Status,
                    emcf.Notes,
                    emcf.CreatedBy,
                    emcf.CreatedAt,
                    emcf.UpdatedBy,
                    emcf.UpdatedAt
                FROM EmployeeMealCardFields emcf
                LEFT JOIN CardType ct ON emcf.CardType = ct.CardId
                LEFT JOIN ActionType at ON emcf.ActionType = at.ActionId
                LEFT JOIN OtherType ot ON emcf.OtherType = ot.OtherId AND emcf.ActionType = ot.ActionId
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.FieldId))
            {
                sql += " AND emcf.FieldId LIKE @FieldId";
                parameters.Add("FieldId", $"%{query.FieldId}%");
            }

            if (!string.IsNullOrEmpty(query.FieldName))
            {
                sql += " AND emcf.FieldName LIKE @FieldName";
                parameters.Add("FieldName", $"%{query.FieldName}%");
            }

            if (!string.IsNullOrEmpty(query.CardType))
            {
                sql += " AND emcf.CardType = @CardType";
                parameters.Add("CardType", query.CardType);
            }

            if (!string.IsNullOrEmpty(query.ActionType))
            {
                sql += " AND emcf.ActionType = @ActionType";
                parameters.Add("ActionType", query.ActionType);
            }

            if (!string.IsNullOrEmpty(query.OtherType))
            {
                sql += " AND emcf.OtherType = @OtherType";
                parameters.Add("OtherType", query.OtherType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND emcf.Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 計算總筆數
            var countSql = @"
                SELECT COUNT(*) 
                FROM EmployeeMealCardFields emcf
                WHERE 1=1";
            if (!string.IsNullOrEmpty(query.FieldId))
            {
                countSql += " AND emcf.FieldId LIKE @FieldId";
            }
            if (!string.IsNullOrEmpty(query.FieldName))
            {
                countSql += " AND emcf.FieldName LIKE @FieldName";
            }
            if (!string.IsNullOrEmpty(query.CardType))
            {
                countSql += " AND emcf.CardType = @CardType";
            }
            if (!string.IsNullOrEmpty(query.ActionType))
            {
                countSql += " AND emcf.ActionType = @ActionType";
            }
            if (!string.IsNullOrEmpty(query.OtherType))
            {
                countSql += " AND emcf.OtherType = @OtherType";
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND emcf.Status = @Status";
            }
            var totalCount = await ExecuteScalarAsync<int>(countSql, parameters);

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "TKey" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY emcf.{sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<EmployeeMealCardField>(sql, parameters);

            return new PagedResult<EmployeeMealCardField>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢員餐卡欄位列表失敗", ex);
            throw;
        }
    }

    public async Task<EmployeeMealCardField?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT 
                    emcf.TKey,
                    emcf.FieldId,
                    emcf.FieldName,
                    emcf.CardType,
                    emcf.ActionType,
                    emcf.OtherType,
                    emcf.MustKeyinYn,
                    emcf.ReadonlyYn,
                    emcf.BtnEtekYn,
                    emcf.SeqNo,
                    emcf.Status,
                    emcf.Notes,
                    emcf.CreatedBy,
                    emcf.CreatedAt,
                    emcf.UpdatedBy,
                    emcf.UpdatedAt
                FROM EmployeeMealCardFields emcf
                WHERE emcf.TKey = @TKey";

            return await QueryFirstOrDefaultAsync<EmployeeMealCardField>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢員餐卡欄位失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<EmployeeMealCardField?> GetByFieldIdAsync(string fieldId)
    {
        try
        {
            const string sql = @"
                SELECT 
                    emcf.TKey,
                    emcf.FieldId,
                    emcf.FieldName,
                    emcf.CardType,
                    emcf.ActionType,
                    emcf.OtherType,
                    emcf.MustKeyinYn,
                    emcf.ReadonlyYn,
                    emcf.BtnEtekYn,
                    emcf.SeqNo,
                    emcf.Status,
                    emcf.Notes,
                    emcf.CreatedBy,
                    emcf.CreatedAt,
                    emcf.UpdatedBy,
                    emcf.UpdatedAt
                FROM EmployeeMealCardFields emcf
                WHERE emcf.FieldId = @FieldId";

            return await QueryFirstOrDefaultAsync<EmployeeMealCardField>(sql, new { FieldId = fieldId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢員餐卡欄位失敗: {fieldId}", ex);
            throw;
        }
    }

    public async Task<EmployeeMealCardField?> GetPreviousAsync(string fieldId)
    {
        try
        {
            const string sql = @"
                SELECT TOP 1
                    emcf.TKey,
                    emcf.FieldId,
                    emcf.FieldName,
                    emcf.CardType,
                    emcf.ActionType,
                    emcf.OtherType,
                    emcf.MustKeyinYn,
                    emcf.ReadonlyYn,
                    emcf.BtnEtekYn,
                    emcf.SeqNo,
                    emcf.Status,
                    emcf.Notes,
                    emcf.CreatedBy,
                    emcf.CreatedAt,
                    emcf.UpdatedBy,
                    emcf.UpdatedAt
                FROM EmployeeMealCardFields emcf
                WHERE emcf.FieldId < @FieldId
                ORDER BY emcf.FieldId DESC";

            return await QueryFirstOrDefaultAsync<EmployeeMealCardField>(sql, new { FieldId = fieldId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢上一筆員餐卡欄位失敗: {fieldId}", ex);
            throw;
        }
    }

    public async Task<long> CreateAsync(EmployeeMealCardField entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO EmployeeMealCardFields 
                (FieldId, FieldName, CardType, ActionType, OtherType, MustKeyinYn, ReadonlyYn, BtnEtekYn, SeqNo, Status, Notes, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@FieldId, @FieldName, @CardType, @ActionType, @OtherType, @MustKeyinYn, @ReadonlyYn, @BtnEtekYn, @SeqNo, @Status, @Notes, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, entity);
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增員餐卡欄位失敗", ex);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(EmployeeMealCardField entity)
    {
        try
        {
            const string sql = @"
                UPDATE EmployeeMealCardFields SET
                    FieldName = @FieldName,
                    CardType = @CardType,
                    ActionType = @ActionType,
                    OtherType = @OtherType,
                    MustKeyinYn = @MustKeyinYn,
                    ReadonlyYn = @ReadonlyYn,
                    BtnEtekYn = @BtnEtekYn,
                    SeqNo = @SeqNo,
                    Status = @Status,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            var affectedRows = await ExecuteAsync(sql, entity);
            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改員餐卡欄位失敗: {entity.TKey}", ex);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(long tKey)
    {
        try
        {
            const string sql = "DELETE FROM EmployeeMealCardFields WHERE TKey = @TKey";
            var affectedRows = await ExecuteAsync(sql, new { TKey = tKey });
            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除員餐卡欄位失敗: {tKey}", ex);
            throw;
        }
    }
}

