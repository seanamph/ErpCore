using ErpCore.Shared.Common;

namespace ErpCore.Application.DTOs.StandardModule;

/// <summary>
/// STD5000 基礎資料 DTO (SYS5110-SYS5150 - 基礎資料維護)
/// </summary>
public class Std5000BaseDataDto
{
    public long TKey { get; set; }
    public string DataId { get; set; } = string.Empty;
    public string DataName { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public string Status { get; set; } = "A";
    public string? Memo { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// STD5000 基礎資料查詢 DTO
/// </summary>
public class Std5000BaseDataQueryDto
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
/// STD5000 基礎資料建立 DTO
/// </summary>
public class CreateStd5000BaseDataDto
{
    public string DataId { get; set; } = string.Empty;
    public string DataName { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public string Status { get; set; } = "A";
    public string? Memo { get; set; }
}

/// <summary>
/// STD5000 基礎資料修改 DTO
/// </summary>
public class UpdateStd5000BaseDataDto
{
    public string DataName { get; set; } = string.Empty;
    public string Status { get; set; } = "A";
    public string? Memo { get; set; }
}

/// <summary>
/// STD5000 會員 DTO (SYS5210-SYS52A0 - 會員管理)
/// </summary>
public class Std5000MemberDto
{
    public long TKey { get; set; }
    public string MemberId { get; set; } = string.Empty;
    public string MemberName { get; set; } = string.Empty;
    public string? MemberType { get; set; }
    public string? IdCard { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime JoinDate { get; set; }
    public decimal Points { get; set; }
    public string Status { get; set; } = "A";
    public string? ShopId { get; set; }
    public string? Memo { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// STD5000 會員查詢 DTO
/// </summary>
public class Std5000MemberQueryDto
{
    public string? MemberId { get; set; }
    public string? MemberName { get; set; }
    public string? MemberType { get; set; }
    public string? Status { get; set; }
    public string? ShopId { get; set; }
    public string? Keyword { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

/// <summary>
/// STD5000 會員建立 DTO
/// </summary>
public class CreateStd5000MemberDto
{
    public string MemberId { get; set; } = string.Empty;
    public string MemberName { get; set; } = string.Empty;
    public string? MemberType { get; set; }
    public string? IdCard { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public DateTime? BirthDate { get; set; }
    public decimal Points { get; set; }
    public string Status { get; set; } = "A";
    public string? ShopId { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// STD5000 會員修改 DTO
/// </summary>
public class UpdateStd5000MemberDto
{
    public string MemberName { get; set; } = string.Empty;
    public string? MemberType { get; set; }
    public string? IdCard { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public DateTime? BirthDate { get; set; }
    public decimal Points { get; set; }
    public string Status { get; set; } = "A";
    public string? ShopId { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// STD5000 會員積分 DTO (SYS5210-SYS52A0 - 會員積分管理)
/// </summary>
public class Std5000MemberPointDto
{
    public long TKey { get; set; }
    public string MemberId { get; set; } = string.Empty;
    public DateTime TransDate { get; set; }
    public string TransType { get; set; } = string.Empty;
    public decimal Points { get; set; }
    public string? TransId { get; set; }
    public string? Memo { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// STD5000 會員積分查詢 DTO
/// </summary>
public class Std5000MemberPointQueryDto
{
    public string? MemberId { get; set; }
    public string? TransType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

/// <summary>
/// STD5000 會員積分建立 DTO
/// </summary>
public class CreateStd5000MemberPointDto
{
    public string MemberId { get; set; } = string.Empty;
    public DateTime TransDate { get; set; }
    public string TransType { get; set; } = string.Empty;
    public decimal Points { get; set; }
    public string? TransId { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// STD5000 交易 DTO (SYS5310-SYS53C6 - 交易管理)
/// </summary>
public class Std5000TransactionDto
{
    public long TKey { get; set; }
    public string TransId { get; set; } = string.Empty;
    public DateTime TransDate { get; set; }
    public string TransType { get; set; } = string.Empty;
    public string? MemberId { get; set; }
    public string? ShopId { get; set; }
    public decimal Amount { get; set; }
    public decimal Points { get; set; }
    public string Status { get; set; } = "A";
    public string? Memo { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<Std5000TransactionDetailDto>? Details { get; set; }
}

/// <summary>
/// STD5000 交易查詢 DTO
/// </summary>
public class Std5000TransactionQueryDto
{
    public string? TransId { get; set; }
    public string? TransType { get; set; }
    public string? MemberId { get; set; }
    public string? ShopId { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Keyword { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

/// <summary>
/// STD5000 交易建立 DTO
/// </summary>
public class CreateStd5000TransactionDto
{
    public string TransId { get; set; } = string.Empty;
    public DateTime TransDate { get; set; }
    public string TransType { get; set; } = string.Empty;
    public string? MemberId { get; set; }
    public string? ShopId { get; set; }
    public decimal Amount { get; set; }
    public decimal Points { get; set; }
    public string Status { get; set; } = "A";
    public string? Memo { get; set; }
    public List<CreateStd5000TransactionDetailDto>? Details { get; set; }
}

/// <summary>
/// STD5000 交易修改 DTO
/// </summary>
public class UpdateStd5000TransactionDto
{
    public string TransType { get; set; } = string.Empty;
    public string? MemberId { get; set; }
    public string? ShopId { get; set; }
    public decimal Amount { get; set; }
    public decimal Points { get; set; }
    public string Status { get; set; } = "A";
    public string? Memo { get; set; }
    public List<CreateStd5000TransactionDetailDto>? Details { get; set; }
}

/// <summary>
/// STD5000 交易明細 DTO (SYS5310-SYS53C6 - 交易明細管理)
/// </summary>
public class Std5000TransactionDetailDto
{
    public long TKey { get; set; }
    public string TransId { get; set; } = string.Empty;
    public int SeqNo { get; set; }
    public string? ProductId { get; set; }
    public string? ProductName { get; set; }
    public decimal Qty { get; set; }
    public decimal Price { get; set; }
    public decimal Amount { get; set; }
    public string? Memo { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// STD5000 交易明細建立 DTO
/// </summary>
public class CreateStd5000TransactionDetailDto
{
    public int SeqNo { get; set; }
    public string? ProductId { get; set; }
    public string? ProductName { get; set; }
    public decimal Qty { get; set; }
    public decimal Price { get; set; }
    public decimal Amount { get; set; }
    public string? Memo { get; set; }
}

