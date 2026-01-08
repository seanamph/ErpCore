import axios from './axios'

/**
 * 供應商商品資料維護 API (SYSW110)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const supplierGoodsApi = {
  // 查詢供應商商品列表
  getSupplierGoods: (params) => {
    return axios.get('/supplier-goods', { params })
  },

  // 查詢單筆供應商商品
  getSupplierGoodsById: (supplierId, barcodeId, shopId) => {
    return axios.get(`/supplier-goods/${supplierId}/${barcodeId}/${shopId}`)
  },

  // 新增供應商商品
  createSupplierGoods: (data) => {
    return axios.post('/supplier-goods', data)
  },

  // 修改供應商商品
  updateSupplierGoods: (supplierId, barcodeId, shopId, data) => {
    return axios.put(`/supplier-goods/${supplierId}/${barcodeId}/${shopId}`, data)
  },

  // 刪除供應商商品
  deleteSupplierGoods: (supplierId, barcodeId, shopId) => {
    return axios.delete(`/supplier-goods/${supplierId}/${barcodeId}/${shopId}`)
  },

  // 批次刪除供應商商品
  batchDeleteSupplierGoods: (data) => {
    return axios.delete('/supplier-goods/batch', { data })
  },

  // 更新供應商商品狀態
  updateStatus: (supplierId, barcodeId, shopId, data) => {
    return axios.put(`/supplier-goods/${supplierId}/${barcodeId}/${shopId}/status`, data)
  }
}

/**
 * 商品進銷碼維護 API (SYSW137)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const productGoodsIdApi = {
  // 查詢商品進銷碼列表
  getProductGoodsIds: (params) => {
    return axios.get('/products/goods-ids', { params })
  },

  // 查詢單筆商品進銷碼
  getProductGoodsIdById: (goodsId) => {
    return axios.get(`/products/goods-ids/${goodsId}`)
  },

  // 新增商品進銷碼
  createProductGoodsId: (data) => {
    return axios.post('/products/goods-ids', data)
  },

  // 修改商品進銷碼
  updateProductGoodsId: (goodsId, data) => {
    return axios.put(`/products/goods-ids/${goodsId}`, data)
  },

  // 刪除商品進銷碼
  deleteProductGoodsId: (goodsId) => {
    return axios.delete(`/products/goods-ids/${goodsId}`)
  },

  // 批次刪除商品進銷碼
  batchDeleteProductGoodsId: (data) => {
    return axios.delete('/products/goods-ids/batch', { data })
  },

  // 檢查商品進銷碼是否存在
  checkProductGoodsIdExists: (goodsId) => {
    return axios.get(`/products/goods-ids/${goodsId}/exists`)
  },

  // 更新商品進銷碼狀態
  updateStatus: (goodsId, data) => {
    return axios.put(`/products/goods-ids/${goodsId}/status`, data)
  }
}

