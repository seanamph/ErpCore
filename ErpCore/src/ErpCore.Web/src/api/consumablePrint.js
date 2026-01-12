import axios from '@/utils/request'

/**
 * 耗材標籤列印作業 API (SYSA254)
 */

export const consumablePrintApi = {
  // 查詢耗材列表（用於列印）
  getPrintList: (params) => {
    return axios.get('/consumables/print/list', { params })
  },

  // 批次列印耗材標籤
  batchPrint: (data) => {
    return axios.post('/consumables/print/batch', data)
  },

  // 查詢列印記錄列表
  getPrintLogs: (params) => {
    return axios.get('/consumables/print/logs', { params })
  }
}
