using System.Data;
using ErpCore.Application.DTOs.Loyalty;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Loyalty;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.Loyalty;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Loyalty;

/// <summary>
/// 忠誠度系統維護服務實作 (LPS - 忠誠度系統維護)
/// </summary>
public class LoyaltyMaintenanceService : BaseService, ILoyaltyMaintenanceService
{
    private readonly ILoyaltyPointTransactionRepository _transactionRepository;
    private readonly ILoyaltyMemberRepository _memberRepository;
    private readonly IDbConnectionFactory _connectionFactory;

    public LoyaltyMaintenanceService(
        ILoyaltyPointTransactionRepository transactionRepository,
        ILoyaltyMemberRepository memberRepository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _transactionRepository = transactionRepository;
        _memberRepository = memberRepository;
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<LoyaltyPointTransactionDto>> GetTransactionsAsync(LoyaltyPointTransactionQueryDto query)
    {
        try
        {
            var repositoryQuery = new LoyaltyPointTransactionQuery
            {
                RRN = query.RRN,
                CardNo = query.CardNo,
                TransType = query.TransType,
                Status = query.Status,
                TransTimeFrom = query.TransTimeFrom,
                TransTimeTo = query.TransTimeTo,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder
            };

            var items = await _transactionRepository.QueryAsync(repositoryQuery);
            var totalCount = await _transactionRepository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(MapToTransactionDto).ToList();

            return new PagedResult<LoyaltyPointTransactionDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢忠誠度點數交易列表失敗", ex);
            throw;
        }
    }

    public async Task<LoyaltyPointTransactionDto?> GetTransactionByRrnAsync(string rrn)
    {
        try
        {
            var entity = await _transactionRepository.GetByRrnAsync(rrn);
            return entity != null ? MapToTransactionDto(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢忠誠度點數交易失敗: {rrn}", ex);
            throw;
        }
    }

    public async Task<long> CreateTransactionAsync(CreateLoyaltyPointTransactionDto dto)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                // 產生交易編號
                var rrn = await _transactionRepository.GenerateRrnAsync();

                // 建立交易記錄
                var transactionEntity = new LoyaltyPointTransaction
                {
                    RRN = rrn,
                    CardNo = dto.CardNo,
                    TraceNo = dto.TraceNo,
                    ExpDate = dto.ExpDate,
                    AwardPoints = dto.AwardPoints,
                    RedeemPoints = dto.RedeemPoints,
                    Amount = dto.Amount,
                    Invoice = dto.Invoice,
                    TransType = dto.TransType,
                    TxnType = dto.TxnType,
                    ForceDate = dto.ForceDate,
                    TransTime = DateTime.Now,
                    Status = "SUCCESS",
                    CreatedBy = _userContext.UserId,
                    CreatedAt = DateTime.Now,
                    UpdatedBy = _userContext.UserId,
                    UpdatedAt = DateTime.Now
                };

                var tKey = await _transactionRepository.CreateAsync(transactionEntity);

                // 更新會員點數
                var member = await _memberRepository.GetByCardNoAsync(dto.CardNo);
                if (member != null)
                {
                    var newTotalPoints = member.TotalPoints + dto.AwardPoints - dto.RedeemPoints;
                    var newAvailablePoints = member.AvailablePoints + dto.AwardPoints - dto.RedeemPoints;

                    if (newAvailablePoints < 0)
                    {
                        throw new Exception("可用點數不足");
                    }

                    await _memberRepository.UpdatePointsAsync(dto.CardNo, newTotalPoints, newAvailablePoints);
                }

                transaction.Commit();
                return tKey;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增忠誠度點數交易失敗: {dto.CardNo}", ex);
            throw;
        }
    }

    public async Task VoidTransactionAsync(string rrn, VoidLoyaltyPointTransactionDto dto)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                // 查詢原交易
                var originalTransaction = await _transactionRepository.GetByRrnAsync(rrn);
                if (originalTransaction == null)
                {
                    throw new Exception($"交易不存在: {rrn}");
                }

                // 取消交易
                await _transactionRepository.VoidTransactionAsync(rrn, dto.ReversalFlag, dto.VoidFlag);

