import axios from './axios'

/**
 * 發票銷售管理模組 API (InvoiceSales)
 * 遵循 C# API 欄位命名 (PascalCase)
 */

// ========== 發票資料維護 (InvoiceData) - SYSG110-SYSG190 ==========
export const invoiceDataApi = {
  // 查詢發票列表
  getInvoices: (params) => {
    return axios.get('/api/v1/invoices', { params })
  },

  // 查詢單筆發票
  getInvoice: (tKey) => {
    return axios.get(`/api/v1/invoices/${tKey}`)
  },

  // 新增發票
  createInvoice: (data) => {
    return axios.post('/api/v1/invoices', data)
  },

  // 修改發票
  updateInvoice: (tKey, data) => {
    return axios.put(`/api/v1/invoices/${tKey}`, data)
  },

  // 刪除發票
  deleteInvoice: (tKey) => {
    return axios.delete(`/api/v1/invoices/${tKey}`)
  }
}

// ========== 電子發票列印 (InvoicePrint) - SYSG210-SYSG2B0 ==========
export const invoicePrintApi = {
  // 查詢電子發票列表
  getElectronicInvoices: (params) => {
    return axios.get('/api/v1/electronic-invoices', { params })
  },

  // 查詢單筆電子發票
  getElectronicInvoice: (tKey) => {
    return axios.get(`/api/v1/electronic-invoices/${tKey}`)
  },

  // 新增電子發票
  createElectronicInvoice: (data) => {
    return axios.post('/api/v1/electronic-invoices', data)
  },

  // 修改電子發票
  updateElectronicInvoice: (tKey, data) => {
    return axios.put(`/api/v1/electronic-invoices/${tKey}`, data)
  },

  // 刪除電子發票
  deleteElectronicInvoice: (tKey) => {
    return axios.delete(`/api/v1/electronic-invoices/${tKey}`)
  },

  // 電子發票手動取號列印
  manualPrint: (data) => {
    return axios.post('/api/v1/electronic-invoices/manual-print', data)
  },

  // 查詢中獎清冊
  getAwardList: (data) => {
    return axios.post('/api/v1/electronic-invoices/award-list', data)
  },

  // 中獎清冊列印
  awardPrint: (data) => {
    return axios.post('/api/v1/electronic-invoices/award-print', data)
  },

  // 查詢電子發票列印設定
  getPrintSettings: () => {
    return axios.get('/api/v1/electronic-invoices/print-settings')
  },

  // 查詢單筆電子發票列印設定
  getPrintSetting: (settingId) => {
    return axios.get(`/api/v1/electronic-invoices/print-settings/${settingId}`)
  },

  // 更新電子發票列印設定
  updatePrintSetting: (settingId, data) => {
    return axios.put(`/api/v1/electronic-invoices/print-settings/${settingId}`, data)
  }
}

// ========== 銷售資料維護 (SalesData) - SYSG410-SYSG460 ==========
// 使用 sales.js 中的 salesApi

// ========== 銷售查詢作業 (SalesQuery) - SYSG510-SYSG5D0 ==========
export const salesQueryApi = {
  // 查詢銷售單列表
  query: (params) => {
    return axios.get('/api/v1/sales-orders/query', { params })
  },

  // 查詢銷售單統計
  getStatistics: (params) => {
    return axios.get('/api/v1/sales-orders/query/statistics', { params })
  }
}

// ========== 報表查詢作業 (SalesReportQuery) - SYSG610-SYSG640 ==========
export const salesReportQueryApi = {
  // 查詢報表資料（明細報表）
  queryDetailReport: (params) => {
    return axios.get('/api/v1/sales-reports/detail', { params })
  },

  // 查詢報表資料（彙總報表）
  querySummaryReport: (params) => {
    return axios.get('/api/v1/sales-reports/summary', { params })
  }
}

// ========== 報表列印作業 (SalesReportPrint) - SYSG710-SYSG7I0 ==========
export const salesReportPrintApi = {
  // 列印報表
  printReport: (data) => {
    return axios.post('/api/v1/reports/print', data)
  },

  // 預覽報表
  previewReport: (data) => {
    return axios.post('/api/v1/reports/preview', data)
  },

  // 查詢報表模板列表
  getTemplates: (params) => {
    return axios.get('/api/v1/reports/templates', { params })
  }
}

// ========== B2B發票資料維護 (B2BInvoiceData) ==========
export const b2bInvoiceDataApi = {
  // 查詢B2B發票列表
  getB2BInvoices: (params) => {
    return axios.get('/api/v1/b2b-invoices', { params })
  },

  // 查詢單筆B2B發票
  getB2BInvoice: (tKey) => {
    return axios.get(`/api/v1/b2b-invoices/${tKey}`)
  },

  // 新增B2B發票
  createB2BInvoice: (data) => {
    return axios.post('/api/v1/b2b-invoices', data)
  },

  // 修改B2B發票
  updateB2BInvoice: (tKey, data) => {
    return axios.put(`/api/v1/b2b-invoices/${tKey}`, data)
  },

  // 刪除B2B發票
  deleteB2BInvoice: (tKey) => {
    return axios.delete(`/api/v1/b2b-invoices/${tKey}`)
  }
}

// ========== B2B電子發票列印 (B2BInvoicePrint) ==========
export const b2bInvoicePrintApi = {
  // 查詢B2B電子發票列表
  getB2BElectronicInvoices: (params) => {
    return axios.get('/api/v1/b2b-electronic-invoices', { params })
  },

  // 查詢單筆B2B電子發票
  getB2BElectronicInvoice: (tKey) => {
    return axios.get(`/api/v1/b2b-electronic-invoices/${tKey}`)
  },

  // B2B電子發票列印
  printB2BInvoice: (data) => {
    return axios.post('/api/v1/b2b-electronic-invoices/print', data)
  }
}

// ========== B2B銷售資料維護 (B2BSalesData) ==========
export const b2bSalesDataApi = {
  // 查詢B2B銷售單列表
  getB2BSalesOrders: (params) => {
    return axios.get('/api/v1/b2b-sales-orders', { params })
  },

  // 查詢單筆B2B銷售單
  getB2BSalesOrder: (tKey) => {
    return axios.get(`/api/v1/b2b-sales-orders/${tKey}`)
  },

  // 根據銷售單號查詢B2B銷售單
  getB2BSalesOrderByOrderId: (orderId) => {
    return axios.get(`/api/v1/b2b-sales-orders/order/${orderId}`)
  },

  // 新增B2B銷售單
  createB2BSalesOrder: (data) => {
    return axios.post('/api/v1/b2b-sales-orders', data)
  },

  // 修改B2B銷售單
  updateB2BSalesOrder: (tKey, data) => {
    return axios.put(`/api/v1/b2b-sales-orders/${tKey}`, data)
  },

  // 刪除B2B銷售單
  deleteB2BSalesOrder: (tKey) => {
    return axios.delete(`/api/v1/b2b-sales-orders/${tKey}`)
  },

  // B2B銷售單傳輸
  transferB2BSalesOrder: (orderId) => {
    return axios.post(`/api/v1/b2b-sales-orders/${orderId}/transfer`)
  }
}

// ========== B2B銷售查詢作業 (B2BSalesQuery) ==========
export const b2bSalesQueryApi = {
  // 查詢B2B銷售單列表
  query: (params) => {
    return axios.get('/api/v1/b2b-sales-orders/query', { params })
  },

  // 查詢B2B銷售單統計
  getStatistics: (params) => {
    return axios.get('/api/v1/b2b-sales-orders/query/statistics', { params })
  }
}

