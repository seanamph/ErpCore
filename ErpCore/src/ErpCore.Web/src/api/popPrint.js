import axios from './axios'

/**
 * POP卡商品卡列印作業 API (SYSW170)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const popPrintApi = {
  // 查詢商品列表（用於列印）
  getProducts: (params) => {
    return axios.get('/pop-print/products', { params })
  },

  // 產生列印資料
  generatePrintData: (data) => {
    return axios.post('/pop-print/generate', data)
  },

  // 執行列印
  print: (data) => {
    return axios.post('/pop-print/print', data)
  },

  // 取得列印設定
  getSettings: (shopId) => {
    return axios.get(`/pop-print/settings/${shopId || ''}`)
  },

  // 更新列印設定
  updateSettings: (shopId, data) => {
    return axios.put(`/pop-print/settings/${shopId || ''}`, data)
  },

  // 查詢列印記錄列表
  getLogs: (params) => {
    return axios.get('/pop-print/logs', { params })
  },

  // 匯出Excel
  exportExcel: (data) => {
    return axios.post('/pop-print/export-excel', data, {
      responseType: 'blob'
    })
  }
}

