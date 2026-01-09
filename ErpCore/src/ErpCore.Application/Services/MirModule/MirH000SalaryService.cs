using ErpCore.Application.DTOs.MirModule;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.MirModule;
using ErpCore.Infrastructure.Repositories.MirModule;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.MirModule;

/// <summary>
/// MIRH000 薪資服務實作
/// </summary>
public class MirH000SalaryService : BaseService, IMirH000SalaryService
{
    private readonly IMirH000SalaryRepository _repository;

    public MirH000SalaryService(
        IMirH000SalaryRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<MirH000SalaryDto>> GetSalaryListAsync(MirH000SalaryQueryDto query)
    {
        try
        {
            var repositoryQuery = new MirH000SalaryQuery
            {
                SalaryId = query.SalaryId,
                PersonnelId = query.PersonnelId,
                SalaryMonth = query.SalaryMonth,
                Status = query.Status,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(x => new MirH000SalaryDto
            {
                TKey = x.TKey,
                SalaryId = x.SalaryId,
                PersonnelId = x.PersonnelId,
                SalaryMonth = x.SalaryMonth,
                BaseSalary = x.BaseSalary,
                Bonus = x.Bonus,
                TotalSalary = x.TotalSalary,
                Status = x.Status,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();

            return new PagedResult<MirH000SalaryDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢薪資列表失敗", ex);
            throw;
        }
    }

    public async Task<MirH000SalaryDto> GetSalaryByIdAsync(string salaryId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(salaryId);
            if (entity == null)
            {
                throw new Exception($"薪資資料不存在: {salaryId}");
            }

            return new MirH000SalaryDto
            {
                TKey = entity.TKey,
                SalaryId = entity.SalaryId,
                PersonnelId = entity.PersonnelId,
                SalaryMonth = entity.SalaryMonth,
                BaseSalary = entity.BaseSalary,
                Bonus = entity.Bonus,
                TotalSalary = entity.TotalSalary,
                Status = entity.Status,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt,
                UpdatedBy = entity.UpdatedBy,
                UpdatedAt = entity.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢薪資資料失敗: {salaryId}", ex);
            throw;
        }
    }

    public async Task<string> CreateSalaryAsync(CreateMirH000SalaryDto dto)
    {
        try
        {
            // 計算總薪資
            var totalSalary = dto.BaseSalary + dto.Bonus;

            var entity = new MirH000Salary
            {
                SalaryId = dto.SalaryId,
                PersonnelId = dto.PersonnelId,
                SalaryMonth = dto.SalaryMonth,
                BaseSalary = dto.BaseSalary,
                Bonus = dto.Bonus,
                TotalSalary = totalSalary,
                Status = dto.Status,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(entity);
            return entity.SalaryId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增薪資資料失敗: {dto.SalaryId}", ex);
            throw;
        }
    }

    public async Task UpdateSalaryAsync(string salaryId, UpdateMirH000SalaryDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(salaryId);
            if (entity == null)
            {
                throw new Exception($"薪資資料不存在: {salaryId}");
            }

            // 計算總薪資
            var totalSalary = dto.BaseSalary + dto.Bonus;

            entity.PersonnelId = dto.PersonnelId;
            entity.SalaryMonth = dto.SalaryMonth;
            entity.BaseSalary = dto.BaseSalary;
            entity.Bonus = dto.Bonus;
            entity.TotalSalary = totalSalary;
            entity.Status = dto.Status;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改薪資資料失敗: {salaryId}", ex);
            throw;
        }
    }

    public async Task DeleteSalaryAsync(string salaryId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(salaryId);
            if (entity == null)
            {
                throw new Exception($"薪資資料不存在: {salaryId}");
            }

            await _repository.DeleteAsync(salaryId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除薪資資料失敗: {salaryId}", ex);
            throw;
        }
    }
}

