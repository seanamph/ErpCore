using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.ChartTools;
using ErpCore.Infrastructure.Repositories.ChartTools;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.ChartTools;

/// <summary>
/// 圖表配置服務實作
/// </summary>
public class ChartConfigService : BaseService, IChartConfigService
{
    private readonly IChartConfigRepository _repository;

    public ChartConfigService(
        IChartConfigRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<ChartConfigDto>> GetChartConfigsAsync(ChartConfigQueryDto query)
    {
        try
        {
            var repositoryQuery = new ChartConfigQuery
            {
                ChartName = query.ChartName,
                ChartType = query.ChartType,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(x => new ChartConfigDto
            {
                ChartConfigId = x.ChartConfigId,
                ChartName = x.ChartName,
                ChartType = x.ChartType,
                Title = x.Title,
                Width = x.Width,
                Height = x.Height,
                CreatedAt = x.CreatedAt
            }).ToList();

            return new PagedResult<ChartConfigDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢圖表配置列表失敗", ex);
            throw;
        }
    }

    public async Task<ChartConfigDto?> GetChartConfigByIdAsync(Guid chartConfigId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(chartConfigId);
            if (entity == null) return null;

            return new ChartConfigDto
            {
                ChartConfigId = entity.ChartConfigId,
                ChartName = entity.ChartName,
                ChartType = entity.ChartType,
                Title = entity.Title,
                Width = entity.Width,
                Height = entity.Height,
                CreatedAt = entity.CreatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢圖表配置失敗: {chartConfigId}", ex);
            throw;
        }
    }

    public async Task<Guid> CreateChartConfigAsync(CreateChartConfigDto dto)
    {
        try
        {
            var entity = new ChartConfig
            {
                ChartConfigId = Guid.NewGuid(),
                ChartName = dto.ChartName,
                ChartType = dto.ChartType,
                DataSource = dto.DataSource,
                XField = dto.XField,
                YField = dto.YField,
                Title = dto.Title,
                Width = dto.Width,
                Height = dto.Height,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var id = await _repository.CreateAsync(entity);
            _logger.LogInfo($"新增圖表配置成功: {id}");
            return id;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增圖表配置失敗", ex);
            throw;
        }
    }

    public async Task UpdateChartConfigAsync(Guid chartConfigId, UpdateChartConfigDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(chartConfigId);
            if (entity == null)
            {
                throw new Exception($"圖表配置不存在: {chartConfigId}");
            }

            entity.ChartName = dto.ChartName;
            entity.ChartType = dto.ChartType;
            entity.DataSource = dto.DataSource;
            entity.XField = dto.XField;
            entity.YField = dto.YField;
            entity.Title = dto.Title;
            entity.Width = dto.Width;
            entity.Height = dto.Height;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
            _logger.LogInfo($"修改圖表配置成功: {chartConfigId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改圖表配置失敗: {chartConfigId}", ex);
            throw;
        }
    }

    public async Task DeleteChartConfigAsync(Guid chartConfigId)
    {
        try
        {
            await _repository.DeleteAsync(chartConfigId);
            _logger.LogInfo($"刪除圖表配置成功: {chartConfigId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除圖表配置失敗: {chartConfigId}", ex);
            throw;
        }
    }
}

