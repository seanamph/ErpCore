using ErpCore.Application.DTOs.StandardModule;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.StandardModule;
using ErpCore.Infrastructure.Repositories.StandardModule;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.StandardModule;

/// <summary>
/// STD5000 會員服務實作 (SYS5210-SYS52A0 - 會員管理)
/// </summary>
public class Std5000MemberService : BaseService, IStd5000MemberService
{
    private readonly IStd5000MemberRepository _repository;
    private readonly IStd5000MemberPointRepository _pointRepository;

    public Std5000MemberService(
        IStd5000MemberRepository repository,
        IStd5000MemberPointRepository pointRepository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _pointRepository = pointRepository;
    }

    public async Task<PagedResult<Std5000MemberDto>> GetStd5000MemberListAsync(Std5000MemberQueryDto query)
    {
        try
        {
            var repositoryQuery = new Std5000MemberQuery
            {
                MemberId = query.MemberId,
                MemberName = query.MemberName,
                MemberType = query.MemberType,
                Status = query.Status,
                ShopId = query.ShopId,
                Keyword = query.Keyword,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(MapToDto).ToList();

            return new PagedResult<Std5000MemberDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢STD5000會員列表失敗", ex);
            throw;
        }
    }

    public async Task<Std5000MemberDto?> GetStd5000MemberByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            return entity != null ? MapToDto(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢STD5000會員失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<Std5000MemberDto?> GetStd5000MemberByMemberIdAsync(string memberId)
    {
        try
        {
            var entity = await _repository.GetByMemberIdAsync(memberId);
            return entity != null ? MapToDto(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢STD5000會員失敗: {memberId}", ex);
            throw;
        }
    }

    public async Task<long> CreateStd5000MemberAsync(CreateStd5000MemberDto dto)
    {
        try
        {
            // 檢查會員編號是否已存在
            var existing = await _repository.GetByMemberIdAsync(dto.MemberId);
            if (existing != null)
            {
                throw new InvalidOperationException($"會員編號已存在: {dto.MemberId}");
            }

            var entity = new Std5000Member
            {
                MemberId = dto.MemberId,
                MemberName = dto.MemberName,
                MemberType = dto.MemberType,
                IdCard = dto.IdCard,
                Phone = dto.Phone,
                Email = dto.Email,
                Address = dto.Address,
                BirthDate = dto.BirthDate,
                JoinDate = DateTime.Now,
                Points = dto.Points,
                Status = dto.Status,
                ShopId = dto.ShopId,
                Memo = dto.Memo,
                CreatedBy = _userContext.UserId,
                CreatedAt = DateTime.Now,
                UpdatedBy = _userContext.UserId,
                UpdatedAt = DateTime.Now
            };

            var tKey = await _repository.CreateAsync(entity);
            _logger.LogInfo($"新增STD5000會員成功: {dto.MemberId}");
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增STD5000會員失敗", ex);
            throw;
        }
    }

    public async Task UpdateStd5000MemberAsync(long tKey, UpdateStd5000MemberDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"會員不存在: {tKey}");
            }

            entity.MemberName = dto.MemberName;
            entity.MemberType = dto.MemberType;
            entity.IdCard = dto.IdCard;
            entity.Phone = dto.Phone;
            entity.Email = dto.Email;
            entity.Address = dto.Address;
            entity.BirthDate = dto.BirthDate;
            entity.Points = dto.Points;
            entity.Status = dto.Status;
            entity.ShopId = dto.ShopId;
            entity.Memo = dto.Memo;
            entity.UpdatedBy = _userContext.UserId;
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
            _logger.LogInfo($"修改STD5000會員成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改STD5000會員失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteStd5000MemberAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"會員不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
            _logger.LogInfo($"刪除STD5000會員成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除STD5000會員失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<PagedResult<Std5000MemberPointDto>> GetMemberPointsAsync(Std5000MemberPointQueryDto query)
    {
        try
        {
            var repositoryQuery = new Std5000MemberPointQuery
            {
                MemberId = query.MemberId,
                TransType = query.TransType,
                StartDate = query.StartDate,
                EndDate = query.EndDate,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder
            };

            var items = await _pointRepository.QueryAsync(repositoryQuery);
            var totalCount = await _pointRepository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(MapPointToDto).ToList();

            return new PagedResult<Std5000MemberPointDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢STD5000會員積分列表失敗", ex);
            throw;
        }
    }

    public async Task<long> AddMemberPointAsync(CreateStd5000MemberPointDto dto)
    {
        try
        {
            // 檢查會員是否存在
            var member = await _repository.GetByMemberIdAsync(dto.MemberId);
            if (member == null)
            {
                throw new InvalidOperationException($"會員不存在: {dto.MemberId}");
            }

            var entity = new Std5000MemberPoint
            {
                MemberId = dto.MemberId,
                TransDate = dto.TransDate,
                TransType = dto.TransType,
                Points = dto.Points,
                TransId = dto.TransId,
                Memo = dto.Memo,
                CreatedBy = _userContext.UserId,
                CreatedAt = DateTime.Now
            };

            var tKey = await _pointRepository.CreateAsync(entity);

            // 更新會員積分
            if (dto.TransType == "EARN")
            {
                member.Points += dto.Points;
            }
            else if (dto.TransType == "USE")
            {
                member.Points -= dto.Points;
            }
            member.UpdatedBy = _userContext.UserId;
            member.UpdatedAt = DateTime.Now;
            await _repository.UpdateAsync(member);

            _logger.LogInfo($"新增STD5000會員積分成功: {dto.MemberId}");
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增STD5000會員積分失敗", ex);
            throw;
        }
    }

    private Std5000MemberDto MapToDto(Std5000Member entity)
    {
        return new Std5000MemberDto
        {
            TKey = entity.TKey,
            MemberId = entity.MemberId,
            MemberName = entity.MemberName,
            MemberType = entity.MemberType,
            IdCard = entity.IdCard,
            Phone = entity.Phone,
            Email = entity.Email,
            Address = entity.Address,
            BirthDate = entity.BirthDate,
            JoinDate = entity.JoinDate,
            Points = entity.Points,
            Status = entity.Status,
            ShopId = entity.ShopId,
            Memo = entity.Memo,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }

    private Std5000MemberPointDto MapPointToDto(Std5000MemberPoint entity)
    {
        return new Std5000MemberPointDto
        {
            TKey = entity.TKey,
            MemberId = entity.MemberId,
            TransDate = entity.TransDate,
            TransType = entity.TransType,
            Points = entity.Points,
            TransId = entity.TransId,
            Memo = entity.Memo,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt
        };
    }
}

