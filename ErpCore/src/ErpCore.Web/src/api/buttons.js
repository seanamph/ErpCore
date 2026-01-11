import request from '@/utils/request'

/**
 * 查詢按鈕列表
 */
export function getButtons(params) {
  return request({
    url: '/api/v1/buttons',
    method: 'get',
    params
  })
}

/**
 * 根據主鍵查詢按鈕資訊
 */
export function getButtonById(tKey) {
  return request({
    url: `/api/v1/buttons/${tKey}`,
    method: 'get'
  })
}

/**
 * 新增按鈕
 */
export function createButton(data) {
  return request({
    url: '/api/v1/buttons',
    method: 'post',
    data
  })
}

/**
 * 修改按鈕
 */
export function updateButton(tKey, data) {
  return request({
    url: `/api/v1/buttons/${tKey}`,
    method: 'put',
    data
  })
}

/**
 * 刪除按鈕
 */
export function deleteButton(tKey) {
  return request({
    url: `/api/v1/buttons/${tKey}`,
    method: 'delete'
  })
}

/**
 * 批次刪除按鈕
 */
export function deleteButtonsBatch(data) {
  return request({
    url: '/api/v1/buttons/batch',
    method: 'delete',
    data
  })
}
