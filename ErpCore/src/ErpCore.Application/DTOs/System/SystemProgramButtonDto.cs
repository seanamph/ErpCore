namespace ErpCore.Application.DTOs.System;

/// <summary>
/// 系統作業與功能列表回應 DTO (SYS0810, SYS0780, SYS0999)
/// </summary>
public class SystemProgramButtonResponseDto
{
    /// <summary>
    /// 系統代碼
    /// </summary>
    public string SystemId { get; set; } = string.Empty;

    /// <summary>
    /// 系統名稱
    /// </summary>
    public string SystemName { get; set; } = string.Empty;

    /// <summary>
    /// 作業列表
    /// </summary>
    public List<ProgramButtonDto> Programs { get; set; } = new();
}

/// <summary>
/// 作業與功能 DTO (SYS0810, SYS0780, SYS0999)
/// </summary>
public class ProgramButtonDto
{
    /// <summary>
    /// 作業代碼
    /// </summary>
    public string ProgramId { get; set; } = string.Empty;

    /// <summary>
    /// 作業名稱
    /// </summary>
    public string ProgramName { get; set; } = string.Empty;

    /// <summary>
    /// 作業類型
    /// </summary>
    public string? ProgramType { get; set; }

    /// <summary>
    /// 排序序號
    /// </summary>
    public int SeqNo { get; set; }

    /// <summary>
    /// 功能按鈕列表
    /// </summary>
    public List<ProgramButtonItemDto> Buttons { get; set; } = new();
}

/// <summary>
/// 功能按鈕 DTO (SYS0810, SYS0780, SYS0999)
/// </summary>
public class ProgramButtonItemDto
{
    /// <summary>
    /// 按鈕代碼
    /// </summary>
    public string ButtonId { get; set; } = string.Empty;

    /// <summary>
    /// 按鈕名稱
    /// </summary>
    public string ButtonName { get; set; } = string.Empty;

    /// <summary>
    /// 按鈕類型
    /// </summary>
    public string? ButtonType { get; set; }

    /// <summary>
    /// 頁面代碼
    /// </summary>
    public string? PageId { get; set; }

    /// <summary>
    /// 排序序號
    /// </summary>
    public int SeqNo { get; set; }
}

