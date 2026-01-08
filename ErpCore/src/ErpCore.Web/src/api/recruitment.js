import axios from './axios'

/**
 * 潛客主檔維護作業 API (SYSC165)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const prospectMasterApi = {
  // 查詢潛客主檔列表
  getProspectMasters: (params) => {
    return axios.get('/prospect-masters', { params })
  },

  // 查詢單筆潛客主檔
  getProspectMaster: (masterId) => {
    return axios.get(`/prospect-masters/${masterId}`)
  },

  // 新增潛客主檔
  createProspectMaster: (data) => {
    return axios.post('/prospect-masters', data)
  },

  // 修改潛客主檔
  updateProspectMaster: (masterId, data) => {
    return axios.put(`/prospect-masters/${masterId}`, data)
  },

  // 刪除潛客主檔
  deleteProspectMaster: (masterId) => {
    return axios.delete(`/prospect-masters/${masterId}`)
  },

  // 批次刪除潛客主檔
  batchDeleteProspectMasters: (data) => {
    return axios.delete('/prospect-masters/batch', { data })
  },

  // 更新潛客主檔狀態
  updateProspectMasterStatus: (masterId, data) => {
    return axios.patch(`/prospect-masters/${masterId}/status`, data)
  }
}

/**
 * 潛客維護作業 API (SYSC180)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const prospectApi = {
  // 查詢潛客列表
  getProspects: (params) => {
    return axios.get('/prospects', { params })
  },

  // 查詢單筆潛客
  getProspect: (prospectId) => {
    return axios.get(`/prospects/${prospectId}`)
  },

  // 新增潛客
  createProspect: (data) => {
    return axios.post('/prospects', data)
  },

  // 修改潛客
  updateProspect: (prospectId, data) => {
    return axios.put(`/prospects/${prospectId}`, data)
  },

  // 刪除潛客
  deleteProspect: (prospectId) => {
    return axios.delete(`/prospects/${prospectId}`)
  },

  // 批次刪除潛客
  batchDeleteProspects: (data) => {
    return axios.delete('/prospects/batch', { data })
  },

  // 更新潛客狀態
  updateProspectStatus: (prospectId, data) => {
    return axios.patch(`/prospects/${prospectId}/status`, data)
  }
}

/**
 * 訪談維護作業 API (SYSC222)
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const interviewApi = {
  // 查詢訪談列表
  getInterviews: (params) => {
    return axios.get('/interviews', { params })
  },

  // 根據潛客查詢訪談列表
  getInterviewsByProspect: (prospectId, params) => {
    return axios.get(`/interviews/by-prospect/${prospectId}`, { params })
  },

  // 查詢單筆訪談
  getInterview: (interviewId) => {
    return axios.get(`/interviews/${interviewId}`)
  },

  // 新增訪談
  createInterview: (data) => {
    return axios.post('/interviews', data)
  },

  // 修改訪談
  updateInterview: (interviewId, data) => {
    return axios.put(`/interviews/${interviewId}`, data)
  },

  // 刪除訪談
  deleteInterview: (interviewId) => {
    return axios.delete(`/interviews/${interviewId}`)
  },

  // 批次刪除訪談
  batchDeleteInterviews: (data) => {
    return axios.delete('/interviews/batch', { data })
  },

  // 更新訪談狀態
  updateInterviewStatus: (interviewId, data) => {
    return axios.patch(`/interviews/${interviewId}/status`, data)
  }
}

