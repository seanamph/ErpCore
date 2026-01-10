import axios from './axios'

/**
 * 報表擴展模組 API
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const reportExtensionApi = {
  // 報表模組O
  moduleO: {
    // 查詢報表列表
    getReports: (params) => {
      return axios.get('/reports/module-o', { params })
    },
    // 執行報表查詢
    queryReport: (reportCode, data) => {
      return axios.post(`/reports/module-o/${reportCode}/query`, data)
    }
  },

  // 報表模組N
  moduleN: {
    // 查詢報表列表
    getReports: (params) => {
      return axios.get('/reports/module-n', { params })
    },
    // 執行報表查詢
    queryReport: (reportCode, data) => {
      return axios.post(`/reports/module-n/${reportCode}/query`, data)
    }
  },

  // 報表模組WP
  moduleWP: {
    // 查詢報表列表
    getReports: (params) => {
      return axios.get('/reports/module-wp', { params })
    },
    // 執行報表查詢
    queryReport: (reportCode, data) => {
      return axios.post(`/reports/module-wp/${reportCode}/query`, data)
    }
  },

  // 報表模組7
  module7: {
    // 查詢報表列表
    getReports: (params) => {
      return axios.get('/reports/module-7', { params })
    },
    // 執行報表查詢
    queryReport: (reportCode, data) => {
      return axios.post(`/reports/module-7/${reportCode}/query`, data)
    }
  },

  // 報表列印作業
  print: {
    // 列印報表
    printReport: (data) => {
      return axios.post('/reports/print', data)
    },
    // 查詢報表列印記錄
    getPrintLogs: (params) => {
      return axios.get('/reports/print/logs', { params })
    }
  },

  // 報表統計作業
  statistics: {
    // 執行統計查詢
    queryStatistics: (data) => {
      return axios.post('/reports/statistics/query', data)
    },
    // 查詢統計記錄
    getStatisticsLogs: (params) => {
      return axios.get('/reports/statistics/logs', { params })
    }
  }
}

