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

/**
 * 查詢系統作業與功能列表 (SYS0810)
 */
export function getSystemProgramButtons(systemId) {
  return request({
    url: `/api/v1/systems/${systemId}/programs-and-buttons`,
    method: 'get'
  })
}

/**
 * 匯出系統作業與功能列表報表 (SYS0810)
 */
export function exportSystemProgramButtons(systemId, exportFormat = 'Excel') {
  return request({
    url: `/api/v1/systems/${systemId}/programs-and-buttons/export`,
    method: 'post',
    params: { exportFormat },
    responseType: 'blob'
  })
}

/**
 * 查詢系統作業與功能列表（出庫用）(SYS0999)
 */
export function getSystemProgramButtonsForExport(systemId) {
  return request({
    url: `/api/v1/systems/${systemId}/programs-and-buttons/export-query`,
    method: 'get'
  })
}

/**
 * 匯出系統作業與功能列表報表（出庫用）(SYS0999)
 */
export function exportSystemProgramButtonsReport(systemId, exportFormat = 'Excel') {
  return request({
    url: `/api/v1/systems/${systemId}/programs-and-buttons/export-report`,
    method: 'post',
    params: { exportFormat },
    responseType: 'blob'
  })
}
