import axios from './axios'

/**
 * SYSB110 - 商品分類資料維護作業 API
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const productCategoriesApi = {
  // 查詢商品分類列表
  getProductCategories: (params) => {
    return axios.get('/product-categories', { params })
  },

  // 查詢單筆商品分類
  getProductCategory: (tKey) => {
    return axios.get(`/product-categories/${tKey}`)
  },

  // 查詢商品分類樹狀結構
  getProductCategoryTree: (params) => {
    return axios.get('/product-categories/tree', { params })
  },

  // 新增商品分類
  createProductCategory: (data) => {
    return axios.post('/product-categories', data)
  },

  // 修改商品分類
  updateProductCategory: (tKey, data) => {
    return axios.put(`/product-categories/${tKey}`, data)
  },

  // 刪除商品分類
  deleteProductCategory: (tKey) => {
    return axios.delete(`/product-categories/${tKey}`)
  },

  // 批次刪除商品分類
  deleteProductCategoriesBatch: (data) => {
    return axios.delete('/product-categories/batch', { data })
  },

  // 查詢大分類列表
  getBClassList: (params) => {
    return axios.get('/product-categories/b-class', { params })
  },

  // 查詢中分類列表
  getMClassList: (params) => {
    return axios.get('/product-categories/m-class', { params })
  },

  // 查詢小分類列表
  getSClassList: (params) => {
    return axios.get('/product-categories/s-class', { params })
  },

  // 更新商品分類狀態
  updateProductCategoryStatus: (tKey, data) => {
    return axios.put(`/product-categories/${tKey}/status`, data)
  },

  // 更新商品分類項目個數
  updateProductCategoryItemCount: (tKey, data) => {
    return axios.put(`/product-categories/${tKey}/item-count`, data)
  }
}

