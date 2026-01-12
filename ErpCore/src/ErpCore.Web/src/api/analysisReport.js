import axios from './axios'

/**
 * 進銷存分析報表 API (SYSA1000, SYSA1011-SYSA1024)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const analysisReportApi = {
  // 查詢進銷存分析報表
  getAnalysisReport: (reportId, params) => {
    return axios.get(`/analysis-reports/${reportId}`, { params })
  },

  // 匯出進銷存分析報表
  exportAnalysisReport: (reportId, data) => {
    return axios.post(`/analysis-reports/${reportId}/export`, data, {
      responseType: 'blob'
    })
  },

  // 列印進銷存分析報表
  printAnalysisReport: (reportId, data) => {
    return axios.post(`/analysis-reports/${reportId}/print`, data, {
      responseType: 'blob'
    })
  },

  // 查詢耗材列表（用於列印）
  getConsumablesForPrint: (data) => {
    return axios.post('/analysis-reports/consumables/print/list', data)
  },

  // 查詢商品分析報表 (SYSA1011)
  getSYSA1011Report: (params) => {
    return axios.get('/analysis-reports/sysa1011', { params })
  },

  // 匯出商品分析報表 (SYSA1011)
  exportSYSA1011Report: (data, format = 'excel') => {
    return axios.post(`/analysis-reports/sysa1011/export?format=${format}`, data, {
      responseType: 'blob'
    })
  },

  // 列印商品分析報表 (SYSA1011)
  printSYSA1011Report: (data) => {
    return axios.post('/analysis-reports/sysa1011/print', data, {
      responseType: 'blob'
    })
  },

  // 查詢商品分類列表 (SYSA1011)
  getGoodsCategories: (categoryType, parentId) => {
    return axios.get('/analysis-reports/sysa1011/categories', {
      params: { categoryType, parentId }
    })
  },

  // 查詢進銷存月報表 (SYSA1012)
  getSYSA1012Report: (params) => {
    return axios.get('/analysis-reports/sysa1012', { params })
  },

  // 匯出進銷存月報表 (SYSA1012)
  exportSYSA1012Report: (data, format = 'excel') => {
    return axios.post(`/analysis-reports/sysa1012/export?format=${format}`, data, {
      responseType: 'blob'
    })
  },

  // 列印進銷存月報表 (SYSA1012)
  printSYSA1012Report: (data) => {
    return axios.post('/analysis-reports/sysa1012/print', data, {
      responseType: 'blob'
    })
  }
}

