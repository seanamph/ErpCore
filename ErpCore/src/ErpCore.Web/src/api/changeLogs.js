import axios from './axios'

/**
 * 異動記錄查詢 API (SYS0610-SYS0660)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const changeLogsApi = {
  // 查詢使用者異動記錄 (SYS0610)
  getUserChangeLogs: (data) => {
    return axios.post('/api/v1/change-logs/users/search', data)
  },

  // 查詢角色異動記錄 (SYS0620)
  getRoleChangeLogs: (data) => {
    return axios.post('/api/v1/change-logs/roles/search', data)
  },

  // 查詢使用者角色對應設定異動記錄 (SYS0630)
  getUserRoleChangeLogs: (data) => {
    return axios.post('/api/v1/change-logs/user-roles/search', data)
  },

  // 查詢系統權限異動記錄 (SYS0640)
  getSystemPermissionChangeLogs: (data) => {
    return axios.post('/api/v1/change-logs/system-permissions/search', data)
  },

  // 查詢可管控欄位異動記錄 (SYS0650)
  getControllableFieldChangeLogs: (data) => {
    return axios.post('/api/v1/change-logs/controllable-fields/search', data)
  },

  // 查詢其他異動記錄 (SYS0660)
  getOtherChangeLogs: (data) => {
    return axios.post('/api/v1/change-logs/others/search', data)
  },

  // 查詢單筆異動記錄
  getChangeLogById: (logId) => {
    return axios.get(`/api/v1/change-logs/${logId}`)
  }
}

