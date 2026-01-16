using System.Text.Json;
using ErpCore.Application.DTOs.Procurement;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Procurement;
using ErpCore.Infrastructure.Repositories.Procurement;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Procurement;

/// <summary>
/// 採購報表列印服務實作
/// </summary>
public class PurchaseReportPrintService : BaseService, IPurchaseReportPrintService
{
    private readonly IPurchaseReportPrintRepository _repository;

    public PurchaseReportPrintService(
        IPurchaseReportPrintRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<PurchaseReportPrintDto>> GetPurchaseReportPrintsAsync(PurchaseReportPrintQueryDto query)
    {
        try
        {
            var repositoryQuery = new PurchaseReportPrintQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                ReportType = query.ReportType,
                ReportCode = query.ReportCode,
                PrintUserId = query.PrintUserId,
                Status = query.Status,
                StartDate = query.StartDate,
                EndDate = query.EndDate
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<PurchaseReportPrintDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢採購報表列印記錄列表失敗", ex);
            throw;
        }
    }

    public async Task<PurchaseReportPrintDto> GetPurchaseReportPrintByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"採購報表列印記錄不存在: {tKey}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢採購報表列印記錄失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<PurchaseReportPrintResultDto> CreatePurchaseReportPrintAsync(CreatePurchaseReportPrintDto dto)
    {
        try
        {
            var currentUser = _userContext.GetCurrentUser();
            var now = DateTime.Now;

            // 序列化篩選條件和列印設定
            var filterConditionsJson = dto.FilterConditions != null
                ? JsonSerializer.Serialize(dto.FilterConditions)
                : null;

            var printSettingsJson = dto.PrintSettings != null
                ? JsonSerializer.Serialize(dto.PrintSettings)
                : null;

            var entity = new PurchaseReportPrint
            {
                ReportType = dto.ReportType,
                ReportCode = dto.ReportCode,
                ReportName = dto.ReportName,
                PrintDate = now,
                PrintUserId = currentUser?.UserId ?? string.Empty,
                PrintUserName = currentUser?.UserName,
                FilterConditions = filterConditionsJson,
                PrintSettings = printSettingsJson,
                FileFormat = dto.FileFormat,
                Status = "P", // 處理中
                CreatedBy = currentUser?.UserId,
                CreatedAt = now,
                UpdatedBy = currentUser?.UserId,
                UpdatedAt = now
            };

            entity = await _repository.CreateAsync(entity);

            // TODO: 實際生成報表檔案（需要整合報表生成服務）
            // 這裡先返回基本資訊，實際報表生成可以異步處理

            return new PurchaseReportPrintResultDto
            {
                TKey = entity.TKey,
                FilePath = entity.FilePath,
                FileName = entity.FileName
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("新增採購報表列印記錄失敗", ex);
            throw;
        }
    }

    public async Task UpdatePurchaseReportPrintAsync(long tKey, UpdatePurchaseReportPrintDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"採購報表列印記錄不存在: {tKey}");
            }

            var currentUser = _userContext.GetCurrentUser();
            var now = DateTime.Now;

            if (!string.IsNullOrEmpty(dto.Status))
            {
                entity.Status = dto.Status;
            }

            if (dto.ErrorMessage != null)
            {
                entity.ErrorMessage = dto.ErrorMessage;
            }

            if (dto.FilePath != null)
            {
                entity.FilePath = dto.FilePath;
            }

            if (dto.FileName != null)
            {
                entity.FileName = dto.FileName;
            }

            if (dto.FileSize.HasValue)
            {
                entity.FileSize = dto.FileSize.Value;
            }

            if (dto.PageCount.HasValue)
            {
                entity.PageCount = dto.PageCount.Value;
            }

            if (dto.RecordCount.HasValue)
            {
                entity.RecordCount = dto.RecordCount.Value;
            }

            if (dto.Notes != null)
            {
                entity.Notes = dto.Notes;
            }

            entity.UpdatedBy = currentUser?.UserId;
            entity.UpdatedAt = now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改採購報表列印記錄失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeletePurchaseReportPrintAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"採購報表列印記錄不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除採購報表列印記錄失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<byte[]> DownloadPurchaseReportPrintAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"採購報表列印記錄不存在: {tKey}");
            }

            if (string.IsNullOrEmpty(entity.FilePath))
            {
                throw new InvalidOperationException($"報表檔案不存在: {tKey}");
            }

            // TODO: 實際讀取檔案並返回 byte[]
            // 這裡需要根據實際的檔案儲存位置來讀取檔案
            throw new NotImplementedException("檔案下載功能待實作");
        }
        catch (Exception ex)
        {
            _logger.LogError($"下載採購報表列印檔案失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<ReportPreviewDto> PreviewPurchaseReportPrintAsync(CreatePurchaseReportPrintDto dto)
    {
        try
        {
            // TODO: 實際生成報表預覽
            // 這裡需要整合報表生成服務來生成預覽資料
            throw new NotImplementedException("報表預覽功能待實作");
        }
        catch (Exception ex)
        {
            _logger.LogError("預覽採購報表失敗", ex);
            throw;
        }
    }

    public async Task<List<PurchaseReportTemplateDto>> GetPurchaseReportTemplatesAsync(string? reportType, string? reportCode)
    {
        try
        {
            var templates = await _repository.GetTemplatesAsync(reportType, reportCode);
            return templates.Select(x => new PurchaseReportTemplateDto
            {
                TemplateId = x.TemplateId,
                ReportType = x.ReportType,
                ReportCode = x.ReportCode,
                TemplateName = x.TemplateName,
                TemplatePath = x.TemplatePath,
                TemplateType = x.TemplateType,
                IsDefault = x.IsDefault,
                Status = x.Status
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢採購報表模板列表失敗", ex);
            throw;
        }
    }

    private PurchaseReportPrintDto MapToDto(PurchaseReportPrint entity)
    {
        return new PurchaseReportPrintDto
        {
            TKey = entity.TKey,
            ReportType = entity.ReportType,
            ReportTypeName = GetReportTypeName(entity.ReportType),
            ReportCode = entity.ReportCode,
            ReportName = entity.ReportName,
            PrintDate = entity.PrintDate,
            PrintUserId = entity.PrintUserId,
            PrintUserName = entity.PrintUserName,
            FilterConditions = entity.FilterConditions,
            PrintSettings = entity.PrintSettings,
            FileFormat = entity.FileFormat,
            FilePath = entity.FilePath,
            FileName = entity.FileName,
            FileSize = entity.FileSize,
            Status = entity.Status,
            StatusName = GetStatusName(entity.Status),
            ErrorMessage = entity.ErrorMessage,
            PageCount = entity.PageCount,
            RecordCount = entity.RecordCount,
            Notes = entity.Notes,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }

    private string GetReportTypeName(string reportType)
    {
        return reportType switch
        {
            "PO" => "採購單",
            "SU" => "供應商",
            "PY" => "付款單",
            _ => reportType
        };
    }

    private string GetStatusName(string status)
    {
        return status switch
        {
            "S" => "成功",
            "F" => "失敗",
            "P" => "處理中",
            _ => status
        };
    }
}
