namespace ErpCore.Application.DTOs.Accounting;

/// <summary>
/// 資產 DTO (SYSN310)
/// </summary>
public class AssetDto
{
    public long TKey { get; set; }
    public string AssetId { get; set; } = string.Empty;
    public string AssetName { get; set; } = string.Empty;
    public string? AssetType { get; set; }
    public DateTime? AcquisitionDate { get; set; }
    public decimal? AcquisitionCost { get; set; }
    public string? DepreciationMethod { get; set; }
    public int? UsefulLife { get; set; }
    public decimal? ResidualValue { get; set; }
    public string Status { get; set; } = "A";
    public string? Location { get; set; }
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 資產查詢 DTO
/// </summary>
public class AssetQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? AssetId { get; set; }
    public string? AssetName { get; set; }
    public string? AssetType { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 新增資產 DTO
/// </summary>
public class CreateAssetDto
{
    public string AssetId { get; set; } = string.Empty;
    public string AssetName { get; set; } = string.Empty;
    public string? AssetType { get; set; }
    public DateTime? AcquisitionDate { get; set; }
    public decimal? AcquisitionCost { get; set; }
    public string? DepreciationMethod { get; set; }
    public int? UsefulLife { get; set; }
    public decimal? ResidualValue { get; set; }
    public string? Location { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 修改資產 DTO
/// </summary>
public class UpdateAssetDto
{
    public string AssetName { get; set; } = string.Empty;
    public string? AssetType { get; set; }
    public DateTime? AcquisitionDate { get; set; }
    public decimal? AcquisitionCost { get; set; }
    public string? DepreciationMethod { get; set; }
    public int? UsefulLife { get; set; }
    public decimal? ResidualValue { get; set; }
    public string? Location { get; set; }
    public string? Notes { get; set; }
}

