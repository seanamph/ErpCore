namespace ErpCore.Application.DTOs.AnalysisReport;

/// <summary>
/// 商品分類 DTO (SYSA1011)
/// </summary>
public class GoodsCategoryDto
{
    public string CategoryId { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryType { get; set; } = string.Empty; // B:大分類, M:中分類, S:小分類
    public string? ParentId { get; set; }
    public int SeqNo { get; set; }
    public string Status { get; set; } = "1";
}
