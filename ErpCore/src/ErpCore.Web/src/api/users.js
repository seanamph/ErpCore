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

