import axios from './axios'

/**
 * POP卡商品卡列印作業 API - AP版本 (SYSW171)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const popPrintApApi = {
  // 查詢商品列表（用於列印）
  getProducts: (params) => {
    return axios.get('/pop-print-ap/products', { params })
  },

  // 產生列印資料
  generatePrintData: (data) => {
    // 自動設定版本為AP
    data.Version = 'AP'
    return axios.post('/pop-print-ap/generate', data)
  },

  // 執行列印
  print: (data) => {
    // 自動設定版本為AP
    data.Version = 'AP'
    return axios.post('/pop-print-ap/print', data)
  },

  // 取得列印設定
  getSettings: (shopId) => {
    return axios.get(`/pop-print-ap/settings/${shopId || ''}`)
  },

  // 更新列印設定
  updateSettings: (shopId, data) => {
    // 自動設定版本為AP
    data.Version = 'AP'
    return axios.put(`/pop-print-ap/settings/${shopId || ''}`, data)
  },

  // 查詢列印記錄列表
  getLogs: (params) => {
    return axios.get('/pop-print-ap/logs', { params })
  },

  // 匯出Excel
  exportExcel: (data) => {
    // 自動設定版本為AP
    data.Version = 'AP'
    return axios.post('/pop-print-ap/export-excel', data, {
      responseType: 'blob'
    })
  }
}

