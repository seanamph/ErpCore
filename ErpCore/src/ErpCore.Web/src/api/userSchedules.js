import request from '@/utils/request'

/**
 * 查詢排程列表
 */
export function getUserSchedules(params) {
  return request({
    url: '/api/v1/user-schedules',
    method: 'get',
    params
  })
}

/**
 * 根據排程編號查詢排程資訊
 */
export function getUserScheduleById(scheduleId) {
  return request({
    url: `/api/v1/user-schedules/${scheduleId}`,
    method: 'get'
  })
}

/**
 * 新增排程
 */
export function createUserSchedule(data) {
  return request({
    url: '/api/v1/user-schedules',
    method: 'post',
    data
  })
}

/**
 * 修改排程
 */
export function updateUserSchedule(scheduleId, data) {
  return request({
    url: `/api/v1/user-schedules/${scheduleId}`,
    method: 'put',
    data
  })
}

/**
 * 取消排程
 */
export function cancelUserSchedule(scheduleId) {
  return request({
    url: `/api/v1/user-schedules/${scheduleId}/cancel`,
    method: 'post'
  })
}

/**
 * 執行排程（系統內部）
 */
export function executeUserSchedule(scheduleId) {
  return request({
    url: `/api/v1/user-schedules/${scheduleId}/execute`,
    method: 'post'
  })
}
