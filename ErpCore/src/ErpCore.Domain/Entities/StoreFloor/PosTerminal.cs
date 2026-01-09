namespace ErpCore.Domain.Entities.StoreFloor;

/// <summary>
/// POS終端實體 (SYS6610-SYS6999 - POS資料維護)
/// </summary>
public class PosTerminal
{
    /// <summary>
    /// POS終端代碼
    /// </summary>
    public string PosTerminalId { get; set; } = string.Empty;

    /// <summary>
    /// POS系統代碼
    /// </summary>
    public string PosSystemId { get; set; } = string.Empty;

    /// <summary>
    /// 終端代碼
    /// </summary>
    public string TerminalCode { get; set; } = string.Empty;

    /// <summary>
    /// 終端名稱
    /// </summary>
    public string TerminalName { get; set; } = string.Empty;

    /// <summary>
    /// 商店編號
    /// </summary>
    public string? ShopId { get; set; }

    /// <summary>
    /// 樓層代碼
    /// </summary>
    public string? FloorId { get; set; }

    /// <summary>
    /// 終端類型
    /// </summary>
    public string? TerminalType { get; set; }

    /// <summary>
    /// IP位址
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// 連接埠
    /// </summary>
    public int? Port { get; set; }

    /// <summary>
    /// JSON格式的設定
    /// </summary>
    public string? Config { get; set; }

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";

    /// <summary>
    /// 最後同步時間
    /// </summary>
    public DateTime? LastSyncDate { get; set; }

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

