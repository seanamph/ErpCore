namespace ErpCore.Domain.Entities.CustomerCustom;

/// <summary>
/// CUS3000 會員實體 (SYS3130-SYS3160 - 會員管理)
/// </summary>
public class Cus3000Member
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
    /// 出生日期
    /// </summary>
    public DateTime? BirthDate { get; set; }

    /// <summary>
    /// 性別 (M:男, F:女)
    /// </summary>
    public string? Gender { get; set; }

    /// <summary>
    /// 照片路徑
    /// </summary>
    public string? PhotoPath { get; set; }

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

