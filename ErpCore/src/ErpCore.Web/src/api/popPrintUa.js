import axios from './axios'

/**
 * POP卡商品卡列印作業 API - UA版本 (SYSW172)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const popPrintUaApi = {
  // 查詢商品列表（用於列印）
  getProducts: (params) => {
    return axios.get('/pop-print-ua/products', { params })
  },

  // 產生列印資料
  generatePrintData: (data) => {
    // 自動設定版本為UA
    data.Version = 'UA'
    return axios.post('/pop-print-ua/generate', data)
  },

  // 執行列印
  print: (data) => {
    // 自動設定版本為UA
    data.Version = 'UA'
    return axios.post('/pop-print-ua/print', data)
  },

  // 取得列印設定
  getSettings: (shopId) => {
    return axios.get(`/pop-print-ua/settings/${shopId || ''}`)
  },

  // 更新列印設定
  updateSettings: (shopId, data) => {
    // 自動設定版本為UA
    data.Version = 'UA'
    return axios.put(`/pop-print-ua/settings/${shopId || ''}`, data)
  },

  // 查詢列印記錄列表
  getLogs: (params) => {
    return axios.get('/pop-print-ua/logs', { params })
  },

  // 匯出Excel
  exportExcel: (data) => {
    // 自動設定版本為UA
    data.Version = 'UA'
    return axios.post('/pop-print-ua/export-excel', data, {
      responseType: 'blob'
    })
  }
}
