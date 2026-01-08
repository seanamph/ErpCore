import axios from '../axios'

/**
 * 店舖管理 API
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const shopsApi = {
  // 查詢店舖列表
  getShops: (params) => {
    return axios.get('/shops', { params })
  },

  // 查詢單筆店舖
  getShop: (shopId) => {
    return axios.get(`/shops/${shopId}`)
  }
}

