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
  },

  // 查詢耗材出庫明細表 (SYSA1013)
  getSYSA1013Report: (params) => {
    return axios.get('/analysis-reports/sysa1013', { params })
  },

  // 匯出耗材出庫明細表 (SYSA1013)
  exportSYSA1013Report: (data, format = 'excel') => {
    return axios.post(`/analysis-reports/sysa1013/export?format=${format}`, data, {
      responseType: 'blob'
    })
  },

  // 列印耗材出庫明細表 (SYSA1013)
  printSYSA1013Report: (data) => {
    return axios.post('/analysis-reports/sysa1013/print', data, {
      responseType: 'blob'
    })
  },

  // 查詢商品分析報表 (SYSA1014)
  getSYSA1014Report: (params) => {
    return axios.get('/analysis-reports/sysa1014', { params })
  },

  // 匯出商品分析報表 (SYSA1014)
  exportSYSA1014Report: (data, format = 'excel') => {
    return axios.post(`/analysis-reports/sysa1014/export?format=${format}`, data, {
      responseType: 'blob'
    })
  },

  // 列印商品分析報表 (SYSA1014)
  printSYSA1014Report: (data) => {
    return axios.post('/analysis-reports/sysa1014/print', data, {
      responseType: 'blob'
    })
  },

  // 查詢商品分析報表 (SYSA1015)
  getSYSA1015Report: (params) => {
    return axios.get('/analysis-reports/sysa1015', { params })
  },

  // 匯出商品分析報表 (SYSA1015)
  exportSYSA1015Report: (data, format = 'excel') => {
    return axios.post(`/analysis-reports/sysa1015/export?format=${format}`, data, {
      responseType: 'blob'
    })
  },

  // 列印商品分析報表 (SYSA1015)
  printSYSA1015Report: (data) => {
    return axios.post('/analysis-reports/sysa1015/print', data, {
      responseType: 'blob'
    })
  },

  // 查詢商品分析報表 (SYSA1016)
  getSYSA1016Report: (params) => {
    return axios.get('/analysis-reports/sysa1016', { params })
  },

  // 匯出商品分析報表 (SYSA1016)
  exportSYSA1016Report: (data, format = 'excel') => {
    return axios.post(`/analysis-reports/sysa1016/export?format=${format}`, data, {
      responseType: 'blob'
    })
  },

  // 列印商品分析報表 (SYSA1016)
  printSYSA1016Report: (data) => {
    return axios.post('/analysis-reports/sysa1016/print', data, {
      responseType: 'blob'
    })
  },

  // 查詢商品分析報表 (SYSA1017)
  getSYSA1017Report: (params) => {
    return axios.get('/analysis-reports/sysa1017', { params })
  },

  // 匯出商品分析報表 (SYSA1017)
  exportSYSA1017Report: (data, format = 'excel') => {
    return axios.post(`/analysis-reports/sysa1017/export?format=${format}`, data, {
      responseType: 'blob'
    })
  },

  // 列印商品分析報表 (SYSA1017)
  printSYSA1017Report: (data) => {
    return axios.post('/analysis-reports/sysa1017/print', data, {
      responseType: 'blob'
    })
  },

  // 查詢工務維修件數統計報表 (SYSA1018)
  getSYSA1018Report: (params) => {
    return axios.get('/analysis-reports/sysa1018', { params })
  },

  // 匯出工務維修件數統計報表 (SYSA1018)
  exportSYSA1018Report: (data, format = 'excel') => {
    return axios.post(`/analysis-reports/sysa1018/export?format=${format}`, data, {
      responseType: 'blob'
    })
  },

  // 列印工務維修件數統計報表 (SYSA1018)
  printSYSA1018Report: (data) => {
    return axios.post('/analysis-reports/sysa1018/print', data, {
      responseType: 'blob'
    })
  },

  // 查詢商品分析報表 (SYSA1019)
  getSYSA1019Report: (params) => {
    return axios.get('/analysis-reports/sysa1019', { params })
  },

  // 匯出商品分析報表 (SYSA1019)
  exportSYSA1019Report: (data, format = 'excel') => {
    return axios.post(`/analysis-reports/sysa1019/export?format=${format}`, data, {
      responseType: 'blob'
    })
  },

  // 列印商品分析報表 (SYSA1019)
  printSYSA1019Report: (data) => {
    return axios.post('/analysis-reports/sysa1019/print', data, {
      responseType: 'blob'
    })
  },

  // 查詢商品分析報表 (SYSA1020)
  getSYSA1020Report: (params) => {
    return axios.get('/analysis-reports/sysa1020', { params })
  },

  // 匯出商品分析報表 (SYSA1020)
  exportSYSA1020Report: (data, format = 'excel') => {
    return axios.post(`/analysis-reports/sysa1020/export?format=${format}`, data, {
      responseType: 'blob'
    })
  },

  // 列印商品分析報表 (SYSA1020)
  printSYSA1020Report: (data) => {
    return axios.post('/analysis-reports/sysa1020/print', data, {
      responseType: 'blob'
    })
  },

  // 查詢月成本報表 (SYSA1021)
  getSYSA1021Report: (params) => {
    return axios.get('/analysis-reports/sysa1021', { params })
  },

  // 匯出月成本報表 (SYSA1021)
  exportSYSA1021Report: (data, format = 'excel') => {
    return axios.post(`/analysis-reports/sysa1021/export?format=${format}`, data, {
      responseType: 'blob'
    })
  },

  // 列印月成本報表 (SYSA1021)
  printSYSA1021Report: (data) => {
    return axios.post('/analysis-reports/sysa1021/print', data, {
      responseType: 'blob'
    })
  },

  // 查詢工務維修統計報表 (SYSA1022)
  getSYSA1022Report: (params) => {
    return axios.get('/analysis-reports/sysa1022', { params })
  },

  // 匯出工務維修統計報表 (SYSA1022)
  exportSYSA1022Report: (data, format = 'excel') => {
    return axios.post(`/analysis-reports/sysa1022/export?format=${format}`, data, {
      responseType: 'blob'
    })
  },

  // 列印工務維修統計報表 (SYSA1022)
  printSYSA1022Report: (data) => {
    return axios.post('/analysis-reports/sysa1022/print', data, {
      responseType: 'blob'
    })
  },

  // 查詢工務維修統計報表(報表類型) (SYSA1023)
  getSYSA1023Report: (params) => {
    return axios.get('/analysis-reports/sysa1023', { params })
  },

  // 匯出工務維修統計報表(報表類型) (SYSA1023)
  exportSYSA1023Report: (data, format = 'excel') => {
    return axios.post(`/analysis-reports/sysa1023/export?format=${format}`, data, {
      responseType: 'blob'
    })
  },

  // 列印工務維修統計報表(報表類型) (SYSA1023)
  printSYSA1023Report: (data) => {
    return axios.post('/analysis-reports/sysa1023/print', data, {
      responseType: 'blob'
    })
  }
}

