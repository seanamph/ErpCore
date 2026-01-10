import axios from './axios'

/**
 * 銷售管理 API
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const salesApi = {
  // ========== 銷售資料維護 (SalesData) - SYSD110-SYSD140 ==========
  
  // 查詢銷售單列表
  getSalesOrders: (params) => {
    return axios.get('/api/v1/sales-orders', { params })
  },

  // 查詢單筆銷售單
  getSalesOrder: (orderId) => {
    return axios.get(`/api/v1/sales-orders/${orderId}`)
  },

  // 新增銷售單
  createSalesOrder: (data) => {
    return axios.post('/api/v1/sales-orders', data)
  },

  // 修改銷售單
  updateSalesOrder: (orderId, data) => {
    return axios.put(`/api/v1/sales-orders/${orderId}`, data)
  },

  // 刪除銷售單
  deleteSalesOrder: (orderId) => {
    return axios.delete(`/api/v1/sales-orders/${orderId}`)
  },

  // ========== 銷售處理作業 (SalesProcess) - SYSD210-SYSD230 ==========
  
  // 審核銷售單
  approveSalesOrder: (orderId, data) => {
    return axios.post(`/api/v1/sales-orders/${orderId}/approve`, data)
  },

  // 出貨銷售單
  shipSalesOrder: (orderId, data) => {
    return axios.post(`/api/v1/sales-orders/${orderId}/ship`, data)
  },

  // 取消銷售單
  cancelSalesOrder: (orderId, data) => {
    return axios.post(`/api/v1/sales-orders/${orderId}/cancel`, data)
  },

  // ========== 銷售報表查詢 (SalesReport) - SYSD310-SYSD430 ==========
  
  // 銷售明細報表查詢
  getSalesDetailReport: (data) => {
    return axios.post('/api/v1/sales-orders/reports/detail', data)
  },

  // 銷售統計報表查詢
  getSalesStatisticsReport: (data) => {
    return axios.post('/api/v1/sales-orders/reports/statistics', data)
  },

  // 銷售客戶報表查詢
  getSalesCustomerReport: (data) => {
    return axios.post('/api/v1/sales-orders/reports/customer', data)
  },

  // 銷售商品報表查詢
  getSalesProductReport: (data) => {
    return axios.post('/api/v1/sales-orders/reports/product', data)
  }
}

