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

