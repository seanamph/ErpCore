import axios from './axios'

/**
 * 收款管理 API (報表管理 - SYSR000)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const receiptApi = {
  // ========== 收款基礎功能 (ReceivingBase) - SYSR110-SYSR120 ==========
  
  // 查詢收款項目列表
  getArItems: (params) => {
    return axios.get('/api/v1/receipt/aritems', { params })
  },

  // 查詢單筆收款項目
  getArItem: (tKey) => {
    return axios.get(`/api/v1/receipt/aritems/${tKey}`)
  },

  // 查詢收款項目（依分店和項目代號）
  getArItemBySiteIdAndAritemId: (siteId, aritemId) => {
    return axios.get(`/api/v1/receipt/aritems/${siteId}/${aritemId}`)
  },

  // 新增收款項目
  createArItem: (data) => {
    return axios.post('/api/v1/receipt/aritems', data)
  },

  // 修改收款項目
  updateArItem: (tKey, data) => {
    return axios.put(`/api/v1/receipt/aritems/${tKey}`, data)
  },

  // 刪除收款項目
  deleteArItem: (tKey) => {
    return axios.delete(`/api/v1/receipt/aritems/${tKey}`)
  },

  // 檢查收款項目是否存在
  checkArItemExists: (siteId, aritemId) => {
    return axios.get('/api/v1/receipt/aritems/exists', { params: { siteId, aritemId } })
  },

  // ========== 收款處理功能 (ReceivingProcess) - SYSR210-SYSR240 ==========
  
  // 查詢應收帳款列表
  getAccountsReceivable: (params) => {
    return axios.get('/api/v1/receipt/accountsreceivable', { params })
  },

  // 查詢單筆應收帳款
  getAccountsReceivableById: (tKey) => {
    return axios.get(`/api/v1/receipt/accountsreceivable/${tKey}`)
  },

  // 新增應收帳款
  createAccountsReceivable: (data) => {
    return axios.post('/api/v1/receipt/accountsreceivable', data)
  },

  // 修改應收帳款
  updateAccountsReceivable: (tKey, data) => {
    return axios.put(`/api/v1/receipt/accountsreceivable/${tKey}`, data)
  },

  // 刪除應收帳款
  deleteAccountsReceivable: (tKey) => {
    return axios.delete(`/api/v1/receipt/accountsreceivable/${tKey}`)
  },

  // ========== 收款擴展功能 (ReceivingExtension) - SYSR310-SYSR450 ==========
  
  // 查詢收款沖帳傳票列表
  getReceiptVoucherTransfer: (params) => {
    return axios.get('/api/v1/receipt/vouchertransfer', { params })
  },

  // 查詢單筆收款沖帳傳票
  getReceiptVoucherTransferById: (tKey) => {
    return axios.get(`/api/v1/receipt/vouchertransfer/${tKey}`)
  },

  // 新增收款沖帳傳票
  createReceiptVoucherTransfer: (data) => {
    return axios.post('/api/v1/receipt/vouchertransfer', data)
  },

  // 修改收款沖帳傳票
  updateReceiptVoucherTransfer: (tKey, data) => {
    return axios.put(`/api/v1/receipt/vouchertransfer/${tKey}`, data)
  },

  // 刪除收款沖帳傳票
  deleteReceiptVoucherTransfer: (tKey) => {
    return axios.delete(`/api/v1/receipt/vouchertransfer/${tKey}`)
  },

  // 拋轉收款沖帳傳票
  transferReceiptVoucher: (tKey) => {
    return axios.post(`/api/v1/receipt/vouchertransfer/${tKey}/transfer`)
  },

  // 批次拋轉收款沖帳傳票
  batchTransferReceiptVoucher: (data) => {
    return axios.post('/api/v1/receipt/vouchertransfer/batch/transfer', data)
  },

  // ========== 收款其他功能 (ReceivingOther) - SYSR510-SYSR570 ==========
  
  // 查詢保證金列表
  getDeposits: (params) => {
    return axios.get('/api/v1/receipt/deposits', { params })
  },

  // 查詢單筆保證金
  getDepositById: (tKey) => {
    return axios.get(`/api/v1/receipt/deposits/${tKey}`)
  },

  // 新增保證金
  createDeposit: (data) => {
    return axios.post('/api/v1/receipt/deposits', data)
  },

  // 修改保證金
  updateDeposit: (tKey, data) => {
    return axios.put(`/api/v1/receipt/deposits/${tKey}`, data)
  },

  // 刪除保證金
  deleteDeposit: (tKey) => {
    return axios.delete(`/api/v1/receipt/deposits/${tKey}`)
  },

  // 保證金退還
  returnDeposit: (tKey, data) => {
    return axios.post(`/api/v1/receipt/deposits/${tKey}/return`, data)
  },

  // 保證金存款
  depositAmount: (tKey, data) => {
    return axios.post(`/api/v1/receipt/deposits/${tKey}/deposit`, data)
  }
}

