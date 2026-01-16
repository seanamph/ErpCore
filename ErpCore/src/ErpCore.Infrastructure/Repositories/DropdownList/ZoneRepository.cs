using Dapper;
using ErpCore.Domain.Entities.DropdownList;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Infrastructure.Repositories.Queries;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.DropdownList;

/// <summary>
/// 區域 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class ZoneRepository : BaseRepository, IZoneRepository
{
    public ZoneRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Zone?> GetByIdAsync(string zoneId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Zones 
                WHERE ZoneId = @ZoneId";

            return await QueryFirstOrDefaultAsync<Zone>(sql, new { ZoneId = zoneId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢區域失敗: {zoneId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<Zone>> QueryAsync(ZoneQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Zones
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ZoneName))
            {
                sql += " AND ZoneName LIKE @ZoneName";
                parameters.Add("ZoneName", $"%{query.ZoneName}%");
            }

            if (!string.IsNullOrEmpty(query.CityId))
            {
                sql += " AND CityId = @CityId";
                parameters.Add("CityId", query.CityId);
            }

            if (!string.IsNullOrEmpty(query.ZipCode))
            {
                sql += " AND ZipCode LIKE @ZipCode";
                parameters.Add("ZipCode", $"%{query.ZipCode}%");
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 排序
            var sortField = query.SortField ?? "ZoneName";
            var sortOrder = query.SortOrder ?? "ASC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            using var connection = _connectionFactory.CreateConnection();

            // 查詢總筆數
            var countSql = sql.Replace("SELECT *", "SELECT COUNT(*)").Split("ORDER BY")[0];
            var totalCount = await connection.QuerySingleAsync<int>(countSql, parameters);

            // 查詢資料
            var items = await connection.QueryAsync<Zone>(sql, parameters);

            return new PagedResult<Zone>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢區域列表失敗", ex);
            throw;
        }
    }

    public async Task<IEnumerable<ZoneOption>> GetOptionsAsync(string? cityId = null, string? status = "1")
    {
        try
        {
            var sql = @"
                SELECT ZoneId AS Value, ZoneName AS Label, ZipCode
                FROM Zones
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(cityId))
            {
                sql += " AND CityId = @CityId";
                parameters.Add("CityId", cityId);
            }

            if (!string.IsNullOrEmpty(status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", status);
            }

            sql += " ORDER BY SeqNo, ZoneName";

            return await QueryAsync<ZoneOption>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢區域選項失敗", ex);
            throw;
        }
    }

    public async Task<IEnumerable<Zone>> GetByCityIdAsync(string cityId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Zones 
                WHERE CityId = @CityId 
                AND Status = '1'
                ORDER BY SeqNo, ZoneName";

            return await QueryAsync<Zone>(sql, new { CityId = cityId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢城市區域失敗: {cityId}", ex);
            throw;
        }
    }

    public async Task<bool> CreateAsync(Zone zone)
    {
        try
        {
            const string sql = @"
                INSERT INTO Zones (ZoneId, ZoneName, CityId, ZipCode, SeqNo, Status, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES (@ZoneId, @ZoneName, @CityId, @ZipCode, @SeqNo, @Status, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt)";

            var affectedRows = await ExecuteAsync(sql, new
            {
                zone.ZoneId,
                zone.ZoneName,
                zone.CityId,
                zone.ZipCode,
                zone.SeqNo,
                zone.Status,
                zone.CreatedBy,
                zone.CreatedAt,
                zone.UpdatedBy,
                zone.UpdatedAt
            });

            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增區域失敗: {zone.ZoneId}", ex);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(Zone zone)
    {
        try
        {
            const string sql = @"
                UPDATE Zones 
                SET ZoneName = @ZoneName, 
                    CityId = @CityId, 
                    ZipCode = @ZipCode, 
                    SeqNo = @SeqNo, 
                    Status = @Status, 
                    UpdatedBy = @UpdatedBy, 
                    UpdatedAt = @UpdatedAt
                WHERE ZoneId = @ZoneId";

            var affectedRows = await ExecuteAsync(sql, new
            {
                zone.ZoneId,
                zone.ZoneName,
                zone.CityId,
                zone.ZipCode,
                zone.SeqNo,
                zone.Status,
                zone.UpdatedBy,
                zone.UpdatedAt
            });

            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新區域失敗: {zone.ZoneId}", ex);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(string zoneId)
    {
        try
        {
            const string sql = @"
                DELETE FROM Zones 
                WHERE ZoneId = @ZoneId";

            var affectedRows = await ExecuteAsync(sql, new { ZoneId = zoneId });

            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除區域失敗: {zoneId}", ex);
            throw;
        }
    }
}

