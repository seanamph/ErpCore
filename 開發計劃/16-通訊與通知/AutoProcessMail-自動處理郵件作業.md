# AutoProcessMail - 自動處理郵件作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: AutoProcessMail
- **功能名稱**: 自動處理郵件作業
- **功能描述**: 提供系統自動處理郵件的功能，包含郵件佇列處理、批次發送、重試機制、錯誤處理等
- **參考舊程式**: 
  - `WEB/IMS_CORE/SYS5000/AutoProcessMail.aspx`
  - `WEB/IMS_CORE/ASP/SYS2000/AUTOMAIL/Mail_Service.asp`
  - `WEB/IMS_CORE/ASP/SYS2000/AUTOMAIL/SYS2000_EMAIL.ASP`
  - `WEB/IMS_CORE/ASP/SYS2000/AUTOMAIL/SYS2000_EMAIL_CDO.ASP`
  - `開發計劃/16-通訊與通知/SYS5000-郵件簡訊發送作業.md`

### 1.2 業務需求
- 自動處理待發送郵件佇列
- 支援批次發送郵件
- 支援郵件發送重試機制
- 支援郵件發送錯誤處理
- 支援郵件發送優先權處理
- 支援郵件發送記錄
- 支援郵件發送統計
- 支援排程任務整合（Hangfire/Quartz.NET）

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `EmailLogs` (郵件發送記錄)
參考 `開發計劃/16-通訊與通知/SYS5000-郵件簡訊發送作業.md` 的 `EmailLogs` 資料表設計

### 2.2 郵件處理佇列表: `EmailQueue` (郵件佇列)

```sql
CREATE TABLE [dbo].[EmailQueue] (
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [EmailLogId] BIGINT NOT NULL, -- 郵件記錄ID
    [Priority] INT NOT NULL DEFAULT 3, -- 優先權 (1-5, 1最高)
    [RetryCount] INT NOT NULL DEFAULT 0, -- 重試次數
    [MaxRetryCount] INT NOT NULL DEFAULT 3, -- 最大重試次數
    [Status] NVARCHAR(20) NOT NULL DEFAULT 'Pending', -- 狀態 (Pending/Processing/Sent/Failed)
    [NextRetryAt] DATETIME2 NULL, -- 下次重試時間
    [ProcessedAt] DATETIME2 NULL, -- 處理時間
    [ErrorMessage] NVARCHAR(MAX) NULL, -- 錯誤訊息
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    CONSTRAINT [FK_EmailQueue_EmailLogs] FOREIGN KEY ([EmailLogId]) REFERENCES [dbo].[EmailLogs] ([Id]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_EmailQueue_Status] ON [dbo].[EmailQueue] ([Status]);
CREATE NONCLUSTERED INDEX [IX_EmailQueue_Priority] ON [dbo].[EmailQueue] ([Priority], [CreatedAt]);
CREATE NONCLUSTERED INDEX [IX_EmailQueue_NextRetryAt] ON [dbo].[EmailQueue] ([NextRetryAt]) WHERE [Status] = 'Pending';
```

### 2.3 資料字典

