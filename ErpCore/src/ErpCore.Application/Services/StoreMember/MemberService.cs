using ErpCore.Application.DTOs.StoreMember;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.StoreMember;
using ErpCore.Infrastructure.Repositories.StoreMember;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.StoreMember;

/// <summary>
/// 會員服務實作 (SYS3000 - 會員資料維護)
/// </summary>
public class MemberService : BaseService, IMemberService
{
    private readonly IMemberRepository _repository;

    public MemberService(
        IMemberRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<MemberDto>> GetMembersAsync(MemberQueryDto query)
    {
        try
        {
            var repositoryQuery = new MemberQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                MemberId = query.MemberId,
                MemberName = query.MemberName,
                PersonalId = query.PersonalId,
                Phone = query.Phone,
                Mobile = query.Mobile,
                Email = query.Email,
                MemberLevel = query.MemberLevel,
                Status = query.Status,
                CardNo = query.CardNo
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(MapToDto).ToList();

            return new PagedResult<MemberDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢會員列表失敗", ex);
            throw;
        }
    }

    public async Task<MemberDto> GetMemberByIdAsync(string memberId)
    {
        try
        {
            var member = await _repository.GetByIdAsync(memberId);
            if (member == null)
            {
                throw new KeyNotFoundException($"會員不存在: {memberId}");
            }

            return MapToDto(member);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢會員失敗: {memberId}", ex);
            throw;
        }
    }

    public async Task<string> CreateMemberAsync(CreateMemberDto dto)
    {
        try
        {
            if (await _repository.ExistsAsync(dto.MemberId))
            {
                throw new InvalidOperationException($"會員編號已存在: {dto.MemberId}");
            }

            var member = new Member
            {
                MemberId = dto.MemberId,
                MemberName = dto.MemberName,
                MemberNameEn = dto.MemberNameEn,
                Gender = dto.Gender,
                BirthDate = dto.BirthDate,
                PersonalId = dto.PersonalId,
                Phone = dto.Phone,
                Mobile = dto.Mobile,
                Email = dto.Email,
                Address = dto.Address,
                City = dto.City,
                Zone = dto.Zone,
                PostalCode = dto.PostalCode,
                MemberLevel = dto.MemberLevel,
                Points = dto.Points,
                TotalPoints = dto.Points,
                CardNo = dto.CardNo,
                CardType = dto.CardType,
                JoinDate = dto.JoinDate ?? DateTime.Now,
                ExpireDate = dto.ExpireDate,
                Status = dto.Status,
                Notes = dto.Notes,
                CreatedBy = GetCurrentUserId(),
                UpdatedBy = GetCurrentUserId()
            };

            await _repository.CreateAsync(member);

            _logger.LogInfo($"新增會員成功: {dto.MemberId}");
            return member.MemberId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增會員失敗: {dto.MemberId}", ex);
            throw;
        }
    }

    public async Task UpdateMemberAsync(string memberId, UpdateMemberDto dto)
    {
        try
        {
            var member = await _repository.GetByIdAsync(memberId);
            if (member == null)
            {
                throw new KeyNotFoundException($"會員不存在: {memberId}");
            }

            member.MemberName = dto.MemberName;
            member.MemberNameEn = dto.MemberNameEn;
            member.Gender = dto.Gender;
            member.BirthDate = dto.BirthDate;
            member.PersonalId = dto.PersonalId;
            member.Phone = dto.Phone;
            member.Mobile = dto.Mobile;
            member.Email = dto.Email;
            member.Address = dto.Address;
            member.City = dto.City;
            member.Zone = dto.Zone;
            member.PostalCode = dto.PostalCode;
            member.MemberLevel = dto.MemberLevel;
            member.Points = dto.Points;
            member.CardNo = dto.CardNo;
            member.CardType = dto.CardType;
            member.JoinDate = dto.JoinDate;
            member.ExpireDate = dto.ExpireDate;
            member.Status = dto.Status;
            member.Notes = dto.Notes;
            member.UpdatedBy = GetCurrentUserId();

            await _repository.UpdateAsync(member);

            _logger.LogInfo($"修改會員成功: {memberId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改會員失敗: {memberId}", ex);
            throw;
        }
    }

    public async Task DeleteMemberAsync(string memberId)
    {
        try
        {
            await _repository.DeleteAsync(memberId);
            _logger.LogInfo($"刪除會員成功: {memberId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除會員失敗: {memberId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string memberId)
    {
        try
        {
            return await _repository.ExistsAsync(memberId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查會員編號是否存在失敗: {memberId}", ex);
            throw;
        }
    }

    public async Task UpdateStatusAsync(string memberId, string status)
    {
        try
        {
            await _repository.UpdateStatusAsync(memberId, status);
            _logger.LogInfo($"更新會員狀態成功: {memberId}, 狀態: {status}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新會員狀態失敗: {memberId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<MemberPointDto>> GetMemberPointsAsync(string memberId, MemberPointQueryDto query)
    {
        try
        {
            var repositoryQuery = new MemberPointQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TransactionDateFrom = query.TransactionDateFrom,
                TransactionDateTo = query.TransactionDateTo,
                TransactionType = query.TransactionType
            };

            var result = await _repository.GetMemberPointsAsync(memberId, repositoryQuery);

            var dtos = result.Items.Select(x => new MemberPointDto
            {
                TKey = x.TKey,
                MemberId = x.MemberId,
                TransactionDate = x.TransactionDate,
                TransactionType = x.TransactionType,
                Points = x.Points,
                Balance = x.Balance,
                ReferenceNo = x.ReferenceNo,
                Description = x.Description,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt
            }).ToList();

            return new PagedResult<MemberPointDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢會員積分記錄失敗: {memberId}", ex);
            throw;
        }
    }

    private static MemberDto MapToDto(Member member)
    {
        return new MemberDto
        {
            TKey = member.TKey,
            MemberId = member.MemberId,
            MemberName = member.MemberName,
            MemberNameEn = member.MemberNameEn,
            Gender = member.Gender,
            BirthDate = member.BirthDate,
            PersonalId = member.PersonalId,
            Phone = member.Phone,
            Mobile = member.Mobile,
            Email = member.Email,
            Address = member.Address,
            City = member.City,
            Zone = member.Zone,
            PostalCode = member.PostalCode,
            MemberLevel = member.MemberLevel,
            Points = member.Points,
            TotalPoints = member.TotalPoints,
            CardNo = member.CardNo,
            CardType = member.CardType,
            JoinDate = member.JoinDate,
            ExpireDate = member.ExpireDate,
            Status = member.Status,
            PhotoPath = member.PhotoPath,
            Notes = member.Notes,
            CreatedBy = member.CreatedBy,
            CreatedAt = member.CreatedAt,
            UpdatedBy = member.UpdatedBy,
            UpdatedAt = member.UpdatedAt
        };
    }
}

