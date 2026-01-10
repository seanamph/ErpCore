<template>
  <div class="account-subject">
    <div class="page-header">
      <h1>會計科目維護 (SYSN110)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="科目代號">
          <el-input v-model="queryForm.StypeId" placeholder="請輸入科目代號" clearable />
        </el-form-item>
        <el-form-item label="科目名稱">
          <el-input v-model="queryForm.StypeName" placeholder="請輸入科目名稱" clearable />
        </el-form-item>
        <el-form-item label="借貸方向">
          <el-select v-model="queryForm.Dc" placeholder="請選擇借貸方向" clearable>
            <el-option label="借方" value="D" />
            <el-option label="貸方" value="C" />
          </el-select>
        </el-form-item>
        <el-form-item label="統制/明細">
          <el-select v-model="queryForm.LedgerMd" placeholder="請選擇統制/明細" clearable>
            <el-option label="統制" value="L" />
            <el-option label="明細" value="M" />
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
        <el-table-column prop="StypeId" label="科目代號" width="120" />
        <el-table-column prop="StypeName" label="科目名稱" width="200" />
        <el-table-column prop="StypeNameE" label="英文名稱" width="200" />
        <el-table-column prop="Dc" label="借貸方向" width="100">
          <template #default="{ row }">
            <el-tag :type="row.Dc === 'D' ? 'success' : 'warning'">
              {{ row.Dc === 'D' ? '借方' : '貸方' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="LedgerMd" label="統制/明細" width="100">
          <template #default="{ row }">
            <el-tag :type="row.LedgerMd === 'L' ? 'info' : 'primary'">
              {{ row.LedgerMd === 'L' ? '統制' : '明細' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="VoucherType" label="傳票格式" width="120" />
        <el-table-column prop="BudgetYn" label="預算科目" width="100">
          <template #default="{ row }">
            <el-tag :type="row.BudgetYn === 'Y' ? 'success' : 'info'">
              {{ row.BudgetYn === 'Y' ? '是' : '否' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="StypeClass" label="科目別" width="120" />
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
            <el-button type="warning" size="small" @click="handleEdit(row)">修改</el-button>
            <el-button type="danger" size="small" @click="handleDelete(row)">刪除</el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分頁 -->
      <el-pagination
        v-model:current-page="pagination.PageIndex"
        v-model:page-size="pagination.PageSize"
        :page-sizes="[10, 20, 50, 100]"
        :total="pagination.TotalCount"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleSizeChange"
        @current-change="handlePageChange"
        style="margin-top: 20px; text-align: right;"
      />
    </el-card>

    <!-- 新增/修改對話框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="dialogTitle"
      width="900px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="140px"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="科目代號" prop="StypeId">
              <el-input v-model="formData.StypeId" :disabled="isEdit" placeholder="請輸入科目代號" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="科目名稱" prop="StypeName">
              <el-input v-model="formData.StypeName" placeholder="請輸入科目名稱" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="英文名稱" prop="StypeNameE">
              <el-input v-model="formData.StypeNameE" placeholder="請輸入英文名稱" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="借貸方向" prop="Dc">
              <el-select v-model="formData.Dc" placeholder="請選擇借貸方向" style="width: 100%">
                <el-option label="借方" value="D" />
                <el-option label="貸方" value="C" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="統制/明細" prop="LedgerMd">
              <el-select v-model="formData.LedgerMd" placeholder="請選擇統制/明細" style="width: 100%">
                <el-option label="統制" value="L" />
                <el-option label="明細" value="M" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="三碼代號" prop="MtypeId">
              <el-input v-model="formData.MtypeId" placeholder="請輸入三碼代號" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="傳票格式" prop="VoucherType">
              <el-input v-model="formData.VoucherType" placeholder="請輸入傳票格式" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="是否為預算科目" prop="BudgetYn">
              <el-select v-model="formData.BudgetYn" placeholder="請選擇" style="width: 100%">
                <el-option label="是" value="Y" />
                <el-option label="否" value="N" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="是否設定部門代號" prop="OrgYn">
              <el-select v-model="formData.OrgYn" placeholder="請選擇" style="width: 100%">
                <el-option label="是" value="Y" />
                <el-option label="否" value="N" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="是否可輸" prop="StypeYn">
              <el-select v-model="formData.StypeYn" placeholder="請選擇" style="width: 100%">
                <el-option label="是" value="Y" />
                <el-option label="否" value="N" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="科目別" prop="StypeClass">
              <el-input v-model="formData.StypeClass" placeholder="請輸入科目別" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="排序" prop="StypeOrder">
              <el-input-number v-model="formData.StypeOrder" :min="0" style="width: 100%" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="IFRS會計科目" prop="IfrsStypeId">
              <el-input v-model="formData.IfrsStypeId" placeholder="請輸入IFRS會計科目" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="集團會計科目" prop="RocStypeId">
              <el-input v-model="formData.RocStypeId" placeholder="請輸入集團會計科目" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="SAP會計科目" prop="SapStypeId">
              <el-input v-model="formData.SapStypeId" placeholder="請輸入SAP會計科目" />
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>

      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSubmit">確定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { accountingApi } from '@/api/accounting'

export default {
  name: 'AccountSubject',
  setup() {
    const loading = ref(false)
    const dialogVisible = ref(false)
    const isEdit = ref(false)
    const formRef = ref(null)
    const tableData = ref([])

    // 查詢表單
    const queryForm = reactive({
      StypeId: '',
      StypeName: '',
      Dc: '',
      LedgerMd: '',
      PageIndex: 1,
      PageSize: 20
    })

    // 分頁資訊
    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

    // 表單資料
    const formData = reactive({
      StypeId: '',
      StypeName: '',
      StypeNameE: '',
      Dc: 'D',
      LedgerMd: 'M',
      MtypeId: '',
      AbatYn: 'N',
      VoucherType: '',
      BudgetYn: 'N',
      OrgYn: 'N',
      ExpYear: null,
      ResiValue: null,
      DepreLid: '',
      AccudepreLid: '',
      StypeYn: 'Y',
      IfrsStypeId: '',
      RocStypeId: '',
      SapStypeId: '',
      StypeClass: '',
      StypeOrder: 0
    })

    // 表單驗證規則
    const formRules = {
      StypeId: [{ required: true, message: '請輸入科目代號', trigger: 'blur' }],
      StypeName: [{ required: true, message: '請輸入科目名稱', trigger: 'blur' }],
      Dc: [{ required: true, message: '請選擇借貸方向', trigger: 'change' }],
      LedgerMd: [{ required: true, message: '請選擇統制/明細', trigger: 'change' }]
    }

    // 查詢資料
    const loadData = async () => {
      loading.value = true
      try {
        const params = {
          ...queryForm,
          PageIndex: pagination.PageIndex,
          PageSize: pagination.PageSize
        }
        const response = await accountingApi.getAccountSubjects(params)
        if (response.Data) {
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
        StypeId: '',
        StypeName: '',
        Dc: '',
        LedgerMd: ''
      })
      handleSearch()
    }

    // 新增
    const handleCreate = () => {
      isEdit.value = false
      dialogVisible.value = true
      Object.assign(formData, {
        StypeId: '',
        StypeName: '',
        StypeNameE: '',
        Dc: 'D',
        LedgerMd: 'M',
        MtypeId: '',
        AbatYn: 'N',
        VoucherType: '',
        BudgetYn: 'N',
        OrgYn: 'N',
        ExpYear: null,
        ResiValue: null,
        DepreLid: '',
        AccudepreLid: '',
        StypeYn: 'Y',
        IfrsStypeId: '',
        RocStypeId: '',
        SapStypeId: '',
        StypeClass: '',
        StypeOrder: 0
      })
    }

    // 查看
    const handleView = async (row) => {
      try {
        const response = await accountingApi.getAccountSubject(row.StypeId)
        if (response.Data) {
          isEdit.value = true
          dialogVisible.value = true
          Object.assign(formData, response.Data)
        }
      } catch (error) {
        ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
      }
    }

    // 修改
    const handleEdit = async (row) => {
      await handleView(row)
    }

    // 刪除
    const handleDelete = async (row) => {
      try {
        await ElMessageBox.confirm('確定要刪除此會計科目嗎？', '確認', {
          type: 'warning'
        })
        await accountingApi.deleteAccountSubject(row.StypeId)
        ElMessage.success('刪除成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 提交表單
    const handleSubmit = async () => {
      if (!formRef.value) return
      await formRef.value.validate(async (valid) => {
        if (valid) {
          try {
            if (isEdit.value) {
              await accountingApi.updateAccountSubject(formData.StypeId, formData)
              ElMessage.success('修改成功')
            } else {
              await accountingApi.createAccountSubject(formData)
              ElMessage.success('新增成功')
            }
            dialogVisible.value = false
            loadData()
          } catch (error) {
            ElMessage.error('操作失敗: ' + (error.message || '未知錯誤'))
          }
        }
      })
    }

    // 分頁大小變更
    const handleSizeChange = (size) => {
      pagination.PageSize = size
      loadData()
    }

    // 分頁變更
    const handlePageChange = (page) => {
      pagination.PageIndex = page
      loadData()
    }

    // 計算對話框標題
    const dialogTitle = computed(() => {
      return isEdit.value ? '修改會計科目' : '新增會計科目'
    })

    onMounted(() => {
      loadData()
    })

    return {
      loading,
      dialogVisible,
      isEdit,
      formRef,
      tableData,
      queryForm,
      pagination,
      formData,
      formRules,
      dialogTitle,
      handleSearch,
      handleReset,
      handleCreate,
      handleView,
      handleEdit,
      handleDelete,
      handleSubmit,
      handleSizeChange,
      handlePageChange
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.account-subject {
  .page-header {
    margin-bottom: 20px;
    
    h1 {
      font-size: 24px;
      font-weight: 500;
      color: $text-color-primary;
    }
  }

  .search-card {
    margin-bottom: 20px;
    background-color: $card-bg;
    border: 1px solid $card-border;
    
    .search-form {
      .el-form-item {
        margin-bottom: 16px;
      }
    }
  }

  .table-card {
    background-color: $card-bg;
    border: 1px solid $card-border;
  }
}
</style>

