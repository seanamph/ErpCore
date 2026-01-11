using System.ComponentModel.DataAnnotations;

namespace ErpCore.Application.DTOs.System;

/// <summary>
/// 系統功能按鈕資料傳輸物件 (SYS0440)
/// </summary>
public class ButtonDto
{
    public long TKey { get; set; }
    public string ProgramId { get; set; } = string.Empty;
    public string? ProgramName { get; set; }
    public string ButtonId { get; set; } = string.Empty;
    public string ButtonName { get; set; } = string.Empty;
    public string? PageId { get; set; }
    public string? ButtonMsg { get; set; }
    public string? ButtonAttr { get; set; }
    public string? ButtonUrl { get; set; }
    public string? MsgType { get; set; }
    public string? MsgTypeName { get; set; }
    public string Status { get; set; } = "1";
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 系統功能按鈕查詢 DTO
/// </summary>
public class ButtonQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string SortOrder { get; set; } = "ASC";
    public ButtonFilterDto? Filters { get; set; }
}

/// <summary>
/// 系統功能按鈕篩選 DTO
/// </summary>
public class ButtonFilterDto
{
    public string? ProgramId { get; set; }
    public string? ButtonId { get; set; }
    public string? ButtonName { get; set; }
    public string? PageId { get; set; }
}

/// <summary>
/// 新增系統功能按鈕 DTO
/// </summary>
public class CreateButtonDto
{
    [Required(ErrorMessage = "作業代碼為必填")]
    [StringLength(50, ErrorMessage = "作業代碼長度不能超過50")]
    public string ProgramId { get; set; } = string.Empty;

    [Required(ErrorMessage = "按鈕代碼為必填")]
    [StringLength(50, ErrorMessage = "按鈕代碼長度不能超過50")]
    public string ButtonId { get; set; } = string.Empty;

    [Required(ErrorMessage = "按鈕名稱為必填")]
    [StringLength(100, ErrorMessage = "按鈕名稱長度不能超過100")]
    public string ButtonName { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "頁面代碼長度不能超過50")]
    public string? PageId { get; set; }

    [StringLength(500, ErrorMessage = "按鈕訊息長度不能超過500")]
    public string? ButtonMsg { get; set; }

    [StringLength(50, ErrorMessage = "按鈕屬性長度不能超過50")]
    public string? ButtonAttr { get; set; }

    [StringLength(500, ErrorMessage = "網頁鏈結位址長度不能超過500")]
    public string? ButtonUrl { get; set; }

    [StringLength(20, ErrorMessage = "訊息型態長度不能超過20")]
    public string? MsgType { get; set; }
}

/// <summary>
/// 修改系統功能按鈕 DTO
/// </summary>
public class UpdateButtonDto
{
    [Required(ErrorMessage = "作業代碼為必填")]
    [StringLength(50, ErrorMessage = "作業代碼長度不能超過50")]
    public string ProgramId { get; set; } = string.Empty;

    [Required(ErrorMessage = "按鈕代碼為必填")]
    [StringLength(50, ErrorMessage = "按鈕代碼長度不能超過50")]
    public string ButtonId { get; set; } = string.Empty;

    [Required(ErrorMessage = "按鈕名稱為必填")]
    [StringLength(100, ErrorMessage = "按鈕名稱長度不能超過100")]
    public string ButtonName { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "頁面代碼長度不能超過50")]
    public string? PageId { get; set; }

    [StringLength(500, ErrorMessage = "按鈕訊息長度不能超過500")]
    public string? ButtonMsg { get; set; }

    [StringLength(50, ErrorMessage = "按鈕屬性長度不能超過50")]
    public string? ButtonAttr { get; set; }

    [StringLength(500, ErrorMessage = "網頁鏈結位址長度不能超過500")]
    public string? ButtonUrl { get; set; }

    [StringLength(20, ErrorMessage = "訊息型態長度不能超過20")]
    public string? MsgType { get; set; }
}

/// <summary>
/// 批次刪除 DTO
/// </summary>
public class BatchDeleteButtonDto
{
    public List<long> TKeys { get; set; } = new();
}
