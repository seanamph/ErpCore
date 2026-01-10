import axios from './axios'

/**
 * 客戶與發票管理模組 API (CustomerData, InvoicePrint, MailFax, LedgerData)
 * 遵循 C# API 欄位命名 (PascalCase)
 */

// 客戶資料維護 API
export const customerDataApi = {
  // 查詢客戶列表
  getCustomers: (params) => {
    return axios.get('/customer-data', { params })
  },

  // 查詢單筆客戶
  getCustomer: (customerId) => {
    return axios.get(`/customer-data/${customerId}`)
  },

  // 新增客戶
  createCustomer: (data) => {
    return axios.post('/customer-data', data)
  },

  // 修改客戶
  updateCustomer: (customerId, data) => {
    return axios.put(`/customer-data/${customerId}`, data)
  },

  // 刪除客戶
  deleteCustomer: (customerId) => {
    return axios.delete(`/customer-data/${customerId}`)
  },

  // 批次刪除客戶
  batchDeleteCustomers: (data) => {
    return axios.post('/customer-data/batch-delete', data)
  },

  // 檢查客戶編號是否存在
  checkCustomerExists: (customerId) => {
    return axios.get(`/customer-data/check/${customerId}`)
  }
}

// 發票列印作業 API
export const invoicePrintApi = {
  // 查詢發票列表
  getInvoices: (params) => {
    return axios.get('/invoice-print', { params })
  },

  // 查詢單筆發票
  getInvoice: (invoiceNo) => {
    return axios.get(`/invoice-print/${invoiceNo}`)
  },

  // 列印發票
  printInvoice: (invoiceNo, data) => {
    return axios.post(`/invoice-print/${invoiceNo}/print`, data)
  },

  // 批次列印發票
  batchPrintInvoices: (data) => {
    return axios.post('/invoice-print/batch-print', data)
  },

  // 查詢列印記錄
  getPrintLogs: (invoiceNo) => {
    return axios.get(`/invoice-print/${invoiceNo}/print-logs`)
  }
}

// 郵件傳真作業 API
export const mailFaxApi = {
  // 查詢郵件傳真作業列表
  getEmailFaxJobs: (params) => {
    return axios.get('/mail-fax', { params })
  },

  // 查詢單筆郵件傳真作業
  getEmailFaxJob: (jobId) => {
    return axios.get(`/mail-fax/${jobId}`)
  },

  // 發送郵件
  sendEmail: (data) => {
    return axios.post('/mail-fax/send-email', data)
  },

  // 發送傳真
  sendFax: (data) => {
    return axios.post('/mail-fax/send-fax', data)
  },

  // 批次發送
  batchSend: (data) => {
    return axios.post('/mail-fax/batch-send', data)
  },

  // 重試發送
  retryJob: (jobId) => {
    return axios.post(`/mail-fax/${jobId}/retry`)
  }
}

// 總帳資料維護 API
export const ledgerDataApi = {
  // 查詢總帳列表
  getGeneralLedgers: (params) => {
    return axios.get('/ledger-data', { params })
  },

  // 查詢單筆總帳
  getGeneralLedger: (ledgerId) => {
    return axios.get(`/ledger-data/${ledgerId}`)
  },

  // 新增總帳
  createGeneralLedger: (data) => {
    return axios.post('/ledger-data', data)
  },

  // 修改總帳
  updateGeneralLedger: (ledgerId, data) => {
    return axios.put(`/ledger-data/${ledgerId}`, data)
  },

  // 刪除總帳
  deleteGeneralLedger: (ledgerId) => {
    return axios.delete(`/ledger-data/${ledgerId}`)
  },

  // 過帳
  postLedger: (ledgerId) => {
    return axios.post(`/ledger-data/${ledgerId}/post`)
  },

  // 查詢科目餘額
  getAccountBalances: (params) => {
    return axios.get('/ledger-data/account-balances', { params })
  },

  // 總帳結帳
  closeLedger: (data) => {
    return axios.post('/ledger-data/close', data)
  }
}

