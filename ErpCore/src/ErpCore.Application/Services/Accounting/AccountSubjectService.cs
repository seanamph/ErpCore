using ErpCore.Application.DTOs.Accounting;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Accounting;
using ErpCore.Infrastructure.Repositories.Accounting;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Accounting;

/// <summary>
/// 會計科目服務實作 (SYSN110)
/// </summary>
public class AccountSubjectService : BaseService, IAccountSubjectService
{
    private readonly IAccountSubjectRepository _repository;

    public AccountSubjectService(
        IAccountSubjectRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<AccountSubjectDto>> GetAccountSubjectsAsync(AccountSubjectQueryDto query)
    {
        try
        {
            var repositoryQuery = new AccountSubjectQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                StypeId = query.StypeId,
                StypeName = query.StypeName,
                Dc = query.Dc,
                LedgerMd = query.LedgerMd,
                VoucherType = query.VoucherType,
                BudgetYn = query.BudgetYn,
                StypeClass = query.StypeClass
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<AccountSubjectDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢會計科目列表失敗", ex);
            throw;
        }
    }

    public async Task<AccountSubjectDto> GetAccountSubjectByIdAsync(string stypeId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(stypeId);
            if (entity == null)
            {
                throw new InvalidOperationException($"會計科目不存在: {stypeId}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢會計科目失敗: {stypeId}", ex);
            throw;
        }
    }

    public async Task<string> CreateAccountSubjectAsync(CreateAccountSubjectDto dto)
    {
        try
        {
            // 驗證必填欄位
            if (string.IsNullOrWhiteSpace(dto.StypeId))
            {
                throw new ArgumentException("科目代號不能為空");
            }

            if (string.IsNullOrWhiteSpace(dto.StypeName))
            {
                throw new ArgumentException("科目名稱不能為空");
            }

            // 檢查科目代號是否已存在
            var exists = await _repository.ExistsAsync(dto.StypeId);
            if (exists)
            {
                throw new InvalidOperationException($"科目代號已存在: {dto.StypeId}");
            }

            var entity = new AccountSubject
            {
                StypeId = dto.StypeId,
                StypeName = dto.StypeName,
                StypeNameE = dto.StypeNameE,
                Dc = dto.Dc,
                LedgerMd = dto.LedgerMd,
                MtypeId = dto.MtypeId,
                AbatYn = dto.AbatYn ?? "N",
                VoucherType = dto.VoucherType,
                BudgetYn = dto.BudgetYn ?? "N",
                OrgYn = dto.OrgYn ?? "N",
                ExpYear = dto.ExpYear,
                ResiValue = dto.ResiValue,
                DepreLid = dto.DepreLid,
                AccudepreLid = dto.AccudepreLid,
                StypeYn = dto.StypeYn ?? "Y",
                IfrsStypeId = dto.IfrsStypeId,
                RocStypeId = dto.RocStypeId,
                SapStypeId = dto.SapStypeId,
                StypeClass = dto.StypeClass,
                StypeOrder = dto.StypeOrder,
                Amt0 = dto.Amt0 ?? 0,
                Amt1 = dto.Amt1 ?? 0,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(entity);

            return entity.StypeId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增會計科目失敗: {dto.StypeId}", ex);
            throw;
        }
    }

    public async Task UpdateAccountSubjectAsync(string stypeId, UpdateAccountSubjectDto dto)
    {
        try
        {
            // 驗證必填欄位
            if (string.IsNullOrWhiteSpace(dto.StypeName))
            {
                throw new ArgumentException("科目名稱不能為空");
            }

            // 檢查會計科目是否存在
            var existing = await _repository.GetByIdAsync(stypeId);
            if (existing == null)
            {
                throw new InvalidOperationException($"會計科目不存在: {stypeId}");
            }

            existing.StypeName = dto.StypeName;
            existing.StypeNameE = dto.StypeNameE;
            existing.Dc = dto.Dc;
            existing.LedgerMd = dto.LedgerMd;
            existing.MtypeId = dto.MtypeId;
            existing.AbatYn = dto.AbatYn;
            existing.VoucherType = dto.VoucherType;
            existing.BudgetYn = dto.BudgetYn;
            existing.OrgYn = dto.OrgYn;
            existing.ExpYear = dto.ExpYear;
            existing.ResiValue = dto.ResiValue;
            existing.DepreLid = dto.DepreLid;
            existing.AccudepreLid = dto.AccudepreLid;
            existing.StypeYn = dto.StypeYn;
            existing.IfrsStypeId = dto.IfrsStypeId;
            existing.RocStypeId = dto.RocStypeId;
            existing.SapStypeId = dto.SapStypeId;
            existing.StypeClass = dto.StypeClass;
            existing.StypeOrder = dto.StypeOrder;
            existing.Amt0 = dto.Amt0;
            existing.Amt1 = dto.Amt1;
            existing.UpdatedBy = GetCurrentUserId();
            existing.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(existing);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改會計科目失敗: {stypeId}", ex);
            throw;
        }
    }

    public async Task DeleteAccountSubjectAsync(string stypeId)
    {
        try
        {
            // 檢查會計科目是否存在
            var existing = await _repository.GetByIdAsync(stypeId);
            if (existing == null)
            {
                throw new InvalidOperationException($"會計科目不存在: {stypeId}");
            }

            // 檢查是否有未沖帳餘額
            var balance = await _repository.GetUnsettledBalanceAsync(stypeId);
            if (balance != 0)
            {
                throw new InvalidOperationException($"會計科目有未沖帳餘額，無法刪除: {stypeId}, 餘額: {balance}");
            }

            await _repository.DeleteAsync(stypeId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除會計科目失敗: {stypeId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string stypeId)
    {
        try
        {
            return await _repository.ExistsAsync(stypeId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查會計科目是否存在失敗: {stypeId}", ex);
            throw;
        }
    }

    public async Task<UnsettledBalanceDto> CheckUnsettledBalanceAsync(string stypeId)
    {
        try
        {
            var balance = await _repository.GetUnsettledBalanceAsync(stypeId);
            return new UnsettledBalanceDto
            {
                HasUnsettledBalance = balance != 0,
                Balance = balance
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查未沖帳餘額失敗: {stypeId}", ex);
            throw;
        }
    }

    private AccountSubjectDto MapToDto(AccountSubject entity)
    {
        return new AccountSubjectDto
        {
            TKey = entity.TKey,
            StypeId = entity.StypeId,
            StypeName = entity.StypeName,
            StypeNameE = entity.StypeNameE,
            Dc = entity.Dc,
            LedgerMd = entity.LedgerMd,
            MtypeId = entity.MtypeId,
            AbatYn = entity.AbatYn,
            VoucherType = entity.VoucherType,
            BudgetYn = entity.BudgetYn,
            OrgYn = entity.OrgYn,
            ExpYear = entity.ExpYear,
            ResiValue = entity.ResiValue,
            DepreLid = entity.DepreLid,
            AccudepreLid = entity.AccudepreLid,
            StypeYn = entity.StypeYn,
            IfrsStypeId = entity.IfrsStypeId,
            RocStypeId = entity.RocStypeId,
            SapStypeId = entity.SapStypeId,
            StypeClass = entity.StypeClass,
            StypeOrder = entity.StypeOrder,
            Amt0 = entity.Amt0,
            Amt1 = entity.Amt1,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt,
            CreatedPriority = entity.CreatedPriority,
            CreatedGroup = entity.CreatedGroup
        };
    }
}

