using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.InvoiceExtension;

/// <summary>
/// 電子發票擴展服務介面
/// </summary>
public interface IEInvoiceExtensionService
{
    Task<PagedResult<EInvoiceExtensionDto>> GetExtensionsAsync(EInvoiceExtensionQueryDto query);
    Task<EInvoiceExtensionDto?> GetExtensionByIdAsync(long extensionId);
    Task<long> CreateExtensionAsync(CreateEInvoiceExtensionDto dto);
    Task UpdateExtensionAsync(long extensionId, UpdateEInvoiceExtensionDto dto);
    Task DeleteExtensionAsync(long extensionId);
}

/// <summary>
/// 電子發票擴展 DTO
/// </summary>
public class EInvoiceExtensionDto
{
    public long ExtensionId { get; set; }
    public long InvoiceId { get; set; }
    public string ExtensionType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 建立電子發票擴展 DTO
/// </summary>
public class CreateEInvoiceExtensionDto
{
    public long InvoiceId { get; set; }
    public string ExtensionType { get; set; } = string.Empty;
    public string? ExtensionData { get; set; }
}

/// <summary>
/// 修改電子發票擴展 DTO
/// </summary>
public class UpdateEInvoiceExtensionDto
{
    public string ExtensionType { get; set; } = string.Empty;
    public string? ExtensionData { get; set; }
}

/// <summary>
/// 電子發票擴展查詢 DTO
/// </summary>
public class EInvoiceExtensionQueryDto
{
    public long? InvoiceId { get; set; }
    public string? ExtensionType { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

