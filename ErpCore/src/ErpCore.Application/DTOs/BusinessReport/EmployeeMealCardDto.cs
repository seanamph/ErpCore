namespace ErpCore.Application.DTOs.BusinessReport;

/// <summary>
/// 員工餐卡申請 DTO (SYSL130)
/// </summary>
public class EmployeeMealCardDto
{
    public long TKey { get; set; }
    public string EmpId { get; set; } = string.Empty;
    public string? EmpName { get; set; }
    public string? OrgId { get; set; }
    public string? SiteId { get; set; }
    public string? CardType { get; set; }
    public string? ActionType { get; set; }
    public string? ActionTypeD { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = "P";
    public string? Verifier { get; set; }
    public DateTime? VerifyDate { get; set; }
    public string? Notes { get; set; }
    public string? TxnNo { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 員工餐卡申請查詢 DTO
/// </summary>
public class EmployeeMealCardQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? EmpId { get; set; }
    public string? EmpName { get; set; }
    public string? OrgId { get; set; }
    public string? SiteId { get; set; }
    public string? CardType { get; set; }
    public string? ActionType { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public DateTime? EndDateFrom { get; set; }
    public DateTime? EndDateTo { get; set; }
}

/// <summary>
/// 新增員工餐卡申請 DTO
/// </summary>
public class CreateEmployeeMealCardDto
{
    public string EmpId { get; set; } = string.Empty;
    public string? EmpName { get; set; }
    public string? OrgId { get; set; }
    public string? SiteId { get; set; }
    public string? CardType { get; set; }
    public string? ActionType { get; set; }
    public string? ActionTypeD { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Notes { get; set; }
    public string? TxnNo { get; set; }
}

/// <summary>
/// 修改員工餐卡申請 DTO
/// </summary>
public class UpdateEmployeeMealCardDto
{
    public string? EmpName { get; set; }
    public string? OrgId { get; set; }
    public string? SiteId { get; set; }
    public string? CardType { get; set; }
    public string? ActionType { get; set; }
    public string? ActionTypeD { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Notes { get; set; }
    public string? TxnNo { get; set; }
}

/// <summary>
/// 批次審核 DTO
/// </summary>
public class BatchVerifyDto
{
    public List<long> TKeys { get; set; } = new();
    public string Action { get; set; } = string.Empty; // approve: 通過, reject: 拒絕
    public string? Notes { get; set; }
}

/// <summary>
/// 批次審核結果 DTO
/// </summary>
public class BatchVerifyResultDto
{
    public int SuccessCount { get; set; }
    public int FailCount { get; set; }
    public List<string> Errors { get; set; } = new();
}

/// <summary>
/// 匯入結果 DTO
/// </summary>
public class ImportResultDto
{
    public int SuccessCount { get; set; }
    public int FailCount { get; set; }
    public List<string> Errors { get; set; } = new();
}

/// <summary>
/// 下拉選單資料 DTO
/// </summary>
public class MealCardDropdownsDto
{
    public List<CardTypeDto> CardTypes { get; set; } = new();
    public List<ActionTypeDto> ActionTypes { get; set; } = new();
    public List<ActionTypeDetailDto> ActionTypeDetails { get; set; } = new();
}

/// <summary>
/// 卡片類型 DTO
/// </summary>
public class CardTypeDto
{
    public string CardId { get; set; } = string.Empty;
    public string CardName { get; set; } = string.Empty;
}

/// <summary>
/// 動作類型 DTO
/// </summary>
public class ActionTypeDto
{
    public string ActionId { get; set; } = string.Empty;
    public string ActionName { get; set; } = string.Empty;
}

/// <summary>
/// 動作類型明細 DTO
/// </summary>
public class ActionTypeDetailDto
{
    public string ActionId { get; set; } = string.Empty;
    public string ActionIdD { get; set; } = string.Empty;
    public string ActionNameD { get; set; } = string.Empty;
}

