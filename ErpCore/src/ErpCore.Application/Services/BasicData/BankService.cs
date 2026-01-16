using ErpCore.Application.DTOs.BasicData;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.BasicData;
using ErpCore.Infrastructure.Repositories.BasicData;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.BasicData;

/// <summary>
/// 銀行服務實作
/// </summary>
public class BankService : BaseService, IBankService
{
    private readonly IBankRepository _repository;

    public BankService(
        IBankRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<BankDto>> GetBanksAsync(BankQueryDto query)
    {
        try
        {
            var repositoryQuery = new BankQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                BankId = query.BankId,
                BankName = query.BankName,
                Status = query.Status,
                BankKind = query.BankKind
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => new BankDto
            {
                BankId = x.BankId,
                BankName = x.BankName,
                AcctLen = x.AcctLen,
                AcctLenMax = x.AcctLenMax,
                Status = x.Status,
                BankKind = x.BankKind,
                SeqNo = x.SeqNo,
                Notes = x.Notes,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();

            return new PagedResult<BankDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢銀行列表失敗", ex);
            throw;
        }
    }

    public async Task<BankDto> GetBankAsync(string bankId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(bankId);
            if (entity == null)
            {
                throw new InvalidOperationException($"銀行不存在: {bankId}");
            }

            return new BankDto
            {
                BankId = entity.BankId,
                BankName = entity.BankName,
                AcctLen = entity.AcctLen,
                AcctLenMax = entity.AcctLenMax,
                Status = entity.Status,
                BankKind = entity.BankKind,
                SeqNo = entity.SeqNo,
                Notes = entity.Notes,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt,
                UpdatedBy = entity.UpdatedBy,
                UpdatedAt = entity.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢銀行失敗: {bankId}", ex);
            throw;
        }
    }

    public async Task<string> CreateBankAsync(CreateBankDto dto)
    {
        try
        {
            // 驗證資料
            ValidateCreateDto(dto);

            // 檢查是否已存在
            var exists = await _repository.ExistsAsync(dto.BankId);
            if (exists)
            {
                throw new InvalidOperationException($"銀行已存在: {dto.BankId}");
            }

            var entity = new Bank
            {
                BankId = dto.BankId,
                BankName = dto.BankName,
                AcctLen = dto.AcctLen,
                AcctLenMax = dto.AcctLenMax,
                Status = dto.Status,
                BankKind = dto.BankKind,
                SeqNo = dto.SeqNo,
                Notes = dto.Notes,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now,
                CreatedPriority = null,
                CreatedGroup = GetCurrentOrgId()
            };

            await _repository.CreateAsync(entity);

            return entity.BankId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增銀行失敗: {dto.BankId}", ex);
            throw;
        }
    }

    public async Task UpdateBankAsync(string bankId, UpdateBankDto dto)
    {
        try
        {
            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(bankId);
            if (entity == null)
            {
                throw new InvalidOperationException($"銀行不存在: {bankId}");
            }

            // 驗證資料
            ValidateUpdateDto(dto);

            entity.BankName = dto.BankName;
            entity.AcctLen = dto.AcctLen;
            entity.AcctLenMax = dto.AcctLenMax;
            entity.Status = dto.Status;
            entity.BankKind = dto.BankKind;
            entity.SeqNo = dto.SeqNo;
            entity.Notes = dto.Notes;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改銀行失敗: {bankId}", ex);
            throw;
        }
    }

    public async Task DeleteBankAsync(string bankId)
    {
        try
        {
            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(bankId);
            if (entity == null)
            {
                throw new InvalidOperationException($"銀行不存在: {bankId}");
            }

            // TODO: 檢查是否有相關資料引用（如銀行帳號等）

            await _repository.DeleteAsync(bankId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除銀行失敗: {bankId}", ex);
            throw;
        }
    }

    public async Task DeleteBanksBatchAsync(BatchDeleteBankDto dto)
    {
        try
        {
            foreach (var bankId in dto.BankIds)
            {
                await DeleteBankAsync(bankId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("批次刪除銀行失敗", ex);
            throw;
        }
    }

    private void ValidateCreateDto(CreateBankDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.BankId))
        {
            throw new ArgumentException("銀行代號不能為空");
        }

        if (string.IsNullOrWhiteSpace(dto.BankName))
        {
            throw new ArgumentException("銀行名稱不能為空");
        }

        if (dto.AcctLen.HasValue && dto.AcctLenMax.HasValue && dto.AcctLen > dto.AcctLenMax)
        {
            throw new ArgumentException("帳號最小長度不能大於最大長度");
        }

        if (!string.IsNullOrEmpty(dto.Status) && dto.Status != "0" && dto.Status != "1")
        {
            throw new ArgumentException("狀態值必須為 0 或 1");
        }
    }

    private void ValidateUpdateDto(UpdateBankDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.BankName))
        {
            throw new ArgumentException("銀行名稱不能為空");
        }

        if (dto.AcctLen.HasValue && dto.AcctLenMax.HasValue && dto.AcctLen > dto.AcctLenMax)
        {
            throw new ArgumentException("帳號最小長度不能大於最大長度");
        }

        if (!string.IsNullOrEmpty(dto.Status) && dto.Status != "0" && dto.Status != "1")
        {
            throw new ArgumentException("狀態值必須為 0 或 1");
        }
    }
}
