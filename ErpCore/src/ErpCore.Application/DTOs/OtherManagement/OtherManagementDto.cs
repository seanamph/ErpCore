namespace ErpCore.Application.DTOs.OtherManagement;

// =============================================
// SYSSFunction DTOs - S系統功能維護 (SYSS000)
// =============================================

/// <summary>
/// S系統功能 DTO
/// </summary>
public class SYSSFunctionDto
{
    public long TKey { get; set; }
    public string FunctionId { get; set; } = string.Empty;
    public string FunctionName { get; set; } = string.Empty;
    public string? FunctionType { get; set; }
    public string? FunctionValue { get; set; }
    public string? FunctionConfig { get; set; }
    public int? SeqNo { get; set; }
    public string Status { get; set; } = "1";
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int? CreatedPriority { get; set; }
    public string? CreatedGroup { get; set; }
}

/// <summary>
/// S系統功能查詢 DTO
/// </summary>
public class SYSSFunctionQueryDto
{
    public string? FunctionId { get; set; }
    public string? FunctionName { get; set; }
    public string? FunctionType { get; set; }
    public string? Status { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

/// <summary>
/// 建立S系統功能 DTO
/// </summary>
public class CreateSYSSFunctionDto
{
    public string FunctionId { get; set; } = string.Empty;
    public string FunctionName { get; set; } = string.Empty;
    public string? FunctionType { get; set; }
    public string? FunctionValue { get; set; }
    public string? FunctionConfig { get; set; }
    public int? SeqNo { get; set; }
    public string Status { get; set; } = "1";
    public string? Notes { get; set; }
    public int? CreatedPriority { get; set; }
    public string? CreatedGroup { get; set; }
}

/// <summary>
/// 修改S系統功能 DTO
/// </summary>
public class UpdateSYSSFunctionDto
{
    public string FunctionName { get; set; } = string.Empty;
    public string? FunctionType { get; set; }
    public string? FunctionValue { get; set; }
    public string? FunctionConfig { get; set; }
    public int? SeqNo { get; set; }
    public string Status { get; set; } = "1";
    public string? Notes { get; set; }
    public int? CreatedPriority { get; set; }
    public string? CreatedGroup { get; set; }
}

// =============================================
// SYSUFunction DTOs - U系統功能維護 (SYSU000)
// =============================================

/// <summary>
/// U系統功能 DTO
/// </summary>
public class SYSUFunctionDto
{
    public long TKey { get; set; }
    public string FunctionId { get; set; } = string.Empty;
    public string FunctionName { get; set; } = string.Empty;
    public string? FunctionType { get; set; }
    public string? FunctionCategory { get; set; }
    public string? FunctionValue { get; set; }
    public string? FunctionConfig { get; set; }
    public int? SeqNo { get; set; }
    public string Status { get; set; } = "1";
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int? CreatedPriority { get; set; }
    public string? CreatedGroup { get; set; }
}

/// <summary>
/// U系統功能查詢 DTO
/// </summary>
public class SYSUFunctionQueryDto
{
    public string? FunctionId { get; set; }
    public string? FunctionName { get; set; }
    public string? FunctionType { get; set; }
    public string? FunctionCategory { get; set; }
    public string? Status { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

/// <summary>
/// 建立U系統功能 DTO
/// </summary>
public class CreateSYSUFunctionDto
{
    public string FunctionId { get; set; } = string.Empty;
    public string FunctionName { get; set; } = string.Empty;
    public string? FunctionType { get; set; }
    public string? FunctionCategory { get; set; }
    public string? FunctionValue { get; set; }
    public string? FunctionConfig { get; set; }
    public int? SeqNo { get; set; }
    public string Status { get; set; } = "1";
    public string? Notes { get; set; }
    public int? CreatedPriority { get; set; }
    public string? CreatedGroup { get; set; }
}

/// <summary>
/// 修改U系統功能 DTO
/// </summary>
public class UpdateSYSUFunctionDto
{
    public string FunctionName { get; set; } = string.Empty;
    public string? FunctionType { get; set; }
    public string? FunctionCategory { get; set; }
    public string? FunctionValue { get; set; }
    public string? FunctionConfig { get; set; }
    public int? SeqNo { get; set; }
    public string Status { get; set; } = "1";
    public string? Notes { get; set; }
    public int? CreatedPriority { get; set; }
    public string? CreatedGroup { get; set; }
}

// =============================================
// SYSVFunction DTOs - V系統功能維護 (SYSV000)
// =============================================

/// <summary>
/// V系統功能 DTO
/// </summary>
public class SYSVFunctionDto
{
    public long TKey { get; set; }
    public string FunctionId { get; set; } = string.Empty;
    public string FunctionName { get; set; } = string.Empty;
    public string? FunctionType { get; set; }
    public string? VoucherType { get; set; }
    public string? FunctionValue { get; set; }
    public string? FunctionConfig { get; set; }
    public int? SeqNo { get; set; }
    public string Status { get; set; } = "1";
    public bool IsCustomerSpecific { get; set; }
    public string? CustomerCode { get; set; }
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int? CreatedPriority { get; set; }
    public string? CreatedGroup { get; set; }
}

/// <summary>
/// V系統功能查詢 DTO
/// </summary>
public class SYSVFunctionQueryDto
{
    public string? FunctionId { get; set; }
    public string? FunctionName { get; set; }
    public string? FunctionType { get; set; }
    public string? VoucherType { get; set; }
    public string? CustomerCode { get; set; }
    public string? Status { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

/// <summary>
/// 建立V系統功能 DTO
/// </summary>
public class CreateSYSVFunctionDto
{
    public string FunctionId { get; set; } = string.Empty;
    public string FunctionName { get; set; } = string.Empty;
    public string? FunctionType { get; set; }
    public string? VoucherType { get; set; }
    public string? FunctionValue { get; set; }
    public string? FunctionConfig { get; set; }
    public int? SeqNo { get; set; }
    public string Status { get; set; } = "1";
    public bool IsCustomerSpecific { get; set; }
    public string? CustomerCode { get; set; }
    public string? Notes { get; set; }
    public int? CreatedPriority { get; set; }
    public string? CreatedGroup { get; set; }
}

/// <summary>
/// 修改V系統功能 DTO
/// </summary>
public class UpdateSYSVFunctionDto
{
    public string FunctionName { get; set; } = string.Empty;
    public string? FunctionType { get; set; }
    public string? VoucherType { get; set; }
    public string? FunctionValue { get; set; }
    public string? FunctionConfig { get; set; }
    public int? SeqNo { get; set; }
    public string Status { get; set; } = "1";
    public bool IsCustomerSpecific { get; set; }
    public string? CustomerCode { get; set; }
    public string? Notes { get; set; }
    public int? CreatedPriority { get; set; }
    public string? CreatedGroup { get; set; }
}

// =============================================
// SYSJFunction DTOs - J系統功能維護 (SYSJ000)
// =============================================

/// <summary>
/// J系統功能 DTO
/// </summary>
public class SYSJFunctionDto
{
    public long TKey { get; set; }
    public string FunctionId { get; set; } = string.Empty;
    public string FunctionName { get; set; } = string.Empty;
    public string? FunctionType { get; set; }
    public string? FunctionValue { get; set; }
    public string? FunctionConfig { get; set; }
    public int? SeqNo { get; set; }
    public string Status { get; set; } = "1";
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int? CreatedPriority { get; set; }
    public string? CreatedGroup { get; set; }
}

/// <summary>
/// J系統功能查詢 DTO
/// </summary>
public class SYSJFunctionQueryDto
{
    public string? FunctionId { get; set; }
    public string? FunctionName { get; set; }
    public string? FunctionType { get; set; }
    public string? Status { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

/// <summary>
/// 建立J系統功能 DTO
/// </summary>
public class CreateSYSJFunctionDto
{
    public string FunctionId { get; set; } = string.Empty;
    public string FunctionName { get; set; } = string.Empty;
    public string? FunctionType { get; set; }
    public string? FunctionValue { get; set; }
    public string? FunctionConfig { get; set; }
    public int? SeqNo { get; set; }
    public string Status { get; set; } = "1";
    public string? Notes { get; set; }
    public int? CreatedPriority { get; set; }
    public string? CreatedGroup { get; set; }
}

/// <summary>
/// 修改J系統功能 DTO
/// </summary>
public class UpdateSYSJFunctionDto
{
    public string FunctionName { get; set; } = string.Empty;
    public string? FunctionType { get; set; }
    public string? FunctionValue { get; set; }
    public string? FunctionConfig { get; set; }
    public int? SeqNo { get; set; }
    public string Status { get; set; } = "1";
    public string? Notes { get; set; }
    public int? CreatedPriority { get; set; }
    public string? CreatedGroup { get; set; }
}

