namespace ErpCore.Application.DTOs.Recruitment;

/// <summary>
/// 潛客 DTO (SYSC180)
/// </summary>
public class ProspectDto
{
    public string ProspectId { get; set; } = string.Empty;
    public string ProspectName { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? ContactTel { get; set; }
    public string? ContactFax { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactAddress { get; set; }
    public string? StoreName { get; set; }
    public string? StoreTel { get; set; }
    public string? SiteId { get; set; }
    public string? RecruitId { get; set; }
    public string? StoreId { get; set; }
    public string? VendorId { get; set; }
    public string? OrgId { get; set; }
    public string? BtypeId { get; set; }
    public string? SalesType { get; set; }
    public string Status { get; set; } = "PENDING";
    public string? OverallStatus { get; set; }
    public string? PaperType { get; set; }
    public string? LocationType { get; set; }
    public string? DecoType { get; set; }
    public string? CommType { get; set; }
    public string? PdType { get; set; }
    public decimal? BaseRent { get; set; }
    public decimal? Deposit { get; set; }
    public string? CreditCard { get; set; }
    public decimal? TargetAmountM { get; set; }
    public decimal? TargetAmountV { get; set; }
    public decimal? ExerciseFees { get; set; }
    public int? CheckDay { get; set; }
    public DateTime? AgmDateB { get; set; }
    public DateTime? AgmDateE { get; set; }
    public string? ContractProidB { get; set; }
    public string? ContractProidE { get; set; }
    public DateTime? FeedbackDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? ContactDate { get; set; }
    public string? VersionNo { get; set; }
    public string? GuiId { get; set; }
    public string? BankId { get; set; }
    public string? AccName { get; set; }
    public string? AccNo { get; set; }
    public string? InvEmail { get; set; }
    public string EdcYn { get; set; } = "N";
    public string ReceYn { get; set; } = "N";
    public string PosYn { get; set; } = "N";
    public string CashYn { get; set; } = "N";
    public string CommYn { get; set; } = "N";
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 潛客查詢 DTO
/// </summary>
public class ProspectQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? ProspectId { get; set; }
    public string? ProspectName { get; set; }
    public string? Status { get; set; }
    public string? SiteId { get; set; }
    public string? RecruitId { get; set; }
    public DateTime? ContactDateFrom { get; set; }
    public DateTime? ContactDateTo { get; set; }
}

/// <summary>
/// 新增潛客 DTO
/// </summary>
public class CreateProspectDto
{
    public string ProspectId { get; set; } = string.Empty;
    public string ProspectName { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? ContactTel { get; set; }
    public string? ContactFax { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactAddress { get; set; }
    public string? StoreName { get; set; }
    public string? StoreTel { get; set; }
    public string? SiteId { get; set; }
    public string? RecruitId { get; set; }
    public string? StoreId { get; set; }
    public string? VendorId { get; set; }
    public string? OrgId { get; set; }
    public string? BtypeId { get; set; }
    public string? SalesType { get; set; }
    public string Status { get; set; } = "PENDING";
    public string? OverallStatus { get; set; }
    public string? PaperType { get; set; }
    public string? LocationType { get; set; }
    public string? DecoType { get; set; }
    public string? CommType { get; set; }
    public string? PdType { get; set; }
    public decimal? BaseRent { get; set; }
    public decimal? Deposit { get; set; }
    public string? CreditCard { get; set; }
    public decimal? TargetAmountM { get; set; }
    public decimal? TargetAmountV { get; set; }
    public decimal? ExerciseFees { get; set; }
    public int? CheckDay { get; set; }
    public DateTime? AgmDateB { get; set; }
    public DateTime? AgmDateE { get; set; }
    public string? ContractProidB { get; set; }
    public string? ContractProidE { get; set; }
    public DateTime? FeedbackDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? ContactDate { get; set; }
    public string? VersionNo { get; set; }
    public string? GuiId { get; set; }
    public string? BankId { get; set; }
    public string? AccName { get; set; }
    public string? AccNo { get; set; }
    public string? InvEmail { get; set; }
    public string EdcYn { get; set; } = "N";
    public string ReceYn { get; set; } = "N";
    public string PosYn { get; set; } = "N";
    public string CashYn { get; set; } = "N";
    public string CommYn { get; set; } = "N";
    public string? Notes { get; set; }
}

/// <summary>
/// 修改潛客 DTO
/// </summary>
public class UpdateProspectDto
{
    public string ProspectName { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? ContactTel { get; set; }
    public string? ContactFax { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactAddress { get; set; }
    public string? StoreName { get; set; }
    public string? StoreTel { get; set; }
    public string? SiteId { get; set; }
    public string? RecruitId { get; set; }
    public string? StoreId { get; set; }
    public string? VendorId { get; set; }
    public string? OrgId { get; set; }
    public string? BtypeId { get; set; }
    public string? SalesType { get; set; }
    public string Status { get; set; } = "PENDING";
    public string? OverallStatus { get; set; }
    public string? PaperType { get; set; }
    public string? LocationType { get; set; }
    public string? DecoType { get; set; }
    public string? CommType { get; set; }
    public string? PdType { get; set; }
    public decimal? BaseRent { get; set; }
    public decimal? Deposit { get; set; }
    public string? CreditCard { get; set; }
    public decimal? TargetAmountM { get; set; }
    public decimal? TargetAmountV { get; set; }
    public decimal? ExerciseFees { get; set; }
    public int? CheckDay { get; set; }
    public DateTime? AgmDateB { get; set; }
    public DateTime? AgmDateE { get; set; }
    public string? ContractProidB { get; set; }
    public string? ContractProidE { get; set; }
    public DateTime? FeedbackDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? ContactDate { get; set; }
    public string? VersionNo { get; set; }
    public string? GuiId { get; set; }
    public string? BankId { get; set; }
    public string? AccName { get; set; }
    public string? AccNo { get; set; }
    public string? InvEmail { get; set; }
    public string EdcYn { get; set; } = "N";
    public string ReceYn { get; set; } = "N";
    public string PosYn { get; set; } = "N";
    public string CashYn { get; set; } = "N";
    public string CommYn { get; set; } = "N";
    public string? Notes { get; set; }
}

/// <summary>
/// 批次刪除潛客 DTO
/// </summary>
public class BatchDeleteProspectDto
{
    public List<string> ProspectIds { get; set; } = new();
}

/// <summary>
/// 更新潛客狀態 DTO
/// </summary>
public class UpdateProspectStatusDto
{
    public string Status { get; set; } = string.Empty;
}

