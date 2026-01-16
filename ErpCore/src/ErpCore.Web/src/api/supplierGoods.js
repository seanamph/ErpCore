import axios from './axios'

/**
 * SYSW110 - 供應商商品資料維護作業 API
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const supplierGoodsApi = {
  // 查詢供應商商品列表
  getSupplierGoods: (params) => {
    return axios.get('/supplier-goods', { params })
  },

  // 查詢單筆供應商商品
  getSupplierGood: (supplierId, barcodeId, shopId) => {
    return axios.get(`/supplier-goods/${supplierId}/${barcodeId}/${shopId}`)
  },

  // 新增供應商商品
  createSupplierGood: (data) => {
    return axios.post('/supplier-goods', data)
  },

  // 修改供應商商品
  updateSupplierGood: (supplierId, barcodeId, shopId, data) => {
    return axios.put(`/supplier-goods/${supplierId}/${barcodeId}/${shopId}`, data)
  },

  // 刪除供應商商品
  deleteSupplierGood: (supplierId, barcodeId, shopId) => {
    return axios.delete(`/supplier-goods/${supplierId}/${barcodeId}/${shopId}`)
  },

  // 批次刪除供應商商品
  deleteSupplierGoodsBatch: (data) => {
    return axios.delete('/supplier-goods/batch', { data })
  },

  // 更新狀態
  updateStatus: (supplierId, barcodeId, shopId, status) => {
    return axios.put(`/supplier-goods/${supplierId}/${barcodeId}/${shopId}/status`, { Status: status })
  }
}
