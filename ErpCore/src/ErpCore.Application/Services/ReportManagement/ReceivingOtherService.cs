using ErpCore.Application.DTOs.ReportManagement;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.ReportManagement;
using ErpCore.Infrastructure.Repositories.ReportManagement;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.ReportManagement;

/// <summary>
/// 收款其他功能服務實作 (SYSR510-SYSR570)
/// </summary>
public class ReceivingOtherService : BaseService, IReceivingOtherService
{
    private readonly IDepositsRepository _repository;

    public ReceivingOtherService(
        IDepositsRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<DepositsDto> GetDepositByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"保證金不存在: {tKey}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢保證金失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<PagedResult<DepositsDto>> QueryDepositsAsync(DepositsQueryDto query)
    {
        try
        {
            var repositoryQuery = new DepositsQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortBy = query.SortBy,
                SortOrder = query.SortOrder,
                DepositNo = query.DepositNo,
                ObjectId = query.ObjectId,
                DepositDateFrom = query.DepositDateFrom,
                DepositDateTo = query.DepositDateTo,
                DepositStatus = query.DepositStatus,
                DepositType = query.DepositType,
                ShopId = query.ShopId,
                SiteId = query.SiteId
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(MapToDto).ToList();

            return new PagedResult<DepositsDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢保證金列表失敗", ex);
            throw;
        }
    }

    public async Task<DepositsDto> CreateDepositAsync(CreateDepositsDto dto)
    {
        try
        {
            // 產生保證金單號
            var depositNo = GenerateDepositNo(dto.DepositDate);

            var entity = new Deposits
            {
                DepositNo = depositNo,
                DepositDate = dto.DepositDate,
                ObjectId = dto.ObjectId,
                DepositAmount = dto.DepositAmount,
                DepositType = dto.DepositType,
                DepositStatus = "A", // 有效
                CheckDueDate = dto.CheckDueDate,
                ShopId = dto.ShopId,
                SiteId = dto.SiteId,
                OrgId = dto.OrgId,
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
            _logger.LogError("新增保證金失敗", ex);
            throw;
        }
    }

    public async Task<DepositsDto> UpdateDepositAsync(long tKey, UpdateDepositsDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"保證金不存在: {tKey}");
            }

            // 只有有效狀態才能修改
            if (entity.DepositStatus != "A")
            {
                throw new InvalidOperationException($"只有有效狀態的保證金才能修改");
            }

            if (dto.DepositAmount.HasValue) entity.DepositAmount = dto.DepositAmount.Value;
            if (dto.DepositType != null) entity.DepositType = dto.DepositType;
            if (dto.CheckDueDate.HasValue) entity.CheckDueDate = dto.CheckDueDate;
            if (dto.Notes != null) entity.Notes = dto.Notes;

            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            var result = await _repository.UpdateAsync(entity);
            return MapToDto(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新保證金失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteDepositAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"保證金不存在: {tKey}");
            }

            // 只有有效狀態才能刪除
            if (entity.DepositStatus != "A")
            {
                throw new InvalidOperationException($"只有有效狀態的保證金才能刪除");
            }

            await _repository.DeleteAsync(tKey);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除保證金失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<DepositsDto> ReturnDepositAsync(long tKey, ReturnDepositsDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"保證金不存在: {tKey}");
            }

            if (entity.DepositStatus != "A")
            {
                throw new InvalidOperationException($"只有有效狀態的保證金才能退還");
            }

            if (dto.ReturnAmount > entity.DepositAmount)
            {
                throw new InvalidOperationException($"退還金額不能超過保證金金額");
            }

            entity.DepositStatus = "R"; // 退還
            entity.ReturnDate = dto.ReturnDate;
            entity.ReturnAmount = dto.ReturnAmount;
            if (dto.Notes != null) entity.Notes = dto.Notes;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            var result = await _repository.UpdateAsync(entity);
            return MapToDto(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"退還保證金失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<DepositsDto> DepositAmountAsync(long tKey, DepositAmountDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"保證金不存在: {tKey}");
            }

            entity.DepositAmount += dto.DepositAmount;
            entity.DepositDate = dto.DepositDate;
            if (dto.Notes != null) entity.Notes = dto.Notes;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            var result = await _repository.UpdateAsync(entity);
            return MapToDto(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"存款保證金失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsDepositNoAsync(string depositNo)
    {
        try
        {
            return await _repository.ExistsAsync(depositNo);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查保證金單號是否存在失敗: DepositNo={depositNo}", ex);
            throw;
        }
    }

    private string GenerateDepositNo(DateTime depositDate)
    {
        // 產生保證金單號：DEP + 日期(yyyyMMdd) + 流水號(3碼)
        var dateStr = depositDate.ToString("yyyyMMdd");
        var random = new Random();
        var serial = random.Next(1, 1000).ToString("D3");
        return $"DEP{dateStr}{serial}";
    }

    private DepositsDto MapToDto(Deposits entity)
    {
        return new DepositsDto
        {
            TKey = entity.TKey,
            DepositNo = entity.DepositNo,
            DepositDate = entity.DepositDate,
            ObjectId = entity.ObjectId,
            DepositAmount = entity.DepositAmount,
            DepositType = entity.DepositType,
            DepositTypeName = entity.DepositType switch
            {
                "CASH" => "現金",
                "CHECK" => "支票",
                "TRANSFER" => "轉帳",
                _ => entity.DepositType
            },
            DepositStatus = entity.DepositStatus,
            DepositStatusName = entity.DepositStatus switch
            {
                "A" => "有效",
                "R" => "退還",
                "C" => "取消",
                _ => entity.DepositStatus
            },
            ReturnDate = entity.ReturnDate,
            ReturnAmount = entity.ReturnAmount,
            VoucherNo = entity.VoucherNo,
            VoucherM_TKey = entity.VoucherM_TKey,
            VoucherStatus = entity.VoucherStatus,
            CheckDueDate = entity.CheckDueDate,
            ShopId = entity.ShopId,
            SiteId = entity.SiteId,
            OrgId = entity.OrgId,
            Notes = entity.Notes,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

