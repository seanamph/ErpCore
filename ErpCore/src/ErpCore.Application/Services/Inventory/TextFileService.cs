using System.Text;
using System.Text.Json;
using OfficeOpenXml;
using ErpCore.Application.DTOs.Inventory;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Inventory;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.Inventory;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using Microsoft.AspNetCore.Http;

namespace ErpCore.Application.Services.Inventory;

/// <summary>
/// 文本文件處理服務實作
/// </summary>
public class TextFileService : BaseService, ITextFileService
{
    private readonly ITextFileProcessLogRepository _logRepository;
    private readonly ITextFileProcessDetailRepository _detailRepository;
    private readonly IDbConnectionFactory _connectionFactory;

    public TextFileService(
        ITextFileProcessLogRepository logRepository,
        ITextFileProcessDetailRepository detailRepository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _logRepository = logRepository;
        _detailRepository = detailRepository;
        _connectionFactory = connectionFactory;
    }

    public async Task<TextFileProcessLogDto> UploadAndProcessFileAsync(IFormFile file, string fileType, string? shopId)
    {
        try
        {
            // 建立處理記錄
            var log = new TextFileProcessLog
            {
                LogId = Guid.NewGuid(),
                FileName = file.FileName,
                FileType = fileType,
                ShopId = shopId,
                ProcessStatus = "PROCESSING",
                ProcessStartTime = DateTime.Now,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _logRepository.CreateAsync(log);

            // 讀取文件內容
            string content;
            using (var stream = new StreamReader(file.OpenReadStream(), Encoding.Default))
            {
                content = await stream.ReadToEndAsync();
            }

            // 解析文件
            var details = new List<TextFileProcessDetail>();
            int successCount = 0;
            int failedCount = 0;

            try
            {
                switch (fileType.ToUpper())
                {
                    case "BACK":
                        var backRecords = HT680TextFileParser.ParseBackFile(content, shopId);
                        foreach (var record in backRecords)
                        {
                            var detail = new TextFileProcessDetail
                            {
                                DetailId = Guid.NewGuid(),
                                LogId = log.LogId,
                                LineNumber = record.LineNumber,
                                RawData = JsonSerializer.Serialize(record),
                                ProcessStatus = "SUCCESS",
                                ProcessedData = JsonSerializer.Serialize(record),
                                CreatedAt = DateTime.Now
                            };
                            details.Add(detail);
                            successCount++;
                        }
                        break;

                    case "INV":
                        var invRecords = HT680TextFileParser.ParseInvFile(content, shopId);
                        foreach (var record in invRecords)
                        {
                            var detail = new TextFileProcessDetail
                            {
                                DetailId = Guid.NewGuid(),
                                LogId = log.LogId,
                                LineNumber = record.LineNumber,
                                RawData = JsonSerializer.Serialize(record),
                                ProcessStatus = "SUCCESS",
                                ProcessedData = JsonSerializer.Serialize(record),
                                CreatedAt = DateTime.Now
                            };
                            details.Add(detail);
                            successCount++;
                        }
                        break;

                    case "ORDER":
                        var orderRecords = HT680TextFileParser.ParseOrderFile(content, shopId);
                        foreach (var record in orderRecords)
                        {
                            var detail = new TextFileProcessDetail
                            {
                                DetailId = Guid.NewGuid(),
                                LogId = log.LogId,
                                LineNumber = record.LineNumber,
                                RawData = JsonSerializer.Serialize(record),
                                ProcessStatus = "SUCCESS",
                                ProcessedData = JsonSerializer.Serialize(record),
                                CreatedAt = DateTime.Now
                            };
                            details.Add(detail);
                            successCount++;
                        }
                        break;

                    case "ORDER_6":
                        var order6Records = HT680TextFileParser.ParseOrder6File(content, shopId);
                        foreach (var record in order6Records)
                        {
                            var detail = new TextFileProcessDetail
                            {
                                DetailId = Guid.NewGuid(),
                                LogId = log.LogId,
                                LineNumber = record.LineNumber,
                                RawData = JsonSerializer.Serialize(record),
                                ProcessStatus = "SUCCESS",
                                ProcessedData = JsonSerializer.Serialize(record),
                                CreatedAt = DateTime.Now
                            };
                            details.Add(detail);
                            successCount++;
                        }
                        break;

                    case "POP":
                        var popRecords = HT680TextFileParser.ParsePopFile(content, shopId);
                        foreach (var record in popRecords)
                        {
                            var detail = new TextFileProcessDetail
                            {
                                DetailId = Guid.NewGuid(),
                                LogId = log.LogId,
                                LineNumber = record.LineNumber,
                                RawData = JsonSerializer.Serialize(record),
                                ProcessStatus = "SUCCESS",
                                ProcessedData = JsonSerializer.Serialize(record),
                                CreatedAt = DateTime.Now
                            };
                            details.Add(detail);
                            successCount++;
                        }
                        break;

                    case "PRIC":
                        var pricRecords = HT680TextFileParser.ParsePricFile(content, shopId);
                        foreach (var record in pricRecords)
                        {
                            var detail = new TextFileProcessDetail
                            {
                                DetailId = Guid.NewGuid(),
                                LogId = log.LogId,
                                LineNumber = record.LineNumber,
                                RawData = JsonSerializer.Serialize(record),
                                ProcessStatus = "SUCCESS",
                                ProcessedData = JsonSerializer.Serialize(record),
                                CreatedAt = DateTime.Now
                            };
                            details.Add(detail);
                            successCount++;
                        }
                        break;

                    default:
                        throw new NotSupportedException($"不支援的文件類型: {fileType}");
                }

                // 批次建立明細
                if (details.Any())
                {
                    await _detailRepository.CreateBatchAsync(details);
                }

                // 更新處理記錄
                log.TotalRecords = details.Count;
                log.SuccessRecords = successCount;
                log.FailedRecords = failedCount;
                log.ProcessStatus = "COMPLETED";
                log.ProcessEndTime = DateTime.Now;
                log.UpdatedBy = GetCurrentUserId();
                log.UpdatedAt = DateTime.Now;

                await _logRepository.UpdateAsync(log);
            }
            catch (Exception ex)
            {
                // 處理失敗
                log.ProcessStatus = "FAILED";
                log.ErrorMessage = ex.Message;
                log.ProcessEndTime = DateTime.Now;
                log.UpdatedBy = GetCurrentUserId();
                log.UpdatedAt = DateTime.Now;
                await _logRepository.UpdateAsync(log);

                _logger.LogError($"處理文件失敗: {file.FileName}", ex);
                throw;
            }

            return new TextFileProcessLogDto
            {
                LogId = log.LogId,
                FileName = log.FileName,
                FileType = log.FileType,
                ShopId = log.ShopId,
                TotalRecords = log.TotalRecords,
                SuccessRecords = log.SuccessRecords,
                FailedRecords = log.FailedRecords,
                ProcessStatus = log.ProcessStatus,
                ProcessStartTime = log.ProcessStartTime,
                ProcessEndTime = log.ProcessEndTime,
                ErrorMessage = log.ErrorMessage,
                CreatedAt = log.CreatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"上傳並處理文件失敗: {file.FileName}", ex);
            throw;
        }
    }

    public async Task<PagedResult<TextFileProcessLogDto>> GetProcessLogsAsync(TextFileProcessLogQueryDto query)
    {
        try
        {
            var repositoryQuery = new TextFileProcessLogQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                FileName = query.FileName,
                FileType = query.FileType,
                ShopId = query.ShopId,
                ProcessStatus = query.ProcessStatus
            };

            var result = await _logRepository.GetPagedAsync(repositoryQuery);

            var dtos = result.Items.Select(x => new TextFileProcessLogDto
            {
                LogId = x.LogId,
                FileName = x.FileName,
                FileType = x.FileType,
                ShopId = x.ShopId,
                TotalRecords = x.TotalRecords,
                SuccessRecords = x.SuccessRecords,
                FailedRecords = x.FailedRecords,
                ProcessStatus = x.ProcessStatus,
                ProcessStartTime = x.ProcessStartTime,
                ProcessEndTime = x.ProcessEndTime,
                ErrorMessage = x.ErrorMessage,
                CreatedAt = x.CreatedAt
            }).ToList();

            return new PagedResult<TextFileProcessLogDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢處理記錄列表失敗", ex);
            throw;
        }
    }

