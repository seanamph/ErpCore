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

  // ========== 付款單管理 (PaymentVoucher) - SYSP271-SYSP2B0 ==========
  
  // 查詢付款單列表
  getPaymentVouchers: (params) => {
    return axios.get('/api/v1/payment-vouchers', { params })
  },

  // 查詢單筆付款單
  getPaymentVoucher: (paymentNo) => {
    return axios.get(`/api/v1/payment-vouchers/${paymentNo}`)
  },

  // 新增付款單
  createPaymentVoucher: (data) => {
    return axios.post('/api/v1/payment-vouchers', data)
  },

  // 修改付款單
  updatePaymentVoucher: (paymentNo, data) => {
    return axios.put(`/api/v1/payment-vouchers/${paymentNo}`, data)
  },

  // 刪除付款單
  deletePaymentVoucher: (paymentNo) => {
    return axios.delete(`/api/v1/payment-vouchers/${paymentNo}`)
  },

  // 確認付款單
  confirmPaymentVoucher: (paymentNo) => {
    return axios.post(`/api/v1/payment-vouchers/${paymentNo}/confirm`)
  },

  // 檢查付款單是否存在
  checkPaymentVoucherExists: (paymentNo) => {
    return axios.get(`/api/v1/payment-vouchers/${paymentNo}/exists`)
  },

  // ========== 銀行帳戶管理 (BankAccount) ==========
  
  // 查詢銀行帳戶列表
  getBankAccounts: (params) => {
    return axios.get('/api/v1/bank-accounts', { params })
  },

  // 查詢單筆銀行帳戶
  getBankAccount: (bankAccountId) => {
    return axios.get(`/api/v1/bank-accounts/${bankAccountId}`)
  },

  // 新增銀行帳戶
  createBankAccount: (data) => {
    return axios.post('/api/v1/bank-accounts', data)
  },

  // 修改銀行帳戶
  updateBankAccount: (bankAccountId, data) => {
    return axios.put(`/api/v1/bank-accounts/${bankAccountId}`, data)
  },

  // 刪除銀行帳戶
  deleteBankAccount: (bankAccountId) => {
    return axios.delete(`/api/v1/bank-accounts/${bankAccountId}`)
  },

  // 更新銀行帳戶狀態
  updateBankAccountStatus: (bankAccountId, status) => {
    return axios.put(`/api/v1/bank-accounts/${bankAccountId}/status`, { Status: status })
  },

  // 查詢銀行帳戶餘額
  getBankAccountBalance: (bankAccountId) => {
    return axios.get(`/api/v1/bank-accounts/${bankAccountId}/balance`)
  },

  // 檢查銀行帳戶是否存在
  checkBankAccountExists: (bankAccountId) => {
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
  },

  // ========== 採購報表列印 (PurchaseReportPrint) ==========
  
  // 查詢採購報表列印記錄列表
  getPurchaseReportPrints: (params) => {
    return axios.get('/api/v1/purchase-report-prints', { params })
  },

  // 查詢單筆採購報表列印記錄
  getPurchaseReportPrint: (tKey) => {
    return axios.get(`/api/v1/purchase-report-prints/${tKey}`)
  },

  // 新增採購報表列印記錄
  createPurchaseReportPrint: (data) => {
    return axios.post('/api/v1/purchase-report-prints', data)
  },

  // 修改採購報表列印記錄
  updatePurchaseReportPrint: (tKey, data) => {
    return axios.put(`/api/v1/purchase-report-prints/${tKey}`, data)
  },

  // 刪除採購報表列印記錄
  deletePurchaseReportPrint: (tKey) => {
    return axios.delete(`/api/v1/purchase-report-prints/${tKey}`)
  },

  // 下載報表檔案
  downloadPurchaseReportPrint: (tKey) => {
    return axios.get(`/api/v1/purchase-report-prints/${tKey}/download`, {
      responseType: 'blob'
    })
  },

  // 預覽報表（不保存記錄）
  previewPurchaseReportPrint: (data) => {
    return axios.post('/api/v1/purchase-report-prints/preview', data)
  },

  // 取得報表模板列表
  getPurchaseReportTemplates: (params) => {
    return axios.get('/api/v1/purchase-report-prints/templates', { params })
  },

  // ========== 採購擴展功能 (PurchaseExtendedFunction) - SYSP610 ==========
  
  // 查詢採購擴展功能列表
  getPurchaseExtendedFunctions: (params) => {
    return axios.get('/api/v1/purchase-extended', { params })
  },

  // 查詢單筆採購擴展功能（根據主鍵）
  getPurchaseExtendedFunction: (tKey) => {
    return axios.get(`/api/v1/purchase-extended/${tKey}`)
  },

  // 查詢單筆採購擴展功能（根據功能代碼）
  getPurchaseExtendedFunctionByExtFunctionId: (extFunctionId) => {
    return axios.get(`/api/v1/purchase-extended/by-id/${extFunctionId}`)
  },

  // 新增採購擴展功能
  createPurchaseExtendedFunction: (data) => {
    return axios.post('/api/v1/purchase-extended', data)
  },

  // 修改採購擴展功能
  updatePurchaseExtendedFunction: (tKey, data) => {
    return axios.put(`/api/v1/purchase-extended/${tKey}`, data)
  },

  // 刪除採購擴展功能
  deletePurchaseExtendedFunction: (tKey) => {
    return axios.delete(`/api/v1/purchase-extended/${tKey}`)
  },

  // 檢查採購擴展功能是否存在
  checkPurchaseExtendedFunctionExists: (extFunctionId) => {
    return axios.get(`/api/v1/purchase-extended/${extFunctionId}/exists`)
  },

  // ========== 採購擴展維護 (PurchaseExtendedMaintenance) - SYSPA10-SYSPB60 ==========
  
  // 查詢採購擴展維護列表
  getPurchaseExtendedMaintenances: (params) => {
    return axios.get('/api/v1/purchase-extended-maintenance', { params })
  },

  // 查詢單筆採購擴展維護（根據主鍵）
  getPurchaseExtendedMaintenance: (tKey) => {
    return axios.get(`/api/v1/purchase-extended-maintenance/${tKey}`)
  },

  // 查詢單筆採購擴展維護（根據維護代碼）
  getPurchaseExtendedMaintenanceByMaintenanceId: (maintenanceId) => {
    return axios.get(`/api/v1/purchase-extended-maintenance/by-id/${maintenanceId}`)
  },

  // 新增採購擴展維護
  createPurchaseExtendedMaintenance: (data) => {
    return axios.post('/api/v1/purchase-extended-maintenance', data)
  },

  // 修改採購擴展維護
  updatePurchaseExtendedMaintenance: (tKey, data) => {
    return axios.put(`/api/v1/purchase-extended-maintenance/${tKey}`, data)
  },

  // 刪除採購擴展維護
  deletePurchaseExtendedMaintenance: (tKey) => {
    return axios.delete(`/api/v1/purchase-extended-maintenance/${tKey}`)
  },

  // 檢查採購擴展維護是否存在
  checkPurchaseExtendedMaintenanceExists: (maintenanceId) => {
    return axios.get(`/api/v1/purchase-extended-maintenance/${maintenanceId}/exists`)
  },

  // ========== 採購報表列印 (PurchaseReportPrint) ==========
  
  // 查詢採購報表列印記錄列表
  getPurchaseReportPrints: (params) => {
    return axios.get('/api/v1/purchase-report-prints', { params })
  },

  // 查詢單筆採購報表列印記錄
  getPurchaseReportPrint: (tKey) => {
    return axios.get(`/api/v1/purchase-report-prints/${tKey}`)
  },

  // 新增採購報表列印記錄
  createPurchaseReportPrint: (data) => {
    return axios.post('/api/v1/purchase-report-prints', data)
  },

  // 修改採購報表列印記錄
  updatePurchaseReportPrint: (tKey, data) => {
    return axios.put(`/api/v1/purchase-report-prints/${tKey}`, data)
  },

  // 刪除採購報表列印記錄
  deletePurchaseReportPrint: (tKey) => {
    return axios.delete(`/api/v1/purchase-report-prints/${tKey}`)
  },

  // 批次刪除採購報表列印記錄
  batchDeletePurchaseReportPrints: (data) => {
    return axios.delete('/api/v1/purchase-report-prints/batch', { data })
  },

  // 下載報表檔案
  downloadPurchaseReportPrint: (tKey) => {
    return axios.get(`/api/v1/purchase-report-prints/${tKey}/download`, {
      responseType: 'blob'
    })
  },

  // 預覽報表
  previewPurchaseReportPrint: (data) => {
    return axios.post('/api/v1/purchase-report-prints/preview', data)
  },

  // 取得報表模板列表
  getPurchaseReportTemplates: (params) => {
    return axios.get('/api/v1/purchase-report-prints/templates', { params })
  }
}

