import axios from './axios'

/**
 * 角色權限管理 API (SYS0310)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const rolePermissionsApi = {
  // 查詢角色權限列表
  getRolePermissions: (roleId, params) => {
    return axios.get(`/api/v1/roles/${roleId}/permissions`, { params })
  },

  // 查詢角色系統權限統計
  getSystemStats: (roleId) => {
    return axios.get(`/api/v1/roles/${roleId}/permissions/systems/stats`)
  },

  // 新增角色權限
  createRolePermissions: (roleId, data) => {
    return axios.post(`/api/v1/roles/${roleId}/permissions`, data)
  },

  // 批量設定角色系統權限
  batchSetSystemPermissions: (roleId, data) => {
    return axios.post(`/api/v1/roles/${roleId}/permissions/systems`, data)
  },

  // 批量設定角色選單權限
  batchSetMenuPermissions: (roleId, data) => {
    return axios.post(`/api/v1/roles/${roleId}/permissions/menus`, data)
  },

  // 批量設定角色作業權限
  batchSetProgramPermissions: (roleId, data) => {
    return axios.post(`/api/v1/roles/${roleId}/permissions/programs`, data)
  },

  // 批量設定角色按鈕權限
  batchSetButtonPermissions: (roleId, data) => {
    return axios.post(`/api/v1/roles/${roleId}/permissions/buttons`, data)
  },

  // 刪除角色權限
  deleteRolePermission: (roleId, tKey) => {
    return axios.delete(`/api/v1/roles/${roleId}/permissions/${tKey}`)
  },

  // 批量刪除角色權限
  batchDeleteRolePermissions: (roleId, data) => {
    return axios.delete(`/api/v1/roles/${roleId}/permissions/batch`, { data })
  }
}

/**
 * 使用者權限管理 API (SYS0320)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const userPermissionsApi = {
  // 查詢使用者權限列表
  getUserPermissions: (userId, params) => {
    return axios.get(`/api/v1/users/${userId}/permissions`, { params })
  },

  // 查詢使用者系統權限統計
  getSystemStats: (userId) => {
    return axios.get(`/api/v1/users/${userId}/permissions/systems/stats`)
  },

  // 新增使用者權限
  createUserPermissions: (userId, data) => {
    return axios.post(`/api/v1/users/${userId}/permissions`, data)
  },

  // 批量設定使用者系統權限
  batchSetSystemPermissions: (userId, data) => {
    return axios.post(`/api/v1/users/${userId}/permissions/systems`, data)
  },

  // 批量設定使用者選單權限
  batchSetMenuPermissions: (userId, data) => {
    return axios.post(`/api/v1/users/${userId}/permissions/menus`, data)
  },

  // 批量設定使用者作業權限
  batchSetProgramPermissions: (userId, data) => {
    return axios.post(`/api/v1/users/${userId}/permissions/programs`, data)
  },

  // 批量設定使用者按鈕權限
  batchSetButtonPermissions: (userId, data) => {
    return axios.post(`/api/v1/users/${userId}/permissions/buttons`, data)
  },

  // 刪除使用者權限
  deleteUserPermission: (userId, tKey) => {
    return axios.delete(`/api/v1/users/${userId}/permissions/${tKey}`)
  },

  // 批量刪除使用者權限
  batchDeleteUserPermissions: (userId, data) => {
    return axios.delete(`/api/v1/users/${userId}/permissions/batch`, { data })
  }
}

/**
 * 角色欄位權限管理 API (SYS0330)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const roleFieldPermissionsApi = {
  // 查詢資料庫列表
  getDatabases: () => {
    return axios.get('/api/v1/role-field-permissions/databases')
  },

  // 查詢表格列表
  getTables: (dbName) => {
    return axios.get('/api/v1/role-field-permissions/tables', { params: { dbName } })
  },

  // 查詢欄位列表
  getFields: (dbName, tableName) => {
    return axios.get('/api/v1/role-field-permissions/fields', { params: { dbName, tableName } })
  },

  // 查詢角色欄位權限列表
  getRoleFieldPermissions: (params) => {
    return axios.get('/api/v1/role-field-permissions', { params })
  },

  // 新增角色欄位權限
  createRoleFieldPermission: (data) => {
    return axios.post('/api/v1/role-field-permissions', data)
  },

  // 更新角色欄位權限
  updateRoleFieldPermission: (id, data) => {
    return axios.put(`/api/v1/role-field-permissions/${id}`, data)
  },

  // 刪除角色欄位權限
  deleteRoleFieldPermission: (id) => {
    return axios.delete(`/api/v1/role-field-permissions/${id}`)
  },

  // 批次設定角色欄位權限
  batchSetPermissions: (data) => {
    return axios.post('/api/v1/role-field-permissions/batch', data)
  }
}

/**
 * 使用者欄位權限管理 API (SYS0340)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const userFieldPermissionsApi = {
  // 查詢資料庫列表
  getDatabases: () => {
    return axios.get('/api/v1/user-field-permissions/databases')
  },

  // 查詢表格列表
  getTables: (dbName) => {
    return axios.get('/api/v1/user-field-permissions/tables', { params: { dbName } })
  },

  // 查詢欄位列表
  getFields: (dbName, tableName) => {
    return axios.get('/api/v1/user-field-permissions/fields', { params: { dbName, tableName } })
  },

  // 查詢使用者欄位權限列表
  getUserFieldPermissions: (params) => {
    return axios.get('/api/v1/user-field-permissions', { params })
  },

  // 新增使用者欄位權限
  createUserFieldPermission: (data) => {
    return axios.post('/api/v1/user-field-permissions', data)
  },

  // 更新使用者欄位權限
  updateUserFieldPermission: (id, data) => {
    return axios.put(`/api/v1/user-field-permissions/${id}`, data)
  },

  // 刪除使用者欄位權限
  deleteUserFieldPermission: (id) => {
    return axios.delete(`/api/v1/user-field-permissions/${id}`)
  },

  // 批次設定使用者欄位權限
  batchSetPermissions: (data) => {
    return axios.post('/api/v1/user-field-permissions/batch', data)
  }
}

/**
 * 項目對應管理 API (SYS0360)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const itemCorrespondsApi = {
  // 查詢項目對應列表
  getItemCorresponds: (params) => {
    return axios.get('/api/v1/item-corresponds', { params })
  },

  // 查詢單筆項目對應
  getItemCorrespond: (itemId) => {
    return axios.get(`/api/v1/item-corresponds/${itemId}`)
  },

  // 新增項目對應
  createItemCorrespond: (data) => {
    return axios.post('/api/v1/item-corresponds', data)
  },

  // 修改項目對應
  updateItemCorrespond: (itemId, data) => {
    return axios.put(`/api/v1/item-corresponds/${itemId}`, data)
  },

  // 刪除項目對應
  deleteItemCorrespond: (itemId) => {
    return axios.delete(`/api/v1/item-corresponds/${itemId}`)
  }
}

/**
 * 項目權限管理 API (SYS0360)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const itemPermissionsApi = {
  // 查詢項目權限列表
  getItemPermissions: (itemId, params) => {
    return axios.get(`/api/v1/item-corresponds/${itemId}/permissions`, { params })
  },

  // 查詢項目系統列表
  getSystemList: (itemId) => {
    return axios.get(`/api/v1/item-corresponds/${itemId}/permissions/systems`)
  },

  // 查詢項目選單列表
  getMenuList: (itemId, systemId) => {
    return axios.get(`/api/v1/item-corresponds/${itemId}/permissions/systems/${systemId}/menus`)
  },

  // 查詢項目作業列表
  getProgramList: (itemId, menuId) => {
    return axios.get(`/api/v1/item-corresponds/${itemId}/permissions/menus/${menuId}/programs`)
  },

  // 查詢項目按鈕列表
  getButtonList: (itemId, programId) => {
    return axios.get(`/api/v1/item-corresponds/${itemId}/permissions/programs/${programId}/buttons`)
  },

  // 設定項目系統權限
  setSystemPermissions: (itemId, data) => {
    return axios.post(`/api/v1/item-corresponds/${itemId}/permissions/systems/permissions`, data)
  },

  // 設定項目選單權限
  setMenuPermissions: (itemId, data) => {
    return axios.post(`/api/v1/item-corresponds/${itemId}/permissions/menus/permissions`, data)
  },

  // 設定項目作業權限
  setProgramPermissions: (itemId, data) => {
    return axios.post(`/api/v1/item-corresponds/${itemId}/permissions/programs/permissions`, data)
  },

  // 設定項目按鈕權限
  setButtonPermissions: (itemId, data) => {
    return axios.post(`/api/v1/item-corresponds/${itemId}/permissions/buttons/permissions`, data)
  },

  // 刪除項目權限
  deleteItemPermission: (itemId, tKey) => {
    return axios.delete(`/api/v1/item-corresponds/${itemId}/permissions/${tKey}`)
  },

  // 批量刪除項目權限
  batchDeleteItemPermissions: (itemId, data) => {
    return axios.delete(`/api/v1/item-corresponds/${itemId}/permissions/batch`, { data })
  }
}

