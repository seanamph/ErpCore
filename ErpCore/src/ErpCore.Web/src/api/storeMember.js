import axios from './axios'

/**
 * 商店與會員管理模組 API (Store, StoreQuery, Member, MemberQuery, Promotion, StoreReport)
 * 遵循 C# API 欄位命名 (PascalCase)
 */

// 商店資料維護 API
export const storeApi = {
  // 查詢商店列表
  getStores: (params) => {
    return axios.get('/shops', { params })
  },

  // 查詢單筆商店
  getStore: (shopId) => {
    return axios.get(`/shops/${shopId}`)
  },

  // 新增商店
  createStore: (data) => {
    return axios.post('/shops', data)
  },

  // 修改商店
  updateStore: (shopId, data) => {
    return axios.put(`/shops/${shopId}`, data)
  },

  // 刪除商店
  deleteStore: (shopId) => {
    return axios.delete(`/shops/${shopId}`)
  },

  // 檢查商店編號是否存在
  checkStoreExists: (shopId) => {
    return axios.get(`/shops/check/${shopId}`)
  }
}

// 商店查詢作業 API
export const storeQueryApi = {
  // 進階查詢商店
  queryStores: (data) => {
    return axios.post('/shops/query', data)
  },

  // 匯出商店查詢結果
  exportStores: (data) => {
    return axios.post('/shops/query/export', data, { responseType: 'blob' })
  },

  // 列印商店報表
  printStoreReport: (data) => {
    return axios.post('/shops/query/print', data, { responseType: 'blob' })
  }
}

// 會員資料維護 API
export const memberApi = {
  // 查詢會員列表
  getMembers: (params) => {
    return axios.get('/members', { params })
  },

  // 查詢單筆會員
  getMember: (memberId) => {
    return axios.get(`/members/${memberId}`)
  },

  // 新增會員
  createMember: (data) => {
    return axios.post('/members', data)
  },

  // 修改會員
  updateMember: (memberId, data) => {
    return axios.put(`/members/${memberId}`, data)
  },

  // 刪除會員
  deleteMember: (memberId) => {
    return axios.delete(`/members/${memberId}`)
  },

  // 更新會員狀態
  updateMemberStatus: (memberId, status) => {
    return axios.put(`/members/${memberId}/status`, { Status: status })
  },

  // 查詢會員積分記錄
  getMemberPoints: (memberId, params) => {
    return axios.get(`/members/${memberId}/points`, { params })
  }
}

// 會員查詢作業 API
export const memberQueryApi = {
  // 進階查詢會員
  queryMembers: (data) => {
    return axios.post('/members/query', data)
  },

  // 查詢會員交易記錄
  getMemberTransactions: (memberId, params) => {
    return axios.get(`/members/query/${memberId}/transactions`, { params })
  },

  // 查詢會員回門禮卡號補登記錄
  getMemberExchangeLogs: (params) => {
    return axios.get('/members/query/exchange-logs', { params })
  },

  // 匯出會員查詢結果
  exportMembers: (data) => {
    return axios.post('/members/query/export', data, { responseType: 'blob' })
  },

  // 列印會員報表
  printMemberReport: (data) => {
    return axios.post('/members/query/print', data, { responseType: 'blob' })
  }
}

// 促銷活動維護 API
export const promotionApi = {
  // 查詢促銷活動列表
  getPromotions: (params) => {
    return axios.get('/promotions', { params })
  },

  // 查詢單筆促銷活動
  getPromotion: (promotionId) => {
    return axios.get(`/promotions/${promotionId}`)
  },

  // 新增促銷活動
  createPromotion: (data) => {
    return axios.post('/promotions', data)
  },

  // 修改促銷活動
  updatePromotion: (promotionId, data) => {
    return axios.put(`/promotions/${promotionId}`, data)
  },

  // 刪除促銷活動
  deletePromotion: (promotionId) => {
    return axios.delete(`/promotions/${promotionId}`)
  },

  // 啟用/停用促銷活動
  updatePromotionStatus: (promotionId, status) => {
    return axios.put(`/promotions/${promotionId}/status`, { Status: status })
  }
}

// 商店報表查詢作業 API
export const storeReportApi = {
  // 查詢商店報表
  getStoreReports: (params) => {
    return axios.get('/shops/reports', { params })
  },

  // 匯出商店報表
  exportStoreReport: (data) => {
    return axios.post('/shops/reports/export', data, { responseType: 'blob' })
  },

  // 列印商店報表
  printStoreReport: (data) => {
    return axios.post('/shops/reports/print', data, { responseType: 'blob' })
  }
}

