import axios from './axios'

/**
 * 工具類 API
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const toolsApi = {
  // ========== 檔案上傳 ==========
  
  // 上傳檔案
  uploadFile: (file, onProgress) => {
    const formData = new FormData()
    formData.append('file', file)
    return axios.post('/tools/file-upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      },
      onUploadProgress: (progressEvent) => {
        if (onProgress) {
          const percentCompleted = Math.round((progressEvent.loaded * 100) / progressEvent.total)
          onProgress(percentCompleted)
        }
      }
    })
  },

  // 查詢上傳記錄
  getUploadLogs: (params) => {
    return axios.get('/tools/file-upload/logs', { params })
  },

  // 刪除上傳檔案
  deleteUploadFile: (fileId) => {
    return axios.delete(`/tools/file-upload/${fileId}`)
  },

  // ========== 條碼處理 ==========
  
  // 產生條碼
  generateBarcode: (data) => {
    return axios.post('/tools/barcode/generate', data, {
      responseType: 'blob'
    })
  },

  // 讀取條碼
  readBarcode: (file) => {
    const formData = new FormData()
    formData.append('file', file)
    return axios.post('/tools/barcode/read', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    })
  },

  // ========== HTML轉PDF ==========
  
  // 轉換HTML為PDF
  htmlToPdf: (data) => {
    return axios.post('/tools/html2pdf/convert', data, {
      responseType: 'blob'
    })
  },

  // 查詢轉換記錄
  getPdfConversionLogs: (params) => {
    return axios.get('/tools/html2pdf/logs', { params })
  }
}

