namespace ErpCore.Domain.Entities.StoreMember;

/// <summary>
/// 會員實體 (SYS3000 - 會員資料維護)
/// </summary>
public class Member
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
    /// 會員英文姓名
    /// </summary>
    public string? MemberNameEn { get; set; }

    /// <summary>
    /// 性別 (M:男, F:女)
    /// </summary>
    public string? Gender { get; set; }

    /// <summary>
    /// 生日
    /// </summary>
    public DateTime? BirthDate { get; set; }

    /// <summary>
    /// 身份證字號
    /// </summary>
    public string? PersonalId { get; set; }

    /// <summary>
    /// 電話
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 手機
    /// </summary>
    public string? Mobile { get; set; }

    /// <summary>
    /// 電子郵件
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// 區域
    /// </summary>
    public string? Zone { get; set; }

    /// <summary>
    /// 郵遞區號
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// 會員等級
    /// </summary>
    public string? MemberLevel { get; set; }

    /// <summary>
    /// 積分
    /// </summary>
    public decimal Points { get; set; }

    /// <summary>
    /// 累計積分
    /// </summary>
    public decimal TotalPoints { get; set; }

    /// <summary>
    /// 卡片號碼
    /// </summary>
    public string? CardNo { get; set; }

    /// <summary>
    /// 卡片類型
    /// </summary>
    public string? CardType { get; set; }

    /// <summary>
    /// 加入日期
    /// </summary>
    public DateTime? JoinDate { get; set; }

    /// <summary>
    /// 到期日期
    /// </summary>
    public DateTime? ExpireDate { get; set; }

    /// <summary>
    /// 狀態 (A:啟用, I:停用, S:暫停)
    /// </summary>
    public string Status { get; set; } = "A";

    /// <summary>
    /// 照片路徑
    /// </summary>
    public string? PhotoPath { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Notes { get; set; }

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

/// <summary>
/// 會員等級實體
/// </summary>
public class MemberLevel
{
    /// <summary>
    /// 等級編號
    /// </summary>
    public string LevelId { get; set; } = string.Empty;

    /// <summary>
    /// 等級名稱
    /// </summary>
    public string LevelName { get; set; } = string.Empty;

    /// <summary>
    /// 等級英文名稱
    /// </summary>
    public string? LevelNameEn { get; set; }

    /// <summary>
    /// 最低積分
    /// </summary>
    public decimal MinPoints { get; set; }

    /// <summary>
    /// 最高積分
    /// </summary>
    public decimal? MaxPoints { get; set; }

    /// <summary>
    /// 折扣率
    /// </summary>
    public decimal DiscountRate { get; set; }

    /// <summary>
    /// 說明
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 狀態
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

/// <summary>
/// 會員卡片實體
/// </summary>
public class MemberCard
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 卡片編號
    /// </summary>
    public string CardId { get; set; } = string.Empty;

    /// <summary>
    /// 會員編號
    /// </summary>
    public string MemberId { get; set; } = string.Empty;

    /// <summary>
    /// 卡片號碼
    /// </summary>
    public string CardNo { get; set; } = string.Empty;

    /// <summary>
    /// 卡片類型
    /// </summary>
    public string? CardType { get; set; }

    /// <summary>
    /// 發卡日期
    /// </summary>
    public DateTime? IssueDate { get; set; }

    /// <summary>
    /// 到期日期
    /// </summary>
    public DateTime? ExpireDate { get; set; }

    /// <summary>
    /// 狀態 (A:啟用, I:停用, L:遺失)
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

/// <summary>
/// 會員積分記錄實體
/// </summary>
public class MemberPoint
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
    /// 交易日期
    /// </summary>
    public DateTime TransactionDate { get; set; }

    /// <summary>
    /// 交易類型 (EARN:獲得, USE:使用, EXPIRE:過期, ADJUST:調整)
    /// </summary>
    public string TransactionType { get; set; } = string.Empty;

    /// <summary>
    /// 積分 (正數為獲得，負數為使用)
    /// </summary>
    public decimal Points { get; set; }

    /// <summary>
    /// 餘額
    /// </summary>
    public decimal Balance { get; set; }

    /// <summary>
    /// 參考單號
    /// </summary>
    public string? ReferenceNo { get; set; }

    /// <summary>
    /// 說明
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

