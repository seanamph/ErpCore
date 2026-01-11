namespace ErpCore.Domain.Entities.System;

/// <summary>
/// 使用者儲位權限實體 (SYS0111)
/// </summary>
public class UserWarehouseArea
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 使用者編號
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 儲位1
    /// </summary>
    public string? WareaId1 { get; set; }

    /// <summary>
    /// 儲位2
    /// </summary>
    public string? WareaId2 { get; set; }

    /// <summary>
    /// 儲位3
    /// </summary>
    public string? WareaId3 { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
