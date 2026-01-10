import axios from './axios'

/**
 * UI組件 API
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const uiComponentApi = {
  // ========== 資料維護UI組件 (IMS30系列) ==========
  // 查詢UI組件列表
  getComponents: (params) => {
    return axios.get('/ui-components/components', { params })
  },

  // 查詢單筆UI組件
  getComponent: (componentId) => {
    return axios.get(`/ui-components/components/${componentId}`)
  },

  // 根據組件代碼和版本查詢
  getComponentByCode: (componentCode, version) => {
    return axios.get(`/ui-components/components/${componentCode}/${version}`)
  },

  // 執行資料維護操作（瀏覽、新增、修改、刪除、查詢）
  executeDataMaintenance: (componentCode, operation, data) => {
    return axios.post(`/ui-components/components/${componentCode}/execute/${operation}`, data)
  },

  // 查詢資料（FQ）
  queryData: (componentCode, params) => {
    return axios.get(`/ui-components/components/${componentCode}/query`, { params })
  },

  // 新增資料（FI）
  insertData: (componentCode, data) => {
    return axios.post(`/ui-components/components/${componentCode}/insert`, data)
  },

  // 修改資料（FU）
  updateData: (componentCode, data) => {
    return axios.put(`/ui-components/components/${componentCode}/update`, data)
  },

  // 刪除資料（FS）
  deleteData: (componentCode, data) => {
    return axios.delete(`/ui-components/components/${componentCode}/delete`, { data })
  },

  // 列印資料（PR）
  printData: (componentCode, data) => {
    return axios.post(`/ui-components/components/${componentCode}/print`, data)
  },

  // ========== UI組件查詢與報表 ==========
  // 查詢UI組件使用記錄
  getComponentUsages: (params) => {
    return axios.get('/ui-components/usages', { params })
  },

  // 查詢UI組件報表
  getComponentReport: (params) => {
    return axios.get('/ui-components/report', { params })
  }
}