    public async Task<TextFileProcessLogDto?> GetProcessLogByIdAsync(Guid logId)
    {
        try
        {
            var log = await _logRepository.GetByIdAsync(logId);
            if (log == null)
                return null;

            return new TextFileProcessLogDto
            {
                LogId = log.LogId,
                FileName = log.FileName,
                FileType = log.FileType,
                ShopId = log.ShopId,
                TotalRecords = log.TotalRecords,
                SuccessRecords = log.SuccessRecords,
                FailedRecords = log.FailedRecords,
                ProcessStatus = log.ProcessStatus,
                ProcessStartTime = log.ProcessStartTime,
                ProcessEndTime = log.ProcessEndTime,
                ErrorMessage = log.ErrorMessage,
                CreatedAt = log.CreatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢處理記錄失敗: LogId={logId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<TextFileProcessDetailDto>> GetProcessDetailsAsync(Guid logId, TextFileProcessDetailQueryDto query)
    {
        try
        {
            var repositoryQuery = new TextFileProcessDetailQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                ProcessStatus = query.ProcessStatus
            };

            var result = await _detailRepository.GetPagedByLogIdAsync(logId, repositoryQuery);

            var dtos = result.Items.Select(x => new TextFileProcessDetailDto
            {
                DetailId = x.DetailId,
                LogId = x.LogId,
                LineNumber = x.LineNumber,
                RawData = x.RawData,
                ProcessStatus = x.ProcessStatus,
                ErrorMessage = x.ErrorMessage,
                ProcessedData = !string.IsNullOrEmpty(x.ProcessedData) 
                    ? JsonSerializer.Deserialize<object>(x.ProcessedData) 
                    : null,
                CreatedAt = x.CreatedAt
            }).ToList();

            return new PagedResult<TextFileProcessDetailDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢處理明細列表失敗: LogId={logId}", ex);
            throw;
        }
    }

    public async Task ReprocessFileAsync(Guid logId)
    {
        try
        {
            var log = await _logRepository.GetByIdAsync(logId);
            if (log == null)
                throw new InvalidOperationException($"處理記錄不存在: {logId}");

            // 更新處理狀態
            log.ProcessStatus = "PROCESSING";
            log.ProcessStartTime = DateTime.Now;
            log.ErrorMessage = null;
            log.UpdatedBy = GetCurrentUserId();
            log.UpdatedAt = DateTime.Now;

            await _logRepository.UpdateAsync(log);

            // 從資料庫讀取原始明細資料
            var detailQuery = new TextFileProcessDetailQuery
            {
                PageIndex = 1,
                PageSize = int.MaxValue
            };
            var existingDetails = await _detailRepository.GetPagedByLogIdAsync(logId, detailQuery);

            if (!existingDetails.Items.Any())
            {
                throw new InvalidOperationException($"處理記錄沒有明細資料: {logId}");
            }

            // 重建原始文件內容（從 RawData 還原）
            var contentBuilder = new StringBuilder();
            foreach (var detail in existingDetails.Items.OrderBy(d => d.LineNumber))
            {
                if (!string.IsNullOrEmpty(detail.RawData))
                {
                    try
                    {
                        // 根據文件類型從 JSON 還原原始行內容
                        string? lineContent = null;
                        
                        switch (log.FileType.ToUpper())
                        {
                            case "BACK":
                                var backRecord = JsonSerializer.Deserialize<HT680BackRecord>(detail.RawData);
                                if (backRecord != null)
                                {
                                    lineContent = $"{backRecord.ShopId.PadRight(4)}{backRecord.GoodsId.PadRight(8)}{backRecord.Qty.ToString("0").PadLeft(8)}";
                                }
                                break;
                            
                            case "INV":
                                var invRecord = JsonSerializer.Deserialize<HT680InvRecord>(detail.RawData);
                                if (invRecord != null)
                                {
                                    lineContent = $"{invRecord.ShelfNo.PadRight(2)}{invRecord.SerialNo.ToString().PadLeft(4)}{invRecord.GoodsId.PadRight(8)}{invRecord.Qty.ToString("0").PadLeft(8)}";
                                }
                                break;
                            
                            case "ORDER":
                                var orderRecord = JsonSerializer.Deserialize<HT680OrderRecord>(detail.RawData);
                                if (orderRecord != null)
                                {
                                    lineContent = $"{orderRecord.ShopId.PadRight(4)}{orderRecord.GoodsId.PadRight(8)}{orderRecord.Qty.ToString("0").PadLeft(8)}";
                                }
                                break;
                            
                            case "ORDER_6":
                                var order6Record = JsonSerializer.Deserialize<HT680Order6Record>(detail.RawData);
                                if (order6Record != null)
                                {
                                    lineContent = $"{order6Record.ShopId.PadRight(6)}{order6Record.GoodsId.PadRight(8)}{order6Record.Qty.ToString("0").PadLeft(8)}";
                                }
                                break;
                            
                            case "POP":
                                var popRecord = JsonSerializer.Deserialize<HT680PopRecord>(detail.RawData);
                                if (popRecord != null)
                                {
                                    lineContent = $"{popRecord.ShopId.PadRight(4)}{popRecord.GoodsId.PadRight(8)}";
                                }
                                break;
                            
                            case "PRIC":
                                var pricRecord = JsonSerializer.Deserialize<HT680PricRecord>(detail.RawData);
                                if (pricRecord != null)
                                {
                                    lineContent = pricRecord.GoodsId.PadRight(8);
                                }
                                break;
                        }
                        
                        if (!string.IsNullOrEmpty(lineContent))
                        {
                            contentBuilder.AppendLine(lineContent);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"還原原始行內容失敗: LogId={logId}, LineNumber={detail.LineNumber}, Error={ex.Message}");
                        // 如果解析失敗，跳過該行
                    }
                }
            }

            var content = contentBuilder.ToString();
            
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new InvalidOperationException($"無法從明細資料還原原始文件內容: {logId}");
            }

            // 刪除舊的明細資料
            await _detailRepository.DeleteByLogIdAsync(logId);

            // 重新解析文件
            var details = new List<TextFileProcessDetail>();
            int successCount = 0;
            int failedCount = 0;

            try
            {
                switch (log.FileType.ToUpper())
                {
                    case "BACK":
                        var backRecords = HT680TextFileParser.ParseBackFile(content, log.ShopId);
                        foreach (var record in backRecords)
                        {
                            var detail = new TextFileProcessDetail
                            {
                                DetailId = Guid.NewGuid(),
                                LogId = log.LogId,
                                LineNumber = record.LineNumber,
                                RawData = JsonSerializer.Serialize(record),
                                ProcessStatus = "SUCCESS",
                                ProcessedData = JsonSerializer.Serialize(record),
                                CreatedAt = DateTime.Now
                            };
                            details.Add(detail);
                            successCount++;
                        }
                        break;

                    case "INV":
                        var invRecords = HT680TextFileParser.ParseInvFile(content, log.ShopId);
                        foreach (var record in invRecords)
                        {
                            var detail = new TextFileProcessDetail
                            {
                                DetailId = Guid.NewGuid(),
                                LogId = log.LogId,
                                LineNumber = record.LineNumber,
                                RawData = JsonSerializer.Serialize(record),
                                ProcessStatus = "SUCCESS",
                                ProcessedData = JsonSerializer.Serialize(record),
                                CreatedAt = DateTime.Now
                            };
                            details.Add(detail);
                            successCount++;
                        }
                        break;

                    case "ORDER":
                        var orderRecords = HT680TextFileParser.ParseOrderFile(content, log.ShopId);
                        foreach (var record in orderRecords)
                        {
                            var detail = new TextFileProcessDetail
                            {
                                DetailId = Guid.NewGuid(),
                                LogId = log.LogId,
                                LineNumber = record.LineNumber,
                                RawData = JsonSerializer.Serialize(record),
                                ProcessStatus = "SUCCESS",
                                ProcessedData = JsonSerializer.Serialize(record),
                                CreatedAt = DateTime.Now
                            };
                            details.Add(detail);
                            successCount++;
                        }
                        break;

                    case "ORDER_6":
                        var order6Records = HT680TextFileParser.ParseOrder6File(content, log.ShopId);
                        foreach (var record in order6Records)
                        {
                            var detail = new TextFileProcessDetail
                            {
                                DetailId = Guid.NewGuid(),
                                LogId = log.LogId,
                                LineNumber = record.LineNumber,
                                RawData = JsonSerializer.Serialize(record),
                                ProcessStatus = "SUCCESS",
                                ProcessedData = JsonSerializer.Serialize(record),
                                CreatedAt = DateTime.Now
                            };
                            details.Add(detail);
                            successCount++;
                        }
                        break;

                    case "POP":
                        var popRecords = HT680TextFileParser.ParsePopFile(content, log.ShopId);
                        foreach (var record in popRecords)
                        {
                            var detail = new TextFileProcessDetail
                            {
                                DetailId = Guid.NewGuid(),
                                LogId = log.LogId,
                                LineNumber = record.LineNumber,
                                RawData = JsonSerializer.Serialize(record),
                                ProcessStatus = "SUCCESS",
                                ProcessedData = JsonSerializer.Serialize(record),
                                CreatedAt = DateTime.Now
                            };
                            details.Add(detail);
                            successCount++;
                        }
                        break;

                    case "PRIC":
                        var pricRecords = HT680TextFileParser.ParsePricFile(content, log.ShopId);
                        foreach (var record in pricRecords)
                        {
                            var detail = new TextFileProcessDetail
                            {
                                DetailId = Guid.NewGuid(),
                                LogId = log.LogId,
                                LineNumber = record.LineNumber,
                                RawData = JsonSerializer.Serialize(record),
                                ProcessStatus = "SUCCESS",
                                ProcessedData = JsonSerializer.Serialize(record),
                                CreatedAt = DateTime.Now
                            };
                            details.Add(detail);
                            successCount++;
                        }
                        break;

                    default:
                        throw new NotSupportedException($"不支援的文件類型: {log.FileType}");
                }

                // 批次建立明細
                if (details.Any())
                {
                    await _detailRepository.CreateBatchAsync(details);
                }

                // 更新處理記錄
                log.TotalRecords = details.Count;
                log.SuccessRecords = successCount;
                log.FailedRecords = failedCount;
                log.ProcessStatus = "COMPLETED";
                log.ProcessEndTime = DateTime.Now;
                log.UpdatedBy = GetCurrentUserId();
                log.UpdatedAt = DateTime.Now;

                await _logRepository.UpdateAsync(log);

                _logger.LogInfo($"重新處理文件成功: LogId={logId}, TotalRecords={log.TotalRecords}, SuccessRecords={successCount}, FailedRecords={failedCount}");
            }
            catch (Exception ex)
            {
                // 處理失敗
                log.ProcessStatus = "FAILED";
                log.ErrorMessage = ex.Message;
                log.ProcessEndTime = DateTime.Now;
                log.UpdatedBy = GetCurrentUserId();
                log.UpdatedAt = DateTime.Now;
                await _logRepository.UpdateAsync(log);

                _logger.LogError($"重新處理文件失敗: LogId={logId}", ex);
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"重新處理文件失敗: LogId={logId}", ex);
            throw;
        }
    }

    public async Task DeleteProcessLogAsync(Guid logId)
    {
        try
        {
            await _logRepository.DeleteAsync(logId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除處理記錄失敗: LogId={logId}", ex);
            throw;
        }
    }

    public async Task<byte[]> DownloadProcessResultAsync(Guid logId, string format)
    {
        try
        {
            var log = await _logRepository.GetByIdAsync(logId);
            if (log == null)
                throw new InvalidOperationException($"處理記錄不存在: {logId}");

            var query = new TextFileProcessDetailQuery
            {
                PageIndex = 1,
                PageSize = int.MaxValue
            };

            var details = await _detailRepository.GetPagedByLogIdAsync(logId, query);

            if (format.ToLower() == "excel")
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("處理結果");

                // 標題行
                worksheet.Cells[1, 1].Value = "行號";
                worksheet.Cells[1, 2].Value = "原始資料";
                worksheet.Cells[1, 3].Value = "處理狀態";
                worksheet.Cells[1, 4].Value = "錯誤訊息";
                worksheet.Cells[1, 5].Value = "處理後資料";

                // 資料行
                int row = 2;
                foreach (var detail in details.Items)
                {
                    worksheet.Cells[row, 1].Value = detail.LineNumber;
                    worksheet.Cells[row, 2].Value = detail.RawData;
                    worksheet.Cells[row, 3].Value = detail.ProcessStatus;
                    worksheet.Cells[row, 4].Value = detail.ErrorMessage;
                    worksheet.Cells[row, 5].Value = detail.ProcessedData;
                    row++;
                }

                return await package.GetAsByteArrayAsync();
            }
            else // CSV
            {
                var sb = new StringBuilder();
                sb.AppendLine("行號,原始資料,處理狀態,錯誤訊息,處理後資料");

                foreach (var detail in details.Items)
                {
                    sb.AppendLine($"{detail.LineNumber},\"{detail.RawData}\",{detail.ProcessStatus},\"{detail.ErrorMessage}\",\"{detail.ProcessedData}\"");
                }

                return Encoding.UTF8.GetBytes(sb.ToString());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"下載處理結果失敗: LogId={logId}, Format={format}", ex);
            throw;
        }
    }
}

