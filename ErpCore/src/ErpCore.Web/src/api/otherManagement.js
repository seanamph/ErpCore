import axios from './axios'

/**
 * 其他管理模組 API (SystemS, SystemU, SystemV, SystemJ)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const systemSApi = {
  // 查詢S系統功能列表
  getSystemSList: (params) => {
    return axios.get('/system-s-functions', { params })
  },

  // 查詢單筆S系統功能（根據主鍵）
  getSystemS: (tKey) => {
    return axios.get(`/system-s-functions/${tKey}`)
  },

  // 查詢單筆S系統功能（根據功能代碼）
  getSystemSByFunctionId: (functionId) => {
    return axios.get(`/system-s-functions/by-id/${functionId}`)
  },

  // 新增S系統功能
  createSystemS: (data) => {
    return axios.post('/system-s-functions', data)
  },

  // 修改S系統功能
  updateSystemS: (tKey, data) => {
    return axios.put(`/system-s-functions/${tKey}`, data)
  },

  // 刪除S系統功能
  deleteSystemS: (tKey) => {
    return axios.delete(`/system-s-functions/${tKey}`)
  },

  // 啟用/停用S系統功能
  updateStatus: (tKey, status) => {
    return axios.put(`/system-s-functions/${tKey}/status`, { Status: status })
  }
}

export const systemUApi = {
  // 查詢U系統功能列表
  getSystemUList: (params) => {
    return axios.get('/system-u-functions', { params })
  },

  // 查詢單筆U系統功能（根據主鍵）
  getSystemU: (tKey) => {
    return axios.get(`/system-u-functions/${tKey}`)
  },

  // 查詢單筆U系統功能（根據功能代碼）
  getSystemUByFunctionId: (functionId) => {
    return axios.get(`/system-u-functions/by-id/${functionId}`)
  },

  // 新增U系統功能
  createSystemU: (data) => {
    return axios.post('/system-u-functions', data)
  },

  // 修改U系統功能
  updateSystemU: (tKey, data) => {
    return axios.put(`/system-u-functions/${tKey}`, data)
  },

  // 刪除U系統功能
  deleteSystemU: (tKey) => {
    return axios.delete(`/system-u-functions/${tKey}`)
  },

  // 啟用/停用U系統功能
  updateStatus: (tKey, status) => {
    return axios.put(`/system-u-functions/${tKey}/status`, { Status: status })
  }
}

export const systemVApi = {
  // 查詢V系統功能列表
  getSystemVList: (params) => {
    return axios.get('/system-v-functions', { params })
  },

  // 查詢單筆V系統功能（根據主鍵）
  getSystemV: (tKey) => {
    return axios.get(`/system-v-functions/${tKey}`)
  },

  // 查詢單筆V系統功能（根據功能代碼）
  getSystemVByFunctionId: (functionId) => {
    return axios.get(`/system-v-functions/by-id/${functionId}`)
  },

  // 新增V系統功能
  createSystemV: (data) => {
    return axios.post('/system-v-functions', data)
  },

  // 修改V系統功能
  updateSystemV: (tKey, data) => {
    return axios.put(`/system-v-functions/${tKey}`, data)
  },

  // 刪除V系統功能
  deleteSystemV: (tKey) => {
    return axios.delete(`/system-v-functions/${tKey}`)
  },

  // 啟用/停用V系統功能
  updateStatus: (tKey, status) => {
    return axios.put(`/system-v-functions/${tKey}/status`, { Status: status })
  }
}

export const systemJApi = {
  // 查詢J系統功能列表
  getSystemJList: (params) => {
    return axios.get('/system-j-functions', { params })
  },

  // 查詢單筆J系統功能（根據主鍵）
  getSystemJ: (tKey) => {
    return axios.get(`/system-j-functions/${tKey}`)
  },

  // 查詢單筆J系統功能（根據功能代碼）
  getSystemJByFunctionId: (functionId) => {
    return axios.get(`/system-j-functions/by-id/${functionId}`)
  },

  // 新增J系統功能
  createSystemJ: (data) => {
    return axios.post('/system-j-functions', data)
  },

  // 修改J系統功能
  updateSystemJ: (tKey, data) => {
    return axios.put(`/system-j-functions/${tKey}`, data)
  },

  // 刪除J系統功能
  deleteSystemJ: (tKey) => {
    return axios.delete(`/system-j-functions/${tKey}`)
  },

  // 啟用/停用J系統功能
  updateStatus: (tKey, status) => {
    return axios.put(`/system-j-functions/${tKey}/status`, { Status: status })
  }
}

