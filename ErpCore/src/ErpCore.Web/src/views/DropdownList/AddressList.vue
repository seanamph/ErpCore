<template>
  <div class="address-list">
    <div class="page-header">
      <h1>地址列表 (ADDR_CITY_LIST, ADDR_ZONE_LIST)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-tabs v-model="activeTab" @tab-change="handleTabChange">
        <el-tab-pane label="城市列表" name="city">
          <el-form :inline="true" :model="cityQueryForm" class="search-form">
            <el-form-item label="城市名稱">
              <el-input v-model="cityQueryForm.CityName" placeholder="請輸入城市名稱" clearable />
            </el-form-item>
            <el-form-item label="國家代碼">
              <el-input v-model="cityQueryForm.CountryCode" placeholder="請輸入國家代碼" clearable />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleCitySearch">查詢</el-button>
              <el-button @click="handleCityReset">重置</el-button>
            </el-form-item>
          </el-form>

          <!-- 城市列表 -->
          <el-table
            :data="cityList"
            v-loading="cityLoading"
            border
            stripe
            highlight-current-row
            @row-click="handleCityRowClick"
            style="width: 100%; cursor: pointer"
          >
            <el-table-column type="index" label="序號" width="60" align="center" />
            <el-table-column prop="CityId" label="城市代碼" width="120" />
            <el-table-column prop="CityName" label="城市名稱" min-width="200" />
            <el-table-column prop="CountryCode" label="國家代碼" width="100" align="center" />
            <el-table-column prop="SeqNo" label="排序序號" width="100" align="center" />
            <el-table-column prop="Status" label="狀態" width="80" align="center">
              <template #default="{ row }">
                <el-tag :type="row.Status === '1' ? 'success' : 'danger'" size="small">
                  {{ row.Status === '1' ? '啟用' : '停用' }}
                </el-tag>
              </template>
            </el-table-column>
          </el-table>

          <!-- 分頁 -->
          <el-pagination
            v-model:current-page="cityPagination.PageIndex"
            v-model:page-size="cityPagination.PageSize"
            :total="cityPagination.TotalCount"
            :page-sizes="[20, 50, 100, 200]"
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="handleCitySizeChange"
            @current-change="handleCityPageChange"
            style="margin-top: 20px; text-align: right"
          />
        </el-tab-pane>

        <el-tab-pane label="區域列表" name="zone">
          <el-form :inline="true" :model="zoneQueryForm" class="search-form">
            <el-form-item label="城市">
              <CitySelect
                v-model="zoneQueryForm.CityId"
                placeholder="請選擇城市"
                @change="handleCityChange"
                style="width: 200px"
              />
            </el-form-item>
            <el-form-item label="區域">
              <ZoneSelect
                v-model="zoneQueryForm.ZoneId"
                :city-id="zoneQueryForm.CityId"
                placeholder="請選擇區域"
                style="width: 200px"
              />
            </el-form-item>
            <el-form-item label="區域名稱">
              <el-input v-model="zoneQueryForm.ZoneName" placeholder="請輸入區域名稱" clearable />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleZoneSearch">查詢</el-button>
              <el-button @click="handleZoneReset">重置</el-button>
            </el-form-item>
          </el-form>

          <!-- 區域列表 -->
          <el-table
            :data="zoneList"
            v-loading="zoneLoading"
            border
            stripe
            highlight-current-row
            @row-click="handleZoneRowClick"
            style="width: 100%; cursor: pointer"
          >
            <el-table-column type="index" label="序號" width="60" align="center" />
            <el-table-column prop="ZoneId" label="區域代碼" width="120" />
            <el-table-column prop="ZoneName" label="區域名稱" min-width="200" />
            <el-table-column prop="CityId" label="城市代碼" width="120" />
            <el-table-column prop="CityName" label="城市名稱" width="150" />
            <el-table-column prop="ZipCode" label="郵遞區號" width="100" align="center" />
            <el-table-column prop="SeqNo" label="排序序號" width="100" align="center" />
            <el-table-column prop="Status" label="狀態" width="80" align="center">
              <template #default="{ row }">
                <el-tag :type="row.Status === '1' ? 'success' : 'danger'" size="small">
                  {{ row.Status === '1' ? '啟用' : '停用' }}
                </el-tag>
              </template>
            </el-table-column>
          </el-table>

          <!-- 分頁 -->
          <el-pagination
            v-model:current-page="zonePagination.PageIndex"
            v-model:page-size="zonePagination.PageSize"
            :total="zonePagination.TotalCount"
            :page-sizes="[20, 50, 100, 200]"
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="handleZoneSizeChange"
            @current-change="handleZonePageChange"
            style="margin-top: 20px; text-align: right"
          />
        </el-tab-pane>
      </el-tabs>
    </el-card>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { dropdownListApi } from '@/api/dropdownList'
