<template>
  <div class="product-category">
    <div class="page-header">
      <h1>商品分類資料維護 (SYSB110)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="分類代碼">
          <el-input v-model="queryForm.ClassId" placeholder="請輸入分類代碼" clearable />
        </el-form-item>
        <el-form-item label="分類名稱">
          <el-input v-model="queryForm.ClassName" placeholder="請輸入分類名稱" clearable />
        </el-form-item>
        <el-form-item label="分類區分">
          <el-select v-model="queryForm.ClassMode" placeholder="請選擇分類區分" clearable>
            <el-option label="全部" value="" />
            <el-option label="大分類" value="1" />
            <el-option label="中分類" value="2" />
            <el-option label="小分類" value="3" />
          </el-select>
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="全部" value="" />
            <el-option label="啟用" value="A" />
            <el-option label="停用" value="I" />
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
      >
        <el-table-column prop="ClassId" label="分類代碼" width="120" />
        <el-table-column prop="ClassName" label="分類名稱" width="200" />
        <el-table-column prop="ClassMode" label="分類區分" width="100">
          <template #default="{ row }">
            {{ row.ClassMode === '1' ? '大分類' : row.ClassMode === '2' ? '中分類' : row.ClassMode === '3' ? '小分類' : '' }}
          </template>
        </el-table-column>
        <el-table-column prop="ClassType" label="分類型式" width="100" />
        <el-table-column prop="BClassId" label="大分類" width="100" />
        <el-table-column prop="MClassId" label="中分類" width="100" />
        <el-table-column prop="ItemCount" label="項目個數" width="100" align="right" />
        <el-table-column prop="Status" label="狀態" width="80">
          <template #default="{ row }">
            <el-tag :type="row.Status === 'A' ? 'success' : 'danger'">
              {{ row.Status === 'A' ? '啟用' : '停用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="Notes" label="備註" min-width="200" show-overflow-tooltip />
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
      width="700px"
      @close="handleDialogClose"
    >
      <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
        <el-form-item label="分類代碼" prop="ClassId">
          <el-input v-model="form.ClassId" placeholder="請輸入分類代碼" :disabled="isEdit" />
        </el-form-item>
        <el-form-item label="分類名稱" prop="ClassName">
          <el-input v-model="form.ClassName" placeholder="請輸入分類名稱" />
        </el-form-item>
        <el-form-item label="分類區分" prop="ClassMode">
          <el-select v-model="form.ClassMode" placeholder="請選擇分類區分" style="width: 100%" :disabled="isEdit">
            <el-option label="大分類" value="1" />
            <el-option label="中分類" value="2" />
            <el-option label="小分類" value="3" />
          </el-select>
        </el-form-item>
        <el-form-item label="分類型式" prop="ClassType">
          <el-select v-model="form.ClassType" placeholder="請選擇分類型式" style="width: 100%">
            <el-option label="資料" value="1" />
            <el-option label="耗材" value="2" />
          </el-select>
        </el-form-item>
        <el-form-item label="大分類" prop="BClassId" v-if="form.ClassMode === '2' || form.ClassMode === '3'">
          <el-select v-model="form.BClassId" placeholder="請選擇大分類" style="width: 100%" filterable @change="handleBClassChange">
            <el-option v-for="item in bClassList" :key="item.ClassId" :label="`${item.ClassId} - ${item.ClassName}`" :value="item.ClassId" />
          </el-select>
        </el-form-item>
        <el-form-item label="中分類" prop="MClassId" v-if="form.ClassMode === '3'">
          <el-select v-model="form.MClassId" placeholder="請選擇中分類" style="width: 100%" filterable :disabled="!form.BClassId">
            <el-option v-for="item in mClassList" :key="item.ClassId" :label="`${item.ClassId} - ${item.ClassName}`" :value="item.ClassId" />
          </el-select>
        </el-form-item>
        <el-form-item label="所屬會計科目(借)" prop="StypeId">
          <el-input v-model="form.StypeId" placeholder="請輸入會計科目代碼" />
        </el-form-item>
        <el-form-item label="所屬會計科目(貸)" prop="StypeId2">
          <el-input v-model="form.StypeId2" placeholder="請輸入會計科目代碼" />
        </el-form-item>
        <el-form-item label="折舊科目(借)" prop="DepreStypeId">
          <el-input v-model="form.DepreStypeId" placeholder="請輸入會計科目代碼" />
        </el-form-item>
        <el-form-item label="累計折舊科目(貸)" prop="DepreStypeId2">
          <el-input v-model="form.DepreStypeId2" placeholder="請輸入會計科目代碼" />
        </el-form-item>
        <el-form-item label="進項稅額科目(借)" prop="StypeTax">
          <el-input v-model="form.StypeTax" placeholder="請輸入會計科目代碼" />
        </el-form-item>
        <el-form-item label="項目個數" prop="ItemCount">
          <el-input-number v-model="form.ItemCount" :min="0" :disabled="true" style="width: 100%" />
        </el-form-item>
        <el-form-item label="狀態" prop="Status">
          <el-select v-model="form.Status" placeholder="請選擇狀態" style="width: 100%">
            <el-option label="啟用" value="A" />
            <el-option label="停用" value="I" />
          </el-select>
        </el-form-item>
        <el-form-item label="備註" prop="Notes">
          <el-input v-model="form.Notes" type="textarea" :rows="3" placeholder="請輸入備註" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="handleDialogClose">取消</el-button>
        <el-button type="primary" @click="handleSubmit">確定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, watch } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { productCategoriesApi } from '@/api/productCategories'

// 查詢表單
const queryForm = reactive({
  ClassId: '',
  ClassName: '',
  ClassMode: '',
  Status: ''
})

// 表格資料
const tableData = ref([])
const loading = ref(false)

// 分頁
const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 對話框
const dialogVisible = ref(false)
const dialogTitle = ref('新增商品分類')
const isEdit = ref(false)
const formRef = ref(null)

// 表單資料
const form = reactive({
  ClassId: '',
  ClassName: '',
  ClassMode: '',
  ClassType: '1',
  BClassId: '',
  MClassId: '',
  ParentTKey: null,
  StypeId: '',
  StypeId2: '',
  DepreStypeId: '',
  DepreStypeId2: '',
  StypeTax: '',
  ItemCount: 0,
  Status: 'A',
  Notes: ''
})

// 大分類列表
const bClassList = ref([])
// 中分類列表
const mClassList = ref([])

// 表單驗證規則
const rules = {
  ClassId: [{ required: true, message: '請輸入分類代碼', trigger: 'blur' }],
  ClassName: [{ required: true, message: '請輸入分類名稱', trigger: 'blur' }],
  ClassMode: [{ required: true, message: '請選擇分類區分', trigger: 'change' }],
  Status: [{ required: true, message: '請選擇狀態', trigger: 'change' }]
}

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      ClassId: queryForm.ClassId || undefined,
      ClassName: queryForm.ClassName || undefined,
      ClassMode: queryForm.ClassMode || undefined,
      Status: queryForm.Status || undefined
    }
    const response = await productCategoriesApi.getProductCategories(params)
    if (response.data.success) {
      tableData.value = response.data.data.items
      pagination.TotalCount = response.data.data.totalCount
    } else {
      ElMessage.error(response.data.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  } finally {
    loading.value = false
  }
}

