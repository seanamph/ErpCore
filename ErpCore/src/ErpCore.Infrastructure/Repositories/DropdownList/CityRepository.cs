using Dapper;
using ErpCore.Application.DTOs.DropdownList;
using ErpCore.Domain.Entities.DropdownList;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.DropdownList;

/// <summary>
/// 城市 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class CityRepository : BaseRepository, ICityRepository
{
    public CityRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<City?> GetByIdAsync(string cityId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Cities 
                WHERE CityId = @CityId";

            return await QueryFirstOrDefaultAsync<City>(sql, new { CityId = cityId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢城市失敗: {cityId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<City>> QueryAsync(CityQueryDto query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Cities
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.CityName))
            {
                sql += " AND CityName LIKE @CityName";
                parameters.Add("CityName", $"%{query.CityName}%");
            }

            if (!string.IsNullOrEmpty(query.CountryCode))
            {
                sql += " AND CountryCode = @CountryCode";
                parameters.Add("CountryCode", query.CountryCode);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 排序
            var sortField = query.SortField ?? "CityName";
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
            var items = await connection.QueryAsync<City>(sql, parameters);

            return new PagedResult<City>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢城市列表失敗", ex);
            throw;
        }
    }

    public async Task<IEnumerable<CityOptionDto>> GetOptionsAsync(string? countryCode = null, string? status = "1")
    {
        try
        {
            var sql = @"
                SELECT CityId AS Value, CityName AS Label 
                FROM Cities
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(countryCode))
            {
                sql += " AND CountryCode = @CountryCode";
                parameters.Add("CountryCode", countryCode);
            }

            if (!string.IsNullOrEmpty(status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", status);
            }

            sql += " ORDER BY SeqNo, CityName";

            return await QueryAsync<CityOptionDto>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢城市選項失敗", ex);
            throw;
        }
    }
}

