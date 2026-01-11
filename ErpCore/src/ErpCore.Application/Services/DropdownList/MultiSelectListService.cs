using Dapper;
using ErpCore.Application.DTOs.DropdownList;
using ErpCore.Application.Services.Base;
using ErpCore.Infrastructure.Data;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.DropdownList;

/// <summary>
/// 多選列表服務實作
/// </summary>
public class MultiSelectListService : BaseService, IMultiSelectListService
{
    private readonly IDbConnectionFactory _connectionFactory;

    public MultiSelectListService(
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<MultiAreaDto>> GetMultiAreasAsync(MultiAreaQueryDto query)
    {
        try
        {
            _logger.LogInfo("查詢多選區域列表");
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"
                SELECT AreaId, AreaName, SeqNo, Status
                FROM Areas
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.AreaName))
            {
                sql += " AND AreaName LIKE @AreaName";
                parameters.Add("AreaName", $"%{query.AreaName}%");
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            var sortField = query.SortField ?? "AreaId";
            var sortOrder = query.SortOrder ?? "ASC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            var result = await connection.QueryAsync<MultiAreaDto>(sql, parameters);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢多選區域列表失敗", ex);
            throw;
        }
    }

    public async Task<IEnumerable<OptionDto>> GetAreaOptionsAsync(string? status = "A")
    {
        try
        {
            _logger.LogInfo("查詢區域選項");
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"
                SELECT AreaId AS Value, AreaName AS Label
                FROM Areas
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", status);
            }

            sql += " ORDER BY SeqNo, AreaId";

            var result = await connection.QueryAsync<OptionDto>(sql, parameters);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢區域選項失敗", ex);
            throw;
        }
    }

    public async Task<IEnumerable<MultiShopDto>> GetMultiShopsAsync(MultiShopQueryDto query)
    {
        try
        {
            _logger.LogInfo("查詢多選店別列表");
            using var connection = _connectionFactory.CreateConnection();

            // 使用 AreaId 映射到 RegionId（如果 Shops 表没有 RegionId 字段）
            // 同时支持 TypeId 和 ShopLevel 筛选
            var sql = @"
                SELECT s.ShopId, s.ShopName, s.Status,
                       s.AreaId AS RegionId, 
                       a.AreaName AS RegionName,
                       s.ShopType AS TypeId,
                       pt.ParameterName AS TypeName,
                       NULL AS ShopLevel,
                       NULL AS ShopLevelName
                FROM Shops s
                LEFT JOIN Areas a ON s.AreaId = a.AreaId
                LEFT JOIN Parameters pt ON pt.ParameterCode = 'SHOP_TYPE' AND pt.ParameterValue = s.ShopType
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ShopName))
            {
                sql += " AND s.ShopName LIKE @ShopName";
                parameters.Add("ShopName", $"%{query.ShopName}%");
            }

            if (!string.IsNullOrEmpty(query.RegionIds))
            {
                var regionIds = query.RegionIds.Split(',', StringSplitOptions.RemoveEmptyEntries);
                sql += " AND s.AreaId IN @RegionIds";
                parameters.Add("RegionIds", regionIds);
            }

            if (!string.IsNullOrEmpty(query.TypeIds))
            {
                var typeIds = query.TypeIds.Split(',', StringSplitOptions.RemoveEmptyEntries);
                sql += " AND s.ShopType IN @TypeIds";
                parameters.Add("TypeIds", typeIds);
            }

            if (!string.IsNullOrEmpty(query.ShopLevels))
            {
                var shopLevels = query.ShopLevels.Split(',', StringSplitOptions.RemoveEmptyEntries);
                // 如果 Shops 表有 ShopLevel 字段，使用它；否则暂时忽略
                // sql += " AND s.ShopLevel IN @ShopLevels";
                // parameters.Add("ShopLevels", shopLevels);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                // 转换状态值：'1' -> 'A', '0' -> 'I'
                var status = query.Status == "1" ? "A" : (query.Status == "0" ? "I" : query.Status);
                sql += " AND s.Status = @Status";
                parameters.Add("Status", status);
            }

            // 特殊查询模式：KIND=1 表示仅查询供应商相关店别
            if (query.Kind == "1")
            {
                // 如果有 SuppShops 表，使用它；否则暂时忽略
                // sql += " AND EXISTS (SELECT 1 FROM SuppShops ss WHERE ss.ShopId = s.ShopId)";
            }

            // 特殊查询模式：GOODS_ID 表示查询商品相关店别
            if (!string.IsNullOrEmpty(query.GoodsId))
            {
                // 如果有 GoodsD 表，使用它；否则暂时忽略
                // sql += " AND EXISTS (SELECT 1 FROM GoodsD gd WHERE gd.ShopId = s.ShopId AND gd.GoodsId = @GoodsId)";
                // parameters.Add("GoodsId", query.GoodsId);
            }

            var sortField = query.SortField ?? "ShopId";
            var sortOrder = query.SortOrder ?? "ASC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            var result = await connection.QueryAsync<MultiShopDto>(sql, parameters);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢多選店別列表失敗", ex);
            throw;
        }
    }

    public async Task<IEnumerable<OptionDto>> GetShopOptionsAsync(MultiShopQueryDto query)
    {
        try
        {
            _logger.LogInfo("查詢店別選項");
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"
                SELECT ShopId AS Value, ShopName AS Label
                FROM Shops
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.RegionIds))
            {
                var regionIds = query.RegionIds.Split(',', StringSplitOptions.RemoveEmptyEntries);
                sql += " AND AreaId IN @RegionIds";
                parameters.Add("RegionIds", regionIds);
            }

            if (!string.IsNullOrEmpty(query.TypeIds))
            {
                var typeIds = query.TypeIds.Split(',', StringSplitOptions.RemoveEmptyEntries);
                sql += " AND ShopType IN @TypeIds";
                parameters.Add("TypeIds", typeIds);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                // 转换状态值：'1' -> 'A', '0' -> 'I'
                var status = query.Status == "1" ? "A" : (query.Status == "0" ? "I" : query.Status);
                sql += " AND Status = @Status";
                parameters.Add("Status", status);
            }

            sql += " ORDER BY ShopId";

            var result = await connection.QueryAsync<OptionDto>(sql, parameters);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢店別選項失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<MultiUserDto>> GetMultiUsersAsync(MultiUserQueryDto query)
    {
        try
        {
            _logger.LogInfo("查詢多選使用者列表");
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"
                SELECT u.UserId, u.UserName, u.OrgId, u.Title, u.Status,
                       d.DepartmentName AS OrgName
                FROM Users u
                LEFT JOIN Departments d ON u.OrgId = d.DepartmentId
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.UserId))
            {
                sql += " AND u.UserId LIKE @UserId";
                parameters.Add("UserId", $"%{query.UserId}%");
            }

            if (!string.IsNullOrEmpty(query.UserName))
            {
                sql += " AND u.UserName LIKE @UserName";
                parameters.Add("UserName", $"%{query.UserName}%");
            }

            if (!string.IsNullOrEmpty(query.OrgId))
            {
                sql += " AND u.OrgId = @OrgId";
                parameters.Add("OrgId", query.OrgId);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND u.Status = @Status";
                parameters.Add("Status", query.Status);
            }

            var sortField = query.SortField ?? "UserId";
            var sortOrder = query.SortOrder ?? "ASC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 查詢總筆數
            var countSql = sql.Replace("SELECT u.UserId, u.UserName, u.OrgId, u.Title, u.Status, d.DepartmentName AS OrgName", "SELECT COUNT(*)");
            var totalCount = await connection.QuerySingleAsync<int>(countSql, parameters);

            // 分頁
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            var items = await connection.QueryAsync<MultiUserDto>(sql, parameters);

            return new PagedResult<MultiUserDto>
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
            _logger.LogError("查詢多選使用者列表失敗", ex);
            throw;
        }
    }

    public async Task<IEnumerable<OptionDto>> GetUserOptionsAsync(string? orgId = null, string? status = "A", string? keyword = null)
    {
        try
        {
            _logger.LogInfo("查詢使用者選項");
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"
                SELECT UserId AS Value, 
                       UserId + ' - ' + UserName AS Label
                FROM Users
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(orgId))
            {
                sql += " AND OrgId = @OrgId";
                parameters.Add("OrgId", orgId);
            }

            if (!string.IsNullOrEmpty(status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", status);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                sql += " AND (UserId LIKE @Keyword OR UserName LIKE @Keyword)";
                parameters.Add("Keyword", $"%{keyword}%");
            }

            sql += " ORDER BY UserId";

            var result = await connection.QueryAsync<OptionDto>(sql, parameters);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢使用者選項失敗", ex);
            throw;
        }
    }
}

