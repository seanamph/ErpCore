import axios from './axios'

/**
 * SYSB450 - 區域基本資料維護作業 API
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const areasApi = {
  // 查詢區域列表
  getAreas: (params) => {
    return axios.get('/areas', { params })
  },

  // 查詢單筆區域
  getArea: (areaId) => {
    return axios.get(`/areas/${areaId}`)
  },

  // 新增區域
  createArea: (data) => {
    return axios.post('/areas', data)
  },

  // 修改區域
  updateArea: (areaId, data) => {
    return axios.put(`/areas/${areaId}`, data)
  },

  // 刪除區域
  deleteArea: (areaId) => {
    return axios.delete(`/areas/${areaId}`)
  },

  // 批次刪除區域
  deleteAreasBatch: (data) => {
    return axios.delete('/areas/batch', { data })
  },

  // 更新區域狀態
  updateAreaStatus: (areaId, data) => {
    return axios.put(`/areas/${areaId}/status`, data)
  }
}

