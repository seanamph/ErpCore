# ErpCore API 文件

## 說明
本文檔提供 ErpCore Web API 的完整 API 文件說明。

## API 基礎資訊

### Base URL
- 開發環境: `https://localhost:5001`
- 生產環境: `https://api.erpcore.com`

### 認證方式
所有 API 請求都需要在 Header 中帶入 JWT Token：
```
Authorization: Bearer {token}
```

## API 端點

### Swagger 文件
- Swagger UI: `https://localhost:5001/swagger`
- Swagger JSON: `https://localhost:5001/swagger/v1/swagger.json`

## API 規範

### RESTful 規範
- GET: 查詢資料
- POST: 新增資料
- PUT: 更新資料
- DELETE: 刪除資料

### 回應格式
所有 API 回應統一使用以下格式：
```json
{
  "success": true,
  "data": {},
  "message": "",
  "errors": []
}
```

### 錯誤處理
- 400: Bad Request - 請求參數錯誤
- 401: Unauthorized - 未授權
- 403: Forbidden - 無權限
- 404: Not Found - 資源不存在
- 500: Internal Server Error - 伺服器錯誤

## 各模組 API

詳細的 API 端點請參考 Swagger 文件或各模組的開發計劃文件。

