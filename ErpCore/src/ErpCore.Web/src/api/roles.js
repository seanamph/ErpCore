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
  }
}

