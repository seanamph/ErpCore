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
  }
}

