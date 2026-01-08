import axios from './axios'

/**
 * SYSB206 - 廠/客基本資料維護作業 API
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const vendorsApi = {
  // 查詢廠商列表
  getVendors: (params) => {
    return axios.get('/vendors', { params })
  },

  // 查詢單筆廠商
  getVendor: (vendorId) => {
    return axios.get(`/vendors/${vendorId}`)
  },

  // 檢查統一編號是否存在
  checkGuiId: (guiId) => {
    return axios.get(`/vendors/check-gui-id/${guiId}`)
  },

  // 新增廠商
  createVendor: (data) => {
    return axios.post('/vendors', data)
  },

  // 修改廠商
  updateVendor: (vendorId, data) => {
    return axios.put(`/vendors/${vendorId}`, data)
  },

  // 刪除廠商
  deleteVendor: (vendorId) => {
    return axios.delete(`/vendors/${vendorId}`)
  },

  // 批次刪除廠商
  deleteVendorsBatch: (data) => {
    return axios.delete('/vendors/batch', { data })
  }
}

