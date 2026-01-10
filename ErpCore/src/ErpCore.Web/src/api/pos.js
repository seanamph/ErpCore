import axios from './axios'

/**
 * POS系統 API
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const posApi = {
  // ========== 交易查詢 ==========
  
  // 查詢POS交易列表
  getTransactions: (params) => {
    return axios.get('/pos/transactions', { params })
  },

  // 查詢單筆POS交易
  getTransaction: (transactionId) => {
    return axios.get(`/pos/transactions/${transactionId}`)
  },

  // 查詢POS交易明細
  getTransactionDetails: (transactionId) => {
    return axios.get(`/pos/transactions/${transactionId}/details`)
  },

  // ========== 報表查詢 ==========
  
  // 查詢POS交易報表
  getTransactionReport: (params) => {
    return axios.get('/pos/transactions/report', { params })
  },

  // 查詢POS銷售統計報表
  getSalesStatisticsReport: (params) => {
    return axios.get('/pos/reports/sales-statistics', { params })
  },

  // 查詢POS商品銷售排行
  getProductSalesRanking: (params) => {
    return axios.get('/pos/reports/product-sales-ranking', { params })
  },

  // 匯出報表
  exportReport: (params, format = 'excel') => {
    return axios.get(`/pos/reports/export/${format}`, { 
      params,
      responseType: 'blob'
    })
  },

  // ========== 資料同步 ==========
  
  // 同步POS交易資料
  syncTransactions: (data) => {
    return axios.post('/pos/sync/transactions', data)
  },

  // 查詢同步記錄列表
  getSyncLogs: (params) => {
    return axios.get('/pos/sync/logs', { params })
  },

  // 查詢單筆同步記錄
  getSyncLog: (logId) => {
    return axios.get(`/pos/sync/logs/${logId}`)
  },

  // 查詢同步狀態
  getSyncStatus: () => {
    return axios.get('/pos/sync/status')
  }
}

