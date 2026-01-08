namespace ErpCore.Domain.Entities.System;

/// <summary>
/// 異動記錄實體 (SYS0610)
/// </summary>
public class ChangeLog
{
    /// <summary>
    /// 異動記錄編號
    /// </summary>
    public long LogId { get; set; }

    /// <summary>
    /// 程式代碼
    /// </summary>
    public string ProgramId { get; set; } = string.Empty;

    /// <summary>
    /// 異動使用者代碼
    /// </summary>
    public string? ChangeUserId { get; set; }

    /// <summary>
    /// 異動時間
    /// </summary>
    public DateTime ChangeDate { get; set; }

    /// <summary>
    /// 異動狀態 (1=新增, 2=刪除, 3=修改)
    /// </summary>
    public string ChangeStatus { get; set; } = string.Empty;

    /// <summary>
    /// 異動欄位名稱 (多個欄位以逗號分隔)
    /// </summary>
    public string? ChangeField { get; set; }

    /// <summary>
    /// 異動前的值 (多個值以逗號分隔)
    /// </summary>
    public string? OldValue { get; set; }

    /// <summary>
    /// 異動後的值 (多個值以逗號分隔)
    /// </summary>
    public string? NewValue { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

