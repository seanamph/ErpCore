namespace ErpCore.Application.DTOs.StoreMember;

/// <summary>
/// 商店 DTO (SYS3000 - 商店資料維護)
/// </summary>
public class ShopDto
{
    public long TKey { get; set; }
    public string ShopId { get; set; } = string.Empty;
    public string ShopName { get; set; } = string.Empty;
    public string? ShopNameEn { get; set; }
    public string? ShopType { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Zone { get; set; }
    public string? PostalCode { get; set; }
    public string? Phone { get; set; }
    public string? Fax { get; set; }
    public string? Email { get; set; }
    public string? ManagerName { get; set; }
    public string? ManagerPhone { get; set; }
    public DateTime? OpenDate { get; set; }
    public DateTime? CloseDate { get; set; }
    public string Status { get; set; } = "A";
    public string? FloorId { get; set; }
    public string? AreaId { get; set; }
    public bool? PosEnabled { get; set; }
    public string? PosSystemId { get; set; }
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 商店查詢 DTO
/// </summary>
public class ShopQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? ShopId { get; set; }
    public string? ShopName { get; set; }
    public string? ShopType { get; set; }
    public string? Status { get; set; }
    public string? City { get; set; }
    public string? FloorId { get; set; }
    public string? AreaId { get; set; }
}

/// <summary>
/// 新增商店 DTO
/// </summary>
public class CreateShopDto
{
    public string ShopId { get; set; } = string.Empty;
    public string ShopName { get; set; } = string.Empty;
    public string? ShopNameEn { get; set; }
    public string? ShopType { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Zone { get; set; }
    public string? PostalCode { get; set; }
    public string? Phone { get; set; }
    public string? Fax { get; set; }
    public string? Email { get; set; }
    public string? ManagerName { get; set; }
    public string? ManagerPhone { get; set; }
    public DateTime? OpenDate { get; set; }
    public DateTime? CloseDate { get; set; }
    public string Status { get; set; } = "A";
    public string? FloorId { get; set; }
    public string? AreaId { get; set; }
    public bool? PosEnabled { get; set; }
    public string? PosSystemId { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 修改商店 DTO
/// </summary>
public class UpdateShopDto
{
    public string ShopName { get; set; } = string.Empty;
    public string? ShopNameEn { get; set; }
    public string? ShopType { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Zone { get; set; }
    public string? PostalCode { get; set; }
    public string? Phone { get; set; }
    public string? Fax { get; set; }
    public string? Email { get; set; }
    public string? ManagerName { get; set; }
    public string? ManagerPhone { get; set; }
    public DateTime? OpenDate { get; set; }
    public DateTime? CloseDate { get; set; }
    public string Status { get; set; } = "A";
    public string? FloorId { get; set; }
    public string? AreaId { get; set; }
    public bool? PosEnabled { get; set; }
    public string? PosSystemId { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 會員 DTO (SYS3000 - 會員資料維護)
/// </summary>
public class MemberDto
{
    public long TKey { get; set; }
    public string MemberId { get; set; } = string.Empty;
    public string MemberName { get; set; } = string.Empty;
    public string? MemberNameEn { get; set; }
    public string? Gender { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? PersonalId { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Zone { get; set; }
    public string? PostalCode { get; set; }
    public string? MemberLevel { get; set; }
    public decimal Points { get; set; }
    public decimal TotalPoints { get; set; }
    public string? CardNo { get; set; }
    public string? CardType { get; set; }
    public DateTime? JoinDate { get; set; }
    public DateTime? ExpireDate { get; set; }
    public string Status { get; set; } = "A";
    public string? PhotoPath { get; set; }
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 會員查詢 DTO
/// </summary>
public class MemberQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? MemberId { get; set; }
    public string? MemberName { get; set; }
    public string? PersonalId { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Email { get; set; }
    public string? MemberLevel { get; set; }
    public string? Status { get; set; }
    public string? CardNo { get; set; }
}

/// <summary>
/// 新增會員 DTO
/// </summary>
public class CreateMemberDto
{
    public string MemberId { get; set; } = string.Empty;
    public string MemberName { get; set; } = string.Empty;
    public string? MemberNameEn { get; set; }
    public string? Gender { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? PersonalId { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Zone { get; set; }
    public string? PostalCode { get; set; }
    public string? MemberLevel { get; set; }
    public decimal Points { get; set; }
    public string? CardNo { get; set; }
    public string? CardType { get; set; }
    public DateTime? JoinDate { get; set; }
    public DateTime? ExpireDate { get; set; }
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
}

/// <summary>
/// 修改會員 DTO
/// </summary>
public class UpdateMemberDto
{
    public string MemberName { get; set; } = string.Empty;
    public string? MemberNameEn { get; set; }
    public string? Gender { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? PersonalId { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Zone { get; set; }
    public string? PostalCode { get; set; }
    public string? MemberLevel { get; set; }
    public decimal Points { get; set; }
    public string? CardNo { get; set; }
    public string? CardType { get; set; }
    public DateTime? JoinDate { get; set; }
    public DateTime? ExpireDate { get; set; }
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
}

/// <summary>
/// 會員積分記錄 DTO
/// </summary>
public class MemberPointDto
{
    public long TKey { get; set; }
    public string MemberId { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public decimal Points { get; set; }
    public decimal Balance { get; set; }
    public string? ReferenceNo { get; set; }
    public string? Description { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 會員積分查詢 DTO
/// </summary>
public class MemberPointQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public DateTime? TransactionDateFrom { get; set; }
    public DateTime? TransactionDateTo { get; set; }
    public string? TransactionType { get; set; }
}

/// <summary>
/// 更新商店狀態 DTO
/// </summary>
public class UpdateShopStatusDto
{
    public string Status { get; set; } = "A";
}

/// <summary>
/// 更新會員狀態 DTO
/// </summary>
public class UpdateMemberStatusDto
{
    public string Status { get; set; } = "A";
}

/// <summary>
/// 上傳照片結果 DTO
/// </summary>
public class UploadPhotoResultDto
{
    public string PhotoPath { get; set; } = string.Empty;
}

/// <summary>
/// 商店查詢結果 DTO (SYS3210-SYS3299)
/// </summary>
public class ShopQueryDto
{
    public string ShopId { get; set; } = string.Empty;
    public string ShopName { get; set; } = string.Empty;
    public string? ShopType { get; set; }
    public string Status { get; set; } = "A";
    public string? City { get; set; }
    public string? Zone { get; set; }
    public string? ManagerName { get; set; }
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 商店查詢請求 DTO
/// </summary>
public class ShopQueryRequestDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public ShopQueryFiltersDto? Filters { get; set; }
}

/// <summary>
/// 商店查詢篩選條件 DTO
/// </summary>
public class ShopQueryFiltersDto
{
    public string? ShopId { get; set; }
    public string? ShopName { get; set; }
    public string? ShopType { get; set; }
    public string? Status { get; set; }
    public string? City { get; set; }
    public string? Zone { get; set; }
    public string? ManagerName { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

/// <summary>
/// 商店匯出請求 DTO
/// </summary>
public class ShopExportRequestDto
{
    public string ExportType { get; set; } = "Excel"; // Excel 或 PDF
    public ShopQueryFiltersDto? Filters { get; set; }
    public List<string>? Columns { get; set; }
}

/// <summary>
/// 會員查詢結果 DTO (SYS3330-SYS33B0)
/// </summary>
public class MemberQueryResultDto
{
    public string MemberId { get; set; } = string.Empty;
    public string MemberName { get; set; } = string.Empty;
    public string? PersonalId { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Email { get; set; }
    public string? MemberLevel { get; set; }
    public string Status { get; set; } = "A";
    public string? CardNo { get; set; }
    public DateTime? JoinDate { get; set; }
}

/// <summary>
/// 會員查詢請求 DTO
/// </summary>
public class MemberQueryRequestDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public MemberQueryFiltersDto? Filters { get; set; }
}

/// <summary>
/// 會員查詢篩選條件 DTO
/// </summary>
public class MemberQueryFiltersDto
{
    public string? MemberId { get; set; }
    public string? MemberName { get; set; }
    public string? PersonalId { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Email { get; set; }
    public string? MemberLevel { get; set; }
    public string? Status { get; set; }
    public string? CardNo { get; set; }
    public DateTime? JoinDateFrom { get; set; }
    public DateTime? JoinDateTo { get; set; }
}

/// <summary>
/// 會員交易記錄 DTO
/// </summary>
public class MemberTransactionDto
{
    public long TKey { get; set; }
    public string MemberId { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public string? TransactionNo { get; set; }
    public decimal Amount { get; set; }
    public decimal Points { get; set; }
    public decimal PointsBalance { get; set; }
    public string? ShopId { get; set; }
    public string? StoreId { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 會員回門禮卡號補登記錄 DTO
/// </summary>
public class MemberExchangeLogDto
{
    public long TKey { get; set; }
    public DateTime LogDate { get; set; }
    public string CardNo { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string? Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 促銷活動 DTO (SYS3510-SYS3600)
/// </summary>
public class PromotionDto
{
    public long TKey { get; set; }
    public string PromotionId { get; set; } = string.Empty;
    public string PromotionName { get; set; } = string.Empty;
    public string? PromotionType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal MinPurchaseAmount { get; set; }
    public decimal? MaxDiscountAmount { get; set; }
    public string? ApplicableShops { get; set; }
    public string? ApplicableProducts { get; set; }
    public string? ApplicableMemberLevels { get; set; }
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 促銷活動查詢 DTO
/// </summary>
public class PromotionQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? PromotionId { get; set; }
    public string? PromotionName { get; set; }
    public string? PromotionType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Status { get; set; }
    public string? ShopId { get; set; }
}

/// <summary>
/// 新增促銷活動 DTO
/// </summary>
public class CreatePromotionDto
{
    public string PromotionId { get; set; } = string.Empty;
    public string PromotionName { get; set; } = string.Empty;
    public string? PromotionType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal MinPurchaseAmount { get; set; }
    public decimal? MaxDiscountAmount { get; set; }
    public string? ApplicableShops { get; set; }
    public string? ApplicableProducts { get; set; }
    public string? ApplicableMemberLevels { get; set; }
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
}

/// <summary>
/// 修改促銷活動 DTO
/// </summary>
public class UpdatePromotionDto
{
    public string PromotionName { get; set; } = string.Empty;
    public string? PromotionType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal MinPurchaseAmount { get; set; }
    public decimal? MaxDiscountAmount { get; set; }
    public string? ApplicableShops { get; set; }
    public string? ApplicableProducts { get; set; }
    public string? ApplicableMemberLevels { get; set; }
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
}

/// <summary>
/// 促銷活動統計 DTO
/// </summary>
public class PromotionStatisticsDto
{
    public string PromotionId { get; set; } = string.Empty;
    public int TotalUsage { get; set; }
    public decimal TotalDiscountAmount { get; set; }
    public int MemberUsage { get; set; }
    public decimal AverageDiscount { get; set; }
}

/// <summary>
/// 商店報表查詢 DTO (SYS3410-SYS3440)
/// </summary>
public class StoreReportQueryDto
{
    public string ReportType { get; set; } = string.Empty; // MEMBER_STATISTICS, SHOP_SALES, PROMOTION_EFFECT
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? ShopId { get; set; }
    public string? MemberLevel { get; set; }
    public string? Status { get; set; }
    public string? PromoId { get; set; }
    public string? PromoType { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// 會員統計報表 DTO
/// </summary>
public class MemberStatisticsReportDto
{
    public string MemberId { get; set; } = string.Empty;
    public string MemberName { get; set; } = string.Empty;
    public string? MemberLevel { get; set; }
    public DateTime? JoinDate { get; set; }
    public string Status { get; set; } = "A";
    public int OrderCount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalDiscount { get; set; }
    public DateTime? LastOrderDate { get; set; }
}

/// <summary>
/// 商店銷售報表 DTO
/// </summary>
public class ShopSalesReportDto
{
    public string ShopId { get; set; } = string.Empty;
    public string ShopName { get; set; } = string.Empty;
    public DateTime ReportDate { get; set; }
    public int OrderCount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalDiscount { get; set; }
    public int MemberCount { get; set; }
}

/// <summary>
/// 促銷活動效果報表 DTO
/// </summary>
public class PromotionEffectReportDto
{
    public string PromoId { get; set; } = string.Empty;
    public string PromoName { get; set; } = string.Empty;
    public string? PromoType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int MemberCount { get; set; }
    public int UsageCount { get; set; }
    public decimal TotalDiscountAmount { get; set; }
    public decimal AvgDiscountAmount { get; set; }
}

/// <summary>
/// 商店報表匯出 DTO
/// </summary>
public class StoreReportExportDto
{
    public string ReportType { get; set; } = string.Empty; // MEMBER_STATISTICS, SHOP_SALES, PROMOTION_EFFECT
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? ShopId { get; set; }
    public string? MemberLevel { get; set; }
    public string? Status { get; set; }
    public string? PromoId { get; set; }
    public string? PromoType { get; set; }
}

/// <summary>
/// 會員匯出請求 DTO
/// </summary>
public class MemberExportRequestDto
{
    public string ExportType { get; set; } = "Excel"; // Excel 或 PDF
    public MemberQueryFiltersDto? Filters { get; set; }
}

/// <summary>
/// 會員列印請求 DTO
/// </summary>
public class MemberPrintRequestDto
{
    public MemberQueryFiltersDto? Filters { get; set; }
}

/// <summary>
/// 會員回門禮卡號補登查詢 DTO
/// </summary>
public class MemberExchangeLogQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public DateTime? LogDate { get; set; }
    public string? CardNo { get; set; }
}

/// <summary>
/// 更新促銷活動狀態 DTO
/// </summary>
public class UpdatePromotionStatusDto
{
    public string Status { get; set; } = "A";
}

