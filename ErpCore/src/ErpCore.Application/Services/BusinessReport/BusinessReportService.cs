using ErpCore.Application.DTOs.BusinessReport;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.BusinessReport;
using ErpCore.Infrastructure.Repositories.BusinessReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.BusinessReport;

/// <summary>
/// 業務報表服務實作 (SYSL135)
/// </summary>
public class BusinessReportService : BaseService, IBusinessReportService
{
    private readonly IBusinessReportRepository _repository;

    public BusinessReportService(
        IBusinessReportRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<BusinessReportDto>> GetBusinessReportsAsync(BusinessReportQueryDto query)
    {
        try
        {
            // 驗證查詢參數
            ValidateQuery(query);

            var repositoryQuery = new BusinessReportQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                SiteId = query.SiteId,
                CardType = query.CardType,
                VendorId = query.VendorId,
                StoreId = query.StoreId,
                OrgId = query.OrgId,
                StartDate = query.StartDate,
                EndDate = query.EndDate
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<BusinessReportDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢業務報表列表失敗", ex);
            throw;
        }
    }

    /// <summary>
    /// 驗證查詢參數
    /// </summary>
    private void ValidateQuery(BusinessReportQueryDto query)
    {
        // 驗證分頁參數
        if (query.PageIndex < 1)
        {
            throw new ArgumentException("頁碼必須大於 0");
        }

        if (query.PageSize < 1 || query.PageSize > 1000)
        {
            throw new ArgumentException("每頁筆數必須在 1 到 1000 之間");
        }

        // 驗證日期範圍
        if (query.StartDate.HasValue && query.EndDate.HasValue)
        {
            if (query.StartDate.Value > query.EndDate.Value)
            {
                throw new ArgumentException("開始日期不能大於結束日期");
            }
        }

        // 驗證卡別為 2004 時，專櫃代碼為必填
        if (query.CardType == "2004" && string.IsNullOrWhiteSpace(query.StoreId))
        {
            throw new ArgumentException("卡別為 2004 時，專櫃代碼為必填");
        }
    }

    /// <summary>
    /// 將 Entity 轉換為 DTO
    /// </summary>
    private BusinessReportDto MapToDto(BusinessReport entity)
    {
        return new BusinessReportDto
        {
            ReportId = entity.ReportId,
            SiteId = entity.SiteId,
            SiteName = entity.SiteName,
            CardType = entity.CardType,
            CardTypeName = entity.CardTypeName,
            VendorId = entity.VendorId,
            VendorName = entity.VendorName,
            StoreId = entity.StoreId,
            StoreName = entity.StoreName,
            AgreementId = entity.AgreementId,
            OrgId = entity.OrgId,
            OrgName = entity.OrgName,
            ActionType = entity.ActionType,
            ActionTypeName = entity.ActionTypeName,
            ReportDate = entity.ReportDate,
            Status = entity.Status,
            Notes = entity.Notes,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

