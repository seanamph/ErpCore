<template>
  <div class="sys0113-query">
    <div class="page-header">
      <h1>使用者基本資料維護(分店廠商部門) (SYS0113)</h1>
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
        <el-table-column prop="UserType" label="使用者型態" width="120" />
        <el-table-column label="操作" width="250" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleEdit(row)">修改</el-button>
            <el-button type="warning" size="small" @click="handleResetPassword(row)">重設密碼</el-button>
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
            <el-form-item label="密碼" prop="UserPassword" v-if="!isEdit">
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

        <!-- 總公司/分店權限設定區塊 -->
        <el-divider>總公司/分店權限設定</el-divider>
        <el-table :data="form.Shops" border style="margin-bottom: 10px">
          <el-table-column type="index" label="序號" width="60" align="center" />
          <el-table-column label="總公司" width="200">
            <template #default="{ row, $index }">
              <el-select
                v-model="row.PShopId"
                placeholder="請選擇"
                clearable
                style="width: 100%"
                @change="handlePShopChange($index)"
              >
                <el-option label="全部" value="xx" />
                <el-option
                  v-for="item in pShopList"
                  :key="item.PShopId"
                  :label="item.PShopName"
                  :value="item.PShopId"
                />
              </el-select>
            </template>
          </el-table-column>
          <el-table-column label="分店" width="200">
            <template #default="{ row, $index }">
              <el-select
                v-model="row.ShopId"
                placeholder="請選擇"
                clearable
                :disabled="!row.PShopId || row.PShopId === 'xx'"
                style="width: 100%"
                @change="handleShopChange($index)"
              >
                <el-option
                  v-for="item in getShopList(row.PShopId)"
                  :key="item.ShopId"
                  :label="item.ShopName"
                  :value="item.ShopId"
                />
              </el-select>
            </template>
          </el-table-column>
          <el-table-column label="據點" width="200">
            <template #default="{ row }">
              <el-select
                v-model="row.SiteId"
                placeholder="請選擇"
                clearable
                :disabled="!row.ShopId"
                style="width: 100%"
              >
                <el-option
                  v-for="item in getSiteList(row.ShopId)"
                  :key="item.SiteId"
                  :label="item.SiteName"
                  :value="item.SiteId"
                />
              </el-select>
            </template>
          </el-table-column>
          <el-table-column label="操作" width="100" align="center">
            <template #default="{ $index }">
              <el-button type="danger" size="small" @click="removeShop($index)">刪除</el-button>
            </template>
          </el-table-column>
        </el-table>
        <el-button type="primary" size="small" @click="addShop">新增總公司/分店</el-button>

        <!-- 廠商權限設定區塊 -->
        <el-divider>廠商權限設定</el-divider>
        <el-table :data="form.Vendors" border style="margin-bottom: 10px">
          <el-table-column type="index" label="序號" width="60" align="center" />
          <el-table-column label="廠商" width="200">
            <template #default="{ row }">
              <el-select v-model="row.VendorId" placeholder="請選擇" clearable style="width: 100%">
                <el-option
                  v-for="item in vendorList"
                  :key="item.VendorId"
                  :label="item.VendorName"
                  :value="item.VendorId"
                />
              </el-select>
            </template>
          </el-table-column>
          <el-table-column label="操作" width="100" align="center">
            <template #default="{ $index }">
              <el-button type="danger" size="small" @click="removeVendor($index)">刪除</el-button>
            </template>
          </el-table-column>
        </el-table>
        <el-button type="primary" size="small" @click="addVendor">新增廠商</el-button>

        <!-- 部門權限設定區塊 -->
        <el-divider>部門權限設定</el-divider>
        <el-table :data="form.Departments" border style="margin-bottom: 10px">
          <el-table-column type="index" label="序號" width="60" align="center" />
          <el-table-column label="部門" width="200">
            <template #default="{ row }">
              <el-select v-model="row.DeptId" placeholder="請選擇" clearable style="width: 100%">
                <el-option
                  v-for="item in departmentList"
                  :key="item.DeptId"
                  :label="item.DeptName"
                  :value="item.DeptId"
                />
              </el-select>
            </template>
          </el-table-column>
          <el-table-column label="操作" width="100" align="center">
            <template #default="{ $index }">
              <el-button type="danger" size="small" @click="removeDepartment($index)">刪除</el-button>
            </template>
          </el-table-column>
        </el-table>
        <el-button type="primary" size="small" @click="addDepartment">新增部門</el-button>

        <!-- 按鈕權限設定區塊 -->
        <el-divider>按鈕權限設定</el-divider>
        <el-table :data="form.Buttons" border style="margin-bottom: 10px">
          <el-table-column type="index" label="序號" width="60" align="center" />
          <el-table-column label="按鈕" width="200">
            <template #default="{ row }">
              <el-select v-model="row.ButtonId" placeholder="請選擇" clearable style="width: 100%">
                <el-option
                  v-for="item in buttonList"
                  :key="item.ButtonId"
                  :label="item.ButtonName"
                  :value="item.ButtonId"
                />
              </el-select>
            </template>
          </el-table-column>
          <el-table-column label="操作" width="100" align="center">
            <template #default="{ $index }">
              <el-button type="danger" size="small" @click="removeButton($index)">刪除</el-button>
            </template>
          </el-table-column>
        </el-table>
        <el-button type="primary" size="small" @click="addButton">新增按鈕</el-button>
      </el-form>
      <template #footer>
        <el-button @click="handleDialogClose">取消</el-button>
        <el-button type="primary" @click="handleSubmit">確定</el-button>
      </template>
    </el-dialog>

    <!-- 重設密碼對話框 -->
    <el-dialog
      title="重設密碼"
      v-model="resetPasswordDialogVisible"
      width="500px"
      @close="handleResetPasswordDialogClose"
    >
      <el-form :model="resetPasswordForm" :rules="resetPasswordRules" ref="resetPasswordFormRef" label-width="120px">
        <el-form-item label="使用者編號">
          <el-input v-model="resetPasswordUserId" disabled />
        </el-form-item>
        <el-form-item label="自動產生密碼">
          <el-switch v-model="resetPasswordForm.AutoGenerate" @change="handleAutoGenerateChange" />
        </el-form-item>
        <el-form-item label="新密碼" prop="NewPassword" v-if="!resetPasswordForm.AutoGenerate">
          <el-input v-model="resetPasswordForm.NewPassword" type="password" placeholder="請輸入新密碼" show-password />
        </el-form-item>
        <el-form-item label="確認密碼" prop="ConfirmPassword" v-if="!resetPasswordForm.AutoGenerate">
          <el-input v-model="resetPasswordForm.ConfirmPassword" type="password" placeholder="請再次輸入密碼" show-password />
        </el-form-item>
        <el-form-item v-if="resetPasswordForm.AutoGenerate && resetPasswordResult">
          <el-alert
            :title="`新密碼: ${resetPasswordResult.NewPassword}`"
            type="success"
            :closable="false"
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="handleResetPasswordDialogClose">取消</el-button>
        <el-button type="primary" @click="handleResetPasswordSubmit">確定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import {
  getUsers,
  getUserDetail,
  createUserWithShopsVendorsDepts,
  updateUserWithShopsVendorsDepts,
  deleteUser,
  getParentShops,
  getShops,
  getSites,
  getVendors,
  getDepartments,
  resetPasswordWithAuto
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
  Shops: [],
  Vendors: [],
  Departments: [],
  Buttons: []
})

