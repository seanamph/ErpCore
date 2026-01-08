using ErpCore.Application.DTOs.BasicData;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.BasicData;
using ErpCore.Infrastructure.Repositories.BasicData;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.BasicData;

/// <summary>
/// 商品分類服務實作
/// </summary>
public class ProductCategoryService : BaseService, IProductCategoryService
{
    private readonly IProductCategoryRepository _repository;

    public ProductCategoryService(
        IProductCategoryRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<ProductCategoryDto>> GetProductCategoriesAsync(ProductCategoryQueryDto query)
    {
        try
        {
            var repositoryQuery = new ProductCategoryQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                ClassId = query.ClassId,
                ClassName = query.ClassName,
                ClassMode = query.ClassMode,
                ClassType = query.ClassType,
                BClassId = query.BClassId,
                MClassId = query.MClassId,
                ParentTKey = query.ParentTKey,
                Status = query.Status
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => new ProductCategoryDto
            {
                TKey = x.TKey,
                ClassId = x.ClassId,
                ClassName = x.ClassName,
                ClassType = x.ClassType,
                ClassMode = x.ClassMode,
                BClassId = x.BClassId,
                MClassId = x.MClassId,
                ParentTKey = x.ParentTKey,
                StypeId = x.StypeId,
                StypeId2 = x.StypeId2,
                DepreStypeId = x.DepreStypeId,
                DepreStypeId2 = x.DepreStypeId2,
                StypeTax = x.StypeTax,
                ItemCount = x.ItemCount,
                Status = x.Status,
                Notes = x.Notes,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();

            return new PagedResult<ProductCategoryDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢商品分類列表失敗", ex);
            throw;
        }
    }

    public async Task<ProductCategoryDto> GetProductCategoryByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"商品分類不存在: {tKey}");
            }

