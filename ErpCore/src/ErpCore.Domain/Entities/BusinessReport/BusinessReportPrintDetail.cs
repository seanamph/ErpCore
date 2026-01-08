namespace ErpCore.Domain.Entities.BusinessReport;

/// <summary>
/// 業務報表列印明細資料實體 (SYSL160)
/// </summary>
public class BusinessReportPrintDetail
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 報表列印ID (外鍵至BusinessReportPrint)
    /// </summary>
    public long PrintId { get; set; }

    /// <summary>
    /// 請假代碼
    /// </summary>
    public string? LeaveId { get; set; }

    /// <summary>
    /// 請假名稱
    /// </summary>
    public string? LeaveName { get; set; }

    /// <summary>
    /// 動作事件
    /// </summary>
    public string? ActEvent { get; set; }

    /// <summary>
    /// 扣款數量
    /// </summary>
    public decimal? DeductionQty { get; set; }

    /// <summary>
    /// 扣款數量預設為空 (Y:是, N:否)
    /// </summary>
    public string? DeductionQtyDefaultEmpty { get; set; }

    /// <summary>
    /// 狀態
    /// </summary>
    public string Status { get; set; } = "1";

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新者
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

