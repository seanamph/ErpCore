<template>
  <div class="multi-select-list">
    <div class="page-header">
      <h1>多選列表 (MULTI_AREA_LIST, MULTI_SHOP_LIST, MULTI_USERS_LIST)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-tabs v-model="activeTab" @tab-change="handleTabChange">
        <el-tab-pane label="多選區域列表" name="area">
          <el-form :inline="true" :model="areaQueryForm" class="search-form">
            <el-form-item label="區域名稱">
              <el-input v-model="areaQueryForm.AreaName" placeholder="請輸入區域名稱" clearable />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleAreaSearch">查詢</el-button>
              <el-button @click="handleAreaReset">重置</el-button>
            </el-form-item>
          </el-form>

          <!-- 區域列表 -->
          <el-table
            :data="areaList"
            v-loading="areaLoading"
            border
            stripe
            @selection-change="handleAreaSelectionChange"
            style="width: 100%"
          >
            <el-table-column type="selection" width="55" />
            <el-table-column type="index" label="序號" width="60" align="center" />
            <el-table-column prop="AreaId" label="區域代碼" width="120" />
            <el-table-column prop="AreaName" label="區域名稱" min-width="200" />
            <el-table-column prop="Status" label="狀態" width="80" align="center">
              <template #default="{ row }">
                <el-tag :type="row.Status === '1' ? 'success' : 'danger'" size="small">
                  {{ row.Status === '1' ? '啟用' : '停用' }}
                </el-tag>
              </template>
            </el-table-column>
          </el-table>
        </el-tab-pane>

        <el-tab-pane label="多選店別列表" name="shop">
          <el-form :inline="true" :model="shopQueryForm" class="search-form">
            <el-form-item label="店別名稱">
              <el-input v-model="shopQueryForm.ShopName" placeholder="請輸入店別名稱" clearable />
            </el-form-item>
            <el-form-item label="區域代碼">
              <el-input v-model="shopQueryForm.AreaId" placeholder="請輸入區域代碼" clearable />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleShopSearch">查詢</el-button>
              <el-button @click="handleShopReset">重置</el-button>
            </el-form-item>
          </el-form>

          <!-- 店別列表 -->
          <el-table
            :data="shopList"
            v-loading="shopLoading"
            border
            stripe
            @selection-change="handleShopSelectionChange"
            style="width: 100%"
          >
            <el-table-column type="selection" width="55" />
            <el-table-column type="index" label="序號" width="60" align="center" />
            <el-table-column prop="ShopId" label="店別代碼" width="120" />
            <el-table-column prop="ShopName" label="店別名稱" min-width="200" />
            <el-table-column prop="AreaId" label="區域代碼" width="120" />
            <el-table-column prop="AreaName" label="區域名稱" width="150" />
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
            v-model:current-page="shopPagination.PageIndex"
            v-model:page-size="shopPagination.PageSize"
            :total="shopPagination.TotalCount"
            :page-sizes="[20, 50, 100, 200]"
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="handleShopSizeChange"
            @current-change="handleShopPageChange"
            style="margin-top: 20px; text-align: right"
          />
        </el-tab-pane>

        <el-tab-pane label="多選使用者列表" name="user">
          <el-form :inline="true" :model="userQueryForm" class="search-form">
            <el-form-item label="使用者名稱">
              <el-input v-model="userQueryForm.UserName" placeholder="請輸入使用者名稱" clearable />
            </el-form-item>
            <el-form-item label="使用者代碼">
              <el-input v-model="userQueryForm.UserId" placeholder="請輸入使用者代碼" clearable />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleUserSearch">查詢</el-button>
              <el-button @click="handleUserReset">重置</el-button>
            </el-form-item>
          </el-form>

          <!-- 使用者列表 -->
          <el-table
            :data="userList"
            v-loading="userLoading"
            border
            stripe
            @selection-change="handleUserSelectionChange"
            style="width: 100%"
          >
            <el-table-column type="selection" width="55" />
            <el-table-column type="index" label="序號" width="60" align="center" />
            <el-table-column prop="UserId" label="使用者代碼" width="120" />
            <el-table-column prop="UserName" label="使用者名稱" min-width="200" />
            <el-table-column prop="Title" label="職稱" width="150" />
            <el-table-column prop="OrgId" label="組織代碼" width="120" />
            <el-table-column prop="Status" label="狀態" width="80" align="center">
              <template #default="{ row }">
                <el-tag :type="row.Status === 'A' ? 'success' : 'danger'" size="small">
                  {{ row.Status === 'A' ? '啟用' : '停用' }}
                </el-tag>
              </template>
            </el-table-column>
          </el-table>

          <!-- 分頁 -->
          <el-pagination
            v-model:current-page="userPagination.PageIndex"
            v-model:page-size="userPagination.PageSize"
            :total="userPagination.TotalCount"
            :page-sizes="[20, 50, 100, 200]"
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="handleUserSizeChange"
            @current-change="handleUserPageChange"
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

