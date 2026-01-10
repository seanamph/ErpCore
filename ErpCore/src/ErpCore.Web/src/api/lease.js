import axios from './axios'

/**
 * 租賃管理 API
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const leaseApi = {
  // ========== 租賃資料維護 (LeaseData) ==========
  
  // 查詢租賃列表
  getLeases: (params) => {
    return axios.get('/api/v1/leases', { params })
  },

  // 查詢單筆租賃
  getLease: (leaseId) => {
    return axios.get(`/api/v1/leases/${leaseId}`)
  },

  // 新增租賃
  createLease: (data) => {
    return axios.post('/api/v1/leases', data)
  },

  // 修改租賃
  updateLease: (leaseId, data) => {
    return axios.put(`/api/v1/leases/${leaseId}`, data)
  },

  // 刪除租賃
  deleteLease: (leaseId) => {
    return axios.delete(`/api/v1/leases/${leaseId}`)
  },

  // 批次刪除租賃
  batchDeleteLeases: (data) => {
    return axios.post('/api/v1/leases/batch-delete', data)
  },

  // 更新租賃狀態
  updateLeaseStatus: (leaseId, data) => {
    return axios.put(`/api/v1/leases/${leaseId}/status`, data)
  },

  // 檢查租賃是否存在
  checkLeaseExists: (leaseId) => {
    return axios.get(`/api/v1/leases/${leaseId}/exists`)
  },

  // ========== 租賃擴展維護 (LeaseExtension) ==========
  
  // 查詢租賃擴展列表
  getLeaseExtensions: (params) => {
    return axios.get('/api/v1/lease-extensions', { params })
  },

  // 查詢單筆租賃擴展
  getLeaseExtension: (extensionId) => {
    return axios.get(`/api/v1/lease-extensions/${extensionId}`)
  },

  // 根據租賃編號查詢擴展列表
  getLeaseExtensionsByLeaseId: (leaseId) => {
    return axios.get(`/api/v1/lease-extensions/by-lease/${leaseId}`)
  },

  // 新增租賃擴展
  createLeaseExtension: (data) => {
    return axios.post('/api/v1/lease-extensions', data)
  },

  // 修改租賃擴展
  updateLeaseExtension: (extensionId, data) => {
    return axios.put(`/api/v1/lease-extensions/${extensionId}`, data)
  },

  // 刪除租賃擴展
  deleteLeaseExtension: (extensionId) => {
    return axios.delete(`/api/v1/lease-extensions/${extensionId}`)
  },

  // 批次刪除租賃擴展
  batchDeleteLeaseExtensions: (data) => {
    return axios.post('/api/v1/lease-extensions/batch-delete', data)
  },

  // 更新租賃擴展狀態
  updateLeaseExtensionStatus: (extensionId, data) => {
    return axios.put(`/api/v1/lease-extensions/${extensionId}/status`, data)
  },

  // 檢查租賃擴展是否存在
  checkLeaseExtensionExists: (extensionId) => {
    return axios.get(`/api/v1/lease-extensions/${extensionId}/exists`)
  },

  // ========== 租賃處理作業 (LeaseProcess) ==========
  
  // 查詢租賃處理列表
  getLeaseProcesses: (params) => {
    return axios.get('/api/v1/lease-processes', { params })
  },

  // 查詢單筆租賃處理
  getLeaseProcess: (processId) => {
    return axios.get(`/api/v1/lease-processes/${processId}`)
  },

  // 根據租賃編號查詢處理列表
  getLeaseProcessesByLeaseId: (leaseId) => {
    return axios.get(`/api/v1/lease-processes/by-lease/${leaseId}`)
  },

  // 新增租賃處理
  createLeaseProcess: (data) => {
    return axios.post('/api/v1/lease-processes', data)
  },

  // 修改租賃處理
  updateLeaseProcess: (processId, data) => {
    return axios.put(`/api/v1/lease-processes/${processId}`, data)
  },

  // 刪除租賃處理
  deleteLeaseProcess: (processId) => {
    return axios.delete(`/api/v1/lease-processes/${processId}`)
  },

  // 更新租賃處理狀態
  updateLeaseProcessStatus: (processId, data) => {
    return axios.put(`/api/v1/lease-processes/${processId}/status`, data)
  },

  // 執行租賃處理
  executeLeaseProcess: (processId, data) => {
    return axios.post(`/api/v1/lease-processes/${processId}/execute`, data)
  },

  // 審核租賃處理
  approveLeaseProcess: (processId, data) => {
    return axios.post(`/api/v1/lease-processes/${processId}/approve`, data)
  },

  // 檢查租賃處理是否存在
  checkLeaseProcessExists: (processId) => {
    return axios.get(`/api/v1/lease-processes/${processId}/exists`)
  }
}