// 重置
const handleReset = () => {
  queryForm.ClassId = ''
  queryForm.ClassName = ''
  queryForm.ClassMode = ''
  queryForm.Status = ''
  pagination.PageIndex = 1
  handleSearch()
}

// 新增
const handleCreate = () => {
  isEdit.value = false
  dialogTitle.value = '新增商品分類'
  resetForm()
  dialogVisible.value = true
}

// 修改
const handleEdit = async (row) => {
  isEdit.value = true
  dialogTitle.value = '修改商品分類'
  try {
    const response = await productCategoriesApi.getProductCategory(row.TKey)
    if (response.data.success) {
      Object.assign(form, response.data.data)
      // 如果是中分類或小分類，載入大分類列表
      if (form.ClassMode === '2' || form.ClassMode === '3') {
        await loadBClassList()
      }
      // 如果是小分類，載入中分類列表
      if (form.ClassMode === '3' && form.BClassId) {
        await handleBClassChange()
      }
    } else {
      ElMessage.error(response.data.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  }
  dialogVisible.value = true
}

// 刪除
const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm('確定要刪除此商品分類嗎？', '提示', {
      type: 'warning'
    })
    const response = await productCategoriesApi.deleteProductCategory(row.TKey)
    if (response.data.success) {
      ElMessage.success('刪除成功')
      handleSearch()
    } else {
      ElMessage.error(response.data.message || '刪除失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗：' + error.message)
    }
  }
}

