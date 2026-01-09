using ErpCore.Shared.Common;

namespace ErpCore.Application.DTOs.StandardModule;

/// <summary>
/// STD3000 資料 DTO (SYS3620 - 標準資料維護)
/// </summary>
public class Std3000DataDto
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 資料代碼
    /// </summary>
    public string DataId { get; set; } = string.Empty;

    /// <summary>
    /// 資料名稱
    /// </summary>
    public string DataName { get; set; } = string.Empty;

    /// <summary>
    /// 資料值
    /// </summary>
    public string? DataValue { get; set; }

    /// <summary>
    /// 資料類型
    /// </summary>
    public string? DataType { get; set; }

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";

    /// <summary>
    /// 排序順序
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Memo { get; set; }

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

/// <summary>
/// STD3000 資料查詢 DTO
/// </summary>
public class Std3000DataQueryDto
{
    public string? DataId { get; set; }
    public string? DataName { get; set; }
    public string? DataType { get; set; }
    public string? Status { get; set; }
    public string? Keyword { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

/// <summary>
/// STD3000 資料建立 DTO
/// </summary>
public class CreateStd3000DataDto
{
    /// <summary>
    /// 資料代碼
    /// </summary>
    public string DataId { get; set; } = string.Empty;

    /// <summary>
    /// 資料名稱
    /// </summary>
    public string DataName { get; set; } = string.Empty;

    /// <summary>
    /// 資料值
    /// </summary>
    public string? DataValue { get; set; }

    /// <summary>
    /// 資料類型
    /// </summary>
    public string? DataType { get; set; }

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";

    /// <summary>
    /// 排序順序
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Memo { get; set; }
}

/// <summary>
/// STD3000 資料修改 DTO
/// </summary>
public class UpdateStd3000DataDto
{
    /// <summary>
    /// 資料名稱
    /// </summary>
    public string DataName { get; set; } = string.Empty;

    /// <summary>
    /// 資料值
    /// </summary>
    public string? DataValue { get; set; }

    /// <summary>
    /// 資料類型
    /// </summary>
    public string? DataType { get; set; }

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";

    /// <summary>
    /// 排序順序
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Memo { get; set; }
}

