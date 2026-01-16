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
    private readonly ExportHelper _exportHelper;

    public BusinessReportService(
        IBusinessReportRepository repository,
        ExportHelper exportHelper,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _exportHelper = exportHelper;
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
    private BusinessReportDto MapToDto(ErpCore.Domain.Entities.BusinessReport.BusinessReport entity)
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

    public async Task<byte[]> ExportBusinessReportsAsync(BusinessReportQueryDto query, string format)
    {
        try
        {
            // 查詢所有資料（不分頁）
            var allDataQuery = new BusinessReportQuery
            {
                PageIndex = 1,
                PageSize = int.MaxValue,
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

            var result = await _repository.QueryAsync(allDataQuery);
            var dtos = result.Items.Select(x => MapToDto(x)).ToList();

            // 定義匯出欄位
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "SiteId", DisplayName = "店別代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SiteName", DisplayName = "店別名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "CardType", DisplayName = "卡片類型", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "CardTypeName", DisplayName = "卡片類型名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "VendorId", DisplayName = "廠商代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "VendorName", DisplayName = "廠商名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "StoreId", DisplayName = "專櫃代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "StoreName", DisplayName = "專櫃名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "AgreementId", DisplayName = "合約代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "OrgId", DisplayName = "組織代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "OrgName", DisplayName = "組織名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ActionType", DisplayName = "動作類型", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ActionTypeName", DisplayName = "動作類型名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ReportDate", DisplayName = "報表日期", DataType = ExportDataType.DateTime }
            };

            if (format.ToLower() == "excel")
            {
                return _exportHelper.ExportToExcel(dtos, columns, "業務報表查詢", "業務報表查詢作業 (SYSL135)");
            }
            else if (format.ToLower() == "pdf")
            {
                return _exportHelper.ExportToPdf(dtos, columns, "業務報表查詢作業 (SYSL135)");
            }
            else
            {
                throw new ArgumentException($"不支援的匯出格式: {format}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出業務報表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> PrintBusinessReportsAsync(BusinessReportQueryDto query)
    {
        try
        {
            // 列印功能使用 PDF 格式
            return await ExportBusinessReportsAsync(query, "pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError("列印業務報表失敗", ex);
            throw;
        }
    }
}

