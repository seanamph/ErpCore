import axios from './axios'

/**
 * 採購單驗收作業 API (SYSW324)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const purchaseReceiptApi = {
  // 查詢待驗收採購單列表
  getPendingOrders: (params) => {
    return axios.get('/purchase-receipts/pending-orders', { params })
  },

  // 查詢驗收單列表
  getPurchaseReceipts: (params) => {
    return axios.get('/purchase-receipts', { params })
  },

  // 查詢單筆驗收單
  getPurchaseReceipt: (receiptId) => {
    return axios.get(`/purchase-receipts/${receiptId}`)
  },

  // 依採購單號建立驗收單
  createReceiptFromOrder: (orderId) => {
    return axios.post(`/purchase-receipts/from-order/${orderId}`)
  },

  // 新增驗收單
  createPurchaseReceipt: (data) => {
    return axios.post('/purchase-receipts', data)
  },

  // 修改驗收單
  updatePurchaseReceipt: (receiptId, data) => {
    return axios.put(`/purchase-receipts/${receiptId}`, data)
  },

  // 刪除驗收單
  deletePurchaseReceipt: (receiptId) => {
    return axios.delete(`/purchase-receipts/${receiptId}`)
  },

  // 確認驗收
  confirmReceipt: (receiptId) => {
    return axios.post(`/purchase-receipts/${receiptId}/confirm`)
  },

  // 取消驗收單
  cancelPurchaseReceipt: (receiptId) => {
    return axios.post(`/purchase-receipts/${receiptId}/cancel`)
  }
}

/**
 * 採購單驗收作業 API V2 (SYSW336)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const purchaseReceiptV2Api = {
  // 查詢待驗收採購單列表
  getPendingOrders: (params) => {
    return axios.get('/purchase-receipts-v2/pending-orders', { params })
  },

  // 查詢驗收單列表
  getPurchaseReceipts: (params) => {
    return axios.get('/purchase-receipts-v2', { params })
  },

  // 查詢單筆驗收單
  getPurchaseReceipt: (receiptId) => {
    return axios.get(`/purchase-receipts-v2/${receiptId}`)
  },

  // 依採購單號建立驗收單
  createReceiptFromOrder: (orderId) => {
    return axios.post(`/purchase-receipts-v2/from-order/${orderId}`)
  },

  // 新增驗收單
  createPurchaseReceipt: (data) => {
    return axios.post('/purchase-receipts-v2', data)
  },

  // 修改驗收單
  updatePurchaseReceipt: (receiptId, data) => {
    return axios.put(`/purchase-receipts-v2/${receiptId}`, data)
  },

  // 刪除驗收單
  deletePurchaseReceipt: (receiptId) => {
    return axios.delete(`/purchase-receipts-v2/${receiptId}`)
  },

  // 確認驗收
  confirmReceipt: (receiptId) => {
    return axios.post(`/purchase-receipts-v2/${receiptId}/confirm`)
  },

  // 取消驗收單
  cancelPurchaseReceipt: (receiptId) => {
    return axios.post(`/purchase-receipts-v2/${receiptId}/cancel`)
  }
}

/**
 * 訂退貨申請作業 API (SYSW315)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const purchaseOrderApi = {
  // 查詢採購單列表
  getPurchaseOrders: (params) => {
    return axios.get('/purchase-orders', { params })
  },

  // 查詢單筆採購單
  getPurchaseOrder: (orderId) => {
    return axios.get(`/purchase-orders/${orderId}`)
  },

  // 新增採購單
  createPurchaseOrder: (data) => {
    return axios.post('/purchase-orders', data)
  },

  // 修改採購單
  updatePurchaseOrder: (orderId, data) => {
    return axios.put(`/purchase-orders/${orderId}`, data)
  },

  // 刪除採購單
  deletePurchaseOrder: (orderId) => {
    return axios.delete(`/purchase-orders/${orderId}`)
  },

  // 送出採購單
  submitPurchaseOrder: (orderId) => {
    return axios.post(`/purchase-orders/${orderId}/submit`)
  },

  // 審核採購單
  approvePurchaseOrder: (orderId) => {
    return axios.post(`/purchase-orders/${orderId}/approve`)
  },

  // 取消採購單
  cancelPurchaseOrder: (orderId) => {
    return axios.post(`/purchase-orders/${orderId}/cancel`)
  }
}

/**
 * 已日結採購單驗收調整作業 API (SYSW333)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const settledAdjustmentApi = {
  // 查詢已日結採購單驗收調整列表
  getSettledAdjustments: (params) => {
    return axios.get('/purchase-receipts/settled-adjustments', { params })
  },

  // 查詢單筆已日結採購單驗收調整
  getSettledAdjustment: (receiptId) => {
    return axios.get(`/purchase-receipts/settled-adjustments/${receiptId}`)
  },

  // 查詢可用已日結採購單
  getSettledOrders: (params) => {
    return axios.get('/purchase-receipts/settled-orders', { params })
  },

  // 新增已日結採購單驗收調整
  createSettledAdjustment: (data) => {
    return axios.post('/purchase-receipts/settled-adjustments', data)
  },

  // 修改已日結採購單驗收調整
  updateSettledAdjustment: (receiptId, data) => {
    return axios.put(`/purchase-receipts/settled-adjustments/${receiptId}`, data)
  },

  // 刪除已日結採購單驗收調整
  deleteSettledAdjustment: (receiptId) => {
    return axios.delete(`/purchase-receipts/settled-adjustments/${receiptId}`)
  },

  // 審核已日結採購單驗收調整
  approveSettledAdjustment: (receiptId, data) => {
    return axios.post(`/purchase-receipts/settled-adjustments/${receiptId}/approve`, data)
  }
}

/**
 * 已日結退貨單驗退調整作業 API (SYSW530)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const closedReturnAdjustmentApi = {
  // 查詢已日結退貨單驗退調整列表
  getClosedReturnAdjustments: (params) => {
    return axios.get('/purchase-receipts/closed-return-adjustments', { params })
  },

  // 查詢單筆已日結退貨單驗退調整
  getClosedReturnAdjustment: (receiptId) => {
    return axios.get(`/purchase-receipts/closed-return-adjustments/${receiptId}`)
  },

  // 查詢可用的已日結退貨單
  getClosedReturnOrders: (params) => {
    return axios.get('/purchase-receipts/closed-return-orders', { params })
  },

  // 新增已日結退貨單驗退調整
  createClosedReturnAdjustment: (data) => {
    return axios.post('/purchase-receipts/closed-return-adjustments', data)
  },

  // 修改已日結退貨單驗退調整
  updateClosedReturnAdjustment: (receiptId, data) => {
    return axios.put(`/purchase-receipts/closed-return-adjustments/${receiptId}`, data)
  },

  // 刪除已日結退貨單驗退調整
  deleteClosedReturnAdjustment: (receiptId) => {
    return axios.delete(`/purchase-receipts/closed-return-adjustments/${receiptId}`)
  },

  // 審核已日結退貨單驗退調整
  approveClosedReturnAdjustment: (receiptId, data) => {
    return axios.post(`/purchase-receipts/closed-return-adjustments/${receiptId}/approve`, data)
  }
}

/**
 * 已日結退貨單驗退調整作業 API (SYSW337)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const closedReturnAdjustmentV2Api = {
  // 查詢已日結退貨單驗退調整列表
  getClosedReturnAdjustments: (params) => {
    return axios.get('/purchase-receipts/closed-return-adjustments-v2', { params })
  },

  // 查詢單筆已日結退貨單驗退調整
  getClosedReturnAdjustment: (receiptId) => {
    return axios.get(`/purchase-receipts/closed-return-adjustments-v2/${receiptId}`)
  },

  // 查詢可用的已日結退貨單
  getClosedReturnOrders: (params) => {
    return axios.get('/purchase-receipts/closed-return-orders-v2', { params })
  },

  // 新增已日結退貨單驗退調整
  createClosedReturnAdjustment: (data) => {
    return axios.post('/purchase-receipts/closed-return-adjustments-v2', data)
  },

  // 修改已日結退貨單驗退調整
  updateClosedReturnAdjustment: (receiptId, data) => {
    return axios.put(`/purchase-receipts/closed-return-adjustments-v2/${receiptId}`, data)
  },

  // 刪除已日結退貨單驗退調整
  deleteClosedReturnAdjustment: (receiptId) => {
    return axios.delete(`/purchase-receipts/closed-return-adjustments-v2/${receiptId}`)
  },

  // 審核已日結退貨單驗退調整
  approveClosedReturnAdjustment: (receiptId, data) => {
    return axios.post(`/purchase-receipts/closed-return-adjustments-v2/${receiptId}/approve`, data)
  }
}

/**
 * 現場打單作業 API (SYSW322)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const onSitePurchaseOrderApi = {
  // 查詢現場打單申請單列表
  getOnSitePurchaseOrders: (params) => {
    return axios.get('/on-site-purchase-orders', { params })
  },

  // 查詢單筆現場打單申請單
  getOnSitePurchaseOrder: (orderId) => {
    return axios.get(`/on-site-purchase-orders/${orderId}`)
  },

  // 新增現場打單申請單
  createOnSitePurchaseOrder: (data) => {
    return axios.post('/on-site-purchase-orders', data)
  },

  // 修改現場打單申請單
  updateOnSitePurchaseOrder: (orderId, data) => {
    return axios.put(`/on-site-purchase-orders/${orderId}`, data)
  },

  // 刪除現場打單申請單
  deleteOnSitePurchaseOrder: (orderId) => {
    return axios.delete(`/on-site-purchase-orders/${orderId}`)
  },

  // 送出現場打單申請單
  submitOnSitePurchaseOrder: (orderId) => {
    return axios.post(`/on-site-purchase-orders/${orderId}/submit`)
  },

  // 根據條碼查詢商品資訊
  getGoodsByBarcode: (barcode) => {
    return axios.get('/on-site-purchase-orders/goods-by-barcode', { params: { barcode } })
  }
}

/**
 * 訂退貨申請作業 API V2 (SYSW316)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const purchaseOrderV2Api = {
  // 查詢採購單列表
  getPurchaseOrders: (params) => {
    return axios.get('/purchase-orders-v2', { params })
  },

  // 查詢單筆採購單
  getPurchaseOrder: (orderId) => {
    return axios.get(`/purchase-orders-v2/${orderId}`)
  },

  // 新增採購單
  createPurchaseOrder: (data) => {
    return axios.post('/purchase-orders-v2', data)
  },

  // 修改採購單
  updatePurchaseOrder: (orderId, data) => {
    return axios.put(`/purchase-orders-v2/${orderId}`, data)
  },

  // 刪除採購單
  deletePurchaseOrder: (orderId) => {
    return axios.delete(`/purchase-orders-v2/${orderId}`)
  },

  // 送出採購單
  submitPurchaseOrder: (orderId) => {
    return axios.post(`/purchase-orders-v2/${orderId}/submit`)
  },

  // 審核採購單
  approvePurchaseOrder: (orderId) => {
    return axios.post(`/purchase-orders-v2/${orderId}/approve`)
  },

  // 取消採購單
  cancelPurchaseOrder: (orderId) => {
    return axios.post(`/purchase-orders-v2/${orderId}/cancel`)
  }
}

