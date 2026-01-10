namespace ErpCore.Domain.Interfaces;

/// <summary>
/// 儲存庫介面（基礎介面）
/// </summary>
/// <typeparam name="TEntity">實體類型</typeparam>
/// <typeparam name="TKey">主鍵類型</typeparam>
public interface IRepository<TEntity, TKey> where TEntity : class
{
    /// <summary>
    /// 根據主鍵取得實體
    /// </summary>
    Task<TEntity?> GetByIdAsync(TKey id);

    /// <summary>
    /// 取得所有實體
    /// </summary>
    Task<IEnumerable<TEntity>> GetAllAsync();

    /// <summary>
    /// 新增實體
    /// </summary>
    Task<TEntity> AddAsync(TEntity entity);

    /// <summary>
    /// 更新實體
    /// </summary>
    Task UpdateAsync(TEntity entity);

    /// <summary>
    /// 刪除實體
    /// </summary>
    Task DeleteAsync(TKey id);
}

