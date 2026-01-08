namespace ErpCore.Application.DTOs.BasicData;

/// <summary>
/// 廠商 DTO
/// </summary>
public class VendorDto
{
    public string VendorId { get; set; } = string.Empty;
    public string GuiId { get; set; } = string.Empty;
    public string GuiType { get; set; } = "1";
    public string VendorName { get; set; } = string.Empty;
    public string? VendorNameE { get; set; }
    public string? VendorNameS { get; set; }
    public string? Mcode { get; set; }
    public string? VendorRegAddr { get; set; }
    public string? TaxAddr { get; set; }
    public string? VendorRegTel { get; set; }
    public string? VendorFax { get; set; }
    public string? VendorEmail { get; set; }
    public string? InvEmail { get; set; }
    public string? ChargeStaff { get; set; }
    public string? ChargeTitle { get; set; }
    public string? ChargePid { get; set; }
    public string? ChargeTel { get; set; }
    public string? ChargeAddr { get; set; }
    public string? ChargeEmail { get; set; }
    public string Status { get; set; } = "A";
    public string SysId { get; set; } = "1";
    public string? PayType { get; set; }
    public string? SuplBankId { get; set; }
    public string? SuplBankAcct { get; set; }
    public string? SuplAcctName { get; set; }
    public string? TicketBe { get; set; }
    public string? CheckTitle { get; set; }
    public string? OrgId { get; set; }
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 廠商查詢 DTO
/// </summary>
public class VendorQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? VendorId { get; set; }
    public string? GuiId { get; set; }
    public string? VendorName { get; set; }
    public string? Status { get; set; }
    public string? SysId { get; set; }
    public string? OrgId { get; set; }
}

/// <summary>
/// 新增廠商 DTO
/// </summary>
public class CreateVendorDto
{
    public string GuiId { get; set; } = string.Empty;
    public string GuiType { get; set; } = "1";
    public string VendorName { get; set; } = string.Empty;
    public string? VendorNameE { get; set; }
    public string? VendorNameS { get; set; }
    public string? Mcode { get; set; }
    public string? VendorRegAddr { get; set; }
    public string? TaxAddr { get; set; }
    public string? VendorRegTel { get; set; }
    public string? VendorFax { get; set; }
    public string? VendorEmail { get; set; }
    public string? InvEmail { get; set; }
    public string? ChargeStaff { get; set; }
    public string? ChargeTitle { get; set; }
    public string? ChargePid { get; set; }
    public string? ChargeTel { get; set; }
    public string? ChargeAddr { get; set; }
    public string? ChargeEmail { get; set; }
    public string Status { get; set; } = "A";
    public string SysId { get; set; } = "1";
    public string? PayType { get; set; }
    public string? SuplBankId { get; set; }
    public string? SuplBankAcct { get; set; }
    public string? SuplAcctName { get; set; }
    public string? TicketBe { get; set; }
    public string? CheckTitle { get; set; }
    public string? OrgId { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 修改廠商 DTO
/// </summary>
public class UpdateVendorDto
{
    public string VendorName { get; set; } = string.Empty;
    public string? VendorNameE { get; set; }
    public string? VendorNameS { get; set; }
    public string? Mcode { get; set; }
    public string? VendorRegAddr { get; set; }
    public string? TaxAddr { get; set; }
    public string? VendorRegTel { get; set; }
    public string? VendorFax { get; set; }
    public string? VendorEmail { get; set; }
    public string? InvEmail { get; set; }
    public string? ChargeStaff { get; set; }
    public string? ChargeTitle { get; set; }
    public string? ChargePid { get; set; }
    public string? ChargeTel { get; set; }
    public string? ChargeAddr { get; set; }
    public string? ChargeEmail { get; set; }
    public string Status { get; set; } = "A";
    public string SysId { get; set; } = "1";
    public string? PayType { get; set; }
    public string? SuplBankId { get; set; }
    public string? SuplBankAcct { get; set; }
    public string? SuplAcctName { get; set; }
    public string? TicketBe { get; set; }
    public string? CheckTitle { get; set; }
    public string? OrgId { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 批次刪除廠商 DTO
/// </summary>
public class BatchDeleteVendorDto
{
    public List<string> VendorIds { get; set; } = new();
}

/// <summary>
/// 檢查統一編號結果 DTO
/// </summary>
public class CheckGuiIdResultDto
{
    public bool Exists { get; set; }
    public string? VendorId { get; set; }
}

