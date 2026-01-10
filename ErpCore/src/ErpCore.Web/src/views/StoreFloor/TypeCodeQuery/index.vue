<template>
  <div class="type-code-query">
    <div class="page-header">
      <h1>È°ûÂ?‰ª?¢º?•Ë©¢‰ΩúÊ•≠ (SYS6501-SYS6560)</h1>
    </div>

    <!-- ?•Ë©¢Ë°®ÂñÆ -->
    <el-card class="search-card" shadow="never">
      <el-form :model="queryForm" class="search-form" label-width="120px">
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="È°ûÂ?‰ª?¢º">
              <el-input v-model="queryForm.TypeCode" placeholder="Ë´ãËº∏?•È??ã‰ª£Á¢? clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="È°ûÂ??çÁ®±">
              <el-input v-model="queryForm.TypeName" placeholder="Ë´ãËº∏?•È??ãÂ?Á®? clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="?ÜÈ?">
              <el-input v-model="queryForm.Category" placeholder="Ë´ãËº∏?•Â?È°? clearable />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="?Ä??>
              <el-select v-model="queryForm.Status" placeholder="Ë´ãÈÅ∏?? clearable>
                <el-option label="?®ÈÉ®" value="" />
                <el-option label="?üÁî®" value="A" />
                <el-option label="?úÁî®" value="I" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">?•Ë©¢</el-button>
          <el-button @click="handleReset">?çÁΩÆ</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- Ë≥áÊ?Ë°®Ê†º -->
    <el-card class="table-card" shadow="never">
      <el-table
        :data="tableData"
        v-loading="loading"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column prop="TypeCode" label="È°ûÂ?‰ª?¢º" width="150" />
        <el-table-column prop="TypeName" label="È°ûÂ??çÁ®±" width="200" />
        <el-table-column prop="TypeNameEn" label="?±Ê??çÁ®±" width="200" />
        <el-table-column prop="Category" label="?ÜÈ?" width="150" />
        <el-table-column prop="Description" label="Ë™™Ê?" width="300" show-overflow-tooltip />
        <el-table-column prop="SortOrder" label="?íÂ??ÜÂ?" width="120" />
        <el-table-column prop="Status" label="?Ä?? width="100">
          <template #default="{ row }">
            <el-tag :type="row.Status === 'A' ? 'success' : 'danger'">
              {{ row.Status === 'A' ? '?üÁî®' : '?úÁî®' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="CreatedAt" label="Âª∫Á??ÇÈ?" width="180">
          <template #default="{ row }">
            {{ formatDateTime(row.CreatedAt) }}
          </template>
        </el-table-column>
      </el-table>

      <!-- ?ÜÈ? -->
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
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { typeCodeQueryApi } from '@/api/storeFloor'

// ?•Ë©¢Ë°®ÂñÆ
const queryForm = reactive({
  TypeCode: '',
  TypeName: '',
  Category: '',
  Status: ''
})

// Ë°®Ê†ºË≥áÊ?
const tableData = ref([])
const loading = ref(false)

// ?ÜÈ?
const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// ?•Ë©¢
const handleSearch = async () => {
  loading.value = true
  try {
    const data = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      TypeCode: queryForm.TypeCode || undefined,
      TypeName: queryForm.TypeName || undefined,
      Category: queryForm.Category || undefined,
      Status: queryForm.Status || undefined
    }
    const response = await typeCodeQueryApi.queryTypeCodes(data)
    if (response.data?.success) {
      tableData.value = response.data.data?.items || []
      pagination.TotalCount = response.data.data?.totalCount || 0
    } else {
      ElMessage.error(response.data?.message || '?•Ë©¢Â§±Ê?')
    }
  } catch (error) {
    ElMessage.error('?•Ë©¢Â§±Ê?Ôº? + error.message)
  } finally {
    loading.value = false
  }
}

// ?çÁΩÆ
const handleReset = () => {
  queryForm.TypeCode = ''
  queryForm.TypeName = ''
  queryForm.Category = ''
  queryForm.Status = ''
  pagination.PageIndex = 1
  handleSearch()
}

// ?ÜÈ?Â§ßÂ?ËÆäÊõ¥
const handleSizeChange = (size) => {
  pagination.PageSize = size
  pagination.PageIndex = 1
  handleSearch()
}

// ?ÜÈ?ËÆäÊõ¥
const handlePageChange = (page) => {
  pagination.PageIndex = page
  handleSearch()
}

// ?ºÂ??ñÊó•?üÊ???
const formatDateTime = (date) => {
  if (!date) return ''
  const d = new Date(date)
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')} ${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}:${String(d.getSeconds()).padStart(2, '0')}`
}

// ?ùÂ???
onMounted(() => {
  handleSearch()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.type-code-query {
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

