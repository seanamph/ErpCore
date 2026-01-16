using ErpCore.Application.DTOs.Inventory;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Inventory;
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

    public SupplierGoodsService(
        ISupplierGoodsRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
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
                Lprc = dto.Lprc ?? 0,
                Mprc = dto.Mprc ?? 0,
                Tax = dto.Tax ?? "1",
                MinQty = dto.MinQty ?? 0,
                MaxQty = dto.MaxQty ?? 0,
                Unit = dto.Unit,
                Rate = dto.Rate ?? 1,
                Status = dto.Status ?? "0",
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Slprc = dto.Slprc ?? 0,
                ArrivalDays = dto.ArrivalDays ?? 0,
                OrdDay1 = dto.OrdDay1 ?? "Y",
                OrdDay2 = dto.OrdDay2 ?? "Y",
                OrdDay3 = dto.OrdDay3 ?? "Y",
                OrdDay4 = dto.OrdDay4 ?? "Y",
                OrdDay5 = dto.OrdDay5 ?? "Y",
                OrdDay6 = dto.OrdDay6 ?? "Y",
                OrdDay7 = dto.OrdDay7 ?? "Y",
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now,
                CreatedPriority = null,
                CreatedGroup = GetCurrentOrgId()
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
            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(supplierId, barcodeId, shopId);
            if (entity == null)
            {
                throw new InvalidOperationException($"供應商商品不存在: {supplierId}/{barcodeId}/{shopId}");
            }

            // 驗證資料
            ValidateUpdateDto(dto);

            entity.Lprc = dto.Lprc ?? 0;
            entity.Mprc = dto.Mprc ?? 0;
            entity.Tax = dto.Tax ?? "1";
            entity.MinQty = dto.MinQty ?? 0;
            entity.MaxQty = dto.MaxQty ?? 0;
            entity.Unit = dto.Unit;
            entity.Rate = dto.Rate ?? 1;
            entity.Status = dto.Status ?? "0";
            entity.StartDate = dto.StartDate;
            entity.EndDate = dto.EndDate;
            entity.Slprc = dto.Slprc ?? 0;
            entity.ArrivalDays = dto.ArrivalDays ?? 0;
            entity.OrdDay1 = dto.OrdDay1 ?? "Y";
            entity.OrdDay2 = dto.OrdDay2 ?? "Y";
            entity.OrdDay3 = dto.OrdDay3 ?? "Y";
            entity.OrdDay4 = dto.OrdDay4 ?? "Y";
            entity.OrdDay5 = dto.OrdDay5 ?? "Y";
            entity.OrdDay6 = dto.OrdDay6 ?? "Y";
            entity.OrdDay7 = dto.OrdDay7 ?? "Y";
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
            var entity = await _repository.GetByIdAsync(supplierId, barcodeId, shopId);
            if (entity == null)
            {
                throw new InvalidOperationException($"供應商商品不存在: {supplierId}/{barcodeId}/{shopId}");
            }

            await _repository.DeleteAsync(supplierId, barcodeId, shopId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除供應商商品失敗: {supplierId}/{barcodeId}/{shopId}", ex);
            throw;
        }
    }

    public async Task DeleteSupplierGoodsBatchAsync(BatchDeleteSupplierGoodsDto dto)
    {
        try
        {
            foreach (var item in dto.Items)
            {
                await DeleteSupplierGoodsAsync(item.SupplierId, item.BarcodeId, item.ShopId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("批次刪除供應商商品失敗", ex);
            throw;
        }
    }

    public async Task UpdateStatusAsync(string supplierId, string barcodeId, string shopId, string status)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(supplierId, barcodeId, shopId);
            if (entity == null)
            {
                throw new InvalidOperationException($"供應商商品不存在: {supplierId}/{barcodeId}/{shopId}");
            }

            if (status != "0" && status != "1")
            {
                throw new ArgumentException("狀態值必須為 0 (正常) 或 1 (停用)");
            }

            entity.Status = status;
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

    private void ValidateCreateDto(CreateSupplierGoodsDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.SupplierId))
        {
            throw new ArgumentException("供應商編號不能為空");
        }

        if (string.IsNullOrWhiteSpace(dto.BarcodeId))
        {
            throw new ArgumentException("商品條碼不能為空");
        }

        if (string.IsNullOrWhiteSpace(dto.ShopId))
        {
            throw new ArgumentException("店別代碼不能為空");
        }

        if (dto.MinQty.HasValue && dto.MaxQty.HasValue && dto.MinQty > dto.MaxQty)
        {
            throw new ArgumentException("最小訂購量不能大於最大訂購量");
        }

        if (dto.StartDate.HasValue && dto.EndDate.HasValue && dto.StartDate > dto.EndDate)
        {
            throw new ArgumentException("有效起始日不能大於有效終止日");
        }

        if (!string.IsNullOrEmpty(dto.Tax) && dto.Tax != "0" && dto.Tax != "1")
        {
            throw new ArgumentException("稅別值必須為 0 (免稅) 或 1 (應稅)");
        }

        if (!string.IsNullOrEmpty(dto.Status) && dto.Status != "0" && dto.Status != "1")
        {
            throw new ArgumentException("狀態值必須為 0 (正常) 或 1 (停用)");
        }
    }

    private void ValidateUpdateDto(UpdateSupplierGoodsDto dto)
    {
        if (dto.MinQty.HasValue && dto.MaxQty.HasValue && dto.MinQty > dto.MaxQty)
        {
            throw new ArgumentException("最小訂購量不能大於最大訂購量");
        }

        if (dto.StartDate.HasValue && dto.EndDate.HasValue && dto.StartDate > dto.EndDate)
        {
            throw new ArgumentException("有效起始日不能大於有效終止日");
        }

        if (!string.IsNullOrEmpty(dto.Tax) && dto.Tax != "0" && dto.Tax != "1")
        {
            throw new ArgumentException("稅別值必須為 0 (免稅) 或 1 (應稅)");
        }

        if (!string.IsNullOrEmpty(dto.Status) && dto.Status != "0" && dto.Status != "1")
        {
            throw new ArgumentException("狀態值必須為 0 (正常) 或 1 (停用)");
        }
    }
}
