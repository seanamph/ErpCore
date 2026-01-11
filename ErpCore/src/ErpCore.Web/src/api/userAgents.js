import request from '@/utils/request'

/**
 * 查詢代理列表
 */
export function getUserAgents(params) {
  return request({
    url: '/api/v1/user-agents',
    method: 'get',
    params
  })
}

/**
 * 查詢單筆代理記錄
 */
export function getUserAgent(agentId) {
  return request({
    url: `/api/v1/user-agents/${agentId}`,
    method: 'get'
  })
}

/**
 * 查詢委託人的代理記錄
 */
export function getUserAgentsByPrincipal(userId, params) {
  return request({
    url: `/api/v1/user-agents/users/${userId}/agent-as-principal`,
    method: 'get',
    params
  })
}

/**
 * 查詢代理人的代理記錄
 */
export function getUserAgentsByAgent(userId, params) {
  return request({
    url: `/api/v1/user-agents/users/${userId}/agent-as-agent`,
    method: 'get',
    params
  })
}

/**
 * 查詢有效代理記錄
 */
export function getActiveUserAgents(params) {
  return request({
    url: '/api/v1/user-agents/active',
    method: 'get',
    params
  })
}

/**
 * 新增代理記錄
 */
export function createUserAgent(data) {
  return request({
    url: '/api/v1/user-agents',
    method: 'post',
    data
  })
}

/**
 * 修改代理記錄
 */
export function updateUserAgent(agentId, data) {
  return request({
    url: `/api/v1/user-agents/${agentId}`,
    method: 'put',
    data
  })
}

/**
 * 刪除代理記錄
 */
export function deleteUserAgent(agentId) {
  return request({
    url: `/api/v1/user-agents/${agentId}`,
    method: 'delete'
  })
}

/**
 * 批次刪除代理記錄
 */
export function deleteUserAgentsBatch(data) {
  return request({
    url: '/api/v1/user-agents/batch',
    method: 'delete',
    data
  })
}

/**
 * 更新代理記錄狀態
 */
export function updateUserAgentStatus(agentId, data) {
  return request({
    url: `/api/v1/user-agents/${agentId}/status`,
    method: 'put',
    data
  })
}

/**
 * 檢查代理權限
 */
export function checkAgentPermission(data) {
  return request({
    url: '/api/v1/user-agents/check-permission',
    method: 'post',
    data
  })
}
