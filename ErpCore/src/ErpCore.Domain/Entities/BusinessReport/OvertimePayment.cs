namespace ErpCore.Domain.Entities.BusinessReport;

/// <summary>
/// 加班發放資料實體 (SYSL510)
/// </summary>
public class OvertimePayment
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 發放單號
    /// </summary>
    public string PaymentNo { get; set; } = string.Empty;

    /// <summary>
    /// 發放日期
    /// </summary>
    public DateTime PaymentDate { get; set; }

    /// <summary>
    /// 員工編號
    /// </summary>
    public string EmployeeId { get; set; } = string.Empty;

    /// <summary>
    /// 員工姓名
    /// </summary>
    public string? EmployeeName { get; set; }

    /// <summary>
    /// 部門編號
    /// </summary>
    public string? DepartmentId { get; set; }

    /// <summary>
    /// 部門名稱
    /// </summary>
    public string? DepartmentName { get; set; }

    /// <summary>
    /// 加班時數
    /// </summary>
    public decimal OvertimeHours { get; set; }

    /// <summary>
    /// 加班金額
    /// </summary>
    public decimal OvertimeAmount { get; set; }

    /// <summary>
    /// 開始日期
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// 結束日期
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// 狀態 (Draft/Submitted/Approved/Rejected)
    /// </summary>
    public string Status { get; set; } = "Draft";

    /// <summary>
    /// 審核者
    /// </summary>
    public string? ApprovedBy { get; set; }

    /// <summary>
    /// 審核者名稱
    /// </summary>
    public string? ApprovedByName { get; set; }

    /// <summary>
    /// 審核時間
    /// </summary>
    public DateTime? ApprovedAt { get; set; }

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

