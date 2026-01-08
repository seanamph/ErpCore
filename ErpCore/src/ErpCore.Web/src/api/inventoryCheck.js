import axios from './axios'

/**
 * 盤點維護作業 API (SYSW53M)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const stocktakingPlanApi = {
  // 查詢盤點計劃列表
  getStocktakingPlans: (params) => {
    return axios.get('/stocktaking-plans', { params })
  },

  // 查詢單筆盤點計劃
  getStocktakingPlan: (planId) => {
    return axios.get(`/stocktaking-plans/${planId}`)
  },

  // 新增盤點計劃
  createStocktakingPlan: (data) => {
    return axios.post('/stocktaking-plans', data)
  },

  // 修改盤點計劃
  updateStocktakingPlan: (planId, data) => {
    return axios.put(`/stocktaking-plans/${planId}`, data)
  },

  // 刪除盤點計劃
  deleteStocktakingPlan: (planId) => {
    return axios.delete(`/stocktaking-plans/${planId}`)
  },

  // 審核盤點計劃
  approveStocktakingPlan: (planId) => {
    return axios.post(`/stocktaking-plans/${planId}/approve`)
  },

  // 上傳盤點資料
  uploadStocktakingData: (planId, formData) => {
    return axios.post(`/stocktaking-plans/${planId}/upload`, formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    })
  },

  // 計算盤點差異
  calculateStocktakingDiff: (planId) => {
    return axios.post(`/stocktaking-plans/${planId}/calculate`)
  },

  // 確認盤點結果
  confirmStocktakingResult: (planId) => {
    return axios.post(`/stocktaking-plans/${planId}/confirm`)
  },

  // 查詢盤點報表
  getStocktakingReport: (planId, params) => {
    return axios.get(`/stocktaking-plans/${planId}/report`, { params })
  }
}

