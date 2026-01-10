import axios from './axios'

/**
 * 主系統項目資料維護 API (CFG0410)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const configSystemsApi = {
  // 查詢主系統列表
  getConfigSystems: (params) => {
    return axios.get('/api/v1/config/systems', { params })
  },

  // 查詢單筆主系統
  getConfigSystem: (systemId) => {
    return axios.get(`/api/v1/config/systems/${systemId}`)
  },

  // 新增主系統
  createConfigSystem: (data) => {
    return axios.post('/api/v1/config/systems', data)
  },

  // 修改主系統
  updateConfigSystem: (systemId, data) => {
    return axios.put(`/api/v1/config/systems/${systemId}`, data)
  },

  // 刪除主系統
  deleteConfigSystem: (systemId) => {
    return axios.delete(`/api/v1/config/systems/${systemId}`)
  },

  // 批次刪除主系統
  batchDeleteConfigSystems: (data) => {
    return axios.delete('/api/v1/config/systems/batch', { data })
  }
}

/**
 * 子系統項目資料維護 API (CFG0420)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const configSubSystemsApi = {
  // 查詢子系統列表
  getConfigSubSystems: (params) => {
    return axios.get('/api/v1/config/subsystems', { params })
  },

  // 查詢單筆子系統
  getConfigSubSystem: (subSystemId) => {
    return axios.get(`/api/v1/config/subsystems/${subSystemId}`)
  },

  // 新增子系統
  createConfigSubSystem: (data) => {
    return axios.post('/api/v1/config/subsystems', data)
  },

  // 修改子系統
  updateConfigSubSystem: (subSystemId, data) => {
    return axios.put(`/api/v1/config/subsystems/${subSystemId}`, data)
  },

  // 刪除子系統
  deleteConfigSubSystem: (subSystemId) => {
    return axios.delete(`/api/v1/config/subsystems/${subSystemId}`)
  },

  // 批次刪除子系統
  batchDeleteConfigSubSystems: (data) => {
    return axios.delete('/api/v1/config/subsystems/batch', { data })
  }
}

/**
 * 系統作業資料維護 API (CFG0430)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const configProgramsApi = {
  // 查詢系統作業列表
  getConfigPrograms: (params) => {
    return axios.get('/api/v1/config/programs', { params })
  },

  // 查詢單筆系統作業
  getConfigProgram: (programId) => {
    return axios.get(`/api/v1/config/programs/${programId}`)
  },

  // 新增系統作業
  createConfigProgram: (data) => {
    return axios.post('/api/v1/config/programs', data)
  },

  // 修改系統作業
  updateConfigProgram: (programId, data) => {
    return axios.put(`/api/v1/config/programs/${programId}`, data)
  },

  // 刪除系統作業
  deleteConfigProgram: (programId) => {
    return axios.delete(`/api/v1/config/programs/${programId}`)
  },

  // 批次刪除系統作業
  batchDeleteConfigPrograms: (data) => {
    return axios.delete('/api/v1/config/programs/batch', { data })
  }
}

/**
 * 系統功能按鈕資料維護 API (CFG0440)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const configButtonsApi = {
  // 查詢系統功能按鈕列表
  getConfigButtons: (params) => {
    return axios.get('/api/v1/config/buttons', { params })
  },

  // 查詢單筆系統功能按鈕
  getConfigButton: (buttonId) => {
    return axios.get(`/api/v1/config/buttons/${buttonId}`)
  },

  // 新增系統功能按鈕
  createConfigButton: (data) => {
    return axios.post('/api/v1/config/buttons', data)
  },

  // 修改系統功能按鈕
  updateConfigButton: (buttonId, data) => {
    return axios.put(`/api/v1/config/buttons/${buttonId}`, data)
  },

  // 刪除系統功能按鈕
  deleteConfigButton: (buttonId) => {
    return axios.delete(`/api/v1/config/buttons/${buttonId}`)
  },

  // 批次刪除系統功能按鈕
  batchDeleteConfigButtons: (data) => {
    return axios.delete('/api/v1/config/buttons/batch', { data })
  }
}

