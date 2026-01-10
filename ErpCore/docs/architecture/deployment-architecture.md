# 部署架構文件

## 概述

本文檔描述 ErpCore 系統的部署架構。

## 部署架構

### 後端部署
- **運行環境**: .NET 7.0
- **Web 伺服器**: Kestrel
- **反向代理**: Nginx / IIS
- **資料庫**: SQL Server

### 前端部署
- **運行環境**: Node.js
- **建置工具**: Vue CLI
- **Web 伺服器**: Nginx

### Docker 部署
- 使用 Docker Compose 進行容器化部署
- 後端和前端分別使用不同的 Dockerfile
- 支援開發環境和生產環境配置

### CI/CD
- 使用 GitHub Actions 或 GitLab CI/CD
- 自動化建置、測試和部署流程

## 部署環境

### 開發環境
- 本地開發環境
- 使用 Docker Compose 快速啟動

### 測試環境
- 用於功能測試和整合測試
- 與生產環境配置相似

### 生產環境
- 正式運行環境
- 高可用性和效能優化配置

