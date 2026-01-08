using ErpCore.Application.DTOs.BasicData;
using ErpCore.Application.Services.Base;
using ErpCore.Infrastructure.Data;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using Dapper;

namespace ErpCore.Application.Services.BasicData;

/// <summary>
/// 店舖服務實作
/// </summary>
public class ShopService : BaseService, IShopService
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ShopService(
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<ShopDto>> GetShopsAsync(ShopQueryDto? query = null)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();

            // 注意：這裡假設店舖表名為 Shops，實際應根據資料庫結構調整
            var sql = @"
                SELECT 
                    ShopId, ShopName, Status
                FROM Shops
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (query != null)
            {
                if (!string.IsNullOrWhiteSpace(query.ShopId))
                {
                    sql += " AND ShopId LIKE @ShopId";
                    parameters.Add("ShopId", $"%{query.ShopId}%");
                }

                if (!string.IsNullOrWhiteSpace(query.ShopName))
                {
                    sql += " AND ShopName LIKE @ShopName";
                    parameters.Add("ShopName", $"%{query.ShopName}%");
                }

                if (!string.IsNullOrWhiteSpace(query.Status))
                {
                    sql += " AND Status = @Status";
                    parameters.Add("Status", query.Status);
                }
            }

            sql += " ORDER BY ShopId";

            var shops = await connection.QueryAsync<ShopDto>(sql, parameters);
            return shops.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢店舖列表失敗", ex);
            throw;
        }
    }
}

