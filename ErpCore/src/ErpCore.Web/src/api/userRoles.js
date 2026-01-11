import axios from './axios'

/**
 * SYS0220 - 使用者之角色設定維護作業 API
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const userRolesApi = {
  // 查詢使用者已分配的角色列表
  getUserRoles: (userId, params) => {
    return axios.get(`/users/${userId}/roles`, { params })
  },

  // 查詢可用角色列表（排除已分配的角色）
  getAvailableRoles: (userId, params) => {
    return axios.get(`/users/${userId}/roles/available`, { params })
  },

  // 為使用者分配角色（新增）
  assignRoles: (userId, data) => {
    return axios.post(`/users/${userId}/roles`, data)
  },

  // 移除使用者的角色（刪除）
  removeRoles: (userId, data) => {
    return axios.delete(`/users/${userId}/roles`, { data })
  },

  // 批量更新使用者角色
  batchUpdateRoles: (userId, data) => {
    return axios.put(`/users/${userId}/roles/batch`, data)
  }
}
