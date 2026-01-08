namespace ErpCore.Application.DTOs.Recruitment;

/// <summary>
/// 潛客主檔 DTO (SYSC165)
/// </summary>
public class ProspectMasterDto
{
    public string MasterId { get; set; } = string.Empty;
    public string MasterName { get; set; } = string.Empty;
    public string? MasterType { get; set; }
    public string? Category { get; set; }
    public string? Industry { get; set; }
    public string? BusinessType { get; set; }
    public string Status { get; set; } = "ACTIVE";
    public int Priority { get; set; }
    public string? Source { get; set; }
    public string? ContactPerson { get; set; }
    public string? ContactTel { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactAddress { get; set; }
    public string? Website { get; set; }
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 潛客主檔查詢 DTO
/// </summary>
public class ProspectMasterQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? MasterId { get; set; }
    public string? MasterName { get; set; }
    public string? MasterType { get; set; }
    public string? Category { get; set; }
    public string? Industry { get; set; }
    public string? BusinessType { get; set; }
    public string? Status { get; set; }
    public string? Source { get; set; }
}

/// <summary>
/// 新增潛客主檔 DTO
/// </summary>
public class CreateProspectMasterDto
{
    public string MasterId { get; set; } = string.Empty;
    public string MasterName { get; set; } = string.Empty;
    public string? MasterType { get; set; }
    public string? Category { get; set; }
    public string? Industry { get; set; }
    public string? BusinessType { get; set; }
    public string Status { get; set; } = "ACTIVE";
    public int Priority { get; set; }
    public string? Source { get; set; }
    public string? ContactPerson { get; set; }
    public string? ContactTel { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactAddress { get; set; }
    public string? Website { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 修改潛客主檔 DTO
/// </summary>
public class UpdateProspectMasterDto
{
    public string MasterName { get; set; } = string.Empty;
    public string? MasterType { get; set; }
    public string? Category { get; set; }
    public string? Industry { get; set; }
    public string? BusinessType { get; set; }
    public string Status { get; set; } = "ACTIVE";
    public int Priority { get; set; }
    public string? Source { get; set; }
    public string? ContactPerson { get; set; }
    public string? ContactTel { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactAddress { get; set; }
    public string? Website { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 批次刪除潛客主檔 DTO
/// </summary>
public class BatchDeleteProspectMasterDto
{
    public List<string> MasterIds { get; set; } = new();
}

/// <summary>
/// 更新潛客主檔狀態 DTO
/// </summary>
public class UpdateProspectMasterStatusDto
{
    public string Status { get; set; } = string.Empty;
}

