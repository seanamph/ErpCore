using ErpCore.Application.DTOs.InvoiceSales;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.InvoiceSales;
using ErpCore.Infrastructure.Repositories.InvoiceSales;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.InvoiceSales;

/// <summary>
/// 銷售單服務實作 (SYSG410-SYSG460 - 銷售資料維護)
/// </summary>
public class SalesOrderService : BaseService, ISalesOrderService
{
    private readonly ISalesOrderRepository _repository;
    private readonly ISalesOrderDetailRepository _detailRepository;

    public SalesOrderService(
        ISalesOrderRepository repository,
        ISalesOrderDetailRepository detailRepository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _detailRepository = detailRepository;
    }

    public async Task<PagedResult<SalesOrderDto>> GetSalesOrdersAsync(SalesOrderQueryDto query)
    {
        try
        {
            var repositoryQuery = new SalesOrderQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                OrderId = query.OrderId,
                OrderType = query.OrderType,
                ShopId = query.ShopId,
                CustomerId = query.CustomerId,
                Status = query.Status,
                OrderDateFrom = query.OrderDateFrom,
                OrderDateTo = query.OrderDateTo
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(MapToDto).ToList();

            return new PagedResult<SalesOrderDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢銷售單列表失敗", ex);
            throw;
        }
    }

