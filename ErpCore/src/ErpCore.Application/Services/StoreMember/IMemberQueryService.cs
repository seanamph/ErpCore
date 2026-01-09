using ErpCore.Application.DTOs.StoreMember;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.StoreMember;

/// <summary>
/// 會員查詢服務介面 (SYS3330-SYS33B0 - 會員查詢作業)
/// </summary>
public interface IMemberQueryService
{
    /// <summary>
    /// 查詢會員列表（進階查詢）
    /// </summary>
    Task<PagedResult<MemberQueryResultDto>> QueryMembersAsync(MemberQueryRequestDto request);

    /// <summary>
    /// 查詢會員交易記錄
    /// </summary>
    Task<PagedResult<MemberTransactionDto>> GetMemberTransactionsAsync(string memberId, MemberPointQueryDto query);

    /// <summary>
    /// 查詢會員回門禮卡號補登記錄
    /// </summary>
    Task<PagedResult<MemberExchangeLogDto>> GetExchangeLogsAsync(MemberExchangeLogQueryDto query);

    /// <summary>
    /// 匯出會員查詢結果
    /// </summary>
    Task<byte[]> ExportMembersAsync(MemberExportRequestDto request);

    /// <summary>
    /// 列印會員報表
    /// </summary>
    Task<byte[]> PrintMemberReportAsync(MemberPrintRequestDto request);
}

