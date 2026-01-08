using Dapper;
using ErpCore.Domain.Entities.SystemConfig;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.SystemConfig;

/// <summary>
/// 系統功能按鈕 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class ConfigButtonRepository : BaseRepository, IConfigButtonRepository
{
    public ConfigButtonRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<ConfigButton?> GetByIdAsync(string buttonId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM ConfigButtons 
                WHERE ButtonId = @ButtonId";

            return await QueryFirstOrDefaultAsync<ConfigButton>(sql, new { ButtonId = buttonId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢按鈕失敗: {buttonId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<ConfigButton>> QueryAsync(ConfigButtonQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM ConfigButtons
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ButtonId))
            {
                sql += " AND ButtonId LIKE @ButtonId";
                parameters.Add("ButtonId", $"%{query.ButtonId}%");
            }

            if (!string.IsNullOrEmpty(query.ButtonName))
            {
                sql += " AND ButtonName LIKE @ButtonName";
                parameters.Add("ButtonName", $"%{query.ButtonName}%");
            }

            if (!string.IsNullOrEmpty(query.ProgramId))
            {
                sql += " AND ProgramId = @ProgramId";
                parameters.Add("ProgramId", query.ProgramId);
            }

            if (!string.IsNullOrEmpty(query.ButtonType))
            {
                sql += " AND ButtonType = @ButtonType";
                parameters.Add("ButtonType", query.ButtonType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "SeqNo, ButtonId" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<ConfigButton>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM ConfigButtons
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.ButtonId))
            {
                countSql += " AND ButtonId LIKE @ButtonId";
                countParameters.Add("ButtonId", $"%{query.ButtonId}%");
            }
            if (!string.IsNullOrEmpty(query.ButtonName))
            {
                countSql += " AND ButtonName LIKE @ButtonName";
                countParameters.Add("ButtonName", $"%{query.ButtonName}%");
            }
            if (!string.IsNullOrEmpty(query.ProgramId))
            {
                countSql += " AND ProgramId = @ProgramId";
                countParameters.Add("ProgramId", query.ProgramId);
            }
            if (!string.IsNullOrEmpty(query.ButtonType))
            {
                countSql += " AND ButtonType = @ButtonType";
                countParameters.Add("ButtonType", query.ButtonType);
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND Status = @Status";
                countParameters.Add("Status", query.Status);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<ConfigButton>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢按鈕列表失敗", ex);
            throw;
        }
    }

    public async Task<ConfigButton> CreateAsync(ConfigButton configButton)
    {
        try
        {
            const string sql = @"
                INSERT INTO ConfigButtons (
                    ButtonId, ProgramId, ButtonName, ButtonType, SeqNo, Status,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                )
                OUTPUT INSERTED.*
                VALUES (
                    @ButtonId, @ProgramId, @ButtonName, @ButtonType, @SeqNo, @Status,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                )";

            var result = await QueryFirstOrDefaultAsync<ConfigButton>(sql, configButton);
            if (result == null)
            {
                throw new InvalidOperationException("新增按鈕失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增按鈕失敗: {configButton.ButtonId}", ex);
            throw;
        }
    }

    public async Task<ConfigButton> UpdateAsync(ConfigButton configButton)
    {
        try
        {
            const string sql = @"
                UPDATE ConfigButtons SET
                    ProgramId = @ProgramId,
                    ButtonName = @ButtonName,
                    ButtonType = @ButtonType,
                    SeqNo = @SeqNo,
                    Status = @Status,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                OUTPUT INSERTED.*
                WHERE ButtonId = @ButtonId";

            var result = await QueryFirstOrDefaultAsync<ConfigButton>(sql, configButton);
            if (result == null)
            {
                throw new InvalidOperationException($"按鈕不存在: {configButton.ButtonId}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改按鈕失敗: {configButton.ButtonId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string buttonId)
    {
        try
        {
            const string sql = @"
                DELETE FROM ConfigButtons 
                WHERE ButtonId = @ButtonId";

            await ExecuteAsync(sql, new { ButtonId = buttonId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除按鈕失敗: {buttonId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string buttonId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM ConfigButtons 
                WHERE ButtonId = @ButtonId";

            var count = await QuerySingleAsync<int>(sql, new { ButtonId = buttonId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查按鈕是否存在失敗: {buttonId}", ex);
            throw;
        }
    }
}

