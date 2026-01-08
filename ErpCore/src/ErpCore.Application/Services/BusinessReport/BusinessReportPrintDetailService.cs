using ErpCore.Application.DTOs.BusinessReport;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.BusinessReport;
using ErpCore.Infrastructure.Repositories.BusinessReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.BusinessReport;

/// <summary>
/// 業務報表列印明細服務實作 (SYSL160)
/// </summary>
public class BusinessReportPrintDetailService : BaseService, IBusinessReportPrintDetailService
{
    private readonly IBusinessReportPrintDetailRepository _repository;
    private readonly IBusinessReportPrintRepository _printRepository;

    public BusinessReportPrintDetailService(
        IBusinessReportPrintDetailRepository repository,
        IBusinessReportPrintRepository printRepository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _printRepository = printRepository;
    }

    public async Task<PagedResult<BusinessReportPrintDetailDto>> GetBusinessReportPrintDetailsAsync(BusinessReportPrintDetailQueryDto query)
    {
        try
        {
            var repositoryQuery = new BusinessReportPrintDetailQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                PrintId = query.PrintId,
                LeaveId = query.LeaveId,
                ActEvent = query.ActEvent
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<BusinessReportPrintDetailDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢業務報表列印明細列表失敗", ex);
            throw;
        }
    }

