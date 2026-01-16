namespace ErpCore.Application.DTOs.BasicData;

/// <summary>
/// 銀行 DTO
/// </summary>
public class BankDto
{
    public string BankId { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public int? AcctLen { get; set; }
    public int? AcctLenMax { get; set; }
    public string Status { get; set; } = "1";
    public string? BankKind { get; set; }
    public int? SeqNo { get; set; }
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 銀行查詢 DTO
/// </summary>
public class BankQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? BankId { get; set; }
    public string? BankName { get; set; }
    public string? Status { get; set; }
    public string? BankKind { get; set; }
}

/// <summary>
/// 新增銀行 DTO
/// </summary>
public class CreateBankDto
{
    public string BankId { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public int? AcctLen { get; set; }
    public int? AcctLenMax { get; set; }
    public string Status { get; set; } = "1";
    public string? BankKind { get; set; }
    public int? SeqNo { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 修改銀行 DTO
/// </summary>
public class UpdateBankDto
{
    public string BankName { get; set; } = string.Empty;
    public int? AcctLen { get; set; }
    public int? AcctLenMax { get; set; }
    public string Status { get; set; } = "1";
    public string? BankKind { get; set; }
    public int? SeqNo { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 批次刪除銀行 DTO
/// </summary>
public class BatchDeleteBankDto
{
    public List<string> BankIds { get; set; } = new();
}
