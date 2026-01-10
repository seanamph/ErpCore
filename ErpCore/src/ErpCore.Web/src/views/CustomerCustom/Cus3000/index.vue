<template>
  <div class="cus3000">
    <div class="page-header">
      <h1>CUS3000 客戶定制模組 (SYS3130-SYS3160, SYS3310-SYS3399, SYS3510-SYS3580)</h1>
    </div>

    <!-- 標籤頁 -->
    <el-tabs v-model="activeTab" type="border-card">
      <!-- 會員管理 -->
      <el-tab-pane label="會員管理" name="members">
        <el-card class="search-card" shadow="never">
          <el-form :inline="true" :model="memberQueryForm" class="search-form">
            <el-form-item label="會員編號">
              <el-input v-model="memberQueryForm.MemberId" placeholder="請輸入會員編號" clearable />
            </el-form-item>
            <el-form-item label="會員名稱">
              <el-input v-model="memberQueryForm.MemberName" placeholder="請輸入會員名稱" clearable />
            </el-form-item>
            <el-form-item label="狀態">
              <el-select v-model="memberQueryForm.Status" placeholder="請選擇狀態" clearable>
                <el-option label="全部" value="" />
                <el-option label="啟用" value="A" />
                <el-option label="停用" value="I" />
              </el-select>
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleMemberSearch">查詢</el-button>
              <el-button @click="handleMemberReset">重置</el-button>
              <el-button type="success" @click="handleMemberCreate">新增會員</el-button>
            </el-form-item>
          </el-form>
        </el-card>

        <el-card class="table-card" shadow="never">
          <el-table
            :data="memberTableData"
            v-loading="memberLoading"
            border
            stripe
            style="width: 100%"
          >
            <el-table-column prop="MemberId" label="會員編號" width="150" />
            <el-table-column prop="MemberName" label="會員名稱" width="200" />
            <el-table-column prop="CardNo" label="卡號" width="150" />
            <el-table-column prop="Status" label="狀態" width="100">
              <template #default="{ row }">
                <el-tag :type="row.Status === 'A' ? 'success' : 'danger'">
                  {{ row.Status === 'A' ? '啟用' : '停用' }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column label="操作" width="200" fixed="right">
              <template #default="{ row }">
                <el-button type="primary" size="small" @click="handleMemberEdit(row)">修改</el-button>
                <el-button type="danger" size="small" @click="handleMemberDelete(row)">刪除</el-button>
              </template>
            </el-table-column>
          </el-table>

          <el-pagination
            v-model:current-page="memberPagination.PageIndex"
            v-model:page-size="memberPagination.PageSize"
            :total="memberPagination.TotalCount"
            :page-sizes="[10, 20, 50, 100]"
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="handleMemberSizeChange"
            @current-change="handleMemberPageChange"
            style="margin-top: 20px; text-align: right"
          />
        </el-card>
      </el-tab-pane>

      <!-- 促銷活動管理 -->
      <el-tab-pane label="促銷活動管理" name="promotions">
        <el-card class="search-card" shadow="never">
          <el-form :inline="true" :model="promotionQueryForm" class="search-form">
            <el-form-item label="促銷活動編號">
              <el-input v-model="promotionQueryForm.PromotionId" placeholder="請輸入促銷活動編號" clearable />
            </el-form-item>
            <el-form-item label="促銷活動名稱">
              <el-input v-model="promotionQueryForm.PromotionName" placeholder="請輸入促銷活動名稱" clearable />
            </el-form-item>
            <el-form-item label="狀態">
              <el-select v-model="promotionQueryForm.Status" placeholder="請選擇狀態" clearable>
                <el-option label="全部" value="" />
                <el-option label="啟用" value="A" />
                <el-option label="停用" value="I" />
              </el-select>
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handlePromotionSearch">查詢</el-button>
              <el-button @click="handlePromotionReset">重置</el-button>
              <el-button type="success" @click="handlePromotionCreate">新增促銷活動</el-button>
            </el-form-item>
          </el-form>
        </el-card>

        <el-card class="table-card" shadow="never">
          <el-table
            :data="promotionTableData"
            v-loading="promotionLoading"
            border
            stripe
            style="width: 100%"
          >
            <el-table-column prop="PromotionId" label="促銷活動編號" width="150" />
            <el-table-column prop="PromotionName" label="促銷活動名稱" width="200" />
            <el-table-column prop="StartDate" label="開始日期" width="150" />
            <el-table-column prop="EndDate" label="結束日期" width="150" />
            <el-table-column prop="Status" label="狀態" width="100">
              <template #default="{ row }">
                <el-tag :type="row.Status === 'A' ? 'success' : 'danger'">
                  {{ row.Status === 'A' ? '啟用' : '停用' }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column label="操作" width="200" fixed="right">
              <template #default="{ row }">
                <el-button type="primary" size="small" @click="handlePromotionEdit(row)">修改</el-button>
                <el-button type="danger" size="small" @click="handlePromotionDelete(row)">刪除</el-button>
              </template>
            </el-table-column>
          </el-table>

          <el-pagination
            v-model:current-page="promotionPagination.PageIndex"
            v-model:page-size="promotionPagination.PageSize"
            :total="promotionPagination.TotalCount"
            :page-sizes="[10, 20, 50, 100]"
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="handlePromotionSizeChange"
            @current-change="handlePromotionPageChange"
            style="margin-top: 20px; text-align: right"
          />
        </el-card>
      </el-tab-pane>

      <!-- 活動管理 -->
      <el-tab-pane label="活動管理" name="activities">
        <el-card class="search-card" shadow="never">
          <el-form :inline="true" :model="activityQueryForm" class="search-form">
            <el-form-item label="活動編號">
              <el-input v-model="activityQueryForm.ActivityId" placeholder="請輸入活動編號" clearable />
            </el-form-item>
            <el-form-item label="活動名稱">
              <el-input v-model="activityQueryForm.ActivityName" placeholder="請輸入活動名稱" clearable />
            </el-form-item>
            <el-form-item label="狀態">
              <el-select v-model="activityQueryForm.Status" placeholder="請選擇狀態" clearable>
                <el-option label="全部" value="" />
                <el-option label="啟用" value="A" />
                <el-option label="停用" value="I" />
              </el-select>
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleActivitySearch">查詢</el-button>
              <el-button @click="handleActivityReset">重置</el-button>
              <el-button type="success" @click="handleActivityCreate">新增活動</el-button>
            </el-form-item>
          </el-form>
        </el-card>

        <el-card class="table-card" shadow="never">
          <el-table
            :data="activityTableData"
            v-loading="activityLoading"
            border
            stripe
            style="width: 100%"
          >
            <el-table-column prop="ActivityId" label="活動編號" width="150" />
            <el-table-column prop="ActivityName" label="活動名稱" width="200" />
            <el-table-column prop="ActivityDate" label="活動日期" width="150" />
            <el-table-column prop="Status" label="狀態" width="100">
              <template #default="{ row }">
                <el-tag :type="row.Status === 'A' ? 'success' : 'danger'">
                  {{ row.Status === 'A' ? '啟用' : '停用' }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column label="操作" width="200" fixed="right">
              <template #default="{ row }">
                <el-button type="primary" size="small" @click="handleActivityEdit(row)">修改</el-button>
                <el-button type="danger" size="small" @click="handleActivityDelete(row)">刪除</el-button>
              </template>
            </el-table-column>
          </el-table>

          <el-pagination
            v-model:current-page="activityPagination.PageIndex"
            v-model:page-size="activityPagination.PageSize"
            :total="activityPagination.TotalCount"
            :page-sizes="[10, 20, 50, 100]"
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="handleActivitySizeChange"
            @current-change="handleActivityPageChange"
            style="margin-top: 20px; text-align: right"
          />
        </el-card>
      </el-tab-pane>
    </el-tabs>

    <!-- 會員對話框 -->
    <el-dialog
      v-model="memberDialogVisible"
      :title="memberDialogTitle"
      width="600px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="memberFormRef"
        :model="memberFormData"
        :rules="memberFormRules"
        label-width="120px"
      >
        <el-form-item label="會員編號" prop="MemberId">
          <el-input v-model="memberFormData.MemberId" :disabled="memberIsEdit" placeholder="請輸入會員編號" />
        </el-form-item>
        <el-form-item label="會員名稱" prop="MemberName">
          <el-input v-model="memberFormData.MemberName" placeholder="請輸入會員名稱" />
        </el-form-item>
        <el-form-item label="卡號">
          <el-input v-model="memberFormData.CardNo" placeholder="請輸入卡號" />
        </el-form-item>
        <el-form-item label="狀態" prop="Status">
          <el-select v-model="memberFormData.Status" placeholder="請選擇狀態" style="width: 100%">
            <el-option label="啟用" value="A" />
            <el-option label="停用" value="I" />
          </el-select>
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="memberDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleMemberSubmit">確定</el-button>
      </template>
    </el-dialog>

    <!-- 促銷活動對話框 -->
    <el-dialog
      v-model="promotionDialogVisible"
      :title="promotionDialogTitle"
      width="600px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="promotionFormRef"
        :model="promotionFormData"
        :rules="promotionFormRules"
        label-width="120px"
      >
        <el-form-item label="促銷活動編號" prop="PromotionId">
          <el-input v-model="promotionFormData.PromotionId" :disabled="promotionIsEdit" placeholder="請輸入促銷活動編號" />
        </el-form-item>
        <el-form-item label="促銷活動名稱" prop="PromotionName">
          <el-input v-model="promotionFormData.PromotionName" placeholder="請輸入促銷活動名稱" />
        </el-form-item>
        <el-form-item label="開始日期" prop="StartDate">
          <el-date-picker
            v-model="promotionFormData.StartDate"
            type="datetime"
            placeholder="請選擇開始日期"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="結束日期" prop="EndDate">
          <el-date-picker
            v-model="promotionFormData.EndDate"
            type="datetime"
            placeholder="請選擇結束日期"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="狀態" prop="Status">
          <el-select v-model="promotionFormData.Status" placeholder="請選擇狀態" style="width: 100%">
            <el-option label="啟用" value="A" />
            <el-option label="停用" value="I" />
          </el-select>
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="promotionDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handlePromotionSubmit">確定</el-button>
      </template>
    </el-dialog>

    <!-- 活動對話框 -->
    <el-dialog
      v-model="activityDialogVisible"
      :title="activityDialogTitle"
      width="600px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="activityFormRef"
        :model="activityFormData"
        :rules="activityFormRules"
        label-width="120px"
      >
        <el-form-item label="活動編號" prop="ActivityId">
          <el-input v-model="activityFormData.ActivityId" :disabled="activityIsEdit" placeholder="請輸入活動編號" />
        </el-form-item>
        <el-form-item label="活動名稱" prop="ActivityName">
          <el-input v-model="activityFormData.ActivityName" placeholder="請輸入活動名稱" />
        </el-form-item>
        <el-form-item label="活動日期" prop="ActivityDate">
          <el-date-picker
            v-model="activityFormData.ActivityDate"
            type="datetime"
            placeholder="請選擇活動日期"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="狀態" prop="Status">
          <el-select v-model="activityFormData.Status" placeholder="請選擇狀態" style="width: 100%">
            <el-option label="啟用" value="A" />
            <el-option label="停用" value="I" />
          </el-select>
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="activityDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleActivitySubmit">確定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import axios from '@/api/axios'

// API
const cus3000Api = {
  // 會員管理
  getMembers: (params) => axios.get('/api/v1/cus3000/members', { params }),
  getMember: (tKey) => axios.get(`/api/v1/cus3000/members/${tKey}`),
  createMember: (data) => axios.post('/api/v1/cus3000/members', data),
  updateMember: (tKey, data) => axios.put(`/api/v1/cus3000/members/${tKey}`, data),
  deleteMember: (tKey) => axios.delete(`/api/v1/cus3000/members/${tKey}`),
  // 促銷活動管理
  getPromotions: (params) => axios.get('/api/v1/cus3000/promotions', { params }),
  getPromotion: (tKey) => axios.get(`/api/v1/cus3000/promotions/${tKey}`),
  createPromotion: (data) => axios.post('/api/v1/cus3000/promotions', data),
  updatePromotion: (tKey, data) => axios.put(`/api/v1/cus3000/promotions/${tKey}`, data),
  deletePromotion: (tKey) => axios.delete(`/api/v1/cus3000/promotions/${tKey}`),
  // 活動管理
  getActivities: (params) => axios.get('/api/v1/cus3000/activities', { params }),
  getActivity: (tKey) => axios.get(`/api/v1/cus3000/activities/${tKey}`),
  createActivity: (data) => axios.post('/api/v1/cus3000/activities', data),
  updateActivity: (tKey, data) => axios.put(`/api/v1/cus3000/activities/${tKey}`, data),
  deleteActivity: (tKey) => axios.delete(`/api/v1/cus3000/activities/${tKey}`)
}

// 標籤頁
const activeTab = ref('members')

// 會員管理
const memberQueryForm = reactive({
  MemberId: '',
  MemberName: '',
  Status: ''
})
const memberTableData = ref([])
const memberLoading = ref(false)
const memberPagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})
const memberDialogVisible = ref(false)
const memberDialogTitle = computed(() => memberIsEdit.value ? '修改會員' : '新增會員')
const memberIsEdit = ref(false)
const memberFormRef = ref(null)
const memberFormData = reactive({
  MemberId: '',
  MemberName: '',
  CardNo: '',
  Status: 'A'
})
const memberFormRules = {
  MemberId: [{ required: true, message: '請輸入會員編號', trigger: 'blur' }],
  MemberName: [{ required: true, message: '請輸入會員名稱', trigger: 'blur' }],
  Status: [{ required: true, message: '請選擇狀態', trigger: 'change' }]
}
const currentMemberTKey = ref(null)

// 促銷活動管理
const promotionQueryForm = reactive({
  PromotionId: '',
  PromotionName: '',
  Status: ''
})
const promotionTableData = ref([])
const promotionLoading = ref(false)
const promotionPagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})
const promotionDialogVisible = ref(false)
const promotionDialogTitle = computed(() => promotionIsEdit.value ? '修改促銷活動' : '新增促銷活動')
const promotionIsEdit = ref(false)
const promotionFormRef = ref(null)
const promotionFormData = reactive({
  PromotionId: '',
  PromotionName: '',
  StartDate: '',
  EndDate: '',
  Status: 'A'
})
const promotionFormRules = {
  PromotionId: [{ required: true, message: '請輸入促銷活動編號', trigger: 'blur' }],
  PromotionName: [{ required: true, message: '請輸入促銷活動名稱', trigger: 'blur' }],
  StartDate: [{ required: true, message: '請選擇開始日期', trigger: 'change' }],
  EndDate: [{ required: true, message: '請選擇結束日期', trigger: 'change' }],
  Status: [{ required: true, message: '請選擇狀態', trigger: 'change' }]
}
const currentPromotionTKey = ref(null)

