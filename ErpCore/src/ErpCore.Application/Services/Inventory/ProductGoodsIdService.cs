using Dapper;
using ErpCore.Application.DTOs.Inventory;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Inventory;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.Inventory;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Inventory;

/// <summary>
/// 商品進銷碼服務實作
/// </summary>
public class ProductGoodsIdService : BaseService, IProductGoodsIdService
{
    private readonly IProductGoodsIdRepository _repository;
    private readonly IDbConnectionFactory _connectionFactory;

    public ProductGoodsIdService(
        IProductGoodsIdRepository repository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<ProductGoodsIdDto>> GetProductGoodsIdsAsync(ProductGoodsIdQueryDto query)
    {
        try
        {
            var repositoryQuery = new ProductGoodsIdQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                GoodsId = query.GoodsId,
                GoodsName = query.GoodsName,
                BarcodeId = query.BarcodeId,
                ScId = query.ScId,
                Status = query.Status,
                ShopId = query.ShopId
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => new ProductGoodsIdDto
            {
                GoodsId = x.GoodsId,
                GoodsName = x.GoodsName,
                InvPrintName = x.InvPrintName,
                GoodsSpace = x.GoodsSpace,
                ScId = x.ScId,
                Tax = x.Tax,
                TaxName = x.Tax == "1" ? "應稅" : "免稅",
                Lprc = x.Lprc,
                Mprc = x.Mprc,
                BarcodeId = x.BarcodeId,
                Unit = x.Unit,
                ConvertRate = x.ConvertRate,
                Capacity = x.Capacity,
                CapacityUnit = x.CapacityUnit,
                Status = x.Status,
                StatusName = x.Status == "1" ? "正常" : "停用",
                Discount = x.Discount,
                AutoOrder = x.AutoOrder,
                PriceKind = x.PriceKind,
                CostKind = x.CostKind,
                SafeDays = x.SafeDays,
                ExpirationDays = x.ExpirationDays,
                National = x.National,
                Place = x.Place,
                GoodsDeep = x.GoodsDeep,
                GoodsWide = x.GoodsWide,
                GoodsHigh = x.GoodsHigh,
                PackDeep = x.PackDeep,
                PackWide = x.PackWide,
                PackHigh = x.PackHigh,
                PackWeight = x.PackWeight,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();

            return new PagedResult<ProductGoodsIdDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢商品進銷碼列表失敗", ex);
            throw;
        }
    }

    public async Task<ProductGoodsIdDto> GetProductGoodsIdByIdAsync(string goodsId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(goodsId);
            if (entity == null)
            {
                throw new InvalidOperationException($"商品進銷碼不存在: {goodsId}");
            }

            return new ProductGoodsIdDto
            {
                GoodsId = entity.GoodsId,
                GoodsName = entity.GoodsName,
                InvPrintName = entity.InvPrintName,
                GoodsSpace = entity.GoodsSpace,
                ScId = entity.ScId,
                Tax = entity.Tax,
                TaxName = entity.Tax == "1" ? "應稅" : "免稅",
                Lprc = entity.Lprc,
                Mprc = entity.Mprc,
                BarcodeId = entity.BarcodeId,
                Unit = entity.Unit,
                ConvertRate = entity.ConvertRate,
                Capacity = entity.Capacity,
                CapacityUnit = entity.CapacityUnit,
                Status = entity.Status,
                StatusName = entity.Status == "1" ? "正常" : "停用",
                Discount = entity.Discount,
                AutoOrder = entity.AutoOrder,
                PriceKind = entity.PriceKind,
                CostKind = entity.CostKind,
                SafeDays = entity.SafeDays,
                ExpirationDays = entity.ExpirationDays,
                National = entity.National,
                Place = entity.Place,
                GoodsDeep = entity.GoodsDeep,
                GoodsWide = entity.GoodsWide,
                GoodsHigh = entity.GoodsHigh,
                PackDeep = entity.PackDeep,
                PackWide = entity.PackWide,
                PackHigh = entity.PackHigh,
                PackWeight = entity.PackWeight,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt,
                UpdatedBy = entity.UpdatedBy,
                UpdatedAt = entity.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢商品進銷碼失敗: {goodsId}", ex);
            throw;
        }
    }

    public async Task<string> CreateProductGoodsIdAsync(CreateProductGoodsIdDto dto)
    {
        try
        {
            // 驗證資料
            ValidateCreateDto(dto);

            // 檢查是否已存在
            var exists = await _repository.ExistsAsync(dto.GoodsId);
            if (exists)
            {
                throw new InvalidOperationException($"商品進銷碼已存在: {dto.GoodsId}");
            }

            var entity = new Product
            {
                GoodsId = dto.GoodsId,
                GoodsName = dto.GoodsName,
                InvPrintName = dto.InvPrintName,
                GoodsSpace = dto.GoodsSpace,
                ScId = dto.ScId,
                Tax = dto.Tax,
                Lprc = dto.Lprc,
                Mprc = dto.Mprc,
                BarcodeId = dto.BarcodeId,
                Unit = dto.Unit,
                ConvertRate = dto.ConvertRate,
                Capacity = dto.Capacity,
                CapacityUnit = dto.CapacityUnit,
                Status = dto.Status,
                Discount = dto.Discount,
                AutoOrder = dto.AutoOrder,
                PriceKind = dto.PriceKind,
                CostKind = dto.CostKind,
                SafeDays = dto.SafeDays,
                ExpirationDays = dto.ExpirationDays,
                National = dto.National,
                Place = dto.Place,
                GoodsDeep = dto.GoodsDeep,
                GoodsWide = dto.GoodsWide,
                GoodsHigh = dto.GoodsHigh,
                PackDeep = dto.PackDeep,
                PackWide = dto.PackWide,
                PackHigh = dto.PackHigh,
                PackWeight = dto.PackWeight,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(entity);
            return entity.GoodsId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增商品進銷碼失敗: {dto.GoodsId}", ex);
            throw;
        }
    }

    public async Task UpdateProductGoodsIdAsync(string goodsId, UpdateProductGoodsIdDto dto)
    {
        try
        {
            // 驗證資料
            ValidateUpdateDto(dto);

            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(goodsId);
            if (entity == null)
            {
                throw new InvalidOperationException($"商品進銷碼不存在: {goodsId}");
            }

            // 更新欄位
            entity.GoodsName = dto.GoodsName;
            entity.InvPrintName = dto.InvPrintName;
            entity.GoodsSpace = dto.GoodsSpace;
            entity.ScId = dto.ScId;
            entity.Tax = dto.Tax;
            entity.Lprc = dto.Lprc;
            entity.Mprc = dto.Mprc;
            entity.BarcodeId = dto.BarcodeId;
            entity.Unit = dto.Unit;
            entity.ConvertRate = dto.ConvertRate;
            entity.Capacity = dto.Capacity;
            entity.CapacityUnit = dto.CapacityUnit;
            entity.Status = dto.Status;
            entity.Discount = dto.Discount;
            entity.AutoOrder = dto.AutoOrder;
            entity.PriceKind = dto.PriceKind;
            entity.CostKind = dto.CostKind;
            entity.SafeDays = dto.SafeDays;
            entity.ExpirationDays = dto.ExpirationDays;
            entity.National = dto.National;
            entity.Place = dto.Place;
            entity.GoodsDeep = dto.GoodsDeep;
            entity.GoodsWide = dto.GoodsWide;
            entity.GoodsHigh = dto.GoodsHigh;
            entity.PackDeep = dto.PackDeep;
            entity.PackWide = dto.PackWide;
            entity.PackHigh = dto.PackHigh;
            entity.PackWeight = dto.PackWeight;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改商品進銷碼失敗: {goodsId}", ex);
            throw;
        }
    }

    public async Task DeleteProductGoodsIdAsync(string goodsId)
    {
        try
        {
            // 檢查是否存在
            var exists = await _repository.ExistsAsync(goodsId);
            if (!exists)
            {
                throw new InvalidOperationException($"商品進銷碼不存在: {goodsId}");
            }

            // 檢查是否有相關業務資料（採購單、訂單、庫存等）
            await CheckBusinessDataUsageAsync(goodsId);

            await _repository.DeleteAsync(goodsId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除商品進銷碼失敗: {goodsId}", ex);
            throw;
        }
    }

    public async Task BatchDeleteProductGoodsIdAsync(BatchDeleteProductGoodsIdDto dto)
    {
        try
        {
            if (dto.GoodsIds == null || dto.GoodsIds.Count == 0)
            {
                throw new ArgumentException("刪除項目不能為空");
            }

            // 檢查是否有相關業務資料
            foreach (var goodsId in dto.GoodsIds)
            {
                await CheckBusinessDataUsageAsync(goodsId);
            }

            await _repository.BatchDeleteAsync(dto.GoodsIds);
        }
        catch (Exception ex)
        {
            _logger.LogError("批次刪除商品進銷碼失敗", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string goodsId)
    {
        try
        {
            return await _repository.ExistsAsync(goodsId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查商品進銷碼是否存在失敗: {goodsId}", ex);
            throw;
        }
    }

    public async Task UpdateStatusAsync(string goodsId, UpdateProductGoodsIdStatusDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(goodsId);
            if (entity == null)
            {
                throw new InvalidOperationException($"商品進銷碼不存在: {goodsId}");
            }

            entity.Status = dto.Status;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新商品進銷碼狀態失敗: {goodsId}", ex);
            throw;
        }
    }

    private void ValidateCreateDto(CreateProductGoodsIdDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.GoodsId))
        {
            throw new ArgumentException("進銷碼不能為空");
        }

        if (string.IsNullOrWhiteSpace(dto.GoodsName))
        {
            throw new ArgumentException("商品名稱不能為空");
        }

        if (dto.Lprc < 0)
        {
            throw new ArgumentException("進價不能小於0");
        }

        if (dto.Mprc < 0)
        {
            throw new ArgumentException("中價不能小於0");
        }
    }

    private void ValidateUpdateDto(UpdateProductGoodsIdDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.GoodsName))
        {
            throw new ArgumentException("商品名稱不能為空");
        }

        if (dto.Lprc < 0)
        {
            throw new ArgumentException("進價不能小於0");
        }

        if (dto.Mprc < 0)
        {
            throw new ArgumentException("中價不能小於0");
        }
    }

    /// <summary>
    /// 檢查商品進銷碼是否有相關業務資料使用
    /// </summary>
    private async Task CheckBusinessDataUsageAsync(string goodsId)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            
            // 檢查採購單明細中是否使用此商品
            const string purchaseOrderSql = @"
                SELECT COUNT(*) 
                FROM PurchaseOrderDetails 
                WHERE GoodsId = @GoodsId";
            
            var purchaseOrderCount = await connection.ExecuteScalarAsync<int>(purchaseOrderSql, new { GoodsId = goodsId });

            if (purchaseOrderCount > 0)
            {
                throw new InvalidOperationException(
                    $"此商品已被 {purchaseOrderCount} 筆採購單使用，無法刪除。請先處理相關採購單。");
            }

            // 檢查調撥單明細中是否使用此商品
            const string transferOrderSql = @"
                SELECT COUNT(*) 
                FROM TransferOrderDetails 
                WHERE GoodsId = @GoodsId";
            
            var transferOrderCount = await connection.ExecuteScalarAsync<int>(transferOrderSql, new { GoodsId = goodsId });

            if (transferOrderCount > 0)
            {
                throw new InvalidOperationException(
                    $"此商品已被 {transferOrderCount} 筆調撥單使用，無法刪除。請先處理相關調撥單。");
            }

            // 可以繼續檢查其他業務資料（如庫存、盤點等）
            // TODO: 根據實際業務需求添加更多檢查
        }
        catch (InvalidOperationException)
        {
            throw; // 重新拋出業務異常
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查商品進銷碼業務資料使用失敗: {goodsId}", ex);
            throw;
        }
    }
}

