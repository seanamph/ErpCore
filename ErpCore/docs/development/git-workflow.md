# Git 工作流程文件

## 概述

本文檔描述 ErpCore 專案的 Git 工作流程。

## 分支策略

### 主要分支
- **main**: 生產環境分支，只接受合併請求
- **develop**: 開發分支，用於整合功能

### 功能分支
- **feature/功能名稱**: 新功能開發
- **bugfix/問題描述**: 錯誤修正
- **hotfix/問題描述**: 緊急修正

## 工作流程

### 1. 建立功能分支
```bash
git checkout develop
git pull origin develop
git checkout -b feature/新功能名稱
```

### 2. 開發和提交
```bash
git add .
git commit -m "feat: 功能描述"
```

### 3. 推送分支
```bash
git push origin feature/新功能名稱
```

### 4. 建立 Pull Request
- 在 GitHub/GitLab 建立 Pull Request
- 目標分支：`develop`
- 填寫 PR 描述和檢查清單

### 5. 程式碼審查
- 等待審查者審查
- 根據審查意見修改

### 6. 合併到 develop
- 審查通過後合併
- 刪除功能分支

### 7. 發布到 main
- 從 `develop` 合併到 `main`
- 建立版本標籤

## 提交訊息規範

### 格式
```
<type>(<scope>): <subject>

<body>

<footer>
```

### Type 類型
- **feat**: 新功能
- **fix**: 錯誤修正
- **docs**: 文件更新
- **style**: 程式碼格式調整
- **refactor**: 重構
- **test**: 測試相關
- **chore**: 建置或工具變更

### 範例
```
feat(Users): 新增使用者管理功能

- 新增使用者 CRUD 操作
- 新增使用者權限設定
- 新增使用者查詢功能

Closes #123
```

## 程式碼審查檢查清單

- [ ] 程式碼符合編碼標準
- [ ] 所有程式碼都有 try-catch 和 NLog 日誌
- [ ] API 欄位命名統一（PascalCase）
- [ ] Vue 前端遵循 C# API 欄位命名
- [ ] 資料庫存取使用 Dapper
- [ ] 測試通過
- [ ] 文件已更新

