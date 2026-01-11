using System.ComponentModel.DataAnnotations;

namespace ErpCore.Application.DTOs.System;

/// <summary>
/// 系統作業資料傳輸物件 (SYS0430)
/// </summary>
public class ProgramDto
{
    public long TKey { get; set; }
    public string ProgramId { get; set; } = string.Empty;
    public string ProgramName { get; set; } = string.Empty;
    public int? SeqNo { get; set; }
    public string MenuId { get; set; } = string.Empty;
    public string? MenuName { get; set; }
    public string? ProgramUrl { get; set; }
    public string? ProgramType { get; set; }
    public string? ProgramTypeName { get; set; }
    public string? MaintainUserId { get; set; }
    public string? MaintainUserName { get; set; }
    public string Status { get; set; } = "1";
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 系統作業查詢 DTO
/// </summary>
public class ProgramQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string SortOrder { get; set; } = "ASC";
    public ProgramQueryFilters? Filters { get; set; }
}

/// <summary>
/// 作業查詢篩選條件
/// </summary>
public class ProgramQueryFilters
{
    public string? ProgramId { get; set; }
    public string? ProgramName { get; set; }
    public string? MenuId { get; set; }
    public string? ProgramType { get; set; }
}

/// <summary>
/// 新增系統作業 DTO
/// </summary>
public class CreateProgramDto
{
    [Required(ErrorMessage = "作業代碼為必填")]
    [StringLength(50, ErrorMessage = "作業代碼長度不能超過50")]
    public string ProgramId { get; set; } = string.Empty;

    [Required(ErrorMessage = "作業名稱為必填")]
    [StringLength(100, ErrorMessage = "作業名稱長度不能超過100")]
    public string ProgramName { get; set; } = string.Empty;

    [Required(ErrorMessage = "排序序號為必填")]
    public int SeqNo { get; set; }

    [Required(ErrorMessage = "子系統項目代碼為必填")]
    [StringLength(50, ErrorMessage = "子系統項目代碼長度不能超過50")]
    public string MenuId { get; set; } = string.Empty;

    [Required(ErrorMessage = "網頁位址為必填")]
    [StringLength(500, ErrorMessage = "網頁位址長度不能超過500")]
    public string ProgramUrl { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "作業型態長度不能超過20")]
    public string? ProgramType { get; set; }
}

/// <summary>
/// 修改系統作業 DTO
/// </summary>
public class UpdateProgramDto
{
    [Required(ErrorMessage = "作業名稱為必填")]
    [StringLength(100, ErrorMessage = "作業名稱長度不能超過100")]
    public string ProgramName { get; set; } = string.Empty;

    [Required(ErrorMessage = "排序序號為必填")]
    public int SeqNo { get; set; }

    [Required(ErrorMessage = "子系統項目代碼為必填")]
    [StringLength(50, ErrorMessage = "子系統項目代碼長度不能超過50")]
    public string MenuId { get; set; } = string.Empty;

    [Required(ErrorMessage = "網頁位址為必填")]
    [StringLength(500, ErrorMessage = "網頁位址長度不能超過500")]
    public string ProgramUrl { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "作業型態長度不能超過20")]
    public string? ProgramType { get; set; }
}

/// <summary>
/// 批次刪除 DTO
/// </summary>
public class BatchDeleteProgramsDto
{
    public List<string> ProgramIds { get; set; } = new();
}
