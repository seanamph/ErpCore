import axios from './axios'

/**
 * 能源管理 API
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const energyApi = {
  // ========== 能源基礎維護 (EnergyBase) - SYSO100-SYSO130 ==========
  
  // 查詢能源基礎列表
  getEnergyBases: (params) => {
    return axios.get('/api/v1/energy-base', { params })
  },

  // 查詢單筆能源基礎
  getEnergyBase: (tKey) => {
    return axios.get(`/api/v1/energy-base/${tKey}`)
  },

  // 新增能源基礎
  createEnergyBase: (data) => {
    return axios.post('/api/v1/energy-base', data)
  },

  // 修改能源基礎
  updateEnergyBase: (tKey, data) => {
    return axios.put(`/api/v1/energy-base/${tKey}`, data)
  },

  // 刪除能源基礎
  deleteEnergyBase: (tKey) => {
    return axios.delete(`/api/v1/energy-base/${tKey}`)
  },

  // ========== 能源處理作業 (EnergyProcess) - SYSO310 ==========
  
  // 查詢能源處理列表
  getEnergyProcesses: (params) => {
    return axios.get('/api/v1/energy-process', { params })
  },

  // 查詢單筆能源處理
  getEnergyProcess: (tKey) => {
    return axios.get(`/api/v1/energy-process/${tKey}`)
  },

  // 新增能源處理
  createEnergyProcess: (data) => {
    return axios.post('/api/v1/energy-process', data)
  },

  // 修改能源處理
  updateEnergyProcess: (tKey, data) => {
    return axios.put(`/api/v1/energy-process/${tKey}`, data)
  },

  // 刪除能源處理
  deleteEnergyProcess: (tKey) => {
    return axios.delete(`/api/v1/energy-process/${tKey}`)
  },

  // ========== 能源擴展維護 (EnergyExtension) - SYSOU10-SYSOU33 ==========
  
  // 查詢能源擴展列表
  getEnergyExtensions: (params) => {
    return axios.get('/api/v1/energy-extension', { params })
  },

  // 查詢單筆能源擴展
  getEnergyExtension: (tKey) => {
    return axios.get(`/api/v1/energy-extension/${tKey}`)
  },

  // 新增能源擴展
  createEnergyExtension: (data) => {
    return axios.post('/api/v1/energy-extension', data)
  },

  // 修改能源擴展
  updateEnergyExtension: (tKey, data) => {
    return axios.put(`/api/v1/energy-extension/${tKey}`, data)
  },

  // 刪除能源擴展
  deleteEnergyExtension: (tKey) => {
    return axios.delete(`/api/v1/energy-extension/${tKey}`)
  }
}

