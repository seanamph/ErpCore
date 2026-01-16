using Dapper;
using ErpCore.Application.DTOs.AnalysisReport;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.AnalysisReport;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.AnalysisReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.AnalysisReport;

/// <summary>
/// 耗材列印服務實作 (SYSA254)
/// </summary>
public class ConsumablePrintService : BaseService, IConsumablePrintService
{
    private readonly IConsumablePrintRepository _repository;
    private readonly IDbConnectionFactory _connectionFactory;

    public ConsumablePrintService(
        IConsumablePrintRepository repository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _connectionFactory = connectionFactory;
    }

    public async Task<ConsumablePrintListDto> GetPrintListAsync(ConsumablePrintQueryDto query)
    {
        try
        {
            var repositoryQuery = new ConsumablePrintQuery
            {
                Type = query.Type,
                Status = query.Status,
                SiteId = query.SiteId,
                AssetStatus = query.AssetStatus,
                ConsumableIds = query.ConsumableIds
            };

            var consumables = await _repository.GetConsumablesForPrintAsync(repositoryQuery);

            // 批量查詢分類名稱
            var categoryIds = consumables.Where(x => !string.IsNullOrEmpty(x.CategoryId))
                .Select(x => x.CategoryId!)
                .Distinct()
                .ToList();
            var categoryNameMap = new Dictionary<string, string>();
            if (categoryIds.Any())
            {
                using var connection = _connectionFactory.CreateConnection();
                var sql = "SELECT CategoryId, CategoryName FROM ConsumableCategories WHERE CategoryId IN @CategoryIds";
                var categories = await connection.QueryAsync<(string CategoryId, string CategoryName)>(sql, new { CategoryIds = categoryIds });
                categoryNameMap = categories.ToDictionary(x => x.CategoryId, x => x.CategoryName);
            }

            // 批量查詢店別名稱
            var siteIds = consumables.Where(x => !string.IsNullOrEmpty(x.SiteId))
                .Select(x => x.SiteId!)
                .Distinct()
                .ToList();
            var siteNameMap = new Dictionary<string, string>();
            if (siteIds.Any())
            {
                using var connection = _connectionFactory.CreateConnection();
                // 嘗試從 Sites 表查詢，如果不存在則從 Shops 表查詢
                var sql = @"
                    SELECT SiteId, SiteName FROM Sites WHERE SiteId IN @SiteIds
                    UNION
                    SELECT ShopId AS SiteId, ShopName AS SiteName FROM Shops WHERE ShopId IN @SiteIds";
                var sites = await connection.QueryAsync<(string SiteId, string SiteName)>(sql, new { SiteIds = siteIds });
                siteNameMap = sites.ToDictionary(x => x.SiteId, x => x.SiteName);
            }

            var items = consumables.Select(x => new ConsumablePrintItemDto
            {
                ConsumableId = x.ConsumableId,
                ConsumableName = x.ConsumableName,
                BarCode = x.BarCode,
                CategoryName = !string.IsNullOrEmpty(x.CategoryId) && categoryNameMap.ContainsKey(x.CategoryId) 
                    ? categoryNameMap[x.CategoryId] : null,
                Unit = x.Unit,
                Specification = x.Specification,
                Brand = x.Brand,
                Model = x.Model,
                Status = x.Status,
                StatusName = x.Status == "1" ? "正常" : "停用",
                AssetStatus = x.AssetStatus,
                SiteId = x.SiteId,
                SiteName = !string.IsNullOrEmpty(x.SiteId) && siteNameMap.ContainsKey(x.SiteId) 
                    ? siteNameMap[x.SiteId] : null,
                Location = x.Location,
                Quantity = x.Quantity,
                Price = x.Price
            }).ToList();

            return new ConsumablePrintListDto
            {
                Items = items,
                TotalCount = items.Count
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢耗材列表失敗", ex);
            throw;
        }
    }

    public async Task<BatchPrintResponseDto> BatchPrintAsync(BatchPrintDto dto, string userId)
    {
        try
        {
            var printLogId = Guid.NewGuid();
            var printCount = 0;

            foreach (var consumableId in dto.ConsumableIds)
            {
                var log = new ConsumablePrintLog
                {
                    LogId = Guid.NewGuid(),
                    ConsumableId = consumableId,
                    PrintType = dto.Type,
                    PrintCount = dto.PrintCount,
                    PrintDate = DateTime.Now,
                    PrintedBy = userId,
                    SiteId = dto.SiteId
                };

                await _repository.CreateLogAsync(log);
                printCount++;
            }

            return new BatchPrintResponseDto
            {
                PrintLogId = printLogId,
                PrintCount = printCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("批次列印失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<ConsumablePrintLogDto>> GetPrintLogsAsync(ConsumablePrintLogQueryDto query)
    {
        try
        {
            var repositoryQuery = new ConsumablePrintLogQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                ConsumableId = query.ConsumableId,
                PrintType = query.PrintType,
                SiteId = query.SiteId,
                PrintedBy = query.PrintedBy,
                StartDate = query.StartDate,
                EndDate = query.EndDate
            };

            var result = await _repository.GetLogsAsync(repositoryQuery);

            var dtos = result.Items.Select(x => new ConsumablePrintLogDto
            {
                LogId = x.LogId,
                ConsumableId = x.ConsumableId,
                ConsumableName = null, // 需要從關聯表取得
                PrintType = x.PrintType,
                PrintTypeName = x.PrintType == "1" ? "耗材管理報表" : "耗材標籤列印",
                PrintCount = x.PrintCount,
                PrintDate = x.PrintDate,
                PrintedBy = x.PrintedBy,
                PrintedByName = null, // 需要從關聯表取得
                SiteId = x.SiteId,
                SiteName = null // 需要從關聯表取得
            }).ToList();

            return new PagedResult<ConsumablePrintLogDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢列印記錄列表失敗", ex);
            throw;
        }
    }
}