// 下拉選單資料
const pShopList = ref([])
const shopCache = ref({}) // 分店快取 { pShopId: [list] }
const siteCache = ref({}) // 據點快取 { shopId: [list] }
const vendorList = ref([])
const departmentList = ref([])
const buttonList = ref([]) // TODO: 需要實作按鈕列表查詢

// 重設密碼對話框
const resetPasswordDialogVisible = ref(false)
const resetPasswordFormRef = ref(null)
const resetPasswordUserId = ref('')
const resetPasswordForm = reactive({
  AutoGenerate: false,
  NewPassword: '',
  ConfirmPassword: ''
})
const resetPasswordResult = ref(null)

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
        if (!isEdit.value && !value) {
          callback(new Error('請輸入密碼'))
        } else {
          callback()
        }
      }
    }
  ],
  Status: [{ required: true, message: '請選擇狀態', trigger: 'change' }]
}

// 重設密碼驗證規則
const resetPasswordRules = {
  NewPassword: [
    { required: true, message: '請輸入新密碼', trigger: 'blur' },
    { min: 6, message: '密碼長度不能少於6個字元', trigger: 'blur' }
  ],
  ConfirmPassword: [
    { required: true, message: '請再次輸入密碼', trigger: 'blur' },
    {
      validator: (rule, value, callback) => {
        if (value !== resetPasswordForm.NewPassword) {
          callback(new Error('兩次輸入的密碼不一致'))
        } else {
          callback()
        }
      },
      trigger: 'blur'
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
    // 載入總公司
    const pShopResponse = await getParentShops()
    if (pShopResponse && pShopResponse.Data) {
      pShopList.value = pShopResponse.Data
    }

    // 載入廠商
    const vendorResponse = await getVendors()
    if (vendorResponse && vendorResponse.Data) {
      vendorList.value = vendorResponse.Data
    }

    // 載入部門
    const departmentResponse = await getDepartments()
    if (departmentResponse && departmentResponse.Data) {
      departmentList.value = departmentResponse.Data
    }

    // TODO: 載入按鈕列表
    buttonList.value = []
  } catch (error) {
    console.error('載入下拉選單資料失敗:', error)
  }
}

// 取得分店列表
const getShopList = (pShopId) => {
  if (!pShopId || pShopId === 'xx') return []
  if (shopCache.value[pShopId]) {
    return shopCache.value[pShopId]
  }
  return []
}

// 取得據點列表
const getSiteList = (shopId) => {
  if (!shopId) return []
  if (siteCache.value[shopId]) {
    return siteCache.value[shopId]
  }
  return []
}

// 總公司變更
const handlePShopChange = async (index) => {
  const row = form.Shops[index]
  // 清空分店和據點
  row.ShopId = null
  row.SiteId = null

  if (row.PShopId && row.PShopId !== 'xx') {
    // 載入分店
    try {
      if (!shopCache.value[row.PShopId]) {
        const response = await getShops(row.PShopId)
        if (response && response.Data) {
          shopCache.value[row.PShopId] = response.Data
        } else {
          shopCache.value[row.PShopId] = []
        }
      }
    } catch (error) {
      console.error('載入分店列表失敗:', error)
      shopCache.value[row.PShopId] = []
    }
  }
}

// 分店變更
const handleShopChange = async (index) => {
  const row = form.Shops[index]
  // 清空據點
  row.SiteId = null

  if (row.ShopId) {
    // 載入據點
    try {
      if (!siteCache.value[row.ShopId]) {
        const response = await getSites(row.ShopId)
        if (response && response.Data) {
          siteCache.value[row.ShopId] = response.Data
        } else {
          siteCache.value[row.ShopId] = []
        }
      }
    } catch (error) {
      console.error('載入據點列表失敗:', error)
      siteCache.value[row.ShopId] = []
    }
  }
}

// 自動產生密碼變更
const handleAutoGenerateChange = (value) => {
  if (value) {
    resetPasswordForm.NewPassword = ''
    resetPasswordForm.ConfirmPassword = ''
  }
}

// 新增總公司/分店
const addShop = () => {
  form.Shops.push({
    PShopId: null,
    ShopId: null,
    SiteId: null
  })
}

// 刪除總公司/分店
const removeShop = (index) => {
  form.Shops.splice(index, 1)
}

// 新增廠商
const addVendor = () => {
  form.Vendors.push({
    VendorId: ''
  })
}

// 刪除廠商
const removeVendor = (index) => {
  form.Vendors.splice(index, 1)
}

// 新增部門
const addDepartment = () => {
  form.Departments.push({
    DeptId: ''
  })
}

// 刪除部門
const removeDepartment = (index) => {
  form.Departments.splice(index, 1)
}

// 新增按鈕
const addButton = () => {
  form.Buttons.push({
    ButtonId: ''
  })
}

// 刪除按鈕
const removeButton = (index) => {
  form.Buttons.splice(index, 1)
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
    Shops: [],
    Vendors: [],
    Departments: [],
    Buttons: []
  })
  dialogVisible.value = true
}

