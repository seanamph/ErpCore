import axios from './axios'

/**
 * 憑證管理 API (SYSK000)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const certificateApi = {
  // ========== 憑證資料維護 (CertificateData) - SYSK110-SYSK150 ==========
  
  // 查詢憑證列表
  getVouchers: (params) => {
    return axios.get('/api/v1/vouchers', { params })
  },

  // 查詢單筆憑證
  getVoucher: (voucherId) => {
    return axios.get(`/api/v1/vouchers/${voucherId}`)
  },

  // 新增憑證
  createVoucher: (data) => {
    return axios.post('/api/v1/vouchers', data)
  },

  // 修改憑證
  updateVoucher: (voucherId, data) => {
    return axios.put(`/api/v1/vouchers/${voucherId}`, data)
  },

  // 刪除憑證
  deleteVoucher: (voucherId) => {
    return axios.delete(`/api/v1/vouchers/${voucherId}`)
  },

  // 審核憑證
  approveVoucher: (voucherId, data) => {
    return axios.put(`/api/v1/vouchers/${voucherId}/approve`, data)
  },

  // 取消憑證
  cancelVoucher: (voucherId, data) => {
    return axios.put(`/api/v1/vouchers/${voucherId}/cancel`, data)
  },

  // ========== 憑證處理作業 (CertificateProcess) - SYSK210-SYSK230 ==========
  
  // 憑證檢查
  checkVouchers: (voucherIds) => {
    return axios.post('/api/v1/vouchers/process/check', voucherIds)
  },

  // 憑證導入
  importVouchers: (file) => {
    const formData = new FormData()
    formData.append('file', file)
    return axios.post('/api/v1/vouchers/process/import', formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    })
  },

  // 憑證列印
  printVouchers: (data) => {
    return axios.post('/api/v1/vouchers/process/print', data)
  },

  // 憑證匯出
  exportVouchers: (data) => {
    return axios.post('/api/v1/vouchers/process/export', data, { responseType: 'blob' })
  },

  // 批量更新憑證狀態
  batchUpdateVoucherStatus: (data) => {
    return axios.put('/api/v1/vouchers/process/batch-status', data)
  },

  // ========== 憑證報表查詢 (CertificateReport) - SYSK310-SYSK500 ==========
  
  // 憑證明細報表查詢
  getVoucherDetailReport: (query) => {
    return axios.post('/api/v1/vouchers/reports/detail', query)
  },

  // 憑證統計報表查詢
  getVoucherStatisticsReport: (query) => {
    return axios.post('/api/v1/vouchers/reports/statistics', query)
  },

  // 憑證分析報表查詢
  getVoucherAnalysisReport: (query) => {
    return axios.post('/api/v1/vouchers/reports/analysis', query)
  }
}

