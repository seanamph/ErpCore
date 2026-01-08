import axios from './axios';

/**
 * 庫存調整作業 API (SYSW490)
 */

/**
 * 庫存調整作業 API (SYSW490)
 * 遵循 C# API 欄位命名 (PascalCase)
 */

/**
 * 查詢調整單列表
 */
export const getInventoryAdjustments = (params) => {
  return axios.get('/inventory-adjustments', { params });
};

/**
 * 查詢單筆調整單
 */
export const getInventoryAdjustment = (adjustmentId) => {
  return axios.get(`/inventory-adjustments/${adjustmentId}`);
};

/**
 * 新增調整單
 */
export const createInventoryAdjustment = (data) => {
  return axios.post('/inventory-adjustments', data);
};

/**
 * 修改調整單
 */
export const updateInventoryAdjustment = (adjustmentId, data) => {
  return axios.put(`/inventory-adjustments/${adjustmentId}`, data);
};

/**
 * 刪除調整單
 */
export const deleteInventoryAdjustment = (adjustmentId) => {
  return axios.delete(`/inventory-adjustments/${adjustmentId}`);
};

/**
 * 確認調整單
 */
export const confirmAdjustment = (adjustmentId) => {
  return axios.post(`/inventory-adjustments/${adjustmentId}/confirm`);
};

/**
 * 取消調整單
 */
export const cancelAdjustment = (adjustmentId) => {
  return axios.post(`/inventory-adjustments/${adjustmentId}/cancel`);
};

/**
 * 取得調整原因列表
 */
export const getAdjustmentReasons = () => {
  return axios.get('/inventory-adjustments/reasons');
};

