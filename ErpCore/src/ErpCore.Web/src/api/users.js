import request from '@/utils/request'

/**
 * 查詢使用者列表
 */
export function getUsers(params) {
  return request({
    url: '/api/v1/users',
    method: 'get',
    params
  })
}

/**
 * 根據使用者編號查詢使用者資訊
 */
export function getUserById(userId) {
  return request({
    url: `/api/v1/users/${userId}`,
    method: 'get'
  })
}

/**
 * 取得當前登入使用者資訊
 */
export function getCurrentUser() {
  return request({
    url: '/api/v1/users/current',
    method: 'get'
  })
}

/**
 * 新增使用者
 */
export function createUser(data) {
  return request({
    url: '/api/v1/users',
    method: 'post',
    data
  })
}

/**
 * 修改使用者
 */
export function updateUser(userId, data) {
  return request({
    url: `/api/v1/users/${userId}`,
    method: 'put',
    data
  })
}

/**
 * 刪除使用者
 */
export function deleteUser(userId) {
  return request({
    url: `/api/v1/users/${userId}`,
    method: 'delete'
  })
}

/**
 * 批次刪除使用者
 */
export function deleteUsersBatch(data) {
  return request({
    url: '/api/v1/users/batch',
    method: 'delete',
    data
  })
}

/**
 * 修改密碼
 */
export function changePassword(userId, data) {
  return request({
    url: `/api/v1/users/${userId}/password`,
    method: 'put',
    data
  })
}

/**
 * 使用者資料瀏覽（只讀模式）
 */
export function browseUsers(params) {
  return request({
    url: '/api/v1/users/browse',
    method: 'get',
    params
  })
}

/**
 * 使用者資料瀏覽（單筆，只讀模式）
 */
export function browseUserById(userId) {
  return request({
    url: `/api/v1/users/${userId}/browse`,
    method: 'get'
  })
}

/**
 * 驗證使用者編號和密碼
 */
export function validateUser(data) {
  return request({
    url: '/api/v1/users/validate',
    method: 'post',
    data
  })
}

/**
 * 修改使用者帳戶原則
 */
export function updateAccountPolicy(userId, data) {
  return request({
    url: `/api/v1/users/${userId}/account-policy`,
    method: 'put',
    data
  })
}

/**
 * 僅修改帳號終止日
 */
export function updateEndDate(userId, data) {
  return request({
    url: `/api/v1/users/${userId}/end-date`,
    method: 'put',
    data
  })
}

/**
 * 使用者查詢功能
 */
export function queryUsers(data) {
  return request({
    url: '/api/v1/users/query',
    method: 'post',
    data
  })
}

/**
 * 匯出使用者查詢結果
 */
export function exportUserQuery(data, format = 'excel') {
  return request({
    url: '/api/v1/users/query/export',
    method: 'post',
    data,
    params: { format },
    responseType: 'blob'
  })
}

/**
 * 重設密碼 (SYS0110)
 */
export function resetPassword(userId, data) {
  return request({
    url: `/api/v1/users/${userId}/reset-password`,
    method: 'post',
    data
  })
}

/**
 * 更新使用者狀態 (SYS0110)
 */
export function updateStatus(userId, data) {
  return request({
    url: `/api/v1/users/${userId}/status`,
    method: 'put',
    data
  })
}

/**
 * 使用者資料查詢 (SYS0140) - GET 方法
 */
export function queryUsersGet(params) {
  return request({
    url: '/api/v1/users/query',
    method: 'get',
    params
  })
}

/**
 * 查詢單筆使用者 (SYS0140)
 */
export function queryUserById(userId) {
  return request({
    url: `/api/v1/users/${userId}/query`,
    method: 'get'
  })
}

/**
 * 匯出使用者查詢結果 (SYS0140)
 */
export function exportUsers(data) {
  return request({
    url: '/api/v1/users/export',
    method: 'post',
    data,
    responseType: 'blob'
  })
}

/**
 * 查詢使用者詳細資料（含業種儲位）(SYS0111)
 */
export function getUserDetail(userId) {
  return request({
    url: `/api/v1/users/${userId}/detail`,
    method: 'get'
  })
}

/**
 * 查詢業種大分類列表 (SYS0111)
 */
export function getBusinessTypeMajors() {
  return request({
    url: '/api/v1/users/business-types/major',
    method: 'get'
  })
}

