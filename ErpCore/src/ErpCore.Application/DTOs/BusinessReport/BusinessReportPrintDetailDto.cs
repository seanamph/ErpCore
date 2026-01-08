namespace ErpCore.Application.DTOs.BusinessReport;

/// <summary>
/// 業務報表列印明細 DTO (SYSL160)
/// </summary>
public class BusinessReportPrintDetailDto
{
    public long TKey { get; set; }
    public long PrintId { get; set; }
    public string? LeaveId { get; set; }
    public string? LeaveName { get; set; }
    public string? ActEvent { get; set; }
    public decimal? DeductionQty { get; set; }
    public string? DeductionQtyDefaultEmpty { get; set; }
    public string Status { get; set; } = "1";
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 業務報表列印明細查詢 DTO (SYSL160)
/// </summary>
public class BusinessReportPrintDetailQueryDto
{
    public long? PrintId { get; set; }
    public string? LeaveId { get; set; }
    public string? ActEvent { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// 新增業務報表列印明細 DTO (SYSL160)
/// </summary>
public class CreateBusinessReportPrintDetailDto
{
    public long PrintId { get; set; }
    public string? LeaveId { get; set; }
    public string? LeaveName { get; set; }
    public string? ActEvent { get; set; }
    public decimal? DeductionQty { get; set; }
    public string? DeductionQtyDefaultEmpty { get; set; }
}

/// <summary>
/// 修改業務報表列印明細 DTO (SYSL160)
/// </summary>
public class UpdateBusinessReportPrintDetailDto
{
    public string? LeaveId { get; set; }
    public string? LeaveName { get; set; }
    public string? ActEvent { get; set; }
    public decimal? DeductionQty { get; set; }
    public string? DeductionQtyDefaultEmpty { get; set; }
}

/// <summary>
/// 批次處理業務報表列印明細 DTO (SYSL160)
/// </summary>
public class BatchProcessBusinessReportPrintDetailDto
{
    public List<CreateBusinessReportPrintDetailDto> CreateItems { get; set; } = new();
    public List<BatchUpdateBusinessReportPrintDetailDto> UpdateItems { get; set; } = new();
    public List<long> DeleteTKeys { get; set; } = new();
}

/// <summary>
/// 批次更新業務報表列印明細 DTO (SYSL160)
/// </summary>
public class BatchUpdateBusinessReportPrintDetailDto
{
    public long TKey { get; set; }
    public string? LeaveId { get; set; }
    public string? LeaveName { get; set; }
    public string? ActEvent { get; set; }
    public decimal? DeductionQty { get; set; }
    public string? DeductionQtyDefaultEmpty { get; set; }
}

