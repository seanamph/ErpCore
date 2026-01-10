import axios from './axios'

/**
 * 其他模組 API
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const otherModuleApi = {
  // CRP報表
  crpReport: {
    // 取得報表設定列表
    getReports: () => {
      return axios.get('/other-module/crp/reports')
    },
    // 根據報表代碼取得報表設定
    getReportByCode: (reportCode) => {
      return axios.get(`/other-module/crp/reports/${reportCode}`)
    },
    // 生成報表
    generateReport: (data) => {
      return axios.post('/other-module/crp/reports/generate', data)
    },
    // 下載報表
    downloadReport: (reportId) => {
      return axios.get(`/other-module/crp/reports/${reportId}/download`, { responseType: 'blob' })
    },
    // 新增報表設定
    createReport: (data) => {
      return axios.post('/other-module/crp/reports', data)
    },
    // 修改報表設定
    updateReport: (reportId, data) => {
      return axios.put(`/other-module/crp/reports/${reportId}`, data)
    },
    // 刪除報表設定
    deleteReport: (reportId) => {
      return axios.delete(`/other-module/crp/reports/${reportId}`)
    },
    // 查詢操作記錄列表
    getLogs: (params) => {
      return axios.get('/other-module/crp/logs', { params })
    }
  },

  // EIP系統整合
  eipIntegration: {
    // 執行 EIP 整合
    executeIntegration: (data) => {
      return axios.post('/other-module/eip/integration', data)
    },
    // 查詢整合記錄
    getIntegrationLogs: (params) => {
      return axios.get('/other-module/eip/logs', { params })
    }
  },

  // 實驗室測試功能
  labTest: {
    // 執行測試
    executeTest: (data) => {
      return axios.post('/other-module/lab/test', data)
    },
    // 查詢測試記錄
    getTestLogs: (params) => {
      return axios.get('/other-module/lab/logs', { params })
    }
  }
}

