namespace ErpCore.Domain.Entities.ReportManagement;

/// <summary>
/// 保證金主檔 (SYSR510-SYSR570)
/// </summary>
public class Deposits
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 保證金單號 (DEPOSIT_NO)
    /// </summary>
    public string DepositNo { get; set; } = string.Empty;

    /// <summary>
    /// 保證金日期 (DEPOSIT_DATE)
    /// </summary>
    public DateTime DepositDate { get; set; }

    /// <summary>
    /// 對象別編號 (OBJECT_ID)
    /// </summary>
    public string? ObjectId { get; set; }

    /// <summary>
    /// 保證金金額 (DEPOSIT_AMOUNT)
    /// </summary>
    public decimal DepositAmount { get; set; }

    /// <summary>
    /// 保證金類型 (DEPOSIT_TYPE)
    /// </summary>
    public string? DepositType { get; set; }

    /// <summary>
    /// 保證金狀態 (DEPOSIT_STATUS, A:有效, R:退還, C:取消)
    /// </summary>
    public string DepositStatus { get; set; } = "A";

    /// <summary>
    /// 退還日期 (RETURN_DATE)
    /// </summary>
    public DateTime? ReturnDate { get; set; }

    /// <summary>
    /// 退還金額 (RETURN_AMOUNT)
    /// </summary>
    public decimal ReturnAmount { get; set; }

    /// <summary>
    /// 傳票單號 (VOUCHER_NO)
    /// </summary>
    public string? VoucherNo { get; set; }

    /// <summary>
    /// 傳票主檔KEY值 (VOUCHERM_T_KEY)
    /// </summary>
    public long? VoucherM_TKey { get; set; }

    /// <summary>
    /// 傳票狀態 (VOUCHER_STATUS)
    /// </summary>
    public string? VoucherStatus { get; set; }

    /// <summary>
    /// 票據到期日 (CHECK_DUE_DATE)
    /// </summary>
    public DateTime? CheckDueDate { get; set; }

    /// <summary>
    /// 分店代碼 (SHOP_ID)
    /// </summary>
    public string? ShopId { get; set; }

    /// <summary>
    /// 分公司代碼 (SITE_ID)
    /// </summary>
    public string? SiteId { get; set; }

    /// <summary>
    /// 組織代碼 (ORG_ID)
    /// </summary>
    public string? OrgId { get; set; }

    /// <summary>
    /// 備註 (NOTES)
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

