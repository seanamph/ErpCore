using ErpCore.Application.DTOs.StandardModule;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.StandardModule;
using ErpCore.Infrastructure.Repositories.StandardModule;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.StandardModule;

/// <summary>
/// STD5000 交易服務實作 (SYS5310-SYS53C6 - 交易管理)
/// </summary>
public class Std5000TransactionService : BaseService, IStd5000TransactionService
{
    private readonly IStd5000TransactionRepository _repository;
    private readonly IStd5000TransactionDetailRepository _detailRepository;

    public Std5000TransactionService(
        IStd5000TransactionRepository repository,
        IStd5000TransactionDetailRepository detailRepository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _detailRepository = detailRepository;
    }

    public async Task<PagedResult<Std5000TransactionDto>> GetStd5000TransactionListAsync(Std5000TransactionQueryDto query)
    {
        try
        {
            var repositoryQuery = new Std5000TransactionQuery
            {
                TransId = query.TransId,
                TransType = query.TransType,
                MemberId = query.MemberId,
                ShopId = query.ShopId,
                Status = query.Status,
                StartDate = query.StartDate,
                EndDate = query.EndDate,
                Keyword = query.Keyword,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = new List<Std5000TransactionDto>();
            foreach (var item in items)
            {
                var dto = MapToDto(item);
                // 載入明細
                var details = await _detailRepository.GetByTransIdAsync(item.TransId);
                dto.Details = details.Select(MapDetailToDto).ToList();
                dtos.Add(dto);
            }

            return new PagedResult<Std5000TransactionDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢STD5000交易列表失敗", ex);
            throw;
        }
    }

    public async Task<Std5000TransactionDto?> GetStd5000TransactionByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null) return null;

            var dto = MapToDto(entity);
            var details = await _detailRepository.GetByTransIdAsync(entity.TransId);
            dto.Details = details.Select(MapDetailToDto).ToList();
            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢STD5000交易失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<Std5000TransactionDto?> GetStd5000TransactionByTransIdAsync(string transId)
    {
        try
        {
            var entity = await _repository.GetByTransIdAsync(transId);
            if (entity == null) return null;

            var dto = MapToDto(entity);
            var details = await _detailRepository.GetByTransIdAsync(transId);
            dto.Details = details.Select(MapDetailToDto).ToList();
            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢STD5000交易失敗: {transId}", ex);
            throw;
        }
    }

    public async Task<long> CreateStd5000TransactionAsync(CreateStd5000TransactionDto dto)
    {
        try
        {
            // 檢查交易單號是否已存在
            var existing = await _repository.GetByTransIdAsync(dto.TransId);
            if (existing != null)
            {
                throw new InvalidOperationException($"交易單號已存在: {dto.TransId}");
            }

            var entity = new Std5000Transaction
            {
                TransId = dto.TransId,
                TransDate = dto.TransDate,
                TransType = dto.TransType,
                MemberId = dto.MemberId,
                ShopId = dto.ShopId,
                Amount = dto.Amount,
                Points = dto.Points,
                Status = dto.Status,
                Memo = dto.Memo,
                CreatedBy = _userContext.UserId,
                CreatedAt = DateTime.Now,
                UpdatedBy = _userContext.UserId,
                UpdatedAt = DateTime.Now
            };

            var tKey = await _repository.CreateAsync(entity);

            // 建立明細
            if (dto.Details != null && dto.Details.Any())
            {
                foreach (var detailDto in dto.Details)
                {
                    var detail = new Std5000TransactionDetail
                    {
                        TransId = dto.TransId,
                        SeqNo = detailDto.SeqNo,
                        ProductId = detailDto.ProductId,
                        ProductName = detailDto.ProductName,
                        Qty = detailDto.Qty,
                        Price = detailDto.Price,
                        Amount = detailDto.Amount,
                        Memo = detailDto.Memo,
                        CreatedBy = _userContext.UserId,
                        CreatedAt = DateTime.Now
                    };
                    await _detailRepository.CreateAsync(detail);
                }
            }

            _logger.LogInfo($"新增STD5000交易成功: {dto.TransId}");
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增STD5000交易失敗", ex);
            throw;
        }
    }

    public async Task UpdateStd5000TransactionAsync(long tKey, UpdateStd5000TransactionDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"交易不存在: {tKey}");
            }

            entity.TransType = dto.TransType;
            entity.MemberId = dto.MemberId;
            entity.ShopId = dto.ShopId;
            entity.Amount = dto.Amount;
            entity.Points = dto.Points;
            entity.Status = dto.Status;
            entity.Memo = dto.Memo;
            entity.UpdatedBy = _userContext.UserId;
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);

            // 更新明細：先刪除舊的，再建立新的
            if (dto.Details != null)
            {
                await _detailRepository.DeleteByTransIdAsync(entity.TransId);
                foreach (var detailDto in dto.Details)
                {
                    var detail = new Std5000TransactionDetail
                    {
                        TransId = entity.TransId,
                        SeqNo = detailDto.SeqNo,
                        ProductId = detailDto.ProductId,
                        ProductName = detailDto.ProductName,
                        Qty = detailDto.Qty,
                        Price = detailDto.Price,
                        Amount = detailDto.Amount,
                        Memo = detailDto.Memo,
                        CreatedBy = _userContext.UserId,
                        CreatedAt = DateTime.Now
                    };
                    await _detailRepository.CreateAsync(detail);
                }
            }

            _logger.LogInfo($"修改STD5000交易成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改STD5000交易失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteStd5000TransactionAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"交易不存在: {tKey}");
            }

            // 先刪除明細
            await _detailRepository.DeleteByTransIdAsync(entity.TransId);
            // 再刪除主檔
            await _repository.DeleteAsync(tKey);
            _logger.LogInfo($"刪除STD5000交易成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除STD5000交易失敗: {tKey}", ex);
            throw;
        }
    }

    private Std5000TransactionDto MapToDto(Std5000Transaction entity)
    {
        return new Std5000TransactionDto
        {
            TKey = entity.TKey,
            TransId = entity.TransId,
            TransDate = entity.TransDate,
            TransType = entity.TransType,
            MemberId = entity.MemberId,
            ShopId = entity.ShopId,
            Amount = entity.Amount,
            Points = entity.Points,
            Status = entity.Status,
            Memo = entity.Memo,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }

    private Std5000TransactionDetailDto MapDetailToDto(Std5000TransactionDetail entity)
    {
        return new Std5000TransactionDetailDto
        {
            TKey = entity.TKey,
            TransId = entity.TransId,
            SeqNo = entity.SeqNo,
            ProductId = entity.ProductId,
            ProductName = entity.ProductName,
            Qty = entity.Qty,
            Price = entity.Price,
            Amount = entity.Amount,
            Memo = entity.Memo,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt
        };
    }
}

