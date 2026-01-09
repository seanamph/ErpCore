using ErpCore.Application.DTOs.StoreMember;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.StoreMember;
using ErpCore.Infrastructure.Repositories.StoreMember;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.StoreMember;

/// <summary>
/// 促銷活動服務實作 (SYS3510-SYS3600 - 促銷活動維護)
/// </summary>
public class PromotionService : BaseService, IPromotionService
{
    private readonly IPromotionRepository _repository;

    public PromotionService(
        IPromotionRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<PromotionDto>> GetPromotionsAsync(PromotionQueryDto query)
    {
        try
        {
            var repositoryQuery = new PromotionQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                PromotionId = query.PromotionId,
                PromotionName = query.PromotionName,
                PromotionType = query.PromotionType,
                StartDate = query.StartDate,
                EndDate = query.EndDate,
                Status = query.Status,
                ShopId = query.ShopId
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(MapToDto).ToList();

            return new PagedResult<PromotionDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢促銷活動列表失敗", ex);
            throw;
        }
    }

    public async Task<PromotionDto> GetPromotionByIdAsync(string promotionId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(promotionId);
            if (entity == null)
            {
                throw new Exception($"促銷活動不存在: {promotionId}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢促銷活動失敗: {promotionId}", ex);
            throw;
        }
    }

    public async Task<string> CreatePromotionAsync(CreatePromotionDto dto)
    {
        try
        {
            if (await _repository.ExistsAsync(dto.PromotionId))
            {
                throw new Exception($"促銷活動編號已存在: {dto.PromotionId}");
            }

            var entity = new Promotion
            {
                PromotionId = dto.PromotionId,
                PromotionName = dto.PromotionName,
                PromotionType = dto.PromotionType,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                DiscountType = dto.DiscountType,
                DiscountValue = dto.DiscountValue,
                MinPurchaseAmount = dto.MinPurchaseAmount,
                MaxDiscountAmount = dto.MaxDiscountAmount,
                ApplicableShops = dto.ApplicableShops,
                ApplicableProducts = dto.ApplicableProducts,
                ApplicableMemberLevels = dto.ApplicableMemberLevels,
                Status = dto.Status,
                Notes = dto.Notes,
                CreatedBy = _userContext.UserId,
                UpdatedBy = _userContext.UserId
            };

            await _repository.CreateAsync(entity);
            _logger.LogInfo($"建立促銷活動成功: {dto.PromotionId}");

            return dto.PromotionId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"建立促銷活動失敗: {dto.PromotionId}", ex);
            throw;
        }
    }

    public async Task UpdatePromotionAsync(string promotionId, UpdatePromotionDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(promotionId);
            if (entity == null)
            {
                throw new Exception($"促銷活動不存在: {promotionId}");
            }

            entity.PromotionName = dto.PromotionName;
            entity.PromotionType = dto.PromotionType;
            entity.StartDate = dto.StartDate;
            entity.EndDate = dto.EndDate;
            entity.DiscountType = dto.DiscountType;
            entity.DiscountValue = dto.DiscountValue;
            entity.MinPurchaseAmount = dto.MinPurchaseAmount;
            entity.MaxDiscountAmount = dto.MaxDiscountAmount;
            entity.ApplicableShops = dto.ApplicableShops;
            entity.ApplicableProducts = dto.ApplicableProducts;
            entity.ApplicableMemberLevels = dto.ApplicableMemberLevels;
            entity.Status = dto.Status;
            entity.Notes = dto.Notes;
            entity.UpdatedBy = _userContext.UserId;

            await _repository.UpdateAsync(entity);
            _logger.LogInfo($"更新促銷活動成功: {promotionId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新促銷活動失敗: {promotionId}", ex);
            throw;
        }
    }

    public async Task DeletePromotionAsync(string promotionId)
    {
        try
        {
            await _repository.DeleteAsync(promotionId);
            _logger.LogInfo($"刪除促銷活動成功: {promotionId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除促銷活動失敗: {promotionId}", ex);
            throw;
        }
    }

    public async Task UpdateStatusAsync(string promotionId, string status)
    {
        try
        {
            await _repository.UpdateStatusAsync(promotionId, status);
            _logger.LogInfo($"更新促銷活動狀態成功: {promotionId}, 狀態: {status}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新促銷活動狀態失敗: {promotionId}", ex);
            throw;
        }
    }

    public async Task<PromotionStatisticsDto> GetStatisticsAsync(string promotionId)
    {
        try
        {
            // 簡化實作：實際應從 PromotionUsage 表查詢統計資料
            // 這裡暫時返回預設值，需要建立 PromotionUsageRepository
            return new PromotionStatisticsDto
            {
                PromotionId = promotionId,
                TotalUsage = 0,
                TotalDiscountAmount = 0,
                MemberUsage = 0,
                AverageDiscount = 0
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢促銷活動統計失敗: {promotionId}", ex);
            throw;
        }
    }

    private PromotionDto MapToDto(Promotion entity)
    {
        return new PromotionDto
        {
            TKey = entity.TKey,
            PromotionId = entity.PromotionId,
            PromotionName = entity.PromotionName,
            PromotionType = entity.PromotionType,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            DiscountType = entity.DiscountType,
            DiscountValue = entity.DiscountValue,
            MinPurchaseAmount = entity.MinPurchaseAmount,
            MaxDiscountAmount = entity.MaxDiscountAmount,
            ApplicableShops = entity.ApplicableShops,
            ApplicableProducts = entity.ApplicableProducts,
            ApplicableMemberLevels = entity.ApplicableMemberLevels,
            Status = entity.Status,
            Notes = entity.Notes,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

