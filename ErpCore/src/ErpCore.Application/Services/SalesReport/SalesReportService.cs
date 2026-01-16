using ErpCore.Application.DTOs.SalesReport;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.SalesReport;
using ErpCore.Infrastructure.Repositories.SalesReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.SalesReport;

/// <summary>
/// 銷售報表服務實作 (SYS1000 - 銷售報表模組系列)
/// </summary>
public class SalesReportService : BaseService, ISalesReportService
{
    private readonly ISalesReportRepository _repository;

    public SalesReportService(
        ISalesReportRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<SalesReportDto>> GetSalesReportsAsync(SalesReportQueryDto query)
    {
        try
        {
            var repositoryQuery = new SalesReportQuery
            {
                ReportCode = query.ReportCode,
                ShopId = query.ShopId,
                StartDate = query.StartDate,
                EndDate = query.EndDate,
                Status = query.Status,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder
            };

            var result = await _repository.QueryAsync(repositoryQuery);
            var dtos = result.Items.Select(MapToDto).ToList();

            return new PagedResult<SalesReportDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢銷售報表列表失敗", ex);
            throw;
        }
    }

    public async Task<SalesReportDto?> GetSalesReportByIdAsync(string reportId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(reportId);
            return entity != null ? MapToDto(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢銷售報表失敗: {reportId}", ex);
            throw;
        }
    }

    public async Task<string> CreateSalesReportAsync(CreateSalesReportDto dto)
    {
        try
        {
            var reportId = GenerateReportId();
            var now = DateTime.Now;
            var userId = _userContext.GetUserId();

            var entity = new SalesReport
            {
                ReportId = reportId,
                ReportCode = dto.ReportCode,
                ReportName = dto.ReportName,
                ReportType = dto.ReportType,
                ShopId = dto.ShopId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                ReportData = dto.ReportData,
                Status = dto.Status,
                CreatedBy = userId,
                CreatedAt = now,
                UpdatedBy = userId,
                UpdatedAt = now
            };

            await _repository.CreateAsync(entity);
            return reportId;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增銷售報表失敗", ex);
            throw;
        }
    }

    public async Task UpdateSalesReportAsync(string reportId, UpdateSalesReportDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(reportId);
            if (entity == null)
            {
                throw new Exception($"銷售報表不存在: {reportId}");
            }

            entity.ReportCode = dto.ReportCode;
            entity.ReportName = dto.ReportName;
            entity.ReportType = dto.ReportType;
            entity.ShopId = dto.ShopId;
            entity.StartDate = dto.StartDate;
            entity.EndDate = dto.EndDate;
            entity.ReportData = dto.ReportData;
            entity.Status = dto.Status;
            entity.UpdatedBy = _userContext.GetUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改銷售報表失敗: {reportId}", ex);
            throw;
        }
    }

    public async Task DeleteSalesReportAsync(string reportId)
    {
        try
        {
            await _repository.DeleteAsync(reportId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除銷售報表失敗: {reportId}", ex);
            throw;
        }
    }

    public async Task<GenerateReportResponseDto> GenerateReportAsync(GenerateReportDto dto)
    {
        try
        {
            // 生成報表邏輯
            var reportId = GenerateReportId();
            var now = DateTime.Now;
            var userId = _userContext.GetUserId();

            var entity = new SalesReport
            {
                ReportId = reportId,
                ReportCode = dto.ReportCode,
                ReportName = $"報表-{dto.ReportCode}",
                ShopId = dto.ShopId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = "A",
                CreatedBy = userId,
                CreatedAt = now,
                UpdatedBy = userId,
                UpdatedAt = now
            };

            await _repository.CreateAsync(entity);

            return new GenerateReportResponseDto
            {
                ReportId = reportId,
                ReportUrl = $"/api/v1/sales-reports/{reportId}/download"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("生成報表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> DownloadReportAsync(string reportId, string format)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(reportId);
            if (entity == null)
            {
                throw new Exception($"銷售報表不存在: {reportId}");
            }

            // 這裡應該實作實際的報表下載邏輯
            // 目前返回空陣列作為範例
            return Array.Empty<byte>();
        }
        catch (Exception ex)
        {
            _logger.LogError($"下載報表失敗: {reportId}", ex);
            throw;
        }
    }

    private SalesReportDto MapToDto(ErpCore.Domain.Entities.SalesReport.SalesReport entity)
    {
        return new SalesReportDto
        {
            ReportId = entity.ReportId,
            ReportCode = entity.ReportCode,
            ReportName = entity.ReportName,
            ReportType = entity.ReportType,
            ShopId = entity.ShopId,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            ReportData = entity.ReportData,
            Status = entity.Status,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }

    private string GenerateReportId()
    {
        return $"RPT{DateTime.Now:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
    }
}

