import axios from './axios'

/**
 * 擴展功能維護 API (SYS9000)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const extensionApi = {
  // 查詢擴展功能列表
  getExtensionFunctions: (params) => {
    return axios.get('/extension-functions', { params })
  },

  // 查詢單筆擴展功能
  getExtensionFunction: (tKey) => {
    return axios.get(`/extension-functions/${tKey}`)
  },

  // 根據擴展代碼查詢
  getExtensionFunctionByExtensionId: (extensionId) => {
    return axios.get(`/extension-functions/by-id/${extensionId}`)
  },

  // 新增擴展功能
  createExtensionFunction: (data) => {
    return axios.post('/extension-functions', data)
  },

  // 修改擴展功能
  updateExtensionFunction: (tKey, data) => {
    return axios.put(`/extension-functions/${tKey}`, data)
  },

  // 刪除擴展功能
  deleteExtensionFunction: (tKey) => {
    return axios.delete(`/extension-functions/${tKey}`)
  },

  // 批次刪除擴展功能
  batchDeleteExtensionFunctions: (tKeys) => {
    return axios.post('/extension-functions/batch-delete', { TKeys: tKeys })
  }
}