    public async Task<SalesOrderDto> GetSalesOrderByIdAsync(long tKey)
    {
        try
        {
            var salesOrder = await _repository.GetByIdAsync(tKey);
            if (salesOrder == null)
            {
                throw new KeyNotFoundException($"銷售單不存在: {tKey}");
            }

            var dto = MapToDto(salesOrder);
            var details = await _detailRepository.GetByOrderIdAsync(salesOrder.OrderId);
            dto.Details = details.Select(MapDetailToDto).ToList();

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢銷售單失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<SalesOrderDto> GetSalesOrderByOrderIdAsync(string orderId)
    {
        try
        {
            var salesOrder = await _repository.GetByOrderIdAsync(orderId);
            if (salesOrder == null)
            {
                throw new KeyNotFoundException($"銷售單不存在: {orderId}");
            }

            var dto = MapToDto(salesOrder);
            var details = await _detailRepository.GetByOrderIdAsync(orderId);
            dto.Details = details.Select(MapDetailToDto).ToList();

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢銷售單失敗: {orderId}", ex);
            throw;
        }
    }

    public async Task<long> CreateSalesOrderAsync(CreateSalesOrderDto dto)
    {
        try
        {
            // 驗證銷售單號唯一性
            var exists = await _repository.ExistsByOrderIdAsync(dto.OrderId);
            if (exists)
            {
                throw new InvalidOperationException($"銷售單號已存在: {dto.OrderId}");
            }

            // 計算總金額和總數量
            decimal totalAmount = 0;
            decimal totalQty = 0;
            foreach (var detail in dto.Details)
            {
                var amount = (detail.OrderQty * (detail.UnitPrice ?? 0));
                totalAmount += amount;
                totalQty += detail.OrderQty;
            }

            var salesOrder = new SalesOrder
            {
                OrderId = dto.OrderId,
                OrderDate = dto.OrderDate,
                OrderType = dto.OrderType,
                ShopId = dto.ShopId,
                CustomerId = dto.CustomerId,
                Status = dto.Status,
                TotalAmount = totalAmount,
                TotalQty = totalQty,
                Memo = dto.Memo,
                ExpectedDate = dto.ExpectedDate,
                SiteId = dto.SiteId,
                OrgId = dto.OrgId,
                CurrencyId = dto.CurrencyId,
                ExchangeRate = dto.ExchangeRate,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var tKey = await _repository.CreateAsync(salesOrder);

            // 新增明細
            foreach (var detailDto in dto.Details)
            {
                var amount = (detailDto.OrderQty * (detailDto.UnitPrice ?? 0));
                var taxAmount = amount * (detailDto.TaxRate ?? 0) / 100;

                var detail = new SalesOrderDetail
                {
                    OrderId = dto.OrderId,
                    LineNum = detailDto.LineNum,
                    GoodsId = detailDto.GoodsId,
                    BarcodeId = detailDto.BarcodeId,
                    OrderQty = detailDto.OrderQty,
                    UnitPrice = detailDto.UnitPrice,
                    Amount = amount,
                    UnitId = detailDto.UnitId,
                    TaxRate = detailDto.TaxRate,
                    TaxAmount = taxAmount,
                    Memo = detailDto.Memo,
                    CreatedBy = GetCurrentUserId(),
                    CreatedAt = DateTime.Now,
                    UpdatedBy = GetCurrentUserId(),
                    UpdatedAt = DateTime.Now
                };

                await _detailRepository.CreateAsync(detail);
            }

            _logger.LogInfo($"新增銷售單成功: {dto.OrderId} (TKey: {tKey})");
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增銷售單失敗: {dto.OrderId}", ex);
            throw;
        }
    }

    public async Task UpdateSalesOrderAsync(UpdateSalesOrderDto dto)
    {
        try
        {
            var salesOrder = await _repository.GetByIdAsync(dto.TKey);
            if (salesOrder == null)
            {
                throw new KeyNotFoundException($"銷售單不存在: {dto.TKey}");
            }

            // 驗證銷售單號唯一性（排除自己）
            var exists = await _repository.ExistsByOrderIdAsync(dto.OrderId, dto.TKey);
            if (exists)
            {
                throw new InvalidOperationException($"銷售單號已存在: {dto.OrderId}");
            }

            // 計算總金額和總數量
            decimal totalAmount = 0;
            decimal totalQty = 0;
            foreach (var detail in dto.Details)
            {
                var amount = (detail.OrderQty * (detail.UnitPrice ?? 0));
                totalAmount += amount;
                totalQty += detail.OrderQty;
            }

            salesOrder.OrderId = dto.OrderId;
            salesOrder.OrderDate = dto.OrderDate;
            salesOrder.OrderType = dto.OrderType;
            salesOrder.ShopId = dto.ShopId;
            salesOrder.CustomerId = dto.CustomerId;
            salesOrder.Status = dto.Status;
            salesOrder.TotalAmount = totalAmount;
            salesOrder.TotalQty = totalQty;
            salesOrder.Memo = dto.Memo;
            salesOrder.ExpectedDate = dto.ExpectedDate;
            salesOrder.SiteId = dto.SiteId;
            salesOrder.OrgId = dto.OrgId;
            salesOrder.CurrencyId = dto.CurrencyId;
            salesOrder.ExchangeRate = dto.ExchangeRate;
            salesOrder.UpdatedBy = GetCurrentUserId();
            salesOrder.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(salesOrder);

            // 刪除舊明細
            await _detailRepository.DeleteByOrderIdAsync(salesOrder.OrderId);

            // 新增新明細
            foreach (var detailDto in dto.Details)
            {
                var amount = (detailDto.OrderQty * (detailDto.UnitPrice ?? 0));
                var taxAmount = amount * (detailDto.TaxRate ?? 0) / 100;

                var detail = new SalesOrderDetail
                {
                    OrderId = salesOrder.OrderId,
                    LineNum = detailDto.LineNum,
                    GoodsId = detailDto.GoodsId,
                    BarcodeId = detailDto.BarcodeId,
                    OrderQty = detailDto.OrderQty,
                    UnitPrice = detailDto.UnitPrice,
                    Amount = amount,
                    UnitId = detailDto.UnitId,
                    TaxRate = detailDto.TaxRate,
                    TaxAmount = taxAmount,
                    Memo = detailDto.Memo,
                    CreatedBy = GetCurrentUserId(),
                    CreatedAt = DateTime.Now,
                    UpdatedBy = GetCurrentUserId(),
                    UpdatedAt = DateTime.Now
                };

                await _detailRepository.CreateAsync(detail);
            }

            _logger.LogInfo($"修改銷售單成功: {dto.OrderId} (TKey: {dto.TKey})");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改銷售單失敗: {dto.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteSalesOrderAsync(long tKey)
    {
        try
        {
            var salesOrder = await _repository.GetByIdAsync(tKey);
            if (salesOrder == null)
            {
                throw new KeyNotFoundException($"銷售單不存在: {tKey}");
            }

            // 刪除明細
            await _detailRepository.DeleteByOrderIdAsync(salesOrder.OrderId);

            // 刪除主檔
            await _repository.DeleteAsync(tKey);

            _logger.LogInfo($"刪除銷售單成功: {salesOrder.OrderId} (TKey: {tKey})");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除銷售單失敗: {tKey}", ex);
            throw;
        }
    }

    /// <summary>
    /// 將 Entity 轉換為 DTO
    /// </summary>
    private SalesOrderDto MapToDto(SalesOrder salesOrder)
    {
        return new SalesOrderDto
        {
            TKey = salesOrder.TKey,
            OrderId = salesOrder.OrderId,
            OrderDate = salesOrder.OrderDate,
            OrderType = salesOrder.OrderType,
            ShopId = salesOrder.ShopId,
            CustomerId = salesOrder.CustomerId,
            Status = salesOrder.Status,
            ApplyUserId = salesOrder.ApplyUserId,
            ApplyDate = salesOrder.ApplyDate,
            ApproveUserId = salesOrder.ApproveUserId,
            ApproveDate = salesOrder.ApproveDate,
            TotalAmount = salesOrder.TotalAmount,
            TotalQty = salesOrder.TotalQty,
            Memo = salesOrder.Memo,
            ExpectedDate = salesOrder.ExpectedDate,
            SiteId = salesOrder.SiteId,
            OrgId = salesOrder.OrgId,
            CurrencyId = salesOrder.CurrencyId,
            ExchangeRate = salesOrder.ExchangeRate,
            CreatedBy = salesOrder.CreatedBy,
            CreatedAt = salesOrder.CreatedAt,
            UpdatedBy = salesOrder.UpdatedBy,
            UpdatedAt = salesOrder.UpdatedAt
        };
    }

    /// <summary>
    /// 將明細 Entity 轉換為 DTO
    /// </summary>
    private SalesOrderDetailDto MapDetailToDto(SalesOrderDetail detail)
    {
        return new SalesOrderDetailDto
        {
            TKey = detail.TKey,
            OrderId = detail.OrderId,
            LineNum = detail.LineNum,
            GoodsId = detail.GoodsId,
            BarcodeId = detail.BarcodeId,
            OrderQty = detail.OrderQty,
            UnitPrice = detail.UnitPrice,
            Amount = detail.Amount,
            ShippedQty = detail.ShippedQty,
            ReturnQty = detail.ReturnQty,
            UnitId = detail.UnitId,
            TaxRate = detail.TaxRate,
            TaxAmount = detail.TaxAmount,
            Memo = detail.Memo
        };
    }
}

