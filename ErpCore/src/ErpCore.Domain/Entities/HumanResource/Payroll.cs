namespace ErpCore.Domain.Entities.HumanResource;

/// <summary>
/// 薪資資料實體 (SYSH210)
/// </summary>
public class Payroll
{
    /// <summary>
    /// 薪資編號
    /// </summary>
    public string PayrollId { get; set; } = string.Empty;

    /// <summary>
    /// 員工編號
    /// </summary>
    public string EmployeeId { get; set; } = string.Empty;

    /// <summary>
    /// 薪資年度
    /// </summary>
    public int PayrollYear { get; set; }

    /// <summary>
    /// 薪資月份
    /// </summary>
    public int PayrollMonth { get; set; }

    /// <summary>
    /// 基本薪資
    /// </summary>
    public decimal BaseSalary { get; set; }

    /// <summary>
    /// 津貼
    /// </summary>
    public decimal Allowance { get; set; }

    /// <summary>
    /// 獎金
    /// </summary>
    public decimal Bonus { get; set; }

    /// <summary>
    /// 扣款
    /// </summary>
    public decimal Deduction { get; set; }

    /// <summary>
    /// 總薪資
    /// </summary>
    public decimal TotalSalary { get; set; }

    /// <summary>
    /// 狀態 (D:草稿, C:已確認, P:已發放)
    /// </summary>
    public string Status { get; set; } = "D";

    /// <summary>
    /// 發放日期
    /// </summary>
    public DateTime? PayDate { get; set; }

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

