import axios from './axios'

/**
 * 採購供應商管理 API
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const procurementApi = {
  // ========== 採購管理 (Procurement) ==========
  
  // 查詢採購單列表
  getPurchaseOrders: (params) => {
    return axios.get('/procurement/orders', { params })
  },

  // 查詢單筆採購單
  getPurchaseOrder: (orderId) => {
    return axios.get(`/procurement/orders/${orderId}`)
  },

  // 新增採購單
  createPurchaseOrder: (data) => {
    return axios.post('/procurement/orders', data)
  },

  // 修改採購單
  updatePurchaseOrder: (orderId, data) => {
    return axios.put(`/procurement/orders/${orderId}`, data)
  },

  // 刪除採購單
  deletePurchaseOrder: (orderId) => {
    return axios.delete(`/procurement/orders/${orderId}`)
  },

  // 批次刪除採購單
  batchDeletePurchaseOrders: (data) => {
    return axios.delete('/procurement/orders/batch', { data })
  },

  // ========== 供應商管理 (Supplier) ==========
  
  // 查詢供應商列表
  getSuppliers: (params) => {
    return axios.get('/suppliers', { params })
  },

  // 查詢單筆供應商
  getSupplier: (supplierId) => {
    return axios.get(`/suppliers/${supplierId}`)
  },

  // 新增供應商
  createSupplier: (data) => {
    return axios.post('/suppliers', data)
  },

  // 修改供應商
  updateSupplier: (supplierId, data) => {
    return axios.put(`/suppliers/${supplierId}`, data)
  },

  // 刪除供應商
  deleteSupplier: (supplierId) => {
    return axios.delete(`/suppliers/${supplierId}`)
  },

  // 檢查供應商是否存在
  checkSupplierExists: (supplierId) => {
    return axios.get(`/suppliers/${supplierId}/exists`)
  },

  // ========== 付款管理 (Payment) ==========
  
  // 查詢付款列表
  getPayments: (params) => {
    return axios.get('/api/v1/payments', { params })
  },

  // 查詢單筆付款
  getPayment: (paymentId) => {
    return axios.get(`/api/v1/payments/${paymentId}`)
  },

  // 新增付款
  createPayment: (data) => {
    return axios.post('/api/v1/payments', data)
  },

  // 修改付款
  updatePayment: (paymentId, data) => {
    return axios.put(`/api/v1/payments/${paymentId}`, data)
  },

  // 刪除付款
  deletePayment: (paymentId) => {
    return axios.delete(`/api/v1/payments/${paymentId}`)
  },

  // 確認付款
  confirmPayment: (paymentId) => {
    return axios.post(`/api/v1/payments/${paymentId}/confirm`)
  },

  // 檢查付款是否存在
  checkPaymentExists: (paymentId) => {
    return axios.get(`/api/v1/payments/${paymentId}/exists`)
  },

  // ========== 銀行管理 (BankManagement) ==========
  
  // 查詢銀行列表
  getBanks: (params) => {
    return axios.get('/api/v1/bank-accounts', { params })
  },

  // 查詢單筆銀行
  getBank: (bankAccountId) => {
    return axios.get(`/api/v1/bank-accounts/${bankAccountId}`)
  },

  // 新增銀行
  createBank: (data) => {
    return axios.post('/api/v1/bank-accounts', data)
  },

  // 修改銀行
  updateBank: (bankAccountId, data) => {
    return axios.put(`/api/v1/bank-accounts/${bankAccountId}`, data)
  },

  // 刪除銀行
  deleteBank: (bankAccountId) => {
    return axios.delete(`/api/v1/bank-accounts/${bankAccountId}`)
  },

  // 更新銀行狀態
  updateBankStatus: (bankAccountId, status) => {
    return axios.put(`/api/v1/bank-accounts/${bankAccountId}/status`, { Status: status })
  },

  // 查詢銀行餘額
  getBankBalance: (bankAccountId) => {
    return axios.get(`/api/v1/bank-accounts/${bankAccountId}/balance`)
  },

  // 檢查銀行是否存在
  checkBankExists: (bankAccountId) => {
    return axios.get(`/api/v1/bank-accounts/${bankAccountId}/exists`)
  },

  // ========== 採購報表 (ProcurementReport) ==========
  
  // 查詢採購報表
  getProcurementReports: (params) => {
    return axios.get('/api/v1/purchase-reports/query', { params })
  },

  // 匯出採購報表
  exportProcurementReport: (data) => {
    return axios.post('/api/v1/purchase-reports/export', data, { responseType: 'blob' })
  },

  // ========== 採購其他功能 (ProcurementOther) ==========
  
  // 查詢採購其他功能列表
  getProcurementOthers: (params) => {
    return axios.get('/api/v1/purchase-others', { params })
  },

  // 查詢單筆採購其他功能（根據主鍵）
  getProcurementOther: (tKey) => {
    return axios.get(`/api/v1/purchase-others/${tKey}`)
  },

  // 查詢單筆採購其他功能（根據功能代碼）
  getProcurementOtherByFunctionId: (functionId) => {
    return axios.get(`/api/v1/purchase-others/by-id/${functionId}`)
  },

  // 新增採購其他功能
  createProcurementOther: (data) => {
    return axios.post('/api/v1/purchase-others', data)
  },

  // 修改採購其他功能
  updateProcurementOther: (tKey, data) => {
    return axios.put(`/api/v1/purchase-others/${tKey}`, data)
  },

  // 刪除採購其他功能
  deleteProcurementOther: (tKey) => {
    return axios.delete(`/api/v1/purchase-others/${tKey}`)
  },

  // 檢查採購其他功能是否存在
  checkProcurementOtherExists: (functionId) => {
    return axios.get(`/api/v1/purchase-others/${functionId}/exists`)
  }
}

