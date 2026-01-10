using System.Text;
using OfficeOpenXml;
using ErpCore.Application.DTOs.SystemExtensionH;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.SystemExtensionE;
using ErpCore.Domain.Entities.SystemExtensionH;
using ErpCore.Infrastructure.Repositories.SystemExtensionE;
using ErpCore.Infrastructure.Repositories.SystemExtensionH;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using Microsoft.AspNetCore.Http;

namespace ErpCore.Application.Services.SystemExtensionH;

/// <summary>
/// 人事批量新增服務實作 (SYSH3D0_FMI - 人事批量新增)
/// </summary>
public class PersonnelBatchService : BaseService, IPersonnelBatchService
{
    private readonly IPersonnelImportLogRepository _logRepository;
    private readonly IPersonnelImportDetailRepository _detailRepository;
    private readonly IPersonnelRepository _personnelRepository;
    private const long MaxFileSize = 10 * 1024 * 1024; // 10MB
    private static readonly string[] AllowedExtensions = { ".xlsx", ".xls" };

    public PersonnelBatchService(
        IPersonnelImportLogRepository logRepository,
        IPersonnelImportDetailRepository detailRepository,
        IPersonnelRepository personnelRepository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _logRepository = logRepository;
        _detailRepository = detailRepository;
        _personnelRepository = personnelRepository;
    }

    /// <summary>
    /// 上傳檔案並解析
    /// </summary>
    public async Task<PersonnelImportLogDto> UploadFileAsync(IFormFile file)
    {
        try
        {
            _logger.LogInfo($"開始上傳人事批量新增檔案: {file.FileName}");

            // 驗證檔案
            ValidateFile(file);

            // 產生匯入批次編號
            var importId = await _logRepository.GenerateImportIdAsync();

            // 讀取Excel檔案
            var details = new List<PersonnelImportDetail>();
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension?.Rows ?? 0;

                    // 跳過標題列，從第2列開始讀取
                    for (int row = 2; row <= rowCount; row++)
                    {
                        var personnelId = worksheet.Cells[row, 1].Text?.Trim();
                        var personnelName = worksheet.Cells[row, 2].Text?.Trim();

                        var detail = new PersonnelImportDetail
                        {
                            ImportId = importId,
                            RowNum = row - 1,
                            PersonnelId = personnelId,
                            PersonnelName = personnelName,
                            ImportStatus = "PENDING",
                            CreatedAt = DateTime.Now
                        };

                        details.Add(detail);
                    }
                }
            }

            // 建立匯入記錄
            var log = new PersonnelImportLog
            {
                ImportId = importId,
                FileName = file.FileName,
                TotalCount = details.Count,
                SuccessCount = 0,
                FailCount = 0,
                ImportStatus = "PENDING",
                ImportDate = DateTime.Now,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now
            };

            await _logRepository.CreateAsync(log);

            // 儲存匯入明細
            await _detailRepository.CreateBatchAsync(details);

            _logger.LogInfo($"上傳人事批量新增檔案成功: {importId}, 總筆數: {details.Count}");

