using ErpCore.Application.DTOs.SystemExtension;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.SystemExtension;
using ErpCore.Infrastructure.Repositories.SystemExtension;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.SystemExtension;

/// <summary>
/// 系統擴展服務實作 (SYSX110, SYSX120, SYSX140)
/// </summary>
public class SystemExtensionService : BaseService, ISystemExtensionService
{
    private readonly ISystemExtensionRepository _repository;

    public SystemExtensionService(
        ISystemExtensionRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<SystemExtensionDto>> GetSystemExtensionsAsync(SystemExtensionQueryDto query)
    {
        try
        {
            var repositoryQuery = new SystemExtensionQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                ExtensionId = query.ExtensionId,
                ExtensionName = query.ExtensionName,
                ExtensionType = query.ExtensionType,
                Status = query.Status,
                CreatedDateFrom = query.CreatedDateFrom,
                CreatedDateTo = query.CreatedDateTo
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<SystemExtensionDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢系統擴展列表失敗", ex);
            throw;
        }
    }

    public async Task<SystemExtensionDto> GetSystemExtensionByTKeyAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByTKeyAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"系統擴展不存在: {tKey}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢系統擴展失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<SystemExtensionDto> GetSystemExtensionByExtensionIdAsync(string extensionId)
    {
        try
        {
            var entity = await _repository.GetByExtensionIdAsync(extensionId);
            if (entity == null)
            {
                throw new InvalidOperationException($"系統擴展不存在: {extensionId}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢系統擴展失敗: {extensionId}", ex);
            throw;
        }
    }

    public async Task<long> CreateSystemExtensionAsync(CreateSystemExtensionDto dto)
    {
        try
        {
            // 驗證必填欄位
            if (string.IsNullOrWhiteSpace(dto.ExtensionId))
            {
                throw new ArgumentException("擴展功能代碼不能為空");
            }

            if (string.IsNullOrWhiteSpace(dto.ExtensionName))
            {
                throw new ArgumentException("擴展功能名稱不能為空");
            }

            // 檢查擴展功能代碼是否已存在
            var exists = await _repository.ExistsAsync(dto.ExtensionId);
            if (exists)
            {
                throw new InvalidOperationException($"擴展功能代碼已存在: {dto.ExtensionId}");
            }

            // 驗證 JSON 格式（如果提供）
            if (!string.IsNullOrWhiteSpace(dto.ExtensionConfig))
            {
                try
                {
                    System.Text.Json.JsonDocument.Parse(dto.ExtensionConfig);
                }
                catch
                {
                    throw new ArgumentException("擴展設定必須是有效的 JSON 格式");
                }
            }

            var entity = new SystemExtension
            {
                ExtensionId = dto.ExtensionId,
                ExtensionName = dto.ExtensionName,
                ExtensionType = dto.ExtensionType,
                ExtensionValue = dto.ExtensionValue,
                ExtensionConfig = dto.ExtensionConfig,
                SeqNo = dto.SeqNo ?? 0,
                Status = dto.Status ?? "1",
                Notes = dto.Notes,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var result = await _repository.CreateAsync(entity);
            return result.TKey;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增系統擴展失敗: {dto.ExtensionId}", ex);
            throw;
        }
    }

    public async Task UpdateSystemExtensionAsync(long tKey, UpdateSystemExtensionDto dto)
    {
        try
        {
            // 驗證必填欄位
            if (string.IsNullOrWhiteSpace(dto.ExtensionName))
            {
                throw new ArgumentException("擴展功能名稱不能為空");
            }

            // 檢查系統擴展是否存在
            var entity = await _repository.GetByTKeyAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"系統擴展不存在: {tKey}");
            }

            // 驗證 JSON 格式（如果提供）
            if (!string.IsNullOrWhiteSpace(dto.ExtensionConfig))
            {
                try
                {
                    System.Text.Json.JsonDocument.Parse(dto.ExtensionConfig);
                }
                catch
                {
                    throw new ArgumentException("擴展設定必須是有效的 JSON 格式");
                }
            }

            entity.ExtensionName = dto.ExtensionName;
            entity.ExtensionType = dto.ExtensionType;
            entity.ExtensionValue = dto.ExtensionValue;
            entity.ExtensionConfig = dto.ExtensionConfig;
            entity.SeqNo = dto.SeqNo ?? entity.SeqNo ?? 0;
            entity.Status = dto.Status ?? entity.Status;
            entity.Notes = dto.Notes;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改系統擴展失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteSystemExtensionAsync(long tKey)
    {
        try
        {
            // 檢查系統擴展是否存在
            var entity = await _repository.GetByTKeyAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"系統擴展不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除系統擴展失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<SystemExtensionStatisticsDto> GetStatisticsAsync(SystemExtensionStatisticsQueryDto query)
    {
        try
        {
            var repositoryQuery = new SystemExtensionStatisticsQuery
            {
                ExtensionType = query.ExtensionType,
                Status = query.Status
            };

            var result = await _repository.GetStatisticsAsync(repositoryQuery);

            return new SystemExtensionStatisticsDto
            {
                TotalCount = result.TotalCount,
                ActiveCount = result.ActiveCount,
                InactiveCount = result.InactiveCount,
                ByType = result.ByType.Select(x => new SystemExtensionTypeCountDto
                {
                    ExtensionType = x.ExtensionType,
                    Count = x.Count
                }).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢系統擴展統計失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportToExcelAsync(SystemExtensionQueryDto query)
    {
        try
        {
            // 查詢所有符合條件的資料（不分頁）
            var repositoryQuery = new SystemExtensionQuery
            {
                PageIndex = 1,
                PageSize = int.MaxValue, // 取得所有資料
                SortField = query.SortField ?? "SeqNo",
                SortOrder = query.SortOrder ?? "ASC",
                ExtensionId = query.ExtensionId,
                ExtensionName = query.ExtensionName,
                ExtensionType = query.ExtensionType,
                Status = query.Status,
                CreatedDateFrom = query.CreatedDateFrom,
                CreatedDateTo = query.CreatedDateTo
            };

            var result = await _repository.QueryAsync(repositoryQuery);
            var dtos = result.Items.Select(x => MapToDto(x)).ToList();

            // 定義匯出欄位
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "ExtensionId", DisplayName = "擴展功能代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ExtensionName", DisplayName = "擴展功能名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ExtensionType", DisplayName = "擴展類型", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ExtensionValue", DisplayName = "擴展值", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SeqNo", DisplayName = "排序序號", DataType = ExportDataType.Number },
                new ExportColumn { PropertyName = "Status", DisplayName = "狀態", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Notes", DisplayName = "備註", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "CreatedBy", DisplayName = "建立人員", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "CreatedAt", DisplayName = "建立時間", DataType = ExportDataType.DateTime },
                new ExportColumn { PropertyName = "UpdatedBy", DisplayName = "更新人員", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "UpdatedAt", DisplayName = "更新時間", DataType = ExportDataType.DateTime }
            };

            // 使用 ExportHelper 匯出
            var exportHelper = new ExportHelper(_logger);
            var excelBytes = exportHelper.ExportToExcel(
                dtos,
                columns,
                "系統擴展查詢",
                "系統擴展查詢報表");

            return excelBytes;
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出系統擴展資料失敗", ex);
            throw;
        }
    }

    /// <summary>
    /// 將 Entity 轉換為 DTO
    /// </summary>
    private SystemExtensionDto MapToDto(ErpCore.Domain.Entities.SystemExtension.SystemExtension entity)
    {
        return new SystemExtensionDto
        {
            TKey = entity.TKey,
            ExtensionId = entity.ExtensionId,
            ExtensionName = entity.ExtensionName,
            ExtensionType = entity.ExtensionType,
            ExtensionValue = entity.ExtensionValue,
            ExtensionConfig = entity.ExtensionConfig,
            SeqNo = entity.SeqNo,
            Status = entity.Status,
            Notes = entity.Notes,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt,
            CreatedPriority = entity.CreatedPriority,
            CreatedGroup = entity.CreatedGroup
        };
    }
}

