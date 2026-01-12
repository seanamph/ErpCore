<template>
  <div class="sys0114-query">
    <div class="page-header">
      <h1>使用者基本資料維護(ActiveDirectory) (SYS0114)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="使用者編號">
          <el-input v-model="queryForm.UserId" placeholder="請輸入使用者編號" clearable />
        </el-form-item>
        <el-form-item label="使用者名稱">
          <el-input v-model="queryForm.UserName" placeholder="請輸入使用者名稱" clearable />
        </el-form-item>
        <el-form-item label="組織">
          <el-input v-model="queryForm.OrgId" placeholder="請輸入組織代碼" clearable />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable style="width: 150px">
            <el-option label="啟用" value="A" />
            <el-option label="停用" value="I" />
            <el-option label="鎖定" value="L" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleCreate">新增</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 資料表格 -->
    <el-card class="table-card" shadow="never">
      <el-table
        :data="tableData"
        v-loading="loading"
        border
        stripe
        style="width: 100%"
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="55" align="center" />
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column prop="UserId" label="使用者編號" width="120" sortable />
        <el-table-column prop="UserName" label="使用者名稱" width="150" sortable />
        <el-table-column prop="Title" label="職稱" width="100" />
        <el-table-column prop="OrgId" label="組織代碼" width="120" />
        <el-table-column prop="Status" label="狀態" width="80">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ getStatusText(row.Status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="UseActiveDirectory" label="使用AD" width="80" align="center">
          <template #default="{ row }">
            <el-tag :type="row.UseActiveDirectory ? 'success' : 'info'">
              {{ row.UseActiveDirectory ? '是' : '否' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="UserType" label="使用者型態" width="120" />
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleEdit(row)">修改</el-button>
            <el-button type="danger" size="small" @click="handleDelete(row)">刪除</el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分頁 -->
      <el-pagination
        v-model:current-page="pagination.PageIndex"
        v-model:page-size="pagination.PageSize"
        :total="pagination.TotalCount"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleSizeChange"
        @current-change="handlePageChange"
        style="margin-top: 20px; text-align: right"
      />
    </el-card>

    <!-- 新增/修改對話框 -->
    <el-dialog
      :title="dialogTitle"
      v-model="dialogVisible"
      width="1200px"
      @close="handleDialogClose"
    >
      <el-form :model="form" :rules="rules" ref="formRef" label-width="150px">
        <!-- 基本資料區塊 -->
        <el-divider>基本資料</el-divider>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="使用者編號" prop="UserId">
              <el-input v-model="form.UserId" placeholder="請輸入使用者編號" :disabled="isEdit" maxlength="50" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="使用者名稱" prop="UserName">
              <el-input v-model="form.UserName" placeholder="請輸入使用者名稱" maxlength="100" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="密碼" prop="UserPassword" v-if="!isEdit && !form.UseActiveDirectory">
              <el-input v-model="form.UserPassword" type="password" placeholder="請輸入密碼" show-password maxlength="255" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="職稱" prop="Title">
              <el-input v-model="form.Title" placeholder="請輸入職稱" maxlength="50" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="組織代碼" prop="OrgId">
              <el-input v-model="form.OrgId" placeholder="請輸入組織代碼" maxlength="50" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="狀態" prop="Status">
              <el-radio-group v-model="form.Status">
                <el-radio label="A">啟用</el-radio>
                <el-radio label="I">停用</el-radio>
                <el-radio label="L">鎖定</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="有效起始日" prop="StartDate">
              <el-date-picker
                v-model="form.StartDate"
                type="date"
                placeholder="請選擇日期"
                style="width: 100%"
                value-format="YYYY-MM-DD"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="有效終止日" prop="EndDate">
              <el-date-picker
                v-model="form.EndDate"
                type="date"
                placeholder="請選擇日期"
                style="width: 100%"
                value-format="YYYY-MM-DD"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="使用者型態" prop="UserType">
              <el-input v-model="form.UserType" placeholder="請輸入使用者型態" maxlength="20" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="使用者等級" prop="UserPriority">
              <el-input-number v-model="form.UserPriority" :min="0" :step="1" style="width: 100%" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="備註" prop="Notes">
          <el-input v-model="form.Notes" type="textarea" :rows="3" placeholder="請輸入備註" maxlength="500" show-word-limit />
        </el-form-item>

        <!-- Active Directory設定區塊 -->
        <el-divider>Active Directory設定</el-divider>
        <el-form-item label="使用Active Directory">
          <el-switch v-model="form.UseActiveDirectory" @change="handleAdChange" />
        </el-form-item>
        <template v-if="form.UseActiveDirectory">
          <el-row :gutter="20">
            <el-col :span="12">
              <el-form-item label="AD網域" prop="AdDomain">
                <el-input v-model="form.AdDomain" placeholder="請輸入AD網域" maxlength="100" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="AD使用者主體名稱" prop="AdUserPrincipalName">
                <el-input v-model="form.AdUserPrincipalName" placeholder="請輸入AD使用者主體名稱" maxlength="255" />
                <el-button type="primary" size="small" @click="validateAdUserHandler" style="margin-top: 5px;">驗證</el-button>
              </el-form-item>
            </el-col>
          </el-row>
          <el-form-item v-if="adValidationResult">
            <el-alert
              :title="adValidationResult.Exists ? `驗證成功: ${adValidationResult.DisplayName || adValidationResult.UserName}` : '驗證失敗: 使用者不存在'"
              :type="adValidationResult.Exists ? 'success' : 'error'"
              :closable="false"
            />
          </el-form-item>
        </template>

        <!-- 業種權限設定區塊 -->
        <el-divider>業種權限設定</el-divider>
        <el-table :data="form.BusinessTypes" border style="margin-bottom: 10px">
          <el-table-column type="index" label="序號" width="60" align="center" />
          <el-table-column label="大分類" width="200">
            <template #default="{ row, $index }">
              <el-select
                v-model="row.BtypeMId"
                placeholder="請選擇"
                clearable
                style="width: 100%"
                @change="handleBtypeMChange($index)"
              >
                <el-option
                  v-for="item in businessTypeMajorList"
                  :key="item.BtypeMId"
                  :label="item.BtypeMName"
                  :value="item.BtypeMId"
                />
              </el-select>
            </template>
          </el-table-column>
          <el-table-column label="中分類" width="200">
            <template #default="{ row, $index }">
              <el-select
                v-model="row.BtypeId"
                placeholder="請選擇"
                clearable
                :disabled="!row.BtypeMId"
                style="width: 100%"
                @change="handleBtypeChange($index)"
              >
                <el-option
                  v-for="item in getBusinessTypeMiddleList(row.BtypeMId)"
                  :key="item.BtypeId"
                  :label="item.BtypeName"
                  :value="item.BtypeId"
                />
              </el-select>
            </template>
          </el-table-column>
          <el-table-column label="小分類" width="200">
            <template #default="{ row }">
              <el-select
                v-model="row.BtypeSId"
                placeholder="請選擇"
                clearable
                :disabled="!row.BtypeId"
                style="width: 100%"
              >
                <el-option
                  v-for="item in getBusinessTypeMinorList(row.BtypeId)"
                  :key="item.BtypeSId"
                  :label="item.BtypeSName"
                  :value="item.BtypeSId"
                />
              </el-select>
            </template>
          </el-table-column>
          <el-table-column label="操作" width="100" align="center">
            <template #default="{ $index }">
              <el-button type="danger" size="small" @click="removeBusinessType($index)">刪除</el-button>
            </template>
          </el-table-column>
        </el-table>
        <el-button type="primary" size="small" @click="addBusinessType">新增業種</el-button>

        <!-- 組織權限設定區塊 -->
        <el-divider>組織權限設定</el-divider>
        <el-table :data="form.Organizations" border style="margin-bottom: 10px">
          <el-table-column type="index" label="序號" width="60" align="center" />
          <el-table-column label="組織" width="200">
            <template #default="{ row }">
              <el-select v-model="row.OrgId" placeholder="請選擇" clearable style="width: 100%">
                <el-option
                  v-for="item in organizationList"
                  :key="item.OrgId"
                  :label="item.OrgName"
                  :value="item.OrgId"
                />
              </el-select>
            </template>
          </el-table-column>
          <el-table-column label="操作" width="100" align="center">
            <template #default="{ $index }">
              <el-button type="danger" size="small" @click="removeOrganization($index)">刪除</el-button>
            </template>
          </el-table-column>
        </el-table>
        <el-button type="primary" size="small" @click="addOrganization">新增組織</el-button>
      </el-form>
      <template #footer>
        <el-button @click="handleDialogClose">取消</el-button>
        <el-button type="primary" @click="handleSubmit">確定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import {
  getUsers,
  getUserDetailWithAdOrgs,
  createUserWithAdOrgs,
  updateUserWithAdOrgs,
  deleteUser,
  getOrganizations,
  validateAdUser,
  getBusinessTypeMajors,
  getBusinessTypeMiddles,
  getBusinessTypeMinors
} from '@/api/users'

// 查詢表單
const queryForm = reactive({
  UserId: '',
  UserName: '',
  OrgId: '',
  Status: ''
})

// 表格資料
const tableData = ref([])
const loading = ref(false)
const selectedRows = ref([])

// 分頁
const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 對話框
const dialogVisible = ref(false)
const dialogTitle = ref('新增使用者')
const isEdit = ref(false)
const formRef = ref(null)

// 表單資料
const form = reactive({
  UserId: '',
  UserName: '',
  UserPassword: '',
  Title: '',
  OrgId: '',
  StartDate: null,
  EndDate: null,
  Status: 'A',
  UserType: '',
  UserPriority: 0,
  Notes: '',
  UseActiveDirectory: false,
  AdDomain: '',
  AdUserPrincipalName: '',
  BusinessTypes: [],
  Organizations: []
})

// 下拉選單資料
const organizationList = ref([])
const businessTypeMajorList = ref([])
const businessTypeMiddleCache = ref({}) // 中分類快取 { btypeMId: [list] }
const businessTypeMinorCache = ref({}) // 小分類快取 { btypeId: [list] }

// AD 驗證結果
const adValidationResult = ref(null)

// 表單驗證規則
const rules = {
  UserId: [
    { required: true, message: '請輸入使用者編號', trigger: 'blur' },
    { max: 50, message: '使用者編號長度不能超過50個字元', trigger: 'blur' }
  ],
  UserName: [
    { required: true, message: '請輸入使用者名稱', trigger: 'blur' },
    { max: 100, message: '使用者名稱長度不能超過100個字元', trigger: 'blur' }
  ],
  UserPassword: [
    {
      required: true,
      message: '請輸入密碼',
      trigger: 'blur',
      validator: (rule, value, callback) => {
        if (!isEdit.value && !form.UseActiveDirectory && !value) {
          callback(new Error('請輸入密碼'))
        } else {
          callback()
        }
      }
    }
  ],
  Status: [{ required: true, message: '請選擇狀態', trigger: 'change' }],
  AdDomain: [
    {
      required: true,
      message: '請輸入AD網域',
      trigger: 'blur',
      validator: (rule, value, callback) => {
        if (form.UseActiveDirectory && !value) {
          callback(new Error('請輸入AD網域'))
        } else {
          callback()
        }
      }
    }
  ],
  AdUserPrincipalName: [
    {
      required: true,
      message: '請輸入AD使用者主體名稱',
      trigger: 'blur',
      validator: (rule, value, callback) => {
        if (form.UseActiveDirectory && !value) {
          callback(new Error('請輸入AD使用者主體名稱'))
        } else {
          callback()
        }
      }
    }
  ]
}

// 取得狀態類型
const getStatusType = (status) => {
  switch (status) {
    case 'A':
      return 'success'
    case 'I':
      return 'danger'
    case 'L':
      return 'warning'
    default:
      return 'info'
  }
}

// 取得狀態文字
const getStatusText = (status) => {
  switch (status) {
    case 'A':
      return '啟用'
    case 'I':
      return '停用'
    case 'L':
      return '鎖定'
    default:
      return status
  }
}

// 載入下拉選單資料
const loadDropdownData = async () => {
  try {
    // 載入組織
    const orgResponse = await getOrganizations()
    if (orgResponse && orgResponse.Data) {
      organizationList.value = orgResponse.Data
    }

    // 載入業種大分類
    const majorResponse = await getBusinessTypeMajors()
    if (majorResponse && majorResponse.Data) {
      businessTypeMajorList.value = majorResponse.Data
    }
  } catch (error) {
    console.error('載入下拉選單資料失敗:', error)
  }
}

// 取得業種中分類列表
const getBusinessTypeMiddleList = (btypeMId) => {
  if (!btypeMId) return []
  if (businessTypeMiddleCache.value[btypeMId]) {
    return businessTypeMiddleCache.value[btypeMId]
  }
  return []
}

// 取得業種小分類列表
const getBusinessTypeMinorList = (btypeId) => {
  if (!btypeId) return []
  if (businessTypeMinorCache.value[btypeId]) {
    return businessTypeMinorCache.value[btypeId]
  }
  return []
}

// AD 變更處理
const handleAdChange = (value) => {
  if (value) {
    // 使用 AD 時，清空密碼
    form.UserPassword = ''
    adValidationResult.value = null
  } else {
    // 不使用 AD 時，清空 AD 相關欄位
    form.AdDomain = ''
    form.AdUserPrincipalName = ''
    adValidationResult.value = null
  }
}

// 驗證 AD 使用者
const validateAdUser = async () => {
  if (!form.AdDomain || !form.AdUserPrincipalName) {
    ElMessage.warning('請先輸入AD網域和使用者主體名稱')
    return
  }

  try {
    const response = await validateAdUser({
      AdDomain: form.AdDomain,
      AdUserPrincipalName: form.AdUserPrincipalName
    })

    if (response && response.Data) {
      adValidationResult.value = response.Data
      if (response.Data.Exists) {
        ElMessage.success('AD使用者驗證成功')
        // 自動填入使用者名稱（如果為空）
        if (!form.UserName && response.Data.DisplayName) {
          form.UserName = response.Data.DisplayName
        }
      } else {
        ElMessage.error('AD使用者不存在')
      }
    }
  } catch (error) {
    ElMessage.error('驗證AD使用者失敗: ' + (error.message || '未知錯誤'))
    adValidationResult.value = { Exists: false }
  }
}

// 業種大分類變更
const handleBtypeMChange = async (index) => {
  const row = form.BusinessTypes[index]
  // 清空中分類和小分類
  row.BtypeId = null
  row.BtypeSId = null

  if (row.BtypeMId) {
    // 載入中分類
    try {
      if (!businessTypeMiddleCache.value[row.BtypeMId]) {
        const response = await getBusinessTypeMiddles(row.BtypeMId)
        if (response && response.Data) {
          businessTypeMiddleCache.value[row.BtypeMId] = response.Data
        } else {
          businessTypeMiddleCache.value[row.BtypeMId] = []
        }
      }
    } catch (error) {
      console.error('載入業種中分類列表失敗:', error)
      businessTypeMiddleCache.value[row.BtypeMId] = []
    }
  }
}

// 業種中分類變更
const handleBtypeChange = async (index) => {
  const row = form.BusinessTypes[index]
  // 清空小分類
  row.BtypeSId = null

  if (row.BtypeId) {
    // 載入小分類
    try {
      if (!businessTypeMinorCache.value[row.BtypeId]) {
        const response = await getBusinessTypeMinors(row.BtypeId)
        if (response && response.Data) {
          businessTypeMinorCache.value[row.BtypeId] = response.Data
        } else {
          businessTypeMinorCache.value[row.BtypeId] = []
        }
      }
    } catch (error) {
      console.error('載入業種小分類列表失敗:', error)
      businessTypeMinorCache.value[row.BtypeId] = []
    }
  }
}

// 新增業種
const addBusinessType = () => {
  form.BusinessTypes.push({
    BtypeMId: null,
    BtypeId: null,
    BtypeSId: null // 前端使用 BtypeSId，后端映射到 PtypeId
  })
}

// 刪除業種
const removeBusinessType = (index) => {
  form.BusinessTypes.splice(index, 1)
}

// 新增組織
const addOrganization = () => {
  form.Organizations.push({
    OrgId: ''
  })
}

// 刪除組織
const removeOrganization = (index) => {
  form.Organizations.splice(index, 1)
}

// 載入資料
const loadData = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      UserId: queryForm.UserId || undefined,
      UserName: queryForm.UserName || undefined,
      OrgId: queryForm.OrgId || undefined,
      Status: queryForm.Status || undefined
    }
    const response = await getUsers(params)
    if (response && response.Data) {
      tableData.value = response.Data.Items || []
      pagination.TotalCount = response.Data.TotalCount || 0
    }
  } catch (error) {
    ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
  } finally {
    loading.value = false
  }
}

