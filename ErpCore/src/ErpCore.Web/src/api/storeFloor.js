import axios from './axios'

/**
 * 商店樓層管理模組 API (StoreManagement, StoreQuery, Floor, FloorQuery, TypeCode, TypeCodeQuery, PosData, PosQuery)
 * 遵循 C# API 欄位命名 (PascalCase)
 */

// 商店資料維護 API (SYS6110-SYS6140)
export const storeManagementApi = {
  // 查詢商店列表
  getShopFloors: (params) => {
    return axios.get('/shop-floors', { params })
  },

  // 查詢單筆商店
  getShopFloor: (shopId) => {
    return axios.get(`/shop-floors/${shopId}`)
  },

  // 新增商店
  createShopFloor: (data) => {
    return axios.post('/shop-floors', data)
  },

  // 修改商店
  updateShopFloor: (shopId, data) => {
    return axios.put(`/shop-floors/${shopId}`, data)
  },

  // 刪除商店
  deleteShopFloor: (shopId) => {
    return axios.delete(`/shop-floors/${shopId}`)
  },

  // 檢查商店編號是否存在
  checkShopFloorExists: (shopId) => {
    return axios.get(`/shop-floors/check/${shopId}`)
  },

  // 更新商店狀態
  updateShopFloorStatus: (shopId, status) => {
    return axios.put(`/shop-floors/${shopId}/status`, { Status: status })
  }
}

// 商店查詢作業 API (SYS6210-SYS6270)
export const storeQueryApi = {
  // 進階查詢商店
  queryShopFloors: (data) => {
    return axios.post('/shop-floors/query', data)
  },

  // 匯出商店查詢結果
  exportShopFloors: (data) => {
    return axios.post('/shop-floors/export', data, { responseType: 'blob' })
  }
}

// 樓層資料維護 API (SYS6310-SYS6370)
export const floorApi = {
  // 查詢樓層列表
  getFloors: (params) => {
    return axios.get('/floors', { params })
  },

  // 查詢單筆樓層
  getFloor: (floorId) => {
    return axios.get(`/floors/${floorId}`)
  },

  // 新增樓層
  createFloor: (data) => {
    return axios.post('/floors', data)
  },

  // 修改樓層
  updateFloor: (floorId, data) => {
    return axios.put(`/floors/${floorId}`, data)
  },

  // 刪除樓層
  deleteFloor: (floorId) => {
    return axios.delete(`/floors/${floorId}`)
  },

  // 檢查樓層代碼是否存在
  checkFloorExists: (floorId) => {
    return axios.get(`/floors/check/${floorId}`)
  },

  // 更新樓層狀態
  updateFloorStatus: (floorId, status) => {
    return axios.put(`/floors/${floorId}/status`, { Status: status })
  }
}

// 樓層查詢作業 API (SYS6381-SYS63A0)
export const floorQueryApi = {
  // 進階查詢樓層
  queryFloors: (data) => {
    return axios.post('/floors/query', data)
  },

  // 查詢樓層統計資訊
  getFloorStatistics: (params) => {
    return axios.get('/floors/query/statistics', { params })
  },

  // 匯出樓層查詢結果
  exportFloors: (data) => {
    return axios.post('/floors/query/export', data, { responseType: 'blob' })
  }
}

// 類型代碼維護 API (SYS6405-SYS6490)
export const typeCodeApi = {
  // 查詢類型代碼列表
  getTypeCodes: (params) => {
    return axios.get('/type-codes', { params })
  },

  // 查詢單筆類型代碼
  getTypeCode: (tKey) => {
    return axios.get(`/type-codes/${tKey}`)
  },

  // 新增類型代碼
  createTypeCode: (data) => {
    return axios.post('/type-codes', data)
  },

  // 修改類型代碼
  updateTypeCode: (tKey, data) => {
    return axios.put(`/type-codes/${tKey}`, data)
  },

  // 刪除類型代碼
  deleteTypeCode: (tKey) => {
    return axios.delete(`/type-codes/${tKey}`)
  },

  // 批次刪除類型代碼
  batchDeleteTypeCodes: (tKeys) => {
    return axios.delete('/type-codes/batch', { data: tKeys })
  },

  // 檢查類型代碼是否存在
  checkTypeCodeExists: (typeCode, category) => {
    return axios.get('/type-codes/check', { params: { typeCode, category } })
  }
}

// 類型代碼查詢作業 API (SYS6501-SYS6560)
export const typeCodeQueryApi = {
  // 進階查詢類型代碼
  queryTypeCodes: (data) => {
    return axios.post('/type-codes/query', data)
  },

  // 查詢類型代碼統計資訊
  getTypeCodeStatistics: (params) => {
    return axios.get('/type-codes/query/statistics', { params })
  }
}

// POS資料維護 API (SYS6610-SYS6999)
export const posDataApi = {
  // 查詢POS終端列表
  getPosTerminals: (params) => {
    return axios.get('/pos-terminals', { params })
  },

  // 查詢單筆POS終端
  getPosTerminal: (posTerminalId) => {
    return axios.get(`/pos-terminals/${posTerminalId}`)
  },

  // 新增POS終端
  createPosTerminal: (data) => {
    return axios.post('/pos-terminals', data)
  },

  // 修改POS終端
  updatePosTerminal: (posTerminalId, data) => {
    return axios.put(`/pos-terminals/${posTerminalId}`, data)
  },

  // 刪除POS終端
  deletePosTerminal: (posTerminalId) => {
    return axios.delete(`/pos-terminals/${posTerminalId}`)
  },

  // POS資料同步
  syncPosTerminal: (posTerminalId) => {
    return axios.post(`/pos-terminals/${posTerminalId}/sync`)
  }
}

// POS查詢作業 API (SYS6A04-SYS6A19)
export const posQueryApi = {
  // 進階查詢POS終端
  queryPosTerminals: (data) => {
    return axios.post('/pos-terminals/query', data)
  },

  // 查詢POS終端統計資訊
  getPosTerminalStatistics: (params) => {
    return axios.get('/pos-terminals/query/statistics', { params })
  }
}

