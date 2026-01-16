using Dapper;
using ErpCore.Domain.Entities.EInvoice;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using System.Data;
using EInvoiceEntity = ErpCore.Domain.Entities.EInvoice.EInvoice;

namespace ErpCore.Infrastructure.Repositories.EInvoice;

/// <summary>
/// 電子發票 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class EInvoiceRepository : BaseRepository, IEInvoiceRepository
{
    public EInvoiceRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<EInvoiceUpload?> GetUploadByIdAsync(long uploadId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM EInvoiceUploads 
                WHERE UploadId = @UploadId";

            return await QueryFirstOrDefaultAsync<EInvoiceUpload>(sql, new { UploadId = uploadId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢上傳記錄失敗: {uploadId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<EInvoiceUpload>> GetUploadsPagedAsync(EInvoiceUploadQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM EInvoiceUploads
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (!string.IsNullOrEmpty(query.UploadBy))
            {
                sql += " AND UploadBy = @UploadBy";
                parameters.Add("UploadBy", query.UploadBy);
            }

            if (!string.IsNullOrEmpty(query.StoreId))
            {
                sql += " AND StoreId = @StoreId";
                parameters.Add("StoreId", query.StoreId);
            }

            if (query.StartDate.HasValue)
            {
                sql += " AND UploadDate >= @StartDate";
                parameters.Add("StartDate", query.StartDate);
            }

            if (query.EndDate.HasValue)
            {
                sql += " AND UploadDate <= @EndDate";
                parameters.Add("EndDate", query.EndDate);
            }

            if (!string.IsNullOrEmpty(query.UploadType))
            {
                sql += " AND UploadType = @UploadType";
                parameters.Add("UploadType", query.UploadType);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "UploadDate" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<EInvoiceUpload>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM EInvoiceUploads
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND Status = @Status";
                countParameters.Add("Status", query.Status);
            }
            if (!string.IsNullOrEmpty(query.UploadBy))
            {
                countSql += " AND UploadBy = @UploadBy";
                countParameters.Add("UploadBy", query.UploadBy);
            }
            if (!string.IsNullOrEmpty(query.StoreId))
            {
                countSql += " AND StoreId = @StoreId";
                countParameters.Add("StoreId", query.StoreId);
            }
            if (query.StartDate.HasValue)
            {
                countSql += " AND UploadDate >= @StartDate";
                countParameters.Add("StartDate", query.StartDate);
            }
            if (query.EndDate.HasValue)
            {
                countSql += " AND UploadDate <= @EndDate";
                countParameters.Add("EndDate", query.EndDate);
            }
            if (!string.IsNullOrEmpty(query.UploadType))
            {
                countSql += " AND UploadType = @UploadType";
                countParameters.Add("UploadType", query.UploadType);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<EInvoiceUpload>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢上傳記錄列表失敗", ex);
            throw;
        }
    }

    public async Task<long> CreateUploadAsync(EInvoiceUpload upload)
    {
        try
        {
            const string sql = @"
                INSERT INTO EInvoiceUploads (
                    FileName, FileSize, FileType, UploadDate, UploadBy, Status,
                    ProcessStartDate, ProcessEndDate, TotalRecords, SuccessRecords, FailedRecords,
                    ErrorMessage, ProcessLog, StoreId, RetailerId, UploadType,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                )
                VALUES (
                    @FileName, @FileSize, @FileType, @UploadDate, @UploadBy, @Status,
                    @ProcessStartDate, @ProcessEndDate, @TotalRecords, @SuccessRecords, @FailedRecords,
                    @ErrorMessage, @ProcessLog, @StoreId, @RetailerId, @UploadType,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var uploadId = await ExecuteScalarAsync<long>(sql, upload);
            return uploadId;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增上傳記錄失敗", ex);
            throw;
        }
    }

    public async Task UpdateUploadAsync(EInvoiceUpload upload)
    {
        try
        {
            const string sql = @"
                UPDATE EInvoiceUploads SET
                    FileName = @FileName,
                    FileSize = @FileSize,
                    FileType = @FileType,
                    UploadDate = @UploadDate,
                    UploadBy = @UploadBy,
                    Status = @Status,
                    ProcessStartDate = @ProcessStartDate,
                    ProcessEndDate = @ProcessEndDate,
                    TotalRecords = @TotalRecords,
                    SuccessRecords = @SuccessRecords,
                    FailedRecords = @FailedRecords,
                    ErrorMessage = @ErrorMessage,
                    ProcessLog = @ProcessLog,
                    StoreId = @StoreId,
                    RetailerId = @RetailerId,
                    UploadType = @UploadType,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE UploadId = @UploadId";

            await ExecuteAsync(sql, upload);
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新上傳記錄失敗: {upload.UploadId}", ex);
            throw;
        }
    }

    public async Task DeleteUploadAsync(long uploadId)
    {
        try
        {
            const string sql = @"
                DELETE FROM EInvoiceUploads 
                WHERE UploadId = @UploadId";

            await ExecuteAsync(sql, new { UploadId = uploadId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除上傳記錄失敗: {uploadId}", ex);
            throw;
        }
    }

    public async Task<List<EInvoiceEntity>> GetEInvoicesByUploadIdAsync(long uploadId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM EInvoices 
                WHERE UploadId = @UploadId
                ORDER BY InvoiceId";

            var items = await QueryAsync<EInvoiceEntity>(sql, new { UploadId = uploadId });
            return items.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢電子發票列表失敗(上傳記錄ID): {uploadId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<EInvoiceEntity>> GetEInvoicesPagedAsync(EInvoiceQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM EInvoices
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (query.UploadId.HasValue)
            {
                sql += " AND UploadId = @UploadId";
                parameters.Add("UploadId", query.UploadId);
            }

            if (!string.IsNullOrEmpty(query.OrderNo))
            {
                sql += " AND OrderNo LIKE @OrderNo";
                parameters.Add("OrderNo", $"%{query.OrderNo}%");
            }

            if (!string.IsNullOrEmpty(query.RetailerOrderNo))
            {
                sql += " AND RetailerOrderNo LIKE @RetailerOrderNo";
                parameters.Add("RetailerOrderNo", $"%{query.RetailerOrderNo}%");
            }

            if (!string.IsNullOrEmpty(query.RetailerOrderDetailNo))
            {
                sql += " AND RetailerOrderDetailNo LIKE @RetailerOrderDetailNo";
                parameters.Add("RetailerOrderDetailNo", $"%{query.RetailerOrderDetailNo}%");
            }

            if (!string.IsNullOrEmpty(query.StoreId))
            {
                sql += " AND StoreId = @StoreId";
                parameters.Add("StoreId", query.StoreId);
            }

            if (!string.IsNullOrEmpty(query.ProviderId))
            {
                sql += " AND ProviderId = @ProviderId";
                parameters.Add("ProviderId", query.ProviderId);
            }

            if (query.OrderDateFrom.HasValue)
            {
                sql += " AND OrderDate >= @OrderDateFrom";
                parameters.Add("OrderDateFrom", query.OrderDateFrom);
            }

            if (query.OrderDateTo.HasValue)
            {
                sql += " AND OrderDate <= @OrderDateTo";
                parameters.Add("OrderDateTo", query.OrderDateTo);
            }

            if (!string.IsNullOrEmpty(query.ProcessStatus))
            {
                sql += " AND ProcessStatus = @ProcessStatus";
                parameters.Add("ProcessStatus", query.ProcessStatus);
            }

            if (!string.IsNullOrEmpty(query.OrderStatus))
            {
                sql += " AND OrderStatus = @OrderStatus";
                parameters.Add("OrderStatus", query.OrderStatus);
            }

            if (!string.IsNullOrEmpty(query.SpecId))
            {
                sql += " AND SpecId LIKE @SpecId";
                parameters.Add("SpecId", $"%{query.SpecId}%");
            }

            if (!string.IsNullOrEmpty(query.ProviderGoodsId))
            {
                sql += " AND ProviderGoodsId LIKE @ProviderGoodsId";
                parameters.Add("ProviderGoodsId", $"%{query.ProviderGoodsId}%");
            }

            if (!string.IsNullOrEmpty(query.NdType))
            {
                sql += " AND NdType = @NdType";
                parameters.Add("NdType", query.NdType);
            }

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

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "OrderDate" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<EInvoiceEntity>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM EInvoices
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (query.UploadId.HasValue)
            {
                countSql += " AND UploadId = @UploadId";
                countParameters.Add("UploadId", query.UploadId);
            }
            if (!string.IsNullOrEmpty(query.OrderNo))
            {
                countSql += " AND OrderNo LIKE @OrderNo";
                countParameters.Add("OrderNo", $"%{query.OrderNo}%");
            }
            if (!string.IsNullOrEmpty(query.RetailerOrderNo))
            {
                countSql += " AND RetailerOrderNo LIKE @RetailerOrderNo";
                countParameters.Add("RetailerOrderNo", $"%{query.RetailerOrderNo}%");
            }
            if (!string.IsNullOrEmpty(query.RetailerOrderDetailNo))
            {
                countSql += " AND RetailerOrderDetailNo LIKE @RetailerOrderDetailNo";
                countParameters.Add("RetailerOrderDetailNo", $"%{query.RetailerOrderDetailNo}%");
            }
            if (!string.IsNullOrEmpty(query.StoreId))
            {
                countSql += " AND StoreId = @StoreId";
                countParameters.Add("StoreId", query.StoreId);
            }
            if (!string.IsNullOrEmpty(query.ProviderId))
            {
                countSql += " AND ProviderId = @ProviderId";
                countParameters.Add("ProviderId", query.ProviderId);
            }
            if (query.OrderDateFrom.HasValue)
            {
                countSql += " AND OrderDate >= @OrderDateFrom";
                countParameters.Add("OrderDateFrom", query.OrderDateFrom);
            }
            if (query.OrderDateTo.HasValue)
            {
                countSql += " AND OrderDate <= @OrderDateTo";
                countParameters.Add("OrderDateTo", query.OrderDateTo);
            }
            if (!string.IsNullOrEmpty(query.ProcessStatus))
            {
                countSql += " AND ProcessStatus = @ProcessStatus";
                countParameters.Add("ProcessStatus", query.ProcessStatus);
            }
            if (!string.IsNullOrEmpty(query.OrderStatus))
            {
                countSql += " AND OrderStatus = @OrderStatus";
                countParameters.Add("OrderStatus", query.OrderStatus);
            }
            if (!string.IsNullOrEmpty(query.SpecId))
            {
                countSql += " AND SpecId LIKE @SpecId";
                countParameters.Add("SpecId", $"%{query.SpecId}%");
            }
            if (!string.IsNullOrEmpty(query.ProviderGoodsId))
            {
                countSql += " AND ProviderGoodsId LIKE @ProviderGoodsId";
                countParameters.Add("ProviderGoodsId", $"%{query.ProviderGoodsId}%");
            }
            if (!string.IsNullOrEmpty(query.NdType))
            {
                countSql += " AND NdType = @NdType";
                countParameters.Add("NdType", query.NdType);
            }
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

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<EInvoiceEntity>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢電子發票列表失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<EInvoiceWithNamesDto>> GetEInvoicesWithNamesPagedAsync(EInvoiceQuery query)
    {
        try
        {
            var sql = @"
                SELECT 
                    ei.*,
                    ISNULL(s.ShopName, ei.StoreId) AS StoreName,
                    ISNULL(v.VendorName, ei.ProviderId) AS ProviderName
                FROM EInvoices ei
                LEFT JOIN Shops s ON ei.StoreId = s.ShopId
                LEFT JOIN Vendors v ON ei.ProviderId = v.VendorId
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (query.UploadId.HasValue)
            {
                sql += " AND ei.UploadId = @UploadId";
                parameters.Add("UploadId", query.UploadId);
            }

            if (!string.IsNullOrEmpty(query.OrderNo))
            {
                sql += " AND ei.OrderNo LIKE @OrderNo";
                parameters.Add("OrderNo", $"%{query.OrderNo}%");
            }

            if (!string.IsNullOrEmpty(query.RetailerOrderNo))
            {
                sql += " AND ei.RetailerOrderNo LIKE @RetailerOrderNo";
                parameters.Add("RetailerOrderNo", $"%{query.RetailerOrderNo}%");
            }

            if (!string.IsNullOrEmpty(query.RetailerOrderDetailNo))
            {
                sql += " AND ei.RetailerOrderDetailNo LIKE @RetailerOrderDetailNo";
                parameters.Add("RetailerOrderDetailNo", $"%{query.RetailerOrderDetailNo}%");
            }

            if (!string.IsNullOrEmpty(query.StoreId))
            {
                sql += " AND ei.StoreId = @StoreId";
                parameters.Add("StoreId", query.StoreId);
            }

            if (!string.IsNullOrEmpty(query.ProviderId))
            {
                sql += " AND ei.ProviderId = @ProviderId";
                parameters.Add("ProviderId", query.ProviderId);
            }

            if (query.OrderDateFrom.HasValue)
            {
                sql += " AND ei.OrderDate >= @OrderDateFrom";
                parameters.Add("OrderDateFrom", query.OrderDateFrom);
            }

            if (query.OrderDateTo.HasValue)
            {
                sql += " AND ei.OrderDate <= @OrderDateTo";
                parameters.Add("OrderDateTo", query.OrderDateTo);
            }

            if (!string.IsNullOrEmpty(query.ProcessStatus))
            {
                sql += " AND ei.ProcessStatus = @ProcessStatus";
                parameters.Add("ProcessStatus", query.ProcessStatus);
            }

            if (!string.IsNullOrEmpty(query.OrderStatus))
            {
                sql += " AND ei.OrderStatus = @OrderStatus";
                parameters.Add("OrderStatus", query.OrderStatus);
            }

            if (!string.IsNullOrEmpty(query.SpecId))
            {
                sql += " AND ei.SpecId LIKE @SpecId";
                parameters.Add("SpecId", $"%{query.SpecId}%");
            }

            if (!string.IsNullOrEmpty(query.ProviderGoodsId))
            {
                sql += " AND ei.ProviderGoodsId LIKE @ProviderGoodsId";
                parameters.Add("ProviderGoodsId", $"%{query.ProviderGoodsId}%");
            }

            if (!string.IsNullOrEmpty(query.NdType))
            {
                sql += " AND ei.NdType = @NdType";
                parameters.Add("NdType", query.NdType);
            }

            if (!string.IsNullOrEmpty(query.GoodsId))
            {
                sql += " AND ei.GoodsId LIKE @GoodsId";
                parameters.Add("GoodsId", $"%{query.GoodsId}%");
            }

            if (!string.IsNullOrEmpty(query.GoodsName))
            {
                sql += " AND ei.GoodsName LIKE @GoodsName";
                parameters.Add("GoodsName", $"%{query.GoodsName}%");
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "ei.OrderDate" : $"ei.{query.SortField}";
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<EInvoiceWithNamesDto>(sql, parameters);

            // 查詢總數（使用相同的 WHERE 條件）
            var countSql = @"
                SELECT COUNT(*) 
                FROM EInvoices ei
                LEFT JOIN Shops s ON ei.StoreId = s.ShopId
                LEFT JOIN Vendors v ON ei.ProviderId = v.VendorId
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (query.UploadId.HasValue)
            {
                countSql += " AND ei.UploadId = @UploadId";
                countParameters.Add("UploadId", query.UploadId);
            }
            if (!string.IsNullOrEmpty(query.OrderNo))
            {
                countSql += " AND ei.OrderNo LIKE @OrderNo";
                countParameters.Add("OrderNo", $"%{query.OrderNo}%");
            }
            if (!string.IsNullOrEmpty(query.RetailerOrderNo))
            {
                countSql += " AND ei.RetailerOrderNo LIKE @RetailerOrderNo";
                countParameters.Add("RetailerOrderNo", $"%{query.RetailerOrderNo}%");
            }
            if (!string.IsNullOrEmpty(query.RetailerOrderDetailNo))
            {
                countSql += " AND ei.RetailerOrderDetailNo LIKE @RetailerOrderDetailNo";
                countParameters.Add("RetailerOrderDetailNo", $"%{query.RetailerOrderDetailNo}%");
            }
            if (!string.IsNullOrEmpty(query.StoreId))
            {
                countSql += " AND ei.StoreId = @StoreId";
                countParameters.Add("StoreId", query.StoreId);
            }
            if (!string.IsNullOrEmpty(query.ProviderId))
            {
                countSql += " AND ei.ProviderId = @ProviderId";
                countParameters.Add("ProviderId", query.ProviderId);
            }
            if (query.OrderDateFrom.HasValue)
            {
                countSql += " AND ei.OrderDate >= @OrderDateFrom";
                countParameters.Add("OrderDateFrom", query.OrderDateFrom);
            }
            if (query.OrderDateTo.HasValue)
            {
                countSql += " AND ei.OrderDate <= @OrderDateTo";
                countParameters.Add("OrderDateTo", query.OrderDateTo);
            }
            if (!string.IsNullOrEmpty(query.ProcessStatus))
            {
                countSql += " AND ei.ProcessStatus = @ProcessStatus";
                countParameters.Add("ProcessStatus", query.ProcessStatus);
            }
            if (!string.IsNullOrEmpty(query.OrderStatus))
            {
                countSql += " AND ei.OrderStatus = @OrderStatus";
                countParameters.Add("OrderStatus", query.OrderStatus);
            }
            if (!string.IsNullOrEmpty(query.SpecId))
            {
                countSql += " AND ei.SpecId LIKE @SpecId";
                countParameters.Add("SpecId", $"%{query.SpecId}%");
            }
            if (!string.IsNullOrEmpty(query.ProviderGoodsId))
            {
                countSql += " AND ei.ProviderGoodsId LIKE @ProviderGoodsId";
                countParameters.Add("ProviderGoodsId", $"%{query.ProviderGoodsId}%");
            }
            if (!string.IsNullOrEmpty(query.NdType))
            {
                countSql += " AND ei.NdType = @NdType";
                countParameters.Add("NdType", query.NdType);
            }
            if (!string.IsNullOrEmpty(query.GoodsId))
            {
                countSql += " AND ei.GoodsId LIKE @GoodsId";
                countParameters.Add("GoodsId", $"%{query.GoodsId}%");
            }
            if (!string.IsNullOrEmpty(query.GoodsName))
            {
                countSql += " AND ei.GoodsName LIKE @GoodsName";
                countParameters.Add("GoodsName", $"%{query.GoodsName}%");
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<EInvoiceWithNamesDto>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢電子發票列表（含名稱）失敗", ex);
            throw;
        }
    }

    public async Task<EInvoiceEntity?> GetEInvoiceByIdAsync(long invoiceId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM EInvoices 
                WHERE InvoiceId = @InvoiceId";

            return await QueryFirstOrDefaultAsync<EInvoiceEntity>(sql, new { InvoiceId = invoiceId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢電子發票失敗: {invoiceId}", ex);
            throw;
        }
    }

    public async Task<long> CreateEInvoiceAsync(EInvoiceEntity invoice)
    {
        try
        {
            const string sql = @"
                INSERT INTO EInvoices (
                    UploadId, OrderNo, RetailerOrderNo, RetailerOrderDetailNo, OrderDate,
                    StoreId, ProviderId, NdType, GoodsId, GoodsName, SpecId, ProviderGoodsId,
                    SpecColor, SpecSize, SuggestPrice, InternetPrice, ShippingType, ShippingFee,
                    OrderQty, OrderShippingFee, OrderSubtotal, OrderTotal, OrderStatus,
                    ProcessStatus, ErrorMessage, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                )
                VALUES (
                    @UploadId, @OrderNo, @RetailerOrderNo, @RetailerOrderDetailNo, @OrderDate,
                    @StoreId, @ProviderId, @NdType, @GoodsId, @GoodsName, @SpecId, @ProviderGoodsId,
                    @SpecColor, @SpecSize, @SuggestPrice, @InternetPrice, @ShippingType, @ShippingFee,
                    @OrderQty, @OrderShippingFee, @OrderSubtotal, @OrderTotal, @OrderStatus,
                    @ProcessStatus, @ErrorMessage, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var invoiceId = await ExecuteScalarAsync<long>(sql, invoice);
            return invoiceId;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增電子發票失敗", ex);
            throw;
        }
    }

    public async Task UpdateEInvoiceAsync(EInvoiceEntity invoice)
    {
        try
        {
            const string sql = @"
                UPDATE EInvoices SET
                    UploadId = @UploadId,
                    OrderNo = @OrderNo,
                    RetailerOrderNo = @RetailerOrderNo,
                    RetailerOrderDetailNo = @RetailerOrderDetailNo,
                    OrderDate = @OrderDate,
                    StoreId = @StoreId,
                    ProviderId = @ProviderId,
                    NdType = @NdType,
                    GoodsId = @GoodsId,
                    GoodsName = @GoodsName,
                    SpecId = @SpecId,
                    ProviderGoodsId = @ProviderGoodsId,
                    SpecColor = @SpecColor,
                    SpecSize = @SpecSize,
                    SuggestPrice = @SuggestPrice,
                    InternetPrice = @InternetPrice,
                    ShippingType = @ShippingType,
                    ShippingFee = @ShippingFee,
                    OrderQty = @OrderQty,
                    OrderShippingFee = @OrderShippingFee,
                    OrderSubtotal = @OrderSubtotal,
                    OrderTotal = @OrderTotal,
                    OrderStatus = @OrderStatus,
                    ProcessStatus = @ProcessStatus,
                    ErrorMessage = @ErrorMessage,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE InvoiceId = @InvoiceId";

            await ExecuteAsync(sql, invoice);
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新電子發票失敗: {invoice.InvoiceId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<EInvoiceReportDto>> GetEInvoiceReportsPagedAsync(EInvoiceReportQuery query)
    {
        try
        {
            // 根據報表類型構建不同的 SQL
            var sql = BuildReportSql(query);
            var parameters = BuildReportParameters(query);

            // ECA4020、ECA4030、ECA4040、ECA4050 和 ECA4060 的排序已在 BuildReportSql 中處理，不需要額外添加
            if (query.ReportType != "ECA4020" && query.ReportType != "ECA4030" && query.ReportType != "ECA4040" && query.ReportType != "ECA4050" && query.ReportType != "ECA4060")
            {
                // 排序
                var sortField = string.IsNullOrEmpty(query.SortField) ? "OrderDate" : query.SortField;
                var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
                sql += $" ORDER BY {sortField} {sortOrder}";
            }

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            // ECA4020 使用不同的 DTO 類型
            if (query.ReportType == "ECA4020")
            {
                var items = await QueryAsync<ECA4020ReportDto>(sql, parameters);
                var countSql4020 = BuildReportCountSql(query);
                var countParameters4020 = BuildReportParameters(query);
                var totalCount4020 = await QuerySingleAsync<int>(countSql4020, countParameters4020);

                // 轉換為 EInvoiceReportDto（為了保持接口一致性）
                var convertedItems = items.Select(x => new EInvoiceReportDto
                {
                    GoodsId = x.GoodsId,
                    GoodsName = x.GoodsName,
                    ProviderGoodsId = x.ProviderGoodsId,
                    OrderQty = x.SumQty,
                    OrderSubtotal = x.SumSubtotal,
                    ShippingFee = x.SumFee,
                    ScId = x.ScId,
                    ScName = x.ScName,
                    AvgPrice = x.AvgPrice,
                    SalesPercent = x.SalesPercent,
                    SalesRanking = x.SalesRanking
                }).ToList();

                return new PagedResult<EInvoiceReportDto>
                {
                    Items = convertedItems,
                    TotalCount = totalCount4020,
                    PageIndex = query.PageIndex,
                    PageSize = query.PageSize
                };
            }

            // ECA4030 使用不同的 DTO 類型
            if (query.ReportType == "ECA4030")
            {
                var items = await QueryAsync<ECA4030ReportDto>(sql, parameters);
                var countSql4030 = BuildReportCountSql(query);
                var countParameters4030 = BuildReportParameters(query);
                var totalCount4030 = await QuerySingleAsync<int>(countSql4030, countParameters4030);

                // 轉換為 EInvoiceReportDto（為了保持接口一致性）
                var convertedItems = items.Select(x => new EInvoiceReportDto
                {
                    RetailerId = x.RetailerId,
                    RetailerName = x.RetailerName,
                    OrderQty = x.SumQty,
                    OrderSubtotal = x.SumSubtotal,
                    ShippingFee = x.SumFee,
                    SalesPercent = x.SalesPercent,
                    SalesRanking = x.SalesRanking
                }).ToList();

                return new PagedResult<EInvoiceReportDto>
                {
                    Items = convertedItems,
                    TotalCount = totalCount4030,
                    PageIndex = query.PageIndex,
                    PageSize = query.PageSize
                };
            }

            // ECA4040 使用不同的 DTO 類型
            if (query.ReportType == "ECA4040")
            {
                var items = await QueryAsync<ECA4040ReportDto>(sql, parameters);
                var countSql4040 = BuildReportCountSql(query);
                var countParameters4040 = BuildReportParameters(query);
                var totalCount4040 = await QuerySingleAsync<int>(countSql4040, countParameters4040);

                // 轉換為 EInvoiceReportDto（為了保持接口一致性）
                var convertedItems = items.Select(x => new EInvoiceReportDto
                {
                    StoreId = x.StoreId,
                    StoreName = x.StoreName,
                    StoreFloor = x.StoreFloor,
                    StoreType = x.StoreType,
                    OrderQty = x.SumQty,
                    OrderSubtotal = x.SumSubtotal,
                    ShippingFee = x.SumFee,
                    SalesPercent = x.SalesPercent,
                    SalesRanking = x.SalesRanking
                }).ToList();

                return new PagedResult<EInvoiceReportDto>
                {
                    Items = convertedItems,
                    TotalCount = totalCount4040,
                    PageIndex = query.PageIndex,
                    PageSize = query.PageSize
                };
            }

            // ECA4050 使用不同的 DTO 類型
            if (query.ReportType == "ECA4050")
            {
                var items = await QueryAsync<ECA4050ReportDto>(sql, parameters);
                var countSql4050 = BuildReportCountSql(query);
                var countParameters4050 = BuildReportParameters(query);
                var totalCount4050 = await QuerySingleAsync<int>(countSql4050, countParameters4050);

                // 轉換為 EInvoiceReportDto（為了保持接口一致性）
                var convertedItems = items.Select(x => new EInvoiceReportDto
                {
                    ShipDate = x.OrderShipDate,
                    OrderQty = x.SumQty,
                    OrderSubtotal = x.SumSubtotal,
                    ShippingFee = x.SumFee,
                    SalesPercent = x.SalesPercent
                }).ToList();

                return new PagedResult<EInvoiceReportDto>
                {
                    Items = convertedItems,
                    TotalCount = totalCount4050,
                    PageIndex = query.PageIndex,
                    PageSize = query.PageSize
                };
            }

            // ECA4060 使用不同的 DTO 類型
            if (query.ReportType == "ECA4060")
            {
                var items = await QueryAsync<ECA4060ReportDto>(sql, parameters);
                var countSql4060 = BuildReportCountSql(query);
                var countParameters4060 = BuildReportParameters(query);
                var totalCount4060 = await QuerySingleAsync<int>(countSql4060, countParameters4060);

                // 轉換為 EInvoiceReportDto（包含完整的 Y/N/A 統計數據）
                var convertedItems = items.Select(x => new EInvoiceReportDto
                {
                    OrderDate = x.OrderDate,
                    OrderQty = x.SumAQty,
                    OrderSubtotal = x.SumASubtotal,
                    ShippingFee = x.SumAFee,
                    SalesPercent = x.SalesAPercent,
                    // ECA4060 專用欄位：已開立/未開立/全部統計
                    SumYQty = x.SumYQty,
                    SumYSubtotal = x.SumYSubtotal,
                    SumYFee = x.SumYFee,
                    SalesYPercent = x.SalesYPercent,
                    SumNQty = x.SumNQty,
                    SumNSubtotal = x.SumNSubtotal,
                    SumNFee = x.SumNFee,
                    SalesNPercent = x.SalesNPercent,
                    SumAQty = x.SumAQty,
                    SumASubtotal = x.SumASubtotal,
                    SumAFee = x.SumAFee,
                    SalesAPercent = x.SalesAPercent
                }).ToList();

                return new PagedResult<EInvoiceReportDto>
                {
                    Items = convertedItems,
                    TotalCount = totalCount4060,
                    PageIndex = query.PageIndex,
                    PageSize = query.PageSize
                };
            }

            var regularItems = await QueryAsync<EInvoiceReportDto>(sql, parameters);

            // 查詢總數
            var countSql = BuildReportCountSql(query);
            var countParameters = BuildReportParameters(query);
            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<EInvoiceReportDto>
            {
                Items = regularItems.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢電子發票報表失敗", ex);
            throw;
        }
    }

    private string BuildReportSql(EInvoiceReportQuery query)
    {
        // ECA4010: 訂單明細報表 - 根據開發計劃實現專門查詢
        if (query.ReportType == "ECA4010")
        {
            var sql = @"
                SELECT 
                    ei.InvoiceId,
                    ei.OrderNo AS OrderNo,
                    ei.OrderDate AS OrderDate,
                    ei.OrderDate AS ShipDate,
                    ei.OrderStatus AS OrderStatus,
                    ei.OrderQty AS OrderQty,
                    ei.OrderSubtotal AS OrderSubtotal,
                    ei.OrderTotal AS OrderTotal,
                    ISNULL(s.ShopId, ei.StoreId) AS StoreId,
                    ISNULL(s.ShopName, ei.StoreId) AS StoreName,
                    eu.RetailerId AS RetailerId,
                    NULL AS RetailerName,
                    ei.ProviderId AS ProviderId,
                    v.VendorName AS ProviderName,
                    ei.GoodsId AS GoodsId,
                    ei.GoodsName AS GoodsName,
                    ei.SpecId AS SpecId,
                    ei.ProviderGoodsId AS ProviderGoodsId,
                    NULL AS ScId,
                    NULL AS ScName,
                    ei.SuggestPrice AS SuggestPrice,
                    ei.InternetPrice AS InternetPrice,
                    ei.ShippingFee AS ShippingFee,
                    ei.ShippingType AS ShippingType,
                    NULL AS TotalQty,
                    NULL AS TotalAmount,
                    NULL AS TotalOrders
                FROM EInvoices ei
                LEFT JOIN Vendors v ON ei.ProviderId = v.VendorId
                LEFT JOIN Shops s ON ei.StoreId = s.ShopId
                LEFT JOIN EInvoiceUploads eu ON ei.UploadId = eu.UploadId
                WHERE ei.ProcessStatus = 'COMPLETED'";
            
            // 添加查詢條件
            if (!string.IsNullOrEmpty(query.OrderNo))
            {
                sql += " AND ei.OrderNo LIKE @OrderNo";
            }

            if (!string.IsNullOrEmpty(query.StoreId))
            {
                sql += " AND (ei.StoreId = @StoreId OR s.ShopId = @StoreId)";
            }

            if (!string.IsNullOrEmpty(query.RetailerId))
            {
                sql += " AND eu.RetailerId = @RetailerId";
            }

            if (!string.IsNullOrEmpty(query.ScId))
            {
                sql += " AND ei.ScId = @ScId";
            }

            if (!string.IsNullOrEmpty(query.ProviderId))
            {
                sql += " AND ei.ProviderId = @ProviderId";
            }

            if (!string.IsNullOrEmpty(query.GoodsId))
            {
                sql += " AND ei.GoodsId LIKE @GoodsId";
            }

            if (!string.IsNullOrEmpty(query.GoodsName))
            {
                sql += " AND ei.GoodsName LIKE @GoodsName";
            }

            if (query.OrderDateFrom.HasValue)
            {
                sql += " AND ei.OrderDate >= @OrderDateFrom";
            }

            if (query.OrderDateTo.HasValue)
            {
                sql += " AND ei.OrderDate <= @OrderDateTo";
            }

            if (!string.IsNullOrEmpty(query.OrderStatus))
            {
                sql += " AND ei.OrderStatus = @OrderStatus";
            }

            return sql;
        }

        // ECA4020: 商品銷售統計報表 - 根據開發計劃實現專門查詢
        if (query.ReportType == "ECA4020")
        {
            var sql = @"
                SELECT 
                    ISNULL(sc.ScId, '') AS ScId,
                    ISNULL(sc.ScName, '') AS ScName,
                    ei.GoodsId AS GoodsId,
                    ei.GoodsName AS GoodsName,
                    ei.ProviderGoodsId AS ProviderGoodsId,
                    SUM(ei.OrderQty) AS SumQty,
                    SUM(ei.OrderSubtotal) AS SumSubtotal,
                    SUM(ei.OrderShippingFee) AS SumFee,
                    CASE 
                        WHEN SUM(ei.OrderQty) > 0 
                        THEN SUM(ei.OrderSubtotal) / SUM(ei.OrderQty)
                        ELSE 0 
                    END AS AvgPrice,
                    CASE 
                        WHEN (SELECT SUM(OrderSubtotal) FROM EInvoices WHERE ProcessStatus = 'COMPLETED') > 0
                        THEN (SUM(ei.OrderSubtotal) * 100.0) / (SELECT SUM(OrderSubtotal) FROM EInvoices WHERE ProcessStatus = 'COMPLETED')
                        ELSE 0
                    END AS SalesPercent,
                    ROW_NUMBER() OVER (ORDER BY SUM(ei.OrderSubtotal) DESC) AS SalesRanking
                FROM EInvoices ei
                LEFT JOIN StoreCounters sc ON ei.ScId = sc.ScId
                WHERE ei.ProcessStatus = 'COMPLETED'";
            
            // 添加查詢條件
            if (!string.IsNullOrEmpty(query.StoreId))
            {
                sql += " AND ei.StoreId = @StoreId";
            }

            if (!string.IsNullOrEmpty(query.ScId))
            {
                sql += " AND ei.ScId = @ScId";
            }

            if (!string.IsNullOrEmpty(query.ProviderId))
            {
                sql += " AND ei.ProviderId = @ProviderId";
            }

            if (!string.IsNullOrEmpty(query.GoodsId))
            {
                sql += " AND ei.GoodsId LIKE @GoodsId";
            }

            if (!string.IsNullOrEmpty(query.GoodsName))
            {
                sql += " AND ei.GoodsName LIKE @GoodsName";
            }

            if (query.OrderDateFrom.HasValue)
            {
                sql += " AND ei.OrderDate >= @OrderDateFrom";
            }

            if (query.OrderDateTo.HasValue)
            {
                sql += " AND ei.OrderDate <= @OrderDateTo";
            }

            sql += @"
                GROUP BY ISNULL(sc.ScId, ''), ISNULL(sc.ScName, ''), ei.GoodsId, ei.GoodsName, ei.ProviderGoodsId";

            // 添加排序
            if (!string.IsNullOrEmpty(query.SortField))
            {
                var sortOrder = query.SortOrder == "DESC" ? "DESC" : "ASC";
                sql += $" ORDER BY {query.SortField} {sortOrder}";
            }
            else
            {
                sql += " ORDER BY SUM(ei.OrderSubtotal) DESC";
            }

            return sql;
        }

        // ECA4030: 零售商銷售統計報表 - 根據開發計劃實現專門查詢
        if (query.ReportType == "ECA4030")
        {
            var sql = @"
                SELECT 
                    ISNULL(r.RetailerId, '') AS RetailerId,
                    ISNULL(r.RetailerName, '') AS RetailerName,
                    SUM(ei.OrderQty) AS SumQty,
                    SUM(ei.OrderSubtotal) AS SumSubtotal,
                    SUM(ei.OrderShippingFee) AS SumFee,
                    CASE 
                        WHEN (SELECT SUM(OrderSubtotal) FROM EInvoices WHERE ProcessStatus = 'COMPLETED') > 0
                        THEN (SUM(ei.OrderSubtotal) * 100.0) / (SELECT SUM(OrderSubtotal) FROM EInvoices WHERE ProcessStatus = 'COMPLETED')
                        ELSE 0
                    END AS SalesPercent,
                    ROW_NUMBER() OVER (ORDER BY SUM(ei.OrderSubtotal) DESC) AS SalesRanking
                FROM EInvoices ei
                LEFT JOIN EInvoiceUploads eu ON ei.UploadId = eu.UploadId
                LEFT JOIN Retailers r ON eu.RetailerId = r.RetailerId
                WHERE ei.ProcessStatus = 'COMPLETED'";
            
            // 添加查詢條件
            if (!string.IsNullOrEmpty(query.StoreId))
            {
                sql += " AND ei.StoreId = @StoreId";
            }

            if (!string.IsNullOrEmpty(query.RetailerId))
            {
                sql += " AND eu.RetailerId = @RetailerId";
            }

            if (query.OrderDateFrom.HasValue)
            {
                sql += " AND ei.OrderDate >= @OrderDateFrom";
            }

            if (query.OrderDateTo.HasValue)
            {
                sql += " AND ei.OrderDate <= @OrderDateTo";
            }

            sql += @"
                GROUP BY ISNULL(r.RetailerId, ''), ISNULL(r.RetailerName, '')";

            // 添加排序
            if (!string.IsNullOrEmpty(query.SortField))
            {
                var sortOrder = query.SortOrder == "DESC" ? "DESC" : "ASC";
                sql += $" ORDER BY {query.SortField} {sortOrder}";
            }
            else
            {
                sql += " ORDER BY SUM(ei.OrderSubtotal) DESC";
            }

            return sql;
        }

        // ECA4040: 店別銷售統計報表 - 根據開發計劃實現專門查詢
        if (query.ReportType == "ECA4040")
        {
            var sql = @"
                SELECT 
                    ISNULL(s.StoreId, '') AS StoreId,
                    ISNULL(s.StoreName, '') AS StoreName,
                    ISNULL(s.FloorId, '') AS StoreFloor,
                    ISNULL(s.StoreType, '') AS StoreType,
                    SUM(ei.OrderQty) AS SumQty,
                    SUM(ei.OrderSubtotal) AS SumSubtotal,
                    SUM(ei.OrderShippingFee) AS SumFee,
                    CASE 
                        WHEN (SELECT SUM(OrderSubtotal) FROM EInvoices WHERE ProcessStatus = 'COMPLETED') > 0
                        THEN (SUM(ei.OrderSubtotal) * 100.0) / (SELECT SUM(OrderSubtotal) FROM EInvoices WHERE ProcessStatus = 'COMPLETED')
                        ELSE 0
                    END AS SalesPercent,
                    ROW_NUMBER() OVER (ORDER BY SUM(ei.OrderSubtotal) DESC) AS SalesRanking
                FROM EInvoices ei
                LEFT JOIN Stores s ON ei.StoreId = s.StoreId
                WHERE ei.ProcessStatus = 'COMPLETED'";
            
            // 添加查詢條件
            if (!string.IsNullOrEmpty(query.StoreId))
            {
                sql += " AND ei.StoreId = @StoreId";
            }

            if (!string.IsNullOrEmpty(query.StoreFloor))
            {
                sql += " AND s.FloorId = @StoreFloor";
            }

            if (!string.IsNullOrEmpty(query.StoreType))
            {
                sql += " AND s.StoreType = @StoreType";
            }

            if (query.OrderDateFrom.HasValue)
            {
                sql += " AND ei.OrderDate >= @OrderDateFrom";
            }

            if (query.OrderDateTo.HasValue)
            {
                sql += " AND ei.OrderDate <= @OrderDateTo";
            }

            sql += @"
                GROUP BY ISNULL(s.StoreId, ''), ISNULL(s.StoreName, ''), ISNULL(s.FloorId, ''), ISNULL(s.StoreType, '')";

            // 添加排序
            if (!string.IsNullOrEmpty(query.SortField))
            {
                var sortOrder = query.SortOrder == "DESC" ? "DESC" : "ASC";
                sql += $" ORDER BY {query.SortField} {sortOrder}";
            }
            else
            {
                sql += " ORDER BY SUM(ei.OrderSubtotal) DESC";
            }

            return sql;
        }

        // ECA4050: 出貨日期統計報表 - 根據開發計劃實現專門查詢
        if (query.ReportType == "ECA4050")
        {
            var sql = @"
                SELECT 
                    ei.OrderShipDate AS OrderShipDate,
                    SUM(ei.OrderQty) AS SumQty,
                    SUM(ei.OrderSubtotal) AS SumSubtotal,
                    SUM(ei.OrderShippingFee) AS SumFee,
                    CASE 
                        WHEN (SELECT SUM(OrderSubtotal) FROM EInvoices WHERE ProcessStatus = 'COMPLETED') > 0
                        THEN (SUM(ei.OrderSubtotal) * 100.0) / (SELECT SUM(OrderSubtotal) FROM EInvoices WHERE ProcessStatus = 'COMPLETED')
                        ELSE 0
                    END AS SalesPercent
                FROM EInvoices ei
                WHERE ei.ProcessStatus = 'COMPLETED'";
            
            // 添加查詢條件
            if (query.ShipDateFrom.HasValue)
            {
                sql += " AND ei.OrderShipDate >= @ShipDateFrom";
            }

            if (query.ShipDateTo.HasValue)
            {
                sql += " AND ei.OrderShipDate <= @ShipDateTo";
            }

            if (!string.IsNullOrEmpty(query.StoreId))
            {
                sql += " AND ei.StoreId = @StoreId";
            }

            sql += @"
                GROUP BY ei.OrderShipDate";

            // 添加排序
            if (!string.IsNullOrEmpty(query.SortField))
            {
                var sortOrder = query.SortOrder == "DESC" ? "DESC" : "ASC";
                sql += $" ORDER BY {query.SortField} {sortOrder}";
            }
            else
            {
                sql += " ORDER BY ei.OrderShipDate DESC";
            }

            return sql;
        }

        // ECA4060: 訂單日期統計報表 - 根據開發計劃實現專門查詢
        if (query.ReportType == "ECA4060")
        {
            var sql = @"
                SELECT 
                    ei.OrderDate AS OrderDate,
                    SUM(CASE WHEN ei.InvoiceStatus = 'Y' THEN ei.OrderQty ELSE 0 END) AS SumYQty,
                    SUM(CASE WHEN ei.InvoiceStatus = 'Y' THEN ei.OrderSubtotal ELSE 0 END) AS SumYSubtotal,
                    SUM(CASE WHEN ei.InvoiceStatus = 'Y' THEN ei.OrderShippingFee ELSE 0 END) AS SumYFee,
                    CASE 
                        WHEN (SELECT SUM(OrderSubtotal) FROM EInvoices WHERE ProcessStatus = 'COMPLETED' AND InvoiceStatus = 'Y') > 0
                        THEN (SUM(CASE WHEN ei.InvoiceStatus = 'Y' THEN ei.OrderSubtotal ELSE 0 END) * 100.0) / (SELECT SUM(OrderSubtotal) FROM EInvoices WHERE ProcessStatus = 'COMPLETED' AND InvoiceStatus = 'Y')
                        ELSE 0
                    END AS SalesYPercent,
                    SUM(CASE WHEN ei.InvoiceStatus = 'N' THEN ei.OrderQty ELSE 0 END) AS SumNQty,
                    SUM(CASE WHEN ei.InvoiceStatus = 'N' THEN ei.OrderSubtotal ELSE 0 END) AS SumNSubtotal,
                    SUM(CASE WHEN ei.InvoiceStatus = 'N' THEN ei.OrderShippingFee ELSE 0 END) AS SumNFee,
                    CASE 
                        WHEN (SELECT SUM(OrderSubtotal) FROM EInvoices WHERE ProcessStatus = 'COMPLETED' AND InvoiceStatus = 'N') > 0
                        THEN (SUM(CASE WHEN ei.InvoiceStatus = 'N' THEN ei.OrderSubtotal ELSE 0 END) * 100.0) / (SELECT SUM(OrderSubtotal) FROM EInvoices WHERE ProcessStatus = 'COMPLETED' AND InvoiceStatus = 'N')
                        ELSE 0
                    END AS SalesNPercent,
                    SUM(ei.OrderQty) AS SumAQty,
                    SUM(ei.OrderSubtotal) AS SumASubtotal,
                    SUM(ei.OrderShippingFee) AS SumAFee,
                    CASE 
                        WHEN (SELECT SUM(OrderSubtotal) FROM EInvoices WHERE ProcessStatus = 'COMPLETED') > 0
                        THEN (SUM(ei.OrderSubtotal) * 100.0) / (SELECT SUM(OrderSubtotal) FROM EInvoices WHERE ProcessStatus = 'COMPLETED')
                        ELSE 0
                    END AS SalesAPercent
                FROM EInvoices ei
                WHERE ei.ProcessStatus = 'COMPLETED'";
            
            // 添加查詢條件
            if (query.OrderDateFrom.HasValue)
            {
                sql += " AND ei.OrderDate >= @OrderDateFrom";
            }

            if (query.OrderDateTo.HasValue)
            {
                sql += " AND ei.OrderDate <= @OrderDateTo";
            }

            if (!string.IsNullOrEmpty(query.StoreId))
            {
                sql += " AND ei.StoreId = @StoreId";
            }

            sql += @"
                GROUP BY ei.OrderDate";

            // 添加排序
            if (!string.IsNullOrEmpty(query.SortField))
            {
                var sortOrder = query.SortOrder == "DESC" ? "DESC" : "ASC";
                sql += $" ORDER BY {query.SortField} {sortOrder}";
            }
            else
            {
                sql += " ORDER BY ei.OrderDate DESC";
            }

            return sql;
        }

        // 其他報表類型的通用查詢
        var defaultSql = @"
            SELECT 
                ei.InvoiceId,
                ei.OrderNo,
                ei.OrderDate,
                ei.OrderDate AS ShipDate,
                ei.OrderStatus,
                ei.OrderQty,
                ei.OrderSubtotal,
                ei.OrderTotal,
                ei.StoreId,
                ei.StoreId AS StoreName,
                NULL AS RetailerId,
                NULL AS RetailerName,
                ei.ProviderId,
                v.VendorName AS ProviderName,
                ei.GoodsId,
                ei.GoodsName,
                ei.SpecId,
                ei.ProviderGoodsId,
                ei.SuggestPrice,
                ei.InternetPrice,
                ei.ShippingFee,
                ei.ShippingType,
                NULL AS TotalQty,
                NULL AS TotalAmount,
                NULL AS TotalOrders
            FROM EInvoices ei
            LEFT JOIN Vendors v ON ei.ProviderId = v.VendorId
            WHERE ei.ProcessStatus = 'COMPLETED'";

        // 添加查詢條件（通用報表類型）
        if (!string.IsNullOrEmpty(query.OrderNo))
        {
            defaultSql += " AND ei.OrderNo LIKE @OrderNo";
        }

        if (!string.IsNullOrEmpty(query.RetailerOrderNo))
        {
            defaultSql += " AND ei.RetailerOrderNo LIKE @RetailerOrderNo";
        }

        if (!string.IsNullOrEmpty(query.StoreId))
        {
            defaultSql += " AND ei.StoreId = @StoreId";
        }

        if (!string.IsNullOrEmpty(query.ProviderId))
        {
            defaultSql += " AND ei.ProviderId = @ProviderId";
        }

        if (!string.IsNullOrEmpty(query.GoodsId))
        {
            defaultSql += " AND ei.GoodsId LIKE @GoodsId";
        }

        if (!string.IsNullOrEmpty(query.GoodsName))
        {
            defaultSql += " AND ei.GoodsName LIKE @GoodsName";
        }

        if (query.OrderDateFrom.HasValue)
        {
            defaultSql += " AND ei.OrderDate >= @OrderDateFrom";
        }

        if (query.OrderDateTo.HasValue)
        {
            defaultSql += " AND ei.OrderDate <= @OrderDateTo";
        }

        if (query.ShipDateFrom.HasValue)
        {
            defaultSql += " AND ei.OrderDate >= @ShipDateFrom";
        }

        if (query.ShipDateTo.HasValue)
        {
            defaultSql += " AND ei.OrderDate <= @ShipDateTo";
        }

        if (!string.IsNullOrEmpty(query.OrderStatus))
        {
            defaultSql += " AND ei.OrderStatus = @OrderStatus";
        }

        if (!string.IsNullOrEmpty(query.ProcessStatus))
        {
            defaultSql += " AND ei.ProcessStatus = @ProcessStatus";
        }

        return defaultSql;
    }

    private string BuildReportCountSql(EInvoiceReportQuery query)
    {
        // ECA4010: 訂單明細報表 - 根據開發計劃實現專門查詢
        if (query.ReportType == "ECA4010")
        {
            var sql = @"
                SELECT COUNT(*) 
                FROM EInvoices ei
                LEFT JOIN Shops s ON ei.StoreId = s.ShopId
                LEFT JOIN EInvoiceUploads eu ON ei.UploadId = eu.UploadId
                WHERE ei.ProcessStatus = 'COMPLETED'";
            
            // 添加查詢條件（與 BuildReportSql 保持一致）
            if (!string.IsNullOrEmpty(query.OrderNo))
            {
                sql += " AND ei.OrderNo LIKE @OrderNo";
            }

            if (!string.IsNullOrEmpty(query.StoreId))
            {
                sql += " AND (ei.StoreId = @StoreId OR s.ShopId = @StoreId)";
            }

            if (!string.IsNullOrEmpty(query.RetailerId))
            {
                sql += " AND eu.RetailerId = @RetailerId";
            }

            if (!string.IsNullOrEmpty(query.ScId))
            {
                sql += " AND ei.ScId = @ScId";
            }

            if (!string.IsNullOrEmpty(query.ProviderId))
            {
                sql += " AND ei.ProviderId = @ProviderId";
            }

            if (!string.IsNullOrEmpty(query.GoodsId))
            {
                sql += " AND ei.GoodsId LIKE @GoodsId";
            }

            if (!string.IsNullOrEmpty(query.GoodsName))
            {
                sql += " AND ei.GoodsName LIKE @GoodsName";
            }

            if (query.OrderDateFrom.HasValue)
            {
                sql += " AND ei.OrderDate >= @OrderDateFrom";
            }

            if (query.OrderDateTo.HasValue)
            {
                sql += " AND ei.OrderDate <= @OrderDateTo";
            }

            if (!string.IsNullOrEmpty(query.OrderStatus))
            {
                sql += " AND ei.OrderStatus = @OrderStatus";
            }

            return sql;
        }

        // ECA4020: 商品銷售統計報表 - 根據開發計劃實現專門查詢（需要計算分組後的數量）
        if (query.ReportType == "ECA4020")
        {
            var sql = @"
                SELECT COUNT(*) FROM (
                    SELECT 
                        ISNULL(sc.ScId, '') AS ScId,
                        ei.GoodsId AS GoodsId
                    FROM EInvoices ei
                    LEFT JOIN StoreCounters sc ON ei.ScId = sc.ScId
                    WHERE ei.ProcessStatus = 'COMPLETED'";
            
            // 添加查詢條件（與 BuildReportSql 保持一致）
            if (!string.IsNullOrEmpty(query.StoreId))
            {
                sql += " AND ei.StoreId = @StoreId";
            }

            if (!string.IsNullOrEmpty(query.ScId))
            {
                sql += " AND ei.ScId = @ScId";
            }

            if (!string.IsNullOrEmpty(query.ProviderId))
            {
                sql += " AND ei.ProviderId = @ProviderId";
            }

            if (!string.IsNullOrEmpty(query.GoodsId))
            {
                sql += " AND ei.GoodsId LIKE @GoodsId";
            }

            if (!string.IsNullOrEmpty(query.GoodsName))
            {
                sql += " AND ei.GoodsName LIKE @GoodsName";
            }

            if (query.OrderDateFrom.HasValue)
            {
                sql += " AND ei.OrderDate >= @OrderDateFrom";
            }

            if (query.OrderDateTo.HasValue)
            {
                sql += " AND ei.OrderDate <= @OrderDateTo";
            }

            sql += @"
                    GROUP BY ISNULL(sc.ScId, ''), ei.GoodsId, ei.GoodsName, ei.ProviderGoodsId
                ) AS grouped";
            
            return sql;
        }

        // ECA4030: 零售商銷售統計報表 - 根據開發計劃實現專門查詢（需要計算分組後的數量）
        if (query.ReportType == "ECA4030")
        {
            var sql = @"
                SELECT COUNT(*) FROM (
                    SELECT 
                        ISNULL(r.RetailerId, '') AS RetailerId
                    FROM EInvoices ei
                    LEFT JOIN EInvoiceUploads eu ON ei.UploadId = eu.UploadId
                    LEFT JOIN Retailers r ON eu.RetailerId = r.RetailerId
                    WHERE ei.ProcessStatus = 'COMPLETED'";
            
            // 添加查詢條件（與 BuildReportSql 保持一致）
            if (!string.IsNullOrEmpty(query.StoreId))
            {
                sql += " AND ei.StoreId = @StoreId";
            }

            if (!string.IsNullOrEmpty(query.RetailerId))
            {
                sql += " AND eu.RetailerId = @RetailerId";
            }

            if (query.OrderDateFrom.HasValue)
            {
                sql += " AND ei.OrderDate >= @OrderDateFrom";
            }

            if (query.OrderDateTo.HasValue)
            {
                sql += " AND ei.OrderDate <= @OrderDateTo";
            }

            sql += @"
                    GROUP BY ISNULL(r.RetailerId, '')
                ) AS grouped";
            
            return sql;
        }

        // ECA4040: 店別銷售統計報表 - 根據開發計劃實現專門查詢（需要計算分組後的數量）
        if (query.ReportType == "ECA4040")
        {
            var sql = @"
                SELECT COUNT(*) FROM (
                    SELECT 
                        ISNULL(s.StoreId, '') AS StoreId
                    FROM EInvoices ei
                    LEFT JOIN Stores s ON ei.StoreId = s.StoreId
                    WHERE ei.ProcessStatus = 'COMPLETED'";
            
            // 添加查詢條件（與 BuildReportSql 保持一致）
            if (!string.IsNullOrEmpty(query.StoreId))
            {
                sql += " AND ei.StoreId = @StoreId";
            }

            if (!string.IsNullOrEmpty(query.StoreFloor))
            {
                sql += " AND s.FloorId = @StoreFloor";
            }

            if (!string.IsNullOrEmpty(query.StoreType))
            {
                sql += " AND s.StoreType = @StoreType";
            }

            if (query.OrderDateFrom.HasValue)
            {
                sql += " AND ei.OrderDate >= @OrderDateFrom";
            }

            if (query.OrderDateTo.HasValue)
            {
                sql += " AND ei.OrderDate <= @OrderDateTo";
            }

            sql += @"
                    GROUP BY ISNULL(s.StoreId, ''), ISNULL(s.StoreName, ''), ISNULL(s.FloorId, ''), ISNULL(s.StoreType, '')
                ) AS grouped";
            
            return sql;
        }

        // ECA4050: 出貨日期統計報表 - 根據開發計劃實現專門查詢（需要計算分組後的數量）
        if (query.ReportType == "ECA4050")
        {
            var sql = @"
                SELECT COUNT(*) FROM (
                    SELECT 
                        ei.OrderShipDate AS OrderShipDate
                    FROM EInvoices ei
                    WHERE ei.ProcessStatus = 'COMPLETED'";
            
            // 添加查詢條件（與 BuildReportSql 保持一致）
            if (query.ShipDateFrom.HasValue)
            {
                sql += " AND ei.OrderShipDate >= @ShipDateFrom";
            }

            if (query.ShipDateTo.HasValue)
            {
                sql += " AND ei.OrderShipDate <= @ShipDateTo";
            }

            if (!string.IsNullOrEmpty(query.StoreId))
            {
                sql += " AND ei.StoreId = @StoreId";
            }

            sql += @"
                    GROUP BY ei.OrderShipDate
                ) AS grouped";
            
            return sql;
        }

        // ECA4060: 訂單日期統計報表 - 根據開發計劃實現專門查詢（需要計算分組後的數量）
        if (query.ReportType == "ECA4060")
        {
            var sql = @"
                SELECT COUNT(*) FROM (
                    SELECT 
                        ei.OrderDate AS OrderDate
                    FROM EInvoices ei
                    WHERE ei.ProcessStatus = 'COMPLETED'";
            
            // 添加查詢條件（與 BuildReportSql 保持一致）
            if (query.OrderDateFrom.HasValue)
            {
                sql += " AND ei.OrderDate >= @OrderDateFrom";
            }

            if (query.OrderDateTo.HasValue)
            {
                sql += " AND ei.OrderDate <= @OrderDateTo";
            }

            if (!string.IsNullOrEmpty(query.StoreId))
            {
                sql += " AND ei.StoreId = @StoreId";
            }

            sql += @"
                    GROUP BY ei.OrderDate
                ) AS grouped";
            
            return sql;
        }

        // 其他報表類型的通用查詢
        var defaultSql = @"
            SELECT COUNT(*) 
            FROM EInvoices ei
            WHERE ei.ProcessStatus = 'COMPLETED'";

        // 添加查詢條件（與 BuildReportSql 保持一致）
        if (!string.IsNullOrEmpty(query.OrderNo))
        {
            defaultSql += " AND ei.OrderNo LIKE @OrderNo";
        }

        if (!string.IsNullOrEmpty(query.RetailerOrderNo))
        {
            defaultSql += " AND ei.RetailerOrderNo LIKE @RetailerOrderNo";
        }

        if (!string.IsNullOrEmpty(query.StoreId))
        {
            defaultSql += " AND ei.StoreId = @StoreId";
        }

        if (!string.IsNullOrEmpty(query.ProviderId))
        {
            defaultSql += " AND ei.ProviderId = @ProviderId";
        }

        if (!string.IsNullOrEmpty(query.GoodsId))
        {
            defaultSql += " AND ei.GoodsId LIKE @GoodsId";
        }

        if (!string.IsNullOrEmpty(query.GoodsName))
        {
            defaultSql += " AND ei.GoodsName LIKE @GoodsName";
        }

        if (query.OrderDateFrom.HasValue)
        {
            defaultSql += " AND ei.OrderDate >= @OrderDateFrom";
        }

        if (query.OrderDateTo.HasValue)
        {
            defaultSql += " AND ei.OrderDate <= @OrderDateTo";
        }

        if (query.ShipDateFrom.HasValue)
        {
            defaultSql += " AND ei.OrderDate >= @ShipDateFrom";
        }

        if (query.ShipDateTo.HasValue)
        {
            defaultSql += " AND ei.OrderDate <= @ShipDateTo";
        }

        if (!string.IsNullOrEmpty(query.OrderStatus))
        {
            defaultSql += " AND ei.OrderStatus = @OrderStatus";
        }

        if (!string.IsNullOrEmpty(query.ProcessStatus))
        {
            defaultSql += " AND ei.ProcessStatus = @ProcessStatus";
        }

        return defaultSql;
    }

    private DynamicParameters BuildReportParameters(EInvoiceReportQuery query)
    {
        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(query.OrderNo))
        {
            parameters.Add("OrderNo", $"%{query.OrderNo}%");
        }

        if (!string.IsNullOrEmpty(query.RetailerOrderNo))
        {
            parameters.Add("RetailerOrderNo", $"%{query.RetailerOrderNo}%");
        }

        if (!string.IsNullOrEmpty(query.StoreId))
        {
            parameters.Add("StoreId", query.StoreId);
        }

        if (!string.IsNullOrEmpty(query.StoreFloor))
        {
            parameters.Add("StoreFloor", query.StoreFloor);
        }

        if (!string.IsNullOrEmpty(query.StoreType))
        {
            parameters.Add("StoreType", query.StoreType);
        }

        if (!string.IsNullOrEmpty(query.RetailerId))
        {
            parameters.Add("RetailerId", query.RetailerId);
        }

        if (!string.IsNullOrEmpty(query.ScId))
        {
            parameters.Add("ScId", query.ScId);
        }

        if (!string.IsNullOrEmpty(query.ProviderId))
        {
            parameters.Add("ProviderId", query.ProviderId);
        }

        if (!string.IsNullOrEmpty(query.GoodsId))
        {
            parameters.Add("GoodsId", $"%{query.GoodsId}%");
        }

        if (!string.IsNullOrEmpty(query.GoodsName))
        {
            parameters.Add("GoodsName", $"%{query.GoodsName}%");
        }

        if (query.OrderDateFrom.HasValue)
        {
            parameters.Add("OrderDateFrom", query.OrderDateFrom);
        }

        if (query.OrderDateTo.HasValue)
        {
            parameters.Add("OrderDateTo", query.OrderDateTo);
        }

        if (query.ShipDateFrom.HasValue)
        {
            parameters.Add("ShipDateFrom", query.ShipDateFrom);
        }

        if (query.ShipDateTo.HasValue)
        {
            parameters.Add("ShipDateTo", query.ShipDateTo);
        }

        if (!string.IsNullOrEmpty(query.OrderStatus))
        {
            parameters.Add("OrderStatus", query.OrderStatus);
        }

        if (!string.IsNullOrEmpty(query.ProcessStatus))
        {
            parameters.Add("ProcessStatus", query.ProcessStatus);
        }

        return parameters;
    }

    // 內部 DTO 類別（避免循環依賴）
    internal class ECA4040ReportDto
    {
        public string? StoreId { get; set; }
        public string? StoreName { get; set; }
        public string? StoreFloor { get; set; }
        public string? StoreType { get; set; }
        public int SumQty { get; set; }
        public decimal SumSubtotal { get; set; }
        public decimal SumFee { get; set; }
        public decimal SalesPercent { get; set; }
        public int SalesRanking { get; set; }
    }

    internal class ECA4050ReportDto
    {
        public DateTime? OrderShipDate { get; set; }
        public int SumQty { get; set; }
        public decimal SumSubtotal { get; set; }
        public decimal SumFee { get; set; }
        public decimal SalesPercent { get; set; }
    }

    internal class ECA4060ReportDto
    {
        public DateTime? OrderDate { get; set; }
        public int SumYQty { get; set; }
        public decimal SumYSubtotal { get; set; }
        public decimal SumYFee { get; set; }
        public decimal SalesYPercent { get; set; }
        public int SumNQty { get; set; }
        public decimal SumNSubtotal { get; set; }
        public decimal SumNFee { get; set; }
        public decimal SalesNPercent { get; set; }
        public int SumAQty { get; set; }
        public decimal SumASubtotal { get; set; }
        public decimal SumAFee { get; set; }
        public decimal SalesAPercent { get; set; }
    }
}

