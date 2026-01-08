namespace ErpCore.Application.DTOs.TaxAccounting;

/// <summary>
/// 傳票確認查詢 DTO (SYST311-SYST312)
/// </summary>
public class VoucherConfirmQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? VoucherNoFrom { get; set; }
    public string? VoucherNoTo { get; set; }
    public DateTime? VoucherDateFrom { get; set; }
    public DateTime? VoucherDateTo { get; set; }
    public List<string>? VoucherTypes { get; set; }
    public string? VoucherStatus { get; set; }
    public DateTime? ConfirmDateFrom { get; set; }
    public DateTime? ConfirmDateTo { get; set; }
}

/// <summary>
/// 批次確認傳票 DTO
/// </summary>
public class BatchConfirmVoucherDto
{
    public List<string> VoucherNos { get; set; } = new();
    public DateTime ConfirmDate { get; set; }
}

/// <summary>
/// 批次確認結果 DTO
/// </summary>
public class BatchConfirmResultDto
{
    public int SuccessCount { get; set; }
    public int FailCount { get; set; }
    public List<BatchConfirmFailItem> FailItems { get; set; } = new();
}

/// <summary>
/// 批次確認失敗項目
/// </summary>
public class BatchConfirmFailItem
{
    public string VoucherNo { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// 傳票過帳查詢 DTO (SYST321-SYST322)
/// </summary>
public class VoucherPostingQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? PostingYearMonth { get; set; }
    public DateTime? VoucherDateFrom { get; set; }
    public DateTime? VoucherDateTo { get; set; }
    public List<string>? VoucherTypes { get; set; }
    public string? VoucherStatus { get; set; }
    public bool PostingByDetail { get; set; }
}

/// <summary>
/// 批次過帳傳票 DTO
/// </summary>
public class BatchPostingVoucherDto
{
    public List<string>? VoucherNos { get; set; }
    public string PostingYearMonth { get; set; } = string.Empty;
    public DateTime PostingDate { get; set; }
    public bool PostingByDetail { get; set; }
    public List<long>? DetailTKeys { get; set; }
}

/// <summary>
/// 批次過帳結果 DTO
/// </summary>
public class BatchPostingResultDto
{
    public int SuccessCount { get; set; }
    public int FailCount { get; set; }
    public List<BatchPostingFailItem> FailItems { get; set; } = new();
}

/// <summary>
/// 批次過帳失敗項目
/// </summary>
public class BatchPostingFailItem
{
    public string VoucherNo { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// 傳票狀態統計 DTO
/// </summary>
public class VoucherStatusCountDto
{
    public string PostingYearMonth { get; set; } = string.Empty;
    public int CreateCount { get; set; }
    public int ConfirmCount { get; set; }
    public int PostingCount { get; set; }
    public decimal CreateAmount { get; set; }
    public decimal ConfirmAmount { get; set; }
    public decimal PostingAmount { get; set; }
}

/// <summary>
/// 反過帳資料年結處理查詢 DTO (SYST351-SYST352)
/// </summary>
public class ReverseYearEndQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? Year { get; set; }
}

/// <summary>
/// 反過帳傳票 DTO
/// </summary>
public class ReversePostingDto
{
    public DateTime ReversePostingDate { get; set; }
    public string? Reason { get; set; }
}

