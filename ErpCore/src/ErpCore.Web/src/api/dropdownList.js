import axios from './axios'

/**
 * 下拉列表 API
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const dropdownListApi = {
  // ========== 地址列表 (ADDR_CITY_LIST, ADDR_ZONE_LIST) ==========
  // 查詢城市列表
  getCities: (params) => {
    return axios.get('/lists/addresses/cities', { params })
  },

  // 查詢單筆城市
  getCity: (cityId) => {
    return axios.get(`/lists/addresses/cities/${cityId}`)
  },

  // 查詢城市選項（用於下拉選單）
  getCityOptions: (params) => {
    return axios.get('/lists/addresses/cities/options', { params })
  },

  // 查詢區域列表
  getZones: (params) => {
    return axios.get('/lists/addresses/zones', { params })
  },

  // 查詢單筆區域
  getZone: (zoneId) => {
    return axios.get(`/lists/addresses/zones/${zoneId}`)
  },

  // 查詢區域選項（用於下拉選單）
  getZoneOptions: (params) => {
    return axios.get('/lists/addresses/zones/options', { params })
  },

  // ========== 日期列表 (DATE_LIST) ==========
  // 取得系統日期格式設定
  getDateFormat: () => {
    return axios.get('/system/date-format')
  },

  // 驗證日期格式
  validateDate: (data) => {
    return axios.post('/system/validate-date', data)
  },

  // 解析日期字串
  parseDate: (data) => {
    return axios.post('/system/parse-date', data)
  },

  // ========== 選單列表 (MENU_LIST) ==========
  // 查詢選單列表
  getMenus: (params) => {
    return axios.get('/lists/menus', { params })
  },

  // 查詢單筆選單
  getMenu: (menuId) => {
    return axios.get(`/lists/menus/${menuId}`)
  },

  // 查詢選單選項（用於下拉選單）
  getMenuOptions: (params) => {
    return axios.get('/lists/menus/options', { params })
  },

  // ========== 多選列表 (MULTI_AREA_LIST, MULTI_SHOP_LIST, MULTI_USERS_LIST) ==========
  // 查詢多選區域列表
  getMultiAreas: (params) => {
    return axios.get('/lists/multi-select/areas', { params })
  },

  // 查詢區域選項（用於下拉選單）
  getAreaOptions: (params) => {
    return axios.get('/lists/multi-select/areas/options', { params })
  },

  // 查詢多選店別列表
  getMultiShops: (params) => {
    return axios.get('/lists/multi-select/shops', { params })
  },

  // 查詢店別選項（用於下拉選單）
  getShopOptions: (params) => {
    return axios.get('/lists/multi-select/shops/options', { params })
  },

  // 查詢多選使用者列表
  getMultiUsers: (params) => {
    return axios.get('/lists/multi-select/users', { params })
  },

  // 查詢使用者選項（用於下拉選單）
  getUserOptions: (params) => {
    return axios.get('/lists/multi-select/users/options', { params })
  },

  // ========== 系統列表 (SYSID_LIST, USER_LIST) ==========
  // 查詢系統列表
  getSystems: (params) => {
    return axios.get('/lists/systems', { params })
  },

  // 查詢單筆系統
  getSystem: (systemId) => {
    return axios.get(`/lists/systems/${systemId}`)
  },

  // 查詢系統選項（用於下拉選單）
  getSystemOptions: (params) => {
    return axios.get('/lists/systems/options', { params })
  },

  // 查詢使用者列表
  getUsers: (params) => {
    return axios.get('/lists/system/users', { params })
  },

  // 查詢使用者選項（用於下拉選單）
  getUserListOptions: (params) => {
    return axios.get('/lists/system/users/options', { params })
  },

  // ========== 使用者列表 (USER_LIST) ==========
  // 查詢使用者列表 (USER_LIST_USER_LIST)
  getUserList: (params) => {
    return axios.get('/lists/users/user-list', { params })
  },

  // 查詢部門使用者列表 (USER_LIST_DEPT_LIST)
  getDeptUserList: (params) => {
    return axios.get('/lists/users/dept-list', { params })
  },

  // 查詢其他使用者列表 (USER_LIST_OTHER_LIST)
  getOtherUserList: (params) => {
    return axios.get('/lists/users/other-list', { params })
  }
}

