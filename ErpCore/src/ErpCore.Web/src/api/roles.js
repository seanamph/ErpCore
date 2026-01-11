import axios from './axios'

/**
 * SYS0210 - 角色基本資料維護作業 API
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const rolesApi = {
  // 查詢角色列表
  getRoles: (params) => {
    return axios.get('/roles', { params })
  },

  // 查詢單筆角色
  getRole: (roleId) => {
    return axios.get(`/roles/${roleId}`)
  },

  // 新增角色
  createRole: (data) => {
    return axios.post('/roles', data)
  },

  // 修改角色
  updateRole: (roleId, data) => {
    return axios.put(`/roles/${roleId}`, data)
  },

  // 刪除角色
  deleteRole: (roleId) => {
    return axios.delete(`/roles/${roleId}`)
  },

  // 批次刪除角色
  deleteRolesBatch: (data) => {
    return axios.delete('/roles/batch', { data })
  },

  // 複製角色
  copyRole: (roleId, data) => {
    return axios.post(`/roles/${roleId}/copy`, data)
  },

  // ========== SYS0230 - 角色之使用者設定維護 ==========

  // 查詢角色使用者列表
  getRoleUsers: (roleId, params) => {
    return axios.get(`/roles/${roleId}/users`, { params })
  },

  // 批量設定角色使用者
  batchAssignRoleUsers: (roleId, data) => {
    return axios.post(`/roles/${roleId}/users/assign`, data)
  },

  // 新增角色使用者
  assignUserToRole: (roleId, data) => {
    return axios.post(`/roles/${roleId}/users`, data)
  },

  // 移除角色使用者
  removeUserFromRole: (roleId, userId) => {
    return axios.delete(`/roles/${roleId}/users/${userId}`)
  },

  // ========== SYS0240 - 角色複製 ==========

  // 複製角色到目標角色
  copyRoleToTarget: (data) => {
    return axios.post('/roles/copy', data)
  },

  // 驗證角色複製
  validateCopyRole: (data) => {
    return axios.post('/roles/copy/validate', data)
  }
}