            return new ProductCategoryDto
            {
                TKey = entity.TKey,
                ClassId = entity.ClassId,
                ClassName = entity.ClassName,
                ClassType = entity.ClassType,
                ClassMode = entity.ClassMode,
                BClassId = entity.BClassId,
                MClassId = entity.MClassId,
                ParentTKey = entity.ParentTKey,
                StypeId = entity.StypeId,
                StypeId2 = entity.StypeId2,
                DepreStypeId = entity.DepreStypeId,
                DepreStypeId2 = entity.DepreStypeId2,
                StypeTax = entity.StypeTax,
                ItemCount = entity.ItemCount,
                Status = entity.Status,
                Notes = entity.Notes,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt,
                UpdatedBy = entity.UpdatedBy,
                UpdatedAt = entity.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢商品分類失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<List<ProductCategoryTreeDto>> GetProductCategoryTreeAsync(ProductCategoryTreeQueryDto query)
    {
        try
        {
            var repositoryQuery = new ProductCategoryTreeQuery
            {
                ClassType = query.ClassType,
                Status = query.Status
            };

            var items = await _repository.GetTreeAsync(repositoryQuery);

            // 建立樹狀結構
            var tree = new List<ProductCategoryTreeDto>();
            var itemMap = items.ToDictionary(x => x.TKey, x => new ProductCategoryTreeDto
            {
                TKey = x.TKey,
                ClassId = x.ClassId,
                ClassName = x.ClassName,
                ClassType = x.ClassType,
                ClassMode = x.ClassMode,
                BClassId = x.BClassId,
                MClassId = x.MClassId,
                ParentTKey = x.ParentTKey,
                StypeId = x.StypeId,
                StypeId2 = x.StypeId2,
                DepreStypeId = x.DepreStypeId,
                DepreStypeId2 = x.DepreStypeId2,
                StypeTax = x.StypeTax,
                ItemCount = x.ItemCount,
                Status = x.Status,
                Notes = x.Notes,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt,
                Children = new List<ProductCategoryTreeDto>()
            });

            foreach (var item in itemMap.Values)
            {
                if (item.ParentTKey.HasValue && itemMap.ContainsKey(item.ParentTKey.Value))
                {
                    itemMap[item.ParentTKey.Value].Children.Add(item);
                }
                else
                {
                    tree.Add(item);
                }
            }

            return tree;
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢商品分類樹狀結構失敗", ex);
            throw;
        }
    }

    public async Task<long> CreateProductCategoryAsync(CreateProductCategoryDto dto)
    {
        try
        {
            // 驗證資料
            ValidateCreateDto(dto);

            // 檢查是否已存在
            var exists = await _repository.ExistsAsync(dto.ClassId, dto.ClassMode, dto.ParentTKey);
            if (exists)
            {
                throw new InvalidOperationException($"商品分類已存在: {dto.ClassId}");
            }

            var entity = new ProductCategory
            {
                ClassId = dto.ClassId,
                ClassName = dto.ClassName,
                ClassType = dto.ClassType ?? "1",
                ClassMode = dto.ClassMode,
                BClassId = dto.BClassId,
                MClassId = dto.MClassId,
                ParentTKey = dto.ParentTKey,
                StypeId = dto.StypeId,
                StypeId2 = dto.StypeId2,
                DepreStypeId = dto.DepreStypeId,
                DepreStypeId2 = dto.DepreStypeId2,
                StypeTax = dto.StypeTax,
                ItemCount = dto.ItemCount ?? 0,
                Status = dto.Status ?? "A",
                Notes = dto.Notes,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now,
                CreatedPriority = null,
                CreatedGroup = GetCurrentOrgId()
            };

            var result = await _repository.CreateAsync(entity);

            return result.TKey;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增商品分類失敗: {dto.ClassId}", ex);
            throw;
        }
    }

    public async Task UpdateProductCategoryAsync(long tKey, UpdateProductCategoryDto dto)
    {
        try
        {
            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"商品分類不存在: {tKey}");
            }

            entity.ClassName = dto.ClassName;
            entity.ClassType = dto.ClassType;
            entity.BClassId = dto.BClassId;
            entity.MClassId = dto.MClassId;
            entity.ParentTKey = dto.ParentTKey;
            entity.StypeId = dto.StypeId;
            entity.StypeId2 = dto.StypeId2;
            entity.DepreStypeId = dto.DepreStypeId;
            entity.DepreStypeId2 = dto.DepreStypeId2;
            entity.StypeTax = dto.StypeTax;
            entity.ItemCount = dto.ItemCount;
            entity.Status = dto.Status ?? "A";
            entity.Notes = dto.Notes;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改商品分類失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteProductCategoryAsync(long tKey)
    {
        try
        {
            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"商品分類不存在: {tKey}");
            }

            // 檢查是否有子分類
            var hasChildren = await _repository.HasChildrenAsync(tKey);
            if (hasChildren)
            {
                throw new InvalidOperationException($"商品分類有子分類，無法刪除: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除商品分類失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteProductCategoriesBatchAsync(BatchDeleteProductCategoryDto dto)
    {
        try
        {
            foreach (var tKey in dto.TKeys)
            {
                await DeleteProductCategoryAsync(tKey);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("批次刪除商品分類失敗", ex);
            throw;
        }
    }

    public async Task<List<ProductCategoryDto>> GetBClassListAsync(ProductCategoryListQueryDto query)
    {
        try
        {
            var repositoryQuery = new ProductCategoryListQuery
            {
                ClassType = query.ClassType,
                Status = query.Status
            };

            var items = await _repository.GetBClassListAsync(repositoryQuery);

            return items.Select(x => new ProductCategoryDto
            {
                TKey = x.TKey,
                ClassId = x.ClassId,
                ClassName = x.ClassName,
                ClassType = x.ClassType,
                ClassMode = x.ClassMode,
                BClassId = x.BClassId,
                MClassId = x.MClassId,
                ParentTKey = x.ParentTKey,
                StypeId = x.StypeId,
                StypeId2 = x.StypeId2,
                DepreStypeId = x.DepreStypeId,
                DepreStypeId2 = x.DepreStypeId2,
                StypeTax = x.StypeTax,
                ItemCount = x.ItemCount,
                Status = x.Status,
                Notes = x.Notes,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢大分類列表失敗", ex);
            throw;
        }
    }

    public async Task<List<ProductCategoryDto>> GetMClassListAsync(ProductCategoryListQueryDto query)
    {
        try
        {
            var repositoryQuery = new ProductCategoryListQuery
            {
                BClassId = query.BClassId,
                ClassType = query.ClassType,
                Status = query.Status
            };

            var items = await _repository.GetMClassListAsync(repositoryQuery);

            return items.Select(x => new ProductCategoryDto
            {
                TKey = x.TKey,
                ClassId = x.ClassId,
                ClassName = x.ClassName,
                ClassType = x.ClassType,
                ClassMode = x.ClassMode,
                BClassId = x.BClassId,
                MClassId = x.MClassId,
                ParentTKey = x.ParentTKey,
                StypeId = x.StypeId,
                StypeId2 = x.StypeId2,
                DepreStypeId = x.DepreStypeId,
                DepreStypeId2 = x.DepreStypeId2,
                StypeTax = x.StypeTax,
                ItemCount = x.ItemCount,
                Status = x.Status,
                Notes = x.Notes,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢中分類列表失敗", ex);
            throw;
        }
    }

    public async Task<List<ProductCategoryDto>> GetSClassListAsync(ProductCategoryListQueryDto query)
    {
        try
        {
            var repositoryQuery = new ProductCategoryListQuery
            {
                BClassId = query.BClassId,
                MClassId = query.MClassId,
                ClassType = query.ClassType,
                Status = query.Status
            };

            var items = await _repository.GetSClassListAsync(repositoryQuery);

            return items.Select(x => new ProductCategoryDto
            {
                TKey = x.TKey,
                ClassId = x.ClassId,
                ClassName = x.ClassName,
                ClassType = x.ClassType,
                ClassMode = x.ClassMode,
                BClassId = x.BClassId,
                MClassId = x.MClassId,
                ParentTKey = x.ParentTKey,
                StypeId = x.StypeId,
                StypeId2 = x.StypeId2,
                DepreStypeId = x.DepreStypeId,
                DepreStypeId2 = x.DepreStypeId2,
                StypeTax = x.StypeTax,
                ItemCount = x.ItemCount,
                Status = x.Status,
                Notes = x.Notes,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢小分類列表失敗", ex);
            throw;
        }
    }

    public async Task UpdateStatusAsync(long tKey, UpdateProductCategoryStatusDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"商品分類不存在: {tKey}");
            }

            entity.Status = dto.Status;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新商品分類狀態失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task UpdateItemCountAsync(long tKey, UpdateProductCategoryItemCountDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"商品分類不存在: {tKey}");
            }

            entity.ItemCount = dto.ItemCount;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新商品分類項目個數失敗: {tKey}", ex);
            throw;
        }
    }

    private void ValidateCreateDto(CreateProductCategoryDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.ClassId))
        {
            throw new ArgumentException("分類代碼不能為空");
        }

        if (string.IsNullOrWhiteSpace(dto.ClassName))
        {
            throw new ArgumentException("分類名稱不能為空");
        }

        if (string.IsNullOrWhiteSpace(dto.ClassMode))
        {
            throw new ArgumentException("分類區分不能為空");
        }

        if (dto.ClassMode != "1" && dto.ClassMode != "2" && dto.ClassMode != "3")
        {
            throw new ArgumentException("分類區分必須為 1 (大分類)、2 (中分類) 或 3 (小分類)");
        }

        if (!string.IsNullOrEmpty(dto.Status) && dto.Status != "A" && dto.Status != "I")
        {
            throw new ArgumentException("狀態值必須為 A (啟用) 或 I (停用)");
        }
    }
}

