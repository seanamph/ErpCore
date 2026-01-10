import axios from './axios'

/**
 * 合同管理 API
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const contractApi = {
  // ========== 合同資料維護 (ContractData) ==========
  
  // 查詢合同列表
  getContracts: (params) => {
    return axios.get('/api/v1/contracts', { params })
  },

  // 查詢單筆合同
  getContract: (contractId, version) => {
    return axios.get(`/api/v1/contracts/${contractId}/${version}`)
  },

  // 新增合同
  createContract: (data) => {
    return axios.post('/api/v1/contracts', data)
  },

  // 修改合同
  updateContract: (contractId, version, data) => {
    return axios.put(`/api/v1/contracts/${contractId}/${version}`, data)
  },

  // 刪除合同
  deleteContract: (contractId, version) => {
    return axios.delete(`/api/v1/contracts/${contractId}/${version}`)
  },

  // 審核合同
  approveContract: (contractId, version, data) => {
    return axios.put(`/api/v1/contracts/${contractId}/${version}/approve`, data)
  },

  // 產生新版本
  createNewVersion: (contractId, version, data) => {
    return axios.post(`/api/v1/contracts/${contractId}/${version}/new-version`, data)
  },

  // 檢查合同是否存在
  checkContractExists: (contractId, version) => {
    return axios.get(`/api/v1/contracts/${contractId}/${version}/exists`)
  },

  // ========== 合同處理作業 (ContractProcess) ==========
  
  // 查詢合同處理列表
  getContractProcesses: (params) => {
    return axios.get('/api/v1/contract-processes', { params })
  },

  // 查詢單筆合同處理
  getContractProcess: (processId) => {
    return axios.get(`/api/v1/contract-processes/${processId}`)
  },

  // 新增合同處理
  createContractProcess: (data) => {
    return axios.post('/api/v1/contract-processes', data)
  },

  // 修改合同處理
  updateContractProcess: (processId, data) => {
    return axios.put(`/api/v1/contract-processes/${processId}`, data)
  },

  // 刪除合同處理
  deleteContractProcess: (processId) => {
    return axios.delete(`/api/v1/contract-processes/${processId}`)
  },

  // ========== 合同擴展維護 (ContractExtension) ==========
  
  // 查詢合同擴展列表
  getContractExtensions: (params) => {
    return axios.get('/api/v1/contract-extensions', { params })
  },

  // 查詢單筆合同擴展
  getContractExtension: (contractId, version) => {
    return axios.get(`/api/v1/contract-extensions/${contractId}/${version}`)
  },

  // 新增合同擴展
  createContractExtension: (data) => {
    return axios.post('/api/v1/contract-extensions', data)
  },

  // 修改合同擴展
  updateContractExtension: (contractId, version, data) => {
    return axios.put(`/api/v1/contract-extensions/${contractId}/${version}`, data)
  },

  // 刪除合同擴展
  deleteContractExtension: (contractId, version) => {
    return axios.delete(`/api/v1/contract-extensions/${contractId}/${version}`)
  },

  // ========== CMS合同維護 (CmsContract) ==========
  
  // 查詢CMS合同列表
  getCmsContracts: (params) => {
    return axios.get('/api/v1/cms-contracts', { params })
  },

  // 查詢單筆CMS合同
  getCmsContract: (cmsContractId, version) => {
    return axios.get(`/api/v1/cms-contracts/${cmsContractId}/${version}`)
  },

  // 新增CMS合同
  createCmsContract: (data) => {
    return axios.post('/api/v1/cms-contracts', data)
  },

  // 修改CMS合同
  updateCmsContract: (cmsContractId, version, data) => {
    return axios.put(`/api/v1/cms-contracts/${cmsContractId}/${version}`, data)
  },

  // 刪除CMS合同
  deleteCmsContract: (cmsContractId, version) => {
    return axios.delete(`/api/v1/cms-contracts/${cmsContractId}/${version}`)
  }
}

