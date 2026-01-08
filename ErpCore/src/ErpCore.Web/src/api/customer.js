import axios from './axios'

/**
 * 客戶基本資料維護作業 API (CUS5110)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const customerApi = {
  // 查詢客戶列表
  getCustomers: (params) => {
    return axios.get('/customers', { params })
  },

  // 查詢單筆客戶
  getCustomer: (customerId) => {
    return axios.get(`/customers/${customerId}`)
  },

  // 驗證統一編號
  validateGuiId: (data) => {
    return axios.post('/customers/validate-gui-id', data)
  },

  // 新增客戶
  createCustomer: (data) => {
    return axios.post('/customers', data)
  },

  // 修改客戶
  updateCustomer: (customerId, data) => {
    return axios.put(`/customers/${customerId}`, data)
  },

  // 刪除客戶
  deleteCustomer: (customerId) => {
    return axios.delete(`/customers/${customerId}`)
  },

  // 批次刪除客戶
  batchDeleteCustomers: (data) => {
    return axios.delete('/customers/batch', { data })
  },

  // 新增客戶聯絡人
  createCustomerContact: (customerId, data) => {
    return axios.post(`/customers/${customerId}/contacts`, data)
  },

  // 修改客戶聯絡人
  updateCustomerContact: (customerId, contactId, data) => {
    return axios.put(`/customers/${customerId}/contacts/${contactId}`, data)
  },

  // 刪除客戶聯絡人
  deleteCustomerContact: (customerId, contactId) => {
    return axios.delete(`/customers/${customerId}/contacts/${contactId}`)
  }
}

/**
 * 客戶查詢作業 API (CUS5120)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const customerQueryApi = {
  // 進階查詢客戶列表
  advancedQuery: (data) => {
    return axios.post('/customers/query', data)
  },

  // 快速搜尋客戶
  search: (params) => {
    return axios.get('/customers/search', { params })
  },

  // 查詢客戶交易記錄
  getTransactions: (customerId, params) => {
    return axios.get(`/customers/${customerId}/transactions`, { params })
  },

  // 儲存查詢歷史記錄
  saveQueryHistory: (data) => {
    return axios.post('/customers/query-history', data)
  },

  // 取得查詢歷史記錄列表
  getQueryHistory: (params) => {
    return axios.get('/customers/query-history', { params })
  },

  // 刪除查詢歷史記錄
  deleteQueryHistory: (historyId) => {
    return axios.delete(`/customers/query-history/${historyId}`)
  }
}

/**
 * 客戶報表 API (CUS5130)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const customerReportApi = {
  // 查詢客戶報表
  getReport: (data) => {
    return axios.post('/customers/reports/cus5130', data)
  },

  // 匯出Excel
  exportExcel: (data) => {
    return axios.post('/customers/reports/cus5130/export/excel', data, {
      responseType: 'blob'
    })
  },

  // 匯出PDF
  exportPdf: (data) => {
    return axios.post('/customers/reports/cus5130/export/pdf', data, {
      responseType: 'blob'
    })
  },

  // 列印報表
  print: (data) => {
    return axios.post('/customers/reports/cus5130/print', data)
  }
}