// 當前標籤
const activeTab = ref('area')

// 區域查詢表單
const areaQueryForm = reactive({
  AreaName: ''
})

// 區域列表
const areaList = ref([])
const areaLoading = ref(false)
const selectedAreas = ref([])

// 店別查詢表單
const shopQueryForm = reactive({
  ShopName: '',
  AreaId: ''
})

// 店別列表
const shopList = ref([])
const shopLoading = ref(false)
const shopPagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})
const selectedShops = ref([])

// 使用者查詢表單
const userQueryForm = reactive({
  UserName: '',
  UserId: ''
})

// 使用者列表
const userList = ref([])
const userLoading = ref(false)
const userPagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})
const selectedUsers = ref([])

// 載入區域列表
const loadAreaList = async () => {
  areaLoading.value = true
  try {
    const params = {
      AreaName: areaQueryForm.AreaName || undefined,
      Status: '1'
    }
    const response = await dropdownListApi.getMultiAreas(params)
    if (response.data?.success) {
      areaList.value = response.data.data || []
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.message || '未知錯誤'))
  } finally {
    areaLoading.value = false
  }
}

// 載入店別列表
const loadShopList = async () => {
  shopLoading.value = true
  try {
    const params = {
      PageIndex: shopPagination.PageIndex,
      PageSize: shopPagination.PageSize,
      ShopName: shopQueryForm.ShopName || undefined,
      AreaId: shopQueryForm.AreaId || undefined,
      Status: '1'
    }
    const response = await dropdownListApi.getMultiShops(params)
    if (response.data?.success) {
      shopList.value = response.data.data || []
      shopPagination.TotalCount = response.data.data?.length || 0
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.message || '未知錯誤'))
  } finally {
    shopLoading.value = false
  }
}

// 載入使用者列表
const loadUserList = async () => {
  userLoading.value = true
  try {
    const params = {
      PageIndex: userPagination.PageIndex,
      PageSize: userPagination.PageSize,
      UserName: userQueryForm.UserName || undefined,
      UserId: userQueryForm.UserId || undefined,
      Status: 'A'
    }
    const response = await dropdownListApi.getMultiUsers(params)
    if (response.data?.success) {
      userList.value = response.data.data?.Items || []
      userPagination.TotalCount = response.data.data?.TotalCount || 0
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.message || '未知錯誤'))
  } finally {
    userLoading.value = false
  }
}

// 區域查詢
const handleAreaSearch = () => {
  loadAreaList()
}

// 區域重置
const handleAreaReset = () => {
  areaQueryForm.AreaName = ''
  handleAreaSearch()
}

// 區域選擇變更
const handleAreaSelectionChange = (selection) => {
  selectedAreas.value = selection
  ElMessage.success(`已選擇 ${selection.length} 個區域`)
}

// 店別查詢
const handleShopSearch = () => {
  shopPagination.PageIndex = 1
  loadShopList()
}

// 店別重置
const handleShopReset = () => {
  shopQueryForm.ShopName = ''
  shopQueryForm.AreaId = ''
  handleShopSearch()
}

// 店別選擇變更
const handleShopSelectionChange = (selection) => {
  selectedShops.value = selection
  ElMessage.success(`已選擇 ${selection.length} 個店別`)
}

// 店別分頁大小變更
const handleShopSizeChange = (size) => {
  shopPagination.PageSize = size
  shopPagination.PageIndex = 1
  loadShopList()
}

// 店別分頁變更
const handleShopPageChange = (page) => {
  shopPagination.PageIndex = page
  loadShopList()
}

// 使用者查詢
const handleUserSearch = () => {
  userPagination.PageIndex = 1
  loadUserList()
}

// 使用者重置
const handleUserReset = () => {
  userQueryForm.UserName = ''
  userQueryForm.UserId = ''
  handleUserSearch()
}

// 使用者選擇變更
const handleUserSelectionChange = (selection) => {
  selectedUsers.value = selection
  ElMessage.success(`已選擇 ${selection.length} 個使用者`)
}

// 使用者分頁大小變更
const handleUserSizeChange = (size) => {
  userPagination.PageSize = size
  userPagination.PageIndex = 1
  loadUserList()
}

// 使用者分頁變更
const handleUserPageChange = (page) => {
  userPagination.PageIndex = page
  loadUserList()
}

// 標籤切換
const handleTabChange = (tabName) => {
  if (tabName === 'area' && areaList.value.length === 0) {
    loadAreaList()
  } else if (tabName === 'shop' && shopList.value.length === 0) {
    loadShopList()
  } else if (tabName === 'user' && userList.value.length === 0) {
    loadUserList()
  }
}

// 初始化
onMounted(() => {
  loadAreaList()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.multi-select-list {
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

