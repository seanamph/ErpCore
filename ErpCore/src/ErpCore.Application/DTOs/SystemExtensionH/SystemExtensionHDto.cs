namespace ErpCore.Application.DTOs.SystemExtensionH;

/// <summary>
/// 人事匯入記錄 DTO (SYSH3D0_FMI - 人事批量新增)
/// </summary>
public class PersonnelImportLogDto
{
    public long TKey { get; set; }
    public string ImportId { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public int TotalCount { get; set; }
    public int SuccessCount { get; set; }
    public int FailCount { get; set; }
    public string ImportStatus { get; set; } = "PENDING";
    public DateTime ImportDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 人事匯入記錄查詢 DTO
/// </summary>
public class PersonnelImportLogQueryDto
{
    public string? ImportId { get; set; }
    public string? ImportStatus { get; set; }
    public DateTime? ImportDateFrom { get; set; }
    public DateTime? ImportDateTo { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// 人事匯入結果 DTO
/// </summary>
public class PersonnelImportResultDto
{
    public string ImportId { get; set; } = string.Empty;
    public int TotalCount { get; set; }
    public int SuccessCount { get; set; }
    public int FailCount { get; set; }
}

/// <summary>
/// 人事匯入進度 DTO
/// </summary>
public class PersonnelImportProgressDto
{
    public string ImportId { get; set; } = string.Empty;
    public string ImportStatus { get; set; } = "PENDING";
    public int TotalCount { get; set; }
    public int ProcessedCount { get; set; }
    public int SuccessCount { get; set; }
    public int FailCount { get; set; }
    public int Progress { get; set; }
}

/// <summary>
/// 員工感應卡 DTO (SYSPH00 - 系統擴展PH)
/// </summary>
public class EmpCardDto
{
    public long TKey { get; set; }
    public string CardNo { get; set; } = string.Empty;
    public string EmpId { get; set; } = string.Empty;
    public DateTime? BeginDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string CardStatus { get; set; } = "1";
    public string? Notes { get; set; }
    public string? BUser { get; set; }
    public DateTime BTime { get; set; }
    public string? CUser { get; set; }
    public DateTime? CTime { get; set; }
    public int? CPriority { get; set; }
    public string? CGroup { get; set; }
}

/// <summary>
/// 員工感應卡查詢 DTO
/// </summary>
public class EmpCardQueryDto
{
    public string? CardNo { get; set; }
    public string? EmpId { get; set; }
    public string? CardStatus { get; set; }
    public DateTime? BeginDateFrom { get; set; }
    public DateTime? BeginDateTo { get; set; }
    public DateTime? EndDateFrom { get; set; }
    public DateTime? EndDateTo { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// 建立員工感應卡 DTO
/// </summary>
public class CreateEmpCardDto
{
    public string CardNo { get; set; } = string.Empty;
    public string EmpId { get; set; } = string.Empty;
    public DateTime? BeginDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string CardStatus { get; set; } = "1";
    public string? Notes { get; set; }
}

/// <summary>
/// 批量建立員工感應卡 DTO
/// </summary>
public class CreateBatchEmpCardDto
{
    public List<CreateEmpCardDto> Items { get; set; } = new();
}

/// <summary>
/// 修改員工感應卡 DTO
/// </summary>
public class UpdateEmpCardDto : CreateEmpCardDto
{
}

