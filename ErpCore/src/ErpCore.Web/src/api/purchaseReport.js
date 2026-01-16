import axios from './axios'

/**
 * 採購報表查詢 API (SYSP410-SYSP4I0)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const purchaseReportApi = {
  // 查詢採購報表列表
  queryPurchaseReports: (params) => {
    return axios.get('/purchase-reports/query', { params })
  },

  // 查詢採購報表明細列表
  queryPurchaseReportDetails: (params) => {
    return axios.get('/purchase-reports/details', { params })
  },

  // 匯出採購報表
  exportPurchaseReport: (data) => {
    return axios.post('/purchase-reports/export', data, {
      responseType: 'blob'
    })
  }
}
