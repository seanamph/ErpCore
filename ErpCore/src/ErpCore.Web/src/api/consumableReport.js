import axios from '@/utils/request'

/**
 * 耗材管理報表 API (SYSA255)
 */

export const consumableReportApi = {
  // 查詢耗材管理報表
  getReport: (params) => {
    return axios.get('/consumables/report', { params })
  },

  // 匯出耗材管理報表
  exportReport: (data) => {
    return axios.post('/consumables/report/export', data, {
      responseType: 'blob'
    })
  },

  // 查詢耗材使用明細
  getTransactions: (consumableId, params) => {
    return axios.get(`/consumables/report/${consumableId}/transactions`, { params })
  }
}
