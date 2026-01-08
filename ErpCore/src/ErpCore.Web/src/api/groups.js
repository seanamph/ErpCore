import axios from './axios'

/**
 * SYSWB70 - 組別資料維護作業 API
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const groupsApi = {
  // 查詢組別列表
  getGroups: (params) => {
    return axios.get('/groups', { params })
  },

  // 查詢單筆組別
  getGroup: (groupId) => {
    return axios.get(`/groups/${groupId}`)
  },

  // 新增組別
  createGroup: (data) => {
    return axios.post('/groups', data)
  },

  // 修改組別
  updateGroup: (groupId, data) => {
    return axios.put(`/groups/${groupId}`, data)
  },

  // 刪除組別
  deleteGroup: (groupId) => {
    return axios.delete(`/groups/${groupId}`)
  },

  // 批次刪除組別
  deleteGroupsBatch: (data) => {
    return axios.delete('/groups/batch', { data })
  },

  // 更新組別狀態
  updateGroupStatus: (groupId, data) => {
    return axios.put(`/groups/${groupId}/status`, data)
  }
}

