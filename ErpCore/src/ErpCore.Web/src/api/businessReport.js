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

/**
 * 業務報表查詢作業 API (SYSL135)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const businessReportApi = {
  // 查詢業務報表列表
  getBusinessReports: (params) => {
    return axios.get('/business-reports/sysl135', { params })
  },

  // 匯出業務報表
  exportBusinessReports: (data, format = 'excel') => {
    return axios.post(`/business-reports/sysl135/export?format=${format}`, data, {
      responseType: 'blob'
    })
  },

  // 列印業務報表
  printBusinessReports: (data) => {
    return axios.post('/business-reports/sysl135/print', data, {
      responseType: 'blob'
    })
  }
}

/**
 * 業務報表管理作業 API (SYSL145)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const businessReportManagementApi = {
  // 查詢業務報表管理列表
  getBusinessReportManagements: (params) => {
    return axios.get('/business-report-management', { params })
  },

  // 查詢單筆業務報表管理
  getBusinessReportManagement: (tKey) => {
    return axios.get(`/business-report-management/${tKey}`)
  },

  // 新增業務報表管理
  createBusinessReportManagement: (data) => {
    return axios.post('/business-report-management', data)
  },

  // 修改業務報表管理
  updateBusinessReportManagement: (tKey, data) => {
    return axios.put(`/business-report-management/${tKey}`, data)
  },

  // 刪除業務報表管理
  deleteBusinessReportManagement: (tKey) => {
    return axios.delete(`/business-report-management/${tKey}`)
  },

  // 批次刪除業務報表管理
  batchDeleteBusinessReportManagement: (data) => {
    return axios.delete('/business-report-management/batch', { data })
  },

  // 載入管理權限資料
  loadManagementData: () => {
    return axios.get('/business-report-management/load-management')
  },

  // 檢查重複資料
  checkDuplicate: (data) => {
    return axios.post('/business-report-management/check-duplicate', data)
  }
}

/**
 * 業務報表列印作業 API (SYSL150)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const businessReportPrintApi = {
  // 查詢業務報表列印列表
  getBusinessReportPrints: (params) => {
    return axios.get('/business-report-print', { params })
  },

  // 查詢單筆業務報表列印
  getBusinessReportPrint: (tKey) => {
    return axios.get(`/business-report-print/${tKey}`)
  },

  // 新增業務報表列印
  createBusinessReportPrint: (data) => {
    return axios.post('/business-report-print', data)
  },

  // 修改業務報表列印
  updateBusinessReportPrint: (tKey, data) => {
    return axios.put(`/business-report-print/${tKey}`, data)
  },

  // 刪除業務報表列印
  deleteBusinessReportPrint: (tKey) => {
    return axios.delete(`/business-report-print/${tKey}`)
  },

  // 批次刪除業務報表列印
  batchDeleteBusinessReportPrint: (data) => {
    return axios.delete('/business-report-print/batch', { data })
  },

  // 批次審核
  batchAudit: (data) => {
    return axios.post('/business-report-print/batch-audit', data)
  },

  // 複製下一年度資料
  copyNextYear: (data) => {
    return axios.post('/business-report-print/copy-next-year', data)
  },

  // 計算數量
  calculateQty: (data) => {
    return axios.post('/business-report-print/calculate-qty', data)
  },

  // 檢查年度是否已審核
  checkYearAudited: (giveYear, siteId) => {
    return axios.get('/business-report-print/check-year-audited', {
      params: { giveYear, siteId }
    })
  }
}

/**
 * 業務報表列印明細作業 API (SYSL160)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const businessReportPrintDetailApi = {
  // 查詢業務報表列印明細列表
  getBusinessReportPrintDetails: (params) => {
    return axios.get('/business-report-print-details', { params })
  },

  // 查詢單筆業務報表列印明細
  getBusinessReportPrintDetail: (tKey) => {
    return axios.get(`/business-report-print-details/${tKey}`)
  },

  // 根據 PrintId 查詢明細列表
  getBusinessReportPrintDetailsByPrintId: (printId) => {
    return axios.get(`/business-report-print-details/print/${printId}`)
  },

  // 新增業務報表列印明細
  createBusinessReportPrintDetail: (data) => {
    return axios.post('/business-report-print-details', data)
  },

  // 修改業務報表列印明細
  updateBusinessReportPrintDetail: (tKey, data) => {
    return axios.put(`/business-report-print-details/${tKey}`, data)
  },

  // 刪除業務報表列印明細
  deleteBusinessReportPrintDetail: (tKey) => {
    return axios.delete(`/business-report-print-details/${tKey}`)
  },

  // 批次處理業務報表列印明細
  batchProcess: (data) => {
    return axios.post('/business-report-print-details/batch', data)
  }
}

