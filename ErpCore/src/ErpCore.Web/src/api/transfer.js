import axios from './axios'

/**
 * 調撥單驗收作業 API (SYSW352)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const transferReceiptApi = {
  // 查詢待驗收調撥單列表
  getPendingOrders: (params) => {
    return axios.get('/transfer-receipts/pending-orders', { params })
  },

  // 查詢驗收單列表
  getTransferReceipts: (params) => {
    return axios.get('/transfer-receipts', { params })
  },

  // 查詢單筆驗收單
  getTransferReceipt: (receiptId) => {
    return axios.get(`/transfer-receipts/${receiptId}`)
  },

  // 依調撥單號建立驗收單
  createReceiptFromOrder: (transferId) => {
    return axios.post(`/transfer-receipts/from-order/${transferId}`)
  },

  // 新增驗收單
  createTransferReceipt: (data) => {
    return axios.post('/transfer-receipts', data)
  },

  // 修改驗收單
  updateTransferReceipt: (receiptId, data) => {
    return axios.put(`/transfer-receipts/${receiptId}`, data)
  },

  // 刪除驗收單
  deleteTransferReceipt: (receiptId) => {
    return axios.delete(`/transfer-receipts/${receiptId}`)
  },

  // 確認驗收
  confirmReceipt: (receiptId) => {
    return axios.post(`/transfer-receipts/${receiptId}/confirm`)
  },

  // 取消驗收單
  cancelTransferReceipt: (receiptId) => {
    return axios.post(`/transfer-receipts/${receiptId}/cancel`)
  }
}

/**
 * 調撥單驗退作業 API (SYSW362)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const transferReturnApi = {
  // 查詢待驗退調撥單列表
  getPendingTransfers: (params) => {
    return axios.get('/transfer-return/pending-transfers', { params })
  },

  // 查詢驗退單列表
  getTransferReturns: (params) => {
    return axios.get('/transfer-return', { params })
  },

  // 查詢單筆驗退單
  getTransferReturn: (returnId) => {
    return axios.get(`/transfer-return/${returnId}`)
  },

  // 依調撥單號建立驗退單
  createReturnFromTransfer: (transferId) => {
    return axios.post(`/transfer-return/from-transfer/${transferId}`)
  },

  // 新增驗退單
  createTransferReturn: (data) => {
    return axios.post('/transfer-return', data)
  },

  // 修改驗退單
  updateTransferReturn: (returnId, data) => {
    return axios.put(`/transfer-return/${returnId}`, data)
  },

  // 刪除驗退單
  deleteTransferReturn: (returnId) => {
    return axios.delete(`/transfer-return/${returnId}`)
  },

  // 確認驗退
  confirmReturn: (returnId) => {
    return axios.post(`/transfer-return/${returnId}/confirm`)
  },

  // 取消驗退單
  cancelTransferReturn: (returnId) => {
    return axios.post(`/transfer-return/${returnId}/cancel`)
  }
}

/**
 * 調撥短溢維護作業 API (SYSW384)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const transferShortageApi = {
  // 查詢短溢單列表
  getTransferShortages: (params) => {
    return axios.get('/transfer-shortage', { params })
  },

  // 查詢單筆短溢單
  getTransferShortage: (shortageId) => {
    return axios.get(`/transfer-shortage/${shortageId}`)
  },

  // 依調撥單號建立短溢單
  createShortageFromTransfer: (transferId) => {
    return axios.post(`/transfer-shortage/from-transfer/${transferId}`)
  },

  // 新增短溢單
  createTransferShortage: (data) => {
    return axios.post('/transfer-shortage', data)
  },

  // 修改短溢單
  updateTransferShortage: (shortageId, data) => {
    return axios.put(`/transfer-shortage/${shortageId}`, data)
  },

  // 刪除短溢單
  deleteTransferShortage: (shortageId) => {
    return axios.delete(`/transfer-shortage/${shortageId}`)
  },

  // 審核短溢單
  approveShortage: (shortageId, data) => {
    return axios.post(`/transfer-shortage/${shortageId}/approve`, data)
  },

  // 處理短溢單
  processShortage: (shortageId, data) => {
    return axios.post(`/transfer-shortage/${shortageId}/process`, data)
  }
}

