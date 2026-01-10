import axios from './axios'

/**
 * 會計財務管理 API
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const accountingApi = {
  // ========== 會計科目維護 (AccountSubject) ==========
  
  // 查詢會計科目列表
  getAccountSubjects: (params) => {
    return axios.get('/accounting/account-subjects', { params })
  },

  // 查詢單筆會計科目
  getAccountSubject: (stypeId) => {
    return axios.get(`/accounting/account-subjects/${stypeId}`)
  },

  // 新增會計科目
  createAccountSubject: (data) => {
    return axios.post('/accounting/account-subjects', data)
  },

  // 修改會計科目
  updateAccountSubject: (stypeId, data) => {
    return axios.put(`/accounting/account-subjects/${stypeId}`, data)
  },

  // 刪除會計科目
  deleteAccountSubject: (stypeId) => {
    return axios.delete(`/accounting/account-subjects/${stypeId}`)
  },

  // 檢查科目代號是否存在
  checkAccountSubjectExists: (stypeId) => {
    return axios.get(`/accounting/account-subjects/${stypeId}/exists`)
  },

  // 檢查未沖帳餘額
  checkUnsettledBalance: (stypeId) => {
    return axios.get(`/accounting/account-subjects/${stypeId}/check-unsettled-balance`)
  },

  // ========== 會計管理 (Accounting) ==========
  
  // 查詢傳票列表
  getVouchers: (params) => {
    return axios.get('/accounting/vouchers', { params })
  },

  // 查詢單筆傳票
  getVoucher: (voucherId) => {
    return axios.get(`/accounting/vouchers/${voucherId}`)
  },

  // 新增傳票
  createVoucher: (data) => {
    return axios.post('/accounting/vouchers', data)
  },

  // 修改傳票
  updateVoucher: (voucherId, data) => {
    return axios.put(`/accounting/vouchers/${voucherId}`, data)
  },

  // 刪除傳票
  deleteVoucher: (voucherId) => {
    return axios.delete(`/accounting/vouchers/${voucherId}`)
  },

  // 過帳傳票
  postVoucher: (voucherId) => {
    return axios.post(`/accounting/vouchers/${voucherId}/post`)
  },

  // 取消過帳傳票
  unpostVoucher: (voucherId) => {
    return axios.post(`/accounting/vouchers/${voucherId}/unpost`)
  },

  // ========== 財務交易 (FinancialTransaction) ==========
  
  // 查詢財務交易列表
  getFinancialTransactions: (params) => {
    return axios.get('/accounting/financial-transactions', { params })
  },

  // 查詢單筆財務交易
  getFinancialTransaction: (transactionId) => {
    return axios.get(`/accounting/financial-transactions/${transactionId}`)
  },

  // 新增財務交易
  createFinancialTransaction: (data) => {
    return axios.post('/accounting/financial-transactions', data)
  },

  // 修改財務交易
  updateFinancialTransaction: (transactionId, data) => {
    return axios.put(`/accounting/financial-transactions/${transactionId}`, data)
  },

  // 刪除財務交易
  deleteFinancialTransaction: (transactionId) => {
    return axios.delete(`/accounting/financial-transactions/${transactionId}`)
  },

  // ========== 資產管理 (Asset) ==========
  
  // 查詢資產列表
  getAssets: (params) => {
    return axios.get('/accounting/assets', { params })
  },

  // 查詢單筆資產
  getAsset: (assetId) => {
    return axios.get(`/accounting/assets/${assetId}`)
  },

  // 新增資產
  createAsset: (data) => {
    return axios.post('/accounting/assets', data)
  },

  // 修改資產
  updateAsset: (assetId, data) => {
    return axios.put(`/accounting/assets/${assetId}`, data)
  },

  // 刪除資產
  deleteAsset: (assetId) => {
    return axios.delete(`/accounting/assets/${assetId}`)
  },

  // ========== 財務報表 (FinancialReport) ==========
  
  // 查詢財務報表
  getFinancialReports: (params) => {
    return axios.get('/accounting/financial-reports', { params })
  },

  // 匯出財務報表
  exportFinancialReport: (params) => {
    return axios.get('/accounting/financial-reports/export', { params, responseType: 'blob' })
  },

  // ========== 其他財務功能 (OtherFinancial) ==========
  
  // 查詢其他財務功能列表
  getOtherFinancials: (params) => {
    return axios.get('/accounting/other-financials', { params })
  },

  // 執行其他財務功能
  executeOtherFinancial: (functionCode, data) => {
    return axios.post(`/accounting/other-financials/${functionCode}/execute`, data)
  }
}

