import axios from './axios'

/**
 * SYSBC20 - 銀行基本資料維護作業 API
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const banksApi = {
  // 查詢銀行列表
  getBanks: (params) => {
    return axios.get('/banks', { params })
  },

  // 查詢單筆銀行
  getBank: (bankId) => {
    return axios.get(`/banks/${bankId}`)
  },

  // 新增銀行
  createBank: (data) => {
    return axios.post('/banks', data)
  },

  // 修改銀行
  updateBank: (bankId, data) => {
    return axios.put(`/banks/${bankId}`, data)
  },

  // 刪除銀行
  deleteBank: (bankId) => {
    return axios.delete(`/banks/${bankId}`)
  },

  // 批次刪除銀行
  deleteBanksBatch: (data) => {
    return axios.delete('/banks/batch', { data })
  }
}

