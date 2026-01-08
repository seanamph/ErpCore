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
/// 供應商商品服務實作
/// </summary>
public class SupplierGoodsService : BaseService, ISupplierGoodsService
{
    private readonly ISupplierGoodsRepository _repository;
    private readonly IDbConnectionFactory _connectionFactory;

    public SupplierGoodsService(
        ISupplierGoodsRepository repository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<SupplierGoodsDto>> GetSupplierGoodsAsync(SupplierGoodsQueryDto query)
    {
        try
        {
            var repositoryQuery = new SupplierGoodsQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                SupplierId = query.SupplierId,
                BarcodeId = query.BarcodeId,
                ShopId = query.ShopId,
                Status = query.Status
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => new SupplierGoodsDto
            {
                SupplierId = x.SupplierId,
                BarcodeId = x.BarcodeId,
                ShopId = x.ShopId,
                Lprc = x.Lprc,
                Mprc = x.Mprc,
                Tax = x.Tax,
                MinQty = x.MinQty,
                MaxQty = x.MaxQty,
                Unit = x.Unit,
                Rate = x.Rate,
                Status = x.Status,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                Slprc = x.Slprc,
                ArrivalDays = x.ArrivalDays,
                OrdDay1 = x.OrdDay1,
                OrdDay2 = x.OrdDay2,
                OrdDay3 = x.OrdDay3,
                OrdDay4 = x.OrdDay4,
                OrdDay5 = x.OrdDay5,
                OrdDay6 = x.OrdDay6,
                OrdDay7 = x.OrdDay7,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();

            return new PagedResult<SupplierGoodsDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢供應商商品列表失敗", ex);
            throw;
        }
    }

    public async Task<SupplierGoodsDto> GetSupplierGoodsByIdAsync(string supplierId, string barcodeId, string shopId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(supplierId, barcodeId, shopId);
            if (entity == null)
            {
                throw new InvalidOperationException($"供應商商品不存在: {supplierId}/{barcodeId}/{shopId}");
            }

            return new SupplierGoodsDto
            {
                SupplierId = entity.SupplierId,
                BarcodeId = entity.BarcodeId,
                ShopId = entity.ShopId,
                Lprc = entity.Lprc,
                Mprc = entity.Mprc,
                Tax = entity.Tax,
                MinQty = entity.MinQty,
                MaxQty = entity.MaxQty,
                Unit = entity.Unit,
                Rate = entity.Rate,
                Status = entity.Status,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                Slprc = entity.Slprc,
                ArrivalDays = entity.ArrivalDays,
                OrdDay1 = entity.OrdDay1,
                OrdDay2 = entity.OrdDay2,
                OrdDay3 = entity.OrdDay3,
                OrdDay4 = entity.OrdDay4,
                OrdDay5 = entity.OrdDay5,
                OrdDay6 = entity.OrdDay6,
                OrdDay7 = entity.OrdDay7,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt,
                UpdatedBy = entity.UpdatedBy,
                UpdatedAt = entity.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢供應商商品失敗: {supplierId}/{barcodeId}/{shopId}", ex);
            throw;
        }
    }

    public async Task CreateSupplierGoodsAsync(CreateSupplierGoodsDto dto)
    {
        try
        {
            // 驗證資料
            ValidateCreateDto(dto);

            // 檢查是否已存在
            var exists = await _repository.ExistsAsync(dto.SupplierId, dto.BarcodeId, dto.ShopId);
            if (exists)
            {
                throw new InvalidOperationException($"供應商商品已存在: {dto.SupplierId}/{dto.BarcodeId}/{dto.ShopId}");
            }

            var entity = new SupplierGoods
            {
                SupplierId = dto.SupplierId,
                BarcodeId = dto.BarcodeId,
                ShopId = dto.ShopId,
                Lprc = dto.Lprc,
                Mprc = dto.Mprc,
                Tax = dto.Tax,
                MinQty = dto.MinQty,
                MaxQty = dto.MaxQty,
                Unit = dto.Unit,
                Rate = dto.Rate,
                Status = dto.Status,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Slprc = dto.Slprc,
                ArrivalDays = dto.ArrivalDays,
                OrdDay1 = dto.OrdDay1,
                OrdDay2 = dto.OrdDay2,
                OrdDay3 = dto.OrdDay3,
                OrdDay4 = dto.OrdDay4,
                OrdDay5 = dto.OrdDay5,
                OrdDay6 = dto.OrdDay6,
                OrdDay7 = dto.OrdDay7,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增供應商商品失敗: {dto.SupplierId}/{dto.BarcodeId}/{dto.ShopId}", ex);
            throw;
        }
    }

    public async Task UpdateSupplierGoodsAsync(string supplierId, string barcodeId, string shopId, UpdateSupplierGoodsDto dto)
    {
        try
        {
            // 驗證資料
            ValidateUpdateDto(dto);

            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(supplierId, barcodeId, shopId);
            if (entity == null)
            {
                throw new InvalidOperationException($"供應商商品不存在: {supplierId}/{barcodeId}/{shopId}");
            }

            // 更新欄位
            entity.Lprc = dto.Lprc;
            entity.Mprc = dto.Mprc;
            entity.Tax = dto.Tax;
            entity.MinQty = dto.MinQty;
            entity.MaxQty = dto.MaxQty;
            entity.Unit = dto.Unit;
            entity.Rate = dto.Rate;
            entity.Status = dto.Status;
            entity.StartDate = dto.StartDate;
            entity.EndDate = dto.EndDate;
            entity.Slprc = dto.Slprc;
            entity.ArrivalDays = dto.ArrivalDays;
            entity.OrdDay1 = dto.OrdDay1;
            entity.OrdDay2 = dto.OrdDay2;
            entity.OrdDay3 = dto.OrdDay3;
            entity.OrdDay4 = dto.OrdDay4;
            entity.OrdDay5 = dto.OrdDay5;
            entity.OrdDay6 = dto.OrdDay6;
            entity.OrdDay7 = dto.OrdDay7;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改供應商商品失敗: {supplierId}/{barcodeId}/{shopId}", ex);
            throw;
        }
    }

    public async Task DeleteSupplierGoodsAsync(string supplierId, string barcodeId, string shopId)
    {
        try
        {
            // 檢查是否存在
            var exists = await _repository.ExistsAsync(supplierId, barcodeId, shopId);
            if (!exists)
            {
                throw new InvalidOperationException($"供應商商品不存在: {supplierId}/{barcodeId}/{shopId}");
            }

            // 檢查是否有相關業務資料（採購單明細等）
            await CheckBusinessDataUsageAsync(supplierId, barcodeId, shopId);

            await _repository.DeleteAsync(supplierId, barcodeId, shopId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除供應商商品失敗: {supplierId}/{barcodeId}/{shopId}", ex);
            throw;
        }
    }

    public async Task BatchDeleteSupplierGoodsAsync(BatchDeleteSupplierGoodsDto dto)
    {
        try
        {
            if (dto.Items == null || dto.Items.Count == 0)
            {
                throw new ArgumentException("刪除項目不能為空");
            }

            var keys = dto.Items.Select(x => new SupplierGoodsKey
            {
                SupplierId = x.SupplierId,
                BarcodeId = x.BarcodeId,
                ShopId = x.ShopId
            }).ToList();

            // 檢查是否有相關業務資料
            foreach (var key in keys)
            {
                await CheckBusinessDataUsageAsync(key.SupplierId, key.BarcodeId, key.ShopId);
            }

            await _repository.BatchDeleteAsync(keys);
        }
        catch (Exception ex)
        {
            _logger.LogError("批次刪除供應商商品失敗", ex);
            throw;
        }
    }

    public async Task UpdateStatusAsync(string supplierId, string barcodeId, string shopId, UpdateSupplierGoodsStatusDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(supplierId, barcodeId, shopId);
            if (entity == null)
            {
                throw new InvalidOperationException($"供應商商品不存在: {supplierId}/{barcodeId}/{shopId}");
            }

            entity.Status = dto.Status;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新供應商商品狀態失敗: {supplierId}/{barcodeId}/{shopId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 驗證新增 DTO
    /// </summary>
    private void ValidateCreateDto(CreateSupplierGoodsDto dto)
    {
        if (string.IsNullOrEmpty(dto.SupplierId))
        {
            throw new ArgumentException("供應商編號不能為空");
        }
        if (string.IsNullOrEmpty(dto.BarcodeId))
        {
            throw new ArgumentException("商品條碼不能為空");
        }
        if (string.IsNullOrEmpty(dto.ShopId))
        {
            throw new ArgumentException("店別代碼不能為空");
        }
        if (dto.Lprc < 0)
        {
            throw new ArgumentException("進價不能小於0");
        }
        if (dto.Mprc < 0)
        {
            throw new ArgumentException("中價不能小於0");
        }
        if (dto.MinQty < 0)
        {
            throw new ArgumentException("最小訂購量不能小於0");
        }
        if (dto.MaxQty < 0)
        {
            throw new ArgumentException("最大訂購量不能小於0");
        }
        if (dto.MinQty > dto.MaxQty)
        {
            throw new ArgumentException("最小訂購量不能大於最大訂購量");
        }
        if (dto.StartDate.HasValue && dto.EndDate.HasValue && dto.StartDate > dto.EndDate)
        {
            throw new ArgumentException("有效起始日不能大於有效終止日");
        }
        if (dto.Tax != "0" && dto.Tax != "1")
        {
            throw new ArgumentException("稅別必須為0或1");
        }
        if (dto.Status != "0" && dto.Status != "1")
        {
            throw new ArgumentException("狀態必須為0或1");
        }
    }

    /// <summary>
    /// 驗證修改 DTO
    /// </summary>
    private void ValidateUpdateDto(UpdateSupplierGoodsDto dto)
    {
        if (dto.Lprc < 0)
        {
            throw new ArgumentException("進價不能小於0");
        }
        if (dto.Mprc < 0)
        {
            throw new ArgumentException("中價不能小於0");
        }
        if (dto.MinQty < 0)
        {
            throw new ArgumentException("最小訂購量不能小於0");
        }
        if (dto.MaxQty < 0)
        {
            throw new ArgumentException("最大訂購量不能小於0");
        }
        if (dto.MinQty > dto.MaxQty)
        {
            throw new ArgumentException("最小訂購量不能大於最大訂購量");
        }
        if (dto.StartDate.HasValue && dto.EndDate.HasValue && dto.StartDate > dto.EndDate)
        {
            throw new ArgumentException("有效起始日不能大於有效終止日");
        }
        if (dto.Tax != "0" && dto.Tax != "1")
        {
            throw new ArgumentException("稅別必須為0或1");
        }
        if (dto.Status != "0" && dto.Status != "1")
        {
            throw new ArgumentException("狀態必須為0或1");
        }
    }

    /// <summary>
    /// 檢查供應商商品是否有相關業務資料使用
    /// </summary>
    private async Task CheckBusinessDataUsageAsync(string supplierId, string barcodeId, string shopId)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            
            // 檢查採購單明細中是否使用此供應商商品
            const string purchaseOrderSql = @"
                SELECT COUNT(*) 
                FROM PurchaseOrderDetails POD
                INNER JOIN PurchaseOrders PO ON POD.OrderId = PO.OrderId
                WHERE PO.SupplierId = @SupplierId 
                  AND POD.BarcodeId = @BarcodeId
                  AND PO.ShopId = @ShopId
                  AND PO.Status != 'X'";
            
            var purchaseOrderCount = await connection.ExecuteScalarAsync<int>(purchaseOrderSql, new
            {
                SupplierId = supplierId,
                BarcodeId = barcodeId,
                ShopId = shopId
            });

            if (purchaseOrderCount > 0)
            {
                throw new InvalidOperationException(
                    $"此供應商商品已被 {purchaseOrderCount} 筆採購單使用，無法刪除。請先處理相關採購單。");
            }

            // 檢查採購驗收單明細中是否使用此供應商商品
            const string purchaseReceiptSql = @"
                SELECT COUNT(*) 
                FROM PurchaseReceiptDetails PRD
                INNER JOIN PurchaseReceipts PR ON PRD.ReceiptId = PR.ReceiptId
                WHERE PR.SupplierId = @SupplierId 
                  AND PRD.BarcodeId = @BarcodeId
                  AND PR.ShopId = @ShopId
                  AND PR.Status != 'X'";
            
            var purchaseReceiptCount = await connection.ExecuteScalarAsync<int>(purchaseReceiptSql, new
            {
                SupplierId = supplierId,
                BarcodeId = barcodeId,
                ShopId = shopId
            });

            if (purchaseReceiptCount > 0)
            {
                throw new InvalidOperationException(
                    $"此供應商商品已被 {purchaseReceiptCount} 筆採購驗收單使用，無法刪除。請先處理相關驗收單。");
            }

            // 檢查庫存資料中是否使用此供應商商品
            const string inventorySql = @"
                SELECT COUNT(*) 
                FROM Inventories 
                WHERE SupplierId = @SupplierId 
                  AND BarcodeId = @BarcodeId
                  AND ShopId = @ShopId";
            
            var inventoryCount = await connection.ExecuteScalarAsync<int>(inventorySql, new
            {
                SupplierId = supplierId,
                BarcodeId = barcodeId,
                ShopId = shopId
            });

            if (inventoryCount > 0)
            {
                throw new InvalidOperationException(
                    $"此供應商商品存在庫存資料（{inventoryCount} 筆），無法刪除。請先處理相關庫存。");
            }
        }
        catch (InvalidOperationException)
        {
            throw; // 重新拋出業務異常
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查供應商商品業務資料使用失敗: {supplierId}/{barcodeId}/{shopId}", ex);
            throw;
        }
    }
}