// 查詢
const handleSearch = () => {
  pagination.PageIndex = 1
  loadData()
}

// 重置
const handleReset = () => {
  Object.assign(queryForm, {
    UserId: '',
    UserName: '',
    OrgId: '',
    Status: ''
  })
  handleSearch()
}

// 新增
const handleCreate = () => {
  isEdit.value = false
  dialogTitle.value = '新增使用者'
  Object.assign(form, {
    UserId: '',
    UserName: '',
    UserPassword: '',
    Title: '',
    OrgId: '',
    StartDate: null,
    EndDate: null,
    Status: 'A',
    UserType: '',
    UserPriority: 0,
    Notes: '',
    UseActiveDirectory: false,
    AdDomain: '',
    AdUserPrincipalName: '',
    BusinessTypes: [],
    Organizations: []
  })
  adValidationResult.value = null
  dialogVisible.value = true
}

// 修改
const handleEdit = async (row) => {
  isEdit.value = true
  dialogTitle.value = '修改使用者'
  try {
    const response = await getUserDetailWithAdOrgs(row.UserId)
    if (response && response.Data) {
      const data = response.Data
      Object.assign(form, {
        UserId: data.UserId,
        UserName: data.UserName,
        UserPassword: '', // 不顯示密碼
        Title: data.Title || '',
        OrgId: data.OrgId || '',
        StartDate: data.StartDate || null,
        EndDate: data.EndDate || null,
        Status: data.Status || 'A',
        UserType: data.UserType || '',
        UserPriority: data.UserPriority || 0,
        Notes: data.Notes || '',
        UseActiveDirectory: data.UseActiveDirectory || false,
        AdDomain: data.AdDomain || '',
        AdUserPrincipalName: data.AdUserPrincipalName || '',
        BusinessTypes: (data.BusinessTypes || []).map((bt) => ({
          BtypeMId: bt.BtypeMId,
          BtypeId: bt.BtypeId,
          BtypeSId: bt.BtypeSId
        })),
        Organizations: (data.Organizations || []).map((org) => ({
          OrgId: org.OrgId
        }))
      })

      // 載入相關的下拉選單資料
      for (const bt of form.BusinessTypes) {
        if (bt.BtypeMId) {
          await handleBtypeMChange(form.BusinessTypes.indexOf(bt))
        }
        if (bt.BtypeId) {
          await handleBtypeChange(form.BusinessTypes.indexOf(bt))
        }
      }

      adValidationResult.value = null
      dialogVisible.value = true
    }
  } catch (error) {
    ElMessage.error('載入資料失敗: ' + (error.message || '未知錯誤'))
  }
}