// 活動管理
const activityQueryForm = reactive({
  ActivityId: '',
  ActivityName: '',
  Status: ''
})
const activityTableData = ref([])
const activityLoading = ref(false)
const activityPagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})
const activityDialogVisible = ref(false)
const activityDialogTitle = computed(() => activityIsEdit.value ? '修改活動' : '新增活動')
const activityIsEdit = ref(false)
const activityFormRef = ref(null)
const activityFormData = reactive({
  ActivityId: '',
  ActivityName: '',
  ActivityDate: '',
  Status: 'A'
})
const activityFormRules = {
  ActivityId: [{ required: true, message: '請輸入活動編號', trigger: 'blur' }],
  ActivityName: [{ required: true, message: '請輸入活動名稱', trigger: 'blur' }],
  ActivityDate: [{ required: true, message: '請選擇活動日期', trigger: 'change' }],
  Status: [{ required: true, message: '請選擇狀態', trigger: 'change' }]
}
const currentActivityTKey = ref(null)

// 會員管理方法
const handleMemberSearch = async () => {
  memberLoading.value = true
  try {
    const params = {
      PageIndex: memberPagination.PageIndex,
      PageSize: memberPagination.PageSize,
      MemberId: memberQueryForm.MemberId || undefined,
      MemberName: memberQueryForm.MemberName || undefined,
      Status: memberQueryForm.Status || undefined
    }
    const response = await cus3000Api.getMembers(params)
    if (response.data?.success) {
      memberTableData.value = response.data.data?.items || []
      memberPagination.TotalCount = response.data.data?.totalCount || 0
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  } finally {
    memberLoading.value = false
  }
}

const handleMemberReset = () => {
  memberQueryForm.MemberId = ''
  memberQueryForm.MemberName = ''
  memberQueryForm.Status = ''
  memberPagination.PageIndex = 1
  handleMemberSearch()
}

const handleMemberCreate = () => {
  memberIsEdit.value = false
  currentMemberTKey.value = null
  Object.assign(memberFormData, {
    MemberId: '',
    MemberName: '',
    CardNo: '',
    Status: 'A'
  })
  memberDialogVisible.value = true
}

const handleMemberEdit = async (row) => {
  try {
    const response = await cus3000Api.getMember(row.TKey)
    if (response.data?.success) {
      memberIsEdit.value = true
      currentMemberTKey.value = row.TKey
      Object.assign(memberFormData, response.data.data)
      memberDialogVisible.value = true
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  }
}

const handleMemberDelete = async (row) => {
  try {
    await ElMessageBox.confirm('確定要刪除此會員嗎？', '確認', { type: 'warning' })
    const response = await cus3000Api.deleteMember(row.TKey)
    if (response.data?.success) {
      ElMessage.success('刪除成功')
      handleMemberSearch()
    } else {
      ElMessage.error(response.data?.message || '刪除失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗：' + error.message)
    }
  }
}

const handleMemberSubmit = async () => {
  if (!memberFormRef.value) return
  await memberFormRef.value.validate(async (valid) => {
    if (valid) {
      try {
        let response
        if (memberIsEdit.value) {
          response = await cus3000Api.updateMember(currentMemberTKey.value, memberFormData)
        } else {
          response = await cus3000Api.createMember(memberFormData)
        }
        if (response.data?.success) {
          ElMessage.success(memberIsEdit.value ? '修改成功' : '新增成功')
          memberDialogVisible.value = false
          handleMemberSearch()
        } else {
          ElMessage.error(response.data?.message || '操作失敗')
        }
      } catch (error) {
        ElMessage.error('操作失敗：' + error.message)
      }
    }
  })
}

const handleMemberSizeChange = (size) => {
  memberPagination.PageSize = size
  memberPagination.PageIndex = 1
  handleMemberSearch()
}

const handleMemberPageChange = (page) => {
  memberPagination.PageIndex = page
  handleMemberSearch()
}

// 促銷活動管理方法
const handlePromotionSearch = async () => {
  promotionLoading.value = true
  try {
    const params = {
      PageIndex: promotionPagination.PageIndex,
      PageSize: promotionPagination.PageSize,
      PromotionId: promotionQueryForm.PromotionId || undefined,
      PromotionName: promotionQueryForm.PromotionName || undefined,
      Status: promotionQueryForm.Status || undefined
    }
    const response = await cus3000Api.getPromotions(params)
    if (response.data?.success) {
      promotionTableData.value = response.data.data?.items || []
      promotionPagination.TotalCount = response.data.data?.totalCount || 0
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  } finally {
    promotionLoading.value = false
  }
}

const handlePromotionReset = () => {
  promotionQueryForm.PromotionId = ''
  promotionQueryForm.PromotionName = ''
  promotionQueryForm.Status = ''
  promotionPagination.PageIndex = 1
  handlePromotionSearch()
}

const handlePromotionCreate = () => {
  promotionIsEdit.value = false
  currentPromotionTKey.value = null
  Object.assign(promotionFormData, {
    PromotionId: '',
    PromotionName: '',
    StartDate: '',
    EndDate: '',
    Status: 'A'
  })
  promotionDialogVisible.value = true
}

const handlePromotionEdit = async (row) => {
  try {
    const response = await cus3000Api.getPromotion(row.TKey)
    if (response.data?.success) {
      promotionIsEdit.value = true
      currentPromotionTKey.value = row.TKey
      Object.assign(promotionFormData, response.data.data)
      promotionDialogVisible.value = true
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  }
}

const handlePromotionDelete = async (row) => {
  try {
    await ElMessageBox.confirm('確定要刪除此促銷活動嗎？', '確認', { type: 'warning' })
    const response = await cus3000Api.deletePromotion(row.TKey)
    if (response.data?.success) {
      ElMessage.success('刪除成功')
      handlePromotionSearch()
    } else {
      ElMessage.error(response.data?.message || '刪除失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗：' + error.message)
    }
  }
}

const handlePromotionSubmit = async () => {
  if (!promotionFormRef.value) return
  await promotionFormRef.value.validate(async (valid) => {
    if (valid) {
      try {
        let response
        if (promotionIsEdit.value) {
          response = await cus3000Api.updatePromotion(currentPromotionTKey.value, promotionFormData)
        } else {
          response = await cus3000Api.createPromotion(promotionFormData)
        }
        if (response.data?.success) {
          ElMessage.success(promotionIsEdit.value ? '修改成功' : '新增成功')
          promotionDialogVisible.value = false
          handlePromotionSearch()
        } else {
          ElMessage.error(response.data?.message || '操作失敗')
        }
      } catch (error) {
        ElMessage.error('操作失敗：' + error.message)
      }
    }
  })
}

const handlePromotionSizeChange = (size) => {
  promotionPagination.PageSize = size
  promotionPagination.PageIndex = 1
  handlePromotionSearch()
}

const handlePromotionPageChange = (page) => {
  promotionPagination.PageIndex = page
  handlePromotionSearch()
}

// 活動管理方法
const handleActivitySearch = async () => {
  activityLoading.value = true
  try {
    const params = {
      PageIndex: activityPagination.PageIndex,
      PageSize: activityPagination.PageSize,
      ActivityId: activityQueryForm.ActivityId || undefined,
      ActivityName: activityQueryForm.ActivityName || undefined,
      Status: activityQueryForm.Status || undefined
    }
    const response = await cus3000Api.getActivities(params)
    if (response.data?.success) {
      activityTableData.value = response.data.data?.items || []
      activityPagination.TotalCount = response.data.data?.totalCount || 0
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  } finally {
    activityLoading.value = false
  }
}

const handleActivityReset = () => {
  activityQueryForm.ActivityId = ''
  activityQueryForm.ActivityName = ''
  activityQueryForm.Status = ''
  activityPagination.PageIndex = 1
  handleActivitySearch()
}

const handleActivityCreate = () => {
  activityIsEdit.value = false
  currentActivityTKey.value = null
  Object.assign(activityFormData, {
    ActivityId: '',
    ActivityName: '',
    ActivityDate: '',
    Status: 'A'
  })
  activityDialogVisible.value = true
}

const handleActivityEdit = async (row) => {
  try {
    const response = await cus3000Api.getActivity(row.TKey)
    if (response.data?.success) {
      activityIsEdit.value = true
      currentActivityTKey.value = row.TKey
      Object.assign(activityFormData, response.data.data)
      activityDialogVisible.value = true
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  }
}

const handleActivityDelete = async (row) => {
  try {
    await ElMessageBox.confirm('確定要刪除此活動嗎？', '確認', { type: 'warning' })
    const response = await cus3000Api.deleteActivity(row.TKey)
    if (response.data?.success) {
      ElMessage.success('刪除成功')
      handleActivitySearch()
    } else {
      ElMessage.error(response.data?.message || '刪除失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗：' + error.message)
    }
  }
}

const handleActivitySubmit = async () => {
  if (!activityFormRef.value) return
  await activityFormRef.value.validate(async (valid) => {
    if (valid) {
      try {
        let response
        if (activityIsEdit.value) {
          response = await cus3000Api.updateActivity(currentActivityTKey.value, activityFormData)
        } else {
          response = await cus3000Api.createActivity(activityFormData)
        }
        if (response.data?.success) {
          ElMessage.success(activityIsEdit.value ? '修改成功' : '新增成功')
          activityDialogVisible.value = false
          handleActivitySearch()
        } else {
          ElMessage.error(response.data?.message || '操作失敗')
        }
      } catch (error) {
        ElMessage.error('操作失敗：' + error.message)
      }
    }
  })
}

const handleActivitySizeChange = (size) => {
  activityPagination.PageSize = size
  activityPagination.PageIndex = 1
  handleActivitySearch()
}

const handleActivityPageChange = (page) => {
  activityPagination.PageIndex = page
  handleActivitySearch()
}

// 初始化
onMounted(() => {
  handleMemberSearch()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.cus3000 {
  padding: 20px;

  .page-header {
    margin-bottom: 20px;
    
    h1 {
      font-size: 24px;
      font-weight: bold;
      color: $primary-color;
      margin: 0;
    }
  }

  .search-card {
    margin-bottom: 20px;
    
    .search-form {
      margin: 0;
    }
  }

  .table-card {
    margin-bottom: 20px;
  }
}
</style>

