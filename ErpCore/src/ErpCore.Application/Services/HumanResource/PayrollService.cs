using ErpCore.Application.DTOs.HumanResource;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.HumanResource;
using ErpCore.Infrastructure.Repositories.HumanResource;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.HumanResource;

/// <summary>
/// 薪資服務實作 (SYSH210)
/// </summary>
public class PayrollService : BaseService, IPayrollService
{
    private readonly IPayrollRepository _repository;
    private readonly IEmployeeRepository _employeeRepository;

    public PayrollService(
        IPayrollRepository repository,
        IEmployeeRepository employeeRepository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _employeeRepository = employeeRepository;
    }

    public async Task<PagedResult<PayrollDto>> GetPayrollsAsync(PayrollQueryDto query)
    {
        try
        {
            var repositoryQuery = new PayrollQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                EmployeeId = query.EmployeeId,
                PayrollYear = query.PayrollYear,
                PayrollMonth = query.PayrollMonth,
                Status = query.Status
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => MapToDto(x)).ToList();

            // 補充員工名稱
            foreach (var dto in dtos)
            {
                if (!string.IsNullOrEmpty(dto.EmployeeId))
                {
                    var employee = await _employeeRepository.GetByIdAsync(dto.EmployeeId);
                    if (employee != null)
                    {
                        dto.EmployeeName = employee.EmployeeName;
                    }
                }
            }

            return new PagedResult<PayrollDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢薪資列表失敗", ex);
            throw;
        }
    }

    public async Task<PayrollDto> GetPayrollByIdAsync(string payrollId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(payrollId);
            if (entity == null)
            {
                throw new InvalidOperationException($"薪資不存在: {payrollId}");
            }

            var dto = MapToDto(entity);

            // 補充員工名稱
            if (!string.IsNullOrEmpty(dto.EmployeeId))
            {
                var employee = await _employeeRepository.GetByIdAsync(dto.EmployeeId);
                if (employee != null)
                {
                    dto.EmployeeName = employee.EmployeeName;
                }
            }

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢薪資失敗: {payrollId}", ex);
            throw;
        }
    }

    public async Task<string> CreatePayrollAsync(CreatePayrollDto dto)
    {
        try
        {
            // 驗證必填欄位
            if (string.IsNullOrWhiteSpace(dto.EmployeeId))
            {
                throw new ArgumentException("員工編號不能為空");
            }

            if (dto.PayrollYear <= 0 || dto.PayrollMonth < 1 || dto.PayrollMonth > 12)
            {
                throw new ArgumentException("年度或月份無效");
            }

            // 檢查員工是否存在
            var employee = await _employeeRepository.GetByIdAsync(dto.EmployeeId);
            if (employee == null)
            {
                throw new InvalidOperationException($"員工不存在: {dto.EmployeeId}");
            }

            // 檢查年度月份組合是否已存在
            var exists = await _repository.ExistsAsync(dto.EmployeeId, dto.PayrollYear, dto.PayrollMonth);
            if (exists)
            {
                throw new InvalidOperationException($"該員工 {dto.PayrollYear} 年 {dto.PayrollMonth} 月的薪資已存在");
            }

            // 計算總薪資
            var totalSalary = dto.BaseSalary + dto.Allowance + dto.Bonus - dto.Deduction;

            var entity = new Payroll
            {
                PayrollId = string.Empty, // Repository 會自動生成
                EmployeeId = dto.EmployeeId,
                PayrollYear = dto.PayrollYear,
                PayrollMonth = dto.PayrollMonth,
                BaseSalary = dto.BaseSalary,
                Allowance = dto.Allowance,
                Bonus = dto.Bonus,
                Deduction = dto.Deduction,
                TotalSalary = totalSalary,
                Status = "D", // 草稿
                PayDate = null,
                Notes = dto.Notes,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(entity);

            return entity.PayrollId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增薪資失敗: {dto.EmployeeId}", ex);
            throw;
        }
    }

    public async Task UpdatePayrollAsync(string payrollId, UpdatePayrollDto dto)
    {
        try
        {
            // 檢查薪資是否存在
            var existing = await _repository.GetByIdAsync(payrollId);
            if (existing == null)
            {
                throw new InvalidOperationException($"薪資不存在: {payrollId}");
            }

            // 僅草稿狀態可修改
            if (existing.Status != "D")
            {
                throw new InvalidOperationException("僅草稿狀態的薪資可以修改");
            }

            existing.BaseSalary = dto.BaseSalary;
            existing.Allowance = dto.Allowance;
            existing.Bonus = dto.Bonus;
            existing.Deduction = dto.Deduction;
            existing.Notes = dto.Notes;
            existing.UpdatedBy = GetCurrentUserId();
            existing.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(existing);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改薪資失敗: {payrollId}", ex);
            throw;
        }
    }

    public async Task DeletePayrollAsync(string payrollId)
    {
        try
        {
            // 檢查薪資是否存在
            var existing = await _repository.GetByIdAsync(payrollId);
            if (existing == null)
            {
                throw new InvalidOperationException($"薪資不存在: {payrollId}");
            }

            // 僅草稿狀態可刪除
            if (existing.Status != "D")
            {
                throw new InvalidOperationException("僅草稿狀態的薪資可以刪除");
            }

            await _repository.DeleteAsync(payrollId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除薪資失敗: {payrollId}", ex);
            throw;
        }
    }

    public async Task ConfirmPayrollAsync(string payrollId)
    {
        try
        {
            // 檢查薪資是否存在
            var existing = await _repository.GetByIdAsync(payrollId);
            if (existing == null)
            {
                throw new InvalidOperationException($"薪資不存在: {payrollId}");
            }

            // 僅草稿狀態可確認
            if (existing.Status != "D")
            {
                throw new InvalidOperationException("僅草稿狀態的薪資可以確認");
            }

            existing.Status = "C"; // 已確認
            existing.UpdatedBy = GetCurrentUserId();
            existing.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(existing);
        }
        catch (Exception ex)
        {
            _logger.LogError($"確認薪資失敗: {payrollId}", ex);
            throw;
        }
    }

    public async Task<PayrollCalculationResultDto> CalculatePayrollAsync(CalculatePayrollDto dto)
    {
        try
        {
            var totalIncome = dto.BaseSalary + dto.Allowance + dto.Bonus;
            var totalDeduction = dto.Deduction;
            var totalSalary = totalIncome - totalDeduction;

            return new PayrollCalculationResultDto
            {
                TotalSalary = totalSalary,
                TotalIncome = totalIncome,
                TotalDeduction = totalDeduction
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("計算薪資失敗", ex);
            throw;
        }
    }

    private PayrollDto MapToDto(Payroll entity)
    {
        return new PayrollDto
        {
            PayrollId = entity.PayrollId,
            EmployeeId = entity.EmployeeId,
            PayrollYear = entity.PayrollYear,
            PayrollMonth = entity.PayrollMonth,
            BaseSalary = entity.BaseSalary,
            Allowance = entity.Allowance,
            Bonus = entity.Bonus,
            Deduction = entity.Deduction,
            TotalSalary = entity.TotalSalary,
            Status = entity.Status,
            PayDate = entity.PayDate,
            Notes = entity.Notes,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

