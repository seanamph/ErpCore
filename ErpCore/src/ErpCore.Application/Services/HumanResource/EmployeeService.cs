using ErpCore.Application.DTOs.HumanResource;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.HumanResource;
using ErpCore.Infrastructure.Repositories.HumanResource;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.HumanResource;

/// <summary>
/// 員工服務實作 (SYSH110)
/// </summary>
public class EmployeeService : BaseService, IEmployeeService
{
    private readonly IEmployeeRepository _repository;

    public EmployeeService(
        IEmployeeRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<EmployeeDto>> GetEmployeesAsync(EmployeeQueryDto query)
    {
        try
        {
            var repositoryQuery = new EmployeeQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                EmployeeId = query.EmployeeId,
                EmployeeName = query.EmployeeName,
                DepartmentId = query.DepartmentId,
                PositionId = query.PositionId,
                Status = query.Status
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<EmployeeDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢員工列表失敗", ex);
            throw;
        }
    }

    public async Task<EmployeeDto> GetEmployeeByIdAsync(string employeeId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(employeeId);
            if (entity == null)
            {
                throw new InvalidOperationException($"員工不存在: {employeeId}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢員工失敗: {employeeId}", ex);
            throw;
        }
    }

    public async Task<string> CreateEmployeeAsync(CreateEmployeeDto dto)
    {
        try
        {
            // 驗證必填欄位
            if (string.IsNullOrWhiteSpace(dto.EmployeeId))
            {
                throw new ArgumentException("員工編號不能為空");
            }

            if (string.IsNullOrWhiteSpace(dto.EmployeeName))
            {
                throw new ArgumentException("員工姓名不能為空");
            }

            // 檢查員工編號是否已存在
            var exists = await _repository.ExistsAsync(dto.EmployeeId);
            if (exists)
            {
                throw new InvalidOperationException($"員工編號已存在: {dto.EmployeeId}");
            }

            var entity = new Employee
            {
                EmployeeId = dto.EmployeeId,
                EmployeeName = dto.EmployeeName,
                IdNumber = dto.IdNumber,
                DepartmentId = dto.DepartmentId,
                PositionId = dto.PositionId,
                HireDate = dto.HireDate,
                ResignDate = null,
                Status = dto.Status,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                BirthDate = dto.BirthDate,
                Gender = dto.Gender,
                Notes = dto.Notes,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(entity);

            return entity.EmployeeId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增員工失敗: {dto.EmployeeId}", ex);
            throw;
        }
    }

    public async Task UpdateEmployeeAsync(string employeeId, UpdateEmployeeDto dto)
    {
        try
        {
            // 驗證必填欄位
            if (string.IsNullOrWhiteSpace(dto.EmployeeName))
            {
                throw new ArgumentException("員工姓名不能為空");
            }

            // 檢查員工是否存在
            var existing = await _repository.GetByIdAsync(employeeId);
            if (existing == null)
            {
                throw new InvalidOperationException($"員工不存在: {employeeId}");
            }

            existing.EmployeeName = dto.EmployeeName;
            existing.IdNumber = dto.IdNumber;
            existing.DepartmentId = dto.DepartmentId;
            existing.PositionId = dto.PositionId;
            existing.HireDate = dto.HireDate;
            existing.ResignDate = dto.ResignDate;
            existing.Status = dto.Status;
            existing.Email = dto.Email;
            existing.Phone = dto.Phone;
            existing.Address = dto.Address;
            existing.BirthDate = dto.BirthDate;
            existing.Gender = dto.Gender;
            existing.Notes = dto.Notes;
            existing.UpdatedBy = GetCurrentUserId();
            existing.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(existing);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改員工失敗: {employeeId}", ex);
            throw;
        }
    }

    public async Task DeleteEmployeeAsync(string employeeId)
    {
        try
        {
            // 檢查員工是否存在
            var existing = await _repository.GetByIdAsync(employeeId);
            if (existing == null)
            {
                throw new InvalidOperationException($"員工不存在: {employeeId}");
            }

            await _repository.DeleteAsync(employeeId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除員工失敗: {employeeId}", ex);
            throw;
        }
    }

    private EmployeeDto MapToDto(Employee entity)
    {
        return new EmployeeDto
        {
            EmployeeId = entity.EmployeeId,
            EmployeeName = entity.EmployeeName,
            IdNumber = entity.IdNumber,
            DepartmentId = entity.DepartmentId,
            PositionId = entity.PositionId,
            HireDate = entity.HireDate,
            ResignDate = entity.ResignDate,
            Status = entity.Status,
            Email = entity.Email,
            Phone = entity.Phone,
            Address = entity.Address,
            BirthDate = entity.BirthDate,
            Gender = entity.Gender,
            Notes = entity.Notes,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

