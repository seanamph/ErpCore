using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Inventory;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Inventory;

/// <summary>
/// POP列印 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class PopPrintRepository : BaseRepository, IPopPrintRepository
{
    public PopPrintRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PagedResult<PopPrintProduct>> GetProductsAsync(PopPrintProductQuery query)
    {
        try
        {
            var sql = @"
                SELECT 
                    GoodsId, GoodsName, BarCode, VendorGoodsId, LogoId, Price, Mprc, Unit, UnitName
                FROM Products
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.GoodsId))
            {
                sql += " AND GoodsId LIKE @GoodsId";
                parameters.Add("GoodsId", $"%{query.GoodsId}%");
            }

            if (!string.IsNullOrEmpty(query.GoodsName))
            {
                sql += " AND GoodsName LIKE @GoodsName";
                parameters.Add("GoodsName", $"%{query.GoodsName}%");
            }

            if (!string.IsNullOrEmpty(query.BarCode))
            {
                sql += " AND BarCode LIKE @BarCode";
                parameters.Add("BarCode", $"%{query.BarCode}%");
            }

            if (!string.IsNullOrEmpty(query.VendorGoodsId))
            {
                sql += " AND VendorGoodsId LIKE @VendorGoodsId";
                parameters.Add("VendorGoodsId", $"%{query.VendorGoodsId}%");
            }

            if (!string.IsNullOrEmpty(query.LogoId))
            {
                sql += " AND LogoId = @LogoId";
                parameters.Add("LogoId", query.LogoId);
            }

            if (!string.IsNullOrEmpty(query.BClassId))
            {
                sql += " AND BClassId = @BClassId";
                parameters.Add("BClassId", query.BClassId);
            }

            if (!string.IsNullOrEmpty(query.MClassId))
            {
                sql += " AND MClassId = @MClassId";
                parameters.Add("MClassId", query.MClassId);
            }

            if (!string.IsNullOrEmpty(query.SClassId))
            {
                sql += " AND SClassId = @SClassId";
                parameters.Add("SClassId", query.SClassId);
            }

            // 排序
            sql += " ORDER BY GoodsId";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<PopPrintProduct>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM Products
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.GoodsId))
            {
                countSql += " AND GoodsId LIKE @GoodsId";
                countParameters.Add("GoodsId", $"%{query.GoodsId}%");
            }
            if (!string.IsNullOrEmpty(query.GoodsName))
            {
                countSql += " AND GoodsName LIKE @GoodsName";
                countParameters.Add("GoodsName", $"%{query.GoodsName}%");
            }
            if (!string.IsNullOrEmpty(query.BarCode))
            {
                countSql += " AND BarCode LIKE @BarCode";
                countParameters.Add("BarCode", $"%{query.BarCode}%");
            }
            if (!string.IsNullOrEmpty(query.VendorGoodsId))
            {
                countSql += " AND VendorGoodsId LIKE @VendorGoodsId";
                countParameters.Add("VendorGoodsId", $"%{query.VendorGoodsId}%");
            }
            if (!string.IsNullOrEmpty(query.LogoId))
            {
                countSql += " AND LogoId = @LogoId";
                countParameters.Add("LogoId", query.LogoId);
            }
            if (!string.IsNullOrEmpty(query.BClassId))
            {
                countSql += " AND BClassId = @BClassId";
                countParameters.Add("BClassId", query.BClassId);
            }
            if (!string.IsNullOrEmpty(query.MClassId))
            {
                countSql += " AND MClassId = @MClassId";
                countParameters.Add("MClassId", query.MClassId);
            }
            if (!string.IsNullOrEmpty(query.SClassId))
            {
                countSql += " AND SClassId = @SClassId";
                countParameters.Add("SClassId", query.SClassId);
            }

            var totalCount = await ExecuteScalarAsync<int>(countSql, countParameters);

            return new PagedResult<PopPrintProduct>
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

    public async Task<PopPrintProduct?> GetProductByIdAsync(string goodsId)
    {
        try
        {
            const string sql = @"
                SELECT 
                    GoodsId, GoodsName, BarCode, VendorGoodsId, LogoId, Price, Mprc, Unit, UnitName
                FROM Products
                WHERE GoodsId = @GoodsId";

            return await QueryFirstOrDefaultAsync<PopPrintProduct>(sql, new { GoodsId = goodsId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢商品失敗: {goodsId}", ex);
            throw;
        }
    }

    public async Task<PopPrintSetting?> GetSettingAsync(string? shopId, string? version = null)
    {
        try
        {
            var sql = @"
                SELECT * FROM PopPrintSettings
                WHERE (ShopId = @ShopId OR (ShopId IS NULL AND @ShopId IS NULL))";
            
            var parameters = new DynamicParameters();
            parameters.Add("ShopId", shopId);
            
            if (!string.IsNullOrEmpty(version))
            {
                sql += " AND (Version = @Version OR (Version IS NULL AND @Version = 'STANDARD'))";
                parameters.Add("Version", version);
            }
            else
            {
                sql += " AND (Version IS NULL OR Version = 'STANDARD')";
            }

            return await QueryFirstOrDefaultAsync<PopPrintSetting>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢列印設定失敗: {shopId}, {version}", ex);
            throw;
        }
    }

    public async Task<PopPrintSetting> CreateOrUpdateSettingAsync(PopPrintSetting setting)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                // 檢查是否存在
                var existing = await GetSettingAsync(setting.ShopId);
                
                if (existing != null)
                {
                    // 更新
                    const string updateSql = @"
                        UPDATE PopPrintSettings SET
                            Ip = @Ip,
                            TypeId = @TypeId,
                            Version = @Version,
                            DebugMode = @DebugMode,
                            HeaderHeightPadding = @HeaderHeightPadding,
                            HeaderHeightPaddingRemain = @HeaderHeightPaddingRemain,
                            PageHeaderHeightPadding = @PageHeaderHeightPadding,
                            PagePadding = @PagePadding,
                            PageSize = @PageSize,
                            ApSpecificSettings = @ApSpecificSettings,
                            UpdatedBy = @UpdatedBy,
                            UpdatedAt = @UpdatedAt
                        WHERE SettingId = @SettingId";

                    setting.UpdatedAt = DateTime.Now;
                    await connection.ExecuteAsync(updateSql, setting, transaction);
                    setting.SettingId = existing.SettingId;
                }
                else
                {
                    // 新增
                    const string insertSql = @"
                        INSERT INTO PopPrintSettings (
                            SettingId, ShopId, Ip, TypeId, Version, DebugMode,
                            HeaderHeightPadding, HeaderHeightPaddingRemain, PageHeaderHeightPadding,
                            PagePadding, PageSize, ApSpecificSettings, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                        ) VALUES (
                            @SettingId, @ShopId, @Ip, @TypeId, @Version, @DebugMode,
                            @HeaderHeightPadding, @HeaderHeightPaddingRemain, @PageHeaderHeightPadding,
                            @PagePadding, @PageSize, @ApSpecificSettings, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                        )";

                    setting.CreatedAt = DateTime.Now;
                    setting.UpdatedAt = DateTime.Now;
                    await connection.ExecuteAsync(insertSql, setting, transaction);
                }

                transaction.Commit();
                return setting;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"建立或更新列印設定失敗: {setting.ShopId}", ex);
            throw;
        }
    }

    public async Task<PopPrintLog> CreateLogAsync(PopPrintLog log)
    {
        try
        {
            const string sql = @"
                INSERT INTO PopPrintLogs (
                    LogId, GoodsId, PrintType, PrintFormat, Version, PrintCount,
                    PrintDate, PrintedBy, ShopId
                ) VALUES (
                    @LogId, @GoodsId, @PrintType, @PrintFormat, @Version, @PrintCount,
                    @PrintDate, @PrintedBy, @ShopId
                )";

            await ExecuteAsync(sql, log);
            return log;
        }
        catch (Exception ex)
        {
            _logger.LogError($"建立列印記錄失敗: {log.GoodsId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<PopPrintLog>> GetLogsAsync(PopPrintLogQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM PopPrintLogs
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.GoodsId))
            {
                sql += " AND GoodsId LIKE @GoodsId";
                parameters.Add("GoodsId", $"%{query.GoodsId}%");
            }

            if (!string.IsNullOrEmpty(query.PrintType))
            {
                sql += " AND PrintType = @PrintType";
                parameters.Add("PrintType", query.PrintType);
            }

            if (!string.IsNullOrEmpty(query.PrintFormat))
            {
                sql += " AND PrintFormat = @PrintFormat";
                parameters.Add("PrintFormat", query.PrintFormat);
            }

            if (!string.IsNullOrEmpty(query.Version))
            {
                sql += " AND Version = @Version";
                parameters.Add("Version", query.Version);
            }

            if (!string.IsNullOrEmpty(query.ShopId))
            {
                sql += " AND ShopId = @ShopId";
                parameters.Add("ShopId", query.ShopId);
            }

            if (query.PrintDateFrom.HasValue)
            {
                sql += " AND PrintDate >= @PrintDateFrom";
                parameters.Add("PrintDateFrom", query.PrintDateFrom.Value);
            }

            if (query.PrintDateTo.HasValue)
            {
                sql += " AND PrintDate <= @PrintDateTo";
                parameters.Add("PrintDateTo", query.PrintDateTo.Value);
            }

            // 排序
            sql += " ORDER BY PrintDate DESC";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<PopPrintLog>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM PopPrintLogs
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.GoodsId))
            {
                countSql += " AND GoodsId LIKE @GoodsId";
                countParameters.Add("GoodsId", $"%{query.GoodsId}%");
            }
            if (!string.IsNullOrEmpty(query.PrintType))
            {
                countSql += " AND PrintType = @PrintType";
                countParameters.Add("PrintType", query.PrintType);
            }
            if (!string.IsNullOrEmpty(query.PrintFormat))
            {
                countSql += " AND PrintFormat = @PrintFormat";
                countParameters.Add("PrintFormat", query.PrintFormat);
            }
            if (!string.IsNullOrEmpty(query.Version))
            {
                countSql += " AND Version = @Version";
                countParameters.Add("Version", query.Version);
            }
            if (!string.IsNullOrEmpty(query.ShopId))
            {
                countSql += " AND ShopId = @ShopId";
                countParameters.Add("ShopId", query.ShopId);
            }
            if (query.PrintDateFrom.HasValue)
            {
                countSql += " AND PrintDate >= @PrintDateFrom";
                countParameters.Add("PrintDateFrom", query.PrintDateFrom.Value);
            }
            if (query.PrintDateTo.HasValue)
            {
                countSql += " AND PrintDate <= @PrintDateTo";
                countParameters.Add("PrintDateTo", query.PrintDateTo.Value);
            }

            var totalCount = await ExecuteScalarAsync<int>(countSql, countParameters);

            return new PagedResult<PopPrintLog>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢列印記錄列表失敗", ex);
            throw;
        }
    }
}

