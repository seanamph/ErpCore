<template>
  <div class="sys0111-query">
    <div class="page-header">
      <h1>使用者基本資料維護(業種儲位) (SYS0111)</h1>
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

        <!-- 業種權限設定區塊 -->
        <el-divider>業種權限設定</el-divider>
        <el-table :data="form.BusinessTypes" border style="margin-bottom: 10px">
          <el-table-column type="index" label="序號" width="60" align="center" />
          <el-table-column label="業種大分類" width="200">
            <template #default="{ row, $index }">
              <el-select
                v-model="row.BtypeMId"
                placeholder="請選擇"
                clearable
                style="width: 100%"
                @change="handleBtypeMChange($index)"
              >
                <el-option label="全部業種" value="xx" />
                <el-option
                  v-for="item in btypeMList"
                  :key="item.BtypeMId"
                  :label="item.BtypeMName"
                  :value="item.BtypeMId"
                />
              </el-select>
            </template>
          </el-table-column>
          <el-table-column label="業種中分類" width="200">
            <template #default="{ row, $index }">
              <el-select
                v-model="row.BtypeId"
                placeholder="請選擇"
                clearable
                :disabled="!row.BtypeMId || row.BtypeMId === 'xx'"
                style="width: 100%"
                @change="handleBtypeChange($index)"
              >
                <el-option
                  v-for="item in getBtypeList(row.BtypeMId)"
                  :key="item.BtypeId"
                  :label="item.BtypeName"
                  :value="item.BtypeId"
                />
              </el-select>
            </template>
          </el-table-column>
          <el-table-column label="業種小分類" width="200">
            <template #default="{ row }">
              <el-select
                v-model="row.PtypeId"
                placeholder="請選擇"
                clearable
                :disabled="!row.BtypeId"
                style="width: 100%"
              >
                <el-option
                  v-for="item in getPtypeList(row.BtypeId)"
                  :key="item.PtypeId"
                  :label="item.PtypeName"
                  :value="item.PtypeId"
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

        <!-- 儲位權限設定區塊 -->
        <el-divider>儲位權限設定</el-divider>
        <el-table :data="form.WarehouseAreas" border style="margin-bottom: 10px">
          <el-table-column type="index" label="序號" width="60" align="center" />
          <el-table-column label="儲位1" width="200">
            <template #default="{ row, $index }">
              <el-select
                v-model="row.WareaId1"
                placeholder="請選擇"
                clearable
                style="width: 100%"
                @change="handleWarea1Change($index)"
              >
                <el-option label="全部儲位" value="xx" />
                <el-option
                  v-for="item in warea1List"
                  :key="item.WareaId"
                  :label="item.WareaName"
                  :value="item.WareaId"
                />
              </el-select>
            </template>
          </el-table-column>
          <el-table-column label="儲位2" width="200">
            <template #default="{ row, $index }">
              <el-select
                v-model="row.WareaId2"
                placeholder="請選擇"
                clearable
                :disabled="!row.WareaId1 || row.WareaId1 === 'xx'"
                style="width: 100%"
                @change="handleWarea2Change($index)"
              >
                <el-option
                  v-for="item in getWarea2List(row.WareaId1)"
                  :key="item.WareaId"
                  :label="item.WareaName"
                  :value="item.WareaId"
                />
              </el-select>
            </template>
          </el-table-column>
          <el-table-column label="儲位3" width="200">
            <template #default="{ row }">
              <el-select
                v-model="row.WareaId3"
                placeholder="請選擇"
                clearable
                :disabled="!row.WareaId2"
                style="width: 100%"
              >
                <el-option
                  v-for="item in getWarea3List(row.WareaId2)"
                  :key="item.WareaId"
                  :label="item.WareaName"
                  :value="item.WareaId"
                />
              </el-select>
            </template>
          </el-table-column>
          <el-table-column label="操作" width="100" align="center">
            <template #default="{ $index }">
              <el-button type="danger" size="small" @click="removeWarehouseArea($index)">刪除</el-button>
            </template>
          </el-table-column>
        </el-table>
        <el-button type="primary" size="small" @click="addWarehouseArea">新增儲位</el-button>

        <!-- 7X承租分店權限設定區塊 -->
        <el-divider>7X承租分店權限設定</el-divider>
        <el-table :data="form.Stores" border style="margin-bottom: 10px">
          <el-table-column type="index" label="序號" width="60" align="center" />
          <el-table-column label="分店代號" width="200">
            <template #default="{ row }">
              <el-select v-model="row.StoreId" placeholder="請選擇" clearable style="width: 100%">
                <el-option
                  v-for="item in storeList"
                  :key="item.StoreId"
                  :label="item.StoreName"
                  :value="item.StoreId"
                />
              </el-select>
            </template>
          </el-table-column>
          <el-table-column label="操作" width="100" align="center">
            <template #default="{ $index }">
              <el-button type="danger" size="small" @click="removeStore($index)">刪除</el-button>
            </template>
          </el-table-column>
        </el-table>
        <el-button type="primary" size="small" @click="addStore">新增分店</el-button>
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
  getUserDetail,
  createUserWithBusinessTypes,
  updateUserWithBusinessTypes,
  deleteUser,
  getBusinessTypeMajors,
  getBusinessTypeMiddles,
  getBusinessTypeMinors,
  getWarehouseAreas,
  getStores
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
  BusinessTypes: [],
  WarehouseAreas: [],
  Stores: []
})

