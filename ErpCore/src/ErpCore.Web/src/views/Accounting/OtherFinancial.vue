<template>
  <div class="other-financial">
    <div class="page-header">
      <h1>其他財務功能 (SYSN610-SYSN910)</h1>
    </div>

    <!-- 功能列表 -->
    <el-card class="function-card" shadow="never">
      <el-row :gutter="20">
        <el-col :span="8" v-for="func in functionList" :key="func.FunctionId">
          <el-card class="function-item" shadow="hover" @click="handleFunctionClick(func)">
            <div class="function-header">
              <h3>{{ func.FunctionName }}</h3>
              <el-tag :type="func.Status === 'A' ? 'success' : 'info'">
                {{ func.Status === 'A' ? '啟用' : '停用' }}
              </el-tag>
            </div>
            <div class="function-body">
              <p>功能代碼: {{ func.FunctionId }}</p>
            </div>
          </el-card>
        </el-col>
      </el-row>
    </el-card>

    <!-- 功能執行對話框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="currentFunction?.FunctionName"
      width="800px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="formRef"
        :model="formData"
        label-width="140px"
      >
        <el-alert
          :title="`功能代碼: ${currentFunction?.FunctionId}`"
          type="info"
          :closable="false"
          style="margin-bottom: 20px;"
        />
        <el-form-item label="執行參數">
          <el-input
            v-model="formData.Params"
            type="textarea"
            :rows="6"
            placeholder="請輸入執行參數（JSON格式）"
          />
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleExecute" :loading="executing">執行</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { accountingApi } from '@/api/accounting'

export default {
  name: 'OtherFinancial',
  setup() {
    const loading = ref(false)
    const executing = ref(false)
    const dialogVisible = ref(false)
    const formRef = ref(null)
    const functionList = ref([])
    const currentFunction = ref(null)

    // 表單資料
    const formData = reactive({
      Params: ''
    })

    // 載入功能列表
    const loadFunctions = async () => {
      loading.value = true
      try {
        const response = await accountingApi.getOtherFinancials()
        if (response.Data && response.Data.Functions) {
          functionList.value = response.Data.Functions
        }
      } catch (error) {
        ElMessage.error('載入功能列表失敗: ' + (error.message || '未知錯誤'))
      } finally {
        loading.value = false
      }
    }

    // 功能點擊
    const handleFunctionClick = (func) => {
      if (func.Status !== 'A') {
        ElMessage.warning('此功能目前停用')
        return
      }
      currentFunction.value = func
      formData.Params = ''
      dialogVisible.value = true
    }

    // 執行功能
    const handleExecute = async () => {
      if (!currentFunction.value) return
      
      let params = {}
      try {
        if (formData.Params.trim()) {
          params = JSON.parse(formData.Params)
        }
      } catch (error) {
        ElMessage.error('參數格式錯誤，請輸入有效的JSON格式')
        return
      }

      executing.value = true
      try {
        await accountingApi.executeOtherFinancial(currentFunction.value.FunctionId, params)
        ElMessage.success('執行成功')
        dialogVisible.value = false
      } catch (error) {
        ElMessage.error('執行失敗: ' + (error.message || '未知錯誤'))
      } finally {
        executing.value = false
      }
    }

    onMounted(() => {
      loadFunctions()
    })

    return {
      loading,
      executing,
      dialogVisible,
      formRef,
      functionList,
      currentFunction,
      formData,
      handleFunctionClick,
      handleExecute
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.other-financial {
  .page-header {
    margin-bottom: 20px;
    
    h1 {
      font-size: 24px;
      font-weight: 500;
      color: $text-color-primary;
    }
  }

  .function-card {
    background-color: $card-bg;
    border: 1px solid $card-border;
    
    .function-item {
      margin-bottom: 20px;
      cursor: pointer;
      transition: all 0.3s;
      
      &:hover {
        transform: translateY(-5px);
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
      }
      
      .function-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 10px;
        
        h3 {
          margin: 0;
          font-size: 18px;
          font-weight: 500;
          color: $text-color-primary;
        }
      }
      
      .function-body {
        p {
          margin: 0;
          color: $text-color-secondary;
          font-size: 14px;
        }
      }
    }
  }
}
</style>

