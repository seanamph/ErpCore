namespace ErpCore.Application.DTOs.Contract;

/// <summary>
/// 合同 DTO (SYSF110-SYSF140)
/// </summary>
public class ContractDto
{
    public long TKey { get; set; }
    public string ContractId { get; set; } = string.Empty;
    public string ContractType { get; set; } = string.Empty;
    public int Version { get; set; } = 1;
    public string VendorId { get; set; } = string.Empty;
    public string? VendorName { get; set; }
    public DateTime? SignDate { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string Status { get; set; } = "D";
    public decimal? TotalAmount { get; set; } = 0;
    public string? CurrencyId { get; set; } = "TWD";
    public decimal? ExchangeRate { get; set; } = 1;
    public string? LocationId { get; set; }
    public string? RecruitId { get; set; }
    public string? Attorney { get; set; }
    public string? Salutation { get; set; }
    public string? VerStatus { get; set; }
    public string? AgmStatus { get; set; }
    public string? Memo { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 建立合同 DTO
/// </summary>
public class CreateContractDto
{
    public string ContractId { get; set; } = string.Empty;
    public string ContractType { get; set; } = string.Empty;
    public int Version { get; set; } = 1;
    public string VendorId { get; set; } = string.Empty;
    public string? VendorName { get; set; }
    public DateTime? SignDate { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string Status { get; set; } = "D";
    public decimal? TotalAmount { get; set; } = 0;
    public string? CurrencyId { get; set; } = "TWD";
    public decimal? ExchangeRate { get; set; } = 1;
    public string? LocationId { get; set; }
    public string? RecruitId { get; set; }
    public string? Attorney { get; set; }
    public string? Salutation { get; set; }
    public string? VerStatus { get; set; }
    public string? AgmStatus { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 修改合同 DTO
/// </summary>
public class UpdateContractDto
{
    public string ContractType { get; set; } = string.Empty;
    public string VendorId { get; set; } = string.Empty;
    public string? VendorName { get; set; }
    public DateTime? SignDate { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string Status { get; set; } = "D";
    public decimal? TotalAmount { get; set; } = 0;
    public string? CurrencyId { get; set; } = "TWD";
    public decimal? ExchangeRate { get; set; } = 1;
    public string? LocationId { get; set; }
    public string? RecruitId { get; set; }
    public string? Attorney { get; set; }
    public string? Salutation { get; set; }
    public string? VerStatus { get; set; }
    public string? AgmStatus { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 查詢合同 DTO
/// </summary>
public class ContractQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ContractId { get; set; }
    public string? ContractType { get; set; }
    public string? VendorId { get; set; }
    public string? Status { get; set; }
    public DateTime? EffectiveDateFrom { get; set; }
    public DateTime? EffectiveDateTo { get; set; }
}

/// <summary>
/// 合同結果 DTO
/// </summary>
public class ContractResultDto
{
    public long TKey { get; set; }
    public string ContractId { get; set; } = string.Empty;
    public int Version { get; set; }
}

/// <summary>
/// 審核合同 DTO
/// </summary>
public class ApproveContractDto
{
    public string ApproveUserId { get; set; } = string.Empty;
    public DateTime? ApproveDate { get; set; }
    public string Status { get; set; } = "A";
}

/// <summary>
/// 新版本 DTO
/// </summary>
public class NewVersionDto
{
    public string VerStatus { get; set; } = "1";
}

// ============================================
// 合同處理 DTO (SYSF210-SYSF220)
// ============================================

/// <summary>
/// 合同處理 DTO
/// </summary>
public class ContractProcessDto
{
    public long TKey { get; set; }
    public string ProcessId { get; set; } = string.Empty;
    public string ContractId { get; set; } = string.Empty;
    public int Version { get; set; }
    public string ProcessType { get; set; } = string.Empty;
    public DateTime ProcessDate { get; set; }
    public decimal? ProcessAmount { get; set; } = 0;
    public string Status { get; set; } = "P";
    public string? ProcessUserId { get; set; }
    public string? ProcessMemo { get; set; }
    public string? SiteId { get; set; }
    public string? OrgId { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 建立合同處理 DTO
/// </summary>
public class CreateContractProcessDto
{
    public string ProcessId { get; set; } = string.Empty;
    public string ContractId { get; set; } = string.Empty;
    public int Version { get; set; }
    public string ProcessType { get; set; } = string.Empty;
    public DateTime ProcessDate { get; set; }
    public decimal? ProcessAmount { get; set; } = 0;
    public string Status { get; set; } = "P";
    public string? ProcessUserId { get; set; }
    public string? ProcessMemo { get; set; }
    public string? SiteId { get; set; }
    public string? OrgId { get; set; }
}

/// <summary>
/// 修改合同處理 DTO
/// </summary>
public class UpdateContractProcessDto
{
    public string ProcessType { get; set; } = string.Empty;
    public DateTime ProcessDate { get; set; }
    public decimal? ProcessAmount { get; set; } = 0;
    public string? ProcessUserId { get; set; }
    public string? ProcessMemo { get; set; }
    public string? SiteId { get; set; }
    public string? OrgId { get; set; }
}

/// <summary>
/// 查詢合同處理 DTO
/// </summary>
public class ContractProcessQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ProcessId { get; set; }
    public string? ContractId { get; set; }
    public int? Version { get; set; }
    public string? ProcessType { get; set; }
    public string? Status { get; set; }
    public DateTime? ProcessDateFrom { get; set; }
    public DateTime? ProcessDateTo { get; set; }
}

/// <summary>
/// 合同處理結果 DTO
/// </summary>
public class ContractProcessResultDto
{
    public long TKey { get; set; }
    public string ProcessId { get; set; } = string.Empty;
}

// ============================================
// 合同擴展 DTO (SYSF350-SYSF540)
// ============================================

/// <summary>
/// 合同擴展 DTO
/// </summary>
public class ContractExtensionDto
{
    public long TKey { get; set; }
    public string ContractId { get; set; } = string.Empty;
    public int Version { get; set; } = 1;
    public string? ExtensionType { get; set; }
    public string? VendorId { get; set; }
    public string? VendorName { get; set; }
    public DateTime? ExtensionDate { get; set; }
    public decimal ExtensionAmount { get; set; } = 0;
    public string Status { get; set; } = "A";
    public string? Memo { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 建立合同擴展 DTO
/// </summary>
public class CreateContractExtensionDto
{
    public string ContractId { get; set; } = string.Empty;
    public int Version { get; set; } = 1;
    public string? ExtensionType { get; set; }
    public string? VendorId { get; set; }
    public string? VendorName { get; set; }
    public DateTime? ExtensionDate { get; set; }
    public decimal ExtensionAmount { get; set; } = 0;
    public string Status { get; set; } = "A";
    public string? Memo { get; set; }
}

/// <summary>
/// 修改合同擴展 DTO
/// </summary>
public class UpdateContractExtensionDto
{
    public string? ExtensionType { get; set; }
    public string? VendorId { get; set; }
    public string? VendorName { get; set; }
    public DateTime? ExtensionDate { get; set; }
    public decimal ExtensionAmount { get; set; } = 0;
    public string Status { get; set; } = "A";
    public string? Memo { get; set; }
}

/// <summary>
/// 查詢合同擴展 DTO
/// </summary>
public class ContractExtensionQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ContractId { get; set; }
    public string? VendorId { get; set; }
    public string? ExtensionType { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 合同擴展結果 DTO
/// </summary>
public class ContractExtensionResultDto
{
    public long TKey { get; set; }
    public string ContractId { get; set; } = string.Empty;
    public int Version { get; set; }
}

/// <summary>
/// 合同傳輸 DTO
/// </summary>
public class ContractTransferDto
{
    public long TKey { get; set; }
    public string ContractId { get; set; } = string.Empty;
    public int Version { get; set; }
    public string? TransferType { get; set; }
    public DateTime TransferDate { get; set; }
    public string? TransferStatus { get; set; }
    public string? TransferResult { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 建立合同傳輸 DTO
/// </summary>
public class CreateContractTransferDto
{
    public string ContractId { get; set; } = string.Empty;
    public int Version { get; set; }
    public string? TransferType { get; set; }
    public DateTime TransferDate { get; set; }
}

/// <summary>
/// 批次刪除 DTO
/// </summary>
public class BatchDeleteDto
{
    public List<long> TKeys { get; set; } = new();
}

/// <summary>
/// 合同複製 DTO
/// </summary>
public class CopyContractDto
{
    public string NewContractId { get; set; } = string.Empty;
    public bool CopyTerms { get; set; } = true;
    public bool CopyExtensions { get; set; } = true;
}

// ============================================
// CMS合同 DTO (CMS2310-CMS2320)
// ============================================

/// <summary>
/// CMS合同 DTO
/// </summary>
public class CmsContractDto
{
    public long TKey { get; set; }
    public string CmsContractId { get; set; } = string.Empty;
    public string ContractType { get; set; } = string.Empty;
    public int Version { get; set; } = 1;
    public string VendorId { get; set; } = string.Empty;
    public string? VendorName { get; set; }
    public DateTime? SignDate { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string Status { get; set; } = "D";
    public decimal TotalAmount { get; set; } = 0;
    public string CurrencyId { get; set; } = "TWD";
    public decimal ExchangeRate { get; set; } = 1;
    public string? LocationId { get; set; }
    public string? Memo { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 建立CMS合同 DTO
/// </summary>
public class CreateCmsContractDto
{
    public string CmsContractId { get; set; } = string.Empty;
    public string ContractType { get; set; } = string.Empty;
    public int Version { get; set; } = 1;
    public string VendorId { get; set; } = string.Empty;
    public string? VendorName { get; set; }
    public DateTime? SignDate { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string Status { get; set; } = "D";
    public decimal TotalAmount { get; set; } = 0;
    public string CurrencyId { get; set; } = "TWD";
    public decimal ExchangeRate { get; set; } = 1;
    public string? LocationId { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 修改CMS合同 DTO
/// </summary>
public class UpdateCmsContractDto
{
    public string ContractType { get; set; } = string.Empty;
    public string VendorId { get; set; } = string.Empty;
    public string? VendorName { get; set; }
    public DateTime? SignDate { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string Status { get; set; } = "D";
    public decimal TotalAmount { get; set; } = 0;
    public string CurrencyId { get; set; } = "TWD";
    public decimal ExchangeRate { get; set; } = 1;
    public string? LocationId { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 查詢CMS合同 DTO
/// </summary>
public class CmsContractQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? CmsContractId { get; set; }
    public string? VendorId { get; set; }
    public string? ContractType { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// CMS合同結果 DTO
/// </summary>
public class CmsContractResultDto
{
    public long TKey { get; set; }
    public string CmsContractId { get; set; } = string.Empty;
    public int Version { get; set; }
}

