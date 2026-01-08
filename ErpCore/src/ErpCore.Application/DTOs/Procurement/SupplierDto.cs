namespace ErpCore.Application.DTOs.Procurement;

/// <summary>
/// 供應商 DTO (SYSP210-SYSP260)
/// </summary>
public class SupplierDto
{
    public long TKey { get; set; }
    public string SupplierId { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public string? SupplierNameE { get; set; }
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Fax { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? PaymentTerms { get; set; }
    public string? TaxId { get; set; }
    public string Status { get; set; } = "A";
    public string? Rating { get; set; }
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 建立供應商 DTO
/// </summary>
public class CreateSupplierDto
{
    public string SupplierId { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public string? SupplierNameE { get; set; }
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Fax { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? PaymentTerms { get; set; }
    public string? TaxId { get; set; }
    public string Status { get; set; } = "A";
    public string? Rating { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 修改供應商 DTO
/// </summary>
public class UpdateSupplierDto
{
    public string SupplierName { get; set; } = string.Empty;
    public string? SupplierNameE { get; set; }
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Fax { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? PaymentTerms { get; set; }
    public string? TaxId { get; set; }
    public string Status { get; set; } = "A";
    public string? Rating { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 查詢供應商 DTO
/// </summary>
public class SupplierQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SupplierId { get; set; }
    public string? SupplierName { get; set; }
    public string? Status { get; set; }
    public string? Rating { get; set; }
}

