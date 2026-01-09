namespace ErpCore.Application.DTOs.StoreFloor;

/// <summary>
/// POS終端 DTO (SYS6610-SYS6999 - POS資料維護)
/// </summary>
public class PosTerminalDto
{
    public string PosTerminalId { get; set; } = string.Empty;
    public string PosSystemId { get; set; } = string.Empty;
    public string TerminalCode { get; set; } = string.Empty;
    public string TerminalName { get; set; } = string.Empty;
    public string? ShopId { get; set; }
    public string? FloorId { get; set; }
    public string? TerminalType { get; set; }
    public string? IpAddress { get; set; }
    public int? Port { get; set; }
    public string? Config { get; set; }
    public string Status { get; set; } = "A";
    public DateTime? LastSyncDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// POS終端查詢 DTO
/// </summary>
public class PosTerminalQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? PosTerminalId { get; set; }
    public string? PosSystemId { get; set; }
    public string? ShopId { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 建立POS終端 DTO
/// </summary>
public class CreatePosTerminalDto
{
    public string PosTerminalId { get; set; } = string.Empty;
    public string PosSystemId { get; set; } = string.Empty;
    public string TerminalCode { get; set; } = string.Empty;
    public string TerminalName { get; set; } = string.Empty;
    public string? ShopId { get; set; }
    public string? FloorId { get; set; }
    public string? TerminalType { get; set; }
    public string? IpAddress { get; set; }
    public int? Port { get; set; }
    public string? Config { get; set; }
    public string Status { get; set; } = "A";
}

/// <summary>
/// 修改POS終端 DTO
/// </summary>
public class UpdatePosTerminalDto
{
    public string PosSystemId { get; set; } = string.Empty;
    public string TerminalCode { get; set; } = string.Empty;
    public string TerminalName { get; set; } = string.Empty;
    public string? ShopId { get; set; }
    public string? FloorId { get; set; }
    public string? TerminalType { get; set; }
    public string? IpAddress { get; set; }
    public int? Port { get; set; }
    public string? Config { get; set; }
    public string Status { get; set; } = "A";
}

