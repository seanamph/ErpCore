<template>
  <div class="system-config">
    <div class="page-header">
      <h1>系統設�? (CFG0410, CFG0420, CFG0430, CFG0440)</h1>
    </div>

    <!-- 標籤??-->
    <el-tabs v-model="activeTab" @tab-change="handleTabChange">
      <!-- 主系統�??��??�維�?-->
      <el-tab-pane label="主系統�??��??�維�?(CFG0410)" name="systems">
        <el-card class="search-card" shadow="never">
          <el-form :inline="true" :model="systemQueryForm" class="search-form">
            <el-form-item label="主系統代�?>
              <el-input v-model="systemQueryForm.SystemId" placeholder="請輸?�主系統�?��" clearable />
            </el-form-item>
            <el-form-item label="主系統�?�?>
              <el-input v-model="systemQueryForm.SystemName" placeholder="請輸?�主系統?�稱" clearable />
            </el-form-item>
            <el-form-item label="系統?��?">
              <el-input v-model="systemQueryForm.SystemType" placeholder="請輸?�系統�??? clearable />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleSystemSearch">?�詢</el-button>
              <el-button @click="handleSystemReset">?�置</el-button>
              <el-button type="success" @click="handleSystemCreate">?��?</el-button>
            </el-form-item>
          </el-form>
        </el-card>

        <el-card class="table-card" shadow="never">
          <el-table
            :data="systemTableData"
            v-loading="systemLoading"
            border
            stripe
            style="width: 100%"
          >
            <el-table-column prop="SystemId" label="主系統代�? width="150" />
            <el-table-column prop="SystemName" label="主系統�?�? width="200" />
            <el-table-column prop="SeqNo" label="?��?序�?" width="100" align="right" />
            <el-table-column prop="SystemType" label="系統?��?" width="150" />
            <el-table-column prop="ServerIp" label="伺�??�主機�?�? width="200" />
            <el-table-column prop="Status" label="?�?? width="100">
              <template #default="{ row }">
                <el-tag :type="row.Status === 'A' ? 'success' : 'danger'">
                  {{ row.Status === 'A' ? '?�用' : '?�用' }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column label="?��?" width="200" fixed="right">
              <template #default="{ row }">
                <el-button type="primary" size="small" @click="handleSystemEdit(row)">修改</el-button>
                <el-button type="danger" size="small" @click="handleSystemDelete(row)">?�除</el-button>
              </template>
            </el-table-column>
          </el-table>
          <el-pagination
            v-model:current-page="systemPagination.PageIndex"
            v-model:page-size="systemPagination.PageSize"
            :total="systemPagination.TotalCount"
            :page-sizes="[10, 20, 50, 100]"
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="handleSystemSizeChange"
            @current-change="handleSystemPageChange"
            style="margin-top: 20px; text-align: right"
          />
        </el-card>
      </el-tab-pane>

      <!-- 子系統�??��??�維�?-->
      <el-tab-pane label="子系統�??��??�維�?(CFG0420)" name="subsystems">
        <el-card class="search-card" shadow="never">
          <el-form :inline="true" :model="subSystemQueryForm" class="search-form">
            <el-form-item label="子系統代�?>
              <el-input v-model="subSystemQueryForm.SubSystemId" placeholder="請輸?��?系統�?��" clearable />
            </el-form-item>
            <el-form-item label="子系統�?�?>
              <el-input v-model="subSystemQueryForm.SubSystemName" placeholder="請輸?��?系統?�稱" clearable />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleSubSystemSearch">?�詢</el-button>
              <el-button @click="handleSubSystemReset">?�置</el-button>
              <el-button type="success" @click="handleSubSystemCreate">?��?</el-button>
            </el-form-item>
          </el-form>
        </el-card>

        <el-card class="table-card" shadow="never">
          <el-table
            :data="subSystemTableData"
            v-loading="subSystemLoading"
            border
            stripe
            style="width: 100%"
          >
            <el-table-column prop="SubSystemId" label="子系統代�? width="150" />
            <el-table-column prop="SubSystemName" label="子系統�?�? width="200" />
            <el-table-column prop="SystemId" label="主系統代�? width="150" />
            <el-table-column prop="SeqNo" label="?��?序�?" width="100" align="right" />
            <el-table-column prop="Status" label="?�?? width="100">
              <template #default="{ row }">
                <el-tag :type="row.Status === 'A' ? 'success' : 'danger'">
                  {{ row.Status === 'A' ? '?�用' : '?�用' }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column label="?��?" width="200" fixed="right">
              <template #default="{ row }">
                <el-button type="primary" size="small" @click="handleSubSystemEdit(row)">修改</el-button>
                <el-button type="danger" size="small" @click="handleSubSystemDelete(row)">?�除</el-button>
              </template>
            </el-table-column>
          </el-table>
          <el-pagination
            v-model:current-page="subSystemPagination.PageIndex"
            v-model:page-size="subSystemPagination.PageSize"
            :total="subSystemPagination.TotalCount"
            :page-sizes="[10, 20, 50, 100]"
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="handleSubSystemSizeChange"
            @current-change="handleSubSystemPageChange"
            style="margin-top: 20px; text-align: right"
          />
        </el-card>
      </el-tab-pane>

      <!-- 系統作業資�?維護 -->
      <el-tab-pane label="系統作業資�?維護 (CFG0430)" name="programs">
        <el-card class="search-card" shadow="never">
          <el-form :inline="true" :model="programQueryForm" class="search-form">
            <el-form-item label="作業�?��">
              <el-input v-model="programQueryForm.ProgramId" placeholder="請輸?��?業代�? clearable />
            </el-form-item>
            <el-form-item label="作業?�稱">
              <el-input v-model="programQueryForm.ProgramName" placeholder="請輸?��?業�?�? clearable />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleProgramSearch">?�詢</el-button>
              <el-button @click="handleProgramReset">?�置</el-button>
              <el-button type="success" @click="handleProgramCreate">?��?</el-button>
            </el-form-item>
          </el-form>
        </el-card>

        <el-card class="table-card" shadow="never">
          <el-table
            :data="programTableData"
            v-loading="programLoading"
            border
            stripe
            style="width: 100%"
          >
            <el-table-column prop="ProgramId" label="作業�?��" width="150" />
            <el-table-column prop="ProgramName" label="作業?�稱" width="200" />
            <el-table-column prop="SystemId" label="主系統代�? width="150" />
            <el-table-column prop="SubSystemId" label="子系統代�? width="150" />
            <el-table-column prop="SeqNo" label="?��?序�?" width="100" align="right" />
            <el-table-column prop="Status" label="?�?? width="100">
              <template #default="{ row }">
                <el-tag :type="row.Status === 'A' ? 'success' : 'danger'">
                  {{ row.Status === 'A' ? '?�用' : '?�用' }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column label="?��?" width="200" fixed="right">
              <template #default="{ row }">
                <el-button type="primary" size="small" @click="handleProgramEdit(row)">修改</el-button>
                <el-button type="danger" size="small" @click="handleProgramDelete(row)">?�除</el-button>
              </template>
            </el-table-column>
          </el-table>
          <el-pagination
            v-model:current-page="programPagination.PageIndex"
            v-model:page-size="programPagination.PageSize"
            :total="programPagination.TotalCount"
            :page-sizes="[10, 20, 50, 100]"
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="handleProgramSizeChange"
            @current-change="handleProgramPageChange"
            style="margin-top: 20px; text-align: right"
          />
        </el-card>
      </el-tab-pane>

      <!-- 系統?�能?��?資�?維護 -->
      <el-tab-pane label="系統?�能?��?資�?維護 (CFG0440)" name="buttons">
        <el-card class="search-card" shadow="never">
          <el-form :inline="true" :model="buttonQueryForm" class="search-form">
            <el-form-item label="?��?�?��">
              <el-input v-model="buttonQueryForm.ButtonId" placeholder="請輸?��??�代�? clearable />
            </el-form-item>
            <el-form-item label="?��??�稱">
              <el-input v-model="buttonQueryForm.ButtonName" placeholder="請輸?��??��?�? clearable />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleButtonSearch">?�詢</el-button>
              <el-button @click="handleButtonReset">?�置</el-button>
              <el-button type="success" @click="handleButtonCreate">?��?</el-button>
            </el-form-item>
          </el-form>
        </el-card>

        <el-card class="table-card" shadow="never">
          <el-table
            :data="buttonTableData"
            v-loading="buttonLoading"
            border
            stripe
            style="width: 100%"
          >
            <el-table-column prop="ButtonId" label="?��?�?��" width="150" />
            <el-table-column prop="ButtonName" label="?��??�稱" width="200" />
            <el-table-column prop="ProgramId" label="作業�?��" width="150" />
            <el-table-column prop="SeqNo" label="?��?序�?" width="100" align="right" />
            <el-table-column prop="Status" label="?�?? width="100">
              <template #default="{ row }">
                <el-tag :type="row.Status === 'A' ? 'success' : 'danger'">
                  {{ row.Status === 'A' ? '?�用' : '?�用' }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column label="?��?" width="200" fixed="right">
              <template #default="{ row }">
                <el-button type="primary" size="small" @click="handleButtonEdit(row)">修改</el-button>
                <el-button type="danger" size="small" @click="handleButtonDelete(row)">?�除</el-button>
              </template>
            </el-table-column>
          </el-table>
          <el-pagination
            v-model:current-page="buttonPagination.PageIndex"
            v-model:page-size="buttonPagination.PageSize"
            :total="buttonPagination.TotalCount"
            :page-sizes="[10, 20, 50, 100]"
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="handleButtonSizeChange"
            @current-change="handleButtonPageChange"
            style="margin-top: 20px; text-align: right"
          />
        </el-card>
      </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import {
  configSystemsApi,
  configSubSystemsApi,
  configProgramsApi,
  configButtonsApi
} from '@/api/systemConfig'

// 標籤??
const activeTab = ref('systems')

// 主系統相??
const systemQueryForm = reactive({
  SystemId: '',
  SystemName: '',
  SystemType: ''
})
const systemTableData = ref([])
const systemLoading = ref(false)
const systemPagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 子系統相??
const subSystemQueryForm = reactive({
  SubSystemId: '',
  SubSystemName: ''
})
const subSystemTableData = ref([])
const subSystemLoading = ref(false)
const subSystemPagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 系統作業?��?
const programQueryForm = reactive({
  ProgramId: '',
  ProgramName: ''
})
const programTableData = ref([])
const programLoading = ref(false)
const programPagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 系統?�能?��??��?
const buttonQueryForm = reactive({
  ButtonId: '',
  ButtonName: ''
})
const buttonTableData = ref([])
const buttonLoading = ref(false)
const buttonPagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 標籤?��???
const handleTabChange = (tabName) => {
  if (tabName === 'systems') {
    handleSystemSearch()
  } else if (tabName === 'subsystems') {
    handleSubSystemSearch()
  } else if (tabName === 'programs') {
    handleProgramSearch()
  } else if (tabName === 'buttons') {
    handleButtonSearch()
  }
}

// 主系統查�?
const handleSystemSearch = async () => {
  systemLoading.value = true
  try {
    const params = {
      PageIndex: systemPagination.PageIndex,
      PageSize: systemPagination.PageSize,
      SystemId: systemQueryForm.SystemId || undefined,
      SystemName: systemQueryForm.SystemName || undefined,
      SystemType: systemQueryForm.SystemType || undefined
    }
    const response = await configSystemsApi.getConfigSystems(params)
    if (response.data.Success) {
      systemTableData.value = response.data.Data.Items || []
      systemPagination.TotalCount = response.data.Data.TotalCount || 0
    } else {
      ElMessage.error(response.data.Message || '?�詢失�?')
    }
  } catch (error) {
    console.error('?�詢主系統�?表失??', error)
    ElMessage.error('?�詢主系統�?表失??)
  } finally {
    systemLoading.value = false
  }
}

const handleSystemReset = () => {
  systemQueryForm.SystemId = ''
  systemQueryForm.SystemName = ''
  systemQueryForm.SystemType = ''
  systemPagination.PageIndex = 1
  handleSystemSearch()
}

const handleSystemCreate = () => {
  ElMessage.info('?��?主系統�??��??�中')
}

const handleSystemEdit = (row) => {
  ElMessage.info('修改主系統�??��??�中')
}

const handleSystemDelete = async (row) => {
  try {
    await ElMessageBox.confirm('確�?要刪?�此主系統�?�?, '?�示', {
      type: 'warning'
    })
    const response = await configSystemsApi.deleteConfigSystem(row.SystemId)
    if (response.data.Success) {
      ElMessage.success('?�除?��?')
      handleSystemSearch()
    } else {
      ElMessage.error(response.data.Message || '?�除失�?')
    }
  } catch (error) {
    if (error !== 'cancel') {
      console.error('?�除主系統失??', error)
      ElMessage.error('?�除主系統失??)
    }
  }
}

// 子系統查�?
const handleSubSystemSearch = async () => {
  subSystemLoading.value = true
  try {
    const params = {
      PageIndex: subSystemPagination.PageIndex,
      PageSize: subSystemPagination.PageSize,
      SubSystemId: subSystemQueryForm.SubSystemId || undefined,
      SubSystemName: subSystemQueryForm.SubSystemName || undefined
    }
    const response = await configSubSystemsApi.getConfigSubSystems(params)
    if (response.data.Success) {
      subSystemTableData.value = response.data.Data.Items || []
      subSystemPagination.TotalCount = response.data.Data.TotalCount || 0
    } else {
      ElMessage.error(response.data.Message || '?�詢失�?')
    }
  } catch (error) {
    console.error('?�詢子系統�?表失??', error)
    ElMessage.error('?�詢子系統�?表失??)
  } finally {
    subSystemLoading.value = false
  }
}

const handleSubSystemReset = () => {
  subSystemQueryForm.SubSystemId = ''
  subSystemQueryForm.SubSystemName = ''
  subSystemPagination.PageIndex = 1
  handleSubSystemSearch()
}

const handleSubSystemCreate = () => {
  ElMessage.info('?��?子系統�??��??�中')
}

const handleSubSystemEdit = (row) => {
  ElMessage.info('修改子系統�??��??�中')
}

const handleSubSystemDelete = async (row) => {
  try {
    await ElMessageBox.confirm('確�?要刪?�此子系統�?�?, '?�示', {
      type: 'warning'
    })
    const response = await configSubSystemsApi.deleteConfigSubSystem(row.SubSystemId)
    if (response.data.Success) {
      ElMessage.success('?�除?��?')
      handleSubSystemSearch()
    } else {
      ElMessage.error(response.data.Message || '?�除失�?')
    }
  } catch (error) {
    if (error !== 'cancel') {
      console.error('?�除子系統失??', error)
      ElMessage.error('?�除子系統失??)
    }
  }
}

// 系統作業?�詢
const handleProgramSearch = async () => {
  programLoading.value = true
  try {
    const params = {
      PageIndex: programPagination.PageIndex,
      PageSize: programPagination.PageSize,
      ProgramId: programQueryForm.ProgramId || undefined,
      ProgramName: programQueryForm.ProgramName || undefined
    }
    const response = await configProgramsApi.getConfigPrograms(params)
    if (response.data.Success) {
      programTableData.value = response.data.Data.Items || []
      programPagination.TotalCount = response.data.Data.TotalCount || 0
    } else {
      ElMessage.error(response.data.Message || '?�詢失�?')
    }
  } catch (error) {
    console.error('?�詢系統作業?�表失�?:', error)
    ElMessage.error('?�詢系統作業?�表失�?')
  } finally {
    programLoading.value = false
  }
}

const handleProgramReset = () => {
  programQueryForm.ProgramId = ''
  programQueryForm.ProgramName = ''
  programPagination.PageIndex = 1
  handleProgramSearch()
}

const handleProgramCreate = () => {
  ElMessage.info('?��?系統作業?�能?�發�?)
}

const handleProgramEdit = (row) => {
  ElMessage.info('修改系統作業?�能?�發�?)
}

const handleProgramDelete = async (row) => {
  try {
    await ElMessageBox.confirm('確�?要刪?�此系統作業?��?', '?�示', {
      type: 'warning'
    })
    const response = await configProgramsApi.deleteConfigProgram(row.ProgramId)
    if (response.data.Success) {
      ElMessage.success('?�除?��?')
      handleProgramSearch()
    } else {
      ElMessage.error(response.data.Message || '?�除失�?')
    }
  } catch (error) {
    if (error !== 'cancel') {
      console.error('?�除系統作業失�?:', error)
      ElMessage.error('?�除系統作業失�?')
    }
  }
}

// 系統?�能?��??�詢
const handleButtonSearch = async () => {
  buttonLoading.value = true
  try {
    const params = {
      PageIndex: buttonPagination.PageIndex,
      PageSize: buttonPagination.PageSize,
      ButtonId: buttonQueryForm.ButtonId || undefined,
      ButtonName: buttonQueryForm.ButtonName || undefined
    }
    const response = await configButtonsApi.getConfigButtons(params)
    if (response.data.Success) {
      buttonTableData.value = response.data.Data.Items || []
      buttonPagination.TotalCount = response.data.Data.TotalCount || 0
    } else {
      ElMessage.error(response.data.Message || '?�詢失�?')
    }
  } catch (error) {
    console.error('?�詢系統?�能?��??�表失�?:', error)
    ElMessage.error('?�詢系統?�能?��??�表失�?')
  } finally {
    buttonLoading.value = false
  }
}

const handleButtonReset = () => {
  buttonQueryForm.ButtonId = ''
  buttonQueryForm.ButtonName = ''
  buttonPagination.PageIndex = 1
  handleButtonSearch()
}

const handleButtonCreate = () => {
  ElMessage.info('?��?系統?�能?��??�能?�發�?)
}

const handleButtonEdit = (row) => {
  ElMessage.info('修改系統?�能?��??�能?�發�?)
}

const handleButtonDelete = async (row) => {
  try {
    await ElMessageBox.confirm('確�?要刪?�此系統?�能?��??��?', '?�示', {
      type: 'warning'
    })
    const response = await configButtonsApi.deleteConfigButton(row.ButtonId)
    if (response.data.Success) {
      ElMessage.success('?�除?��?')
      handleButtonSearch()
    } else {
      ElMessage.error(response.data.Message || '?�除失�?')
    }
  } catch (error) {
    if (error !== 'cancel') {
      console.error('?�除系統?�能?��?失�?:', error)
      ElMessage.error('?�除系統?�能?��?失�?')
    }
  }
}

// ?��?變更
const handleSystemSizeChange = (size) => {
  systemPagination.PageSize = size
  systemPagination.PageIndex = 1
  handleSystemSearch()
}

const handleSystemPageChange = (page) => {
  systemPagination.PageIndex = page
  handleSystemSearch()
}

const handleSubSystemSizeChange = (size) => {
  subSystemPagination.PageSize = size
  subSystemPagination.PageIndex = 1
  handleSubSystemSearch()
}

const handleSubSystemPageChange = (page) => {
  subSystemPagination.PageIndex = page
  handleSubSystemSearch()
}

const handleProgramSizeChange = (size) => {
  programPagination.PageSize = size
  programPagination.PageIndex = 1
  handleProgramSearch()
}

const handleProgramPageChange = (page) => {
  programPagination.PageIndex = page
  handleProgramSearch()
}

const handleButtonSizeChange = (size) => {
  buttonPagination.PageSize = size
  buttonPagination.PageIndex = 1
  handleButtonSearch()
}

const handleButtonPageChange = (page) => {
  buttonPagination.PageIndex = page
  handleButtonSearch()
}

// ?��???
onMounted(() => {
  handleSystemSearch()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';
.system-config {
  padding: 20px;
}

.page-header {
  margin-bottom: 20px;
}

.page-header h1 {
  color: $primary-color;
  font-size: 24px;
  font-weight: bold;
}

.search-card {
  margin-bottom: 20px;
}

.table-card {
  margin-top: 20px;
}
</style>

