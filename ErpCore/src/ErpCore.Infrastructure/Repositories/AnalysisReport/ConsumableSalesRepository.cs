using Dapper;
using ErpCore.Domain.Entities.AnalysisReport;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using System.Data;

namespace ErpCore.Infrastructure.Repositories.AnalysisReport;

/// <summary>
/// 耗材出售單 Repository 實作 (SYSA297)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class ConsumableSalesRepository : BaseRepository, IConsumableSalesRepository
{
    public ConsumableSalesRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PagedResult<ConsumableSales>> GetSalesAsync(ConsumableSalesQuery query)
    {
        try
        {
            var sql = @"
                SELECT 
                    s.TxnNo, s.Rrn, s.SiteId, s.PurchaseDate, s.Status,
                    s.TotalAmount, s.TaxAmount, s.NetAmount, s.ApplyCount, s.DetailCount,
                    s.Notes, s.CreatedBy, s.CreatedAt, s.UpdatedBy, s.UpdatedAt,
                    s.ApprovedBy, s.ApprovedAt,
                    shop.ShopName
                FROM ConsumableSales s
                LEFT JOIN Shops shop ON s.SiteId = shop.ShopId
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.TxnNo))
            {
                sql += " AND s.TxnNo LIKE @TxnNo";
                parameters.Add("TxnNo", $"%{query.TxnNo}%");
            }

            if (!string.IsNullOrEmpty(query.SiteId))
            {
                sql += " AND s.SiteId = @SiteId";
                parameters.Add("SiteId", query.SiteId);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND s.Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (query.StartDate.HasValue)
            {
                sql += " AND s.PurchaseDate >= @StartDate";
                parameters.Add("StartDate", query.StartDate.Value);
            }

            if (query.EndDate.HasValue)
            {
                sql += " AND s.PurchaseDate <= @EndDate";
                parameters.Add("EndDate", query.EndDate.Value);
            }

            // 排序
            var sortField = query.SortField ?? "PurchaseDate";
            var sortOrder = query.SortOrder ?? "DESC";
            sql += $" ORDER BY s.{sortField} {sortOrder}";

            // 查詢總數
            var countSql = sql.Replace("SELECT s.TxnNo, s.Rrn", "SELECT COUNT(*)").Split("ORDER BY")[0];
            var totalCount = await QuerySingleAsync<int>(countSql, parameters);

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            using var connection = _connectionFactory.CreateConnection();
            var items = await connection.QueryAsync<dynamic>(sql, parameters);

            var salesList = items.Select(x => new ConsumableSales
            {
                TxnNo = x.TxnNo,
                Rrn = x.Rrn,
                SiteId = x.SiteId,
                PurchaseDate = x.PurchaseDate,
                Status = x.Status,
                TotalAmount = x.TotalAmount,
                TaxAmount = x.TaxAmount,
                NetAmount = x.NetAmount,
                ApplyCount = x.ApplyCount,
                DetailCount = x.DetailCount,
                Notes = x.Notes,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt,
                ApprovedBy = x.ApprovedBy,
                ApprovedAt = x.ApprovedAt
            }).ToList();

            // 添加SiteName到扩展属性（通过Details列表存储，稍后在Service中提取）
            // 或者创建一个包含SiteName的查询结果类
            // 这里我们使用一个临时解决方案：在Service中单独查询SiteName

            return new PagedResult<ConsumableSales>
            {
                Items = salesList,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢耗材出售單列表失敗", ex);
            throw;
        }
    }

    public async Task<ConsumableSales?> GetSalesByTxnNoAsync(string txnNo)
    {
        try
        {
            var sql = @"
                SELECT 
                    s.TxnNo, s.Rrn, s.SiteId, s.PurchaseDate, s.Status,
                    s.TotalAmount, s.TaxAmount, s.NetAmount, s.ApplyCount, s.DetailCount,
                    s.Notes, s.CreatedBy, s.CreatedAt, s.UpdatedBy, s.UpdatedAt,
                    s.ApprovedBy, s.ApprovedAt
                FROM ConsumableSales s
                WHERE s.TxnNo = @TxnNo";

            var parameters = new DynamicParameters();
            parameters.Add("TxnNo", txnNo);

            using var connection = _connectionFactory.CreateConnection();
            var sales = await connection.QueryFirstOrDefaultAsync<ConsumableSales>(sql, parameters);

            if (sales != null)
            {
                // 查詢明細
                var detailSql = @"
                    SELECT 
                        d.DetailId, d.TxnNo, d.SeqNo, d.ConsumableId, d.ConsumableName,
                        d.Quantity, d.Unit, d.UnitPrice, d.Amount, d.Tax, d.TaxAmount,
                        d.NetAmount, d.PurchaseStatus, d.Notes, d.CreatedBy, d.CreatedAt
                    FROM ConsumableSalesDetails d
                    WHERE d.TxnNo = @TxnNo
                    ORDER BY d.SeqNo";

                var details = await connection.QueryAsync<ConsumableSalesDetail>(detailSql, parameters);
                sales.Details = details.ToList();
            }

            return sales;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢耗材出售單失敗: {txnNo}", ex);
            throw;
        }
    }

    public async Task<string> CreateSalesAsync(ConsumableSales sales)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                // 插入主檔
                var mainSql = @"
                    INSERT INTO ConsumableSales (
                        TxnNo, Rrn, SiteId, PurchaseDate, Status,
                        TotalAmount, TaxAmount, NetAmount, ApplyCount, DetailCount,
                        Notes, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                    ) VALUES (
                        @TxnNo, @Rrn, @SiteId, @PurchaseDate, @Status,
                        @TotalAmount, @TaxAmount, @NetAmount, @ApplyCount, @DetailCount,
                        @Notes, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                    )";

                var mainParams = new DynamicParameters();
                mainParams.Add("TxnNo", sales.TxnNo);
                mainParams.Add("Rrn", sales.Rrn);
                mainParams.Add("SiteId", sales.SiteId);
                mainParams.Add("PurchaseDate", sales.PurchaseDate);
                mainParams.Add("Status", sales.Status);
                mainParams.Add("TotalAmount", sales.TotalAmount);
                mainParams.Add("TaxAmount", sales.TaxAmount);
                mainParams.Add("NetAmount", sales.NetAmount);
                mainParams.Add("ApplyCount", sales.ApplyCount);
                mainParams.Add("DetailCount", sales.DetailCount);
                mainParams.Add("Notes", sales.Notes);
                mainParams.Add("CreatedBy", sales.CreatedBy);
                mainParams.Add("CreatedAt", sales.CreatedAt);
                mainParams.Add("UpdatedBy", sales.UpdatedBy);
                mainParams.Add("UpdatedAt", sales.UpdatedAt);

                await connection.ExecuteAsync(mainSql, mainParams, transaction);

                // 插入明細
                if (sales.Details != null && sales.Details.Any())
                {
                    var detailSql = @"
                        INSERT INTO ConsumableSalesDetails (
                            DetailId, TxnNo, SeqNo, ConsumableId, ConsumableName,
                            Quantity, Unit, UnitPrice, Amount, Tax, TaxAmount,
                            NetAmount, PurchaseStatus, Notes, CreatedBy, CreatedAt
                        ) VALUES (
                            @DetailId, @TxnNo, @SeqNo, @ConsumableId, @ConsumableName,
                            @Quantity, @Unit, @UnitPrice, @Amount, @Tax, @TaxAmount,
                            @NetAmount, @PurchaseStatus, @Notes, @CreatedBy, @CreatedAt
                        )";

                    foreach (var detail in sales.Details)
                    {
                        var detailParams = new DynamicParameters();
                        detailParams.Add("DetailId", detail.DetailId);
                        detailParams.Add("TxnNo", detail.TxnNo);
                        detailParams.Add("SeqNo", detail.SeqNo);
                        detailParams.Add("ConsumableId", detail.ConsumableId);
                        detailParams.Add("ConsumableName", detail.ConsumableName);
                        detailParams.Add("Quantity", detail.Quantity);
                        detailParams.Add("Unit", detail.Unit);
                        detailParams.Add("UnitPrice", detail.UnitPrice);
                        detailParams.Add("Amount", detail.Amount);
                        detailParams.Add("Tax", detail.Tax);
                        detailParams.Add("TaxAmount", detail.TaxAmount);
                        detailParams.Add("NetAmount", detail.NetAmount);
                        detailParams.Add("PurchaseStatus", detail.PurchaseStatus);
                        detailParams.Add("Notes", detail.Notes);
                        detailParams.Add("CreatedBy", detail.CreatedBy);
                        detailParams.Add("CreatedAt", detail.CreatedAt);

                        await connection.ExecuteAsync(detailSql, detailParams, transaction);
                    }
                }

                transaction.Commit();
                return sales.TxnNo;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增耗材出售單失敗: {sales.TxnNo}", ex);
            throw;
        }
    }

    public async Task UpdateSalesAsync(ConsumableSales sales)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                // 更新主檔
                var mainSql = @"
                    UPDATE ConsumableSales SET
                        SiteId = @SiteId,
                        PurchaseDate = @PurchaseDate,
                        Status = @Status,
                        TotalAmount = @TotalAmount,
                        TaxAmount = @TaxAmount,
                        NetAmount = @NetAmount,
                        ApplyCount = @ApplyCount,
                        DetailCount = @DetailCount,
                        Notes = @Notes,
                        UpdatedBy = @UpdatedBy,
                        UpdatedAt = @UpdatedAt
                    WHERE TxnNo = @TxnNo";

                var mainParams = new DynamicParameters();
                mainParams.Add("TxnNo", sales.TxnNo);
                mainParams.Add("SiteId", sales.SiteId);
                mainParams.Add("PurchaseDate", sales.PurchaseDate);
                mainParams.Add("Status", sales.Status);
                mainParams.Add("TotalAmount", sales.TotalAmount);
                mainParams.Add("TaxAmount", sales.TaxAmount);
                mainParams.Add("NetAmount", sales.NetAmount);
                mainParams.Add("ApplyCount", sales.ApplyCount);
                mainParams.Add("DetailCount", sales.DetailCount);
                mainParams.Add("Notes", sales.Notes);
                mainParams.Add("UpdatedBy", sales.UpdatedBy);
                mainParams.Add("UpdatedAt", sales.UpdatedAt);

                await connection.ExecuteAsync(mainSql, mainParams, transaction);

                // 刪除舊明細
                var deleteDetailSql = "DELETE FROM ConsumableSalesDetails WHERE TxnNo = @TxnNo";
                await connection.ExecuteAsync(deleteDetailSql, new { TxnNo = sales.TxnNo }, transaction);

                // 插入新明細
                if (sales.Details != null && sales.Details.Any())
                {
                    var detailSql = @"
                        INSERT INTO ConsumableSalesDetails (
                            DetailId, TxnNo, SeqNo, ConsumableId, ConsumableName,
                            Quantity, Unit, UnitPrice, Amount, Tax, TaxAmount,
                            NetAmount, PurchaseStatus, Notes, CreatedBy, CreatedAt
                        ) VALUES (
                            @DetailId, @TxnNo, @SeqNo, @ConsumableId, @ConsumableName,
                            @Quantity, @Unit, @UnitPrice, @Amount, @Tax, @TaxAmount,
                            @NetAmount, @PurchaseStatus, @Notes, @CreatedBy, @CreatedAt
                        )";

                    foreach (var detail in sales.Details)
                    {
                        var detailParams = new DynamicParameters();
                        detailParams.Add("DetailId", detail.DetailId);
                        detailParams.Add("TxnNo", detail.TxnNo);
                        detailParams.Add("SeqNo", detail.SeqNo);
                        detailParams.Add("ConsumableId", detail.ConsumableId);
                        detailParams.Add("ConsumableName", detail.ConsumableName);
                        detailParams.Add("Quantity", detail.Quantity);
                        detailParams.Add("Unit", detail.Unit);
                        detailParams.Add("UnitPrice", detail.UnitPrice);
                        detailParams.Add("Amount", detail.Amount);
                        detailParams.Add("Tax", detail.Tax);
                        detailParams.Add("TaxAmount", detail.TaxAmount);
                        detailParams.Add("NetAmount", detail.NetAmount);
                        detailParams.Add("PurchaseStatus", detail.PurchaseStatus);
                        detailParams.Add("Notes", detail.Notes);
                        detailParams.Add("CreatedBy", detail.CreatedBy);
                        detailParams.Add("CreatedAt", detail.CreatedAt);

                        await connection.ExecuteAsync(detailSql, detailParams, transaction);
                    }
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
            _logger.LogError($"更新耗材出售單失敗: {sales.TxnNo}", ex);
            throw;
        }
    }

    public async Task DeleteSalesAsync(string txnNo)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                // 刪除明細（外鍵約束會自動處理）
                var deleteDetailSql = "DELETE FROM ConsumableSalesDetails WHERE TxnNo = @TxnNo";
                await connection.ExecuteAsync(deleteDetailSql, new { TxnNo = txnNo }, transaction);

                // 刪除主檔
                var deleteMainSql = "DELETE FROM ConsumableSales WHERE TxnNo = @TxnNo";
                await connection.ExecuteAsync(deleteMainSql, new { TxnNo = txnNo }, transaction);

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
            _logger.LogError($"刪除耗材出售單失敗: {txnNo}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string txnNo)
    {
        try
        {
            var sql = "SELECT COUNT(*) FROM ConsumableSales WHERE TxnNo = @TxnNo";
            var count = await QuerySingleAsync<int>(sql, new { TxnNo = txnNo });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查交易單號是否存在失敗: {txnNo}", ex);
            throw;
        }
    }

    public async Task<string> GenerateTxnNoAsync(string siteId, DateTime date)
    {
        try
        {
            var dateStr = date.ToString("yyyyMMdd");
            var prefix = $"SEL{dateStr}{siteId}";

            var sql = @"
                SELECT ISNULL(MAX(CAST(SUBSTRING(TxnNo, LEN(@Prefix) + 1, LEN(TxnNo)) AS INT)), 0) + 1
                FROM ConsumableSales
                WHERE TxnNo LIKE @Prefix + '%'";

            var parameters = new DynamicParameters();
            parameters.Add("Prefix", prefix);

            var seqNo = await QuerySingleAsync<int>(sql, parameters);
            return $"{prefix}{seqNo:D4}";
        }
        catch (Exception ex)
        {
            _logger.LogError($"生成交易單號失敗: {siteId}, {date}", ex);
            throw;
        }
    }

    public async Task<decimal> GetInventoryQuantityAsync(string consumableId, string siteId)
    {
        try
        {
            var sql = @"
                SELECT ISNULL(Quantity, 0) - ISNULL(ReservedQuantity, 0)
                FROM ConsumableInventory
                WHERE ConsumableId = @ConsumableId AND SiteId = @SiteId";

            var parameters = new DynamicParameters();
            parameters.Add("ConsumableId", consumableId);
            parameters.Add("SiteId", siteId);

            var quantity = await QuerySingleAsync<decimal?>(sql, parameters);
            return quantity ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"取得耗材庫存數量失敗: {consumableId}, {siteId}", ex);
            throw;
        }
    }

    public async Task UpdateInventoryQuantityAsync(string consumableId, string siteId, decimal quantityChange, System.Data.IDbTransaction? transaction = null)
    {
        try
        {
            using var connection = transaction?.Connection ?? _connectionFactory.CreateConnection();
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }

            // 檢查庫存記錄是否存在
            var checkSql = "SELECT COUNT(*) FROM ConsumableInventory WHERE ConsumableId = @ConsumableId AND SiteId = @SiteId";
            var checkParams = new DynamicParameters();
            checkParams.Add("ConsumableId", consumableId);
            checkParams.Add("SiteId", siteId);

            var exists = await connection.QuerySingleAsync<int>(checkSql, checkParams, transaction);

            if (exists > 0)
            {
                // 更新現有記錄
                var updateSql = @"
                    UPDATE ConsumableInventory
                    SET Quantity = Quantity + @QuantityChange,
                        LastUpdated = GETDATE()
                    WHERE ConsumableId = @ConsumableId AND SiteId = @SiteId";

                var updateParams = new DynamicParameters();
                updateParams.Add("ConsumableId", consumableId);
                updateParams.Add("SiteId", siteId);
                updateParams.Add("QuantityChange", quantityChange);

                await connection.ExecuteAsync(updateSql, updateParams, transaction);
            }
            else
            {
                // 新增記錄
                var insertSql = @"
                    INSERT INTO ConsumableInventory (InventoryId, ConsumableId, SiteId, Quantity, ReservedQuantity, LastUpdated)
                    VALUES (NEWID(), @ConsumableId, @SiteId, @Quantity, 0, GETDATE())";

                var insertParams = new DynamicParameters();
                insertParams.Add("ConsumableId", consumableId);
                insertParams.Add("SiteId", siteId);
                insertParams.Add("Quantity", quantityChange > 0 ? quantityChange : 0);

                await connection.ExecuteAsync(insertSql, insertParams, transaction);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新耗材庫存數量失敗: {consumableId}, {siteId}, {quantityChange}", ex);
            throw;
        }
    }
}
