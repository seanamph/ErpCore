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
  }
}