// 修改
const handleEdit = async (row) => {
  isEdit.value = true
  dialogTitle.value = '修改使用者'
  try {
    const response = await getUserDetail(row.UserId)
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
        Shops: (data.Shops || []).map((s) => ({
          PShopId: s.PShopId,
          ShopId: s.ShopId,
          SiteId: s.SiteId
        })),
        Vendors: (data.Vendors || []).map((v) => ({
          VendorId: v.VendorId
        })),
        Departments: (data.Departments || []).map((d) => ({
          DeptId: d.DeptId
        })),
        Buttons: (data.Buttons || []).map((b) => ({
          ButtonId: b.ButtonId
        }))
      })

      // 載入相關的下拉選單資料
      for (const shop of form.Shops) {
        if (shop.PShopId && shop.PShopId !== 'xx') {
          await handlePShopChange(form.Shops.indexOf(shop))
        }
        if (shop.ShopId) {
          await handleShopChange(form.Shops.indexOf(shop))
        }
      }

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

// 重設密碼
const handleResetPassword = (row) => {
  resetPasswordUserId.value = row.UserId
  resetPasswordForm.AutoGenerate = false
  resetPasswordForm.NewPassword = ''
  resetPasswordForm.ConfirmPassword = ''
  resetPasswordResult.value = null
  resetPasswordDialogVisible.value = true
}

