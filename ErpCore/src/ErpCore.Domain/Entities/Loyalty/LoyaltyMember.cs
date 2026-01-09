namespace ErpCore.Domain.Entities.Loyalty;

/// <summary>
/// 忠誠度會員主檔 (LPS - 忠誠度系統維護)
/// </summary>
public class LoyaltyMember
{
    /// <summary>
    /// 會員卡號
    /// </summary>
    public string CardNo { get; set; } = string.Empty;

    /// <summary>
    /// 會員姓名
    /// </summary>
    public string? MemberName { get; set; }

    /// <summary>
    /// 電話
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 電子郵件
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 總點數
    /// </summary>
    public decimal TotalPoints { get; set; }

    /// <summary>
    /// 可用點數
    /// </summary>
    public decimal AvailablePoints { get; set; }

    /// <summary>
    /// 到期日
    /// </summary>
    public string? ExpDate { get; set; }

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";

    /// <summary>
    /// 建立人員
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新人員
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

