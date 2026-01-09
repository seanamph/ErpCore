using ErpCore.Application.DTOs.StoreMember;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.StoreMember;
using ErpCore.Infrastructure.Repositories.StoreMember;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.StoreMember;

/// <summary>
/// 會員查詢服務實作 (SYS3330-SYS33B0 - 會員查詢作業)
/// </summary>
public class MemberQueryService : BaseService, IMemberQueryService
{
    private readonly IMemberRepository _repository;

    public MemberQueryService(
        IMemberRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<MemberQueryResultDto>> QueryMembersAsync(MemberQueryRequestDto request)
    {
        try
        {
            _logger.LogInfo("查詢會員列表（進階查詢）");

            var query = new MemberQuery
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                SortField = request.SortField,
                SortOrder = request.SortOrder
            };

            if (request.Filters != null)
            {
                query.MemberId = request.Filters.MemberId;
                query.MemberName = request.Filters.MemberName;
                query.PersonalId = request.Filters.PersonalId;
                query.Phone = request.Filters.Phone;
                query.Mobile = request.Filters.Mobile;
                query.Email = request.Filters.Email;
                query.MemberLevel = request.Filters.MemberLevel;
                query.Status = request.Filters.Status;
                query.CardNo = request.Filters.CardNo;
            }

            var result = await _repository.QueryAsync(query);

            var dtos = result.Items.Select(x => new MemberQueryResultDto
            {
                MemberId = x.MemberId,
                MemberName = x.MemberName,
                PersonalId = x.PersonalId,
                Phone = x.Phone,
                Mobile = x.Mobile,
                Email = x.Email,
                MemberLevel = x.MemberLevel,
                Status = x.Status,
                CardNo = x.CardNo,
                JoinDate = x.JoinDate
            }).ToList();

            return new PagedResult<MemberQueryResultDto>
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

    public async Task<PagedResult<MemberTransactionDto>> GetMemberTransactionsAsync(string memberId, MemberPointQueryDto query)
    {
        try
        {
            _logger.LogInfo($"查詢會員交易記錄: {memberId}");

            // 簡化實作：實際應從 MemberTransactions 表查詢
            // 這裡暫時返回空結果，需要建立 MemberTransactionRepository
            return new PagedResult<MemberTransactionDto>
            {
                Items = new List<MemberTransactionDto>(),
                TotalCount = 0,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢會員交易記錄失敗: {memberId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<MemberExchangeLogDto>> GetExchangeLogsAsync(MemberExchangeLogQueryDto query)
    {
        try
        {
            _logger.LogInfo("查詢會員回門禮卡號補登記錄");

            // 簡化實作：實際應從 MemberExchangeLogs 表查詢
            // 這裡暫時返回空結果，需要建立 MemberExchangeLogRepository
            return new PagedResult<MemberExchangeLogDto>
            {
                Items = new List<MemberExchangeLogDto>(),
                TotalCount = 0,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢會員回門禮卡號補登記錄失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportMembersAsync(MemberExportRequestDto request)
    {
        try
        {
            _logger.LogInfo("匯出會員查詢結果");

            var queryRequest = new MemberQueryRequestDto
            {
                PageIndex = 1,
                PageSize = int.MaxValue,
                Filters = request.Filters
            };

            var result = await QueryMembersAsync(queryRequest);

            // 簡化實作：返回 CSV 格式
            var csv = "會員編號,會員姓名,身份證字號,電話,手機,電子郵件,會員等級,狀態,卡片號碼,加入日期\n";
            foreach (var item in result.Items)
            {
                csv += $"{item.MemberId},{item.MemberName},{item.PersonalId ?? ""},{item.Phone ?? ""},{item.Mobile ?? ""},{item.Email ?? ""},{item.MemberLevel ?? ""},{item.Status},{item.CardNo ?? ""},{item.JoinDate?.ToString("yyyy-MM-dd") ?? ""}\n";
            }

            return System.Text.Encoding.UTF8.GetBytes(csv);
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出會員查詢結果失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> PrintMemberReportAsync(MemberPrintRequestDto request)
    {
        try
        {
            _logger.LogInfo("列印會員報表");

            var queryRequest = new MemberQueryRequestDto
            {
                PageIndex = 1,
                PageSize = int.MaxValue,
                Filters = request.Filters
            };

            var result = await QueryMembersAsync(queryRequest);

            // 簡化實作：返回 CSV 格式（實際應生成 PDF）
            var csv = "會員報表\n";
            csv += "會員編號,會員姓名,會員等級,狀態,加入日期\n";
            foreach (var item in result.Items)
            {
                csv += $"{item.MemberId},{item.MemberName},{item.MemberLevel ?? ""},{item.Status},{item.JoinDate?.ToString("yyyy-MM-dd") ?? ""}\n";
            }

            return System.Text.Encoding.UTF8.GetBytes(csv);
        }
        catch (Exception ex)
        {
            _logger.LogError("列印會員報表失敗", ex);
            throw;
        }
    }
}