            return MapToDto(log);
        }
        catch (Exception ex)
        {
            _logger.LogError($"上傳人事批量新增檔案失敗: {file.FileName}", ex);
            throw;
        }
    }

    /// <summary>
    /// 執行批量匯入
    /// </summary>
    public async Task<PersonnelImportResultDto> ExecuteImportAsync(string importId)
    {
        try
        {
            _logger.LogInfo($"開始執行人事批量匯入: {importId}");

            // 取得匯入記錄
            var log = await _logRepository.GetByIdAsync(importId);
            if (log == null)
            {
                throw new Exception($"匯入記錄不存在: {importId}");
            }

            if (log.ImportStatus != "PENDING")
            {
                throw new Exception($"匯入記錄狀態不正確: {log.ImportStatus}");
            }

            // 更新狀態為處理中
            await _logRepository.UpdateStatusAsync(importId, "PROCESSING");

            // 取得匯入明細
            var details = await _detailRepository.GetByImportIdAsync(importId);

            int successCount = 0;
            int failCount = 0;

            // 處理每一筆資料
            foreach (var detail in details)
            {
                try
                {
                    // 驗證資料
                    if (string.IsNullOrWhiteSpace(detail.PersonnelId))
                    {
                        detail.ImportStatus = "FAILED";
                        detail.ErrorMessage = "人事編號不能為空";
                        failCount++;
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(detail.PersonnelName))
                    {
                        detail.ImportStatus = "FAILED";
                        detail.ErrorMessage = "人事姓名不能為空";
                        failCount++;
                        continue;
                    }

                    // 實際新增人事資料到資料庫
                    // 檢查人事編號是否已存在
                    var exists = await _personnelRepository.ExistsAsync(detail.PersonnelId);
                    if (exists)
                    {
                        detail.ImportStatus = "FAILED";
                        detail.ErrorMessage = "人事編號已存在";
                        failCount++;
                        await _detailRepository.UpdateAsync(detail);
                        continue;
                    }

                    // 新增人事資料
                    var personnel = new Personnel
                    {
                        PersonnelId = detail.PersonnelId,
                        PersonnelName = detail.PersonnelName,
                        Status = "A", // 預設為在職
                        CreatedBy = GetCurrentUserId(),
                        CreatedAt = DateTime.Now,
                        UpdatedBy = GetCurrentUserId(),
                        UpdatedAt = DateTime.Now
                    };

                    await _personnelRepository.CreateAsync(personnel);

                    // 更新明細狀態
                    detail.ImportStatus = "SUCCESS";
                    await _detailRepository.UpdateAsync(detail);

                    successCount++;
                }
                catch (Exception ex)
                {
                    detail.ImportStatus = "FAILED";
                    detail.ErrorMessage = ex.Message;
                    failCount++;
                    _logger.LogError($"處理人事匯入明細失敗: {detail.RowNum}", ex);
                }
            }

            // 更新匯入記錄
            log.SuccessCount = successCount;
            log.FailCount = failCount;
            log.ImportStatus = failCount == 0 ? "SUCCESS" : "PARTIAL";
            await _logRepository.UpdateAsync(log);

            _logger.LogInfo($"執行人事批量匯入完成: {importId}, 成功: {successCount}, 失敗: {failCount}");

            return new PersonnelImportResultDto
            {
                ImportId = importId,
                TotalCount = log.TotalCount,
                SuccessCount = successCount,
                FailCount = failCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"執行人事批量匯入失敗: {importId}", ex);
            await _logRepository.UpdateStatusAsync(importId, "FAILED");
            throw;
        }
    }

    /// <summary>
    /// 取得匯入進度
    /// </summary>
    public async Task<PersonnelImportProgressDto> GetProgressAsync(string importId)
    {
        try
        {
            var log = await _logRepository.GetByIdAsync(importId);
            if (log == null)
            {
                throw new Exception($"匯入記錄不存在: {importId}");
            }

            var processedCount = log.SuccessCount + log.FailCount;
            var progress = log.TotalCount > 0 
                ? (int)((double)processedCount / log.TotalCount * 100) 
                : 0;

            return new PersonnelImportProgressDto
            {
                ImportId = importId,
                ImportStatus = log.ImportStatus,
                TotalCount = log.TotalCount,
                ProcessedCount = processedCount,
                SuccessCount = log.SuccessCount,
                FailCount = log.FailCount,
                Progress = progress
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢匯入進度失敗: {importId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 取得匯入記錄
    /// </summary>
    public async Task<PersonnelImportLogDto> GetImportLogAsync(string importId)
    {
        try
        {
            var log = await _logRepository.GetByIdAsync(importId);
            if (log == null)
            {
                throw new Exception($"匯入記錄不存在: {importId}");
            }

            return MapToDto(log);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢匯入記錄失敗: {importId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 查詢匯入記錄列表
    /// </summary>
    public async Task<PagedResult<PersonnelImportLogDto>> GetImportLogsAsync(PersonnelImportLogQueryDto query)
    {
        try
        {
            var repositoryQuery = new PersonnelImportLogQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                ImportId = query.ImportId,
                ImportStatus = query.ImportStatus,
                ImportDateFrom = query.ImportDateFrom,
                ImportDateTo = query.ImportDateTo
            };

            var items = await _logRepository.QueryAsync(repositoryQuery);
            var totalCount = await _logRepository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(MapToDto).ToList();

            return new PagedResult<PersonnelImportLogDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢匯入記錄列表失敗", ex);
            throw;
        }
    }

    /// <summary>
    /// 匯出失敗資料
    /// </summary>
    public async Task<byte[]> ExportFailedDataAsync(string importId)
    {
        try
        {
            var details = await _detailRepository.GetByImportIdAsync(importId);
            var failedDetails = details.Where(d => d.ImportStatus == "FAILED").ToList();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("失敗資料");

                // 標題列
                worksheet.Cells[1, 1].Value = "行號";
                worksheet.Cells[1, 2].Value = "人事編號";
                worksheet.Cells[1, 3].Value = "人事姓名";
                worksheet.Cells[1, 4].Value = "錯誤訊息";

                // 資料列
                for (int i = 0; i < failedDetails.Count; i++)
                {
                    var detail = failedDetails[i];
                    worksheet.Cells[i + 2, 1].Value = detail.RowNum;
                    worksheet.Cells[i + 2, 2].Value = detail.PersonnelId;
                    worksheet.Cells[i + 2, 3].Value = detail.PersonnelName;
                    worksheet.Cells[i + 2, 4].Value = detail.ErrorMessage;
                }

                return package.GetAsByteArray();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"匯出失敗資料失敗: {importId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 驗證檔案
    /// </summary>
    private void ValidateFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("檔案不能為空");
        }

        if (file.Length > MaxFileSize)
        {
            throw new ArgumentException($"檔案大小超過限制: {MaxFileSize / 1024 / 1024}MB");
        }

        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(fileExtension))
        {
            throw new ArgumentException($"不支援的檔案格式: {fileExtension}，僅支援 .xlsx, .xls");
        }
    }

    /// <summary>
    /// 映射 Entity 到 DTO
    /// </summary>
    private PersonnelImportLogDto MapToDto(PersonnelImportLog entity)
    {
        return new PersonnelImportLogDto
        {
            TKey = entity.TKey,
            ImportId = entity.ImportId,
            FileName = entity.FileName,
            TotalCount = entity.TotalCount,
            SuccessCount = entity.SuccessCount,
            FailCount = entity.FailCount,
            ImportStatus = entity.ImportStatus,
            ImportDate = entity.ImportDate,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt
        };
    }
}