// 重設密碼提交
const handleResetPasswordSubmit = async () => {
  try {
    if (!resetPasswordForm.AutoGenerate) {
      await resetPasswordFormRef.value.validate()
    }

    const response = await resetPasswordWithAuto(resetPasswordUserId.value, {
      AutoGenerate: resetPasswordForm.AutoGenerate,
      NewPassword: resetPasswordForm.NewPassword || undefined
    })

    if (response && response.Data) {
      resetPasswordResult.value = response.Data
      ElMessage.success('密碼重設成功')
      if (resetPasswordForm.AutoGenerate) {
        // 顯示新密碼
        ElMessageBox.alert(`新密碼: ${response.Data.NewPassword}`, '密碼重設成功', {
          type: 'success',
          confirmButtonText: '確定'
        })
      }
    }
  } catch (error) {
    if (error !== false) {
      ElMessage.error('重設密碼失敗: ' + (error.message || '未知錯誤'))
    }
  }
}

// 關閉重設密碼對話框
const handleResetPasswordDialogClose = () => {
  resetPasswordFormRef.value?.resetFields()
  resetPasswordDialogVisible.value = false
  resetPasswordResult.value = null
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
      UserPassword: form.UserPassword,
      Title: form.Title,
      OrgId: form.OrgId,
      StartDate: form.StartDate || undefined,
      EndDate: form.EndDate || undefined,
      Status: form.Status,
      UserType: form.UserType,
      UserPriority: form.UserPriority,
      Notes: form.Notes,
      Shops: form.Shops
        .filter((s) => s.PShopId || s.ShopId || s.SiteId)
        .map((s) => ({
          PShopId: s.PShopId || undefined,
          ShopId: s.ShopId || undefined,
          SiteId: s.SiteId || undefined
        })),
      Vendors: form.Vendors.filter((v) => v.VendorId).map((v) => ({ VendorId: v.VendorId })),
      Departments: form.Departments.filter((d) => d.DeptId).map((d) => ({ DeptId: d.DeptId })),
      Buttons: form.Buttons.filter((b) => b.ButtonId).map((b) => ({ ButtonId: b.ButtonId }))
    }

    if (isEdit.value) {
      await updateUserWithShopsVendorsDepts(form.UserId, submitData)
      ElMessage.success('修改成功')
    } else {
      await createUserWithShopsVendorsDepts(submitData)
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

.sys0113-query {
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
