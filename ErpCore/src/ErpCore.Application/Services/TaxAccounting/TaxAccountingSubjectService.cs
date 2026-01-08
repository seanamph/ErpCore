using ErpCore.Application.DTOs.Accounting;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Accounting;
using ErpCore.Infrastructure.Repositories.TaxAccounting;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.TaxAccounting;

/// <summary>
/// 會計科目服務實作 (SYST111-SYST11A)
/// </summary>
public class TaxAccountingSubjectService : BaseService, ITaxAccountingSubjectService
{
    private readonly ITaxAccountingSubjectRepository _repository;

    public TaxAccountingSubjectService(
        ITaxAccountingSubjectRepository repository,
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

            // 驗證固定資產相關欄位
            if (dto.VoucherType == "2") // 固定資產
            {
                if (!dto.ExpYear.HasValue)
                {
                    throw new ArgumentException("傳票格式為固定資產時，折舊攤提年限不能為空");
                }

                if (!dto.ResiValue.HasValue)
                {
                    throw new ArgumentException("傳票格式為固定資產時，殘值年限不能為空");
                }

                if (string.IsNullOrWhiteSpace(dto.DepreLid))
                {
                    throw new ArgumentException("傳票格式為固定資產時，折舊會計科目不能為空");
                }

                if (string.IsNullOrWhiteSpace(dto.AccudepreLid))
                {
                    throw new ArgumentException("傳票格式為固定資產時，累計折舊會計科目不能為空");
                }

                // 檢查折舊會計科目是否為可輸科目
                var depreSubject = await _repository.GetByIdAsync(dto.DepreLid);
                if (depreSubject == null || depreSubject.StypeYn != "Y")
                {
                    throw new InvalidOperationException("折舊會計科目必須為可輸科目");
                }

                // 檢查累計折舊會計科目是否為可輸科目
                var accudepreSubject = await _repository.GetByIdAsync(dto.AccudepreLid);
                if (accudepreSubject == null || accudepreSubject.StypeYn != "Y")
                {
                    throw new InvalidOperationException("累計折舊會計科目必須為可輸科目");
                }
            }

            var entity = MapToEntity(dto);
            entity.CreatedBy = GetCurrentUserId();
            entity.CreatedAt = DateTime.Now;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.CreateAsync(entity);

            _logger.LogInfo($"新增會計科目成功: {dto.StypeId}");

            return dto.StypeId;
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
            // 檢查科目是否存在
            var existing = await _repository.GetByIdAsync(stypeId);
            if (existing == null)
            {
                throw new InvalidOperationException($"會計科目不存在: {stypeId}");
            }

            // 驗證必填欄位
            if (string.IsNullOrWhiteSpace(dto.StypeName))
            {
                throw new ArgumentException("科目名稱不能為空");
            }

            // 驗證固定資產相關欄位
            if (dto.VoucherType == "2") // 固定資產
            {
                if (!dto.ExpYear.HasValue)
                {
                    throw new ArgumentException("傳票格式為固定資產時，折舊攤提年限不能為空");
                }

                if (!dto.ResiValue.HasValue)
                {
                    throw new ArgumentException("傳票格式為固定資產時，殘值年限不能為空");
                }

                if (string.IsNullOrWhiteSpace(dto.DepreLid))
                {
                    throw new ArgumentException("傳票格式為固定資產時，折舊會計科目不能為空");
                }

                if (string.IsNullOrWhiteSpace(dto.AccudepreLid))
                {
                    throw new ArgumentException("傳票格式為固定資產時，累計折舊會計科目不能為空");
                }

                // 檢查折舊會計科目是否為可輸科目
                var depreSubject = await _repository.GetByIdAsync(dto.DepreLid);
                if (depreSubject == null || depreSubject.StypeYn != "Y")
                {
                    throw new InvalidOperationException("折舊會計科目必須為可輸科目");
                }

                // 檢查累計折舊會計科目是否為可輸科目
                var accudepreSubject = await _repository.GetByIdAsync(dto.AccudepreLid);
                if (accudepreSubject == null || accudepreSubject.StypeYn != "Y")
                {
                    throw new InvalidOperationException("累計折舊會計科目必須為可輸科目");
                }
            }

            // 檢查是否修改是否立沖欄位，如有需檢查未沖帳餘額
            if (existing.AbatYn != dto.AbatYn)
            {
                var balanceCheck = await CheckUnsettledBalanceAsync(stypeId);
                if (balanceCheck.HasUnsettledBalance)
                {
                    throw new InvalidOperationException($"科目有未沖帳餘額 {balanceCheck.Balance}，無法修改是否立沖欄位");
                }
            }

            var entity = MapToEntity(stypeId, dto);
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);

            _logger.LogInfo($"修改會計科目成功: {stypeId}");
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
            // 檢查科目是否存在
            var existing = await _repository.GetByIdAsync(stypeId);
            if (existing == null)
            {
                throw new InvalidOperationException($"會計科目不存在: {stypeId}");
            }

            // 檢查是否有未沖帳餘額
            var balanceCheck = await CheckUnsettledBalanceAsync(stypeId);
            if (balanceCheck.HasUnsettledBalance)
            {
                throw new InvalidOperationException($"科目有未沖帳餘額 {balanceCheck.Balance}，無法刪除");
            }

            await _repository.DeleteAsync(stypeId);

            _logger.LogInfo($"刪除會計科目成功: {stypeId}");
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
            _logger.LogError($"檢查科目代號是否存在失敗: {stypeId}", ex);
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

    /// <summary>
    /// 將 Entity 轉換為 DTO
    /// </summary>
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

    /// <summary>
    /// 將 Create DTO 轉換為 Entity
    /// </summary>
    private AccountSubject MapToEntity(CreateAccountSubjectDto dto)
    {
        return new AccountSubject
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
            StypeOrder = dto.StypeOrder
        };
    }

    /// <summary>
    /// 將 Update DTO 轉換為 Entity
    /// </summary>
    private AccountSubject MapToEntity(string stypeId, UpdateAccountSubjectDto dto)
    {
        return new AccountSubject
        {
            StypeId = stypeId,
            StypeName = dto.StypeName,
            StypeNameE = dto.StypeNameE,
            Dc = dto.Dc,
            LedgerMd = dto.LedgerMd,
            MtypeId = dto.MtypeId,
            AbatYn = dto.AbatYn,
            VoucherType = dto.VoucherType,
            BudgetYn = dto.BudgetYn,
            OrgYn = dto.OrgYn,
            ExpYear = dto.ExpYear,
            ResiValue = dto.ResiValue,
            DepreLid = dto.DepreLid,
            AccudepreLid = dto.AccudepreLid,
            StypeYn = dto.StypeYn,
            IfrsStypeId = dto.IfrsStypeId,
            RocStypeId = dto.RocStypeId,
            SapStypeId = dto.SapStypeId,
            StypeClass = dto.StypeClass,
            StypeOrder = dto.StypeOrder,
            Amt0 = dto.Amt0,
            Amt1 = dto.Amt1
        };
    }
}

