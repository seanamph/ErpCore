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
    return axios.get(`/parameters/${title}/${tag}`)
  },

  // 根據標題查詢參數列表
  getParametersByTitle: (title) => {
    return axios.get(`/parameters/by-title/${title}`)
  },

  // 新增參數
  createParameter: (data) => {
    return axios.post('/parameters', data)
  },

  // 修改參數
  updateParameter: (title, tag, data) => {
    return axios.put(`/parameters/${title}/${tag}`, data)
  },

  // 刪除參數
  deleteParameter: (title, tag) => {
    return axios.delete(`/parameters/${title}/${tag}`)
  },

  // 批次刪除參數
  deleteParametersBatch: (data) => {
    return axios.delete('/parameters/batch', { data })
  },

  // 取得參數值
  getParameterValue: (title, tag, lang) => {
    return axios.get(`/parameters/value/${title}/${tag}`, { params: { lang } })
  }
}

