namespace ErpCore.Domain.Entities.StandardModule;

/// <summary>
/// STD5000 會員實體 (SYS5210-SYS52A0 - 會員管理)
/// </summary>
public class Std5000Member
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
    /// 會員類型
    /// </summary>
    public string? MemberType { get; set; }

    /// <summary>
    /// 身分證字號
    /// </summary>
    public string? IdCard { get; set; }

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
    /// 生日
    /// </summary>
    public DateTime? BirthDate { get; set; }

    /// <summary>
    /// 加入日期
    /// </summary>
    public DateTime JoinDate { get; set; }

    /// <summary>
    /// 積分
    /// </summary>
    public decimal Points { get; set; }

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";

    /// <summary>
    /// 店別代碼
    /// </summary>
    public string? ShopId { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Memo { get; set; }

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

