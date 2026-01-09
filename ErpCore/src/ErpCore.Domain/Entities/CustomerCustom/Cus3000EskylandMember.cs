namespace ErpCore.Domain.Entities.CustomerCustom;

/// <summary>
/// CUS3000.ESKYLAND 會員實體
/// ESKYLAND客戶定制版本，功能類似CUS3000但針對ESKYLAND客戶場景優化
/// </summary>
public class Cus3000EskylandMember
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 會員編號
    /// </summary>
    public string MemberId { get; set; } = string.Empty;

    /// <summary>
    /// 會員姓名
    /// </summary>
    public string MemberName { get; set; } = string.Empty;

    /// <summary>
    /// 會員卡號
    /// </summary>
    public string? CardNo { get; set; }

    /// <summary>
    /// ESKYLAND特定欄位
    /// </summary>
    public string? EskylandSpecificField { get; set; }

    /// <summary>
    /// 電話
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 電子郵件
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";

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

