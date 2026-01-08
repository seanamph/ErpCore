import axios from './axios'

/**
 * SYSBC30 - 地區設定 API
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const regionsApi = {
  // 查詢地區列表
  getRegions: (params) => {
    return axios.get('/regions', { params })
  },

  // 查詢單筆地區
  getRegion: (regionId) => {
    return axios.get(`/regions/${regionId}`)
  },

  // 新增地區
  createRegion: (data) => {
    return axios.post('/regions', data)
  },

  // 修改地區
  updateRegion: (regionId, data) => {
    return axios.put(`/regions/${regionId}`, data)
  },

  // 刪除地區
  deleteRegion: (regionId) => {
    return axios.delete(`/regions/${regionId}`)
  },

  // 批次刪除地區
  deleteRegionsBatch: (data) => {
    return axios.delete('/regions/batch', { data })
  }
}

