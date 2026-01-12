import axios from '@/utils/request'

/**
 * 單位領用申請作業 API (SYSA210)
 */

export const materialApplyApi = {
  // 查詢領用申請列表
  getMaterialApplies: (params) => {
    return axios.get('/api/v1/material-applies', { params })
  },

  // 查詢單筆領用申請（含明細）
  getMaterialApplyDetail: (applyId) => {
    return axios.get(`/api/v1/material-applies/${applyId}`)
  },

  // 新增領用申請
  createMaterialApply: (data) => {
    return axios.post('/api/v1/material-applies', data)
  },

  // 修改領用申請
  updateMaterialApply: (applyId, data) => {
    return axios.put(`/api/v1/material-applies/${applyId}`, data)
  },

  // 刪除領用申請
  deleteMaterialApply: (applyId) => {
    return axios.delete(`/api/v1/material-applies/${applyId}`)
  },

  // 審核領用申請
  approveMaterialApply: (applyId, data) => {
    return axios.post(`/api/v1/material-applies/${applyId}/approve`, data)
  },

  // 發料作業
  issueMaterialApply: (applyId, data) => {
    return axios.post(`/api/v1/material-applies/${applyId}/issue`, data)
  },

  // 批次新增領用申請
  batchCreateMaterialApply: (data) => {
    return axios.post('/api/v1/material-applies/batch', data)
  },

  // 產生領用單號
  generateApplyId: () => {
    return axios.get('/api/v1/material-applies/generate-apply-id')
  }
}
