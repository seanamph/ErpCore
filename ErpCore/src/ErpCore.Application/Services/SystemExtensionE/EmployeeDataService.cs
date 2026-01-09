using ErpCore.Application.DTOs.SystemExtensionE;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.SystemExtensionE;
using ErpCore.Infrastructure.Repositories.SystemExtensionE;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.SystemExtensionE;

/// <summary>
/// 員工資料服務實作 (SYSPE10-SYSPE11 - 員工資料維護)
/// </summary>
public class EmployeeDataService : BaseService, IEmployeeDataService
{
    private readonly IEmployeeRepository _repository;

    public EmployeeDataService(
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

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(MapToDto).ToList();

            return new PagedResult<EmployeeDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
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
                throw new Exception($"員工不存在: {employeeId}");
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
            // 檢查員工編號是否已存在
            var exists = await _repository.ExistsAsync(dto.EmployeeId);
            if (exists)
            {
                throw new Exception($"員工編號已存在: {dto.EmployeeId}");
            }

            var entity = new Employee
            {
                EmployeeId = dto.EmployeeId,
                EmployeeName = dto.EmployeeName,
                IdNumber = dto.IdNumber,
                DepartmentId = dto.DepartmentId,
                PositionId = dto.PositionId,
                HireDate = dto.HireDate,
                ResignDate = dto.ResignDate,
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
            _logger.LogInfo($"新增員工成功: {dto.EmployeeId}");
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
            var entity = await _repository.GetByIdAsync(employeeId);
            if (entity == null)
            {
                throw new Exception($"員工不存在: {employeeId}");
            }

            entity.EmployeeName = dto.EmployeeName;
            entity.IdNumber = dto.IdNumber;
            entity.DepartmentId = dto.DepartmentId;
            entity.PositionId = dto.PositionId;
            entity.HireDate = dto.HireDate;
            entity.ResignDate = dto.ResignDate;
            entity.Status = dto.Status;
            entity.Email = dto.Email;
            entity.Phone = dto.Phone;
            entity.Address = dto.Address;
            entity.BirthDate = dto.BirthDate;
            entity.Gender = dto.Gender;
            entity.Notes = dto.Notes;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
            _logger.LogInfo($"修改員工成功: {employeeId}");
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
            var entity = await _repository.GetByIdAsync(employeeId);
            if (entity == null)
            {
                throw new Exception($"員工不存在: {employeeId}");
            }

            await _repository.DeleteAsync(employeeId);
            _logger.LogInfo($"刪除員工成功: {employeeId}");
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

/// <summary>
/// 人事資料服務實作 (SYSPED0 - 人事資料維護)
/// </summary>
public class PersonnelDataService : BaseService, IPersonnelDataService
{
    private readonly IPersonnelRepository _repository;

    public PersonnelDataService(
        IPersonnelRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<PersonnelDto>> GetPersonnelAsync(PersonnelQueryDto query)
    {
        try
        {
            var repositoryQuery = new PersonnelQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                PersonnelId = query.PersonnelId,
                PersonnelName = query.PersonnelName,
                DepartmentId = query.DepartmentId,
                PositionId = query.PositionId,
                Status = query.Status
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(MapToDto).ToList();

            return new PagedResult<PersonnelDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢人事列表失敗", ex);
            throw;
        }
    }

    public async Task<PersonnelDto> GetPersonnelByIdAsync(string personnelId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(personnelId);
            if (entity == null)
            {
                throw new Exception($"人事不存在: {personnelId}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢人事失敗: {personnelId}", ex);
            throw;
        }
    }

    public async Task<string> CreatePersonnelAsync(CreatePersonnelDto dto)
    {
        try
        {
            // 檢查人事編號是否已存在
            var exists = await _repository.ExistsAsync(dto.PersonnelId);
            if (exists)
            {
                throw new Exception($"人事編號已存在: {dto.PersonnelId}");
            }

            var entity = new Personnel
            {
                PersonnelId = dto.PersonnelId,
                PersonnelName = dto.PersonnelName,
                IdNumber = dto.IdNumber,
                DepartmentId = dto.DepartmentId,
                PositionId = dto.PositionId,
                HireDate = dto.HireDate,
                ResignDate = dto.ResignDate,
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
            _logger.LogInfo($"新增人事成功: {dto.PersonnelId}");
            return entity.PersonnelId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增人事失敗: {dto.PersonnelId}", ex);
            throw;
        }
    }

    public async Task UpdatePersonnelAsync(string personnelId, UpdatePersonnelDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(personnelId);
            if (entity == null)
            {
                throw new Exception($"人事不存在: {personnelId}");
            }

            entity.PersonnelName = dto.PersonnelName;
            entity.IdNumber = dto.IdNumber;
            entity.DepartmentId = dto.DepartmentId;
            entity.PositionId = dto.PositionId;
            entity.HireDate = dto.HireDate;
            entity.ResignDate = dto.ResignDate;
            entity.Status = dto.Status;
            entity.Email = dto.Email;
            entity.Phone = dto.Phone;
            entity.Address = dto.Address;
            entity.BirthDate = dto.BirthDate;
            entity.Gender = dto.Gender;
            entity.Notes = dto.Notes;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
            _logger.LogInfo($"修改人事成功: {personnelId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改人事失敗: {personnelId}", ex);
            throw;
        }
    }

    public async Task DeletePersonnelAsync(string personnelId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(personnelId);
            if (entity == null)
            {
                throw new Exception($"人事不存在: {personnelId}");
            }

            await _repository.DeleteAsync(personnelId);
            _logger.LogInfo($"刪除人事成功: {personnelId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除人事失敗: {personnelId}", ex);
            throw;
        }
    }

    private PersonnelDto MapToDto(Personnel entity)
    {
        return new PersonnelDto
        {
            PersonnelId = entity.PersonnelId,
            PersonnelName = entity.PersonnelName,
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

