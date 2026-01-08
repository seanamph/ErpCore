import request from '@/utils/request'

/**
 * 查詢商品列表
 */
export function getProducts(params) {
  return request({
    url: '/api/v1/products',
    method: 'get',
    params
  })
}

/**
 * 根據商品編號查詢商品資訊
 */
export function getProductById(goodsId) {
  return request({
    url: `/api/v1/products/${goodsId}`,
    method: 'get'
  })
}

/**
 * 根據商品編號查詢商品名稱（簡化版，用於快速查詢）
 */
export function getProductName(goodsId) {
  return request({
    url: `/api/v1/products/${goodsId}/name`,
    method: 'get'
  })
}

/**
 * 根據商品名稱查詢商品列表（用於前端下拉選擇）
 * @param {string} goodsName - 商品名稱（模糊查詢）
 * @param {number} maxCount - 最大返回筆數，預設50
 */
export function searchProductsByName(goodsName, maxCount = 50) {
  return request({
    url: '/api/v1/products/search',
    method: 'get',
    params: {
      goodsName,
      maxCount
    }
  })
}

