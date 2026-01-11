import request from '@/utils/request'

/**
 * 查詢主系統列表
 */
export function getSystems(params) {
  return request({
    url: '/api/v1/systems',
    method: 'get',
    params
  })
}

/**
 * 根據主系統代碼查詢主系統資訊
 */
export function getSystemById(systemId) {
  return request({
    url: `/api/v1/systems/${systemId}`,
    method: 'get'
  })
}

/**
 * 新增主系統
 */
export function createSystem(data) {
  return request({
    url: '/api/v1/systems',
    method: 'post',
    data
  })
}

/**
 * 修改主系統
 */
export function updateSystem(systemId, data) {
  return request({
    url: `/api/v1/systems/${systemId}`,
    method: 'put',
    data
  })
}

/**
 * 刪除主系統
 */
export function deleteSystem(systemId) {
  return request({
    url: `/api/v1/systems/${systemId}`,
    method: 'delete'
  })
}

/**
 * 批次刪除主系統
 */
export function deleteSystemsBatch(data) {
  return request({
    url: '/api/v1/systems/batch',
    method: 'delete',
    data
  })
}

/**
 * 取得系統型態選項
 */
export function getSystemTypes() {
  return request({
    url: '/api/v1/systems/system-types',
    method: 'get'
  })
}
