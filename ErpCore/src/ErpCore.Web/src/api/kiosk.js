import axios from './axios'

/**
 * Kiosk自助服務終端 API
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const kioskApi = {
  // ========== 報表查詢 ==========
  
  // 查詢Kiosk交易記錄
  getTransactions: (params) => {
    return axios.get('/kiosk/reports/transactions', { params })
  },

  // 查詢Kiosk交易統計
  getStatistics: (params) => {
    return axios.get('/kiosk/reports/statistics', { params })
  },

  // 查詢Kiosk功能代碼統計
  getFunctionStatistics: (params) => {
    return axios.get('/kiosk/reports/function-statistics', { params })
  },

  // 查詢Kiosk錯誤分析
  getErrorAnalysis: (params) => {
    return axios.get('/kiosk/reports/error-analysis', { params })
  },

  // 匯出Kiosk交易報表
  exportReport: (data) => {
    return axios.post('/kiosk/reports/export', data, {
      responseType: 'blob'
    })
  },

  // ========== 資料處理 ==========
  
  // 處理Kiosk請求
  processRequest: (data) => {
    return axios.post('/kiosk/process', data)
  }
}

