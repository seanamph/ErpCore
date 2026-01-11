import request from '@/utils/request'

/**
 * 查詢子系統列表
 */
export function getMenus(params) {
  return request({
    url: '/api/v1/menus',
    method: 'get',
    params
  })
}

/**
 * 根據子系統代碼查詢子系統資訊
 */
export function getMenuById(menuId) {
  return request({
    url: `/api/v1/menus/${menuId}`,
    method: 'get'
  })
}

/**
 * 新增子系統
 */
export function createMenu(data) {
  return request({
    url: '/api/v1/menus',
    method: 'post',
    data
  })
}

/**
 * 修改子系統
 */
export function updateMenu(menuId, data) {
  return request({
    url: `/api/v1/menus/${menuId}`,
    method: 'put',
    data
  })
}

/**
 * 刪除子系統
 */
export function deleteMenu(menuId) {
  return request({
    url: `/api/v1/menus/${menuId}`,
    method: 'delete'
  })
}

/**
 * 批次刪除子系統
 */
export function deleteMenusBatch(data) {
  return request({
    url: '/api/v1/menus/batch',
    method: 'delete',
    data
  })
}
