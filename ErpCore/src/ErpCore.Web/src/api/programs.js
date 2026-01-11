import request from '@/utils/request'

/**
 * 查詢作業列表
 */
export function getPrograms(params) {
  return request({
    url: '/api/v1/programs',
    method: 'get',
    params
  })
}

/**
 * 根據作業代碼查詢作業資訊
 */
export function getProgramById(programId) {
  return request({
    url: `/api/v1/programs/${programId}`,
    method: 'get'
  })
}

/**
 * 新增作業
 */
export function createProgram(data) {
  return request({
    url: '/api/v1/programs',
    method: 'post',
    data
  })
}

/**
 * 修改作業
 */
export function updateProgram(programId, data) {
  return request({
    url: `/api/v1/programs/${programId}`,
    method: 'put',
    data
  })
}

/**
 * 刪除作業
 */
export function deleteProgram(programId) {
  return request({
    url: `/api/v1/programs/${programId}`,
    method: 'delete'
  })
}

/**
 * 批次刪除作業
 */
export function deleteProgramsBatch(data) {
  return request({
    url: '/api/v1/programs/batch',
    method: 'delete',
    data
  })
}