// 下拉選單資料
const btypeMList = ref([])
const btypeMiddleCache = ref({}) // 中分類快取 { btypeMId: [list] }
const btypeMinorCache = ref({}) // 小分類快取 { btypeId: [list] }
const warea1List = ref([])
const warea2Cache = ref({}) // 儲位2快取 { wareaId1: [list] }
const warea3Cache = ref({}) // 儲位3快取 { wareaId2: [list] }
const storeList = ref([])

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
    // 載入業種大分類
    const btypeMResponse = await getBusinessTypeMajors()
    if (btypeMResponse && btypeMResponse.Data) {
      btypeMList.value = btypeMResponse.Data
    }

    // 載入儲位1
    const warea1Response = await getWarehouseAreas(1)
    if (warea1Response && warea1Response.Data) {
      warea1List.value = warea1Response.Data
    }

    // 載入7X承租分店
    const storeResponse = await getStores()
    if (storeResponse && storeResponse.Data) {
      storeList.value = storeResponse.Data
    }
  } catch (error) {
    console.error('載入下拉選單資料失敗:', error)
  }
}

// 取得業種中分類列表
const getBtypeList = (btypeMId) => {
  if (!btypeMId || btypeMId === 'xx') return []
  if (btypeMiddleCache.value[btypeMId]) {
    return btypeMiddleCache.value[btypeMId]
  }
  return []
}

// 取得業種小分類列表
const getPtypeList = (btypeId) => {
  if (!btypeId) return []
  if (btypeMinorCache.value[btypeId]) {
    return btypeMinorCache.value[btypeId]
  }
  return []
}

// 取得儲位2列表
const getWarea2List = (wareaId1) => {
  if (!wareaId1 || wareaId1 === 'xx') return []
  if (warea2Cache.value[wareaId1]) {
    return warea2Cache.value[wareaId1]
  }
  return []
}

// 取得儲位3列表
const getWarea3List = (wareaId2) => {
  if (!wareaId2) return []
  if (warea3Cache.value[wareaId2]) {
    return warea3Cache.value[wareaId2]
  }
  return []
}

// 業種大分類變更
const handleBtypeMChange = async (index) => {
  const row = form.BusinessTypes[index]
  // 清空中分類和小分類
  row.BtypeId = null
  row.PtypeId = null

  if (row.BtypeMId && row.BtypeMId !== 'xx') {
    // 載入中分類
    try {
      if (!btypeMiddleCache.value[row.BtypeMId]) {
        const response = await getBusinessTypeMiddles(row.BtypeMId)
        if (response && response.Data) {
          btypeMiddleCache.value[row.BtypeMId] = response.Data
        } else {
          btypeMiddleCache.value[row.BtypeMId] = []
        }
      }
    } catch (error) {
      console.error('載入業種中分類失敗:', error)
      btypeMiddleCache.value[row.BtypeMId] = []
    }
  }
}

// 業種中分類變更
const handleBtypeChange = async (index) => {
  const row = form.BusinessTypes[index]
  // 清空小分類
  row.PtypeId = null

  if (row.BtypeId) {
    // 載入小分類
    try {
      if (!btypeMinorCache.value[row.BtypeId]) {
        const response = await getBusinessTypeMinors(row.BtypeId)
        if (response && response.Data) {
          btypeMinorCache.value[row.BtypeId] = response.Data
        } else {
          btypeMinorCache.value[row.BtypeId] = []
        }
      }
    } catch (error) {
      console.error('載入業種小分類失敗:', error)
      btypeMinorCache.value[row.BtypeId] = []
    }
  }
}

