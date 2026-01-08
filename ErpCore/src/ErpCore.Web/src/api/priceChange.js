import axios from './axios'

/**
 * 商品永久變價作業 API (SYSW150)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const priceChangeApi = {
  // 查詢變價單列表
  getPriceChanges: (params) => {
    return axios.get('/price-changes', { params })
  },

  // 查詢單筆變價單（含明細）
  getPriceChangeById: (priceChangeId, priceChangeType) => {
    return axios.get(`/price-changes/${priceChangeId}/${priceChangeType}`)
  },

  // 新增變價單
  createPriceChange: (data) => {
    return axios.post('/price-changes', data)
  },

  // 修改變價單
  updatePriceChange: (priceChangeId, priceChangeType, data) => {
    return axios.put(`/price-changes/${priceChangeId}/${priceChangeType}`, data)
  },

  // 刪除變價單
  deletePriceChange: (priceChangeId, priceChangeType) => {
    return axios.delete(`/price-changes/${priceChangeId}/${priceChangeType}`)
  },

  // 審核變價單
  approvePriceChange: (priceChangeId, priceChangeType, data) => {
    return axios.put(`/price-changes/${priceChangeId}/${priceChangeType}/approve`, data)
  },

  // 確認變價單
  confirmPriceChange: (priceChangeId, priceChangeType, data) => {
    return axios.put(`/price-changes/${priceChangeId}/${priceChangeType}/confirm`, data)
  },

  // 作廢變價單
  cancelPriceChange: (priceChangeId, priceChangeType) => {
    return axios.put(`/price-changes/${priceChangeId}/${priceChangeType}/cancel`)
  }
}

