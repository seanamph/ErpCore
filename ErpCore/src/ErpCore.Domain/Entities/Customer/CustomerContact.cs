namespace ErpCore.Domain.Entities.Customer;

/// <summary>
/// 客戶聯絡人資料實體
/// </summary>
public class CustomerContact
{
    /// <summary>
    /// 聯絡人ID
    /// </summary>
    public Guid ContactId { get; set; }

    /// <summary>
    /// 客戶編號
    /// </summary>
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// 聯絡人姓名
    /// </summary>
    public string ContactName { get; set; } = string.Empty;

    /// <summary>
    /// 職稱
    /// </summary>
    public string? ContactTitle { get; set; }

    /// <summary>
    /// 電話
    /// </summary>
    public string? ContactTel { get; set; }

    /// <summary>
    /// 手機
    /// </summary>
    public string? ContactCell { get; set; }

    /// <summary>
    /// 電子郵件
    /// </summary>
    public string? ContactEmail { get; set; }

    /// <summary>
    /// 是否為主要聯絡人
    /// </summary>
    public bool IsPrimary { get; set; }

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

