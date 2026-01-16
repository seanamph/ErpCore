namespace ErpCore.Application.DTOs.System;

/// <summary>
/// 按鈕操作記錄 DTO (SYS0790)
/// </summary>
public class ButtonLogDto
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 使用者編號
    /// </summary>
    public string BUser { get; set; } = string.Empty;

    /// <summary>
    /// 使用者名稱
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 操作時間
    /// </summary>
    public DateTime BTime { get; set; }

    /// <summary>
    /// 作業代碼
    /// </summary>
    public string? ProgId { get; set; }

    /// <summary>
    /// 作業名稱
    /// </summary>
    public string? ProgName { get; set; }

    /// <summary>
    /// 按鈕名稱
    /// </summary>
    public string? ButtonName { get; set; }

    /// <summary>
    /// 網頁位址
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// 框架名稱
    /// </summary>
    public string? FrameName { get; set; }
}

/// <summary>
/// 按鈕操作記錄查詢 DTO (SYS0790)
/// </summary>
public class ButtonLogQueryDto
{
    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每頁筆數
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// 排序欄位
    /// </summary>
    public string? SortField { get; set; }

    /// <summary>
    /// 排序方向
    /// </summary>
    public string SortOrder { get; set; } = "DESC";

    /// <summary>
    /// 篩選條件
    /// </summary>
    public ButtonLogFilterDto? Filters { get; set; }
}

/// <summary>
/// 按鈕操作記錄篩選 DTO (SYS0790)
/// </summary>
public class ButtonLogFilterDto
{
    /// <summary>
    /// 使用者編號列表
    /// </summary>
    public List<string>? UserIds { get; set; }

    /// <summary>
    /// 作業代碼
    /// </summary>
    public string? ProgId { get; set; }

    /// <summary>
    /// 開始日期
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// 開始時間 (HHmm 格式)
    /// </summary>
    public string? StartTime { get; set; }

    /// <summary>
    /// 結束日期
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// 結束時間 (HHmm 格式)
    /// </summary>
    public string? EndTime { get; set; }
}

/// <summary>
/// 新增按鈕操作記錄 DTO (SYS0790)
/// </summary>
public class CreateButtonLogDto
{
    /// <summary>
    /// 作業代碼
    /// </summary>
    [global::System.ComponentModel.DataAnnotations.StringLength(50, ErrorMessage = "作業代碼長度不能超過50")]
    public string? ProgId { get; set; }

    /// <summary>
    /// 作業名稱
    /// </summary>
    [global::System.ComponentModel.DataAnnotations.StringLength(100, ErrorMessage = "作業名稱長度不能超過100")]
    public string? ProgName { get; set; }

    /// <summary>
    /// 按鈕名稱
    /// </summary>
    [global::System.ComponentModel.DataAnnotations.StringLength(100, ErrorMessage = "按鈕名稱長度不能超過100")]
    public string? ButtonName { get; set; }

    /// <summary>
    /// 網頁位址
    /// </summary>
    [global::System.ComponentModel.DataAnnotations.StringLength(500, ErrorMessage = "網頁位址長度不能超過500")]
    public string? Url { get; set; }

    /// <summary>
    /// 框架名稱
    /// </summary>
    [global::System.ComponentModel.DataAnnotations.StringLength(100, ErrorMessage = "框架名稱長度不能超過100")]
    public string? FrameName { get; set; }
}

/// <summary>
/// 匯出按鈕操作記錄報表請求 DTO (SYS0790)
/// </summary>
public class ButtonLogExportRequestDto
{
    /// <summary>
    /// 查詢條件
    /// </summary>
    public ButtonLogQueryDto Query { get; set; } = new();

    /// <summary>
    /// 匯出格式 (excel/pdf)
    /// </summary>
    public string Format { get; set; } = "excel";
}

