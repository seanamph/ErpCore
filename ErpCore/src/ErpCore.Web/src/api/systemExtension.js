import axios from './axios'

/**
 * 系統擴展資料維護作業 API (SYSX110, SYSX120, SYSX140)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const systemExtensionApi = {
  // ========== SYSX110 - 系統擴展資料維護 ==========
  
  // 查詢系統擴展列表
  getSystemExtensions: (params) => {
    return axios.get('/system-extensions', { params })
  },

  // 查詢單筆系統擴展（根據主鍵）
  getSystemExtension: (tKey) => {
    return axios.get(`/system-extensions/${tKey}`)
  },

  // 查詢單筆系統擴展（根據擴展功能代碼）
  getSystemExtensionByExtensionId: (extensionId) => {
    return axios.get(`/system-extensions/by-id/${extensionId}`)
  },

  // 新增系統擴展
  createSystemExtension: (data) => {
    return axios.post('/system-extensions', data)
  },

  // 修改系統擴展
  updateSystemExtension: (tKey, data) => {
    return axios.put(`/system-extensions/${tKey}`, data)
  },

  // 刪除系統擴展
  deleteSystemExtension: (tKey) => {
    return axios.delete(`/system-extensions/${tKey}`)
  },

  // ========== SYSX120 - 系統擴展查詢 ==========

  // 查詢系統擴展統計資訊
  getStatistics: (params) => {
    return axios.get('/system-extensions/statistics', { params })
  },

  // ========== SYSX140 - 系統擴展報表 ==========

  // 查詢報表資料
  queryReport: (data) => {
    return axios.post('/system-extensions/reports/query', data)
  },

  // 產生 PDF 報表
  generatePDF: (data) => {
    return axios.post('/system-extensions/reports/pdf', data, {
      responseType: 'blob'
    })
  },

  // 產生 Excel 報表
  generateExcel: (data) => {
    return axios.post('/system-extensions/reports/excel', data, {
      responseType: 'blob'
    })
  },

  // 查詢報表記錄列表
  getReports: (params) => {
    return axios.get('/system-extensions/reports', { params })
  },

  // 下載報表檔案
  downloadReport: (reportId) => {
    return axios.get(`/system-extensions/reports/${reportId}/download`, {
      responseType: 'blob'
    })
  },

  // 刪除報表記錄
  deleteReport: (reportId) => {
    return axios.delete(`/system-extensions/reports/${reportId}`)
  }
}

