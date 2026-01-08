import axios from './axios'

/**
 * 員工餐卡申請維護作業 API (SYSL130)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const employeeMealCardApi = {
  // 查詢員工餐卡申請列表
  getMealCards: (params) => {
    return axios.get('/business-reports/meal-cards', { params })
  },

  // 查詢單筆員工餐卡申請
  getMealCard: (tKey) => {
    return axios.get(`/business-reports/meal-cards/${tKey}`)
  },

  // 新增員工餐卡申請
  createMealCard: (data) => {
    return axios.post('/business-reports/meal-cards', data)
  },

  // 修改員工餐卡申請
  updateMealCard: (tKey, data) => {
    return axios.put(`/business-reports/meal-cards/${tKey}`, data)
  },

  // 刪除員工餐卡申請
  deleteMealCard: (tKey) => {
    return axios.delete(`/business-reports/meal-cards/${tKey}`)
  },

  // 批次審核員工餐卡申請
  batchVerify: (data) => {
    return axios.post('/business-reports/meal-cards/batch-verify', data)
  },

  // 取得下拉選單資料
  getDropdowns: () => {
    return axios.get('/business-reports/meal-cards/dropdowns')
  }
}

