import axios from '@/utils/request'

/**
 * 耗材出售單 API (SYSA297)
 */

export const consumableSalesApi = {
  // 查詢耗材出售單列表
  getSales: (params) => {
    return axios.get('/api/v1/consumable-sales', { params })
  },

  // 查詢單筆耗材出售單
  getSalesDetail: (txnNo) => {
    return axios.get(`/api/v1/consumable-sales/${txnNo}`)
  },

  // 新增耗材出售單
  createSales: (data) => {
    return axios.post('/api/v1/consumable-sales', data)
  },

  // 修改耗材出售單
  updateSales: (txnNo, data) => {
    return axios.put(`/api/v1/consumable-sales/${txnNo}`, data)
  },

  // 刪除耗材出售單
  deleteSales: (txnNo) => {
    return axios.delete(`/api/v1/consumable-sales/${txnNo}`)
  },

  // 審核耗材出售單
  approveSales: (txnNo, data) => {
    return axios.post(`/api/v1/consumable-sales/${txnNo}/approve`, data)
  },

  // 取消耗材出售單
  cancelSales: (txnNo) => {
    return axios.post(`/api/v1/consumable-sales/${txnNo}/cancel`)
  }
}
