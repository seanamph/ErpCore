using ErpCore.Application.DTOs.CustomerCustom;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.CustomerCustom;
using ErpCore.Infrastructure.Repositories.CustomerCustom;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.CustomerCustom;

/// <summary>
/// CUS3000.ESKYLAND 會員服務實作
/// </summary>
public class Cus3000EskylandMemberService : BaseService, ICus3000EskylandMemberService
{
    private readonly ICus3000EskylandMemberRepository _repository;

    public Cus3000EskylandMemberService(
        ICus3000EskylandMemberRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<Cus3000EskylandMemberDto>> GetCus3000EskylandMemberListAsync(Cus3000EskylandMemberQueryDto query)
    {
        try
        {
            var repositoryQuery = new Cus3000EskylandMemberQuery
            {
                MemberId = query.MemberId,
                MemberName = query.MemberName,
                CardNo = query.CardNo,
                Phone = query.Phone,
                Status = query.Status,
                Keyword = query.Keyword,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(MapToDto).ToList();

            return new PagedResult<Cus3000EskylandMemberDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢CUS3000.ESKYLAND會員列表失敗", ex);
            throw;
        }
    }

    public async Task<Cus3000EskylandMemberDto?> GetCus3000EskylandMemberByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            return entity != null ? MapToDto(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢CUS3000.ESKYLAND會員失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<Cus3000EskylandMemberDto?> GetCus3000EskylandMemberByMemberIdAsync(string memberId)
    {
        try
        {
            var entity = await _repository.GetByMemberIdAsync(memberId);
            return entity != null ? MapToDto(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢CUS3000.ESKYLAND會員失敗: {memberId}", ex);
            throw;
        }
    }

    public async Task<Cus3000EskylandMemberDto?> GetCus3000EskylandMemberByCardNoAsync(string cardNo)
    {
        try
        {
            var entity = await _repository.GetByCardNoAsync(cardNo);
            return entity != null ? MapToDto(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢CUS3000.ESKYLAND會員失敗: {cardNo}", ex);
            throw;
        }
    }

    public async Task<long> CreateCus3000EskylandMemberAsync(CreateCus3000EskylandMemberDto dto)
    {
        try
        {
            // 檢查會員編號是否已存在
            var existing = await _repository.GetByMemberIdAsync(dto.MemberId);
            if (existing != null)
            {
                throw new InvalidOperationException($"會員編號已存在: {dto.MemberId}");
            }

            // 檢查會員卡號是否已存在
            if (!string.IsNullOrEmpty(dto.CardNo))
            {
                var existingCard = await _repository.GetByCardNoAsync(dto.CardNo);
                if (existingCard != null)
                {
                    throw new InvalidOperationException($"會員卡號已存在: {dto.CardNo}");
                }
            }

            var entity = new Cus3000EskylandMember
            {
                MemberId = dto.MemberId,
                MemberName = dto.MemberName,
                CardNo = dto.CardNo,
                EskylandSpecificField = dto.EskylandSpecificField,
                Phone = dto.Phone,
                Email = dto.Email,
                Address = dto.Address,
                Status = dto.Status,
                CreatedBy = _userContext.UserId,
                CreatedAt = DateTime.Now,
                UpdatedBy = _userContext.UserId,
                UpdatedAt = DateTime.Now
            };

            var tKey = await _repository.CreateAsync(entity);
            _logger.LogInfo($"新增CUS3000.ESKYLAND會員成功: {tKey}");
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增CUS3000.ESKYLAND會員失敗", ex);
            throw;
        }
    }

    public async Task UpdateCus3000EskylandMemberAsync(long tKey, UpdateCus3000EskylandMemberDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"會員不存在: {tKey}");
            }

            // 檢查會員卡號是否已被其他會員使用
            if (!string.IsNullOrEmpty(dto.CardNo) && dto.CardNo != entity.CardNo)
            {
                var existingCard = await _repository.GetByCardNoAsync(dto.CardNo);
                if (existingCard != null && existingCard.TKey != tKey)
                {
                    throw new InvalidOperationException($"會員卡號已被使用: {dto.CardNo}");
                }
            }

            entity.MemberName = dto.MemberName;
            entity.CardNo = dto.CardNo;
            entity.EskylandSpecificField = dto.EskylandSpecificField;
            entity.Phone = dto.Phone;
            entity.Email = dto.Email;
            entity.Address = dto.Address;
            entity.Status = dto.Status;
            entity.UpdatedBy = _userContext.UserId;
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
            _logger.LogInfo($"修改CUS3000.ESKYLAND會員成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改CUS3000.ESKYLAND會員失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteCus3000EskylandMemberAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"會員不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
            _logger.LogInfo($"刪除CUS3000.ESKYLAND會員成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除CUS3000.ESKYLAND會員失敗: {tKey}", ex);
            throw;
        }
    }

    private Cus3000EskylandMemberDto MapToDto(Cus3000EskylandMember entity)
    {
        return new Cus3000EskylandMemberDto
        {
            TKey = entity.TKey,
            MemberId = entity.MemberId,
            MemberName = entity.MemberName,
            CardNo = entity.CardNo,
            EskylandSpecificField = entity.EskylandSpecificField,
            Phone = entity.Phone,
            Email = entity.Email,
            Address = entity.Address,
            Status = entity.Status,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

