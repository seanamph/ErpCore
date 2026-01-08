using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Inventory;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Inventory;

/// <summary>
/// 供應商商品 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class SupplierGoodsRepository : BaseRepository, ISupplierGoodsRepository
{
    public SupplierGoodsRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<SupplierGoods?> GetByIdAsync(string supplierId, string barcodeId, string shopId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM SupplierGoods 
                WHERE SupplierId = @SupplierId 
                  AND BarcodeId = @BarcodeId 
                  AND ShopId = @ShopId";

            return await QueryFirstOrDefaultAsync<SupplierGoods>(sql, new
            {
                SupplierId = supplierId,
                BarcodeId = barcodeId,
                ShopId = shopId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢供應商商品失敗: {supplierId}/{barcodeId}/{shopId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<SupplierGoods>> QueryAsync(SupplierGoodsQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM SupplierGoods 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.SupplierId))
            {
                sql += " AND SupplierId LIKE @SupplierId";
                parameters.Add("SupplierId", $"%{query.SupplierId}%");
            }

            if (!string.IsNullOrEmpty(query.BarcodeId))
            {
                sql += " AND BarcodeId LIKE @BarcodeId";
                parameters.Add("BarcodeId", $"%{query.BarcodeId}%");
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

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "SupplierId" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<SupplierGoods>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM SupplierGoods 
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.SupplierId))
            {
                countSql += " AND SupplierId LIKE @SupplierId";
                countParameters.Add("SupplierId", $"%{query.SupplierId}%");
            }
            if (!string.IsNullOrEmpty(query.BarcodeId))
            {
                countSql += " AND BarcodeId LIKE @BarcodeId";
                countParameters.Add("BarcodeId", $"%{query.BarcodeId}%");
            }
            if (!string.IsNullOrEmpty(query.ShopId))
            {
                countSql += " AND ShopId = @ShopId";
                countParameters.Add("ShopId", query.ShopId);
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND Status = @Status";
                countParameters.Add("Status", query.Status);
            }

            var totalCount = await ExecuteScalarAsync<int>(countSql, countParameters);

            return new PagedResult<SupplierGoods>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢供應商商品列表失敗", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string supplierId, string barcodeId, string shopId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM SupplierGoods 
                WHERE SupplierId = @SupplierId 
                  AND BarcodeId = @BarcodeId 
                  AND ShopId = @ShopId";

            var count = await ExecuteScalarAsync<int>(sql, new
            {
                SupplierId = supplierId,
                BarcodeId = barcodeId,
                ShopId = shopId
            });

            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查供應商商品是否存在失敗: {supplierId}/{barcodeId}/{shopId}", ex);
            throw;
        }
    }

    public async Task<SupplierGoods> CreateAsync(SupplierGoods supplierGoods)
    {
        try
        {
            const string sql = @"
                INSERT INTO SupplierGoods (
                    SupplierId, BarcodeId, ShopId, Lprc, Mprc, Tax, MinQty, MaxQty, 
                    Unit, Rate, Status, StartDate, EndDate, Slprc, ArrivalDays,
                    OrdDay1, OrdDay2, OrdDay3, OrdDay4, OrdDay5, OrdDay6, OrdDay7,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup
                ) VALUES (
                    @SupplierId, @BarcodeId, @ShopId, @Lprc, @Mprc, @Tax, @MinQty, @MaxQty,
                    @Unit, @Rate, @Status, @StartDate, @EndDate, @Slprc, @ArrivalDays,
                    @OrdDay1, @OrdDay2, @OrdDay3, @OrdDay4, @OrdDay5, @OrdDay6, @OrdDay7,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup
                )";

            await ExecuteAsync(sql, supplierGoods);

            return supplierGoods;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增供應商商品失敗: {supplierGoods.SupplierId}/{supplierGoods.BarcodeId}/{supplierGoods.ShopId}", ex);
            throw;
        }
    }

    public async Task<SupplierGoods> UpdateAsync(SupplierGoods supplierGoods)
    {
        try
        {
            const string sql = @"
                UPDATE SupplierGoods SET
                    Lprc = @Lprc,
                    Mprc = @Mprc,
                    Tax = @Tax,
                    MinQty = @MinQty,
                    MaxQty = @MaxQty,
                    Unit = @Unit,
                    Rate = @Rate,
                    Status = @Status,
                    StartDate = @StartDate,
                    EndDate = @EndDate,
                    Slprc = @Slprc,
                    ArrivalDays = @ArrivalDays,
                    OrdDay1 = @OrdDay1,
                    OrdDay2 = @OrdDay2,
                    OrdDay3 = @OrdDay3,
                    OrdDay4 = @OrdDay4,
                    OrdDay5 = @OrdDay5,
                    OrdDay6 = @OrdDay6,
                    OrdDay7 = @OrdDay7,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE SupplierId = @SupplierId 
                  AND BarcodeId = @BarcodeId 
                  AND ShopId = @ShopId";

            await ExecuteAsync(sql, supplierGoods);

            return supplierGoods;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改供應商商品失敗: {supplierGoods.SupplierId}/{supplierGoods.BarcodeId}/{supplierGoods.ShopId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string supplierId, string barcodeId, string shopId)
    {
        try
        {
            const string sql = @"
                DELETE FROM SupplierGoods 
                WHERE SupplierId = @SupplierId 
                  AND BarcodeId = @BarcodeId 
                  AND ShopId = @ShopId";

            await ExecuteAsync(sql, new
            {
                SupplierId = supplierId,
                BarcodeId = barcodeId,
                ShopId = shopId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除供應商商品失敗: {supplierId}/{barcodeId}/{shopId}", ex);
            throw;
        }
    }

    public async Task BatchDeleteAsync(List<SupplierGoodsKey> keys)
    {
        try
        {
            if (keys == null || keys.Count == 0)
            {
                return;
            }

            using var connection = _connectionFactory.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                const string sql = @"
                    DELETE FROM SupplierGoods 
                    WHERE SupplierId = @SupplierId 
                      AND BarcodeId = @BarcodeId 
                      AND ShopId = @ShopId";

                foreach (var key in keys)
                {
                    await connection.ExecuteAsync(sql, key, transaction);
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
            _logger.LogError("批次刪除供應商商品失敗", ex);
            throw;
        }
    }
}

