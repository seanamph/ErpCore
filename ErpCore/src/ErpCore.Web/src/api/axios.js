import axios from 'axios'

// 建立 axios 實例
const service = axios.create({
  baseURL: process.env.VUE_APP_API_BASE_URL || '/api/v1',
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json'
  }
})

// 請求攔截器
service.interceptors.request.use(
  config => {
    // 可在這裡加入 token 等認證資訊
    // const token = localStorage.getItem('token')
    // if (token) {
    //   config.headers.Authorization = `Bearer ${token}`
    // }
    return config
  },
  error => {
    console.error('請求錯誤:', error)
    return Promise.reject(error)
  }
)

// 回應攔截器
service.interceptors.response.use(
  response => {
    // 統一處理 API 回應格式
    const res = response.data
    // 支援 Success 和 success 兩種格式
    if ((res.Success !== undefined && !res.Success) || (res.success !== undefined && !res.success)) {
      // API 返回錯誤
      const message = res.Message || res.message || '未知錯誤'
      console.error('API 錯誤:', message)
      return Promise.reject(new Error(message))
    }
    return res
  },
  error => {
    console.error('回應錯誤:', error)
    const message = error.response?.data?.message || error.response?.data?.Message || error.message || '請求失敗'
    return Promise.reject(new Error(message))
  }
)

export default service

