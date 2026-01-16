using ErpCore.Application.DTOs.EInvoice;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.EInvoice;
using ErpCore.Infrastructure.Repositories.EInvoice;
using ErpCore.Shared.Common;
using ErpCore.Application.Common;
using ErpCore.Shared.Logging;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System.Text;

namespace ErpCore.Application.Services.EInvoice;

/// <summary>
/// 電子發票服務實作
/// </summary>
public class EInvoiceService : BaseService, IEInvoiceService
{
    private readonly IEInvoiceRepository _repository;
    private readonly ExportHelper _exportHelper;

    public EInvoiceService(
        IEInvoiceRepository repository,
        ILoggerService logger,
        IUserContext userContext,
        ExportHelper exportHelper) : base(logger, userContext)
    {
        _repository = repository;
        _exportHelper = exportHelper;
    }

    public async Task<EInvoiceUploadDto> UploadFileAsync(IFormFile file, string? storeId, string? retailerId, string? uploadType)
    {
        try
        {
            // 驗證檔案
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("檔案不能為空");
            }

            // 驗證檔案類型
            var allowedExtensions = new[] { ".xlsx", ".xls", ".xml" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new ArgumentException($"不支援的檔案格式: {fileExtension}");
            }

            // 驗證檔案大小 (50MB)
            const long maxFileSize = 50 * 1024 * 1024;
            if (file.Length > maxFileSize)
            {
                throw new ArgumentException($"檔案大小超過限制: {file.Length} bytes");
            }

            // 建立上傳記錄
            var upload = new EInvoiceUpload
            {
                FileName = file.FileName,
                FileSize = file.Length,
                FileType = fileExtension == ".xml" ? "XML" : "Excel",
                UploadDate = DateTime.Now,
                UploadBy = GetCurrentUserId(),
                Status = "PENDING",
                TotalRecords = 0,
                SuccessRecords = 0,
                FailedRecords = 0,
                StoreId = storeId,
                RetailerId = retailerId,
                UploadType = uploadType ?? "ECA3010",
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var uploadId = await _repository.CreateUploadAsync(upload);
            upload.UploadId = uploadId;

            // 保存上傳的檔案
            var filePath = GetUploadFilePath(uploadId, file.FileName);
            var uploadDir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(uploadDir) && !Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            _logger.LogInfo($"電子發票檔案上傳成功: {file.FileName}, UploadId: {uploadId}, FilePath: {filePath}");

            return MapToUploadDto(upload);
        }
        catch (Exception ex)
        {
            _logger.LogError($"上傳電子發票檔案失敗: {file?.FileName}", ex);
            throw;
        }
    }

