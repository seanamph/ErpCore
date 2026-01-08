using ErpCore.Application.DTOs.ReportManagement;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.ReportManagement;
using ErpCore.Infrastructure.Repositories.ReportManagement;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.ReportManagement;

/// <summary>
/// 應收帳款服務實作 (SYSR210-SYSR240)
/// </summary>
public class AccountsReceivableService : BaseService, IAccountsReceivableService
{
    private readonly IAccountsReceivableRepository _repository;

    public AccountsReceivableService(
        IAccountsReceivableRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<AccountsReceivableDto>> GetAllAsync()
    {
        try
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢應收帳款列表失敗", ex);
            throw;
        }
    }

    public async Task<AccountsReceivableDto> GetByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"應收帳款不存在: {tKey}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢應收帳款失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<AccountsReceivableDto>> GetByReceiptDateRangeAsync(DateTime? startDate, DateTime? endDate)
    {
        try
        {
            var entities = await _repository.GetByReceiptDateRangeAsync(startDate, endDate);
            return entities.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢應收帳款列表失敗: StartDate={startDate}, EndDate={endDate}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<AccountsReceivableDto>> GetByVoucherNoAsync(string voucherNo)
    {
        try
        {
            var entities = await _repository.GetByVoucherNoAsync(voucherNo);
            return entities.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢應收帳款列表失敗: VoucherNo={voucherNo}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<AccountsReceivableDto>> GetByReceiptNoAsync(string receiptNo)
    {
        try
        {
            var entities = await _repository.GetByReceiptNoAsync(receiptNo);
            return entities.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢應收帳款列表失敗: ReceiptNo={receiptNo}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<AccountsReceivableDto>> GetByObjectIdAsync(string objectId)
    {
        try
        {
            var entities = await _repository.GetByObjectIdAsync(objectId);
            return entities.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢應收帳款列表失敗: ObjectId={objectId}", ex);
            throw;
        }
    }

    public async Task<AccountsReceivableDto> CreateAsync(CreateAccountsReceivableDto dto)
    {
        try
        {
            var entity = new AccountsReceivable
            {
                VoucherM_TKey = dto.VoucherM_TKey,
                ObjectId = dto.ObjectId,
                AcctKey = dto.AcctKey,
                ReceiptDate = dto.ReceiptDate,
                ReceiptAmount = dto.ReceiptAmount,
                AritemId = dto.AritemId,
                ReceiptNo = dto.ReceiptNo,
                VoucherNo = dto.VoucherNo,
                VoucherStatus = dto.VoucherStatus,
                ShopId = dto.ShopId,
                SiteId = dto.SiteId,
                OrgId = dto.OrgId,
                CurrencyId = dto.CurrencyId ?? "TWD",
                ExchangeRate = dto.ExchangeRate == 0 ? 1 : dto.ExchangeRate,
                Notes = dto.Notes,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var result = await _repository.CreateAsync(entity);
            return MapToDto(result);
        }
        catch (Exception ex)
        {
            _logger.LogError("新增應收帳款失敗", ex);
            throw;
        }
    }

    public async Task<AccountsReceivableDto> UpdateAsync(long tKey, UpdateAccountsReceivableDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"應收帳款不存在: {tKey}");
            }

            entity.ReceiptDate = dto.ReceiptDate;
            entity.ReceiptAmount = dto.ReceiptAmount;
            entity.AritemId = dto.AritemId;
            entity.ReceiptNo = dto.ReceiptNo;
            entity.VoucherNo = dto.VoucherNo;
            entity.VoucherStatus = dto.VoucherStatus;
            entity.CurrencyId = dto.CurrencyId;
            entity.ExchangeRate = dto.ExchangeRate;
            entity.Notes = dto.Notes;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            var result = await _repository.UpdateAsync(entity);
            return MapToDto(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新應收帳款失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"應收帳款不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除應收帳款失敗: {tKey}", ex);
            throw;
        }
    }

    private AccountsReceivableDto MapToDto(AccountsReceivable entity)
    {
        return new AccountsReceivableDto
        {
            TKey = entity.TKey,
            VoucherM_TKey = entity.VoucherM_TKey,
            ObjectId = entity.ObjectId,
            AcctKey = entity.AcctKey,
            ReceiptDate = entity.ReceiptDate,
            ReceiptAmount = entity.ReceiptAmount,
            AritemId = entity.AritemId,
            ReceiptNo = entity.ReceiptNo,
            VoucherNo = entity.VoucherNo,
            VoucherStatus = entity.VoucherStatus,
            ShopId = entity.ShopId,
            SiteId = entity.SiteId,
            OrgId = entity.OrgId,
            CurrencyId = entity.CurrencyId,
            ExchangeRate = entity.ExchangeRate,
            Notes = entity.Notes,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

