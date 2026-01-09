namespace ErpCore.Domain.Entities.MirModule;

/// <summary>
/// MIRH000 薪資主檔
/// </summary>
public class MirH000Salary
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 薪資編號
    /// </summary>
    public string SalaryId { get; set; } = string.Empty;

    /// <summary>
    /// 人事編號
    /// </summary>
    public string PersonnelId { get; set; } = string.Empty;

    /// <summary>
    /// 薪資月份 (YYYYMM)
    /// </summary>
    public string SalaryMonth { get; set; } = string.Empty;

    /// <summary>
    /// 基本薪資
    /// </summary>
    public decimal BaseSalary { get; set; }

    /// <summary>
    /// 獎金
    /// </summary>
    public decimal Bonus { get; set; }

    /// <summary>
    /// 總薪資
    /// </summary>
    public decimal TotalSalary { get; set; }

    /// <summary>
    /// 狀態
    /// </summary>
    public string Status { get; set; } = "A";

    /// <summary>
    /// 建立人員
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新人員
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

