namespace ErpCore.Domain.Entities.System;

/// <summary>
/// 使用者權限代理實體
/// </summary>
public class UserAgent
{
    /// <summary>
    /// 代理編號
    /// </summary>
    public Guid AgentId { get; set; }

    /// <summary>
    /// 委託人
    /// </summary>
    public string PrincipalUserId { get; set; } = string.Empty;

    /// <summary>
    /// 代理人
    /// </summary>
    public string AgentUserId { get; set; } = string.Empty;

    /// <summary>
    /// 開始時間
    /// </summary>
    public DateTime BeginTime { get; set; }

    /// <summary>
    /// 結束時間
    /// </summary>
    public DateTime EndTime { get; set; }

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";

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

