import axios from 'axios'
import { ElMessage } from 'element-plus'

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
    if (res.Success !== undefined && !res.Success) {
      // API 返回錯誤
      ElMessage.error(res.Message || '未知錯誤')
      return Promise.reject(new Error(res.Message || '未知錯誤'))
    }
    return response
  },
  error => {
    console.error('回應錯誤:', error)
    const message = error.response?.data?.message || error.message || '請求失敗'
    ElMessage.error(message)
    return Promise.reject(error)
  }
)

export default service
