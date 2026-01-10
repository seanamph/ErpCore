namespace ErpCore.Domain.Entities.Base;

/// <summary>
/// 基礎實體類別
/// 提供所有實體共用的基本屬性：Id, CreatedAt, UpdatedAt
/// </summary>
/// <typeparam name="TKey">主鍵類型</typeparam>
public abstract class BaseEntity<TKey>
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public TKey Id { get; set; } = default!;

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

/// <summary>
/// 基礎實體類別（使用 long 作為主鍵）
/// </summary>
public abstract class BaseEntity : BaseEntity<long>
{
}

