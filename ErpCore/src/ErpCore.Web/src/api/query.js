import axios from './axios'

/**
 * 查詢管理 API
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const queryApi = {
  // ========== 查詢功能維護 (Query) - SYSQ000 ==========
  
  // 查詢功能列表
  getQueries: (params) => {
    return axios.get('/api/v1/queries', { params })
  },

  // 查詢單筆功能
  getQuery: (tKey) => {
    return axios.get(`/api/v1/queries/${tKey}`)
  },

  // 新增查詢功能
  createQuery: (data) => {
    return axios.post('/api/v1/queries', data)
  },

  // 修改查詢功能
  updateQuery: (tKey, data) => {
    return axios.put(`/api/v1/queries/${tKey}`, data)
  },

  // 刪除查詢功能
  deleteQuery: (tKey) => {
    return axios.delete(`/api/v1/queries/${tKey}`)
  }
}

/**
 * 質量管理基礎功能 API (QualityBase) - SYSQ110-SYSQ120
 */
export const qualityBaseApi = {
  // ========== 零用金參數 (SYSQ110) ==========
  
  // 查詢零用金參數列表
  getCashParams: () => {
    return axios.get('/api/v1/quality/cash-params')
  },

  // 查詢單筆零用金參數
  getCashParam: (tKey) => {
    return axios.get(`/api/v1/quality/cash-params/${tKey}`)
  },

  // 新增零用金參數
  createCashParam: (data) => {
    return axios.post('/api/v1/quality/cash-params', data)
  },

  // 修改零用金參數
  updateCashParam: (tKey, data) => {
    return axios.put(`/api/v1/quality/cash-params/${tKey}`, data)
  },

  // 刪除零用金參數
  deleteCashParam: (tKey) => {
    return axios.delete(`/api/v1/quality/cash-params/${tKey}`)
  },

  // ========== 保管人及額度設定 (SYSQ120) ==========
  
  // 查詢保管人及額度列表
  getPcKeepList: (params) => {
    return axios.get('/api/v1/quality/pc-keep', { params })
  },

  // 查詢單筆保管人及額度
  getPcKeep: (tKey) => {
    return axios.get(`/api/v1/quality/pc-keep/${tKey}`)
  },

  // 新增保管人及額度
  createPcKeep: (data) => {
    return axios.post('/api/v1/quality/pc-keep', data)
  },

  // 修改保管人及額度
  updatePcKeep: (tKey, data) => {
    return axios.put(`/api/v1/quality/pc-keep/${tKey}`, data)
  },

  // 刪除保管人及額度
  deletePcKeep: (tKey) => {
    return axios.delete(`/api/v1/quality/pc-keep/${tKey}`)
  }
}

/**
 * 質量管理處理功能 API (QualityProcess) - SYSQ210-SYSQ250
 */
export const qualityProcessApi = {
  // ========== 零用金維護 (SYSQ210) ==========
  
  // 查詢零用金列表
  getPcCashList: (params) => {
    return axios.get('/api/v1/quality/process/pc-cash', { params })
  },

  // 查詢單筆零用金
  getPcCash: (tKey) => {
    return axios.get(`/api/v1/quality/process/pc-cash/${tKey}`)
  },

  // 新增零用金
  createPcCash: (data) => {
    return axios.post('/api/v1/quality/process/pc-cash', data)
  },

  // 修改零用金
  updatePcCash: (tKey, data) => {
    return axios.put(`/api/v1/quality/process/pc-cash/${tKey}`, data)
  },

  // 刪除零用金
  deletePcCash: (tKey) => {
    return axios.delete(`/api/v1/quality/process/pc-cash/${tKey}`)
  },

  // 批量新增零用金
  batchCreatePcCash: (data) => {
    return axios.post('/api/v1/quality/process/pc-cash/batch', data)
  }
}

/**
 * 質量管理報表功能 API (QualityReport) - SYSQ310-SYSQ340
 */
export const qualityReportApi = {
  // 零用金支付表報表 (SYSQ310)
  getPcCashPaymentReport: (data) => {
    return axios.post('/api/v1/quality/report/pc-cash-payment', data)
  },

  // 支付申請單報表 (SYSQ320)
  getPaymentApplicationReport: (data) => {
    return axios.post('/api/v1/quality/report/payment-application', data)
  },

  // 零用金撥補明細表報表 (SYSQ330)
  getPcCashReplenishmentReport: (data) => {
    return axios.post('/api/v1/quality/report/pc-cash-replenishment', data)
  },

  // 零用金報表 (SYSQ340)
  getPcCashReport: (data) => {
    return axios.post('/api/v1/quality/report/pc-cash-report', data)
  }
}

