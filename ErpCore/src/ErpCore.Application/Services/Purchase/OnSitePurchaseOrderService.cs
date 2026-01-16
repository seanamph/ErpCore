using ErpCore.Application.DTOs.Purchase;
using ErpCore.Infrastructure.Repositories.Inventory;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Purchase;

/// <summary>
/// 現場打單作業服務實作 (SYSW322)
/// 使用現有的 PurchaseOrderService，但自動設定 SourceProgram = 'SYSW322'
/// </summary>
public class OnSitePurchaseOrderService : IOnSitePurchaseOrderService
{
    private readonly IPurchaseOrderService _purchaseOrderService;
    private readonly IProductGoodsIdRepository _productRepository;
    private readonly ILoggerService _logger;

    public OnSitePurchaseOrderService(
        IPurchaseOrderService purchaseOrderService,
        IProductGoodsIdRepository productRepository,
        ILoggerService logger)
    {
        _purchaseOrderService = purchaseOrderService;
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<PagedResult<PurchaseOrderDto>> GetOnSitePurchaseOrdersAsync(PurchaseOrderQueryDto query)
    {
        try
        {
            // 自動過濾 SYSW322 的資料
            query.SourceProgram = "SYSW322";
            return await _purchaseOrderService.GetPurchaseOrdersAsync(query);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢現場打單申請單列表失敗", ex);
            throw;
        }
    }

    public async Task<PurchaseOrderFullDto> GetOnSitePurchaseOrderByIdAsync(string orderId)
    {
        try
        {
            var result = await _purchaseOrderService.GetPurchaseOrderByIdAsync(orderId);
            // 驗證是否為 SYSW322 的資料
            if (result != null && result.SourceProgram != "SYSW322")
            {
                throw new InvalidOperationException($"採購單 {orderId} 不屬於 SYSW322 功能");
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢現場打單申請單失敗: {orderId}", ex);
            throw;
        }
    }

    public async Task<string> CreateOnSitePurchaseOrderAsync(CreatePurchaseOrderDto dto)
    {
        try
        {
            // 自動設定來源程式為 SYSW322
            dto.SourceProgram = "SYSW322";
            return await _purchaseOrderService.CreatePurchaseOrderAsync(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError("新增現場打單申請單失敗", ex);
            throw;
        }
    }

    public async Task UpdateOnSitePurchaseOrderAsync(string orderId, UpdatePurchaseOrderDto dto)
    {
        try
        {
            // 驗證是否為 SYSW322 的資料
            var existing = await _purchaseOrderService.GetPurchaseOrderByIdAsync(orderId);
            if (existing != null && existing.SourceProgram != "SYSW322")
            {
                throw new InvalidOperationException($"採購單 {orderId} 不屬於 SYSW322 功能");
            }

            await _purchaseOrderService.UpdatePurchaseOrderAsync(orderId, dto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改現場打單申請單失敗: {orderId}", ex);
            throw;
        }
    }

    public async Task DeleteOnSitePurchaseOrderAsync(string orderId)
    {
        try
        {
            // 驗證是否為 SYSW322 的資料
            var existing = await _purchaseOrderService.GetPurchaseOrderByIdAsync(orderId);
            if (existing != null && existing.SourceProgram != "SYSW322")
            {
                throw new InvalidOperationException($"採購單 {orderId} 不屬於 SYSW322 功能");
            }

            await _purchaseOrderService.DeletePurchaseOrderAsync(orderId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除現場打單申請單失敗: {orderId}", ex);
            throw;
        }
    }

    public async Task SubmitOnSitePurchaseOrderAsync(string orderId)
    {
        try
        {
            // 驗證是否為 SYSW322 的資料
            var existing = await _purchaseOrderService.GetPurchaseOrderByIdAsync(orderId);
            if (existing != null && existing.SourceProgram != "SYSW322")
            {
                throw new InvalidOperationException($"採購單 {orderId} 不屬於 SYSW322 功能");
            }

            await _purchaseOrderService.SubmitPurchaseOrderAsync(orderId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"送出現場打單申請單失敗: {orderId}", ex);
            throw;
        }
    }

    public async Task ApproveOnSitePurchaseOrderAsync(string orderId)
    {
        try
        {
            // 驗證是否為 SYSW322 的資料
            var existing = await _purchaseOrderService.GetPurchaseOrderByIdAsync(orderId);
            if (existing != null && existing.SourceProgram != "SYSW322")
            {
                throw new InvalidOperationException($"採購單 {orderId} 不屬於 SYSW322 功能");
            }

            await _purchaseOrderService.ApprovePurchaseOrderAsync(orderId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"審核現場打單申請單失敗: {orderId}", ex);
            throw;
        }
    }

    public async Task CancelOnSitePurchaseOrderAsync(string orderId)
    {
        try
        {
            // 驗證是否為 SYSW322 的資料
            var existing = await _purchaseOrderService.GetPurchaseOrderByIdAsync(orderId);
            if (existing != null && existing.SourceProgram != "SYSW322")
            {
                throw new InvalidOperationException($"採購單 {orderId} 不屬於 SYSW322 功能");
            }

            await _purchaseOrderService.CancelPurchaseOrderAsync(orderId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"取消現場打單申請單失敗: {orderId}", ex);
            throw;
        }
    }

    public async Task<GoodsByBarcodeDto> GetGoodsByBarcodeAsync(string barcode)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(barcode))
            {
                throw new ArgumentException("條碼不能為空", nameof(barcode));
            }

            // 根據條碼查詢商品
            var query = new ProductGoodsIdQuery
            {
                BarcodeId = barcode,
                PageIndex = 1,
                PageSize = 1
            };

            var result = await _productRepository.QueryAsync(query);

            if (result.Items == null || !result.Items.Any())
            {
                throw new InvalidOperationException($"找不到條碼為 {barcode} 的商品");
            }

            var product = result.Items.First();

            return new GoodsByBarcodeDto
            {
                GoodsId = product.GoodsId,
                GoodsName = product.GoodsName,
                BarcodeId = product.BarcodeId ?? barcode,
                UnitPrice = product.Lprc > 0 ? product.Lprc : product.Mprc, // 優先使用進價，否則使用中價
                Unit = product.Unit
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"根據條碼查詢商品失敗: {barcode}", ex);
            throw;
        }
    }
}
