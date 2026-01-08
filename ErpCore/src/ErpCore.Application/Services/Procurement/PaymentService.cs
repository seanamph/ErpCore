using ErpCore.Application.DTOs.Procurement;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Procurement;
using ErpCore.Infrastructure.Repositories.Procurement;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Procurement;

/// <summary>
/// 付款單服務實作 (SYSP271-SYSP2B0)
/// </summary>
public class PaymentService : BaseService, IPaymentService
{
    private readonly IPaymentRepository _repository;
    private readonly ISupplierRepository _supplierRepository;

    public PaymentService(
        IPaymentRepository repository,
        ISupplierRepository supplierRepository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _supplierRepository = supplierRepository;
    }

    public async Task<PagedResult<PaymentDto>> GetPaymentsAsync(PaymentQueryDto query)
    {
        try
        {
            var repositoryQuery = new PaymentQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                PaymentId = query.PaymentId,
                PaymentDateFrom = query.PaymentDateFrom,
                PaymentDateTo = query.PaymentDateTo,
                SupplierId = query.SupplierId,
                PaymentType = query.PaymentType,
                Status = query.Status
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(x => MapToDto(x)).ToList();

            // 補充供應商名稱和銀行帳戶名稱
            foreach (var dto in dtos)
            {
                if (!string.IsNullOrEmpty(dto.SupplierId))
                {
                    var supplier = await _supplierRepository.GetByIdAsync(dto.SupplierId);
                    if (supplier != null)
                    {
                        dto.SupplierName = supplier.SupplierName;
                    }
                }
            }

            return new PagedResult<PaymentDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢付款單列表失敗", ex);
            throw;
        }
    }

    public async Task<PaymentDto> GetPaymentByIdAsync(string paymentId)
    {
        try
        {
            var payment = await _repository.GetByIdAsync(paymentId);
            if (payment == null)
            {
                throw new InvalidOperationException($"付款單不存在: {paymentId}");
            }

            var dto = MapToDto(payment);

            // 補充供應商名稱
            if (!string.IsNullOrEmpty(dto.SupplierId))
            {
                var supplier = await _supplierRepository.GetByIdAsync(dto.SupplierId);
                if (supplier != null)
                {
                    dto.SupplierName = supplier.SupplierName;
                }
            }

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢付款單失敗: {paymentId}", ex);
            throw;
        }
    }

    public async Task<string> CreatePaymentAsync(CreatePaymentDto dto)
    {
        try
        {
            // 檢查付款單號是否已存在
            if (await _repository.ExistsAsync(dto.PaymentId))
            {
                throw new InvalidOperationException($"付款單號已存在: {dto.PaymentId}");
            }

            // 檢查供應商是否存在
            if (!await _supplierRepository.ExistsAsync(dto.SupplierId))
            {
                throw new InvalidOperationException($"供應商不存在: {dto.SupplierId}");
            }

            var payment = new Payment
            {
                PaymentId = dto.PaymentId,
                PaymentDate = dto.PaymentDate,
                SupplierId = dto.SupplierId,
                PaymentType = dto.PaymentType,
                Amount = dto.Amount,
                CurrencyId = dto.CurrencyId ?? "TWD",
                ExchangeRate = dto.ExchangeRate ?? 1,
                BankAccountId = dto.BankAccountId,
                CheckNumber = dto.CheckNumber,
                Status = dto.Status,
                Memo = dto.Memo,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(payment);

            _logger.LogInfo($"新增付款單成功: {dto.PaymentId}");
            return payment.PaymentId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增付款單失敗: {dto.PaymentId}", ex);
            throw;
        }
    }

    public async Task UpdatePaymentAsync(string paymentId, UpdatePaymentDto dto)
    {
        try
        {
            var payment = await _repository.GetByIdAsync(paymentId);
            if (payment == null)
            {
                throw new InvalidOperationException($"付款單不存在: {paymentId}");
            }

            // 僅草稿狀態的資料可修改
            if (payment.Status != "D")
            {
                throw new InvalidOperationException($"僅草稿狀態的付款單可修改: {paymentId}");
            }

            // 檢查供應商是否存在
            if (!await _supplierRepository.ExistsAsync(dto.SupplierId))
            {
                throw new InvalidOperationException($"供應商不存在: {dto.SupplierId}");
            }

            payment.PaymentDate = dto.PaymentDate;
            payment.SupplierId = dto.SupplierId;
            payment.PaymentType = dto.PaymentType;
            payment.Amount = dto.Amount;
            payment.CurrencyId = dto.CurrencyId ?? "TWD";
            payment.ExchangeRate = dto.ExchangeRate ?? 1;
            payment.BankAccountId = dto.BankAccountId;
            payment.CheckNumber = dto.CheckNumber;
            payment.Status = dto.Status;
            payment.Memo = dto.Memo;
            payment.UpdatedBy = GetCurrentUserId();
            payment.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(payment);

            _logger.LogInfo($"修改付款單成功: {paymentId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改付款單失敗: {paymentId}", ex);
            throw;
        }
    }

    public async Task DeletePaymentAsync(string paymentId)
    {
        try
        {
            var payment = await _repository.GetByIdAsync(paymentId);
            if (payment == null)
            {
                throw new InvalidOperationException($"付款單不存在: {paymentId}");
            }

            // 僅草稿狀態的資料可刪除
            if (payment.Status != "D")
            {
                throw new InvalidOperationException($"僅草稿狀態的付款單可刪除: {paymentId}");
            }

            await _repository.DeleteAsync(paymentId);

            _logger.LogInfo($"刪除付款單成功: {paymentId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除付款單失敗: {paymentId}", ex);
            throw;
        }
    }

    public async Task ConfirmPaymentAsync(string paymentId)
    {
        try
        {
            var payment = await _repository.GetByIdAsync(paymentId);
            if (payment == null)
            {
                throw new InvalidOperationException($"付款單不存在: {paymentId}");
            }

            // 僅草稿狀態的資料可確認
            if (payment.Status != "D")
            {
                throw new InvalidOperationException($"僅草稿狀態的付款單可確認: {paymentId}");
            }

            payment.Status = "A"; // A:已確認
            payment.UpdatedBy = GetCurrentUserId();
            payment.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(payment);

            _logger.LogInfo($"確認付款單成功: {paymentId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"確認付款單失敗: {paymentId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string paymentId)
    {
        try
        {
            return await _repository.ExistsAsync(paymentId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查付款單是否存在失敗: {paymentId}", ex);
            throw;
        }
    }

    private PaymentDto MapToDto(Payment payment)
    {
        return new PaymentDto
        {
            TKey = payment.TKey,
            PaymentId = payment.PaymentId,
            PaymentDate = payment.PaymentDate,
            SupplierId = payment.SupplierId,
            PaymentType = payment.PaymentType,
            Amount = payment.Amount,
            CurrencyId = payment.CurrencyId,
            ExchangeRate = payment.ExchangeRate,
            BankAccountId = payment.BankAccountId,
            CheckNumber = payment.CheckNumber,
            Status = payment.Status,
            Memo = payment.Memo,
            CreatedBy = payment.CreatedBy,
            CreatedAt = payment.CreatedAt,
            UpdatedBy = payment.UpdatedBy,
            UpdatedAt = payment.UpdatedAt
        };
    }
}

