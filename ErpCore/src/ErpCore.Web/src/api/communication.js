import axios from './axios'

/**
 * 通訊與通知 API
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const communicationApi = {
  // ========== 自動處理郵件作業 (AutoProcessMail) ==========
  // 處理郵件佇列
  processEmailQueue: (data) => {
    return axios.post('/email/process-queue', data)
  },

  // 重試失敗郵件
  retryFailedEmails: (data) => {
    return axios.post('/email/retry-failed', data)
  },

  // 查詢郵件佇列狀態
  getEmailQueueStatus: () => {
    return axios.get('/email/queue-status')
  },

  // ========== 資料編碼作業 (EncodeData) ==========
  // 編碼資料
  encodeData: (data) => {
    return axios.post('/encode/encode', data)
  },

  // 解碼資料
  decodeData: (data) => {
    return axios.post('/encode/decode', data)
  },

  // 查詢編碼記錄
  getEncodeLogs: (params) => {
    return axios.get('/encode/logs', { params })
  },

  // ========== 郵件簡訊發送作業 (SYS5000) ==========
  // 發送郵件
  sendEmail: (data) => {
    return axios.post('/mail-sms/send-email', data)
  },

  // 發送簡訊
  sendSms: (data) => {
    return axios.post('/mail-sms/send-sms', data)
  },

  // 查詢郵件記錄
  getEmailLogs: (params) => {
    return axios.get('/mail-sms/email-logs', { params })
  },

  // 查詢簡訊記錄
  getSmsLogs: (params) => {
    return axios.get('/mail-sms/sms-logs', { params })
  }
}

