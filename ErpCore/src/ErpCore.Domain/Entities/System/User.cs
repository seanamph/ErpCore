namespace ErpCore.Domain.Entities.System;

/// <summary>
/// 使用者基本資料實體 (SYS0110)
/// </summary>
public class User
{
    /// <summary>
    /// 使用者編號
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 使用者名稱
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 使用者密碼 (加密儲存)
    /// </summary>
    public string? UserPassword { get; set; }

    /// <summary>
    /// 職稱
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// 組織代碼
    /// </summary>
    public string? OrgId { get; set; }

    /// <summary>
    /// 帳號有效起始日
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// 帳號終止日
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// 上次登入時間
    /// </summary>
    public DateTime? LastLoginDate { get; set; }

    /// <summary>
    /// 上次登入IP
    /// </summary>
    public string? LastLoginIp { get; set; }

    /// <summary>
    /// 帳號狀態 (A:啟用, I:停用, L:鎖定)
    /// </summary>
    public string Status { get; set; } = "A";

    /// <summary>
    /// 使用者型態
    /// </summary>
    public string? UserType { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// 使用者等級
    /// </summary>
    public int? UserPriority { get; set; }

    /// <summary>
    /// 所屬分店
    /// </summary>
    public string? ShopId { get; set; }

    /// <summary>
    /// 登入次數
    /// </summary>
    public int? LoginCount { get; set; }

    /// <summary>
    /// 密碼變更日期
    /// </summary>
    public DateTime? ChangePwdDate { get; set; }

    /// <summary>
    /// 樓層代碼
    /// </summary>
    public string? FloorId { get; set; }

    /// <summary>
    /// 區域別代碼
    /// </summary>
    public string? AreaId { get; set; }

    /// <summary>
    /// 業種代碼
    /// </summary>
    public string? BtypeId { get; set; }

    /// <summary>
    /// 專櫃代碼
    /// </summary>
    public string? StoreId { get; set; }

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

