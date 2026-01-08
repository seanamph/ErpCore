using ErpCore.Application.DTOs.BasicData;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.BasicData;

/// <summary>
/// 組別服務介面
/// </summary>
public interface IGroupService
{
    /// <summary>
    /// 查詢組別列表
    /// </summary>
    Task<PagedResult<GroupDto>> GetGroupsAsync(GroupQueryDto query);

    /// <summary>
    /// 查詢單筆組別
    /// </summary>
    Task<GroupDto> GetGroupByIdAsync(string groupId);

    /// <summary>
    /// 新增組別
    /// </summary>
    Task<string> CreateGroupAsync(CreateGroupDto dto);

    /// <summary>
    /// 修改組別
    /// </summary>
    Task UpdateGroupAsync(string groupId, UpdateGroupDto dto);

    /// <summary>
    /// 刪除組別
    /// </summary>
    Task DeleteGroupAsync(string groupId);

    /// <summary>
    /// 批次刪除組別
    /// </summary>
    Task DeleteGroupsBatchAsync(BatchDeleteGroupDto dto);

    /// <summary>
    /// 更新組別狀態
    /// </summary>
    Task UpdateStatusAsync(string groupId, UpdateGroupStatusDto dto);
}

