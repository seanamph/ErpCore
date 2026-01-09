namespace ErpCore.Domain.Entities.SystemExtensionE;

/// <summary>
/// 員工主檔 (SYSPE10-SYSPE11 - 員工資料維護)
/// </summary>
public class Employee
{
    /// <summary>
    /// 員工編號
    /// </summary>
    public string EmployeeId { get; set; } = string.Empty;

    /// <summary>
    /// 員工姓名
    /// </summary>
    public string EmployeeName { get; set; } = string.Empty;

    /// <summary>
    /// 身份證字號
    /// </summary>
    public string? IdNumber { get; set; }

    /// <summary>
    /// 部門代碼
    /// </summary>
    public string? DepartmentId { get; set; }

    /// <summary>
    /// 職位代碼
    /// </summary>
    public string? PositionId { get; set; }

    /// <summary>
    /// 到職日期
    /// </summary>
    public DateTime? HireDate { get; set; }

    /// <summary>
    /// 離職日期
    /// </summary>
    public DateTime? ResignDate { get; set; }

    /// <summary>
    /// 狀態 (A:在職, I:離職, L:留停)
    /// </summary>
    public string Status { get; set; } = "A";

    /// <summary>
    /// 電子郵件
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 電話
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// 生日
    /// </summary>
    public DateTime? BirthDate { get; set; }

    /// <summary>
    /// 性別 (M:男, F:女)
    /// </summary>
    public string? Gender { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Notes { get; set; }

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