// 提交
const handleSubmit = async () => {
  if (!formRef.value) return
  await formRef.value.validate(async (valid) => {
    if (valid) {
      try {
        let response
        if (isEdit.value) {
          // 從當前編輯的行獲取TKey
          const currentRow = tableData.value.find(item => item.TKey === form.TKey)
          if (currentRow && currentRow.TKey) {
            const updateData = {
              ClassName: form.ClassName,
              ClassType: form.ClassType,
              BClassId: form.BClassId,
              MClassId: form.MClassId,
              ParentTKey: form.ParentTKey,
              StypeId: form.StypeId,
              StypeId2: form.StypeId2,
              DepreStypeId: form.DepreStypeId,
              DepreStypeId2: form.DepreStypeId2,
              StypeTax: form.StypeTax,
              ItemCount: form.ItemCount,
              Status: form.Status,
              Notes: form.Notes
            }
            response = await productCategoriesApi.updateProductCategory(currentRow.TKey, updateData)
          } else {
            ElMessage.error('找不到要修改的資料')
            return
          }
        } else {
          response = await productCategoriesApi.createProductCategory(form)
        }
        if (response.data.success) {
          ElMessage.success(isEdit.value ? '修改成功' : '新增成功')
          dialogVisible.value = false
          handleSearch()
        } else {
          ElMessage.error(response.data.message || (isEdit.value ? '修改失敗' : '新增失敗'))
        }
      } catch (error) {
        ElMessage.error((isEdit.value ? '修改失敗' : '新增失敗') + '：' + error.message)
      }
    }
  })
}

// 關閉對話框
const handleDialogClose = () => {
  dialogVisible.value = false
  resetForm()
}

// 重置表單
const resetForm = () => {
  form.ClassId = ''
  form.ClassName = ''
  form.ClassMode = ''
  form.ClassType = '1'
  form.BClassId = ''
  form.MClassId = ''
  form.ParentTKey = null
  form.StypeId = ''
  form.StypeId2 = ''
  form.DepreStypeId = ''
  form.DepreStypeId2 = ''
  form.StypeTax = ''
  form.ItemCount = 0
  form.Status = 'A'
  form.Notes = ''
  bClassList.value = []
  mClassList.value = []
  formRef.value?.resetFields()
}

// 載入大分類列表
const loadBClassList = async () => {
  try {
    const response = await productCategoriesApi.getBClassList({ Status: 'A' })
    if (response.data.success) {
      bClassList.value = response.data.data || []
    }
  } catch (error) {
    console.error('載入大分類列表失敗：', error)
  }
}

// 大分類變更
const handleBClassChange = async () => {
  form.MClassId = ''
  mClassList.value = []
  if (form.BClassId && form.ClassMode === '3') {
    try {
      const response = await productCategoriesApi.getMClassList({ BClassId: form.BClassId, Status: 'A' })
      if (response.data.success) {
        mClassList.value = response.data.data || []
      }
    } catch (error) {
      console.error('載入中分類列表失敗：', error)
    }
  }
}

// 監聽分類區分變更
const watchClassMode = () => {
  if (form.ClassMode === '1') {
    form.BClassId = ''
    form.MClassId = ''
    form.ParentTKey = null
  } else if (form.ClassMode === '2') {
    form.MClassId = ''
    form.ParentTKey = null
    loadBClassList()
  } else if (form.ClassMode === '3') {
    form.ParentTKey = null
    loadBClassList()
    if (form.BClassId) {
      handleBClassChange()
    }
  }
}

// 分頁大小變更
const handleSizeChange = (size) => {
  pagination.PageSize = size
  pagination.PageIndex = 1
  handleSearch()
}

// 分頁變更
const handlePageChange = (page) => {
  pagination.PageIndex = page
  handleSearch()
}

// 初始化
onMounted(() => {
  handleSearch()
  loadBClassList()
})

// 監聽表單分類區分變更
watch(() => form.ClassMode, () => {
  watchClassMode()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.product-category {
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
    background-color: $card-bg;
    border-color: $card-border;
    
    .search-form {
      margin: 0;
    }
  }

  .table-card {
    margin-bottom: 20px;
    background-color: $card-bg;
    border-color: $card-border;
  }
}
</style>

