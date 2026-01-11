import axios from './axios'

/**
 * SYSBC40 - 參數資料設定維護作業 API
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const parametersApi = {
  // 查詢參數列表
  getParameters: (params) => {
    return axios.get('/parameters', { params })
  },

  // 查詢單筆參數
  getParameter: (title, tag) => {
    return axios.get(`/parameters/${encodeURIComponent(title)}/${encodeURIComponent(tag)}`)
  },

  // 根據參數標題查詢參數列表
  getParametersByTitle: (title) => {
    return axios.get(`/parameters/by-title/${encodeURIComponent(title)}`)
  },

  // 新增參數
  createParameter: (data) => {
    return axios.post('/parameters', data)
  },

  // 修改參數
  updateParameter: (title, tag, data) => {
    return axios.put(`/parameters/${encodeURIComponent(title)}/${encodeURIComponent(tag)}`, data)
  },

  // 刪除參數
  deleteParameter: (title, tag) => {
    return axios.delete(`/parameters/${encodeURIComponent(title)}/${encodeURIComponent(tag)}`)
  },

  // 批次刪除參數
  deleteParametersBatch: (data) => {
    return axios.delete('/parameters/batch', { data })
  },

  // 取得參數值
  getParameterValue: (title, tag, lang) => {
    return axios.get(`/parameters/value/${encodeURIComponent(title)}/${encodeURIComponent(tag)}`, {
      params: { lang }
    })
  }
}