    public async Task<PagedResult<EInvoiceUploadDto>> GetUploadsAsync(EInvoiceUploadQueryDto query)
    {
        try
        {
            var repositoryQuery = new EInvoiceUploadQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                Status = query.Status,
                UploadBy = query.UploadBy,
                StoreId = query.StoreId,
                StartDate = query.StartDate,
                EndDate = query.EndDate,
                UploadType = query.UploadType
            };

            var result = await _repository.GetUploadsPagedAsync(repositoryQuery);

            var dtos = result.Items.Select(x => MapToUploadDto(x)).ToList();

            return new PagedResult<EInvoiceUploadDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢上傳記錄列表失敗", ex);
            throw;
        }
    }

    public async Task<EInvoiceUploadDto> GetUploadAsync(long uploadId)
    {
        try
        {
            var entity = await _repository.GetUploadByIdAsync(uploadId);
            if (entity == null)
            {
                throw new InvalidOperationException($"上傳記錄不存在: {uploadId}");
            }

            return MapToUploadDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢上傳記錄失敗: {uploadId}", ex);
            throw;
        }
    }

    public async Task<EInvoiceProcessStatusDto> GetProcessStatusAsync(long uploadId)
    {
        try
        {
            var upload = await _repository.GetUploadByIdAsync(uploadId);
            if (upload == null)
            {
                throw new InvalidOperationException($"上傳記錄不存在: {uploadId}");
            }

            var currentProcessed = upload.SuccessRecords + upload.FailedRecords;
            var progress = upload.TotalRecords > 0 
                ? (double)currentProcessed / upload.TotalRecords * 100 
                : 0;

            DateTime? estimatedEndDate = null;
            if (upload.ProcessStartDate.HasValue && upload.Status == "PROCESSING" && currentProcessed > 0)
            {
                var elapsed = DateTime.Now - upload.ProcessStartDate.Value;
                var avgTimePerRecord = elapsed.TotalMilliseconds / currentProcessed;
                var remainingRecords = upload.TotalRecords - currentProcessed;
                var estimatedRemaining = TimeSpan.FromMilliseconds(avgTimePerRecord * remainingRecords);
                estimatedEndDate = DateTime.Now.Add(estimatedRemaining);
            }

            return new EInvoiceProcessStatusDto
            {
                UploadId = uploadId,
                Status = upload.Status,
                TotalRecords = upload.TotalRecords,
                SuccessRecords = upload.SuccessRecords,
                FailedRecords = upload.FailedRecords,
                CurrentProcessed = currentProcessed,
                Progress = progress,
                ProcessStartDate = upload.ProcessStartDate,
                EstimatedEndDate = estimatedEndDate
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢處理狀態失敗: {uploadId}", ex);
            throw;
        }
    }

    public async Task StartProcessAsync(long uploadId)
    {
        try
        {
            var upload = await _repository.GetUploadByIdAsync(uploadId);
            if (upload == null)
            {
                throw new InvalidOperationException($"上傳記錄不存在: {uploadId}");
            }

            if (upload.Status != "PENDING")
            {
                throw new InvalidOperationException($"上傳記錄狀態不正確: {upload.Status}");
            }

            // 更新狀態為處理中
            upload.Status = "PROCESSING";
            upload.ProcessStartDate = DateTime.Now;
            upload.UpdatedBy = GetCurrentUserId();
            upload.UpdatedAt = DateTime.Now;

            await _repository.UpdateUploadAsync(upload);

            _logger.LogInfo($"開始處理電子發票檔案: UploadId: {uploadId}, FileName: {upload.FileName}");

            // 取得檔案路徑（檔案應該已在上傳時保存）
            var filePath = GetUploadFilePath(uploadId, upload.FileName);
            
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"上傳的檔案不存在: {filePath}");
            }

            // 讀取並解析檔案
            List<EInvoice> invoices;
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                invoices = await EInvoiceFileParser.ParseEInvoiceFileAsync(
                    fileStream, 
                    upload.FileName, 
                    upload.UploadType);
            }

            // 更新總筆數
            upload.TotalRecords = invoices.Count;
            await _repository.UpdateUploadAsync(upload);

            // 批次建立電子發票記錄
            int successCount = 0;
            int failedCount = 0;
            var errorMessages = new StringBuilder();

            foreach (var invoice in invoices)
            {
                try
                {
                    invoice.UploadId = uploadId;
                    invoice.CreatedBy = GetCurrentUserId();
                    invoice.CreatedAt = DateTime.Now;
                    invoice.UpdatedBy = GetCurrentUserId();
                    invoice.UpdatedAt = DateTime.Now;
                    invoice.ProcessStatus = "COMPLETED";

                    await _repository.CreateEInvoiceAsync(invoice);
                    successCount++;
                }
                catch (Exception ex)
                {
                    failedCount++;
                    var errorMsg = $"訂單編號 {invoice.OrderNo} 處理失敗: {ex.Message}";
                    errorMessages.AppendLine(errorMsg);
                    _logger.LogError(errorMsg, ex);
                }
            }

            // 更新處理結果
            upload.SuccessRecords = successCount;
            upload.FailedRecords = failedCount;
            upload.Status = failedCount == 0 ? "COMPLETED" : (successCount > 0 ? "COMPLETED" : "FAILED");
            upload.ProcessEndDate = DateTime.Now;
            
            if (errorMessages.Length > 0)
            {
                upload.ErrorMessage = errorMessages.ToString();
                upload.ProcessLog = $"成功: {successCount}, 失敗: {failedCount}\n{errorMessages}";
            }
            else
            {
                upload.ProcessLog = $"成功處理 {successCount} 筆記錄";
            }

            upload.UpdatedBy = GetCurrentUserId();
            upload.UpdatedAt = DateTime.Now;

            await _repository.UpdateUploadAsync(upload);

            _logger.LogInfo($"電子發票檔案處理完成: UploadId: {uploadId}, 成功: {successCount}, 失敗: {failedCount}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"處理電子發票檔案失敗: {uploadId}", ex);
            
            // 更新狀態為失敗
            try
            {
                var upload = await _repository.GetUploadByIdAsync(uploadId);
                if (upload != null)
                {
                    upload.Status = "FAILED";
                    upload.ErrorMessage = ex.Message;
                    upload.ProcessEndDate = DateTime.Now;
                    upload.UpdatedBy = GetCurrentUserId();
                    upload.UpdatedAt = DateTime.Now;
                    await _repository.UpdateUploadAsync(upload);
                }
            }
            catch (Exception updateEx)
            {
                _logger.LogError($"更新上傳記錄狀態失敗: {uploadId}", updateEx);
            }

            throw;
        }
    }

    /// <summary>
    /// 取得上傳檔案路徑
    /// </summary>
    private string GetUploadFilePath(long uploadId, string fileName)
    {
        // 檔案儲存目錄（可從設定檔讀取）
        var uploadDir = Path.Combine(
            Directory.GetCurrentDirectory(),
            "Uploads",
            "EInvoice",
            uploadId.ToString());
        
        // 確保目錄存在
        if (!Directory.Exists(uploadDir))
        {
            Directory.CreateDirectory(uploadDir);
        }

        return Path.Combine(uploadDir, fileName);
    }

    public async Task<PagedResult<EInvoiceDto>> GetEInvoicesAsync(EInvoiceQueryDto query)
    {
        try
        {
            var repositoryQuery = new EInvoiceQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                UploadId = query.UploadId,
                OrderNo = query.OrderNo,
                RetailerOrderNo = query.RetailerOrderNo,
                RetailerOrderDetailNo = query.RetailerOrderDetailNo,
                StoreId = query.StoreId,
                ProviderId = query.ProviderId,
                OrderDateFrom = query.OrderDateFrom,
                OrderDateTo = query.OrderDateTo,
                ProcessStatus = query.ProcessStatus,
                OrderStatus = query.OrderStatus,
                GoodsId = query.GoodsId,
                GoodsName = query.GoodsName,
                SpecId = query.SpecId,
                ProviderGoodsId = query.ProviderGoodsId,
                NdType = query.NdType
            };

            // 使用包含名稱的查詢方法
            var result = await _repository.GetEInvoicesWithNamesPagedAsync(repositoryQuery);

            var dtos = result.Items.Select(x => MapToDtoFromWithNames(x)).ToList();

            return new PagedResult<EInvoiceDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢電子發票列表失敗", ex);
            throw;
        }
    }

    public async Task<EInvoiceDto> GetEInvoiceAsync(long invoiceId)
    {
        try
        {
            var entity = await _repository.GetEInvoiceByIdAsync(invoiceId);
            if (entity == null)
            {
                throw new InvalidOperationException($"電子發票不存在: {invoiceId}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢電子發票失敗: {invoiceId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 匯出電子發票查詢結果到 Excel (ECA3020)
    /// </summary>
    public async Task<byte[]> ExportEInvoicesToExcelAsync(EInvoiceQueryDto query)
    {
        try
        {
            // 查詢所有資料（不分頁）
            var allDataQuery = new EInvoiceQueryDto
            {
                PageIndex = 1,
                PageSize = int.MaxValue,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                UploadId = query.UploadId,
                OrderNo = query.OrderNo,
                RetailerOrderNo = query.RetailerOrderNo,
                RetailerOrderDetailNo = query.RetailerOrderDetailNo,
                StoreId = query.StoreId,
                ProviderId = query.ProviderId,
                OrderDateFrom = query.OrderDateFrom,
                OrderDateTo = query.OrderDateTo,
                ProcessStatus = query.ProcessStatus,
                OrderStatus = query.OrderStatus,
                GoodsId = query.GoodsId,
                GoodsName = query.GoodsName,
                SpecId = query.SpecId,
                ProviderGoodsId = query.ProviderGoodsId,
                NdType = query.NdType
            };

            var result = await GetEInvoicesAsync(allDataQuery);

            // 定義匯出欄位
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "InvoiceId", DisplayName = "發票ID", DataType = ExportDataType.Number },
                new ExportColumn { PropertyName = "OrderNo", DisplayName = "訂單編號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "RetailerOrderNo", DisplayName = "零售商訂單編號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "RetailerOrderDetailNo", DisplayName = "零售商訂單明細編號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "OrderDate", DisplayName = "訂單日期", DataType = ExportDataType.Date },
                new ExportColumn { PropertyName = "StoreId", DisplayName = "店別代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "StoreName", DisplayName = "店別名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ProviderId", DisplayName = "供應商代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ProviderName", DisplayName = "供應商名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "NdType", DisplayName = "類型", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsId", DisplayName = "商品編號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsName", DisplayName = "商品名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SpecId", DisplayName = "規格ID", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ProviderGoodsId", DisplayName = "供應商商品編號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SpecColor", DisplayName = "規格顏色", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SpecSize", DisplayName = "規格尺寸", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SuggestPrice", DisplayName = "建議售價", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "InternetPrice", DisplayName = "網路售價", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "ShippingType", DisplayName = "運送方式", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ShippingFee", DisplayName = "運費", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "OrderQty", DisplayName = "訂單數量", DataType = ExportDataType.Number },
                new ExportColumn { PropertyName = "OrderShippingFee", DisplayName = "訂單運費", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "OrderSubtotal", DisplayName = "訂單小計", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "OrderTotal", DisplayName = "訂單總計", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "OrderStatus", DisplayName = "訂單狀態", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ProcessStatus", DisplayName = "處理狀態", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "CreatedAt", DisplayName = "建立時間", DataType = ExportDataType.DateTime }
            };

            return _exportHelper.ExportToExcel(result.Items, columns, "電子發票查詢", "電子發票查詢結果");
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出電子發票查詢結果到 Excel 失敗", ex);
            throw;
        }
    }

    /// <summary>
    /// 匯出電子發票查詢結果到 PDF (ECA3020)
    /// </summary>
    public async Task<byte[]> ExportEInvoicesToPdfAsync(EInvoiceQueryDto query)
    {
        try
        {
            // 查詢所有資料（不分頁）
            var allDataQuery = new EInvoiceQueryDto
            {
                PageIndex = 1,
                PageSize = int.MaxValue,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                UploadId = query.UploadId,
                OrderNo = query.OrderNo,
                RetailerOrderNo = query.RetailerOrderNo,
                RetailerOrderDetailNo = query.RetailerOrderDetailNo,
                StoreId = query.StoreId,
                ProviderId = query.ProviderId,
                OrderDateFrom = query.OrderDateFrom,
                OrderDateTo = query.OrderDateTo,
                ProcessStatus = query.ProcessStatus,
                OrderStatus = query.OrderStatus,
                GoodsId = query.GoodsId,
                GoodsName = query.GoodsName,
                SpecId = query.SpecId,
                ProviderGoodsId = query.ProviderGoodsId,
                NdType = query.NdType
            };

            var result = await GetEInvoicesAsync(allDataQuery);

            // 定義匯出欄位
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "InvoiceId", DisplayName = "發票ID", DataType = ExportDataType.Number },
                new ExportColumn { PropertyName = "OrderNo", DisplayName = "訂單編號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "RetailerOrderNo", DisplayName = "零售商訂單編號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "OrderDate", DisplayName = "訂單日期", DataType = ExportDataType.Date },
                new ExportColumn { PropertyName = "StoreName", DisplayName = "店別名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ProviderName", DisplayName = "供應商名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsId", DisplayName = "商品編號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsName", DisplayName = "商品名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "OrderQty", DisplayName = "訂單數量", DataType = ExportDataType.Number },
                new ExportColumn { PropertyName = "OrderTotal", DisplayName = "訂單總計", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "OrderStatus", DisplayName = "訂單狀態", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ProcessStatus", DisplayName = "處理狀態", DataType = ExportDataType.String }
            };

            return _exportHelper.ExportToPdf(result.Items, columns, "電子發票查詢結果");
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出電子發票查詢結果到 PDF 失敗", ex);
            throw;
        }
    }

    public async Task DeleteUploadAsync(long uploadId)
    {
        try
        {
            var upload = await _repository.GetUploadByIdAsync(uploadId);
            if (upload == null)
            {
                throw new InvalidOperationException($"上傳記錄不存在: {uploadId}");
            }

            if (upload.Status == "PROCESSING" || upload.Status == "COMPLETED")
            {
                throw new InvalidOperationException($"無法刪除狀態為 {upload.Status} 的上傳記錄");
            }

            await _repository.DeleteUploadAsync(uploadId);

            _logger.LogInfo($"刪除上傳記錄成功: UploadId: {uploadId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除上傳記錄失敗: {uploadId}", ex);
            throw;
        }
    }

    public async Task<(byte[] fileBytes, string fileName, string contentType)> DownloadUploadFileAsync(long uploadId)
    {
        try
        {
            var upload = await _repository.GetUploadByIdAsync(uploadId);
            if (upload == null)
            {
                throw new InvalidOperationException($"上傳記錄不存在: {uploadId}");
            }

            var filePath = GetUploadFilePath(uploadId, upload.FileName);
            
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"上傳的檔案不存在: {filePath}");
            }

            var fileBytes = await File.ReadAllBytesAsync(filePath);
            var contentType = GetContentType(upload.FileName);

            _logger.LogInfo($"下載上傳檔案成功: UploadId: {uploadId}, FileName: {upload.FileName}");

            return (fileBytes, upload.FileName, contentType);
        }
        catch (Exception ex)
        {
            _logger.LogError($"下載上傳檔案失敗: {uploadId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 取得檔案內容類型
    /// </summary>
    private string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLower();
        return extension switch
        {
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".xls" => "application/vnd.ms-excel",
            ".xml" => "text/xml",
            _ => "application/octet-stream"
        };
    }

    public async Task<PagedResult<ErpCore.Application.DTOs.EInvoice.EInvoiceReportDto>> GetEInvoiceReportsAsync(EInvoiceReportQueryDto query)
    {
        try
        {
            var repositoryQuery = new EInvoiceReportQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                ReportType = query.ReportType,
                OrderNo = query.OrderNo,
                RetailerOrderNo = query.RetailerOrderNo,
                StoreId = query.StoreId,
                StoreFloor = query.StoreFloor,
                StoreType = query.StoreType,
                RetailerId = query.RetailerId,
                ScId = query.ScId,
                ProviderId = query.ProviderId,
                GoodsId = query.GoodsId,
                GoodsName = query.GoodsName,
                OrderDateFrom = query.OrderDateFrom,
                OrderDateTo = query.OrderDateTo,
                ShipDateFrom = query.ShipDateFrom,
                ShipDateTo = query.ShipDateTo,
                OrderStatus = query.OrderStatus,
                ProcessStatus = query.ProcessStatus
            };

            var result = await _repository.GetEInvoiceReportsPagedAsync(repositoryQuery);

            return new PagedResult<ErpCore.Application.DTOs.EInvoice.EInvoiceReportDto>
            {
                Items = result.Items.Select(r => new ErpCore.Application.DTOs.EInvoice.EInvoiceReportDto
                {
                    ReportId = r.ReportId,
                    ReportType = r.ReportType,
                    OrderNo = r.OrderNo,
                    RetailerOrderNo = r.RetailerOrderNo,
                    StoreId = r.StoreId,
                    StoreFloor = r.StoreFloor,
                    StoreType = r.StoreType,
                    RetailerId = r.RetailerId,
                    ScId = r.ScId,
                    ProviderId = r.ProviderId,
                    GoodsId = r.GoodsId,
                    GoodsName = r.GoodsName,
                    OrderDate = r.OrderDate,
                    ShipDate = r.ShipDate,
                    OrderStatus = r.OrderStatus,
                    ProcessStatus = r.ProcessStatus,
                    CreatedAt = r.CreatedAt
                }).ToList(),
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢電子發票報表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportEInvoiceReportsToExcelAsync(EInvoiceReportQueryDto query)
    {
        try
        {
            // 查詢所有資料（不分頁）
            var allDataQuery = new EInvoiceReportQueryDto
            {
                PageIndex = 1,
                PageSize = int.MaxValue,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                ReportType = query.ReportType,
                OrderNo = query.OrderNo,
                RetailerOrderNo = query.RetailerOrderNo,
                StoreId = query.StoreId,
                StoreFloor = query.StoreFloor,
                StoreType = query.StoreType,
                RetailerId = query.RetailerId,
                ScId = query.ScId,
                ProviderId = query.ProviderId,
                GoodsId = query.GoodsId,
                GoodsName = query.GoodsName,
                OrderDateFrom = query.OrderDateFrom,
                OrderDateTo = query.OrderDateTo,
                ShipDateFrom = query.ShipDateFrom,
                ShipDateTo = query.ShipDateTo,
                OrderStatus = query.OrderStatus,
                ProcessStatus = query.ProcessStatus
            };

            var result = await GetEInvoiceReportsAsync(allDataQuery);

            // 根據報表類型定義不同的匯出欄位
            var columns = GetExportColumnsForReportType(query.ReportType);

            var reportTypeName = GetReportTypeName(query.ReportType);
            return _exportHelper.ExportToExcel(result.Items, columns, reportTypeName, $"電子發票報表 - {reportTypeName}");
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出電子發票報表到 Excel 失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportEInvoiceReportsToPdfAsync(EInvoiceReportQueryDto query)
    {
        try
        {
            // 查詢所有資料（不分頁）
            var allDataQuery = new EInvoiceReportQueryDto
            {
                PageIndex = 1,
                PageSize = int.MaxValue,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                ReportType = query.ReportType,
                OrderNo = query.OrderNo,
                RetailerOrderNo = query.RetailerOrderNo,
                StoreId = query.StoreId,
                StoreFloor = query.StoreFloor,
                StoreType = query.StoreType,
                RetailerId = query.RetailerId,
                ScId = query.ScId,
                ProviderId = query.ProviderId,
                GoodsId = query.GoodsId,
                GoodsName = query.GoodsName,
                OrderDateFrom = query.OrderDateFrom,
                OrderDateTo = query.OrderDateTo,
                ShipDateFrom = query.ShipDateFrom,
                ShipDateTo = query.ShipDateTo,
                OrderStatus = query.OrderStatus,
                ProcessStatus = query.ProcessStatus
            };

            var result = await GetEInvoiceReportsAsync(allDataQuery);

            // 根據報表類型定義不同的匯出欄位
            var columns = GetExportColumnsForReportType(query.ReportType);

            var reportTypeName = GetReportTypeName(query.ReportType);
            return _exportHelper.ExportToPdf(result.Items, columns, $"電子發票報表 - {reportTypeName}");
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出電子發票報表到 PDF 失敗", ex);
            throw;
        }
    }

    /// <summary>
    /// 根據報表類型取得匯出欄位定義
    /// </summary>
    private List<ExportColumn> GetExportColumnsForReportType(string? reportType)
    {
        // ECA4020: 商品銷售統計報表 - 根據開發計劃實現專門欄位
        if (reportType == "ECA4020")
        {
            return new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "SalesRanking", DisplayName = "排名", DataType = ExportDataType.Number },
                new ExportColumn { PropertyName = "ScId", DisplayName = "專櫃代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ScName", DisplayName = "專櫃名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsId", DisplayName = "商品編號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsName", DisplayName = "商品名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ProviderGoodsId", DisplayName = "供應商商品編號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "OrderQty", DisplayName = "銷售數量", DataType = ExportDataType.Number },
                new ExportColumn { PropertyName = "OrderSubtotal", DisplayName = "銷售金額", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "ShippingFee", DisplayName = "運費", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "AvgPrice", DisplayName = "平均價格", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "SalesPercent", DisplayName = "銷售占比(%)", DataType = ExportDataType.Decimal }
            };
        }

        // ECA4030: 零售商銷售統計報表 - 根據開發計劃實現專門欄位
        if (reportType == "ECA4030")
        {
            return new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "SalesRanking", DisplayName = "排名", DataType = ExportDataType.Number },
                new ExportColumn { PropertyName = "RetailerId", DisplayName = "零售商代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "RetailerName", DisplayName = "零售商名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "OrderQty", DisplayName = "銷售數量", DataType = ExportDataType.Number },
                new ExportColumn { PropertyName = "OrderSubtotal", DisplayName = "銷售金額", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "ShippingFee", DisplayName = "運費", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "SalesPercent", DisplayName = "銷售占比(%)", DataType = ExportDataType.Decimal }
            };
        }

        // ECA4040: 店別銷售統計報表 - 根據開發計劃實現專門欄位
        if (reportType == "ECA4040")
        {
            return new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "SalesRanking", DisplayName = "排名", DataType = ExportDataType.Number },
                new ExportColumn { PropertyName = "StoreId", DisplayName = "店別代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "StoreName", DisplayName = "店別名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "StoreFloor", DisplayName = "樓層", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "StoreType", DisplayName = "類型", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "OrderQty", DisplayName = "銷售數量", DataType = ExportDataType.Number },
                new ExportColumn { PropertyName = "OrderSubtotal", DisplayName = "銷售金額", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "ShippingFee", DisplayName = "運費", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "SalesPercent", DisplayName = "銷售占比(%)", DataType = ExportDataType.Decimal }
            };
        }

        // ECA4050: 出貨日期統計報表 - 根據開發計劃實現專門欄位
        if (reportType == "ECA4050")
        {
            return new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "ShipDate", DisplayName = "出貨日期", DataType = ExportDataType.Date },
                new ExportColumn { PropertyName = "OrderQty", DisplayName = "銷售數量", DataType = ExportDataType.Number },
                new ExportColumn { PropertyName = "OrderSubtotal", DisplayName = "銷售金額", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "ShippingFee", DisplayName = "運費", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "SalesPercent", DisplayName = "銷售占比(%)", DataType = ExportDataType.Decimal }
            };
        }

        // ECA4060: 訂單日期統計報表 - 根據開發計劃實現專門欄位
        if (reportType == "ECA4060")
        {
            return new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "OrderDate", DisplayName = "訂單日期", DataType = ExportDataType.Date },
                new ExportColumn { PropertyName = "SumYQty", DisplayName = "已開立數量", DataType = ExportDataType.Number },
                new ExportColumn { PropertyName = "SumYSubtotal", DisplayName = "已開立金額", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "SumYFee", DisplayName = "已開立運費", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "SalesYPercent", DisplayName = "已開立占比(%)", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "SumNQty", DisplayName = "未開立數量", DataType = ExportDataType.Number },
                new ExportColumn { PropertyName = "SumNSubtotal", DisplayName = "未開立金額", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "SumNFee", DisplayName = "未開立運費", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "SalesNPercent", DisplayName = "未開立占比(%)", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "SumAQty", DisplayName = "全部數量", DataType = ExportDataType.Number },
                new ExportColumn { PropertyName = "SumASubtotal", DisplayName = "全部金額", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "SumAFee", DisplayName = "全部運費", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "SalesAPercent", DisplayName = "全部占比(%)", DataType = ExportDataType.Decimal }
            };
        }

        // 根據不同的報表類型返回不同的欄位定義
        // 預設返回所有欄位
        return new List<ExportColumn>
        {
            new ExportColumn { PropertyName = "InvoiceId", DisplayName = "發票ID", DataType = ExportDataType.Number },
            new ExportColumn { PropertyName = "OrderNo", DisplayName = "訂單編號", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "OrderDate", DisplayName = "訂單日期", DataType = ExportDataType.Date },
            new ExportColumn { PropertyName = "ShipDate", DisplayName = "出貨日期", DataType = ExportDataType.Date },
            new ExportColumn { PropertyName = "OrderStatus", DisplayName = "訂單狀態", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "OrderQty", DisplayName = "訂單數量", DataType = ExportDataType.Number },
            new ExportColumn { PropertyName = "OrderSubtotal", DisplayName = "訂單小計", DataType = ExportDataType.Decimal },
            new ExportColumn { PropertyName = "OrderTotal", DisplayName = "訂單總額", DataType = ExportDataType.Decimal },
            new ExportColumn { PropertyName = "StoreId", DisplayName = "店別代碼", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "StoreName", DisplayName = "店別名稱", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "RetailerId", DisplayName = "零售商代碼", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "RetailerName", DisplayName = "零售商名稱", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "ProviderId", DisplayName = "供應商代碼", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "ProviderName", DisplayName = "供應商名稱", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "GoodsId", DisplayName = "商品編號", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "GoodsName", DisplayName = "商品名稱", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "SpecId", DisplayName = "規格ID", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "ProviderGoodsId", DisplayName = "供應商商品編號", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "SuggestPrice", DisplayName = "建議售價", DataType = ExportDataType.Decimal },
            new ExportColumn { PropertyName = "InternetPrice", DisplayName = "網路售價", DataType = ExportDataType.Decimal },
            new ExportColumn { PropertyName = "ShippingFee", DisplayName = "運費", DataType = ExportDataType.Decimal },
            new ExportColumn { PropertyName = "ShippingType", DisplayName = "運送方式", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "TotalQty", DisplayName = "總數量", DataType = ExportDataType.Number },
            new ExportColumn { PropertyName = "TotalAmount", DisplayName = "總金額", DataType = ExportDataType.Decimal },
            new ExportColumn { PropertyName = "TotalOrders", DisplayName = "總訂單數", DataType = ExportDataType.Number }
        };
    }

    /// <summary>
    /// 取得報表類型名稱
    /// </summary>
    private string GetReportTypeName(string? reportType)
    {
        return reportType switch
        {
            "ECA4010" => "訂單明細",
            "ECA4020" => "商品銷售統計",
            "ECA4030" => "零售商銷售統計",
            "ECA4040" => "店別銷售統計",
            "ECA4050" => "出貨日期統計",
            "ECA4060" => "訂單日期統計",
            _ => "電子發票報表"
        };
    }

    private EInvoiceUploadDto MapToUploadDto(EInvoiceUpload entity)
    {
        return new EInvoiceUploadDto
        {
            UploadId = entity.UploadId,
            FileName = entity.FileName,
            FileSize = entity.FileSize,
            FileType = entity.FileType,
            UploadDate = entity.UploadDate,
            UploadBy = entity.UploadBy,
            Status = entity.Status,
            ProcessStartDate = entity.ProcessStartDate,
            ProcessEndDate = entity.ProcessEndDate,
            TotalRecords = entity.TotalRecords,
            SuccessRecords = entity.SuccessRecords,
            FailedRecords = entity.FailedRecords,
            ErrorMessage = entity.ErrorMessage,
            ProcessLog = entity.ProcessLog,
            StoreId = entity.StoreId,
            RetailerId = entity.RetailerId,
            UploadType = entity.UploadType,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }

    private EInvoiceDto MapToDto(ErpCore.Domain.Entities.EInvoice.EInvoice entity)
    {
        return new EInvoiceDto
        {
            InvoiceId = entity.InvoiceId,
            UploadId = entity.UploadId,
            OrderNo = entity.OrderNo,
            RetailerOrderNo = entity.RetailerOrderNo,
            RetailerOrderDetailNo = entity.RetailerOrderDetailNo,
            OrderDate = entity.OrderDate,
            StoreId = entity.StoreId,
            ProviderId = entity.ProviderId,
            NdType = entity.NdType,
            GoodsId = entity.GoodsId,
            GoodsName = entity.GoodsName,
            SpecId = entity.SpecId,
            ProviderGoodsId = entity.ProviderGoodsId,
            SpecColor = entity.SpecColor,
            SpecSize = entity.SpecSize,
            SuggestPrice = entity.SuggestPrice,
            InternetPrice = entity.InternetPrice,
            ShippingType = entity.ShippingType,
            ShippingFee = entity.ShippingFee,
            OrderQty = entity.OrderQty,
            OrderShippingFee = entity.OrderShippingFee,
            OrderSubtotal = entity.OrderSubtotal,
            OrderTotal = entity.OrderTotal,
            OrderStatus = entity.OrderStatus,
            ProcessStatus = entity.ProcessStatus,
            ErrorMessage = entity.ErrorMessage,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }

    private EInvoiceDto MapToDtoFromWithNames(Infrastructure.Repositories.EInvoice.EInvoiceWithNamesDto item)
    {
        return new EInvoiceDto
        {
            InvoiceId = item.InvoiceId,
            UploadId = item.UploadId,
            OrderNo = item.OrderNo,
            RetailerOrderNo = item.RetailerOrderNo,
            RetailerOrderDetailNo = item.RetailerOrderDetailNo,
            OrderDate = item.OrderDate,
            StoreId = item.StoreId,
            StoreName = item.StoreName,
            ProviderId = item.ProviderId,
            ProviderName = item.ProviderName,
            NdType = item.NdType,
            GoodsId = item.GoodsId,
            GoodsName = item.GoodsName,
            SpecId = item.SpecId,
            ProviderGoodsId = item.ProviderGoodsId,
            SpecColor = item.SpecColor,
            SpecSize = item.SpecSize,
            SuggestPrice = item.SuggestPrice,
            InternetPrice = item.InternetPrice,
            ShippingType = item.ShippingType,
            ShippingFee = item.ShippingFee,
            OrderQty = item.OrderQty,
            OrderShippingFee = item.OrderShippingFee,
            OrderSubtotal = item.OrderSubtotal,
            OrderTotal = item.OrderTotal,
            OrderStatus = item.OrderStatus,
            ProcessStatus = item.ProcessStatus,
            ErrorMessage = item.ErrorMessage,
            CreatedBy = item.CreatedBy,
            CreatedAt = item.CreatedAt,
            UpdatedBy = item.UpdatedBy,
            UpdatedAt = item.UpdatedAt
        };
    }

    /// <summary>
    /// 下載處理結果 (成功/失敗清單) (ECA3010)
    /// </summary>
    public async Task<byte[]> DownloadResultAsync(long uploadId, string type)
    {
        try
        {
            var upload = await _repository.GetUploadByIdAsync(uploadId);
            if (upload == null)
            {
                throw new InvalidOperationException($"上傳記錄不存在: {uploadId}");
            }

            // 取得該上傳記錄的所有電子發票
            var allInvoices = await _repository.GetEInvoicesByUploadIdAsync(uploadId);
            
            // 根據 type 篩選
            List<EInvoice> filteredInvoices;
            string sheetName;
            
            switch (type?.ToLower())
            {
                case "success":
                    filteredInvoices = allInvoices.Where(x => x.ProcessStatus == "COMPLETED" && string.IsNullOrEmpty(x.ErrorMessage)).ToList();
                    sheetName = "成功清單";
                    break;
                case "failed":
                    filteredInvoices = allInvoices.Where(x => x.ProcessStatus != "COMPLETED" || !string.IsNullOrEmpty(x.ErrorMessage)).ToList();
                    sheetName = "失敗清單";
                    break;
                case "all":
                default:
                    filteredInvoices = allInvoices;
                    sheetName = "全部清單";
                    break;
            }

            // 轉換為 DTO
            var dtos = filteredInvoices.Select(x => MapToDto(x)).ToList();

            // 定義匯出欄位
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "InvoiceId", DisplayName = "發票ID", DataType = ExportDataType.Number },
                new ExportColumn { PropertyName = "OrderNo", DisplayName = "訂單編號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "RetailerOrderNo", DisplayName = "零售商訂單編號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "RetailerOrderDetailNo", DisplayName = "零售商訂單明細編號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "OrderDate", DisplayName = "訂單日期", DataType = ExportDataType.Date },
                new ExportColumn { PropertyName = "StoreId", DisplayName = "店別代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ProviderId", DisplayName = "供應商代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "NdType", DisplayName = "類型", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsId", DisplayName = "商品編號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsName", DisplayName = "商品名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SpecId", DisplayName = "規格ID", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ProviderGoodsId", DisplayName = "供應商商品編號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SpecColor", DisplayName = "規格顏色", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SpecSize", DisplayName = "規格尺寸", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SuggestPrice", DisplayName = "建議售價", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "InternetPrice", DisplayName = "網路售價", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "ShippingType", DisplayName = "運送方式", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ShippingFee", DisplayName = "運費", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "OrderQty", DisplayName = "訂單數量", DataType = ExportDataType.Number },
                new ExportColumn { PropertyName = "OrderShippingFee", DisplayName = "訂單運費", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "OrderSubtotal", DisplayName = "訂單小計", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "OrderTotal", DisplayName = "訂單總計", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "OrderStatus", DisplayName = "訂單狀態", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ProcessStatus", DisplayName = "處理狀態", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ErrorMessage", DisplayName = "錯誤訊息", DataType = ExportDataType.String }
            };

            var title = $"電子發票處理結果 - {sheetName} (上傳記錄: {upload.FileName})";
            return _exportHelper.ExportToExcel(dtos, columns, sheetName, title);
        }
        catch (Exception ex)
        {
            _logger.LogError($"下載處理結果失敗: {uploadId}, Type: {type}", ex);
            throw;
        }
    }
}

