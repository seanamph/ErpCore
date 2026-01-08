using ErpCore.Application.DTOs.Inventory;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Inventory;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.Inventory;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using Dapper;

namespace ErpCore.Application.Services.Inventory;

/// <summary>
/// 商品服務實作
/// </summary>
public class ProductService : BaseService, IProductService
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ProductService(
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<ProductDto>> GetProductsAsync(ProductQueryDto query)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"
                SELECT 
                    GoodsId, GoodsName, Lprc, Mprc, BarcodeId, Unit, Status
                FROM Products
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(query.GoodsId))
            {
                sql += " AND GoodsId LIKE @GoodsId";
                parameters.Add("GoodsId", $"%{query.GoodsId}%");
            }

            if (!string.IsNullOrWhiteSpace(query.GoodsName))
            {
                sql += " AND GoodsName LIKE @GoodsName";
                parameters.Add("GoodsName", $"%{query.GoodsName}%");
            }

            if (!string.IsNullOrWhiteSpace(query.BarcodeId))
            {
                sql += " AND BarcodeId = @BarcodeId";
                parameters.Add("BarcodeId", query.BarcodeId);
            }

            if (!string.IsNullOrWhiteSpace(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " ORDER BY GoodsId OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await connection.QueryAsync<ProductDto>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM Products WHERE 1=1";
            var countParameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(query.GoodsId))
            {
                countSql += " AND GoodsId LIKE @GoodsId";
                countParameters.Add("GoodsId", $"%{query.GoodsId}%");
            }

            if (!string.IsNullOrWhiteSpace(query.GoodsName))
            {
                countSql += " AND GoodsName LIKE @GoodsName";
                countParameters.Add("GoodsName", $"%{query.GoodsName}%");
            }

            if (!string.IsNullOrWhiteSpace(query.BarcodeId))
            {
                countSql += " AND BarcodeId = @BarcodeId";
                countParameters.Add("BarcodeId", query.BarcodeId);
            }

            if (!string.IsNullOrWhiteSpace(query.Status))
            {
                countSql += " AND Status = @Status";
                countParameters.Add("Status", query.Status);
            }

            var totalCount = await connection.ExecuteScalarAsync<int>(countSql, countParameters);

            return new PagedResult<ProductDto>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢商品列表失敗", ex);
            throw;
        }
    }

    public async Task<ProductDto?> GetProductByIdAsync(string goodsId)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = @"
                SELECT 
                    GoodsId, GoodsName, Lprc, Mprc, BarcodeId, Unit, Status
                FROM Products
                WHERE GoodsId = @GoodsId";

            var product = await connection.QueryFirstOrDefaultAsync<ProductDto>(sql, new { GoodsId = goodsId });
            return product;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢商品資訊失敗: {goodsId}", ex);
            throw;
        }
    }

    public async Task<string?> GetProductNameAsync(string goodsId)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = @"
                SELECT GoodsName
                FROM Products
                WHERE GoodsId = @GoodsId";

            var goodsName = await connection.QueryFirstOrDefaultAsync<string>(sql, new { GoodsId = goodsId });
            return goodsName;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢商品名稱失敗: {goodsId}", ex);
            throw;
        }
    }

    public async Task<List<ProductDto>> GetProductsByNameAsync(string goodsName, int maxCount = 50)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"
                SELECT TOP (@MaxCount)
                    GoodsId, GoodsName, Lprc, Mprc, BarcodeId, Unit, Status
                FROM Products
                WHERE GoodsName LIKE @GoodsName
                    AND Status = '1'
                ORDER BY GoodsId";

            var parameters = new DynamicParameters();
            parameters.Add("GoodsName", $"%{goodsName}%");
            parameters.Add("MaxCount", maxCount);

            var items = await connection.QueryAsync<ProductDto>(sql, parameters);
            return items.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"根據商品名稱查詢商品列表失敗: {goodsName}", ex);
            throw;
        }
    }
}

