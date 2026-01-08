import axios from './axios'

/**
 * 系統權限列表 API (SYS0710)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const systemPermissionApi = {
  // 查詢系統權限列表
  getList: (params) => {
    return axios.get('/system-permissions/list', { params })
  },

  // 匯出系統權限報表
  exportReport: (data) => {
    return axios.post('/system-permissions/export', data, {
      responseType: 'blob'
    })
  }
}

/**
 * 作業權限之使用者列表 API (SYS0720)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const programUserPermissionApi = {
  // 查詢作業權限之使用者列表
  getList: (params) => {
    return axios.get('/program-user-permissions/list', { params })
  },

  // 匯出作業權限之使用者報表
  exportReport: (data) => {
    return axios.post('/program-user-permissions/export', data, {
      responseType: 'blob'
    })
  }
}

/**
 * 角色系統權限列表 API (SYS0731)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const roleSystemPermissionApi = {
  // 查詢角色系統權限列表
  getList: (params) => {
    return axios.get('/role-system-permissions/list', { params })
  },

  // 匯出角色系統權限報表
  exportReport: (data) => {
    return axios.post('/role-system-permissions/export', data, {
      responseType: 'blob'
    })
  }
}

/**
 * 作業權限之角色列表 API (SYS0740)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const programRolePermissionApi = {
  // 查詢作業權限之角色列表
  getList: (params) => {
    return axios.get('/program-role-permissions/list', { params })
  },

  // 匯出作業權限之角色報表
  exportReport: (data) => {
    return axios.post('/program-role-permissions/export', data, {
      responseType: 'blob'
    })
  }
}

/**
 * 角色之使用者列表 API (SYS0750)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const roleUserApi = {
  // 查詢角色之使用者列表
  getList: (params) => {
    return axios.get('/role-users/list', { params })
  },

  // 刪除使用者角色對應
  deleteRoleUser: (roleId, userId) => {
    return axios.delete(`/role-users/${roleId}/${userId}`)
  },

  // 批次刪除使用者角色對應
  batchDeleteRoleUsers: (data) => {
    return axios.delete('/role-users/batch', { data })
  },

  // 匯出角色之使用者報表
  exportReport: (data) => {
    return axios.post('/role-users/export', data, {
      responseType: 'blob'
    })
  }
}

/**
 * 權限分類報表 API (SYS0770)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const permissionCategoryReportApi = {
  // 查詢權限分類報表
  getReport: (params) => {
    return axios.get('/permission-category-reports', { params })
  },

  // 匯出權限分類報表
  exportReport: (data) => {
    return axios.post('/permission-category-reports/export', data, {
      responseType: 'blob'
    })
  }
}

/**
 * 系統作業與功能列表 API (SYS0810)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const systemProgramButtonApi = {
  // 查詢系統作業與功能列表
  getSystemProgramButtons: (systemId) => {
    return axios.get(`/systems/${systemId}/programs-and-buttons`)
  },

  // 匯出系統作業與功能列表報表
  exportSystemProgramButtons: (systemId, format = 'Excel') => {
    return axios.post(`/systems/${systemId}/programs-and-buttons/export`, null, {
      params: { exportFormat: format },
      responseType: 'blob'
    })
  }
}

/**
 * 按鈕操作記錄報表 API (SYS0790)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const buttonLogApi = {
  // 查詢按鈕操作記錄列表
  getButtonLogs: (params) => {
    return axios.get('/button-logs', { params })
  },

  // 新增按鈕操作記錄
  createButtonLog: (data) => {
    return axios.post('/button-logs', data)
  },

  // 匯出按鈕操作記錄報表
  exportReport: (data, format = 'excel') => {
    return axios.post('/button-logs/export', data, {
      params: { format },
      responseType: 'blob'
    })
  }
}

/**
 * 使用者查詢結果匯出 API (SYS0910)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const userQueryApi = {
  // 查詢使用者列表
  queryUsers: (data) => {
    return axios.post('/users/query', data)
  },

  // 匯出使用者查詢結果
  exportUserQuery: (data, format = 'excel') => {
    return axios.post('/users/query/export', data, {
      params: { format },
      responseType: 'blob'
    })
  }
}

