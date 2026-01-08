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

    public async Task<BusinessReportPrintDto?> GetBusinessReportPrintByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                return null;
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
            var entity = new BusinessReportPrint
            {
                GiveYear = dto.GiveYear,
                SiteId = dto.SiteId,
                OrgId = dto.OrgId,
                EmpId = dto.EmpId,
                EmpName = dto.EmpName,
                Qty = dto.Qty,
                Status = dto.Status,
                Notes = dto.Notes,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var tKey = await _repository.CreateAsync(entity);
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增業務報表列印失敗", ex);
            throw;
        }
    }

    public async Task<bool> UpdateBusinessReportPrintAsync(long tKey, UpdateBusinessReportPrintDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new Exception($"找不到業務報表列印資料: {tKey}");
            }

            // 檢查年度修改唯讀控制（已審核的年度不可修改）
            if (entity.Status == "A" && entity.GiveYear != DateTime.Now.Year)
            {
                throw new Exception("已審核的年度資料不可修改");
            }

            entity.OrgId = dto.OrgId ?? entity.OrgId;
            entity.EmpName = dto.EmpName ?? entity.EmpName;
            entity.Qty = dto.Qty ?? entity.Qty;
            entity.Status = dto.Status ?? entity.Status;
            entity.Notes = dto.Notes ?? entity.Notes;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            return await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改業務報表列印失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<bool> DeleteBusinessReportPrintAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new Exception($"找不到業務報表列印資料: {tKey}");
            }

            // 檢查年度修改唯讀控制（已審核的年度不可刪除）
            if (entity.Status == "A" && entity.GiveYear != DateTime.Now.Year)
            {
                throw new Exception("已審核的年度資料不可刪除");
            }

            return await _repository.DeleteAsync(tKey);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除業務報表列印失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<BatchAuditResultDto> BatchAuditAsync(BatchAuditBusinessReportPrintDto dto)
    {
        try
        {
            var successCount = 0;
            var failCount = 0;

            foreach (var tKey in dto.TKeys)
            {
                try
                {
                    var entity = await _repository.GetByIdAsync(tKey);
                    if (entity == null)
                    {
                        failCount++;
                        continue;
                    }

                    // 檢查審核權限（可根據需求擴展）
                    // 支援專屬審核帳號設定

                    entity.Status = dto.Status;
                    entity.Verifier = GetCurrentUserId();
                    entity.VerifyDate = DateTime.Now;
                    entity.Notes = dto.Notes ?? entity.Notes;
                    entity.UpdatedBy = GetCurrentUserId();
                    entity.UpdatedAt = DateTime.Now;

                    await _repository.UpdateAsync(entity);
                    successCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"批次審核失敗: {tKey}", ex);
                    failCount++;
                }
            }

            return new BatchAuditResultDto
            {
                SuccessCount = successCount,
                FailCount = failCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("批次審核業務報表列印失敗", ex);
            throw;
        }
    }

    public async Task<CopyResultDto> CopyNextYearAsync(CopyNextYearDto dto)
    {
        try
        {
            var copiedCount = await _repository.CopyNextYearAsync(dto.SourceYear, dto.TargetYear, dto.SiteId);

            return new CopyResultDto
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
            var qty = await _repository.CalculateQtyAsync(dto.TKey, dto.CalculationRules);

            return new CalculateQtyResultDto
            {
                Qty = qty
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"計算數量失敗: {dto.TKey}", ex);
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
            SiteName = entity.SiteName,
            OrgId = entity.OrgId,
            OrgName = entity.OrgName,
            EmpId = entity.EmpId,
            EmpName = entity.EmpName,
            Qty = entity.Qty,
            Status = entity.Status,
            Verifier = entity.Verifier,
            VerifierName = entity.VerifierName,
            VerifyDate = entity.VerifyDate,
            Notes = entity.Notes,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

