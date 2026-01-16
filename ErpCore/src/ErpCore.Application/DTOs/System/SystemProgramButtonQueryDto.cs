namespace ErpCore.Application.DTOs.System;

/// <summary>
/// 系統作業與功能列表查詢 DTO (SYS0810)
/// </summary>
public class SystemProgramButtonQueryDto
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
    public List<SystemProgramButtonQueryItemDto> Programs { get; set; } = new();
}

/// <summary>
/// 作業與按鈕 DTO
/// </summary>
public class SystemProgramButtonQueryItemDto
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
    /// 作業型態
    /// </summary>
    public string? ProgramType { get; set; }

    /// <summary>
    /// 排序序號
    /// </summary>
    public int? SeqNo { get; set; }

    /// <summary>
    /// 功能按鈕列表
    /// </summary>
    public List<ButtonInfoDto> Buttons { get; set; } = new();
}

/// <summary>
/// 按鈕資訊 DTO
/// </summary>
public class ButtonInfoDto
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
    /// 按鈕型態
    /// </summary>
    public string? ButtonType { get; set; }

    /// <summary>
    /// 排序序號
    /// </summary>
    public int? SeqNo { get; set; }
}

/// <summary>
/// 匯出系統作業與功能列表請求 DTO
/// </summary>
public class ExportSystemProgramButtonRequestDto
{
    /// <summary>
    /// 系統代碼
    /// </summary>
    public string SystemId { get; set; } = string.Empty;

    /// <summary>
    /// 匯出格式 (Excel/PDF)
    /// </summary>
    public string ExportFormat { get; set; } = "Excel";
}
