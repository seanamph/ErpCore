import axios from './axios'

/**
 * 人力資源管理 API
 * 遵循 C# API 欄位命名 (PascalCase)
 */
export const humanResourceApi = {
  // ========== 人事管理 (Personnel) ==========
  
  // 查詢員工列表
  getEmployees: (params) => {
    return axios.get('/human-resource/personnel', { params })
  },

  // 查詢單筆員工
  getEmployee: (employeeId) => {
    return axios.get(`/human-resource/personnel/${employeeId}`)
  },

  // 新增員工
  createEmployee: (data) => {
    return axios.post('/human-resource/personnel', data)
  },

  // 修改員工
  updateEmployee: (employeeId, data) => {
    return axios.put(`/human-resource/personnel/${employeeId}`, data)
  },

  // 刪除員工
  deleteEmployee: (employeeId) => {
    return axios.delete(`/human-resource/personnel/${employeeId}`)
  },

  // ========== 薪資管理 (Payroll) ==========
  
  // 查詢薪資列表
  getPayrolls: (params) => {
    return axios.get('/human-resource/payroll', { params })
  },

  // 查詢單筆薪資
  getPayroll: (payrollId) => {
    return axios.get(`/human-resource/payroll/${payrollId}`)
  },

  // 新增薪資
  createPayroll: (data) => {
    return axios.post('/human-resource/payroll', data)
  },

  // 修改薪資
  updatePayroll: (payrollId, data) => {
    return axios.put(`/human-resource/payroll/${payrollId}`, data)
  },

  // 刪除薪資
  deletePayroll: (payrollId) => {
    return axios.delete(`/human-resource/payroll/${payrollId}`)
  },

  // 確認薪資
  confirmPayroll: (payrollId) => {
    return axios.post(`/human-resource/payroll/${payrollId}/confirm`)
  },

  // 計算薪資（不儲存）
  calculatePayroll: (data) => {
    return axios.post('/human-resource/payroll/calculate', data)
  },

  // ========== 考勤管理 (Attendance) ==========
  
  // 查詢考勤列表
  getAttendances: (params) => {
    return axios.get('/human-resource/attendance', { params })
  },

  // 查詢單筆考勤
  getAttendance: (attendanceId) => {
    return axios.get(`/human-resource/attendance/${attendanceId}`)
  },

  // 新增考勤
  createAttendance: (data) => {
    return axios.post('/human-resource/attendance', data)
  },

  // 修改考勤
  updateAttendance: (attendanceId, data) => {
    return axios.put(`/human-resource/attendance/${attendanceId}`, data)
  },

  // 刪除考勤
  deleteAttendance: (attendanceId) => {
    return axios.delete(`/human-resource/attendance/${attendanceId}`)
  },

  // 上班打卡
  checkIn: (data) => {
    return axios.post('/human-resource/attendance/check-in', data)
  },

  // 下班打卡
  checkOut: (data) => {
    return axios.post('/human-resource/attendance/check-out', data)
  },

  // 補打卡
  supplementAttendance: (data) => {
    return axios.post('/human-resource/attendance/supplement', data)
  }
}

