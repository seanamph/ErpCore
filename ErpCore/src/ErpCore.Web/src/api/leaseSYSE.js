import axios from './axios'

/**
 * 租賃管理SYSE API
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const leaseSYSEApi = {
  // ========== 租賃條件維護 (LeaseTerm) ==========
  
  // 查詢租賃條件列表
  getLeaseTerms: (params) => {
    return axios.get('/api/v1/lease-syse/data/terms', { params })
  },

  // 查詢單筆租賃條件
  getLeaseTerm: (tKey) => {
    return axios.get(`/api/v1/lease-syse/data/terms/${tKey}`)
  },

  // 根據租賃編號和版本查詢租賃條件
  getLeaseTermsByLeaseIdAndVersion: (leaseId, version) => {
    return axios.get(`/api/v1/lease-syse/data/leases/${leaseId}/versions/${version}/terms`)
  },

  // 新增租賃條件
  createLeaseTerm: (data) => {
    return axios.post('/api/v1/lease-syse/data/terms', data)
  },

  // 修改租賃條件
  updateLeaseTerm: (tKey, data) => {
    return axios.put(`/api/v1/lease-syse/data/terms/${tKey}`, data)
  },

  // 刪除租賃條件
  deleteLeaseTerm: (tKey) => {
    return axios.delete(`/api/v1/lease-syse/data/terms/${tKey}`)
  },

  // 根據租賃編號和版本刪除租賃條件
  deleteLeaseTermsByLeaseIdAndVersion: (leaseId, version) => {
    return axios.delete(`/api/v1/lease-syse/data/leases/${leaseId}/versions/${version}/terms`)
  },

  // 檢查租賃條件是否存在
  checkLeaseTermExists: (tKey) => {
    return axios.get(`/api/v1/lease-syse/data/terms/${tKey}/exists`)
  },

  // ========== 租賃會計分類維護 (LeaseAccountingCategory) ==========
  
  // 查詢租賃會計分類列表
  getLeaseAccountingCategories: (params) => {
    return axios.get('/api/v1/lease-syse/data/accounting-categories', { params })
  },

  // 查詢單筆租賃會計分類
  getLeaseAccountingCategory: (tKey) => {
    return axios.get(`/api/v1/lease-syse/data/accounting-categories/${tKey}`)
  },

  // 根據租賃編號和版本查詢租賃會計分類
  getLeaseAccountingCategoriesByLeaseIdAndVersion: (leaseId, version) => {
    return axios.get(`/api/v1/lease-syse/data/leases/${leaseId}/versions/${version}/accounting-categories`)
  },

  // 新增租賃會計分類
  createLeaseAccountingCategory: (data) => {
    return axios.post('/api/v1/lease-syse/data/accounting-categories', data)
  },

  // 修改租賃會計分類
  updateLeaseAccountingCategory: (tKey, data) => {
    return axios.put(`/api/v1/lease-syse/data/accounting-categories/${tKey}`, data)
  },

  // 刪除租賃會計分類
  deleteLeaseAccountingCategory: (tKey) => {
    return axios.delete(`/api/v1/lease-syse/data/accounting-categories/${tKey}`)
  },

  // 根據租賃編號和版本刪除租賃會計分類
  deleteLeaseAccountingCategoriesByLeaseIdAndVersion: (leaseId, version) => {
    return axios.delete(`/api/v1/lease-syse/data/leases/${leaseId}/versions/${version}/accounting-categories`)
  },

  // 檢查租賃會計分類是否存在
  checkLeaseAccountingCategoryExists: (tKey) => {
    return axios.get(`/api/v1/lease-syse/data/accounting-categories/${tKey}/exists`)
  },

  // ========== 租賃費用管理 (LeaseFee) ==========
  
  // 查詢費用列表
  getLeaseFees: (params) => {
    return axios.get('/api/v1/lease-syse/fees', { params })
  },

  // 查詢單筆費用
  getLeaseFee: (feeId) => {
    return axios.get(`/api/v1/lease-syse/fees/${feeId}`)
  },

  // 根據租賃編號查詢費用
  getLeaseFeesByLeaseId: (leaseId) => {
    return axios.get(`/api/v1/lease-syse/fees/leases/${leaseId}`)
  },

  // 新增費用
  createLeaseFee: (data) => {
    return axios.post('/api/v1/lease-syse/fees', data)
  },

  // 修改費用
  updateLeaseFee: (feeId, data) => {
    return axios.put(`/api/v1/lease-syse/fees/${feeId}`, data)
  },

  // 刪除費用
  deleteLeaseFee: (feeId) => {
    return axios.delete(`/api/v1/lease-syse/fees/${feeId}`)
  },

  // 更新費用狀態
  updateLeaseFeeStatus: (feeId, data) => {
    return axios.put(`/api/v1/lease-syse/fees/${feeId}/status`, data)
  },

  // 更新費用已付金額
  updateLeaseFeePaidAmount: (feeId, data) => {
    return axios.put(`/api/v1/lease-syse/fees/${feeId}/paid-amount`, data)
  },

  // 檢查費用是否存在
  checkLeaseFeeExists: (feeId) => {
    return axios.get(`/api/v1/lease-syse/fees/${feeId}/exists`)
  },

  // ========== 費用項目主檔 (LeaseFeeItem) ==========
  
  // 查詢費用項目列表
  getLeaseFeeItems: (params) => {
    return axios.get('/api/v1/lease-syse/fees/items', { params })
  },

  // 查詢單筆費用項目
  getLeaseFeeItem: (feeItemId) => {
    return axios.get(`/api/v1/lease-syse/fees/items/${feeItemId}`)
  },

  // 新增費用項目
  createLeaseFeeItem: (data) => {
    return axios.post('/api/v1/lease-syse/fees/items', data)
  },

  // 修改費用項目
  updateLeaseFeeItem: (feeItemId, data) => {
    return axios.put(`/api/v1/lease-syse/fees/items/${feeItemId}`, data)
  },

  // 刪除費用項目
  deleteLeaseFeeItem: (feeItemId) => {
    return axios.delete(`/api/v1/lease-syse/fees/items/${feeItemId}`)
  },

  // 更新費用項目狀態
  updateLeaseFeeItemStatus: (feeItemId, data) => {
    return axios.put(`/api/v1/lease-syse/fees/items/${feeItemId}/status`, data)
  },

  // 檢查費用項目是否存在
  checkLeaseFeeItemExists: (feeItemId) => {
    return axios.get(`/api/v1/lease-syse/fees/items/${feeItemId}/exists`)
  }
}

