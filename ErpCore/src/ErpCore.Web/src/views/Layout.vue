<template>
  <div class="layout">
    <el-container>
      <el-header>
        <h1>ErpCore 系統</h1>
      </el-header>
      <el-container>
        <el-aside width="200px">
          <el-menu
            :default-active="activeMenu"
            router
            mode="vertical"
          >
            <el-menu-item index="/transfer/receipt">
              <span>調撥單驗收作業</span>
            </el-menu-item>
            <el-menu-item index="/transfer/return">
              <span>調撥單驗退作業</span>
            </el-menu-item>
            <el-menu-item index="/transfer/shortage">
              <span>調撥短溢維護作業</span>
            </el-menu-item>
          </el-menu>
        </el-aside>
        <el-main>
          <router-view />
        </el-main>
      </el-container>
    </el-container>
  </div>
</template>

<script>
export default {
  name: 'Layout',
  computed: {
    activeMenu() {
      return this.$route.path
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.layout {
  height: 100vh;
  
  .el-header {
    background-color: $primary-color;
    color: $badge-primary-text;
    display: flex;
    align-items: center;
    padding: 0 20px;
    
    h1 {
      margin: 0;
      font-size: 20px;
      color: $badge-primary-text;
    }
  }
  
  .el-aside {
    background-color: $card-bg;
    border-right: 1px solid $border-color-base;
  }
  
  .el-main {
    background-color: $background-color-base;
    padding: 20px;
  }
}
</style>

