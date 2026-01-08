namespace ErpCore.Domain.Entities.TaxAccounting;

/// <summary>
/// 暫存傳票主檔實體 (SYSTA00-SYSTA70)
/// </summary>
public class TmpVoucherM
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 傳票編號
    /// </summary>
    public string? VoucherId { get; set; }

    /// <summary>
    /// 傳票日期
    /// </summary>
    public DateTime? VoucherDate { get; set; }

    /// <summary>
    /// 傳票類型 (如SYSTA10, SYSTA30, SYSTA40等)
    /// </summary>
    public string? TypeId { get; set; }

    /// <summary>
    /// 系統代號 (如SYSA000, SYSV000, SYSH000等)
    /// </summary>
    public string? SysId { get; set; }

    /// <summary>
    /// 傳票狀態 (1:未審核, 2:已審核, 3:已拋轉)
    /// </summary>
    public string Status { get; set; } = "1";

    /// <summary>
    /// 拋轉標記 (0:未拋轉, 1:已拋轉)
    /// </summary>
    public string? UpFlag { get; set; } = "0";

    /// <summary>
    /// 傳票備註
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// 廠商代號
    /// </summary>
    public string? VendorId { get; set; }

    /// <summary>
    /// 專櫃代號
    /// </summary>
    public string? StoreId { get; set; }

    /// <summary>
    /// 公司別/分店代號
    /// </summary>
    public string? SiteId { get; set; }

    /// <summary>
    /// 單據別
    /// </summary>
    public string? SlipType { get; set; }

    /// <summary>
    /// 單據編號
    /// </summary>
    public string? SlipNo { get; set; }

    /// <summary>
    /// 傳送標記
    /// </summary>
    public string? SendFlag { get; set; } = "0";

    /// <summary>
    /// 程式代號
    /// </summary>
    public string? ProgId { get; set; }

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

    /// <summary>
    /// 建立者等級
    /// </summary>
    public int? CreatedPriority { get; set; }

    /// <summary>
    /// 建立者群組
    /// </summary>
    public string? CreatedGroup { get; set; }
}

