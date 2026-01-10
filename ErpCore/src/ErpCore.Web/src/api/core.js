import axios from './axios'

/**
 * 核心功能模組 API
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const coreApi = {
  // 使用者管理
  userManagement: {
    // 查詢當前使用者資料
    getUserProfile: () => {
      return axios.get('/core/user-management/profile')
    },
    // 更新使用者資料
    updateUserProfile: (data) => {
      return axios.put('/core/user-management/profile', data)
    },
    // 修改密碼
    changePassword: (data) => {
      return axios.post('/core/user-management/change-password', data)
    },
    // 重置所有使用者密碼
    resetAllPasswords: (data) => {
      return axios.post('/core/user-management/reset-all-passwords', data)
    }
  },

  // 框架功能
  framework: {
    // 選單功能
    getMenuTree: (params) => {
      return axios.get('/core/framework/menus/tree', { params })
    },
    searchMenus: (params) => {
      return axios.get('/core/framework/menus/search', { params })
    },
    getFavorites: (params) => {
      return axios.get('/core/framework/menus/favorites', { params })
    },
    addFavorite: (data) => {
      return axios.post('/core/framework/menus/favorites', data)
    },
    removeFavorite: (favoriteId) => {
      return axios.delete(`/core/framework/menus/favorites/${favoriteId}`)
    },
    updateFavoriteSort: (data) => {
      return axios.put('/core/framework/menus/favorites/sort', data)
    },
    switchFrame: (data) => {
      return axios.post('/core/framework/menus/favorites/switch-frame', data)
    },
    // 標題功能
    getHeaderInfo: () => {
      return axios.get('/core/framework/header-info')
    },
    getCurrentTime: () => {
      return axios.get('/core/framework/current-time')
    },
    // 主選單功能
    getUserModules: () => {
      return axios.get('/core/framework/main-menu/modules')
    },
    getModuleSubsystems: (moduleId) => {
      return axios.get(`/core/framework/main-menu/modules/${moduleId}/subsystems`)
    },
    // 訊息功能
    getMessages: (params) => {
      return axios.get('/core/framework/messages', { params })
    },
    getMessage: (messageId) => {
      return axios.get(`/core/framework/messages/${messageId}`)
    },
    markAsRead: (messageId) => {
      return axios.post(`/core/framework/messages/${messageId}/read`)
    },
    batchMarkAsRead: (data) => {
      return axios.post('/core/framework/messages/batch-read', data)
    },
    getMessagesByDate: (params) => {
      return axios.get('/core/framework/messages/by-date', { params })
    },
    getUnreadCount: () => {
      return axios.get('/core/framework/messages/unread-count')
    }
  },

  // 資料維護功能 (IMS30系列)
  dataMaintenance: {
    // IMS30_FB - 資料瀏覽功能
    browseData: (moduleCode, params) => {
      return axios.get(`/core/data-maintenance/browse/${moduleCode}`, { params })
    },
    getBrowseConfig: (moduleCode) => {
      return axios.get(`/core/data-maintenance/browse/${moduleCode}/config`)
    },
    createBrowseConfig: (data) => {
      return axios.post('/core/data-maintenance/browse/config', data)
    },
    updateBrowseConfig: (configId, data) => {
      return axios.put(`/core/data-maintenance/browse/config/${configId}`, data)
    },
    deleteBrowseConfig: (configId) => {
      return axios.delete(`/core/data-maintenance/browse/config/${configId}`)
    },
    // IMS30_FI - 資料新增功能
    getInsertConfig: (moduleCode) => {
      return axios.get(`/core/data-maintenance/insert/${moduleCode}/config`)
    },
    insertData: (moduleCode, data) => {
      return axios.post(`/core/data-maintenance/insert/${moduleCode}`, data)
    },
    createInsertConfig: (data) => {
      return axios.post('/core/data-maintenance/insert/config', data)
    },
    updateInsertConfig: (configId, data) => {
      return axios.put(`/core/data-maintenance/insert/config/${configId}`, data)
    },
    deleteInsertConfig: (configId) => {
      return axios.delete(`/core/data-maintenance/insert/config/${configId}`)
    },
    // IMS30_FQ - 資料查詢功能
    queryData: (moduleCode, params) => {
      return axios.get(`/core/data-maintenance/query/${moduleCode}`, { params })
    },
    getQueryConfig: (moduleCode) => {
      return axios.get(`/core/data-maintenance/query/${moduleCode}/config`)
    },
    saveQuery: (moduleCode, data) => {
      return axios.post(`/core/data-maintenance/query/${moduleCode}/save-query`, data)
    },
    getSavedQueries: (moduleCode) => {
      return axios.get(`/core/data-maintenance/query/${moduleCode}/saved-queries`)
    },
    deleteSavedQuery: (queryId) => {
      return axios.delete(`/core/data-maintenance/query/saved-queries/${queryId}`)
    },
    // IMS30_FS - 資料排序功能
    getSortConfig: (moduleCode) => {
      return axios.get(`/core/data-maintenance/sort/${moduleCode}/config`)
    },
    applySort: (moduleCode, data) => {
      return axios.post(`/core/data-maintenance/sort/${moduleCode}/apply`, data)
    },
    saveSort: (moduleCode, data) => {
      return axios.post(`/core/data-maintenance/sort/${moduleCode}/save-sort`, data)
    },
    getSavedSorts: (moduleCode) => {
      return axios.get(`/core/data-maintenance/sort/${moduleCode}/saved-sorts`)
    },
    deleteSavedSort: (sortId) => {
      return axios.delete(`/core/data-maintenance/sort/saved-sorts/${sortId}`)
    },
    // IMS30_FU - 資料修改功能
    getDataForUpdate: (moduleCode, id) => {
      return axios.get(`/core/data-maintenance/update/${moduleCode}/${id}`)
    },
    getUpdateConfig: (moduleCode) => {
      return axios.get(`/core/data-maintenance/update/${moduleCode}/config`)
    },
    updateData: (moduleCode, id, data) => {
      return axios.put(`/core/data-maintenance/update/${moduleCode}/${id}`, data)
    },
    createUpdateConfig: (data) => {
      return axios.post('/core/data-maintenance/update/config', data)
    },
    updateUpdateConfig: (configId, data) => {
      return axios.put(`/core/data-maintenance/update/config/${configId}`, data)
    },
    deleteUpdateConfig: (configId) => {
      return axios.delete(`/core/data-maintenance/update/config/${configId}`)
    },
    // IMS30_PR - 資料列印功能
    getPrintConfigs: (moduleCode) => {
      return axios.get(`/core/data-maintenance/print/${moduleCode}/config`)
    },
    previewPrint: (moduleCode, data) => {
      return axios.post(`/core/data-maintenance/print/${moduleCode}/preview`, data, { responseType: 'blob' })
    },
    printData: (moduleCode, data) => {
      return axios.post(`/core/data-maintenance/print/${moduleCode}/print`, data, { responseType: 'blob' })
    },
    createPrintConfig: (data) => {
      return axios.post('/core/data-maintenance/print/config', data)
    },
    updatePrintConfig: (configId, data) => {
      return axios.put(`/core/data-maintenance/print/config/${configId}`, data)
    },
    deletePrintConfig: (configId) => {
      return axios.delete(`/core/data-maintenance/print/config/${configId}`)
    }
  },

  // 工具功能
  tools: {
    // Export_Excel - Excel匯出功能
    getExportConfigs: (moduleCode) => {
      return axios.get(`/core/tools/excel-export/${moduleCode}/config`)
    },
    exportExcel: (moduleCode, data) => {
      return axios.post(`/core/tools/excel-export/${moduleCode}/export`, data, { responseType: 'blob' })
    },
    getExportTaskStatus: (taskId) => {
      return axios.get(`/core/tools/excel-export/tasks/${taskId}`)
    },
    downloadExportFile: (taskId) => {
      return axios.get(`/core/tools/excel-export/download/${taskId}`, { responseType: 'blob' })
    },
    createExportConfig: (data) => {
      return axios.post('/core/tools/excel-export/config', data)
    },
    updateExportConfig: (configId, data) => {
      return axios.put(`/core/tools/excel-export/config/${configId}`, data)
    },
    deleteExportConfig: (configId) => {
      return axios.delete(`/core/tools/excel-export/config/${configId}`)
    },
    // Encode_String - 字串編碼工具
    encodeString: (data) => {
      return axios.post('/core/tools/encode-string/encode', data)
    },
    decodeString: (data) => {
      return axios.post('/core/tools/encode-string/decode', data)
    },
    // ASPXTOASP - ASP轉ASPX工具
    aspxToAsp: (data) => {
      return axios.post('/core/tools/aspx-to-asp', data)
    },
    aspToAspx: (data) => {
      return axios.post('/core/tools/asp-to-aspx', data)
    },
    getPageTransitions: (params) => {
      return axios.get('/core/tools/page-transitions', { params })
    },
    getPageTransitionMappings: () => {
      return axios.get('/core/tools/page-transition-mappings')
    },
    createPageTransitionMapping: (data) => {
      return axios.post('/core/tools/page-transition-mappings', data)
    },
    updatePageTransitionMapping: (mappingId, data) => {
      return axios.put(`/core/tools/page-transition-mappings/${mappingId}`, data)
    },
    deletePageTransitionMapping: (mappingId) => {
      return axios.delete(`/core/tools/page-transition-mappings/${mappingId}`)
    }
  },

  // 系統功能
  systemFunction: {
    // Identify - 系統識別功能
    getSystemIdentity: () => {
      return axios.get('/core/system-function/system-identity')
    },
    getMenuConfig: () => {
      return axios.get('/core/system-function/system-identity/menu')
    },
    updateSystemIdentity: (data) => {
      return axios.put('/core/system-function/system-identity', data)
    },
    initializeSystem: () => {
      return axios.post('/core/system-function/system-identity/initialize')
    },
    // MakeRegFile - 系統註冊功能
    getHardwareInfo: () => {
      return axios.get('/core/system-function/registration/hardware-info')
    },
    generateRegistration: (data) => {
      return axios.post('/core/system-function/registration/generate', data)
    },
    verifyRegistration: (data) => {
      return axios.post('/core/system-function/registration/verify', data)
    },
    uploadRegistrationFile: (file) => {
      const formData = new FormData()
      formData.append('file', file)
      return axios.post('/core/system-function/registration/upload', formData, {
        headers: { 'Content-Type': 'multipart/form-data' }
      })
    },
    downloadRegistrationFile: (registrationId) => {
      return axios.get(`/core/system-function/registration/files/${registrationId}/download`, { responseType: 'blob' })
    },
    getSystemRegistrations: (params) => {
      return axios.get('/core/system-function/registrations', { params })
    },
    revokeRegistration: (registrationId) => {
      return axios.put(`/core/system-function/registrations/${registrationId}/revoke`)
    },
    webRegister: (data) => {
      return axios.post('/core/system-function/registration/web-register', data)
    },
    // about - 關於頁面功能
    getAboutInfo: () => {
      return axios.get('/core/system-function/about')
    }
  }
}

