import axios from './axios'

/**
 * 租賃管理SYSM API
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const leaseSYSMApi = {
  // ========== 停車位資料 (ParkingSpace) ==========
  
  // 查詢停車位列表
  getParkingSpaces: (params) => {
    return axios.get('/api/v1/lease-sysm/data/parking-spaces', { params })
  },

  // 查詢單筆停車位
  getParkingSpace: (parkingSpaceId) => {
    return axios.get(`/api/v1/lease-sysm/data/parking-spaces/${parkingSpaceId}`)
  },

  // 查詢可用停車位
  getAvailableParkingSpaces: (shopId) => {
    return axios.get('/api/v1/lease-sysm/data/parking-spaces/available', { params: { shopId } })
  },

  // 新增停車位
  createParkingSpace: (data) => {
    return axios.post('/api/v1/lease-sysm/data/parking-spaces', data)
  },

  // 修改停車位
  updateParkingSpace: (parkingSpaceId, data) => {
    return axios.put(`/api/v1/lease-sysm/data/parking-spaces/${parkingSpaceId}`, data)
  },

  // 刪除停車位
  deleteParkingSpace: (parkingSpaceId) => {
    return axios.delete(`/api/v1/lease-sysm/data/parking-spaces/${parkingSpaceId}`)
  },

  // 更新停車位狀態
  updateParkingSpaceStatus: (parkingSpaceId, data) => {
    return axios.put(`/api/v1/lease-sysm/data/parking-spaces/${parkingSpaceId}/status`, data)
  },

  // 檢查停車位是否存在
  checkParkingSpaceExists: (parkingSpaceId) => {
    return axios.get(`/api/v1/lease-sysm/data/parking-spaces/${parkingSpaceId}/exists`)
  },

  // ========== 租賃合同資料 (LeaseContract) ==========
  
  // 查詢租賃合同列表
  getLeaseContracts: (params) => {
    return axios.get('/api/v1/lease-sysm/data/contracts', { params })
  },

  // 查詢單筆租賃合同
  getLeaseContract: (contractNo) => {
    return axios.get(`/api/v1/lease-sysm/data/contracts/${contractNo}`)
  },

  // 根據租賃編號查詢租賃合同
  getLeaseContractsByLeaseId: (leaseId) => {
    return axios.get(`/api/v1/lease-sysm/data/leases/${leaseId}/contracts`)
  },

  // 新增租賃合同
  createLeaseContract: (data) => {
    return axios.post('/api/v1/lease-sysm/data/contracts', data)
  },

  // 修改租賃合同
  updateLeaseContract: (contractNo, data) => {
    return axios.put(`/api/v1/lease-sysm/data/contracts/${contractNo}`, data)
  },

  // 刪除租賃合同
  deleteLeaseContract: (contractNo) => {
    return axios.delete(`/api/v1/lease-sysm/data/contracts/${contractNo}`)
  },

  // 更新租賃合同狀態
  updateLeaseContractStatus: (contractNo, data) => {
    return axios.put(`/api/v1/lease-sysm/data/contracts/${contractNo}/status`, data)
  },

  // 檢查租賃合同是否存在
  checkLeaseContractExists: (contractNo) => {
    return axios.get(`/api/v1/lease-sysm/data/contracts/${contractNo}/exists`)
  },

  // ========== 租賃報表查詢 (LeaseReport) ==========
  
  // 查詢租賃報表查詢記錄列表
  getLeaseReports: (params) => {
    return axios.get('/api/v1/lease-sysm/reports/queries', { params })
  },

  // 查詢單筆租賃報表查詢記錄
  getLeaseReport: (queryId) => {
    return axios.get(`/api/v1/lease-sysm/reports/queries/${queryId}`)
  },

  // 新增租賃報表查詢記錄
  createLeaseReport: (data) => {
    return axios.post('/api/v1/lease-sysm/reports/queries', data)
  },

  // 刪除租賃報表查詢記錄
  deleteLeaseReport: (queryId) => {
    return axios.delete(`/api/v1/lease-sysm/reports/queries/${queryId}`)
  },

  // 匯出租賃報表
  exportLeaseReports: (params) => {
    return axios.get('/api/v1/lease-sysm/reports/export', { params, responseType: 'blob' })
  }
}

