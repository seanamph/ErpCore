namespace ErpCore.Domain.Entities.Contract;

/// <summary>
/// 合同傳輸記錄 (SYSF350-SYSF540)
/// </summary>
public class ContractTransfer
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 合同編號
    /// </summary>
    public string ContractId { get; set; } = string.Empty;

    /// <summary>
    /// 版本號
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// 傳輸類型
    /// </summary>
    public string? TransferType { get; set; }

    /// <summary>
    /// 傳輸日期
    /// </summary>
    public DateTime TransferDate { get; set; }

    /// <summary>
    /// 傳輸狀態
    /// </summary>
    public string? TransferStatus { get; set; }

    /// <summary>
    /// 傳輸結果
    /// </summary>
    public string? TransferResult { get; set; }

    /// <summary>
    /// 建立人員
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

