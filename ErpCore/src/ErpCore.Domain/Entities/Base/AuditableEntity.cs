namespace ErpCore.Domain.Entities.Base;

/// <summary>
/// 可審計實體類別
/// 繼承 BaseEntity，額外提供審計追蹤屬性：CreatedBy, UpdatedBy
/// </summary>
/// <typeparam name="TKey">主鍵類型</typeparam>
public abstract class AuditableEntity<TKey> : BaseEntity<TKey>
{
    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 更新者
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 建立者等級
    /// </summary>
    public int? CreatedPriority { get; set; }

    /// <summary>
    /// 建立者群組
    /// </summary>
    public string? CreatedGroup { get; set; }
}

/// <summary>
/// 可審計實體類別（使用 long 作為主鍵）
/// </summary>
public abstract class AuditableEntity : AuditableEntity<long>
{
}

