using ErpCore.Application.DTOs.Sales;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Sales;
using ErpCore.Infrastructure.Repositories.Sales;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Sales;

/// <summary>
/// 銷售單服務實作 (SYSD110-SYSD140)
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
                OrderId = query.OrderId,
                OrderType = query.OrderType,
                ShopId = query.ShopId,
                CustomerId = query.CustomerId,
                Status = query.Status,
                OrderDateFrom = query.OrderDateFrom,
                OrderDateTo = query.OrderDateTo
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<SalesOrderDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢銷售單列表失敗", ex);
            throw;
        }
    }

    public async Task<SalesOrderDto> GetSalesOrderByIdAsync(string orderId)
    {
        try
        {
            var salesOrder = await _repository.GetByIdAsync(orderId);
            if (salesOrder == null)
            {
                throw new InvalidOperationException($"銷售單不存在: {orderId}");
            }

            var dto = MapToDto(salesOrder);
            var details = await _detailRepository.GetByOrderIdAsync(orderId);
            dto.Details = details.Select(x => MapDetailToDto(x)).ToList();

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢銷售單失敗: {orderId}", ex);
            throw;
        }
    }

    public async Task<string> CreateSalesOrderAsync(CreateSalesOrderDto dto)
    {
        try
        {
            // 檢查銷售單號是否已存在
            if (await _repository.ExistsAsync(dto.OrderId))
            {
                throw new InvalidOperationException($"銷售單號已存在: {dto.OrderId}");
            }

            // 計算總金額和總數量
            decimal totalAmount = 0;
            decimal totalQty = 0;
            decimal totalDiscountAmount = 0;
            decimal totalTaxAmount = 0;

            foreach (var detail in dto.Details)
            {
                var amount = (detail.OrderQty * (detail.UnitPrice ?? 0));
                var discountAmount = amount * (detail.DiscountRate ?? 0) / 100;
                var taxAmount = (amount - discountAmount) * (detail.TaxRate ?? 0) / 100;

                totalAmount += amount - discountAmount + taxAmount;
                totalQty += detail.OrderQty;
                totalDiscountAmount += discountAmount;
                totalTaxAmount += taxAmount;
            }

            var salesOrder = new SalesOrder
            {
                OrderId = dto.OrderId,
                OrderDate = dto.OrderDate,
                OrderType = dto.OrderType,
                ShopId = dto.ShopId,
                CustomerId = dto.CustomerId,
                Status = dto.Status,
                ExpectedDate = dto.ExpectedDate,
                Memo = dto.Memo,
                SiteId = dto.SiteId,
                OrgId = dto.OrgId,
                CurrencyId = dto.CurrencyId ?? "TWD",
                ExchangeRate = dto.ExchangeRate ?? 1,
                TotalAmount = totalAmount,
                TotalQty = totalQty,
                DiscountAmount = totalDiscountAmount,
                TaxAmount = totalTaxAmount,
                CreatedBy = _userContext.UserId,
                CreatedAt = DateTime.Now,
                UpdatedBy = _userContext.UserId,
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(salesOrder);

            // 建立明細
            foreach (var detailDto in dto.Details)
            {
                var amount = (detailDto.OrderQty * (detailDto.UnitPrice ?? 0));
                var discountAmount = amount * (detailDto.DiscountRate ?? 0) / 100;
                var taxAmount = (amount - discountAmount) * (detailDto.TaxRate ?? 0) / 100;

                var detail = new SalesOrderDetail
                {
                    OrderId = dto.OrderId,
                    LineNum = detailDto.LineNum,
                    GoodsId = detailDto.GoodsId,
                    BarcodeId = detailDto.BarcodeId,
                    OrderQty = detailDto.OrderQty,
                    UnitPrice = detailDto.UnitPrice,
                    Amount = amount - discountAmount + taxAmount,
                    UnitId = detailDto.UnitId,
                    DiscountRate = detailDto.DiscountRate ?? 0,
                    DiscountAmount = discountAmount,
                    TaxRate = detailDto.TaxRate ?? 0,
                    TaxAmount = taxAmount,
                    Memo = detailDto.Memo,
                    CreatedBy = _userContext.UserId,
                    CreatedAt = DateTime.Now,
                    UpdatedBy = _userContext.UserId,
                    UpdatedAt = DateTime.Now
                };

                await _detailRepository.CreateAsync(detail);
            }

            return dto.OrderId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增銷售單失敗: {dto.OrderId}", ex);
            throw;
        }
    }

    public async Task UpdateSalesOrderAsync(string orderId, UpdateSalesOrderDto dto)
    {
        try
        {
            var salesOrder = await _repository.GetByIdAsync(orderId);
            if (salesOrder == null)
            {
                throw new InvalidOperationException($"銷售單不存在: {orderId}");
            }

            // 只有草稿狀態的銷售單可以修改
            if (salesOrder.Status != "D")
            {
                throw new InvalidOperationException($"只有草稿狀態的銷售單可以修改: {orderId}");
            }

            // 計算總金額和總數量
            decimal totalAmount = 0;
            decimal totalQty = 0;
            decimal totalDiscountAmount = 0;
            decimal totalTaxAmount = 0;

            foreach (var detail in dto.Details)
            {
                var amount = (detail.OrderQty * (detail.UnitPrice ?? 0));
                var discountAmount = amount * (detail.DiscountRate ?? 0) / 100;
                var taxAmount = (amount - discountAmount) * (detail.TaxRate ?? 0) / 100;

                totalAmount += amount - discountAmount + taxAmount;
                totalQty += detail.OrderQty;
                totalDiscountAmount += discountAmount;
                totalTaxAmount += taxAmount;
            }

            salesOrder.OrderDate = dto.OrderDate;
            salesOrder.OrderType = dto.OrderType;
            salesOrder.ShopId = dto.ShopId;
            salesOrder.CustomerId = dto.CustomerId;
            salesOrder.Status = dto.Status;
            salesOrder.ExpectedDate = dto.ExpectedDate;
            salesOrder.Memo = dto.Memo;
            salesOrder.SiteId = dto.SiteId;
            salesOrder.OrgId = dto.OrgId;
            salesOrder.CurrencyId = dto.CurrencyId ?? "TWD";
            salesOrder.ExchangeRate = dto.ExchangeRate ?? 1;
            salesOrder.TotalAmount = totalAmount;
            salesOrder.TotalQty = totalQty;
            salesOrder.DiscountAmount = totalDiscountAmount;
            salesOrder.TaxAmount = totalTaxAmount;
            salesOrder.UpdatedBy = _userContext.UserId;
            salesOrder.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(salesOrder);

            // 刪除舊明細
            await _detailRepository.DeleteByOrderIdAsync(orderId);

            // 建立新明細
            foreach (var detailDto in dto.Details)
            {
                var amount = (detailDto.OrderQty * (detailDto.UnitPrice ?? 0));
                var discountAmount = amount * (detailDto.DiscountRate ?? 0) / 100;
                var taxAmount = (amount - discountAmount) * (detailDto.TaxRate ?? 0) / 100;

                var detail = new SalesOrderDetail
                {
                    OrderId = orderId,
                    LineNum = detailDto.LineNum,
                    GoodsId = detailDto.GoodsId,
                    BarcodeId = detailDto.BarcodeId,
                    OrderQty = detailDto.OrderQty,
                    UnitPrice = detailDto.UnitPrice,
                    Amount = amount - discountAmount + taxAmount,
                    UnitId = detailDto.UnitId,
                    DiscountRate = detailDto.DiscountRate ?? 0,
                    DiscountAmount = discountAmount,
                    TaxRate = detailDto.TaxRate ?? 0,
                    TaxAmount = taxAmount,
                    Memo = detailDto.Memo,
                    CreatedBy = _userContext.UserId,
                    CreatedAt = DateTime.Now,
                    UpdatedBy = _userContext.UserId,
                    UpdatedAt = DateTime.Now
                };

                await _detailRepository.CreateAsync(detail);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改銷售單失敗: {orderId}", ex);
            throw;
        }
    }

    public async Task DeleteSalesOrderAsync(string orderId)
    {
        try
        {
            var salesOrder = await _repository.GetByIdAsync(orderId);
            if (salesOrder == null)
            {
                throw new InvalidOperationException($"銷售單不存在: {orderId}");
            }

            // 只有草稿狀態的銷售單可以刪除
            if (salesOrder.Status != "D")
            {
                throw new InvalidOperationException($"只有草稿狀態的銷售單可以刪除: {orderId}");
            }

            await _repository.DeleteAsync(orderId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除銷售單失敗: {orderId}", ex);
            throw;
        }
    }

    public async Task ApproveSalesOrderAsync(string orderId, ApproveSalesOrderDto dto)
    {
        try
        {
            var salesOrder = await _repository.GetByIdAsync(orderId);
            if (salesOrder == null)
            {
                throw new InvalidOperationException($"銷售單不存在: {orderId}");
            }

            // 只有已送出狀態的銷售單可以審核
            if (salesOrder.Status != "S")
            {
                throw new InvalidOperationException($"只有已送出狀態的銷售單可以審核: {orderId}");
            }

            salesOrder.Status = "A";
            salesOrder.ApproveUserId = dto.ApproveUserId;
            salesOrder.ApproveDate = DateTime.Now;
            salesOrder.UpdatedBy = _userContext.UserId;
            salesOrder.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(salesOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError($"審核銷售單失敗: {orderId}", ex);
            throw;
        }
    }

    public async Task ShipSalesOrderAsync(string orderId, ShipSalesOrderDto dto)
    {
        try
        {
            var salesOrder = await _repository.GetByIdAsync(orderId);
            if (salesOrder == null)
            {
                throw new InvalidOperationException($"銷售單不存在: {orderId}");
            }

            // 只有已審核狀態的銷售單可以出貨
            if (salesOrder.Status != "A")
            {
                throw new InvalidOperationException($"只有已審核狀態的銷售單可以出貨: {orderId}");
            }

            salesOrder.Status = "O";
            salesOrder.ShipDate = dto.ShipDate;
            salesOrder.UpdatedBy = _userContext.UserId;
            salesOrder.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(salesOrder);

            // 更新明細出貨數量
            foreach (var shipDetail in dto.Details)
            {
                var detail = await _detailRepository.GetByOrderIdAndLineNumAsync(orderId, shipDetail.LineNum);
                if (detail != null)
                {
                    if (shipDetail.ShippedQty > detail.OrderQty)
                    {
                        throw new InvalidOperationException($"出貨數量不能超過訂購數量: LineNum: {shipDetail.LineNum}");
                    }

                    detail.ShippedQty = shipDetail.ShippedQty;
                    detail.UpdatedBy = _userContext.UserId;
                    detail.UpdatedAt = DateTime.Now;

                    await _detailRepository.UpdateAsync(detail);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"出貨銷售單失敗: {orderId}", ex);
            throw;
        }
    }

    public async Task CancelSalesOrderAsync(string orderId, CancelSalesOrderDto dto)
    {
        try
        {
            var salesOrder = await _repository.GetByIdAsync(orderId);
            if (salesOrder == null)
            {
                throw new InvalidOperationException($"銷售單不存在: {orderId}");
            }

            // 只有草稿或已送出狀態的銷售單可以取消
            if (salesOrder.Status != "D" && salesOrder.Status != "S")
            {
                throw new InvalidOperationException($"只有草稿或已送出狀態的銷售單可以取消: {orderId}");
            }

            salesOrder.Status = "X";
            salesOrder.Memo = dto.Memo;
            salesOrder.UpdatedBy = _userContext.UserId;
            salesOrder.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(salesOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError($"取消銷售單失敗: {orderId}", ex);
            throw;
        }
    }

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
            ShipDate = salesOrder.ShipDate,
            TotalAmount = salesOrder.TotalAmount,
            TotalQty = salesOrder.TotalQty,
            DiscountAmount = salesOrder.DiscountAmount,
            TaxAmount = salesOrder.TaxAmount,
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
            DiscountRate = detail.DiscountRate,
            DiscountAmount = detail.DiscountAmount,
            TaxRate = detail.TaxRate,
            TaxAmount = detail.TaxAmount,
            Memo = detail.Memo
        };
    }
}

