using ErpCore.Application.DTOs.BusinessReport;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.BusinessReport;
using ErpCore.Infrastructure.Repositories.BusinessReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.BusinessReport;

/// <summary>
/// 業務報表列印服務實作 (SYSL150)
/// </summary>
public class BusinessReportPrintService : BaseService, IBusinessReportPrintService
{
    private readonly IBusinessReportPrintRepository _repository;

    public BusinessReportPrintService(
        IBusinessReportPrintRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<BusinessReportPrintDto>> GetBusinessReportPrintsAsync(BusinessReportPrintQueryDto query)
    {
        try
        {
            var repositoryQuery = new BusinessReportPrintQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                GiveYear = query.GiveYear,
                SiteId = query.SiteId,
                OrgId = query.OrgId,
                EmpId = query.EmpId,
                Status = query.Status
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<BusinessReportPrintDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢業務報表列印列表失敗", ex);
            throw;
        }
    }

    public async Task<BusinessReportPrintDto> GetBusinessReportPrintByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"業務報表列印不存在: {tKey}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢業務報表列印失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<long> CreateBusinessReportPrintAsync(CreateBusinessReportPrintDto dto)
    {
        try
        {
            // 驗證必填欄位
            if (dto.GiveYear <= 0)
            {
                throw new ArgumentException("發放年度不能為空或小於等於0");
            }

            if (string.IsNullOrWhiteSpace(dto.SiteId))
            {
                throw new ArgumentException("店別代碼不能為空");
            }

            if (string.IsNullOrWhiteSpace(dto.EmpId))
            {
                throw new ArgumentException("員工編號不能為空");
            }

            // 檢查年度是否已審核（年度修改唯讀控制）
            var isYearAudited = await _repository.IsYearAuditedAsync(dto.GiveYear, dto.SiteId);
            if (isYearAudited)
            {
                throw new InvalidOperationException($"年度 {dto.GiveYear} 已審核，無法新增資料");
            }

            var entity = new BusinessReportPrint
            {
                GiveYear = dto.GiveYear,
                SiteId = dto.SiteId,
                OrgId = dto.OrgId,
                EmpId = dto.EmpId,
                EmpName = dto.EmpName,
                Qty = dto.Qty ?? 0,
                Status = dto.Status,
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
            _logger.LogError("新增業務報表列印失敗", ex);
            throw;
        }
    }

    public async Task UpdateBusinessReportPrintAsync(long tKey, UpdateBusinessReportPrintDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"業務報表列印不存在: {tKey}");
            }

            // 驗證必填欄位
            if (dto.GiveYear <= 0)
            {
                throw new ArgumentException("發放年度不能為空或小於等於0");
            }

            if (string.IsNullOrWhiteSpace(dto.SiteId))
            {
                throw new ArgumentException("店別代碼不能為空");
            }

            if (string.IsNullOrWhiteSpace(dto.EmpId))
            {
                throw new ArgumentException("員工編號不能為空");
            }

            // 檢查年度是否已審核（年度修改唯讀控制）
            var isYearAudited = await _repository.IsYearAuditedAsync(dto.GiveYear, dto.SiteId);
            if (isYearAudited)
            {
                throw new InvalidOperationException($"年度 {dto.GiveYear} 已審核，無法修改資料");
            }

            // 已審核的資料不可修改
            if (entity.Status == "A")
            {
                throw new InvalidOperationException("已審核的資料不可修改");
            }

            entity.GiveYear = dto.GiveYear;
            entity.SiteId = dto.SiteId;
            entity.OrgId = dto.OrgId;
            entity.EmpId = dto.EmpId;
            entity.EmpName = dto.EmpName;
            entity.Qty = dto.Qty ?? 0;
            entity.Status = dto.Status;
            entity.Notes = dto.Notes;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改業務報表列印失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteBusinessReportPrintAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"業務報表列印不存在: {tKey}");
            }

            // 已審核的資料不可刪除
            if (entity.Status == "A")
            {
                throw new InvalidOperationException("已審核的資料不可刪除");
            }

            await _repository.DeleteAsync(tKey);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除業務報表列印失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<int> BatchDeleteBusinessReportPrintAsync(List<long> tKeys)
    {
        try
        {
            // 檢查是否有已審核的資料
            foreach (var tKey in tKeys)
            {
                var entity = await _repository.GetByIdAsync(tKey);
                if (entity != null && entity.Status == "A")
                {
                    throw new InvalidOperationException($"資料 {tKey} 已審核，無法刪除");
                }
            }

            return await _repository.BatchDeleteAsync(tKeys);
        }
        catch (Exception ex)
        {
            _logger.LogError("批次刪除業務報表列印失敗", ex);
            throw;
        }
    }

    public async Task<BatchAuditResultDto> BatchAuditAsync(BatchAuditDto dto)
    {
        try
        {
            var verifier = GetCurrentUserId();
            var verifyDate = DateTime.Now;

            var successCount = await _repository.BatchAuditAsync(
                dto.TKeys,
                dto.Status,
                verifier,
                verifyDate,
                dto.Notes);

            return new BatchAuditResultDto
            {
                SuccessCount = successCount,
                FailCount = dto.TKeys.Count - successCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("批次審核業務報表列印失敗", ex);
            throw;
        }
    }

    public async Task<CopyNextYearResultDto> CopyNextYearAsync(CopyNextYearDto dto)
    {
        try
        {
            if (dto.SourceYear <= 0 || dto.TargetYear <= 0)
            {
                throw new ArgumentException("年度必須大於0");
            }

            if (dto.TargetYear <= dto.SourceYear)
            {
                throw new ArgumentException("目標年度必須大於來源年度");
            }

            var copiedCount = await _repository.CopyNextYearAsync(
                dto.SourceYear,
                dto.TargetYear,
                dto.SiteId);

            return new CopyNextYearResultDto
            {
                CopiedCount = copiedCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"複製下一年度資料失敗: SourceYear={dto.SourceYear}, TargetYear={dto.TargetYear}", ex);
            throw;
        }
    }

    public async Task<CalculateQtyResultDto> CalculateQtyAsync(CalculateQtyDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(dto.TKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"業務報表列印不存在: {dto.TKey}");
            }

            // 這裡可以根據業務規則計算數量
            // 目前先返回現有數量，實際業務邏輯需要根據需求實現
            decimal calculatedQty = entity.Qty ?? 0;

            // 如果有計算規則，可以根據規則計算
            if (dto.CalculationRules != null && dto.CalculationRules.Count > 0)
            {
                // 根據計算規則計算數量
                // 這裡需要根據實際業務需求實現
            }

            return new CalculateQtyResultDto
            {
                Qty = calculatedQty
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"計算數量失敗: {dto.TKey}", ex);
            throw;
        }
    }

    public async Task<bool> IsYearAuditedAsync(int giveYear, string? siteId = null)
    {
        try
        {
            return await _repository.IsYearAuditedAsync(giveYear, siteId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查年度審核狀態失敗: GiveYear={giveYear}", ex);
            throw;
        }
    }

    /// <summary>
    /// 將 Entity 轉換為 DTO
    /// </summary>
    private BusinessReportPrintDto MapToDto(BusinessReportPrint entity)
    {
        return new BusinessReportPrintDto
        {
            TKey = entity.TKey,
            GiveYear = entity.GiveYear,
            SiteId = entity.SiteId,
            OrgId = entity.OrgId,
            EmpId = entity.EmpId,
            EmpName = entity.EmpName,
            Qty = entity.Qty,
            Status = entity.Status,
            StatusName = GetStatusName(entity.Status),
            Verifier = entity.Verifier,
            VerifyDate = entity.VerifyDate,
            Notes = entity.Notes,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }

    /// <summary>
    /// 取得狀態名稱
    /// </summary>
    private string GetStatusName(string status)
    {
        return status switch
        {
            "P" => "待審核",
            "A" => "已審核",
            "R" => "已拒絕",
            _ => status
        };
    }
}
