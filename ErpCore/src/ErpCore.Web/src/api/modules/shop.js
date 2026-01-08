import request from '@/utils/request'

/**
 * 查詢店舖列表
 */
export function getShops(params) {
  return request({
    url: '/api/v1/shops',
    method: 'get',
    params
  })
}

