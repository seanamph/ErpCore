namespace ErpCore.Domain.Entities.Recruitment;

/// <summary>
/// 訪談資料實體 (SYSC222)
/// </summary>
public class Interview
{
    /// <summary>
    /// 訪談ID
    /// </summary>
    public long InterviewId { get; set; }

    /// <summary>
    /// 潛客代碼
    /// </summary>
    public string ProspectId { get; set; } = string.Empty;

    /// <summary>
    /// 訪談日期
    /// </summary>
    public DateTime InterviewDate { get; set; }

    /// <summary>
    /// 訪談時間
    /// </summary>
    public TimeSpan? InterviewTime { get; set; }

    /// <summary>
    /// 訪談類型 (PHONE:電話, FACE_TO_FACE:面對面, ONLINE:線上)
    /// </summary>
    public string? InterviewType { get; set; }

    /// <summary>
    /// 訪談人員
    /// </summary>
    public string? Interviewer { get; set; }

    /// <summary>
    /// 訪談地點
    /// </summary>
    public string? InterviewLocation { get; set; }

    /// <summary>
    /// 訪談內容
    /// </summary>
    public string? InterviewContent { get; set; }

    /// <summary>
    /// 訪談結果 (SUCCESS:成功, FOLLOW_UP:待追蹤, CANCELLED:取消, NO_SHOW:未到)
    /// </summary>
    public string? InterviewResult { get; set; }

    /// <summary>
    /// 後續行動
    /// </summary>
    public string? NextAction { get; set; }

    /// <summary>
    /// 後續行動日期
    /// </summary>
    public DateTime? NextActionDate { get; set; }

    /// <summary>
    /// 追蹤日期
    /// </summary>
    public DateTime? FollowUpDate { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// 狀態 (ACTIVE:有效, CANCELLED:取消)
    /// </summary>
    public string Status { get; set; } = "ACTIVE";

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

    /// <summary>
    /// 建立者等級
    /// </summary>
    public int? CreatedPriority { get; set; }

    /// <summary>
    /// 建立者群組
    /// </summary>
    public string? CreatedGroup { get; set; }
}