// 刪除
const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm(`確定要刪除使用者「${row.UserName}」嗎？`, '確認刪除', {
      type: 'warning'
    })
    await deleteUser(row.UserId)
    ElMessage.success('刪除成功')
    loadData()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
    }
  }
}

// 選擇變更
const handleSelectionChange = (selection) => {
  selectedRows.value = selection
}

// 提交
const handleSubmit = async () => {
  try {
    await formRef.value.validate()

    const submitData = {
      UserId: form.UserId,
      UserName: form.UserName,
      UserPassword: form.UseActiveDirectory ? undefined : form.UserPassword,
      Title: form.Title,
      OrgId: form.OrgId,
      StartDate: form.StartDate || undefined,
      EndDate: form.EndDate || undefined,
      Status: form.Status,
      UserType: form.UserType,
      UserPriority: form.UserPriority,
      Notes: form.Notes,
      UseActiveDirectory: form.UseActiveDirectory,
      AdDomain: form.UseActiveDirectory ? form.AdDomain : undefined,
      AdUserPrincipalName: form.UseActiveDirectory ? form.AdUserPrincipalName : undefined,
      BusinessTypes: form.BusinessTypes
        .filter((bt) => bt.BtypeMId || bt.BtypeId || bt.BtypeSId)
        .map((bt) => ({
          BtypeMId: bt.BtypeMId || undefined,
          BtypeId: bt.BtypeId || undefined,
          PtypeId: bt.BtypeSId || undefined
        })),
      Organizations: form.Organizations.filter((org) => org.OrgId).map((org) => ({ OrgId: org.OrgId }))
    }

    if (isEdit.value) {
      await updateUserWithAdOrgs(form.UserId, submitData)
      ElMessage.success('修改成功')
    } else {
      await createUserWithAdOrgs(submitData)
      ElMessage.success('新增成功')
    }
    dialogVisible.value = false
    loadData()
  } catch (error) {
    if (error !== false) {
      ElMessage.error((isEdit.value ? '修改' : '新增') + '失敗: ' + (error.message || '未知錯誤'))
    }
  }
}

// 關閉對話框
const handleDialogClose = () => {
  formRef.value?.resetFields()
  dialogVisible.value = false
  adValidationResult.value = null
}

// 分頁大小變更
const handleSizeChange = (size) => {
  pagination.PageSize = size
  pagination.PageIndex = 1
  loadData()
}

// 分頁變更
const handlePageChange = (page) => {
  pagination.PageIndex = page
  loadData()
}

// 初始化
onMounted(() => {
  loadDropdownData()
  loadData()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';
@import '@/assets/styles/base.scss';

.sys0114-query {
  .page-header {
    h1 {
      color: $primary-color;
    }
  }

  .search-card {
    margin-bottom: 20px;
    background-color: $card-bg;
    border-color: $card-border;
    color: $card-text;
  }

  .table-card {
    margin-bottom: 20px;
    background-color: $card-bg;
    border-color: $card-border;
    color: $card-text;
  }
}
</style>
