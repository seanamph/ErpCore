<template>
  <div class="system-config">
    <div class="page-header">
      <h1>系統設定 (CFG0410, CFG0420, CFG0430, CFG0440)</h1>
    </div>

    <!-- 標籤頁 -->
    <el-tabs v-model="activeTab" @tab-change="handleTabChange">
      <!-- 主系統項目資料維護 -->
      <el-tab-pane label="主系統項目資料維護(CFG0410)" name="systems">
        <el-card class="search-card" shadow="never">
          <el-form :inline="true" :model="systemQueryForm" class="search-form">
            <el-form-item label="主系統代碼">
              <el-input v-model="systemQueryForm.SystemId" placeholder="請輸入主系統代碼" clearable />
            </el-form-item>
            <el-form-item label="主系統名稱">
              <el-input v-model="systemQueryForm.SystemName" placeholder="請輸入主系統名稱" clearable />
            </el-form-item>
            <el-form-item label="系統型態">
              <el-input v-model="systemQueryForm.SystemType" placeholder="請輸入系統型態" clearable />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleSystemSearch">查詢</el-button>
              <el-button @click="handleSystemReset">重置</el-button>
              <el-button type="success" @click="handleSystemCreate">新增</el-button>
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
            <el-table-column prop="SystemId" label="主系統代碼" width="150" />
            <el-table-column prop="SystemName" label="主系統名稱" width="200" />
            <el-table-column prop="SeqNo" label="排序序號" width="100" align="right" />
            <el-table-column prop="SystemType" label="系統型態" width="150" />
            <el-table-column prop="ServerIp" label="伺服器主機名稱" width="200" />
            <el-table-column prop="ModuleId" label="模組代碼" width="150" />
            <el-table-column prop="Status" label="狀態" width="100">
              <template #default="{ row }">
                <el-tag :type="row.Status === 'A' ? 'success' : 'danger'">
                  {{ row.Status === 'A' ? '啟用' : '停用' }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column label="操作" width="200" fixed="right">
              <template #default="{ row }">
                <el-button type="primary" size="small" @click="handleSystemEdit(row)">修改</el-button>
                <el-button type="danger" size="small" @click="handleSystemDelete(row)">刪除</el-button>
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

        <!-- 新增/修改對話框 -->
        <el-dialog
          :title="systemDialogTitle"
          v-model="systemDialogVisible"
          width="800px"
          @close="handleSystemDialogClose"
        >
          <el-form :model="systemForm" :rules="systemRules" ref="systemFormRef" label-width="150px">
            <el-form-item label="主系統代碼" prop="SystemId">
              <el-input v-model="systemForm.SystemId" placeholder="請輸入主系統代碼" :disabled="systemIsEdit" maxlength="50" />
            </el-form-item>
            <el-form-item label="主系統名稱" prop="SystemName">
              <el-input v-model="systemForm.SystemName" placeholder="請輸入主系統名稱" maxlength="100" />
            </el-form-item>
            <el-form-item label="排序序號" prop="SeqNo">
              <el-input-number v-model="systemForm.SeqNo" :min="0" :step="1" style="width: 100%" />
            </el-form-item>
            <el-form-item label="系統型態" prop="SystemType">
              <el-input v-model="systemForm.SystemType" placeholder="請輸入系統型態" maxlength="20" />
            </el-form-item>
            <el-form-item label="伺服器主機名稱" prop="ServerIp">
              <el-input v-model="systemForm.ServerIp" placeholder="請輸入伺服器主機名稱" maxlength="100" />
            </el-form-item>
            <el-form-item label="模組代碼" prop="ModuleId">
              <el-input v-model="systemForm.ModuleId" placeholder="請輸入模組代碼" maxlength="50" />
            </el-form-item>
            <el-form-item label="資料庫使用者" prop="DbUser">
              <el-input v-model="systemForm.DbUser" placeholder="請輸入資料庫使用者" maxlength="50" />
            </el-form-item>
            <el-form-item label="資料庫密碼" prop="DbPass">
              <el-input v-model="systemForm.DbPass" type="password" placeholder="請輸入資料庫密碼" show-password maxlength="255" />
            </el-form-item>
            <el-form-item label="備註" prop="Notes">
              <el-input v-model="systemForm.Notes" type="textarea" :rows="3" placeholder="請輸入備註" maxlength="500" show-word-limit />
            </el-form-item>
            <el-form-item label="狀態" prop="Status">
              <el-radio-group v-model="systemForm.Status">
                <el-radio label="A">啟用</el-radio>
                <el-radio label="I">停用</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-form>
          <template #footer>
            <el-button @click="handleSystemDialogClose">取消</el-button>
            <el-button type="primary" @click="handleSystemSubmit">確定</el-button>
          </template>
        </el-dialog>
      </el-tab-pane>

      <!-- 子系統項目資料維護 -->
      <el-tab-pane label="子系統項目資料維護(CFG0420)" name="subsystems">
        <el-card class="search-card" shadow="never">
          <el-form :inline="true" :model="subSystemQueryForm" class="search-form">
            <el-form-item label="子系統代碼">
              <el-input v-model="subSystemQueryForm.SubSystemId" placeholder="請輸入子系統代碼" clearable />
            </el-form-item>
            <el-form-item label="子系統名稱">
              <el-input v-model="subSystemQueryForm.SubSystemName" placeholder="請輸入子系統名稱" clearable />
            </el-form-item>
            <el-form-item label="主系統代碼">
              <el-select v-model="subSystemQueryForm.SystemId" placeholder="請選擇主系統" clearable filterable style="width: 200px">
                <el-option v-for="system in systemList" :key="system.SystemId" :label="system.SystemName" :value="system.SystemId" />
              </el-select>
            </el-form-item>
            <el-form-item label="上層子系統代碼">
              <el-select v-model="subSystemQueryForm.ParentSubSystemId" placeholder="請選擇上層子系統" clearable filterable style="width: 200px">
                <el-option label="無（根節點）" value="0" />
                <el-option v-for="subSystem in subSystemList" :key="subSystem.SubSystemId" :label="subSystem.SubSystemName" :value="subSystem.SubSystemId" />
              </el-select>
            </el-form-item>
            <el-form-item label="狀態">
              <el-select v-model="subSystemQueryForm.Status" placeholder="請選擇狀態" clearable style="width: 120px">
                <el-option label="啟用" value="A" />
                <el-option label="停用" value="I" />
              </el-select>
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleSubSystemSearch">查詢</el-button>
              <el-button @click="handleSubSystemReset">重置</el-button>
              <el-button type="success" @click="handleSubSystemCreate">新增</el-button>
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
            <el-table-column prop="SubSystemId" label="子系統代碼" width="150" />
            <el-table-column prop="SubSystemName" label="子系統名稱" width="200" />
            <el-table-column prop="SeqNo" label="排序序號" width="100" align="right" />
            <el-table-column prop="SystemId" label="主系統代碼" width="150" />
            <el-table-column prop="SystemName" label="主系統名稱" width="200" />
            <el-table-column prop="ParentSubSystemId" label="上層子系統代碼" width="150" />
            <el-table-column prop="ParentSubSystemName" label="上層子系統名稱" width="200" />
            <el-table-column prop="Status" label="狀態" width="100">
              <template #default="{ row }">
                <el-tag :type="row.Status === 'A' ? 'success' : 'danger'">
                  {{ row.Status === 'A' ? '啟用' : '停用' }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="Notes" label="備註" min-width="200" show-overflow-tooltip />
            <el-table-column label="操作" width="200" fixed="right">
              <template #default="{ row }">
                <el-button type="primary" size="small" @click="handleSubSystemEdit(row)">修改</el-button>
                <el-button type="danger" size="small" @click="handleSubSystemDelete(row)">刪除</el-button>
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

        <!-- 新增/修改對話框 -->
        <el-dialog
          :title="subSystemDialogTitle"
          v-model="subSystemDialogVisible"
          width="800px"
          @close="handleSubSystemDialogClose"
        >
          <el-form :model="subSystemForm" :rules="subSystemRules" ref="subSystemFormRef" label-width="150px">
            <el-form-item label="子系統項目代碼" prop="SubSystemId">
              <el-input v-model="subSystemForm.SubSystemId" placeholder="請輸入子系統項目代碼" :disabled="subSystemIsEdit" maxlength="50" />
            </el-form-item>
            <el-form-item label="子系統項目名稱" prop="SubSystemName">
              <el-input v-model="subSystemForm.SubSystemName" placeholder="請輸入子系統項目名稱" maxlength="100" />
            </el-form-item>
            <el-form-item label="排序序號" prop="SeqNo">
              <el-input-number v-model="subSystemForm.SeqNo" :min="0" :step="1" style="width: 100%" />
            </el-form-item>
            <el-form-item label="主系統代碼" prop="SystemId">
              <el-select v-model="subSystemForm.SystemId" placeholder="請選擇主系統" filterable style="width: 100%">
                <el-option v-for="system in systemList" :key="system.SystemId" :label="system.SystemName" :value="system.SystemId" />
              </el-select>
            </el-form-item>
            <el-form-item label="上層子系統代碼" prop="ParentSubSystemId">
              <el-select v-model="subSystemForm.ParentSubSystemId" placeholder="請選擇上層子系統" filterable style="width: 100%">
                <el-option label="無（根節點）" value="0" />
                <el-option v-for="subSystem in subSystemList" :key="subSystem.SubSystemId" :label="subSystem.SubSystemName" :value="subSystem.SubSystemId" />
              </el-select>
            </el-form-item>
            <el-form-item label="狀態" prop="Status">
              <el-radio-group v-model="subSystemForm.Status">
                <el-radio label="A">啟用</el-radio>
                <el-radio label="I">停用</el-radio>
              </el-radio-group>
            </el-form-item>
            <el-form-item label="備註" prop="Notes">
              <el-input v-model="subSystemForm.Notes" type="textarea" :rows="3" placeholder="請輸入備註" maxlength="500" show-word-limit />
            </el-form-item>
          </el-form>
          <template #footer>
            <el-button @click="handleSubSystemDialogClose">取消</el-button>
            <el-button type="primary" @click="handleSubSystemSubmit">確定</el-button>
          </template>
        </el-dialog>
      </el-tab-pane>

      <!-- 系統作業資料維護 -->
      <el-tab-pane label="系統作業資料維護 (CFG0430)" name="programs">
        <el-card class="search-card" shadow="never">
          <el-form :inline="true" :model="programQueryForm" class="search-form">
            <el-form-item label="作業代碼">
              <el-input v-model="programQueryForm.ProgramId" placeholder="請輸入作業代碼" clearable />
            </el-form-item>
            <el-form-item label="作業名稱">
              <el-input v-model="programQueryForm.ProgramName" placeholder="請輸入作業名稱" clearable />
            </el-form-item>
            <el-form-item label="主系統代碼">
              <el-select v-model="programQueryForm.SystemId" placeholder="請選擇主系統" filterable clearable style="width: 200px">
                <el-option v-for="system in systemList" :key="system.SystemId" :label="system.SystemName" :value="system.SystemId" />
              </el-select>
            </el-form-item>
            <el-form-item label="子系統代碼">
              <el-select v-model="programQueryForm.SubSystemId" placeholder="請選擇子系統" filterable clearable style="width: 200px">
                <el-option v-for="subSystem in subSystemList" :key="subSystem.SubSystemId" :label="subSystem.SubSystemName" :value="subSystem.SubSystemId" />
              </el-select>
            </el-form-item>
            <el-form-item label="狀態">
              <el-select v-model="programQueryForm.Status" placeholder="請選擇狀態" clearable style="width: 150px">
                <el-option label="啟用" value="A" />
                <el-option label="停用" value="I" />
              </el-select>
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleProgramSearch">查詢</el-button>
              <el-button @click="handleProgramReset">重置</el-button>
              <el-button type="success" @click="handleProgramCreate">新增</el-button>
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
            <el-table-column prop="ProgramId" label="作業代碼" width="150" />
            <el-table-column prop="ProgramName" label="作業名稱" width="200" />
            <el-table-column prop="SystemId" label="主系統代碼" width="150" />
            <el-table-column prop="SystemName" label="主系統名稱" width="200" />
            <el-table-column prop="SubSystemId" label="子系統代碼" width="150" />
            <el-table-column prop="SubSystemName" label="子系統名稱" width="200" />
            <el-table-column prop="SeqNo" label="排序序號" width="100" align="right" />
            <el-table-column prop="Status" label="狀態" width="100">
              <template #default="{ row }">
                <el-tag :type="row.Status === 'A' ? 'success' : 'danger'">
                  {{ row.Status === 'A' ? '啟用' : '停用' }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column label="操作" width="200" fixed="right">
              <template #default="{ row }">
                <el-button type="primary" size="small" @click="handleProgramEdit(row)">修改</el-button>
                <el-button type="danger" size="small" @click="handleProgramDelete(row)">刪除</el-button>
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

        <!-- 新增/修改對話框 -->
        <el-dialog
          :title="programDialogTitle"
          v-model="programDialogVisible"
          width="800px"
          @close="handleProgramDialogClose"
        >
          <el-form :model="programForm" :rules="programRules" ref="programFormRef" label-width="150px">
            <el-form-item label="作業代碼" prop="ProgramId">
              <el-input v-model="programForm.ProgramId" placeholder="請輸入作業代碼" :disabled="programIsEdit" maxlength="50" />
            </el-form-item>
            <el-form-item label="作業名稱" prop="ProgramName">
              <el-input v-model="programForm.ProgramName" placeholder="請輸入作業名稱" maxlength="100" />
            </el-form-item>
            <el-form-item label="主系統代碼" prop="SystemId">
              <el-select v-model="programForm.SystemId" placeholder="請選擇主系統" filterable style="width: 100%" @change="handleProgramSystemChange">
                <el-option v-for="system in systemList" :key="system.SystemId" :label="system.SystemName" :value="system.SystemId" />
              </el-select>
            </el-form-item>
            <el-form-item label="子系統代碼" prop="SubSystemId">
              <el-select v-model="programForm.SubSystemId" placeholder="請選擇子系統" filterable clearable style="width: 100%">
                <el-option v-for="subSystem in filteredSubSystemList" :key="subSystem.SubSystemId" :label="subSystem.SubSystemName" :value="subSystem.SubSystemId" />
              </el-select>
            </el-form-item>
            <el-form-item label="排序序號" prop="SeqNo">
              <el-input-number v-model="programForm.SeqNo" :min="0" :step="1" style="width: 100%" />
            </el-form-item>
            <el-form-item label="狀態" prop="Status">
              <el-radio-group v-model="programForm.Status">
                <el-radio label="A">啟用</el-radio>
                <el-radio label="I">停用</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-form>
          <template #footer>
            <el-button @click="handleProgramDialogClose">取消</el-button>
            <el-button type="primary" @click="handleProgramSubmit">確定</el-button>
          </template>
        </el-dialog>
      </el-tab-pane>

      <!-- 系統功能按鈕資料維護 -->
      <el-tab-pane label="系統功能按鈕資料維護 (CFG0440)" name="buttons">
        <el-card class="search-card" shadow="never">
          <el-form :inline="true" :model="buttonQueryForm" class="search-form">
            <el-form-item label="按鈕代碼">
              <el-input v-model="buttonQueryForm.ButtonId" placeholder="請輸入按鈕代碼" clearable />
            </el-form-item>
            <el-form-item label="按鈕名稱">
              <el-input v-model="buttonQueryForm.ButtonName" placeholder="請輸入按鈕名稱" clearable />
            </el-form-item>
            <el-form-item label="作業代碼">
              <el-select v-model="buttonQueryForm.ProgramId" placeholder="請選擇作業" filterable clearable style="width: 200px">
                <el-option v-for="program in programList" :key="program.ProgramId" :label="`${program.ProgramId} - ${program.ProgramName}`" :value="program.ProgramId" />
              </el-select>
            </el-form-item>
            <el-form-item label="按鈕型態">
              <el-input v-model="buttonQueryForm.ButtonType" placeholder="請輸入按鈕型態" clearable />
            </el-form-item>
            <el-form-item label="狀態">
              <el-select v-model="buttonQueryForm.Status" placeholder="請選擇狀態" clearable style="width: 150px">
                <el-option label="啟用" value="A" />
                <el-option label="停用" value="I" />
              </el-select>
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleButtonSearch">查詢</el-button>
              <el-button @click="handleButtonReset">重置</el-button>
              <el-button type="success" @click="handleButtonCreate">新增</el-button>
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
            <el-table-column prop="ButtonId" label="按鈕代碼" width="150" />
            <el-table-column prop="ButtonName" label="按鈕名稱" width="200" />
            <el-table-column prop="ProgramId" label="作業代碼" width="150" />
            <el-table-column prop="SeqNo" label="排序序號" width="100" align="right" />
            <el-table-column prop="Status" label="狀態" width="100">
              <template #default="{ row }">
                <el-tag :type="row.Status === 'A' ? 'success' : 'danger'">
                  {{ row.Status === 'A' ? '啟用' : '停用' }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column label="操作" width="200" fixed="right">
              <template #default="{ row }">
                <el-button type="primary" size="small" @click="handleButtonEdit(row)">修改</el-button>
                <el-button type="danger" size="small" @click="handleButtonDelete(row)">刪除</el-button>
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

        <!-- 新增/修改對話框 -->
        <el-dialog
          :title="buttonDialogTitle"
          v-model="buttonDialogVisible"
          width="800px"
          @close="handleButtonDialogClose"
        >
          <el-form :model="buttonForm" :rules="buttonRules" ref="buttonFormRef" label-width="150px">
            <el-form-item label="按鈕代碼" prop="ButtonId">
              <el-input v-model="buttonForm.ButtonId" placeholder="請輸入按鈕代碼" :disabled="buttonIsEdit" maxlength="50" />
            </el-form-item>
            <el-form-item label="按鈕名稱" prop="ButtonName">
              <el-input v-model="buttonForm.ButtonName" placeholder="請輸入按鈕名稱" maxlength="100" />
            </el-form-item>
            <el-form-item label="作業代碼" prop="ProgramId">
              <el-select v-model="buttonForm.ProgramId" placeholder="請選擇作業" filterable style="width: 100%">
                <el-option v-for="program in programList" :key="program.ProgramId" :label="`${program.ProgramId} - ${program.ProgramName}`" :value="program.ProgramId" />
              </el-select>
            </el-form-item>
            <el-form-item label="按鈕型態" prop="ButtonType">
              <el-input v-model="buttonForm.ButtonType" placeholder="請輸入按鈕型態" maxlength="20" />
            </el-form-item>
            <el-form-item label="排序序號" prop="SeqNo">
              <el-input-number v-model="buttonForm.SeqNo" :min="0" :step="1" style="width: 100%" />
            </el-form-item>
            <el-form-item label="狀態" prop="Status">
              <el-radio-group v-model="buttonForm.Status">
                <el-radio label="A">啟用</el-radio>
                <el-radio label="I">停用</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-form>
          <template #footer>
            <el-button @click="handleButtonDialogClose">取消</el-button>
            <el-button type="primary" @click="handleButtonSubmit">確定</el-button>
          </template>
        </el-dialog>
      </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, computed } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import {
  configSystemsApi,
  configSubSystemsApi,
  configProgramsApi,
  configButtonsApi
} from '@/api/systemConfig'

// 標籤頁
const activeTab = ref('systems')

// 主系統相關
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

// 主系統對話框
const systemDialogVisible = ref(false)
const systemDialogTitle = ref('新增主系統')
const systemIsEdit = ref(false)
const systemFormRef = ref(null)
const systemForm = reactive({
  SystemId: '',
  SystemName: '',
  SeqNo: null,
  SystemType: '',
  ServerIp: '',
  ModuleId: '',
  DbUser: '',
  DbPass: '',
  Notes: '',
  Status: 'A'
})

// 主系統表單驗證規則
const systemRules = {
  SystemId: [
    { required: true, message: '請輸入主系統代碼', trigger: 'blur' },
    { max: 50, message: '主系統代碼長度不能超過50個字元', trigger: 'blur' }
  ],
  SystemName: [
    { required: true, message: '請輸入主系統名稱', trigger: 'blur' },
    { max: 100, message: '主系統名稱長度不能超過100個字元', trigger: 'blur' }
  ]
}

// 子系統相關
const subSystemQueryForm = reactive({
  SubSystemId: '',
  SubSystemName: '',
  SystemId: '',
  ParentSubSystemId: '',
  Status: ''
})
const subSystemTableData = ref([])
const subSystemLoading = ref(false)
const subSystemPagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 子系統對話框
const subSystemDialogVisible = ref(false)
const subSystemDialogTitle = ref('新增子系統')
const subSystemIsEdit = ref(false)
const subSystemFormRef = ref(null)
const subSystemForm = reactive({
  SubSystemId: '',
  SubSystemName: '',
  SeqNo: null,
  SystemId: '',
  ParentSubSystemId: '0',
  Status: 'A',
  Notes: ''
})

// 子系統表單驗證規則
const subSystemRules = {
  SubSystemId: [
    { required: true, message: '請輸入子系統項目代碼', trigger: 'blur' },
    { max: 50, message: '子系統項目代碼長度不能超過50個字元', trigger: 'blur' }
  ],
  SubSystemName: [
    { required: true, message: '請輸入子系統項目名稱', trigger: 'blur' },
    { max: 100, message: '子系統項目名稱長度不能超過100個字元', trigger: 'blur' }
  ],
  SystemId: [
    { required: true, message: '請選擇主系統代碼', trigger: 'change' }
  ]
}

// 主系統列表（用於下拉選擇）
const systemList = ref([])

// 子系統列表（用於下拉選擇）
const subSystemList = ref([])

// 系統作業相關
const programQueryForm = reactive({
  ProgramId: '',
  ProgramName: '',
  SystemId: '',
  SubSystemId: '',
  Status: ''
})
const programTableData = ref([])
const programLoading = ref(false)
const programPagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 系統作業對話框
const programDialogVisible = ref(false)
const programDialogTitle = ref('新增系統作業')
const programIsEdit = ref(false)
const programForm = reactive({
  ProgramId: '',
  ProgramName: '',
  SystemId: '',
  SubSystemId: '',
  SeqNo: null,
  Status: 'A'
})
const programFormRef = ref(null)
const programRules = {
  ProgramId: [
    { required: true, message: '請輸入作業代碼', trigger: 'blur' },
    { max: 50, message: '作業代碼長度不能超過50個字符', trigger: 'blur' }
  ],
  ProgramName: [
    { required: true, message: '請輸入作業名稱', trigger: 'blur' },
    { max: 100, message: '作業名稱長度不能超過100個字符', trigger: 'blur' }
  ],
  SystemId: [
    { required: true, message: '請選擇主系統', trigger: 'change' }
  ]
}

// 過濾後的子系統列表（根據選擇的主系統）
const filteredSubSystemList = computed(() => {
  if (!programForm.SystemId) {
    return []
  }
  return subSystemList.value.filter(subSystem => subSystem.SystemId === programForm.SystemId)
})

// 系統功能按鈕相關
const buttonQueryForm = reactive({
  ButtonId: '',
  ButtonName: '',
  ProgramId: '',
  ButtonType: '',
  Status: ''
})
const buttonTableData = ref([])
const buttonLoading = ref(false)
const buttonPagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 系統功能按鈕對話框
const buttonDialogVisible = ref(false)
const buttonDialogTitle = ref('新增系統功能按鈕')
const buttonIsEdit = ref(false)
const buttonFormRef = ref(null)
const buttonForm = reactive({
  ButtonId: '',
  ButtonName: '',
  ProgramId: '',
  ButtonType: '',
  SeqNo: null,
  Status: 'A'
})

// 系統功能按鈕表單驗證規則
const buttonRules = {
  ButtonId: [
    { required: true, message: '請輸入按鈕代碼', trigger: 'blur' },
    { max: 50, message: '按鈕代碼長度不能超過50個字元', trigger: 'blur' }
  ],
  ButtonName: [
    { required: true, message: '請輸入按鈕名稱', trigger: 'blur' },
    { max: 100, message: '按鈕名稱長度不能超過100個字元', trigger: 'blur' }
  ],
  ProgramId: [
    { required: true, message: '請選擇作業代碼', trigger: 'change' }
  ]
}

// 作業列表（用於下拉選擇）
const programList = ref([])

// 標籤頁變更
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

// 主系統查詢
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
      ElMessage.error(response.data.Message || '查詢失敗')
    }
  } catch (error) {
    console.error('查詢主系統列表失敗', error)
    ElMessage.error('查詢主系統列表失敗')
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
  systemIsEdit.value = false
  systemDialogTitle.value = '新增主系統'
  Object.assign(systemForm, {
    SystemId: '',
    SystemName: '',
    SeqNo: null,
    SystemType: '',
    ServerIp: '',
    ModuleId: '',
    DbUser: '',
    DbPass: '',
    Notes: '',
    Status: 'A'
  })
  systemDialogVisible.value = true
}

const handleSystemEdit = async (row) => {
  systemIsEdit.value = true
  systemDialogTitle.value = '修改主系統'
  try {
    const response = await configSystemsApi.getConfigSystem(row.SystemId)
    if (response.data.Success) {
      const data = response.data.Data
      Object.assign(systemForm, {
        SystemId: data.SystemId,
        SystemName: data.SystemName,
        SeqNo: data.SeqNo,
        SystemType: data.SystemType || '',
        ServerIp: data.ServerIp || '',
        ModuleId: data.ModuleId || '',
        DbUser: data.DbUser || '',
        DbPass: '', // 不顯示密碼
        Notes: data.Notes || '',
        Status: data.Status || 'A'
      })
      systemDialogVisible.value = true
    } else {
      ElMessage.error(response.data.Message || '載入資料失敗')
    }
  } catch (error) {
    console.error('載入主系統資料失敗', error)
    ElMessage.error('載入主系統資料失敗')
  }
}

const handleSystemDelete = async (row) => {
  try {
    await ElMessageBox.confirm('確定要刪除此主系統嗎？', '提示', {
      type: 'warning'
    })
    const response = await configSystemsApi.deleteConfigSystem(row.SystemId)
    if (response.data.Success) {
      ElMessage.success('刪除成功')
      handleSystemSearch()
    } else {
      ElMessage.error(response.data.Message || '刪除失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      console.error('刪除主系統失敗', error)
      ElMessage.error('刪除主系統失敗')
    }
  }
}

const handleSystemSubmit = async () => {
  try {
    await systemFormRef.value.validate()
    if (systemIsEdit.value) {
      const updateData = {
        SystemName: systemForm.SystemName,
        SeqNo: systemForm.SeqNo,
        SystemType: systemForm.SystemType,
        ServerIp: systemForm.ServerIp,
        ModuleId: systemForm.ModuleId,
        DbUser: systemForm.DbUser,
        DbPass: systemForm.DbPass || undefined,
        Notes: systemForm.Notes,
        Status: systemForm.Status
      }
      const response = await configSystemsApi.updateConfigSystem(systemForm.SystemId, updateData)
      if (response.data.Success) {
        ElMessage.success('修改成功')
        systemDialogVisible.value = false
        handleSystemSearch()
      } else {
        ElMessage.error(response.data.Message || '修改失敗')
      }
    } else {
      const response = await configSystemsApi.createConfigSystem(systemForm)
      if (response.data.Success) {
        ElMessage.success('新增成功')
        systemDialogVisible.value = false
        handleSystemSearch()
      } else {
        ElMessage.error(response.data.Message || '新增失敗')
      }
    }
  } catch (error) {
    if (error !== false) {
      console.error('提交失敗', error)
      ElMessage.error((systemIsEdit.value ? '修改' : '新增') + '失敗')
    }
  }
}

const handleSystemDialogClose = () => {
  systemFormRef.value?.resetFields()
  systemDialogVisible.value = false
}

// 分頁變更
const handleSystemSizeChange = (size) => {
  systemPagination.PageSize = size
  systemPagination.PageIndex = 1
  handleSystemSearch()
}

const handleSystemPageChange = (page) => {
  systemPagination.PageIndex = page
  handleSystemSearch()
}

// 載入主系統列表
const loadSystemList = async () => {
  try {
    const params = {
      PageIndex: 1,
      PageSize: 1000,
      Status: 'A'
    }
    const response = await configSystemsApi.getConfigSystems(params)
    if (response.data.Success) {
      systemList.value = response.data.Data.Items || []
    }
  } catch (error) {
    console.error('載入主系統列表失敗', error)
  }
}

// 載入子系統列表
const loadSubSystemList = async () => {
  try {
    const params = {
      PageIndex: 1,
      PageSize: 1000,
      Status: 'A'
    }
    const response = await configSubSystemsApi.getConfigSubSystems(params)
    if (response.data.Success) {
      subSystemList.value = response.data.Data.Items || []
    }
  } catch (error) {
    console.error('載入子系統列表失敗', error)
  }
}

// 載入作業列表
const loadProgramList = async () => {
  try {
    const params = {
      PageIndex: 1,
      PageSize: 1000,
      Status: 'A'
    }
    const response = await configProgramsApi.getConfigPrograms(params)
    if (response.data.Success) {
      programList.value = response.data.Data.Items || []
    }
  } catch (error) {
    console.error('載入作業列表失敗', error)
  }
}

// 子系統查詢
const handleSubSystemSearch = async () => {
  subSystemLoading.value = true
  try {
    const params = {
      PageIndex: subSystemPagination.PageIndex,
      PageSize: subSystemPagination.PageSize,
      SubSystemId: subSystemQueryForm.SubSystemId || undefined,
      SubSystemName: subSystemQueryForm.SubSystemName || undefined,
      SystemId: subSystemQueryForm.SystemId || undefined,
      ParentSubSystemId: subSystemQueryForm.ParentSubSystemId || undefined,
      Status: subSystemQueryForm.Status || undefined
    }
    const response = await configSubSystemsApi.getConfigSubSystems(params)
    if (response.data.Success) {
      subSystemTableData.value = response.data.Data.Items || []
      subSystemPagination.TotalCount = response.data.Data.TotalCount || 0
    } else {
      ElMessage.error(response.data.Message || '查詢失敗')
    }
  } catch (error) {
    console.error('查詢子系統列表失敗', error)
    ElMessage.error('查詢子系統列表失敗')
  } finally {
    subSystemLoading.value = false
  }
}

const handleSubSystemReset = () => {
  subSystemQueryForm.SubSystemId = ''
  subSystemQueryForm.SubSystemName = ''
  subSystemQueryForm.SystemId = ''
  subSystemQueryForm.ParentSubSystemId = ''
  subSystemQueryForm.Status = ''
  subSystemPagination.PageIndex = 1
  handleSubSystemSearch()
}

const handleSubSystemCreate = () => {
  subSystemIsEdit.value = false
  subSystemDialogTitle.value = '新增子系統'
  Object.assign(subSystemForm, {
    SubSystemId: '',
    SubSystemName: '',
    SeqNo: null,
    SystemId: '',
    ParentSubSystemId: '0',
    Status: 'A',
    Notes: ''
  })
  subSystemDialogVisible.value = true
}

const handleSubSystemEdit = async (row) => {
  subSystemIsEdit.value = true
  subSystemDialogTitle.value = '修改子系統'
  try {
    const response = await configSubSystemsApi.getConfigSubSystem(row.SubSystemId)
    if (response.data.Success) {
      const data = response.data.Data
      Object.assign(subSystemForm, {
        SubSystemId: data.SubSystemId,
        SubSystemName: data.SubSystemName,
        SeqNo: data.SeqNo,
        SystemId: data.SystemId,
        ParentSubSystemId: data.ParentSubSystemId || '0',
        Status: data.Status || 'A',
        Notes: data.Notes || ''
      })
      subSystemDialogVisible.value = true
    } else {
      ElMessage.error(response.data.Message || '載入資料失敗')
    }
  } catch (error) {
    console.error('載入子系統資料失敗', error)
    ElMessage.error('載入子系統資料失敗')
  }
}

const handleSubSystemDelete = async (row) => {
  try {
    await ElMessageBox.confirm('確定要刪除此子系統嗎？', '提示', {
      type: 'warning'
    })
    const response = await configSubSystemsApi.deleteConfigSubSystem(row.SubSystemId)
    if (response.data.Success) {
      ElMessage.success('刪除成功')
      handleSubSystemSearch()
    } else {
      ElMessage.error(response.data.Message || '刪除失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      console.error('刪除子系統失敗', error)
      ElMessage.error('刪除子系統失敗')
    }
  }
}

const handleSubSystemSubmit = async () => {
  try {
    await subSystemFormRef.value.validate()
    if (subSystemIsEdit.value) {
      const updateData = {
        SubSystemName: subSystemForm.SubSystemName,
        SeqNo: subSystemForm.SeqNo,
        SystemId: subSystemForm.SystemId,
        ParentSubSystemId: subSystemForm.ParentSubSystemId || '0',
        Status: subSystemForm.Status,
        Notes: subSystemForm.Notes
      }
      const response = await configSubSystemsApi.updateConfigSubSystem(subSystemForm.SubSystemId, updateData)
      if (response.data.Success) {
        ElMessage.success('修改成功')
        subSystemDialogVisible.value = false
        handleSubSystemSearch()
        loadSubSystemList() // 重新載入子系統列表
      } else {
        ElMessage.error(response.data.Message || '修改失敗')
      }
    } else {
      const response = await configSubSystemsApi.createConfigSubSystem(subSystemForm)
      if (response.data.Success) {
        ElMessage.success('新增成功')
        subSystemDialogVisible.value = false
        handleSubSystemSearch()
        loadSubSystemList() // 重新載入子系統列表
      } else {
        ElMessage.error(response.data.Message || '新增失敗')
      }
    }
  } catch (error) {
    if (error !== false) {
      console.error('提交失敗', error)
      ElMessage.error((subSystemIsEdit.value ? '修改' : '新增') + '失敗')
    }
  }
}

const handleSubSystemDialogClose = () => {
  subSystemFormRef.value?.resetFields()
  subSystemDialogVisible.value = false
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

// 系統作業查詢
const handleProgramSearch = async () => {
  programLoading.value = true
  try {
    const params = {
      PageIndex: programPagination.PageIndex,
      PageSize: programPagination.PageSize,
      ProgramId: programQueryForm.ProgramId || undefined,
      ProgramName: programQueryForm.ProgramName || undefined,
      SystemId: programQueryForm.SystemId || undefined,
      SubSystemId: programQueryForm.SubSystemId || undefined,
      Status: programQueryForm.Status || undefined
    }
    const response = await configProgramsApi.getConfigPrograms(params)
    if (response.data.Success) {
      programTableData.value = response.data.Data.Items || []
      programPagination.TotalCount = response.data.Data.TotalCount || 0
    } else {
      ElMessage.error(response.data.Message || '查詢失敗')
    }
  } catch (error) {
    console.error('查詢系統作業列表失敗:', error)
    ElMessage.error('查詢系統作業列表失敗')
  } finally {
    programLoading.value = false
  }
}

const handleProgramReset = () => {
  programQueryForm.ProgramId = ''
  programQueryForm.ProgramName = ''
  programQueryForm.SystemId = ''
  programQueryForm.SubSystemId = ''
  programQueryForm.Status = ''
  programPagination.PageIndex = 1
  handleProgramSearch()
}

const handleProgramCreate = () => {
  programIsEdit.value = false
  programDialogTitle.value = '新增系統作業'
  Object.assign(programForm, {
    ProgramId: '',
    ProgramName: '',
    SystemId: '',
    SubSystemId: '',
    SeqNo: null,
    Status: 'A'
  })
  programDialogVisible.value = true
}

const handleProgramEdit = async (row) => {
  programIsEdit.value = true
  programDialogTitle.value = '修改系統作業'
  try {
    const response = await configProgramsApi.getConfigProgram(row.ProgramId)
    if (response.data.Success) {
      const data = response.data.Data
      Object.assign(programForm, {
        ProgramId: data.ProgramId,
        ProgramName: data.ProgramName,
        SystemId: data.SystemId,
        SubSystemId: data.SubSystemId || '',
        SeqNo: data.SeqNo,
        Status: data.Status || 'A'
      })
      programDialogVisible.value = true
    } else {
      ElMessage.error(response.data.Message || '載入資料失敗')
    }
  } catch (error) {
    console.error('載入系統作業資料失敗', error)
    ElMessage.error('載入系統作業資料失敗')
  }
}

const handleProgramSubmit = async () => {
  try {
    await programFormRef.value.validate()
    if (programIsEdit.value) {
      const updateData = {
        SystemId: programForm.SystemId,
        SubSystemId: programForm.SubSystemId || undefined,
        ProgramName: programForm.ProgramName,
        SeqNo: programForm.SeqNo,
        Status: programForm.Status
      }
      const response = await configProgramsApi.updateConfigProgram(programForm.ProgramId, updateData)
      if (response.data.Success) {
        ElMessage.success('修改成功')
        programDialogVisible.value = false
        handleProgramSearch()
      } else {
        ElMessage.error(response.data.Message || '修改失敗')
      }
    } else {
      const response = await configProgramsApi.createConfigProgram(programForm)
      if (response.data.Success) {
        ElMessage.success('新增成功')
        programDialogVisible.value = false
        handleProgramSearch()
      } else {
        ElMessage.error(response.data.Message || '新增失敗')
      }
    }
  } catch (error) {
    if (error !== false) {
      console.error('提交失敗', error)
      ElMessage.error((programIsEdit.value ? '修改' : '新增') + '失敗')
    }
  }
}

const handleProgramDialogClose = () => {
  programFormRef.value?.resetFields()
  programDialogVisible.value = false
}

const handleProgramSystemChange = () => {
  // 當主系統變更時，清空子系統選擇
  programForm.SubSystemId = ''
}

const handleProgramDelete = async (row) => {
  try {
    await ElMessageBox.confirm('確定要刪除此系統作業嗎？', '提示', {
      type: 'warning'
    })
    const response = await configProgramsApi.deleteConfigProgram(row.ProgramId)
    if (response.data.Success) {
      ElMessage.success('刪除成功')
      handleProgramSearch()
    } else {
      ElMessage.error(response.data.Message || '刪除失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      console.error('刪除系統作業失敗:', error)
      ElMessage.error('刪除系統作業失敗')
    }
  }
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

// 系統功能按鈕查詢
const handleButtonSearch = async () => {
  buttonLoading.value = true
  try {
    const params = {
      PageIndex: buttonPagination.PageIndex,
      PageSize: buttonPagination.PageSize,
      ButtonId: buttonQueryForm.ButtonId || undefined,
      ButtonName: buttonQueryForm.ButtonName || undefined,
      ProgramId: buttonQueryForm.ProgramId || undefined,
      ButtonType: buttonQueryForm.ButtonType || undefined,
      Status: buttonQueryForm.Status || undefined
    }
    const response = await configButtonsApi.getConfigButtons(params)
    if (response.data.Success) {
      buttonTableData.value = response.data.Data.Items || []
      buttonPagination.TotalCount = response.data.Data.TotalCount || 0
    } else {
      ElMessage.error(response.data.Message || '查詢失敗')
    }
  } catch (error) {
    console.error('查詢系統功能按鈕列表失敗:', error)
    ElMessage.error('查詢系統功能按鈕列表失敗')
  } finally {
    buttonLoading.value = false
  }
}

const handleButtonReset = () => {
  buttonQueryForm.ButtonId = ''
  buttonQueryForm.ButtonName = ''
  buttonQueryForm.ProgramId = ''
  buttonQueryForm.ButtonType = ''
  buttonQueryForm.Status = ''
  buttonPagination.PageIndex = 1
  handleButtonSearch()
}

const handleButtonCreate = () => {
  buttonIsEdit.value = false
  buttonDialogTitle.value = '新增系統功能按鈕'
  Object.assign(buttonForm, {
    ButtonId: '',
    ButtonName: '',
    ProgramId: '',
    ButtonType: '',
    SeqNo: null,
    Status: 'A'
  })
  buttonDialogVisible.value = true
}

const handleButtonEdit = async (row) => {
  buttonIsEdit.value = true
  buttonDialogTitle.value = '修改系統功能按鈕'
  try {
    const response = await configButtonsApi.getConfigButton(row.ButtonId)
    if (response.data.Success) {
      const data = response.data.Data
      Object.assign(buttonForm, {
        ButtonId: data.ButtonId,
        ButtonName: data.ButtonName,
        ProgramId: data.ProgramId,
        ButtonType: data.ButtonType || '',
        SeqNo: data.SeqNo,
        Status: data.Status || 'A'
      })
      buttonDialogVisible.value = true
    } else {
      ElMessage.error(response.data.Message || '載入資料失敗')
    }
  } catch (error) {
    console.error('載入系統功能按鈕資料失敗', error)
    ElMessage.error('載入系統功能按鈕資料失敗')
  }
}

const handleButtonSubmit = async () => {
  try {
    await buttonFormRef.value.validate()
    if (buttonIsEdit.value) {
      const updateData = {
        ProgramId: buttonForm.ProgramId,
        ButtonName: buttonForm.ButtonName,
        ButtonType: buttonForm.ButtonType || undefined,
        SeqNo: buttonForm.SeqNo,
        Status: buttonForm.Status
      }
      const response = await configButtonsApi.updateConfigButton(buttonForm.ButtonId, updateData)
      if (response.data.Success) {
        ElMessage.success('修改成功')
        buttonDialogVisible.value = false
        handleButtonSearch()
      } else {
        ElMessage.error(response.data.Message || '修改失敗')
      }
    } else {
      const response = await configButtonsApi.createConfigButton(buttonForm)
      if (response.data.Success) {
        ElMessage.success('新增成功')
        buttonDialogVisible.value = false
        handleButtonSearch()
      } else {
        ElMessage.error(response.data.Message || '新增失敗')
      }
    }
  } catch (error) {
    if (error !== false) {
      console.error('提交失敗', error)
      ElMessage.error((buttonIsEdit.value ? '修改' : '新增') + '失敗')
    }
  }
}

const handleButtonDialogClose = () => {
  buttonFormRef.value?.resetFields()
  buttonDialogVisible.value = false
}

const handleButtonDelete = async (row) => {
  try {
    await ElMessageBox.confirm('確定要刪除此系統功能按鈕嗎？', '提示', {
      type: 'warning'
    })
    const response = await configButtonsApi.deleteConfigButton(row.ButtonId)
    if (response.data.Success) {
      ElMessage.success('刪除成功')
      handleButtonSearch()
    } else {
      ElMessage.error(response.data.Message || '刪除失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      console.error('刪除系統功能按鈕失敗:', error)
      ElMessage.error('刪除系統功能按鈕失敗')
    }
  }
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

// 初始化
onMounted(() => {
  handleSystemSearch()
  loadSystemList()
  loadSubSystemList()
  loadProgramList()
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
