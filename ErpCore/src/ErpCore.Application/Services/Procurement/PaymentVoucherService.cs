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
public class PaymentVoucherService : BaseService, IPaymentVoucherService
{
    private readonly IPaymentVoucherRepository _repository;
    private readonly ISupplierRepository _supplierRepository;

    public PaymentVoucherService(
        IPaymentVoucherRepository repository,
        ISupplierRepository supplierRepository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _supplierRepository = supplierRepository;
    }

    public async Task<PagedResult<PaymentVoucherDto>> GetPaymentVouchersAsync(PaymentVoucherQueryDto query)
    {
        try
        {
            var repositoryQuery = new PaymentVoucherQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                PaymentNo = query.PaymentNo,
                PaymentDateFrom = query.PaymentDateFrom,
                PaymentDateTo = query.PaymentDateTo,
                SupplierId = query.SupplierId,
                PaymentMethod = query.PaymentMethod,
                Status = query.Status
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(x => MapToDto(x)).ToList();

            // 補充供應商名稱
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

            return new PagedResult<PaymentVoucherDto>
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

    public async Task<PaymentVoucherDto> GetPaymentVoucherByIdAsync(string paymentNo)
    {
        try
        {
            var paymentVoucher = await _repository.GetByIdAsync(paymentNo);
            if (paymentVoucher == null)
            {
                throw new InvalidOperationException($"付款單不存在: {paymentNo}");
            }

            var dto = MapToDto(paymentVoucher);

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
            _logger.LogError($"查詢付款單失敗: {paymentNo}", ex);
            throw;
        }
    }

    public async Task<string> CreatePaymentVoucherAsync(CreatePaymentVoucherDto dto)
    {
        try
        {
            // 檢查付款單號是否已存在
            if (await _repository.ExistsAsync(dto.PaymentNo))
            {
                throw new InvalidOperationException($"付款單號已存在: {dto.PaymentNo}");
            }

            // 檢查供應商是否存在
            if (!await _supplierRepository.ExistsAsync(dto.SupplierId))
            {
                throw new InvalidOperationException($"供應商不存在: {dto.SupplierId}");
            }

            var paymentVoucher = new PaymentVoucher
            {
                PaymentNo = dto.PaymentNo,
                PaymentDate = dto.PaymentDate,
                SupplierId = dto.SupplierId,
                PaymentAmount = dto.PaymentAmount,
                PaymentMethod = dto.PaymentMethod,
                BankAccount = dto.BankAccount,
                Status = dto.Status ?? "DRAFT",
                Notes = dto.Notes,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(paymentVoucher);

            _logger.LogInfo($"新增付款單成功: {dto.PaymentNo}");
            return paymentVoucher.PaymentNo;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增付款單失敗: {dto.PaymentNo}", ex);
            throw;
        }
    }

    public async Task UpdatePaymentVoucherAsync(string paymentNo, UpdatePaymentVoucherDto dto)
    {
        try
        {
            var paymentVoucher = await _repository.GetByIdAsync(paymentNo);
            if (paymentVoucher == null)
            {
                throw new InvalidOperationException($"付款單不存在: {paymentNo}");
            }

            // 僅草稿狀態的資料可修改
            if (paymentVoucher.Status != "DRAFT")
            {
                throw new InvalidOperationException($"僅草稿狀態的付款單可修改: {paymentNo}");
            }

            // 檢查供應商是否存在
            if (!await _supplierRepository.ExistsAsync(dto.SupplierId))
            {
                throw new InvalidOperationException($"供應商不存在: {dto.SupplierId}");
            }

            paymentVoucher.PaymentDate = dto.PaymentDate;
            paymentVoucher.SupplierId = dto.SupplierId;
            paymentVoucher.PaymentAmount = dto.PaymentAmount;
            paymentVoucher.PaymentMethod = dto.PaymentMethod;
            paymentVoucher.BankAccount = dto.BankAccount;
            paymentVoucher.Notes = dto.Notes;
            paymentVoucher.UpdatedBy = GetCurrentUserId();
            paymentVoucher.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(paymentVoucher);

            _logger.LogInfo($"修改付款單成功: {paymentNo}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改付款單失敗: {paymentNo}", ex);
            throw;
        }
    }

    public async Task DeletePaymentVoucherAsync(string paymentNo)
    {
        try
        {
            var paymentVoucher = await _repository.GetByIdAsync(paymentNo);
            if (paymentVoucher == null)
            {
                throw new InvalidOperationException($"付款單不存在: {paymentNo}");
            }

            // 僅草稿狀態的資料可刪除
            if (paymentVoucher.Status != "DRAFT")
            {
                throw new InvalidOperationException($"僅草稿狀態的付款單可刪除: {paymentNo}");
            }

            await _repository.DeleteAsync(paymentNo);

            _logger.LogInfo($"刪除付款單成功: {paymentNo}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除付款單失敗: {paymentNo}", ex);
            throw;
        }
    }

    public async Task ConfirmPaymentVoucherAsync(string paymentNo)
    {
        try
        {
            var paymentVoucher = await _repository.GetByIdAsync(paymentNo);
            if (paymentVoucher == null)
            {
                throw new InvalidOperationException($"付款單不存在: {paymentNo}");
            }

            // 僅草稿狀態的資料可確認
            if (paymentVoucher.Status != "DRAFT")
            {
                throw new InvalidOperationException($"僅草稿狀態的付款單可確認: {paymentNo}");
            }

            paymentVoucher.Status = "CONFIRMED";
            paymentVoucher.Verifier = GetCurrentUserId();
            paymentVoucher.VerifyDate = DateTime.Now;
            paymentVoucher.UpdatedBy = GetCurrentUserId();
            paymentVoucher.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(paymentVoucher);

            _logger.LogInfo($"確認付款單成功: {paymentNo}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"確認付款單失敗: {paymentNo}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string paymentNo)
    {
        try
        {
            return await _repository.ExistsAsync(paymentNo);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查付款單是否存在失敗: {paymentNo}", ex);
            throw;
        }
    }

    private PaymentVoucherDto MapToDto(PaymentVoucher paymentVoucher)
    {
        return new PaymentVoucherDto
        {
            TKey = paymentVoucher.TKey,
            PaymentNo = paymentVoucher.PaymentNo,
            PaymentDate = paymentVoucher.PaymentDate,
            SupplierId = paymentVoucher.SupplierId,
            PaymentAmount = paymentVoucher.PaymentAmount,
            PaymentMethod = paymentVoucher.PaymentMethod,
            BankAccount = paymentVoucher.BankAccount,
            Status = paymentVoucher.Status,
            Verifier = paymentVoucher.Verifier,
            VerifyDate = paymentVoucher.VerifyDate,
            Notes = paymentVoucher.Notes,
            CreatedBy = paymentVoucher.CreatedBy,
            CreatedAt = paymentVoucher.CreatedAt,
            UpdatedBy = paymentVoucher.UpdatedBy,
            UpdatedAt = paymentVoucher.UpdatedAt
        };
    }
}
