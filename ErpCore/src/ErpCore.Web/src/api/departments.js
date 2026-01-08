import axios from './axios'

/**
 * SYSWB40 - 部別資料維護作業 API
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const departmentsApi = {
  // 查詢部別列表
  getDepartments: (params) => {
    return axios.get('/departments', { params })
  },

  // 查詢單筆部別
  getDepartment: (deptId) => {
    return axios.get(`/departments/${deptId}`)
  },

  // 新增部別
  createDepartment: (data) => {
    return axios.post('/departments', data)
  },

  // 修改部別
  updateDepartment: (deptId, data) => {
    return axios.put(`/departments/${deptId}`, data)
  },

  // 刪除部別
  deleteDepartment: (deptId) => {
    return axios.delete(`/departments/${deptId}`)
  },

  // 批次刪除部別
  deleteDepartmentsBatch: (data) => {
    return axios.delete('/departments/batch', { data })
  },

  // 更新部別狀態
  updateDepartmentStatus: (deptId, data) => {
    return axios.put(`/departments/${deptId}/status`, data)
  }
}

