namespace ErpCore.Application.DTOs.System;

/// <summary>
/// 使用者異常登入記錄 DTO (SYS0760)
/// </summary>
public class LoginLogDto
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 異常事件代碼
    /// </summary>
    public string EventId { get; set; } = string.Empty;

    /// <summary>
    /// 異常事件代碼名稱
    /// </summary>
    public string? EventIdName { get; set; }

    /// <summary>
    /// 使用者代碼
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// 使用者名稱
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 登入IP位址
    /// </summary>
    public string? LoginIp { get; set; }

    /// <summary>
    /// 事件發生時間
    /// </summary>
    public DateTime EventTime { get; set; }

    /// <summary>
    /// 建立使用者
    /// </summary>
    public string? BUser { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime? BTime { get; set; }

    /// <summary>
    /// 異動使用者
    /// </summary>
    public string? CUser { get; set; }

    /// <summary>
    /// 異動時間
    /// </summary>
    public DateTime? CTime { get; set; }

    /// <summary>
    /// 建立優先權
    /// </summary>
    public int? CPriority { get; set; }

    /// <summary>
    /// 建立群組
    /// </summary>
    public string? CGroup { get; set; }
}

/// <summary>
/// 使用者異常登入記錄查詢 DTO (SYS0760)
/// </summary>
public class LoginLogQueryDto
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
    /// 排序欄位1
    /// </summary>
    public string? SortBy1 { get; set; }

    /// <summary>
    /// 排序方向1
    /// </summary>
    public string SortOrder1 { get; set; } = "ASC";

    /// <summary>
    /// 排序欄位2
    /// </summary>
    public string? SortBy2 { get; set; }

    /// <summary>
    /// 排序方向2
    /// </summary>
    public string SortOrder2 { get; set; } = "ASC";

    /// <summary>
    /// 排序欄位3
    /// </summary>
    public string? SortBy3 { get; set; }

    /// <summary>
    /// 排序方向3
    /// </summary>
    public string SortOrder3 { get; set; } = "ASC";

    /// <summary>
    /// 異常事件代碼列表
    /// </summary>
    public List<string>? EventIds { get; set; }

    /// <summary>
    /// 使用者代碼
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// 事件時間起
    /// </summary>
    public DateTime? EventTimeFrom { get; set; }

    /// <summary>
    /// 事件時間迄
    /// </summary>
    public DateTime? EventTimeTo { get; set; }
}

/// <summary>
/// 異常事件代碼選項 DTO (SYS0760)
/// </summary>
public class EventTypeDto
{
    /// <summary>
    /// 事件代碼
    /// </summary>
    public string Tag { get; set; } = string.Empty;

    /// <summary>
    /// 事件說明
    /// </summary>
    public string Content { get; set; } = string.Empty;
}

/// <summary>
/// 刪除異常登入記錄請求 DTO (SYS0760)
/// </summary>
public class DeleteLoginLogRequestDto
{
    /// <summary>
    /// 要刪除的記錄主鍵列表
    /// </summary>
    public List<long> TKeys { get; set; } = new();
}

/// <summary>
/// 刪除結果 DTO (SYS0760)
/// </summary>
public class DeleteResultDto
{
    /// <summary>
    /// 刪除筆數
    /// </summary>
    public int DeletedCount { get; set; }
}

/// <summary>
/// 異常登入報表請求 DTO (SYS0760)
/// </summary>
public class LoginLogReportDto
{
    /// <summary>
    /// 異常事件代碼列表
    /// </summary>
    public List<string>? EventIds { get; set; }

    /// <summary>
    /// 使用者代碼
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// 事件時間起
    /// </summary>
    public DateTime? EventTimeFrom { get; set; }

    /// <summary>
    /// 事件時間迄
    /// </summary>
    public DateTime? EventTimeTo { get; set; }

    /// <summary>
    /// 匯出格式 (PDF/EXCEL)
    /// </summary>
    public string Format { get; set; } = "PDF";
}

/// <summary>
/// 報表結果 DTO (SYS0760)
/// </summary>
public class ReportResultDto
{
    /// <summary>
    /// 報表下載URL
    /// </summary>
    public string ReportUrl { get; set; } = string.Empty;

    /// <summary>
    /// 檔案名稱
    /// </summary>
    public string FileName { get; set; } = string.Empty;
}
