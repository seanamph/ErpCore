using System.Data;
using Dapper;
using ErpCore.Domain.Entities.ChartTools;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.ChartTools;

/// <summary>
/// 圖表配置 Repository 實作
/// </summary>
public class ChartConfigRepository : BaseRepository, IChartConfigRepository
{
    public ChartConfigRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<ChartConfig?> GetByIdAsync(Guid chartConfigId)
    {
        try
        {
            const string sql = @"SELECT * FROM ChartConfigs WHERE ChartConfigId = @ChartConfigId";
            return await QueryFirstOrDefaultAsync<ChartConfig>(sql, new { ChartConfigId = chartConfigId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢圖表配置失敗: {chartConfigId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<ChartConfig>> QueryAsync(ChartConfigQuery query)
    {
        try
        {
            var sql = @"SELECT * FROM ChartConfigs WHERE 1=1";
            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ChartName))
            {
                sql += " AND ChartName LIKE @ChartName";
                parameters.Add("ChartName", $"%{query.ChartName}%");
            }

            if (!string.IsNullOrEmpty(query.ChartType))
            {
                sql += " AND ChartType = @ChartType";
                parameters.Add("ChartType", query.ChartType);
            }

            sql += " ORDER BY CreatedAt DESC";
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<ChartConfig>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢圖表配置列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(ChartConfigQuery query)
    {
        try
        {
            var sql = @"SELECT COUNT(*) FROM ChartConfigs WHERE 1=1";
            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ChartName))
            {
                sql += " AND ChartName LIKE @ChartName";
                parameters.Add("ChartName", $"%{query.ChartName}%");
            }

            if (!string.IsNullOrEmpty(query.ChartType))
            {
                sql += " AND ChartType = @ChartType";
                parameters.Add("ChartType", query.ChartType);
            }

            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢圖表配置數量失敗", ex);
            throw;
        }
    }

    public async Task<Guid> CreateAsync(ChartConfig entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO ChartConfigs 
                (ChartConfigId, ChartName, ChartType, DataSource, XField, YField, Title, XAxisTitle, YAxisTitle, Width, Height, Colors, Options, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@ChartConfigId, @ChartName, @ChartType, @DataSource, @XField, @YField, @Title, @XAxisTitle, @YAxisTitle, @Width, @Height, @Colors, @Options, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt)";

            if (entity.ChartConfigId == Guid.Empty)
            {
                entity.ChartConfigId = Guid.NewGuid();
            }

            await ExecuteAsync(sql, entity);
            return entity.ChartConfigId;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增圖表配置失敗", ex);
            throw;
        }
    }

    public async Task UpdateAsync(ChartConfig entity)
    {
        try
        {
            const string sql = @"
                UPDATE ChartConfigs 
                SET ChartName = @ChartName, ChartType = @ChartType, DataSource = @DataSource, 
                    XField = @XField, YField = @YField, Title = @Title, XAxisTitle = @XAxisTitle, 
                    YAxisTitle = @YAxisTitle, Width = @Width, Height = @Height, Colors = @Colors, 
                    Options = @Options, UpdatedBy = @UpdatedBy, UpdatedAt = @UpdatedAt
                WHERE ChartConfigId = @ChartConfigId";

            await ExecuteAsync(sql, entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改圖表配置失敗: {entity.ChartConfigId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(Guid chartConfigId)
    {
        try
        {
            const string sql = @"DELETE FROM ChartConfigs WHERE ChartConfigId = @ChartConfigId";
            await ExecuteAsync(sql, new { ChartConfigId = chartConfigId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除圖表配置失敗: {chartConfigId}", ex);
            throw;
        }
    }
}

