import axios from './axios'

/**
 * 會計稅務管理 API
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const taxAccountingApi = {
  // ========== 會計科目維護 (AccountingSubject) ==========
  
  // 查詢會計科目列表
  getAccountSubjects: (params) => {
    return axios.get('/tax-accounting/account-subjects', { params })
  },

  // 查詢單筆會計科目
  getAccountSubject: (stypeId) => {
    return axios.get(`/tax-accounting/account-subjects/${stypeId}`)
  },

  // 新增會計科目
  createAccountSubject: (data) => {
    return axios.post('/tax-accounting/account-subjects', data)
  },

  // 修改會計科目
  updateAccountSubject: (stypeId, data) => {
    return axios.put(`/tax-accounting/account-subjects/${stypeId}`, data)
  },

  // 刪除會計科目
  deleteAccountSubject: (stypeId) => {
    return axios.delete(`/tax-accounting/account-subjects/${stypeId}`)
  },

  // 檢查科目代號是否存在
  checkAccountSubjectExists: (stypeId) => {
    return axios.get(`/tax-accounting/account-subjects/${stypeId}/exists`)
  },

  // 檢查未沖帳餘額
  checkUnsettledBalance: (stypeId) => {
    return axios.get(`/tax-accounting/account-subjects/${stypeId}/check-unsettled-balance`)
  },

  // ========== 會計憑證管理 (AccountingVoucher) ==========
  
  // 傳票型態設定
  getVoucherTypes: (params) => {
    return axios.get('/tax-accounting/vouchers/types', { params })
  },

  getVoucherType: (voucherId) => {
    return axios.get(`/tax-accounting/vouchers/types/${voucherId}`)
  },

  createVoucherType: (data) => {
    return axios.post('/tax-accounting/vouchers/types', data)
  },

  updateVoucherType: (voucherId, data) => {
    return axios.put(`/tax-accounting/vouchers/types/${voucherId}`, data)
  },

  deleteVoucherType: (voucherId) => {
    return axios.delete(`/tax-accounting/vouchers/types/${voucherId}`)
  },

  // 常用傳票資料維護
  getCommonVouchers: (params) => {
    return axios.get('/tax-accounting/vouchers/common', { params })
  },

  getCommonVoucher: (voucherId) => {
    return axios.get(`/tax-accounting/vouchers/common/${voucherId}`)
  },

  createCommonVoucher: (data) => {
    return axios.post('/tax-accounting/vouchers/common', data)
  },

  updateCommonVoucher: (voucherId, data) => {
    return axios.put(`/tax-accounting/vouchers/common/${voucherId}`, data)
  },

  deleteCommonVoucher: (voucherId) => {
    return axios.delete(`/tax-accounting/vouchers/common/${voucherId}`)
  },

  // ========== 會計帳簿管理 (AccountingBook) ==========
  
  // 現金流量大分類設定
  getCashFlowLargeTypes: (params) => {
    return axios.get('/tax-accounting/accounting-books/large-types', { params })
  },

  getCashFlowLargeType: (largeTypeId) => {
    return axios.get(`/tax-accounting/accounting-books/large-types/${largeTypeId}`)
  },

  createCashFlowLargeType: (data) => {
    return axios.post('/tax-accounting/accounting-books/large-types', data)
  },

  updateCashFlowLargeType: (largeTypeId, data) => {
    return axios.put(`/tax-accounting/accounting-books/large-types/${largeTypeId}`, data)
  },

  deleteCashFlowLargeType: (largeTypeId) => {
    return axios.delete(`/tax-accounting/accounting-books/large-types/${largeTypeId}`)
  },

  // 現金流量中分類設定
  getCashFlowMediumTypes: (params) => {
    return axios.get('/tax-accounting/accounting-books/medium-types', { params })
  },

  getCashFlowMediumType: (mediumTypeId) => {
    return axios.get(`/tax-accounting/accounting-books/medium-types/${mediumTypeId}`)
  },

  createCashFlowMediumType: (data) => {
    return axios.post('/tax-accounting/accounting-books/medium-types', data)
  },

  updateCashFlowMediumType: (mediumTypeId, data) => {
    return axios.put(`/tax-accounting/accounting-books/medium-types/${mediumTypeId}`, data)
  },

  deleteCashFlowMediumType: (mediumTypeId) => {
    return axios.delete(`/tax-accounting/accounting-books/medium-types/${mediumTypeId}`)
  },

  // 現金流量科目分類設定
  getCashFlowSubjectTypes: (params) => {
    return axios.get('/tax-accounting/accounting-books/subject-types', { params })
  },

  getCashFlowSubjectType: (subjectTypeId) => {
    return axios.get(`/tax-accounting/accounting-books/subject-types/${subjectTypeId}`)
  },

  createCashFlowSubjectType: (data) => {
    return axios.post('/tax-accounting/accounting-books/subject-types', data)
  },

  updateCashFlowSubjectType: (subjectTypeId, data) => {
    return axios.put(`/tax-accounting/accounting-books/subject-types/${subjectTypeId}`, data)
  },

  deleteCashFlowSubjectType: (subjectTypeId) => {
    return axios.delete(`/tax-accounting/accounting-books/subject-types/${subjectTypeId}`)
  },

  // 現金流量小計設定
  getCashFlowSubTotals: (params) => {
    return axios.get('/tax-accounting/accounting-books/sub-totals', { params })
  },

  getCashFlowSubTotal: (subTotalId) => {
    return axios.get(`/tax-accounting/accounting-books/sub-totals/${subTotalId}`)
  },

  createCashFlowSubTotal: (data) => {
    return axios.post('/tax-accounting/accounting-books/sub-totals', data)
  },

  updateCashFlowSubTotal: (subTotalId, data) => {
    return axios.put(`/tax-accounting/accounting-books/sub-totals/${subTotalId}`, data)
  },

  deleteCashFlowSubTotal: (subTotalId) => {
    return axios.delete(`/tax-accounting/accounting-books/sub-totals/${subTotalId}`)
  },

  // ========== 發票資料維護 (InvoiceData) ==========
  
  getVouchers: (params) => {
    return axios.get('/tax-accounting/invoice-data/vouchers', { params })
  },

  getVoucher: (voucherId) => {
    return axios.get(`/tax-accounting/invoice-data/vouchers/${voucherId}`)
  },

  createVoucher: (data) => {
    return axios.post('/tax-accounting/invoice-data/vouchers', data)
  },

  updateVoucher: (voucherId, data) => {
    return axios.put(`/tax-accounting/invoice-data/vouchers/${voucherId}`, data)
  },

  deleteVoucher: (voucherId) => {
    return axios.delete(`/tax-accounting/invoice-data/vouchers/${voucherId}`)
  },

  // ========== 交易資料處理 (TransactionData) ==========
  
  getTransactions: (params) => {
    return axios.get('/tax-accounting/transaction-data', { params })
  },

  getTransaction: (transactionId) => {
    return axios.get(`/tax-accounting/transaction-data/${transactionId}`)
  },

  createTransaction: (data) => {
    return axios.post('/tax-accounting/transaction-data', data)
  },

  updateTransaction: (transactionId, data) => {
    return axios.put(`/tax-accounting/transaction-data/${transactionId}`, data)
  },

  deleteTransaction: (transactionId) => {
    return axios.delete(`/tax-accounting/transaction-data/${transactionId}`)
  },

  // ========== 稅務報表查詢 (TaxReport) ==========
  
  getTaxReports: (params) => {
    return axios.get('/tax-accounting/tax-reports', { params })
  },

  getTaxReport: (reportId) => {
    return axios.get(`/tax-accounting/tax-reports/${reportId}`)
  },

  // ========== 稅務報表列印 (TaxReportPrint) ==========
  
  getTaxReportPrints: (params) => {
    return axios.get('/tax-accounting/tax-report-prints', { params })
  },

  printTaxReport: (reportId, data) => {
    return axios.post(`/tax-accounting/tax-report-prints/${reportId}/print`, data)
  },

  // ========== 暫存傳票審核作業 (VoucherAudit) ==========
  
  getTmpVouchers: (params) => {
    return axios.get('/tax-accounting/voucher-audit/tmp-vouchers', { params })
  },

  getTmpVoucher: (voucherId) => {
    return axios.get(`/tax-accounting/voucher-audit/tmp-vouchers/${voucherId}`)
  },

  auditVoucher: (voucherId, data) => {
    return axios.post(`/tax-accounting/voucher-audit/tmp-vouchers/${voucherId}/audit`, data)
  },

  rejectVoucher: (voucherId, data) => {
    return axios.post(`/tax-accounting/voucher-audit/tmp-vouchers/${voucherId}/reject`, data)
  },

  // ========== 傳票轉入作業 (VoucherImport) ==========
  
  getImportLogs: (params) => {
    return axios.get('/tax-accounting/voucher-import/logs', { params })
  },

  getImportLog: (logId) => {
    return axios.get(`/tax-accounting/voucher-import/logs/${logId}`)
  },

  importVouchers: (data) => {
    return axios.post('/tax-accounting/voucher-import/import', data)
  }
}

