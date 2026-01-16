import axios from './axios'

/**
 * 電子發票處理作業 API (ECA3010)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const einvoiceApi = {
  // 上傳電子發票檔案
  uploadFile: (formData) => {
    return axios.post('/einvoices/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    })
  },

  // 查詢上傳記錄列表
  getUploads: (params) => {
    return axios.get('/einvoices/uploads', { params })
  },

  // 查詢單筆上傳記錄
  getUpload: (uploadId) => {
    return axios.get(`/einvoices/uploads/${uploadId}`)
  },

  // 查詢處理狀態
  getProcessStatus: (uploadId) => {
    return axios.get(`/einvoices/uploads/${uploadId}/status`)
  },

  // 開始處理上傳檔案
  startProcess: (uploadId) => {
    return axios.post(`/einvoices/uploads/${uploadId}/process`)
  },

  // 刪除上傳記錄
  deleteUpload: (uploadId) => {
    return axios.delete(`/einvoices/uploads/${uploadId}`)
  },

  // 下載上傳檔案 (ECA3010)
  downloadUpload: (uploadId) => {
    return axios.get(`/einvoices/uploads/${uploadId}/download`, {
      responseType: 'blob'
    })
  },

  // 下載處理結果 (成功/失敗清單) (ECA3010)
  downloadResult: (uploadId, type = 'all') => {
    return axios.get(`/einvoices/uploads/${uploadId}/download`, {
      params: { type },
      responseType: 'blob'
    })
  },

  // 注意：ECA2050 使用通用的上傳 API，通過 uploadType='ECA2050' 參數區分
  // 上傳時在 formData 中包含 uploadType: 'ECA2050'
  // 查詢時在 params 中包含 UploadType: 'ECA2050'

  // ========== ECA3030 專用 API ==========
  // 上傳電子發票檔案 (ECA3030)
  uploadFileECA3030: (formData) => {
    return axios.post('/einvoices/eca3030/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    })
  },

  // 查詢上傳記錄列表 (ECA3030)
  getUploadsECA3030: (params) => {
    return axios.get('/einvoices/eca3030/uploads', { params })
  },

  // 查詢單筆上傳記錄 (ECA3030)
  getUploadECA3030: (uploadId) => {
    return axios.get(`/einvoices/eca3030/uploads/${uploadId}`)
  },

  // 查詢處理狀態 (ECA3030)
  getProcessStatusECA3030: (uploadId) => {
    return axios.get(`/einvoices/eca3030/uploads/${uploadId}/status`)
  },

  // 開始處理上傳檔案 (ECA3030)
  startProcessECA3030: (uploadId) => {
    return axios.post(`/einvoices/eca3030/uploads/${uploadId}/process`)
  },

  // 刪除上傳記錄 (ECA3030)
  deleteUploadECA3030: (uploadId) => {
    return axios.delete(`/einvoices/eca3030/uploads/${uploadId}`)
  },

  // 下載上傳檔案 (ECA3030)
  downloadUploadECA3030: (uploadId) => {
    return axios.get(`/einvoices/eca3030/uploads/${uploadId}/download`, {
      responseType: 'blob'
    })
  }

  // 查詢電子發票列表
  getEInvoices: (params) => {
    return axios.get('/einvoices', { params })
  },

  // 查詢單筆電子發票
  getEInvoice: (invoiceId) => {
    return axios.get(`/einvoices/${invoiceId}`)
  },

  // 匯出Excel (ECA3020)
  exportExcel: (data) => {
    return axios.post('/einvoices/export/excel', data, {
      responseType: 'blob'
    })
  },

  // 匯出PDF (ECA3020)
  exportPdf: (data) => {
    return axios.post('/einvoices/export/pdf', data, {
      responseType: 'blob'
    })
  }
}

/**
 * 電子發票報表 API (ECA3040, ECA4010-ECA4060)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const einvoiceReportApi = {
  // 查詢電子發票報表
  getReports: (data) => {
    return axios.post('/einvoices/reports', data)
  },

  // 匯出Excel
  exportExcel: (data) => {
    return axios.post('/einvoices/reports/export/excel', data, {
      responseType: 'blob'
    })
  },

  // 匯出PDF
  exportPdf: (data) => {
    return axios.post('/einvoices/reports/export/pdf', data, {
      responseType: 'blob'
    })
  },

  // 列印報表 (ECA3040)
  print: (data) => {
    return axios.post('/einvoices/reports/print', data, {
      responseType: 'blob'
    })
  }
}

