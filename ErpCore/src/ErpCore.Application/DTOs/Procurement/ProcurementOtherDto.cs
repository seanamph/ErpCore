namespace ErpCore.Application.DTOs.Procurement;

/// <summary>
/// 採購其他功能 DTO (SYSP510-SYSP530)
/// </summary>
public class ProcurementOtherDto
{
    public long TKey { get; set; }
    public string FunctionId { get; set; } = string.Empty;
    public string FunctionName { get; set; } = string.Empty;
    public string? FunctionType { get; set; }
    public string? FunctionDesc { get; set; }
    public string? FunctionConfig { get; set; }
    public string Status { get; set; } = "A";
    public int SeqNo { get; set; }
    public string? Memo { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 建立採購其他功能 DTO
/// </summary>
public class CreateProcurementOtherDto
{
    public string FunctionId { get; set; } = string.Empty;
    public string FunctionName { get; set; } = string.Empty;
    public string? FunctionType { get; set; }
    public string? FunctionDesc { get; set; }
    public string? FunctionConfig { get; set; }
    public string Status { get; set; } = "A";
    public int SeqNo { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 修改採購其他功能 DTO
/// </summary>
public class UpdateProcurementOtherDto
{
    public string FunctionName { get; set; } = string.Empty;
    public string? FunctionType { get; set; }
    public string? FunctionDesc { get; set; }
    public string? FunctionConfig { get; set; }
    public string Status { get; set; } = "A";
    public int SeqNo { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 查詢採購其他功能 DTO
/// </summary>
public class ProcurementOtherQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? FunctionId { get; set; }
    public string? FunctionName { get; set; }
    public string? FunctionType { get; set; }
    public string? Status { get; set; }
}