    public async Task<BusinessReportPrintDetailDto?> GetBusinessReportPrintDetailByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                return null;
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢業務報表列印明細失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<List<BusinessReportPrintDetailDto>> GetBusinessReportPrintDetailsByPrintIdAsync(long printId)
    {
        try
        {
            var entities = await _repository.GetByPrintIdAsync(printId);
            return entities.Select(x => MapToDto(x)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢業務報表列印明細失敗: PrintId={printId}", ex);
            throw;
        }
    }

    public async Task<long> CreateBusinessReportPrintDetailAsync(CreateBusinessReportPrintDetailDto dto)
    {
        try
        {
            // 驗證 PrintId 是否存在
            var printEntity = await _printRepository.GetByIdAsync(dto.PrintId);
            if (printEntity == null)
            {
                throw new Exception($"找不到業務報表列印資料: {dto.PrintId}");
            }

            // 驗證業務邏輯：扣款數量需在動作事件有值時才能輸入
            if (dto.DeductionQty.HasValue && string.IsNullOrEmpty(dto.ActEvent))
            {
                throw new Exception("扣款數量需在動作事件有值時才能輸入");
            }

            // 驗證業務邏輯：扣款數量預設為空時，扣款數量欄位應為空
            if (dto.DeductionQtyDefaultEmpty == "Y" && dto.DeductionQty.HasValue)
            {
                throw new Exception("扣款數量預設為空時，扣款數量欄位應為空");
            }

            var entity = new BusinessReportPrintDetail
            {
                PrintId = dto.PrintId,
                LeaveId = dto.LeaveId,
                LeaveName = dto.LeaveName,
                ActEvent = dto.ActEvent,
                DeductionQty = dto.DeductionQtyDefaultEmpty == "Y" ? null : dto.DeductionQty,
                DeductionQtyDefaultEmpty = dto.DeductionQtyDefaultEmpty ?? "N",
                Status = "1",
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var tKey = await _repository.CreateAsync(entity);
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增業務報表列印明細失敗", ex);
            throw;
        }
    }

    public async Task<bool> UpdateBusinessReportPrintDetailAsync(long tKey, UpdateBusinessReportPrintDetailDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new Exception($"找不到業務報表列印明細資料: {tKey}");
            }

            // 驗證業務邏輯：扣款數量需在動作事件有值時才能輸入
            if (dto.DeductionQty.HasValue && string.IsNullOrEmpty(dto.ActEvent))
            {
                throw new Exception("扣款數量需在動作事件有值時才能輸入");
            }

            // 驗證業務邏輯：扣款數量預設為空時，扣款數量欄位應為空
            if (dto.DeductionQtyDefaultEmpty == "Y" && dto.DeductionQty.HasValue)
            {
                throw new Exception("扣款數量預設為空時，扣款數量欄位應為空");
            }

            entity.LeaveId = dto.LeaveId ?? entity.LeaveId;
            entity.LeaveName = dto.LeaveName ?? entity.LeaveName;
            entity.ActEvent = dto.ActEvent ?? entity.ActEvent;
            entity.DeductionQty = dto.DeductionQtyDefaultEmpty == "Y" ? null : (dto.DeductionQty ?? entity.DeductionQty);
            entity.DeductionQtyDefaultEmpty = dto.DeductionQtyDefaultEmpty ?? entity.DeductionQtyDefaultEmpty;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            return await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改業務報表列印明細失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<bool> DeleteBusinessReportPrintDetailAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new Exception($"找不到業務報表列印明細資料: {tKey}");
            }

            return await _repository.DeleteAsync(tKey);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除業務報表列印明細失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<BatchProcessBusinessReportPrintDetailResultDto> BatchProcessAsync(BatchProcessBusinessReportPrintDetailDto dto)
    {
        try
        {
            var createEntities = new List<BusinessReportPrintDetail>();
            var updateEntities = new List<BusinessReportPrintDetail>();

            // 準備新增資料
            foreach (var createDto in dto.CreateItems)
            {
                // 驗證 PrintId 是否存在
                var printEntity = await _printRepository.GetByIdAsync(createDto.PrintId);
                if (printEntity == null)
                {
                    throw new Exception($"找不到業務報表列印資料: {createDto.PrintId}");
                }

                // 驗證業務邏輯
                if (createDto.DeductionQty.HasValue && string.IsNullOrEmpty(createDto.ActEvent))
                {
                    throw new Exception("扣款數量需在動作事件有值時才能輸入");
                }

                if (createDto.DeductionQtyDefaultEmpty == "Y" && createDto.DeductionQty.HasValue)
                {
                    throw new Exception("扣款數量預設為空時，扣款數量欄位應為空");
                }

                createEntities.Add(new BusinessReportPrintDetail
                {
                    PrintId = createDto.PrintId,
                    LeaveId = createDto.LeaveId,
                    LeaveName = createDto.LeaveName,
                    ActEvent = createDto.ActEvent,
                    DeductionQty = createDto.DeductionQtyDefaultEmpty == "Y" ? null : createDto.DeductionQty,
                    DeductionQtyDefaultEmpty = createDto.DeductionQtyDefaultEmpty ?? "N",
                    Status = "1",
                    CreatedBy = GetCurrentUserId(),
                    CreatedAt = DateTime.Now,
                    UpdatedBy = GetCurrentUserId(),
                    UpdatedAt = DateTime.Now
                });
            }

            // 準備修改資料
            foreach (var updateDto in dto.UpdateItems)
            {
                var entity = await _repository.GetByIdAsync(updateDto.TKey);
                if (entity == null)
                {
                    continue;
                }

                // 驗證業務邏輯
                var actEvent = updateDto.ActEvent ?? entity.ActEvent;
                var deductionQty = updateDto.DeductionQty ?? entity.DeductionQty;
                var deductionQtyDefaultEmpty = updateDto.DeductionQtyDefaultEmpty ?? entity.DeductionQtyDefaultEmpty;

                if (deductionQty.HasValue && string.IsNullOrEmpty(actEvent))
                {
                    throw new Exception("扣款數量需在動作事件有值時才能輸入");
                }

                if (deductionQtyDefaultEmpty == "Y" && deductionQty.HasValue)
                {
                    throw new Exception("扣款數量預設為空時，扣款數量欄位應為空");
                }

                entity.LeaveId = updateDto.LeaveId ?? entity.LeaveId;
                entity.LeaveName = updateDto.LeaveName ?? entity.LeaveName;
                entity.ActEvent = actEvent;
                entity.DeductionQty = deductionQtyDefaultEmpty == "Y" ? null : deductionQty;
                entity.DeductionQtyDefaultEmpty = deductionQtyDefaultEmpty;
                entity.UpdatedBy = GetCurrentUserId();
                entity.UpdatedAt = DateTime.Now;

                updateEntities.Add(entity);
            }

            var result = await _repository.BatchProcessAsync(createEntities, updateEntities, dto.DeleteTKeys);

            return new BatchProcessBusinessReportPrintDetailResultDto
            {
                CreateCount = result.CreateCount,
                UpdateCount = result.UpdateCount,
                DeleteCount = result.DeleteCount,
                FailCount = result.FailCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("批次處理業務報表列印明細失敗", ex);
            throw;
        }
    }

    /// <summary>
    /// 將 Entity 轉換為 DTO
    /// </summary>
    private BusinessReportPrintDetailDto MapToDto(BusinessReportPrintDetail entity)
    {
        return new BusinessReportPrintDetailDto
        {
            TKey = entity.TKey,
            PrintId = entity.PrintId,
            LeaveId = entity.LeaveId,
            LeaveName = entity.LeaveName,
            ActEvent = entity.ActEvent,
            DeductionQty = entity.DeductionQty,
            DeductionQtyDefaultEmpty = entity.DeductionQtyDefaultEmpty,
            Status = entity.Status,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

