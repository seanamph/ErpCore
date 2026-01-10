<template>
  <div class="system-exit">
    <div class="page-header">
      <h1>系統退出 (SYS9999)</h1>
    </div>

    <el-card class="exit-card" shadow="never">
      <div class="exit-content">
        <el-icon :size="64" class="exit-icon"><SwitchButton /></el-icon>
        <h2>確定要退出系統嗎？</h2>
        <p>退出後將清除所有登入資訊，需要重新登入才能使用系統</p>
        <div class="exit-actions">
          <el-button type="danger" size="large" @click="handleExit">確定退出</el-button>
          <el-button size="large" @click="handleCancel">取消</el-button>
        </div>
      </div>
    </el-card>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { SwitchButton } from '@element-plus/icons-vue'
import { useRouter } from 'vue-router'
import axios from '@/api/axios'

const router = useRouter()

// API
const systemExitApi = {
  exit: () => axios.post('/api/v1/system-exit')
}

// 退出系統
const handleExit = async () => {
  try {
    await ElMessageBox.confirm('確定要退出系統嗎？', '確認退出', {
      type: 'warning',
      confirmButtonText: '確定',
      cancelButtonText: '取消'
    })
    
    try {
      await systemExitApi.exit()
    } catch (error) {
      // 即使API失敗也繼續退出流程
      console.error('退出API失敗：', error)
    }
    
    // 清除本地儲存的登入資訊
    localStorage.removeItem('token')
    localStorage.removeItem('userInfo')
    sessionStorage.clear()
    
    ElMessage.success('已退出系統')
    
    // 跳轉到登入頁
    router.push('/login')
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('退出失敗：' + error.message)
    }
  }
}

// 取消
const handleCancel = () => {
  router.back()
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.system-exit {
  padding: 20px;
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 60vh;

  .page-header {
    position: absolute;
    top: 20px;
    left: 20px;
    
    h1 {
      font-size: 24px;
      font-weight: bold;
      color: $primary-color;
      margin: 0;
    }
  }

  .exit-card {
    max-width: 500px;
    width: 100%;

    .exit-content {
      text-align: center;
      padding: 40px 20px;

      .exit-icon {
        color: $danger-color;
        margin-bottom: 20px;
      }

      h2 {
        font-size: 24px;
        color: $text-color-primary;
        margin-bottom: 20px;
      }

      p {
        font-size: 16px;
        color: $text-color-secondary;
        margin-bottom: 30px;
      }

      .exit-actions {
        display: flex;
        justify-content: center;
        gap: 20px;
      }
    }
  }
}
</style>

