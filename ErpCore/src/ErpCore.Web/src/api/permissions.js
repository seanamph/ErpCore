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
  // 查詢角色欄位權限列表
  getRoleFieldPermissions: (roleId, params) => {
    return axios.get(`/api/v1/roles/${roleId}/field-permissions`, { params })
  },

  // 新增角色欄位權限
  createRoleFieldPermissions: (roleId, data) => {
    return axios.post(`/api/v1/roles/${roleId}/field-permissions`, data)
  },

  // 更新角色欄位權限
  updateRoleFieldPermissions: (roleId, data) => {
    return axios.put(`/api/v1/roles/${roleId}/field-permissions`, data)
  },

  // 刪除角色欄位權限
  deleteRoleFieldPermission: (roleId, tKey) => {
    return axios.delete(`/api/v1/roles/${roleId}/field-permissions/${tKey}`)
  }
}

/**
 * 使用者欄位權限管理 API (SYS0340)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const userFieldPermissionsApi = {
  // 查詢使用者欄位權限列表
  getUserFieldPermissions: (userId, params) => {
    return axios.get(`/api/v1/users/${userId}/field-permissions`, { params })
  },

  // 新增使用者欄位權限
  createUserFieldPermissions: (userId, data) => {
    return axios.post(`/api/v1/users/${userId}/field-permissions`, data)
  },

  // 更新使用者欄位權限
  updateUserFieldPermissions: (userId, data) => {
    return axios.put(`/api/v1/users/${userId}/field-permissions`, data)
  },

  // 刪除使用者欄位權限
  deleteUserFieldPermission: (userId, tKey) => {
    return axios.delete(`/api/v1/users/${userId}/field-permissions/${tKey}`)
  }
}

