using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.ChartTools;

/// <summary>
/// 圖表配置服務介面
/// </summary>
public interface IChartConfigService
{
    Task<PagedResult<ChartConfigDto>> GetChartConfigsAsync(ChartConfigQueryDto query);
    Task<ChartConfigDto?> GetChartConfigByIdAsync(Guid chartConfigId);
    Task<Guid> CreateChartConfigAsync(CreateChartConfigDto dto);
    Task UpdateChartConfigAsync(Guid chartConfigId, UpdateChartConfigDto dto);
    Task DeleteChartConfigAsync(Guid chartConfigId);
}

/// <summary>
/// 圖表配置 DTO
/// </summary>
public class ChartConfigDto
{
    public Guid ChartConfigId { get; set; }
    public string ChartName { get; set; } = string.Empty;
    public string ChartType { get; set; } = string.Empty;
    public string? Title { get; set; }
    public int Width { get; set; } = 800;
    public int Height { get; set; } = 400;
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 建立圖表配置 DTO
/// </summary>
public class CreateChartConfigDto
{
    public string ChartName { get; set; } = string.Empty;
    public string ChartType { get; set; } = string.Empty;
    public string? DataSource { get; set; }
    public string? XField { get; set; }
    public string? YField { get; set; }
    public string? Title { get; set; }
    public int Width { get; set; } = 800;
    public int Height { get; set; } = 400;
}

/// <summary>
/// 修改圖表配置 DTO
/// </summary>
public class UpdateChartConfigDto
{
    public string ChartName { get; set; } = string.Empty;
    public string ChartType { get; set; } = string.Empty;
    public string? DataSource { get; set; }
    public string? XField { get; set; }
    public string? YField { get; set; }
    public string? Title { get; set; }
    public int Width { get; set; } = 800;
    public int Height { get; set; } = 400;
}

/// <summary>
/// 圖表配置查詢 DTO
/// </summary>
public class ChartConfigQueryDto
{
    public string? ChartName { get; set; }
    public string? ChartType { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

