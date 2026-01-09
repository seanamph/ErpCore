namespace ErpCore.Application.DTOs.StoreFloor;

/// <summary>
/// 類型代碼 DTO (SYS6405-SYS6490 - 類型代碼維護)
/// </summary>
public class TypeCodeDto
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 類型代碼
    /// </summary>
    public string TypeCode { get; set; } = string.Empty;

    /// <summary>
    /// 類型名稱
    /// </summary>
    public string TypeName { get; set; } = string.Empty;

    /// <summary>
    /// 類型英文名稱
    /// </summary>
    public string? TypeNameEn { get; set; }

    /// <summary>
    /// 分類
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// 說明
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 排序順序
    /// </summary>
    public int? SortOrder { get; set; }

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";

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
/// 類型代碼查詢 DTO
/// </summary>
public class TypeCodeQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? TypeCode { get; set; }
    public string? TypeName { get; set; }
    public string? Category { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 建立類型代碼 DTO
/// </summary>
public class CreateTypeCodeDto
{
    /// <summary>
    /// 類型代碼
    /// </summary>
    public string TypeCode { get; set; } = string.Empty;

    /// <summary>
    /// 類型名稱
    /// </summary>
    public string TypeName { get; set; } = string.Empty;

    /// <summary>
    /// 類型英文名稱
    /// </summary>
    public string? TypeNameEn { get; set; }

    /// <summary>
    /// 分類
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// 說明
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 排序順序
    /// </summary>
    public int? SortOrder { get; set; }

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";
}

/// <summary>
/// 修改類型代碼 DTO
/// </summary>
public class UpdateTypeCodeDto
{
    /// <summary>
    /// 類型名稱
    /// </summary>
    public string TypeName { get; set; } = string.Empty;

    /// <summary>
    /// 類型英文名稱
    /// </summary>
    public string? TypeNameEn { get; set; }

    /// <summary>
    /// 分類
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// 說明
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 排序順序
    /// </summary>
    public int? SortOrder { get; set; }

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";
}

