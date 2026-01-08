using Dapper;
using ErpCore.Domain.Entities.EInvoice;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using System.Data;

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

    public async Task<List<EInvoice>> GetEInvoicesByUploadIdAsync(long uploadId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM EInvoices 
                WHERE UploadId = @UploadId
                ORDER BY InvoiceId";

            var items = await QueryAsync<EInvoice>(sql, new { UploadId = uploadId });
            return items.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢電子發票列表失敗(上傳記錄ID): {uploadId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<EInvoice>> GetEInvoicesPagedAsync(EInvoiceQuery query)
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

            var items = await QueryAsync<EInvoice>(sql, parameters);

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

            return new PagedResult<EInvoice>
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

    public async Task<EInvoice?> GetEInvoiceByIdAsync(long invoiceId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM EInvoices 
                WHERE InvoiceId = @InvoiceId";

            return await QueryFirstOrDefaultAsync<EInvoice>(sql, new { InvoiceId = invoiceId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢電子發票失敗: {invoiceId}", ex);
            throw;
        }
    }

    public async Task<long> CreateEInvoiceAsync(EInvoice invoice)
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

    public async Task UpdateEInvoiceAsync(EInvoice invoice)
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

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "OrderDate" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<EInvoiceReportDto>(sql, parameters);

            // 查詢總數
            var countSql = BuildReportCountSql(query);
            var countParameters = BuildReportParameters(query);
            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<EInvoiceReportDto>
            {
                Items = items.ToList(),
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
        var sql = @"
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

        // 添加查詢條件
        if (!string.IsNullOrEmpty(query.OrderNo))
        {
            sql += " AND ei.OrderNo LIKE @OrderNo";
        }

        if (!string.IsNullOrEmpty(query.RetailerOrderNo))
        {
            sql += " AND ei.RetailerOrderNo LIKE @RetailerOrderNo";
        }

        if (!string.IsNullOrEmpty(query.StoreId))
        {
            sql += " AND ei.StoreId = @StoreId";
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

        if (query.ShipDateFrom.HasValue)
        {
            sql += " AND ei.OrderDate >= @ShipDateFrom";
        }

        if (query.ShipDateTo.HasValue)
        {
            sql += " AND ei.OrderDate <= @ShipDateTo";
        }

        if (!string.IsNullOrEmpty(query.OrderStatus))
        {
            sql += " AND ei.OrderStatus = @OrderStatus";
        }

        if (!string.IsNullOrEmpty(query.ProcessStatus))
        {
            sql += " AND ei.ProcessStatus = @ProcessStatus";
        }

        return sql;
    }

    private string BuildReportCountSql(EInvoiceReportQuery query)
    {
        var sql = @"
            SELECT COUNT(*) 
            FROM EInvoices ei
            WHERE ei.ProcessStatus = 'COMPLETED'";

        // 添加查詢條件（與 BuildReportSql 保持一致）
        if (!string.IsNullOrEmpty(query.OrderNo))
        {
            sql += " AND ei.OrderNo LIKE @OrderNo";
        }

        if (!string.IsNullOrEmpty(query.RetailerOrderNo))
        {
            sql += " AND ei.RetailerOrderNo LIKE @RetailerOrderNo";
        }

        if (!string.IsNullOrEmpty(query.StoreId))
        {
            sql += " AND ei.StoreId = @StoreId";
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

        if (query.ShipDateFrom.HasValue)
        {
            sql += " AND ei.OrderDate >= @ShipDateFrom";
        }

        if (query.ShipDateTo.HasValue)
        {
            sql += " AND ei.OrderDate <= @ShipDateTo";
        }

        if (!string.IsNullOrEmpty(query.OrderStatus))
        {
            sql += " AND ei.OrderStatus = @OrderStatus";
        }

        if (!string.IsNullOrEmpty(query.ProcessStatus))
        {
            sql += " AND ei.ProcessStatus = @ProcessStatus";
        }

        return sql;
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
}

