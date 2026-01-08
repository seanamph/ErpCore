import axios from './axios'

/**
 * HT680 - BAT格式文本文件處理系列 API
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const textFileApi = {
  // 上傳文本文件並開始處理
  uploadFile: (file, fileType, shopId) => {
    const formData = new FormData()
    formData.append('file', file)
    formData.append('fileType', fileType)
    if (shopId) {
      formData.append('shopId', shopId)
    }
    return axios.post('/textfile/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    })
  },

  // 查詢處理記錄列表
  getProcessLogs: (params) => {
    return axios.get('/textfile/process-logs', { params })
  },

  // 根據處理記錄ID查詢單筆記錄
  getProcessLogById: (logId) => {
    return axios.get(`/textfile/process-logs/${logId}`)
  },

  // 查詢處理記錄的明細列表
  getProcessDetails: (logId, params) => {
    return axios.get(`/textfile/process-logs/${logId}/details`, { params })
  },

  // 重新處理文件
  reprocessFile: (logId) => {
    return axios.post(`/textfile/process-logs/${logId}/reprocess`)
  },

  // 刪除處理記錄
  deleteProcessLog: (logId) => {
    return axios.delete(`/textfile/process-logs/${logId}`)
  },

  // 下載處理結果文件（Excel或CSV格式）
  downloadProcessResult: (logId, format = 'excel') => {
    return axios.get(`/textfile/process-logs/${logId}/download`, {
      params: { format },
      responseType: 'blob'
    })
  }
}

