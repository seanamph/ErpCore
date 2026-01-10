# 測試指南文件

## 概述

本文檔描述 ErpCore 專案的測試策略和指南。

## 測試類型

### 單元測試
- **位置**: `tests/ErpCore.UnitTests`
- **目標**: 測試個別方法和類別的邏輯
- **工具**: xUnit、Moq

### 整合測試
- **位置**: `tests/ErpCore.IntegrationTests`
- **目標**: 測試 API 端點和資料庫操作
- **工具**: xUnit、TestServer

### 端對端測試
- **位置**: `tests/ErpCore.E2ETests`
- **目標**: 測試完整的使用者流程
- **工具**: Selenium、Playwright

## 測試規範

### 單元測試範例
```csharp
[Fact]
public async Task GetUserById_ShouldReturnUser_WhenUserExists()
{
    // Arrange
    var userId = 1;
    var expectedUser = new User { UserId = userId, UserName = "test" };
    
    // Act
    var result = await _userService.GetUserByIdAsync(userId);
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal(expectedUser.UserId, result.UserId);
}
```

### 整合測試範例
```csharp
[Fact]
public async Task GetUsers_ShouldReturnOk_WhenRequestIsValid()
{
    // Arrange
    var client = _factory.CreateClient();
    
    // Act
    var response = await client.GetAsync("/api/v1/users");
    
    // Assert
    response.EnsureSuccessStatusCode();
}
```

## 測試覆蓋率

### 目標
- 單元測試覆蓋率：80% 以上
- 整合測試：所有 API 端點
- 端對端測試：主要使用者流程

### 執行測試
```bash
# 執行所有測試
dotnet test

# 執行單元測試
dotnet test tests/ErpCore.UnitTests

# 執行整合測試
dotnet test tests/ErpCore.IntegrationTests

# 執行端對端測試
dotnet test tests/ErpCore.E2ETests
```

## 測試資料

### 測試資料庫
- 使用獨立的測試資料庫
- 每個測試執行前重置資料
- 使用種子資料初始化

### Mock 資料
- 使用 Moq 建立 Mock 物件
- 模擬外部服務和依賴

## CI/CD 整合

### GitHub Actions
- 每次推送自動執行測試
- 測試失敗阻止合併

### GitLab CI/CD
- 自動執行測試流程
- 測試報告整合

