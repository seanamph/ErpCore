using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Inventory;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Inventory;

/// <summary>
/// 商品進銷碼 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class ProductGoodsIdRepository : BaseRepository, IProductGoodsIdRepository
{
    public ProductGoodsIdRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Product?> GetByIdAsync(string goodsId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Products 
                WHERE GoodsId = @GoodsId";

            return await QueryFirstOrDefaultAsync<Product>(sql, new { GoodsId = goodsId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢商品失敗: {goodsId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<Product>> QueryAsync(ProductGoodsIdQuery query)
    {
        try
        {
            var sql = @"
                SELECT p.* FROM Products p
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.GoodsId))
            {
                sql += " AND p.GoodsId LIKE @GoodsId";
                parameters.Add("GoodsId", $"%{query.GoodsId}%");
            }

            if (!string.IsNullOrEmpty(query.GoodsName))
            {
                sql += " AND p.GoodsName LIKE @GoodsName";
                parameters.Add("GoodsName", $"%{query.GoodsName}%");
            }

            if (!string.IsNullOrEmpty(query.BarcodeId))
            {
                sql += " AND p.BarcodeId LIKE @BarcodeId";
                parameters.Add("BarcodeId", $"%{query.BarcodeId}%");
            }

            if (!string.IsNullOrEmpty(query.ScId))
            {
                sql += " AND p.ScId = @ScId";
                parameters.Add("ScId", query.ScId);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND p.Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 如果有店別篩選，需要 JOIN ProductDetails
            if (!string.IsNullOrEmpty(query.ShopId))
            {
                sql = @"
                    SELECT DISTINCT p.* FROM Products p
                    INNER JOIN ProductDetails pd ON p.GoodsId = pd.GoodsId
                    WHERE 1=1";
                
                if (!string.IsNullOrEmpty(query.GoodsId))
                {
                    sql += " AND p.GoodsId LIKE @GoodsId";
                }
                if (!string.IsNullOrEmpty(query.GoodsName))
                {
                    sql += " AND p.GoodsName LIKE @GoodsName";
                }
                if (!string.IsNullOrEmpty(query.BarcodeId))
                {
                    sql += " AND p.BarcodeId LIKE @BarcodeId";
                }
                if (!string.IsNullOrEmpty(query.ScId))
                {
                    sql += " AND p.ScId = @ScId";
                }
                if (!string.IsNullOrEmpty(query.Status))
                {
                    sql += " AND p.Status = @Status";
                }
                sql += " AND pd.ShopId = @ShopId";
                parameters.Add("ShopId", query.ShopId);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "p.GoodsId" : $"p.{query.SortField}";
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<Product>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM Products p
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.GoodsId))
            {
                countSql += " AND p.GoodsId LIKE @GoodsId";
                countParameters.Add("GoodsId", $"%{query.GoodsId}%");
            }
            if (!string.IsNullOrEmpty(query.GoodsName))
            {
                countSql += " AND p.GoodsName LIKE @GoodsName";
                countParameters.Add("GoodsName", $"%{query.GoodsName}%");
            }
            if (!string.IsNullOrEmpty(query.BarcodeId))
            {
                countSql += " AND p.BarcodeId LIKE @BarcodeId";
                countParameters.Add("BarcodeId", $"%{query.BarcodeId}%");
            }
            if (!string.IsNullOrEmpty(query.ScId))
            {
                countSql += " AND p.ScId = @ScId";
                countParameters.Add("ScId", query.ScId);
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND p.Status = @Status";
                countParameters.Add("Status", query.Status);
            }
            if (!string.IsNullOrEmpty(query.ShopId))
            {
                countSql = @"
                    SELECT COUNT(DISTINCT p.GoodsId) FROM Products p
                    INNER JOIN ProductDetails pd ON p.GoodsId = pd.GoodsId
                    WHERE 1=1";
                if (!string.IsNullOrEmpty(query.GoodsId))
                {
                    countSql += " AND p.GoodsId LIKE @GoodsId";
                }
                if (!string.IsNullOrEmpty(query.GoodsName))
                {
                    countSql += " AND p.GoodsName LIKE @GoodsName";
                }
                if (!string.IsNullOrEmpty(query.BarcodeId))
                {
                    countSql += " AND p.BarcodeId LIKE @BarcodeId";
                }
                if (!string.IsNullOrEmpty(query.ScId))
                {
                    countSql += " AND p.ScId = @ScId";
                }
                if (!string.IsNullOrEmpty(query.Status))
                {
                    countSql += " AND p.Status = @Status";
                }
                countSql += " AND pd.ShopId = @ShopId";
                countParameters.Add("ShopId", query.ShopId);
            }

            var totalCount = await ExecuteScalarAsync<int>(countSql, countParameters);

            return new PagedResult<Product>
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

    public async Task<bool> ExistsAsync(string goodsId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM Products 
                WHERE GoodsId = @GoodsId";

            var count = await ExecuteScalarAsync<int>(sql, new { GoodsId = goodsId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查商品是否存在失敗: {goodsId}", ex);
            throw;
        }
    }

    public async Task<Product> CreateAsync(Product product)
    {
        try
        {
            const string sql = @"
                INSERT INTO Products (
                    GoodsId, GoodsName, InvPrintName, GoodsSpace, ScId, Tax, Lprc, Mprc,
                    BarcodeId, Unit, ConvertRate, Capacity, CapacityUnit, Status, Discount,
                    AutoOrder, PriceKind, CostKind, SafeDays, ExpirationDays, National, Place,
                    GoodsDeep, GoodsWide, GoodsHigh, PackDeep, PackWide, PackHigh, PackWeight,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup
                ) VALUES (
                    @GoodsId, @GoodsName, @InvPrintName, @GoodsSpace, @ScId, @Tax, @Lprc, @Mprc,
                    @BarcodeId, @Unit, @ConvertRate, @Capacity, @CapacityUnit, @Status, @Discount,
                    @AutoOrder, @PriceKind, @CostKind, @SafeDays, @ExpirationDays, @National, @Place,
                    @GoodsDeep, @GoodsWide, @GoodsHigh, @PackDeep, @PackWide, @PackHigh, @PackWeight,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup
                )";

            product.CreatedAt = DateTime.Now;
            product.UpdatedAt = DateTime.Now;

            await ExecuteAsync(sql, product);
            return product;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增商品失敗: {product.GoodsId}", ex);
            throw;
        }
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        try
        {
            const string sql = @"
                UPDATE Products SET
                    GoodsName = @GoodsName,
                    InvPrintName = @InvPrintName,
                    GoodsSpace = @GoodsSpace,
                    ScId = @ScId,
                    Tax = @Tax,
                    Lprc = @Lprc,
                    Mprc = @Mprc,
                    BarcodeId = @BarcodeId,
                    Unit = @Unit,
                    ConvertRate = @ConvertRate,
                    Capacity = @Capacity,
                    CapacityUnit = @CapacityUnit,
                    Status = @Status,
                    Discount = @Discount,
                    AutoOrder = @AutoOrder,
                    PriceKind = @PriceKind,
                    CostKind = @CostKind,
                    SafeDays = @SafeDays,
                    ExpirationDays = @ExpirationDays,
                    National = @National,
                    Place = @Place,
                    GoodsDeep = @GoodsDeep,
                    GoodsWide = @GoodsWide,
                    GoodsHigh = @GoodsHigh,
                    PackDeep = @PackDeep,
                    PackWide = @PackWide,
                    PackHigh = @PackHigh,
                    PackWeight = @PackWeight,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE GoodsId = @GoodsId";

            product.UpdatedAt = DateTime.Now;

            await ExecuteAsync(sql, product);
            return product;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改商品失敗: {product.GoodsId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string goodsId)
    {
        try
        {
            const string sql = @"
                DELETE FROM Products 
                WHERE GoodsId = @GoodsId";

            await ExecuteAsync(sql, new { GoodsId = goodsId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除商品失敗: {goodsId}", ex);
            throw;
        }
    }

    public async Task BatchDeleteAsync(List<string> goodsIds)
    {
        try
        {
            if (goodsIds == null || goodsIds.Count == 0)
            {
                return;
            }

            using var connection = _connectionFactory.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                const string sql = @"
                    DELETE FROM Products 
                    WHERE GoodsId = @GoodsId";

                foreach (var goodsId in goodsIds)
                {
                    await connection.ExecuteAsync(sql, new { GoodsId = goodsId }, transaction);
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"批次刪除商品失敗", ex);
            throw;
        }
    }
}

