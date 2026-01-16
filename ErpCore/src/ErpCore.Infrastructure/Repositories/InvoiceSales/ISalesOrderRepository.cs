using ErpCore.Domain.Entities.InvoiceSales;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.InvoiceSales;

/// <summary>
/// 銷售單 Repository 接口 (SYSG410-SYSG460 - 銷售資料維護)
/// </summary>
public interface ISalesOrderRepository
{
    /// <summary>
    /// 根據主鍵查詢銷售單
    /// </summary>
    Task<SalesOrder?> GetByIdAsync(long tKey);

    /// <summary>
    /// 根據銷售單號查詢銷售單
    /// </summary>
    Task<SalesOrder?> GetByOrderIdAsync(string orderId);

    /// <summary>
    /// 查詢銷售單列表
    /// </summary>
    Task<PagedResult<SalesOrder>> QueryAsync(SalesOrderQuery query);

    /// <summary>
    /// 新增銷售單
    /// </summary>
    Task<long> CreateAsync(SalesOrder salesOrder);

    /// <summary>
    /// 修改銷售單
    /// </summary>
    Task<int> UpdateAsync(SalesOrder salesOrder);

    /// <summary>
    /// 刪除銷售單
    /// </summary>
    Task<int> DeleteAsync(long tKey);

    /// <summary>
    /// 檢查銷售單號是否存在
    /// </summary>
    Task<bool> ExistsByOrderIdAsync(string orderId, long? excludeTKey = null);
}

/// <summary>
/// 銷售單明細 Repository 接口
/// </summary>
public interface ISalesOrderDetailRepository
{
    /// <summary>
    /// 根據銷售單號查詢明細列表
    /// </summary>
    Task<List<SalesOrderDetail>> GetByOrderIdAsync(string orderId);

    /// <summary>
    /// 新增銷售單明細
    /// </summary>
    Task<long> CreateAsync(SalesOrderDetail detail);

    /// <summary>
    /// 修改銷售單明細
    /// </summary>
    Task<int> UpdateAsync(SalesOrderDetail detail);

    /// <summary>
    /// 刪除銷售單明細
    /// </summary>
    Task<int> DeleteAsync(long tKey);

    /// <summary>
    /// 根據銷售單號刪除所有明細
    /// </summary>
    Task<int> DeleteByOrderIdAsync(string orderId);
}


