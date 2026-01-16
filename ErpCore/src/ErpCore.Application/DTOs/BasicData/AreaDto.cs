namespace ErpCore.Application.DTOs.BasicData;

/// <summary>
/// 區域 DTO
/// </summary>
public class AreaDto
{
    public string AreaId { get; set; } = string.Empty;
    public string AreaName { get; set; } = string.Empty;
    public int? SeqNo { get; set; }
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 區域查詢 DTO
/// </summary>
public class AreaQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? AreaId { get; set; }
    public string? AreaName { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 新增區域 DTO
/// </summary>
public class CreateAreaDto
{
    public string AreaId { get; set; } = string.Empty;
    public string AreaName { get; set; } = string.Empty;
    public int? SeqNo { get; set; }
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
}

/// <summary>
/// 修改區域 DTO
/// </summary>
public class UpdateAreaDto
{
    public string AreaName { get; set; } = string.Empty;
    public int? SeqNo { get; set; }
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
}

/// <summary>
/// 批次刪除區域 DTO
/// </summary>
public class BatchDeleteAreaDto
{
    public List<string> AreaIds { get; set; } = new();
}

/// <summary>
/// 更新狀態 DTO
/// </summary>
public class UpdateAreaStatusDto
{
    public string Status { get; set; } = "A";
}
