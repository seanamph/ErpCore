using System.Data;
using Dapper;
using ErpCore.Domain.Entities.StoreFloor;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.StoreFloor;

/// <summary>
/// POS終端 Repository 實作 (SYS6610-SYS6999 - POS資料維護)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class PosTerminalRepository : BaseRepository, IPosTerminalRepository
{
    public PosTerminalRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PosTerminal?> GetByIdAsync(string posTerminalId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM PosTerminals 
                WHERE PosTerminalId = @PosTerminalId";

            return await QueryFirstOrDefaultAsync<PosTerminal>(sql, new { PosTerminalId = posTerminalId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢POS終端失敗: {posTerminalId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<PosTerminal>> QueryAsync(PosTerminalQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM PosTerminals
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.PosTerminalId))
            {
                sql += " AND PosTerminalId LIKE @PosTerminalId";
                parameters.Add("PosTerminalId", $"%{query.PosTerminalId}%");
            }

            if (!string.IsNullOrEmpty(query.PosSystemId))
            {
                sql += " AND PosSystemId = @PosSystemId";
                parameters.Add("PosSystemId", query.PosSystemId);
            }

            if (!string.IsNullOrEmpty(query.ShopId))
            {
                sql += " AND ShopId = @ShopId";
                parameters.Add("ShopId", query.ShopId);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            var sortField = query.SortField ?? "CreatedAt";
            var sortOrder = query.SortOrder ?? "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<PosTerminal>(sql, parameters);
            var totalCount = await GetCountAsync(query);

            return new PagedResult<PosTerminal>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢POS終端列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(PosTerminalQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM PosTerminals
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.PosTerminalId))
            {
                sql += " AND PosTerminalId LIKE @PosTerminalId";
                parameters.Add("PosTerminalId", $"%{query.PosTerminalId}%");
            }

            if (!string.IsNullOrEmpty(query.PosSystemId))
            {
                sql += " AND PosSystemId = @PosSystemId";
                parameters.Add("PosSystemId", query.PosSystemId);
            }

            if (!string.IsNullOrEmpty(query.ShopId))
            {
                sql += " AND ShopId = @ShopId";
                parameters.Add("ShopId", query.ShopId);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            return await QuerySingleAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢POS終端總數失敗", ex);
            throw;
        }
    }

    public async Task<PosTerminal> CreateAsync(PosTerminal posTerminal)
    {
        try
        {
            const string sql = @"
                INSERT INTO PosTerminals (
                    PosTerminalId, PosSystemId, TerminalCode, TerminalName, ShopId, FloorId,
                    TerminalType, IpAddress, Port, Config, Status, LastSyncDate,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                )
                VALUES (
                    @PosTerminalId, @PosSystemId, @TerminalCode, @TerminalName, @ShopId, @FloorId,
                    @TerminalType, @IpAddress, @Port, @Config, @Status, @LastSyncDate,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                )";

            var parameters = new DynamicParameters();
            parameters.Add("PosTerminalId", posTerminal.PosTerminalId);
            parameters.Add("PosSystemId", posTerminal.PosSystemId);
            parameters.Add("TerminalCode", posTerminal.TerminalCode);
            parameters.Add("TerminalName", posTerminal.TerminalName);
            parameters.Add("ShopId", posTerminal.ShopId);
            parameters.Add("FloorId", posTerminal.FloorId);
            parameters.Add("TerminalType", posTerminal.TerminalType);
            parameters.Add("IpAddress", posTerminal.IpAddress);
            parameters.Add("Port", posTerminal.Port);
            parameters.Add("Config", posTerminal.Config);
            parameters.Add("Status", posTerminal.Status);
            parameters.Add("LastSyncDate", posTerminal.LastSyncDate);
            parameters.Add("CreatedBy", posTerminal.CreatedBy);
            parameters.Add("CreatedAt", posTerminal.CreatedAt);
            parameters.Add("UpdatedBy", posTerminal.UpdatedBy);
            parameters.Add("UpdatedAt", posTerminal.UpdatedAt);

            await ExecuteAsync(sql, parameters);

            return posTerminal;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增POS終端失敗: {posTerminal.PosTerminalId}", ex);
            throw;
        }
    }

    public async Task<PosTerminal> UpdateAsync(PosTerminal posTerminal)
    {
        try
        {
            const string sql = @"
                UPDATE PosTerminals SET
                    PosSystemId = @PosSystemId,
                    TerminalCode = @TerminalCode,
                    TerminalName = @TerminalName,
                    ShopId = @ShopId,
                    FloorId = @FloorId,
                    TerminalType = @TerminalType,
                    IpAddress = @IpAddress,
                    Port = @Port,
                    Config = @Config,
                    Status = @Status,
                    LastSyncDate = @LastSyncDate,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE PosTerminalId = @PosTerminalId";

            var parameters = new DynamicParameters();
            parameters.Add("PosTerminalId", posTerminal.PosTerminalId);
            parameters.Add("PosSystemId", posTerminal.PosSystemId);
            parameters.Add("TerminalCode", posTerminal.TerminalCode);
            parameters.Add("TerminalName", posTerminal.TerminalName);
            parameters.Add("ShopId", posTerminal.ShopId);
            parameters.Add("FloorId", posTerminal.FloorId);
            parameters.Add("TerminalType", posTerminal.TerminalType);
            parameters.Add("IpAddress", posTerminal.IpAddress);
            parameters.Add("Port", posTerminal.Port);
            parameters.Add("Config", posTerminal.Config);
            parameters.Add("Status", posTerminal.Status);
            parameters.Add("LastSyncDate", posTerminal.LastSyncDate);
            parameters.Add("UpdatedBy", posTerminal.UpdatedBy);
            parameters.Add("UpdatedAt", posTerminal.UpdatedAt);

            await ExecuteAsync(sql, parameters);

            return posTerminal;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改POS終端失敗: {posTerminal.PosTerminalId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string posTerminalId)
    {
        try
        {
            const string sql = @"
                DELETE FROM PosTerminals
                WHERE PosTerminalId = @PosTerminalId";

            await ExecuteAsync(sql, new { PosTerminalId = posTerminalId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除POS終端失敗: {posTerminalId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string posTerminalId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM PosTerminals
                WHERE PosTerminalId = @PosTerminalId";

            var count = await QuerySingleAsync<int>(sql, new { PosTerminalId = posTerminalId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查POS終端是否存在失敗: {posTerminalId}", ex);
            throw;
        }
    }

    public async Task UpdateLastSyncDateAsync(string posTerminalId, DateTime syncDate)
    {
        try
        {
            const string sql = @"
                UPDATE PosTerminals SET
                    LastSyncDate = @LastSyncDate,
                    UpdatedAt = GETDATE()
                WHERE PosTerminalId = @PosTerminalId";

            await ExecuteAsync(sql, new { PosTerminalId = posTerminalId, LastSyncDate = syncDate });
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新POS終端同步時間失敗: {posTerminalId}", ex);
            throw;
        }
    }
}

