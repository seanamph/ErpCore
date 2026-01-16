namespace ErpCore.Application.DTOs.Procurement;

/// <summary>
/// 採購擴展維護 DTO (SYSPA10-SYSPB60)
/// </summary>
public class PurchaseExtendedMaintenanceDto
{
    public long TKey { get; set; }
    public string MaintenanceId { get; set; } = string.Empty;
    public string MaintenanceName { get; set; } = string.Empty;
    public string? MaintenanceType { get; set; }
    public string? MaintenanceDesc { get; set; }
    public string? MaintenanceConfig { get; set; }
    public string? ParameterConfig { get; set; }
    public string Status { get; set; } = "A";
    public int SeqNo { get; set; }
    public string? Memo { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 建立採購擴展維護 DTO
/// </summary>
public class CreatePurchaseExtendedMaintenanceDto
{
    public string MaintenanceId { get; set; } = string.Empty;
    public string MaintenanceName { get; set; } = string.Empty;
    public string? MaintenanceType { get; set; }
    public string? MaintenanceDesc { get; set; }
    public string? MaintenanceConfig { get; set; }
    public string? ParameterConfig { get; set; }
    public string Status { get; set; } = "A";
    public int SeqNo { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 修改採購擴展維護 DTO
/// </summary>
public class UpdatePurchaseExtendedMaintenanceDto
{
    public string MaintenanceName { get; set; } = string.Empty;
    public string? MaintenanceType { get; set; }
    public string? MaintenanceDesc { get; set; }
    public string? MaintenanceConfig { get; set; }
    public string? ParameterConfig { get; set; }
    public string Status { get; set; } = "A";
    public int SeqNo { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 查詢採購擴展維護 DTO
/// </summary>
public class PurchaseExtendedMaintenanceQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? MaintenanceId { get; set; }
    public string? MaintenanceName { get; set; }
    public string? MaintenanceType { get; set; }
    public string? Status { get; set; }
}
