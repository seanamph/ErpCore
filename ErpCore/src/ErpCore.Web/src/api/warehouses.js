import axios from './axios'

/**
 * SYSWB60 - 庫別資料維護作業 API
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const warehousesApi = {
  // 查詢庫別列表
  getWarehouses: (params) => {
    return axios.get('/warehouses', { params })
  },

  // 查詢單筆庫別
  getWarehouse: (warehouseId) => {
    return axios.get(`/warehouses/${warehouseId}`)
  },

  // 新增庫別
  createWarehouse: (data) => {
    return axios.post('/warehouses', data)
  },

  // 修改庫別
  updateWarehouse: (warehouseId, data) => {
    return axios.put(`/warehouses/${warehouseId}`, data)
  },

  // 刪除庫別
  deleteWarehouse: (warehouseId) => {
    return axios.delete(`/warehouses/${warehouseId}`)
  },

  // 批次刪除庫別
  deleteWarehousesBatch: (data) => {
    return axios.delete('/warehouses/batch', { data })
  },

  // 更新庫別狀態
  updateWarehouseStatus: (warehouseId, data) => {
    return axios.put(`/warehouses/${warehouseId}/status`, data)
  }
}
