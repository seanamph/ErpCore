namespace ErpCore.Application.DTOs.BasicData;

/// <summary>
/// 庫別 DTO
/// </summary>
public class WarehouseDto
{
    public string WarehouseId { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public string? WarehouseType { get; set; }
    public string? Location { get; set; }
    public int? SeqNo { get; set; }
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 庫別查詢 DTO
/// </summary>
public class WarehouseQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public string? WarehouseType { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 新增庫別 DTO
/// </summary>
public class CreateWarehouseDto
{
    public string WarehouseId { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public string? WarehouseType { get; set; }
    public string? Location { get; set; }
    public int? SeqNo { get; set; }
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
}

/// <summary>
/// 修改庫別 DTO
/// </summary>
public class UpdateWarehouseDto
{
    public string WarehouseName { get; set; } = string.Empty;
    public string? WarehouseType { get; set; }
    public string? Location { get; set; }
    public int? SeqNo { get; set; }
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
}

/// <summary>
/// 批次刪除庫別 DTO
/// </summary>
public class BatchDeleteWarehouseDto
{
    public List<string> WarehouseIds { get; set; } = new();
}

/// <summary>
/// 更新狀態 DTO
/// </summary>
public class UpdateWarehouseStatusDto
{
    public string Status { get; set; } = "A";
}