                // 回滾會員點數
                var member = await _memberRepository.GetByCardNoAsync(originalTransaction.CardNo);
                if (member != null)
                {
                    var newTotalPoints = member.TotalPoints - originalTransaction.AwardPoints + originalTransaction.RedeemPoints;
                    var newAvailablePoints = member.AvailablePoints - originalTransaction.AwardPoints + originalTransaction.RedeemPoints;

                    await _memberRepository.UpdatePointsAsync(originalTransaction.CardNo, newTotalPoints, newAvailablePoints);
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"取消忠誠度點數交易失敗: {rrn}", ex);
            throw;
        }
    }

    public async Task<PagedResult<LoyaltyMemberDto>> GetMembersAsync(LoyaltyMemberQueryDto query)
    {
        try
        {
            var repositoryQuery = new LoyaltyMemberQuery
            {
                CardNo = query.CardNo,
                MemberName = query.MemberName,
                Status = query.Status,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder
            };

            var items = await _memberRepository.QueryAsync(repositoryQuery);
            var totalCount = await _memberRepository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(MapToMemberDto).ToList();

            return new PagedResult<LoyaltyMemberDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢忠誠度會員列表失敗", ex);
            throw;
        }
    }

    public async Task<LoyaltyMemberDto?> GetMemberByCardNoAsync(string cardNo)
    {
        try
        {
            var entity = await _memberRepository.GetByCardNoAsync(cardNo);
            return entity != null ? MapToMemberDto(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢忠誠度會員失敗: {cardNo}", ex);
            throw;
        }
    }

    public async Task<LoyaltyMemberPointsDto?> GetMemberPointsAsync(string cardNo)
    {
        try
        {
            var member = await _memberRepository.GetByCardNoAsync(cardNo);
            if (member == null)
            {
                return null;
            }

            return new LoyaltyMemberPointsDto
            {
                CardNo = member.CardNo,
                TotalPoints = member.TotalPoints,
                AvailablePoints = member.AvailablePoints,
                ExpDate = member.ExpDate
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢會員點數失敗: {cardNo}", ex);
            throw;
        }
    }

    public async Task<string> CreateMemberAsync(CreateLoyaltyMemberDto dto)
    {
        try
        {
            var entity = new LoyaltyMember
            {
                CardNo = dto.CardNo,
                MemberName = dto.MemberName,
                Phone = dto.Phone,
                Email = dto.Email,
                ExpDate = dto.ExpDate,
                Status = dto.Status,
                TotalPoints = 0,
                AvailablePoints = 0,
                CreatedBy = _userContext.UserId,
                CreatedAt = DateTime.Now,
                UpdatedBy = _userContext.UserId,
                UpdatedAt = DateTime.Now
            };

            return await _memberRepository.CreateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增忠誠度會員失敗: {dto.CardNo}", ex);
            throw;
        }
    }

    public async Task UpdateMemberAsync(string cardNo, UpdateLoyaltyMemberDto dto)
    {
        try
        {
            var entity = await _memberRepository.GetByCardNoAsync(cardNo);
            if (entity == null)
            {
                throw new Exception($"忠誠度會員不存在: {cardNo}");
            }

            entity.MemberName = dto.MemberName;
            entity.Phone = dto.Phone;
            entity.Email = dto.Email;
            entity.ExpDate = dto.ExpDate;
            entity.Status = dto.Status;
            entity.UpdatedBy = _userContext.UserId;
            entity.UpdatedAt = DateTime.Now;

            await _memberRepository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改忠誠度會員失敗: {cardNo}", ex);
            throw;
        }
    }

    private static LoyaltyPointTransactionDto MapToTransactionDto(LoyaltyPointTransaction entity)
    {
        return new LoyaltyPointTransactionDto
        {
            TKey = entity.TKey,
            RRN = entity.RRN,
            CardNo = entity.CardNo,
            TraceNo = entity.TraceNo,
            ExpDate = entity.ExpDate,
            AwardPoints = entity.AwardPoints,
            RedeemPoints = entity.RedeemPoints,
            ReversalFlag = entity.ReversalFlag,
            Amount = entity.Amount,
            VoidFlag = entity.VoidFlag,
            AuthCode = entity.AuthCode,
            ForceDate = entity.ForceDate,
            Invoice = entity.Invoice,
            TransType = entity.TransType,
            TxnType = entity.TxnType,
            TransTime = entity.TransTime,
            Status = entity.Status,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }

    private static LoyaltyMemberDto MapToMemberDto(LoyaltyMember entity)
    {
        return new LoyaltyMemberDto
        {
            CardNo = entity.CardNo,
            MemberName = entity.MemberName,
            Phone = entity.Phone,
            Email = entity.Email,
            TotalPoints = entity.TotalPoints,
            AvailablePoints = entity.AvailablePoints,
            ExpDate = entity.ExpDate,
            Status = entity.Status,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