// 儲位1變更
const handleWarea1Change = async (index) => {
  const row = form.WarehouseAreas[index]
  // 清空儲位2和儲位3
  row.WareaId2 = null
  row.WareaId3 = null

  if (row.WareaId1 && row.WareaId1 !== 'xx') {
    // 載入儲位2
    try {
      if (!warea2Cache.value[row.WareaId1]) {
        const response = await getWarehouseAreas(2, row.WareaId1)
        if (response && response.Data) {
          warea2Cache.value[row.WareaId1] = response.Data
        } else {
          warea2Cache.value[row.WareaId1] = []
        }
      }
    } catch (error) {
      console.error('載入儲位2失敗:', error)
      warea2Cache.value[row.WareaId1] = []
    }
  }
}

// 儲位2變更
const handleWarea2Change = async (index) => {
  const row = form.WarehouseAreas[index]
  // 清空儲位3
  row.WareaId3 = null

  if (row.WareaId2) {
    // 載入儲位3
    try {
      if (!warea3Cache.value[row.WareaId2]) {
        const response = await getWarehouseAreas(3, row.WareaId2)
        if (response && response.Data) {
          warea3Cache.value[row.WareaId2] = response.Data
        } else {
          warea3Cache.value[row.WareaId2] = []
        }
      }
    } catch (error) {
      console.error('載入儲位3失敗:', error)
      warea3Cache.value[row.WareaId2] = []
    }
  }
}

// 新增業種
const addBusinessType = () => {
  form.BusinessTypes.push({
    BtypeMId: null,
    BtypeId: null,
    PtypeId: null
  })
}

// 刪除業種
const removeBusinessType = (index) => {
  form.BusinessTypes.splice(index, 1)
}

// 新增儲位
const addWarehouseArea = () => {
  form.WarehouseAreas.push({
    WareaId1: null,
    WareaId2: null,
    WareaId3: null
  })
}

// 刪除儲位
const removeWarehouseArea = (index) => {
  form.WarehouseAreas.splice(index, 1)
}

// 新增分店
const addStore = () => {
  form.Stores.push({
    StoreId: ''
  })
}

// 刪除分店
const removeStore = (index) => {
  form.Stores.splice(index, 1)
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
    BusinessTypes: [],
    WarehouseAreas: [],
    Stores: []
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
        BusinessTypes: (data.BusinessTypes || []).map((bt) => ({
          BtypeMId: bt.BtypeMId,
          BtypeId: bt.BtypeId,
          PtypeId: bt.PtypeId
        })),
        WarehouseAreas: (data.WarehouseAreas || []).map((wa) => ({
          WareaId1: wa.WareaId1,
          WareaId2: wa.WareaId2,
          WareaId3: wa.WareaId3
        })),
        Stores: (data.Stores || []).map((s) => ({
          StoreId: s.StoreId
        }))
      })

      // 載入相關的下拉選單資料
      for (const bt of form.BusinessTypes) {
        if (bt.BtypeMId && bt.BtypeMId !== 'xx') {
          await handleBtypeMChange(form.BusinessTypes.indexOf(bt))
        }
        if (bt.BtypeId) {
          await handleBtypeChange(form.BusinessTypes.indexOf(bt))
        }
      }

      for (const wa of form.WarehouseAreas) {
        if (wa.WareaId1 && wa.WareaId1 !== 'xx') {
          await handleWarea1Change(form.WarehouseAreas.indexOf(wa))
        }
        if (wa.WareaId2) {
          await handleWarea2Change(form.WarehouseAreas.indexOf(wa))
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
      BusinessTypes: form.BusinessTypes
        .filter((bt) => bt.BtypeMId || bt.BtypeId || bt.PtypeId)
        .map((bt) => ({
          BtypeMId: bt.BtypeMId || undefined,
          BtypeId: bt.BtypeId || undefined,
          PtypeId: bt.PtypeId || undefined
        })),
      WarehouseAreas: form.WarehouseAreas
        .filter((wa) => wa.WareaId1 || wa.WareaId2 || wa.WareaId3)
        .map((wa) => ({
          WareaId1: wa.WareaId1 || undefined,
          WareaId2: wa.WareaId2 || undefined,
          WareaId3: wa.WareaId3 || undefined
        })),
      Stores: form.Stores.filter((s) => s.StoreId).map((s) => ({ StoreId: s.StoreId }))
    }

    if (isEdit.value) {
      await updateUserWithBusinessTypes(form.UserId, submitData)
      ElMessage.success('修改成功')
    } else {
      await createUserWithBusinessTypes(submitData)
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

.sys0111-query {
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
