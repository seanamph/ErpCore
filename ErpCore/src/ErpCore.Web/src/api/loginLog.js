import request from '@/utils/request'

/**
 * 使用者異常登入記錄 API (SYS0760)
 */
export const loginLogApi = {
  // 查詢異常登入記錄列表
  getLoginLogs: (params) => {
    return request({
      url: '/api/v1/system/login-log',
      method: 'get',
      params
    })
  },

  // 取得異常事件代碼選項
  getEventTypes: () => {
    return request({
      url: '/api/v1/system/login-log/event-types',
      method: 'get'
    })
  },

  // 刪除異常登入記錄
  deleteLoginLogs: (data) => {
    return request({
      url: '/api/v1/system/login-log',
      method: 'delete',
      data
    })
  },

  // 產生異常登入報表
  generateReport: (data, format = 'PDF') => {
    return request({
      url: `/api/v1/system/login-log/report?format=${format}`,
      method: 'post',
      data,
      responseType: 'blob'
    })
  }
}