#### EmailQueue 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| Id | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| EmailLogId | BIGINT | - | NO | - | 郵件記錄ID | 外鍵 |
| Priority | INT | - | NO | 3 | 優先權 | 1-5, 1最高 |
| RetryCount | INT | - | NO | 0 | 重試次數 | - |
| MaxRetryCount | INT | - | NO | 3 | 最大重試次數 | - |
| Status | NVARCHAR | 20 | NO | 'Pending' | 狀態 | Pending/Processing/Sent/Failed |
| NextRetryAt | DATETIME2 | - | YES | - | 下次重試時間 | - |
| ProcessedAt | DATETIME2 | - | YES | - | 處理時間 | - |
| ErrorMessage | NVARCHAR(MAX) | - | YES | - | 錯誤訊息 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 處理郵件佇列
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/email/process-queue`
- **說明**: 處理郵件佇列中的待發送郵件（通常由排程任務呼叫）
- **請求參數**:
  ```json
  {
    "batchSize": 100,
    "maxRetryCount": 3
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "處理完成",
    "data": {
      "processedCount": 95,
      "successCount": 90,
      "failedCount": 5,
      "retryCount": 3
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 重試失敗郵件
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/email/retry-failed`
- **說明**: 重試失敗的郵件
- **請求參數**:
  ```json
  {
    "emailQueueIds": [1, 2, 3],
    "maxRetryCount": 3
  }
  ```
- **回應格式**: 同處理郵件佇列

#### 3.1.3 查詢郵件佇列狀態
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/email/queue-status`
- **說明**: 查詢郵件佇列狀態統計
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "pendingCount": 10,
      "processingCount": 5,
      "sentCount": 1000,
      "failedCount": 5,
      "totalCount": 1020
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

---

## 四、後端實作

### 4.1 Service (`EmailQueueService.cs`)

```csharp
public class EmailQueueService : IEmailQueueService
{
    private readonly IDbConnection _db;
    private readonly IEmailService _emailService;
    private readonly ILogger<EmailQueueService> _logger;

    public EmailQueueService(
        IDbConnection db,
        IEmailService emailService,
        ILogger<EmailQueueService> logger)
    {
        _db = db;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<EmailQueueProcessResult> ProcessQueueAsync(int batchSize = 100, int maxRetryCount = 3)
    {
        var result = new EmailQueueProcessResult();

        // 取得待處理的郵件（優先權高的先處理）
        var sql = @"
            SELECT TOP (@BatchSize)
                eq.Id,
                eq.EmailLogId,
                eq.Priority,
                eq.RetryCount,
                eq.MaxRetryCount,
                el.FromAddress,
                el.FromName,
                el.ToAddress,
                el.CcAddress,
                el.BccAddress,
                el.Subject,
                el.Body,
                el.BodyType,
                el.Priority,
                el.SmtpServer,
                el.SmtpPort
            FROM EmailQueue eq
            INNER JOIN EmailLogs el ON eq.EmailLogId = el.Id
            WHERE eq.Status = 'Pending'
                AND (eq.NextRetryAt IS NULL OR eq.NextRetryAt <= GETDATE())
            ORDER BY eq.Priority ASC, eq.CreatedAt ASC;
        ";

        var pendingEmails = await _db.QueryAsync<EmailQueueItem>(sql, new { BatchSize = batchSize });

        foreach (var emailItem in pendingEmails)
        {
            try
            {
                // 更新狀態為處理中
                await UpdateQueueStatusAsync(emailItem.Id, "Processing");

                // 發送郵件
                var sendResult = await _emailService.SendEmailAsync(new SendEmailDto
                {
                    FromAddress = emailItem.FromAddress,
                    FromName = emailItem.FromName,
                    ToAddress = emailItem.ToAddress,
                    CcAddress = emailItem.CcAddress,
                    BccAddress = emailItem.BccAddress,
                    Subject = emailItem.Subject,
                    Body = emailItem.Body,
                    BodyType = emailItem.BodyType,
                    Priority = emailItem.Priority,
                    SmtpServer = emailItem.SmtpServer,
                    SmtpPort = emailItem.SmtpPort
                });

                if (sendResult.Success)
                {
                    // 發送成功
                    await UpdateQueueStatusAsync(emailItem.Id, "Sent", sendResult.SentAt);
                    await UpdateEmailLogStatusAsync(emailItem.EmailLogId, "Sent", sendResult.SentAt);
                    result.SuccessCount++;
                }
                else
                {
                    // 發送失敗，檢查是否需要重試
                    if (emailItem.RetryCount < emailItem.MaxRetryCount)
                    {
                        // 計算下次重試時間（指數退避）
                        var nextRetryAt = DateTime.Now.AddMinutes(Math.Pow(2, emailItem.RetryCount));
                        await UpdateQueueRetryAsync(emailItem.Id, emailItem.RetryCount + 1, nextRetryAt, sendResult.ErrorMessage);
                        result.RetryCount++;
                    }
                    else
                    {
                        // 超過最大重試次數，標記為失敗
                        await UpdateQueueStatusAsync(emailItem.Id, "Failed", null, sendResult.ErrorMessage);
                        await UpdateEmailLogStatusAsync(emailItem.EmailLogId, "Failed", null, sendResult.ErrorMessage);
                        result.FailedCount++;
                    }
                }

                result.ProcessedCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "處理郵件佇列項目失敗: {EmailQueueId}", emailItem.Id);

                // 檢查是否需要重試
                if (emailItem.RetryCount < emailItem.MaxRetryCount)
                {
                    var nextRetryAt = DateTime.Now.AddMinutes(Math.Pow(2, emailItem.RetryCount));
                    await UpdateQueueRetryAsync(emailItem.Id, emailItem.RetryCount + 1, nextRetryAt, ex.Message);
                    result.RetryCount++;
                }
                else
                {
                    await UpdateQueueStatusAsync(emailItem.Id, "Failed", null, ex.Message);
                    await UpdateEmailLogStatusAsync(emailItem.EmailLogId, "Failed", null, ex.Message);
                    result.FailedCount++;
                }

                result.ProcessedCount++;
            }
        }

        return result;
    }

    public async Task<EmailQueueProcessResult> RetryFailedEmailsAsync(List<long> emailQueueIds, int maxRetryCount = 3)
    {
        var result = new EmailQueueProcessResult();

        foreach (var queueId in emailQueueIds)
        {
            try
            {
                // 重置佇列項目狀態
                var sql = @"
                    UPDATE EmailQueue 
                    SET Status = 'Pending',
                        RetryCount = 0,
                        NextRetryAt = GETDATE(),
                        ErrorMessage = NULL
                    WHERE Id = @QueueId AND Status = 'Failed';
                ";

                await _db.ExecuteAsync(sql, new { QueueId = queueId });

                result.ProcessedCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "重試郵件失敗: {EmailQueueId}", queueId);
                result.FailedCount++;
            }
        }

        // 處理重試的郵件
        var processResult = await ProcessQueueAsync(emailQueueIds.Count, maxRetryCount);
        result.SuccessCount += processResult.SuccessCount;
        result.RetryCount += processResult.RetryCount;
        result.FailedCount += processResult.FailedCount;

        return result;
    }

    public async Task<EmailQueueStatusDto> GetQueueStatusAsync()
    {
        var sql = @"
            SELECT 
                SUM(CASE WHEN Status = 'Pending' THEN 1 ELSE 0 END) AS PendingCount,
                SUM(CASE WHEN Status = 'Processing' THEN 1 ELSE 0 END) AS ProcessingCount,
                SUM(CASE WHEN Status = 'Sent' THEN 1 ELSE 0 END) AS SentCount,
                SUM(CASE WHEN Status = 'Failed' THEN 1 ELSE 0 END) AS FailedCount,
                COUNT(*) AS TotalCount
            FROM EmailQueue;
        ";

        return await _db.QueryFirstOrDefaultAsync<EmailQueueStatusDto>(sql);
    }

    private async Task UpdateQueueStatusAsync(long queueId, string status, DateTime? processedAt = null, string errorMessage = null)
    {
        var sql = @"
            UPDATE EmailQueue 
            SET Status = @Status,
                ProcessedAt = @ProcessedAt,
                ErrorMessage = @ErrorMessage
            WHERE Id = @QueueId;
        ";

        await _db.ExecuteAsync(sql, new
        {
            QueueId = queueId,
            Status = status,
            ProcessedAt = processedAt,
            ErrorMessage = errorMessage
        });
    }

    private async Task UpdateQueueRetryAsync(long queueId, int retryCount, DateTime nextRetryAt, string errorMessage = null)
    {
        var sql = @"
            UPDATE EmailQueue 
            SET Status = 'Pending',
                RetryCount = @RetryCount,
                NextRetryAt = @NextRetryAt,
                ErrorMessage = @ErrorMessage
            WHERE Id = @QueueId;
        ";

        await _db.ExecuteAsync(sql, new
        {
            QueueId = queueId,
            RetryCount = retryCount,
            NextRetryAt = nextRetryAt,
            ErrorMessage = errorMessage
        });
    }

    private async Task UpdateEmailLogStatusAsync(long emailLogId, string status, DateTime? sentAt = null, string errorMessage = null)
    {
        var sql = @"
            UPDATE EmailLogs 
            SET Status = @Status,
                SentAt = @SentAt,
                ErrorMessage = @ErrorMessage
            WHERE Id = @EmailLogId;
        ";

        await _db.ExecuteAsync(sql, new
        {
            EmailLogId = emailLogId,
            Status = status,
            SentAt = sentAt,
            ErrorMessage = errorMessage
        });
    }
}
```

### 4.2 排程任務整合 (Hangfire)

```csharp
// 在 Startup.cs 或 Program.cs 中設定
public void ConfigureServices(IServiceCollection services)
{
    // ... 其他服務設定

    // 設定 Hangfire
    services.AddHangfire(config => config
        .UseSqlServerStorage(connectionString));

    services.AddHangfireServer();
}

public void Configure(IApplicationBuilder app)
{
    // ... 其他設定

    app.UseHangfireDashboard();

    // 設定排程任務：每5分鐘處理一次郵件佇列
    RecurringJob.AddOrUpdate<IEmailQueueService>(
        "process-email-queue",
        service => service.ProcessQueueAsync(100, 3),
        "*/5 * * * *"); // Cron 表達式：每5分鐘
}
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (0.5天)
- [ ] 建立 EmailQueue 資料表
- [ ] 建立索引
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (3天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] EmailQueueService 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 排程任務整合（Hangfire/Quartz.NET）
- [ ] 單元測試

### 5.3 階段三: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 排程任務測試
- [ ] 重試機制測試
- [ ] 效能測試

**總計**: 4.5天

---

## 六、注意事項

### 6.1 效能考量
- 批次處理時需控制批次大小
- 大量郵件時考慮使用訊息佇列（RabbitMQ）
- 考慮使用分散式鎖避免重複處理

### 6.2 錯誤處理
- 實作指數退避重試機制
- 記錄詳細錯誤訊息
- 提供失敗郵件重試功能

### 6.3 監控與告警
- 監控佇列長度
- 監控處理速度
- 監控失敗率
- 設定告警機制

---

## 七、測試案例

### 7.1 功能測試
1. **佇列處理測試**
   - 正常郵件發送
   - 失敗郵件重試
   - 超過最大重試次數處理
   - 優先權處理

2. **排程任務測試**
   - 排程任務執行
   - 批次處理
   - 錯誤處理

### 7.2 效能測試
- 大量郵件處理效能
- 批次處理效能
- 重試機制效能

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

