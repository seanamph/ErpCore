<template>
  <div class="framework">
    <div class="page-header">
      <h1>框架功能</h1>
    </div>

    <el-tabs v-model="activeTab" type="card">
      <!-- 選單功能 -->
      <el-tab-pane label="選單功能" name="menu">
        <el-card class="card" shadow="never">
          <el-row :gutter="20">
            <el-col :span="12">
              <h3>選單樹狀結構</h3>
              <el-tree
                :data="menuTree"
                :props="{ children: 'Children', label: 'MenuName' }"
                default-expand-all
                style="margin-top: 20px"
              />
            </el-col>
            <el-col :span="12">
              <h3>我的最愛</h3>
              <el-table
                :data="favorites"
                border
                stripe
                style="margin-top: 20px"
              >
                <el-table-column prop="MenuName" label="選單名稱" />
                <el-table-column prop="MenuUrl" label="選單路徑" />
                <el-table-column label="操作" width="150">
                  <template #default="{ row }">
                    <el-button type="danger" size="small" @click="handleRemoveFavorite(row)">移除</el-button>
                  </template>
                </el-table-column>
              </el-table>
            </el-col>
          </el-row>
        </el-card>
      </el-tab-pane>

      <!-- 標題功能 -->
      <el-tab-pane label="標題功能" name="header">
        <el-card class="card" shadow="never">
          <el-descriptions :column="2" border>
            <el-descriptions-item label="系統名稱">{{ headerInfo.SystemName }}</el-descriptions-item>
            <el-descriptions-item label="系統版本">{{ headerInfo.SystemVersion }}</el-descriptions-item>
            <el-descriptions-item label="當前時間">{{ currentTime }}</el-descriptions-item>
            <el-descriptions-item label="使用者">{{ headerInfo.UserName }}</el-descriptions-item>
          </el-descriptions>
        </el-card>
      </el-tab-pane>

      <!-- 主選單功能 -->
      <el-tab-pane label="主選單功能" name="mainMenu">
        <el-card class="card" shadow="never">
          <el-row :gutter="20">
            <el-col :span="8" v-for="module in userModules" :key="module.ModuleId">
              <el-card shadow="hover" style="margin-bottom: 20px">
                <template #header>
                  <span>{{ module.ModuleName }}</span>
                </template>
                <p>{{ module.Description }}</p>
                <el-button type="primary" size="small" @click="handleViewSubsystems(module)">查看子系統</el-button>
              </el-card>
            </el-col>
          </el-row>
        </el-card>
      </el-tab-pane>

      <!-- 訊息功能 -->
      <el-tab-pane label="訊息功能" name="message">
        <el-card class="card" shadow="never">
          <el-form :inline="true" class="search-form">
            <el-form-item label="日期">
              <el-date-picker
                v-model="messageDate"
                type="date"
                placeholder="選擇日期"
                format="YYYY-MM-DD"
                value-format="YYYY-MM-DD"
              />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleSearchMessages">查詢</el-button>
            </el-form-item>
            <el-form-item>
              <el-badge :value="unreadCount" class="item">
                <el-button>未讀訊息</el-button>
              </el-badge>
            </el-form-item>
          </el-form>
          <el-table
            :data="messages"
            border
            stripe
            style="margin-top: 20px"
          >
            <el-table-column prop="MessageTitle" label="訊息標題" />
            <el-table-column prop="MessageContent" label="訊息內容" show-overflow-tooltip />
            <el-table-column prop="IsRead" label="已讀" width="100">
              <template #default="{ row }">
                <el-tag :type="row.IsRead ? 'success' : 'warning'">
                  {{ row.IsRead ? '已讀' : '未讀' }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="CreatedAt" label="建立時間" width="180" />
            <el-table-column label="操作" width="150">
              <template #default="{ row }">
                <el-button type="primary" size="small" @click="handleViewMessage(row)">查看</el-button>
              </template>
            </el-table-column>
          </el-table>
        </el-card>
      </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { coreApi } from '@/api/core'

// 標籤頁
const activeTab = ref('menu')

// 選單樹
const menuTree = ref([])

// 我的最愛
const favorites = ref([])

// 標題資訊
const headerInfo = reactive({
  SystemName: '',
  SystemVersion: '',
  UserName: ''
})
const currentTime = ref('')

// 使用者模組
const userModules = ref([])

// 訊息
const messages = ref([])
const messageDate = ref('')
const unreadCount = ref(0)

// 載入選單樹
const loadMenuTree = async () => {
  try {
    const response = await coreApi.framework.getMenuTree()
    if (response.data.success) {
      menuTree.value = response.data.data || []
    } else {
      ElMessage.error(response.data.message || '載入選單樹失敗')
    }
  } catch (error) {
    ElMessage.error('載入選單樹失敗：' + error.message)
  }
}

// 載入我的最愛
const loadFavorites = async () => {
  try {
    const response = await coreApi.framework.getFavorites()
    if (response.data.success) {
      favorites.value = response.data.data || []
    } else {
      ElMessage.error(response.data.message || '載入我的最愛失敗')
    }
  } catch (error) {
    ElMessage.error('載入我的最愛失敗：' + error.message)
  }
}

// 移除我的最愛
const handleRemoveFavorite = async (row) => {
  try {
    const response = await coreApi.framework.removeFavorite(row.FavoriteId)
    if (response.data.success) {
      ElMessage.success('移除成功')
      loadFavorites()
    } else {
      ElMessage.error(response.data.message || '移除失敗')
    }
  } catch (error) {
    ElMessage.error('移除失敗：' + error.message)
  }
}

// 載入標題資訊
const loadHeaderInfo = async () => {
  try {
    const response = await coreApi.framework.getHeaderInfo()
    if (response.data.success) {
      Object.assign(headerInfo, response.data.data)
    } else {
      ElMessage.error(response.data.message || '載入標題資訊失敗')
    }
  } catch (error) {
    ElMessage.error('載入標題資訊失敗：' + error.message)
  }
}

// 載入當前時間
const loadCurrentTime = async () => {
  try {
    const response = await coreApi.framework.getCurrentTime()
    if (response.data.success) {
      currentTime.value = response.data.data
    }
  } catch (error) {
    console.error('載入當前時間失敗：' + error.message)
  }
}

// 載入使用者模組
const loadUserModules = async () => {
  try {
    const response = await coreApi.framework.getUserModules()
    if (response.data.success) {
      userModules.value = response.data.data || []
    } else {
      ElMessage.error(response.data.message || '載入使用者模組失敗')
    }
  } catch (error) {
    ElMessage.error('載入使用者模組失敗：' + error.message)
  }
}

// 查看子系統
const handleViewSubsystems = async (module) => {
  try {
    const response = await coreApi.framework.getModuleSubsystems(module.ModuleId)
    if (response.data.success) {
      ElMessage.info(`子系統數量：${response.data.data.length}`)
    } else {
      ElMessage.error(response.data.message || '載入子系統失敗')
    }
  } catch (error) {
    ElMessage.error('載入子系統失敗：' + error.message)
  }
}

// 查詢訊息
const handleSearchMessages = async () => {
  try {
    const params = messageDate.value ? { Date: messageDate.value } : {}
    const response = await coreApi.framework.getMessagesByDate(params)
    if (response.data.success) {
      messages.value = response.data.data || []
    } else {
      ElMessage.error(response.data.message || '查詢訊息失敗')
    }
  } catch (error) {
    ElMessage.error('查詢訊息失敗：' + error.message)
  }
}

// 查看訊息
const handleViewMessage = async (row) => {
  try {
    const response = await coreApi.framework.getMessage(row.MessageId)
    if (response.data.success) {
      ElMessage.info('訊息內容：' + response.data.data.MessageContent)
      if (!row.IsRead) {
        await coreApi.framework.markAsRead(row.MessageId)
        loadUnreadCount()
        handleSearchMessages()
      }
    } else {
      ElMessage.error(response.data.message || '載入訊息失敗')
    }
  } catch (error) {
    ElMessage.error('載入訊息失敗：' + error.message)
  }
}

// 載入未讀數量
const loadUnreadCount = async () => {
  try {
    const response = await coreApi.framework.getUnreadCount()
    if (response.data.success) {
      unreadCount.value = response.data.data || 0
    }
  } catch (error) {
    console.error('載入未讀數量失敗：' + error.message)
  }
}

// 初始化
onMounted(() => {
  loadMenuTree()
  loadFavorites()
  loadHeaderInfo()
  loadCurrentTime()
  loadUserModules()
  handleSearchMessages()
  loadUnreadCount()
  // 定時更新當前時間
  setInterval(() => {
    loadCurrentTime()
  }, 1000)
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.framework {
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

  .card {
    margin-top: 20px;
    background-color: $card-bg;
    border-color: $card-border;
    color: $card-text;
  }

  .search-form {
    margin: 0;
  }
}
</style>

