namespace ErpCore.Application.DTOs.System;

/// <summary>
/// 主系統項目資料傳輸物件 (SYS0410)
/// </summary>
public class SystemDto
{
    public string SystemId { get; set; } = string.Empty;
    public string SystemName { get; set; } = string.Empty;
    public int? SeqNo { get; set; }
    public string? SystemType { get; set; }
    public string? SystemTypeName { get; set; }
    public string? ServerIp { get; set; }
    public string? ModuleId { get; set; }
    public string? DbUser { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = "A";
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 主系統項目查詢 DTO
/// </summary>
public class SystemQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string SortOrder { get; set; } = "ASC";
    public SystemQueryFilters? Filters { get; set; }
}

/// <summary>
/// 主系統查詢篩選條件
/// </summary>
public class SystemQueryFilters
{
    public string? SystemId { get; set; }
    public string? SystemName { get; set; }
    public string? SystemType { get; set; }
    public string? ServerIp { get; set; }
}

/// <summary>
/// 新增主系統項目 DTO
/// </summary>
public class CreateSystemDto
{
    public string SystemId { get; set; } = string.Empty;
    public string SystemName { get; set; } = string.Empty;
    public int? SeqNo { get; set; }
    public string? SystemType { get; set; }
    public string? ServerIp { get; set; }
    public string? ModuleId { get; set; }
    public string? DbUser { get; set; }
    public string? DbPass { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = "A";
}

/// <summary>
/// 修改主系統項目 DTO
/// </summary>
public class UpdateSystemDto
{
    public string SystemName { get; set; } = string.Empty;
    public int? SeqNo { get; set; }
    public string? SystemType { get; set; }
    public string? ServerIp { get; set; }
    public string? ModuleId { get; set; }
    public string? DbUser { get; set; }
    public string? DbPass { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = "A";
}

/// <summary>
/// 批次刪除 DTO
/// </summary>
public class BatchDeleteSystemsDto
{
    public List<string> SystemIds { get; set; } = new();
}

/// <summary>
/// 系統型態選項 DTO
/// </summary>
public class SystemTypeOptionDto
{
    public string Value { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
}