/**
 * 查詢業種中分類列表 (SYS0111)
 */
export function getBusinessTypeMiddles(btypeMId) {
  return request({
    url: '/api/v1/users/business-types/middle',
    method: 'get',
    params: { btypeMId }
  })
}

/**
 * 查詢業種小分類列表 (SYS0111)
 */
export function getBusinessTypeMinors(btypeId) {
  return request({
    url: '/api/v1/users/business-types/minor',
    method: 'get',
    params: { btypeId }
  })
}

/**
 * 查詢儲位列表 (SYS0111)
 */
export function getWarehouseAreas(level, parentId) {
  return request({
    url: '/api/v1/users/warehouse-areas',
    method: 'get',
    params: { level, parentId }
  })
}

/**
 * 查詢7X承租分店列表 (SYS0111)
 */
export function getStores() {
  return request({
    url: '/api/v1/users/stores',
    method: 'get'
  })
}

/**
 * 新增使用者（含業種儲位設定）(SYS0111)
 */
export function createUserWithBusinessTypes(data) {
  return request({
    url: '/api/v1/users/with-business-types',
    method: 'post',
    data
  })
}

/**
 * 修改使用者（含業種儲位設定）(SYS0111)
 */
export function updateUserWithBusinessTypes(userId, data) {
  return request({
    url: `/api/v1/users/${userId}/with-business-types`,
    method: 'put',
    data
  })
}

/**
 * 查詢總公司列表 (SYS0113)
 */
export function getParentShops() {
  return request({
    url: '/api/v1/users/shops/parent',
    method: 'get'
  })
}

/**
 * 查詢分店列表 (SYS0113)
 */
export function getShops(pShopId) {
  return request({
    url: '/api/v1/users/shops',
    method: 'get',
    params: { pShopId }
  })
}

/**
 * 查詢據點列表 (SYS0113)
 */
export function getSites(shopId) {
  return request({
    url: '/api/v1/users/sites',
    method: 'get',
    params: { shopId }
  })
}

/**
 * 查詢廠商列表 (SYS0113)
 */
export function getVendors() {
  return request({
    url: '/api/v1/users/vendors',
    method: 'get'
  })
}

/**
 * 查詢部門列表 (SYS0113)
 */
export function getDepartments() {
  return request({
    url: '/api/v1/users/departments',
    method: 'get'
  })
}

/**
 * 新增使用者（含分店廠商部門設定）(SYS0113)
 */
export function createUserWithShopsVendorsDepts(data) {
  return request({
    url: '/api/v1/users/with-shops-vendors-depts',
    method: 'post',
    data
  })
}

/**
 * 修改使用者（含分店廠商部門設定）(SYS0113)
 */
export function updateUserWithShopsVendorsDepts(userId, data) {
  return request({
    url: `/api/v1/users/${userId}/with-shops-vendors-depts`,
    method: 'put',
    data
  })
}

/**
 * 重設密碼（含自動產生）(SYS0113)
 */
export function resetPasswordWithAuto(userId, data) {
  return request({
    url: `/api/v1/users/${userId}/reset-password-auto`,
    method: 'post',
    data
  })
}

// ========== SYS0114 相關 API ==========

/**
 * 查詢使用者詳細資料（含AD和組織）(SYS0114)
 */
export function getUserDetailWithAdOrgs(userId) {
  return request({
    url: `/api/v1/users/${userId}/detail-ad-orgs`,
    method: 'get'
  })
}

/**
 * 查詢組織列表 (SYS0114)
 */
export function getOrganizations() {
  return request({
    url: '/api/v1/users/organizations',
    method: 'get'
  })
}

/**
 * 新增使用者（含AD和組織設定）(SYS0114)
 */
export function createUserWithAdOrgs(data) {
  return request({
    url: '/api/v1/users/with-ad-orgs',
    method: 'post',
    data
  })
}

/**
 * 修改使用者（含AD和組織設定）(SYS0114)
 */
export function updateUserWithAdOrgs(userId, data) {
  return request({
    url: `/api/v1/users/${userId}/with-ad-orgs`,
    method: 'put',
    data
  })
}

/**
 * 驗證Active Directory使用者 (SYS0114)
 */
export function validateAdUser(data) {
  return request({
    url: '/api/v1/users/validate-ad-user',
    method: 'post',
    data
  })
}

