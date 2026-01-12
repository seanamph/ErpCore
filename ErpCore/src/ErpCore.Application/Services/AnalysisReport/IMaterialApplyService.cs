using ErpCore.Application.DTOs.AnalysisReport;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.AnalysisReport;

/// <summary>
/// 單位領用申請單服務介面 (SYSA210)
/// </summary>
public interface IMaterialApplyService
{
    /// <summary>
    /// 查詢領用申請列表
    /// </summary>
    Task<PagedResult<MaterialApplyDto>> GetListAsync(MaterialApplyQueryDto query);

    /// <summary>
    /// 根據領用單號查詢領用申請（含明細）
    /// </summary>
    Task<MaterialApplyDetailDto> GetByApplyIdAsync(string applyId);

    /// <summary>
    /// 新增領用申請
    /// </summary>
    Task<MaterialApplyDetailDto> CreateAsync(CreateMaterialApplyDto dto, string userId);

    /// <summary>
    /// 更新領用申請
    /// </summary>
    Task<MaterialApplyDetailDto> UpdateAsync(string applyId, UpdateMaterialApplyDto dto, string userId);

    /// <summary>
    /// 刪除領用申請
    /// </summary>
    Task DeleteAsync(string applyId, string userId);

    /// <summary>
    /// 審核領用申請
    /// </summary>
    Task<MaterialApplyDetailDto> ApproveAsync(string applyId, ApproveMaterialApplyDto dto, string userId);

    /// <summary>
    /// 發料作業
    /// </summary>
    Task<MaterialApplyDetailDto> IssueAsync(string applyId, IssueMaterialApplyDto dto, string userId);

    /// <summary>
    /// 批次新增領用申請
    /// </summary>
    Task<List<MaterialApplyDetailDto>> BatchCreateAsync(BatchCreateMaterialApplyDto dto, string userId);

    /// <summary>
    /// 產生領用單號
    /// </summary>
    Task<string> GenerateApplyIdAsync();
}
