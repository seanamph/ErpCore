import request from '@/utils/request'

/**
 * SYS0210 - 角色基本資料維護作業 API
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const rolesApi = {
  // 查詢角色列表
  getRoles: (params) => {
    return request({
      url: '/api/v1/roles',
      method: 'get',
      params
    })
  },

  // 查詢單筆角色
  getRole: (roleId) => {
    return request({
      url: `/api/v1/roles/${roleId}`,
      method: 'get'
    })
  },

  // 新增角色
  createRole: (data) => {
    return request({
      url: '/api/v1/roles',
      method: 'post',
      data
    })
  },

  // 修改角色
  updateRole: (roleId, data) => {
    return request({
      url: `/api/v1/roles/${roleId}`,
      method: 'put',
      data
    })
  },

  // 刪除角色
  deleteRole: (roleId) => {
    return request({
      url: `/api/v1/roles/${roleId}`,
      method: 'delete'
    })
  },

  // 批次刪除角色
  deleteRolesBatch: (data) => {
    return request({
      url: '/api/v1/roles/batch',
      method: 'delete',
      data
    })
  },

  // 複製角色
  copyRole: (roleId, data) => {
    return request({
      url: `/api/v1/roles/${roleId}/copy`,
      method: 'post',
      data
    })
  },

  // ========== SYS0230 - 角色之使用者設定維護 ==========

  // 查詢角色使用者列表
  getRoleUsers: (roleId, params) => {
    return request({
      url: `/api/v1/roles/${roleId}/users`,
      method: 'get',
      params
    })
  },

  // 批量設定角色使用者
  batchAssignRoleUsers: (roleId, data) => {
    return request({
      url: `/api/v1/roles/${roleId}/users/assign`,
      method: 'post',
      data
    })
  },

  // 新增角色使用者
  assignUserToRole: (roleId, data) => {
    return request({
      url: `/api/v1/roles/${roleId}/users`,
      method: 'post',
      data
    })
  },

  // 移除角色使用者
  removeUserFromRole: (roleId, userId) => {
    return request({
      url: `/api/v1/roles/${roleId}/users/${userId}`,
      method: 'delete'
    })
  },

  // ========== SYS0240 - 角色複製 ==========

  // 複製角色到目標角色
  copyRoleToTarget: (data) => {
    return request({
      url: '/api/v1/roles/copy',
      method: 'post',
      data
    })
  },

  // 驗證角色複製
  validateCopyRole: (data) => {
    return request({
      url: '/api/v1/roles/copy/validate',
      method: 'post',
      data
    })
  }
}