import CitySelect from '@/components/CitySelect.vue'
import ZoneSelect from '@/components/ZoneSelect.vue'

// 當前標籤
const activeTab = ref('city')

// 城市查詢表單
const cityQueryForm = reactive({
  CityName: '',
  CountryCode: ''
})

// 城市列表
const cityList = ref([])
const cityLoading = ref(false)
const cityPagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 區域查詢表單
const zoneQueryForm = reactive({
  ZoneName: '',
  CityId: '',
  ZoneId: ''
})

// 區域列表
const zoneList = ref([])
const zoneLoading = ref(false)
const zonePagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 查詢城市列表
const loadCityList = async () => {
  cityLoading.value = true
  try {
    const params = {
      PageIndex: cityPagination.PageIndex,
      PageSize: cityPagination.PageSize,
      CityName: cityQueryForm.CityName || undefined,
      CountryCode: cityQueryForm.CountryCode || undefined,
      Status: '1'
    }
    const response = await dropdownListApi.getCities(params)
    if (response.data?.success) {
      cityList.value = response.data.data?.Items || []
      cityPagination.TotalCount = response.data.data?.TotalCount || 0
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.message || '未知錯誤'))
  } finally {
    cityLoading.value = false
  }
}

// 查詢區域列表
const loadZoneList = async () => {
  zoneLoading.value = true
  try {
    const params = {
      PageIndex: zonePagination.PageIndex,
      PageSize: zonePagination.PageSize,
      ZoneName: zoneQueryForm.ZoneName || undefined,
      CityId: zoneQueryForm.CityId || undefined,
      Status: '1'
    }
    const response = await dropdownListApi.getZones(params)
    if (response.data?.success) {
      zoneList.value = response.data.data?.Items || []
      zonePagination.TotalCount = response.data.data?.TotalCount || 0
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.message || '未知錯誤'))
  } finally {
    zoneLoading.value = false
  }
}

// 城市變更處理
const handleCityChange = () => {
  // 當城市變更時，清空區域選擇
  zoneQueryForm.ZoneId = ''
}

// 城市查詢
const handleCitySearch = () => {
  cityPagination.PageIndex = 1
  loadCityList()
}

// 城市重置
const handleCityReset = () => {
  cityQueryForm.CityName = ''
  cityQueryForm.CountryCode = ''
  handleCitySearch()
}

// 城市行點擊
const handleCityRowClick = (row) => {
  ElMessage.success(`已選擇城市：${row.CityName} (${row.CityId})`)
}

// 城市分頁大小變更
const handleCitySizeChange = (size) => {
  cityPagination.PageSize = size
  cityPagination.PageIndex = 1
  loadCityList()
}

// 城市分頁變更
const handleCityPageChange = (page) => {
  cityPagination.PageIndex = page
  loadCityList()
}

// 區域查詢
const handleZoneSearch = () => {
  zonePagination.PageIndex = 1
  loadZoneList()
}

// 區域重置
const handleZoneReset = () => {
  zoneQueryForm.ZoneName = ''
  zoneQueryForm.CityId = ''
  zoneQueryForm.ZoneId = ''
  handleZoneSearch()
}

// 區域行點擊
const handleZoneRowClick = (row) => {
  ElMessage.success(`已選擇區域：${row.ZoneName} (${row.ZoneId})`)
}

// 區域分頁大小變更
const handleZoneSizeChange = (size) => {
  zonePagination.PageSize = size
  zonePagination.PageIndex = 1
  loadZoneList()
}

// 區域分頁變更
const handleZonePageChange = (page) => {
  zonePagination.PageIndex = page
  loadZoneList()
}

// 標籤切換
const handleTabChange = (tabName) => {
  if (tabName === 'city' && cityList.value.length === 0) {
    loadCityList()
  } else if (tabName === 'zone' && zoneList.value.length === 0) {
    loadZoneList()
  }
}

// 初始化
onMounted(() => {
  loadCityList()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.address-list {
  padding: 20px;
}

.page-header {
  margin-bottom: 20px;
}

.page-header h1 {
  font-size: 24px;
  font-weight: 500;
}

.search-card {
  margin-bottom: 20px;
}

.search-form {
  margin: 0;
}
</style>

