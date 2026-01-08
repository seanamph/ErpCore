using ErpCore.Application.DTOs.BusinessReport;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.BusinessReport;
using ErpCore.Infrastructure.Repositories.BusinessReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.BusinessReport;

/// <summary>
/// 加班發放服務實作 (SYSL510)
/// </summary>
public class OvertimePaymentService : BaseService, IOvertimePaymentService
{
    private readonly IOvertimePaymentRepository _repository;

    public OvertimePaymentService(
        IOvertimePaymentRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<OvertimePaymentDto>> GetOvertimePaymentsAsync(OvertimePaymentQueryDto query)
    {
        try
        {
            var repositoryQuery = new OvertimePaymentQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                PaymentNo = query.PaymentNo,
                EmployeeId = query.EmployeeId,
                DepartmentId = query.DepartmentId,
                StartDateFrom = query.StartDateFrom,
                StartDateTo = query.StartDateTo,
                EndDateFrom = query.EndDateFrom,
                EndDateTo = query.EndDateTo,
                Status = query.Status
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<OvertimePaymentDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢加班發放列表失敗", ex);
            throw;
        }
    }

    public async Task<OvertimePaymentDto?> GetOvertimePaymentByPaymentNoAsync(string paymentNo)
    {
        try
        {
            var entity = await _repository.GetByPaymentNoAsync(paymentNo);
            if (entity == null)
            {
                return null;
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢加班發放失敗: {paymentNo}", ex);
            throw;
        }
    }

    public async Task<string> CreateOvertimePaymentAsync(CreateOvertimePaymentDto dto)
    {
        try
        {
            // 驗證資料
            if (string.IsNullOrEmpty(dto.EmployeeId))
            {
                throw new Exception("員工編號不能為空");
            }

            if (dto.OvertimeHours <= 0)
            {
                throw new Exception("加班時數必須大於0");
            }

            if (dto.OvertimeAmount < 0)
            {
                throw new Exception("加班金額不能為負數");
            }

            if (dto.StartDate > dto.EndDate)
            {
                throw new Exception("開始日期不能大於結束日期");
            }

            // 產生發放單號
            var paymentNo = await GeneratePaymentNoAsync();

            var entity = new OvertimePayment
            {
                PaymentNo = paymentNo,
                PaymentDate = DateTime.Now,
                EmployeeId = dto.EmployeeId,
                EmployeeName = dto.EmployeeName,
                DepartmentId = dto.DepartmentId,
                OvertimeHours = dto.OvertimeHours,
                OvertimeAmount = dto.OvertimeAmount,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = "Draft",
                Notes = dto.Notes,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(entity);
            return paymentNo;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增加班發放失敗", ex);
            throw;
        }
    }

    public async Task UpdateOvertimePaymentAsync(string paymentNo, UpdateOvertimePaymentDto dto)
    {
        try
        {
            var entity = await _repository.GetByPaymentNoAsync(paymentNo);
            if (entity == null)
            {
                throw new Exception($"找不到加班發放: {paymentNo}");
            }

            // 已審核的資料不能修改
            if (entity.Status == "Approved")
            {
                throw new Exception("已審核的加班發放不能修改");
            }

            // 更新資料
            if (!string.IsNullOrEmpty(dto.EmployeeName))
            {
                entity.EmployeeName = dto.EmployeeName;
            }

            if (!string.IsNullOrEmpty(dto.DepartmentId))
            {
                entity.DepartmentId = dto.DepartmentId;
            }

            if (dto.OvertimeHours.HasValue)
            {
                if (dto.OvertimeHours.Value <= 0)
                {
                    throw new Exception("加班時數必須大於0");
                }
                entity.OvertimeHours = dto.OvertimeHours.Value;
            }

            if (dto.OvertimeAmount.HasValue)
            {
                if (dto.OvertimeAmount.Value < 0)
                {
                    throw new Exception("加班金額不能為負數");
                }
                entity.OvertimeAmount = dto.OvertimeAmount.Value;
            }

            if (dto.StartDate.HasValue)
            {
                entity.StartDate = dto.StartDate.Value;
            }

            if (dto.EndDate.HasValue)
            {
                entity.EndDate = dto.EndDate.Value;
            }

            if (dto.StartDate.HasValue && dto.EndDate.HasValue && dto.StartDate.Value > dto.EndDate.Value)
            {
                throw new Exception("開始日期不能大於結束日期");
            }

            if (!string.IsNullOrEmpty(dto.Notes))
            {
                entity.Notes = dto.Notes;
            }

            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改加班發放失敗: {paymentNo}", ex);
            throw;
        }
    }

    public async Task DeleteOvertimePaymentAsync(string paymentNo)
    {
        try
        {
            var entity = await _repository.GetByPaymentNoAsync(paymentNo);
            if (entity == null)
            {
                throw new Exception($"找不到加班發放: {paymentNo}");
            }

            // 已審核的資料不能刪除
            if (entity.Status == "Approved")
            {
                throw new Exception("已審核的加班發放不能刪除");
            }

            await _repository.DeleteAsync(entity.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除加班發放失敗: {paymentNo}", ex);
            throw;
        }
    }

    public async Task ApproveOvertimePaymentAsync(string paymentNo, ApproveOvertimePaymentDto dto)
    {
        try
        {
            var entity = await _repository.GetByPaymentNoAsync(paymentNo);
            if (entity == null)
            {
                throw new Exception($"找不到加班發放: {paymentNo}");
            }

            // 驗證狀態
            if (entity.Status != "Draft" && entity.Status != "Submitted")
            {
                throw new Exception($"狀態為 {entity.Status} 的加班發放不能審核");
            }

            // 更新狀態
            entity.Status = dto.Status;
            entity.ApprovedBy = GetCurrentUserId();
            entity.ApprovedAt = DateTime.Now;

            if (!string.IsNullOrEmpty(dto.Notes))
            {
                entity.Notes = dto.Notes;
            }

            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"審核加班發放失敗: {paymentNo}", ex);
            throw;
        }
    }

    /// <summary>
    /// 產生發放單號
    /// 格式: OT + YYYYMMDD + 流水號(3碼)
    /// 範例: OT20250109001
    /// </summary>
    private async Task<string> GeneratePaymentNoAsync()
    {
        try
        {
            var dateStr = DateTime.Now.ToString("yyyyMMdd");
            var prefix = $"OT{dateStr}";

            // 查詢當日最大流水號
            var maxNo = await _repository.QueryAsync(new OvertimePaymentQuery
            {
                PageIndex = 1,
                PageSize = 1,
                PaymentNo = prefix,
                SortField = "PaymentNo",
                SortOrder = "DESC"
            });

            int sequence = 1;
            if (maxNo.Items.Any())
            {
                var lastNo = maxNo.Items.First().PaymentNo;
                if (lastNo.StartsWith(prefix) && lastNo.Length >= prefix.Length + 3)
                {
                    var seqStr = lastNo.Substring(prefix.Length, 3);
                    if (int.TryParse(seqStr, out var lastSeq))
                    {
                        sequence = lastSeq + 1;
                    }
                }
            }

            var paymentNo = $"{prefix}{sequence:D3}";

            // 檢查單號是否已存在（防止重複）
            var exists = await _repository.ExistsByPaymentNoAsync(paymentNo);
            if (exists)
            {
                // 如果存在，遞增流水號
                sequence++;
                paymentNo = $"{prefix}{sequence:D3}";
            }

            return paymentNo;
        }
        catch (Exception ex)
        {
            _logger.LogError("產生發放單號失敗", ex);
            throw;
        }
    }

    private OvertimePaymentDto MapToDto(OvertimePayment entity)
    {
        return new OvertimePaymentDto
        {
            Id = entity.Id,
            PaymentNo = entity.PaymentNo,
            PaymentDate = entity.PaymentDate,
            EmployeeId = entity.EmployeeId,
            EmployeeName = entity.EmployeeName,
            DepartmentId = entity.DepartmentId,
            DepartmentName = entity.DepartmentName,
            OvertimeHours = entity.OvertimeHours,
            OvertimeAmount = entity.OvertimeAmount,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            Status = entity.Status,
            ApprovedBy = entity.ApprovedBy,
            ApprovedByName = entity.ApprovedByName,
            ApprovedAt = entity.ApprovedAt,
            Notes = entity.Notes,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

