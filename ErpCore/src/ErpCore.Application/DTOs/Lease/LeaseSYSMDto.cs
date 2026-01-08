namespace ErpCore.Application.DTOs.Lease;

/// <summary>
/// 停車位資料 DTO (SYSM111-SYSM138)
/// </summary>
public class ParkingSpaceDto
{
    public long TKey { get; set; }
    public string ParkingSpaceId { get; set; } = string.Empty;
    public string? ParkingSpaceNo { get; set; }
    public string? ShopId { get; set; }
    public string? FloorId { get; set; }
    public decimal? Area { get; set; }
    public string Status { get; set; } = "A";
    public string? StatusName { get; set; }
    public string? LeaseId { get; set; }
    public string? Memo { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 建立停車位 DTO
/// </summary>
public class CreateParkingSpaceDto
{
    public string ParkingSpaceId { get; set; } = string.Empty;
    public string? ParkingSpaceNo { get; set; }
    public string? ShopId { get; set; }
    public string? FloorId { get; set; }
    public decimal? Area { get; set; }
    public string Status { get; set; } = "A";
    public string? LeaseId { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 修改停車位 DTO
/// </summary>
public class UpdateParkingSpaceDto
{
    public string? ParkingSpaceNo { get; set; }
    public string? ShopId { get; set; }
    public string? FloorId { get; set; }
    public decimal? Area { get; set; }
    public string Status { get; set; } = "A";
    public string? LeaseId { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 租賃合同資料 DTO (SYSM111-SYSM138)
/// </summary>
public class LeaseContractDto
{
    public long TKey { get; set; }
    public string ContractNo { get; set; } = string.Empty;
    public string LeaseId { get; set; } = string.Empty;
    public DateTime ContractDate { get; set; }
    public string? ContractType { get; set; }
    public string? ContractContent { get; set; }
    public string Status { get; set; } = "A";
    public string? StatusName { get; set; }
    public string? SignedBy { get; set; }
    public DateTime? SignedDate { get; set; }
    public string? Memo { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 建立租賃合同 DTO
/// </summary>
public class CreateLeaseContractDto
{
    public string ContractNo { get; set; } = string.Empty;
    public string LeaseId { get; set; } = string.Empty;
    public DateTime ContractDate { get; set; }
    public string? ContractType { get; set; }
    public string? ContractContent { get; set; }
    public string Status { get; set; } = "A";
    public string? SignedBy { get; set; }
    public DateTime? SignedDate { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 修改租賃合同 DTO
/// </summary>
public class UpdateLeaseContractDto
{
    public DateTime ContractDate { get; set; }
    public string? ContractType { get; set; }
    public string? ContractContent { get; set; }
    public string Status { get; set; } = "A";
    public string? SignedBy { get; set; }
    public DateTime? SignedDate { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 租賃報表查詢記錄 DTO (SYSM141-SYSM144)
/// </summary>
public class LeaseReportQueryDto
{
    public long TKey { get; set; }
    public string QueryId { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public string? QueryName { get; set; }
    public string? QueryParams { get; set; }
    public string? QueryResult { get; set; }
    public DateTime QueryDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 建立租賃報表查詢記錄 DTO
/// </summary>
public class CreateLeaseReportQueryDto
{
    public string QueryId { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public string? QueryName { get; set; }
    public string? QueryParams { get; set; }
    public string? QueryResult { get; set; }
    public DateTime QueryDate { get; set; }
}

/// <summary>
/// 查詢停車位 DTO
/// </summary>
public class ParkingSpaceQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ParkingSpaceId { get; set; }
    public string? ShopId { get; set; }
    public string? FloorId { get; set; }
    public string? Status { get; set; }
    public string? LeaseId { get; set; }
}

/// <summary>
/// 查詢租賃合同 DTO
/// </summary>
public class LeaseContractQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ContractNo { get; set; }
    public string? LeaseId { get; set; }
    public string? ContractType { get; set; }
    public string? Status { get; set; }
    public DateTime? ContractDateFrom { get; set; }
    public DateTime? ContractDateTo { get; set; }
}

/// <summary>
/// 查詢租賃報表查詢記錄 DTO
/// </summary>
public class LeaseReportQueryQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? QueryId { get; set; }
    public string? ReportType { get; set; }
    public string? QueryName { get; set; }
    public DateTime? QueryDateFrom { get; set; }
    public DateTime? QueryDateTo { get; set; }
}

/// <summary>
/// 更新停車位狀態 DTO
/// </summary>
public class UpdateParkingSpaceStatusDto
{
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// 更新租賃合同狀態 DTO
/// </summary>
public class UpdateLeaseContractStatusDto
{
    public string Status { get; set; } = string.Empty;
}

