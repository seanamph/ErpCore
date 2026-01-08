# FrameMessage - 框架訊息功能 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: FrameMessage
- **功能名稱**: 框架訊息功能
- **功能描述**: 提供系統框架的訊息顯示區域，包含日曆顯示、訊息列表顯示功能，用於顯示系統公告、通知等訊息
- **參考舊程式**: 
  - `IMS3/HANSHIN/IMS3/KERNEL/FrameMessage.aspx`
  - `IMS3/HANSHIN/IMS3/KERNEL/FrameMessage.aspx.cs`

### 1.2 業務需求
- 顯示日曆元件，用於選擇日期查看該日期的訊息
- 顯示訊息列表區域，用於顯示系統公告、通知等訊息
- 支援訊息分類顯示（系統公告、個人通知等）
- 支援訊息標記已讀/未讀狀態
- 支援訊息點擊查看詳情
- 支援訊息搜尋功能
- 支援多語言顯示
- 支援頁面轉場效果設定

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `SystemMessages` (系統訊息)

```sql
CREATE TABLE [dbo].[SystemMessages] (
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [MessageId] NVARCHAR(50) NOT NULL UNIQUE,
    [Title] NVARCHAR(200) NOT NULL,
    [Content] NVARCHAR(MAX) NULL,
    [MessageType] NVARCHAR(20) NOT NULL, -- SYSTEM, PERSONAL, ANNOUNCEMENT
    [Priority] NVARCHAR(10) NOT NULL DEFAULT 'NORMAL', -- HIGH, NORMAL, LOW
    [StartDate] DATETIME2 NOT NULL,
    [EndDate] DATETIME2 NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NULL
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_SystemMessages_MessageType] ON [dbo].[SystemMessages] ([MessageType]);
CREATE NONCLUSTERED INDEX [IX_SystemMessages_StartDate] ON [dbo].[SystemMessages] ([StartDate]);
CREATE NONCLUSTERED INDEX [IX_SystemMessages_EndDate] ON [dbo].[SystemMessages] ([EndDate]);
CREATE NONCLUSTERED INDEX [IX_SystemMessages_IsActive] ON [dbo].[SystemMessages] ([IsActive]);
```

### 2.2 相關資料表

#### 2.2.1 `UserMessageReads` - 使用者訊息已讀記錄

```sql
CREATE TABLE [dbo].[UserMessageReads] (
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [UserId] NVARCHAR(50) NOT NULL,
    [MessageId] NVARCHAR(50) NOT NULL,
    [ReadAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_UserMessageReads_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserMessageReads_SystemMessages] FOREIGN KEY ([MessageId]) REFERENCES [dbo].[SystemMessages] ([MessageId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_UserMessageReads_UserId] ON [dbo].[UserMessageReads] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_UserMessageReads_MessageId] ON [dbo].[UserMessageReads] ([MessageId]);
CREATE UNIQUE NONCLUSTERED INDEX [IX_UserMessageReads_UserId_MessageId] ON [dbo].[UserMessageReads] ([UserId], [MessageId]);
```

#### 2.2.2 `UserMessages` - 個人訊息

```sql
CREATE TABLE [dbo].[UserMessages] (
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [MessageId] NVARCHAR(50) NOT NULL UNIQUE,
    [UserId] NVARCHAR(50) NOT NULL,
    [Title] NVARCHAR(200) NOT NULL,
    [Content] NVARCHAR(MAX) NULL,
    [IsRead] BIT NOT NULL DEFAULT 0,
    [ReadAt] DATETIME2 NULL,
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_UserMessages_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_UserMessages_UserId] ON [dbo].[UserMessages] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_UserMessages_IsRead] ON [dbo].[UserMessages] ([IsRead]);
CREATE NONCLUSTERED INDEX [IX_UserMessages_CreatedAt] ON [dbo].[UserMessages] ([CreatedAt]);
```

### 2.3 資料字典

#### SystemMessages 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| Id | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| MessageId | NVARCHAR | 50 | NO | - | 訊息編號 | 唯一值 |
| Title | NVARCHAR | 200 | NO | - | 訊息標題 | - |
| Content | NVARCHAR | MAX | YES | - | 訊息內容 | - |
| MessageType | NVARCHAR | 20 | NO | - | 訊息類型 | SYSTEM, PERSONAL, ANNOUNCEMENT |
| Priority | NVARCHAR | 10 | NO | NORMAL | 優先級 | HIGH, NORMAL, LOW |
| StartDate | DATETIME2 | - | NO | - | 開始日期 | - |
| EndDate | DATETIME2 | - | YES | - | 結束日期 | - |
| IsActive | BIT | - | NO | 1 | 是否啟用 | 1:是, 0:否 |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | YES | - | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢訊息列表

- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/messages`
- **說明**: 查詢訊息列表，支援分頁、篩選
- **請求參數**:
  - `page`: 頁碼 (預設: 1)
  - `pageSize`: 每頁筆數 (預設: 20)
  - `messageType`: 訊息類型 (SYSTEM, PERSONAL, ANNOUNCEMENT)
  - `startDate`: 開始日期
  - `endDate`: 結束日期
  - `isRead`: 是否已讀 (true/false)
  - `keyword`: 關鍵字搜尋
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "items": [
        {
          "messageId": "MSG001",
          "title": "系統維護通知",
          "content": "系統將於本週末進行維護...",
          "messageType": "SYSTEM",
          "priority": "HIGH",
          "startDate": "2024-01-01T00:00:00Z",
          "endDate": "2024-01-31T23:59:59Z",
          "isRead": false,
          "createdAt": "2024-01-01T00:00:00Z"
        }
      ],
      "total": 100,
      "page": 1,
      "pageSize": 20
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 查詢單筆訊息

- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/messages/{messageId}`
- **說明**: 根據訊息編號查詢單筆訊息詳情
- **路徑參數**:
  - `messageId`: 訊息編號
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "messageId": "MSG001",
      "title": "系統維護通知",
      "content": "系統將於本週末進行維護...",
      "messageType": "SYSTEM",
      "priority": "HIGH",
      "startDate": "2024-01-01T00:00:00Z",
      "endDate": "2024-01-31T23:59:59Z",
      "isRead": false,
      "createdAt": "2024-01-01T00:00:00Z",
      "createdBy": "ADMIN"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 標記訊息為已讀

- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/messages/{messageId}/read`
- **說明**: 標記訊息為已讀
- **路徑參數**:
  - `messageId`: 訊息編號
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "標記成功",
    "data": null,
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 批次標記訊息為已讀

- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/messages/batch-read`
- **說明**: 批次標記多筆訊息為已讀
- **請求格式**:
  ```json
  {
    "messageIds": ["MSG001", "MSG002", "MSG003"]
  }
  ```
- **回應格式**: 同單筆標記

#### 3.1.5 查詢日期範圍內的訊息

- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/messages/by-date`
- **說明**: 根據日期範圍查詢訊息
- **請求參數**:
  - `date`: 日期 (格式: yyyy-MM-dd)
  - `startDate`: 開始日期
  - `endDate`: 結束日期
- **回應格式**: 同訊息列表

#### 3.1.6 查詢未讀訊息數量

- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/messages/unread-count`
- **說明**: 查詢當前使用者的未讀訊息數量
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "unreadCount": 5,
      "unreadByType": {
        "SYSTEM": 2,
        "PERSONAL": 3,
        "ANNOUNCEMENT": 0
      }
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `MessagesController.cs`

```csharp
[ApiController]
[Route("api/v1/messages")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessagesController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<MessageDto>>>> GetMessages(
        [FromQuery] MessageQueryDto query)
    {
        var result = await _messageService.GetMessagesAsync(query);
        return Ok(ApiResponse.Success(result));
    }

    [HttpGet("{messageId}")]
    public async Task<ActionResult<ApiResponse<MessageDto>>> GetMessage(string messageId)
    {
        var result = await _messageService.GetMessageAsync(messageId);
        if (result == null)
            return NotFound(ApiResponse.Error("訊息不存在"));
        return Ok(ApiResponse.Success(result));
    }

    [HttpPost("{messageId}/read")]
    public async Task<ActionResult<ApiResponse>> MarkAsRead(string messageId)
    {
        await _messageService.MarkAsReadAsync(messageId);
        return Ok(ApiResponse.Success());
    }

    [HttpPost("batch-read")]
    public async Task<ActionResult<ApiResponse>> BatchMarkAsRead([FromBody] BatchReadDto dto)
    {
        await _messageService.BatchMarkAsReadAsync(dto.MessageIds);
        return Ok(ApiResponse.Success());
    }

    [HttpGet("by-date")]
    public async Task<ActionResult<ApiResponse<List<MessageDto>>>> GetMessagesByDate(
        [FromQuery] DateTime? date,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var result = await _messageService.GetMessagesByDateAsync(date, startDate, endDate);
        return Ok(ApiResponse.Success(result));
    }

    [HttpGet("unread-count")]
    public async Task<ActionResult<ApiResponse<UnreadCountDto>>> GetUnreadCount()
    {
        var result = await _messageService.GetUnreadCountAsync();
        return Ok(ApiResponse.Success(result));
    }
}
```

#### 3.2.2 Service: `IMessageService.cs` / `MessageService.cs`

```csharp
public interface IMessageService
{
    Task<PagedResult<MessageDto>> GetMessagesAsync(MessageQueryDto query);
    Task<MessageDto> GetMessageAsync(string messageId);
    Task MarkAsReadAsync(string messageId);
    Task BatchMarkAsReadAsync(List<string> messageIds);
    Task<List<MessageDto>> GetMessagesByDateAsync(DateTime? date, DateTime? startDate, DateTime? endDate);
    Task<UnreadCountDto> GetUnreadCountAsync();
}

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;
    private readonly ICurrentUserService _currentUserService;

    public MessageService(
        IMessageRepository messageRepository,
        ICurrentUserService currentUserService)
    {
        _messageRepository = messageRepository;
        _currentUserService = currentUserService;
    }

    // 實作方法...
}
```

#### 3.2.3 Repository: `IMessageRepository.cs` / `MessageRepository.cs`

```csharp
public interface IMessageRepository
{
    Task<PagedResult<Message>> GetMessagesAsync(MessageQuery query);
    Task<Message> GetMessageAsync(string messageId);
    Task<bool> IsReadAsync(string userId, string messageId);
    Task MarkAsReadAsync(string userId, string messageId);
    Task BatchMarkAsReadAsync(string userId, List<string> messageIds);
    Task<List<Message>> GetMessagesByDateAsync(string userId, DateTime? date, DateTime? startDate, DateTime? endDate);
    Task<int> GetUnreadCountAsync(string userId);
    Task<Dictionary<string, int>> GetUnreadCountByTypeAsync(string userId);
}

public class MessageRepository : IMessageRepository
{
    private readonly IDbConnection _db;

    public MessageRepository(IDbConnection db)
    {
        _db = db;
    }

    // 實作方法...
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 訊息顯示頁面 (`MessageFrame.vue`)

```vue
<template>
  <div class="message-frame">
    <!-- 日曆區域 -->
    <div class="calendar-section">
      <el-calendar v-model="selectedDate" @pick="handleDatePick">
        <template #date-cell="{ data }">
          <div class="calendar-cell">
            <span :class="{ 'has-message': hasMessageOnDate(data.day) }">
              {{ data.day.split('-')[2] }}
            </span>
          </div>
        </template>
      </el-calendar>
    </div>

    <!-- 訊息列表區域 -->
    <div class="message-section">
      <div class="message-header">
        <h3>{{ $t('message.title') }}</h3>
        <el-input
          v-model="searchKeyword"
          :placeholder="$t('message.searchPlaceholder')"
          clearable
          @input="handleSearch"
          style="width: 200px; margin-left: 10px;"
        />
      </div>

      <el-tabs v-model="activeTab" @tab-change="handleTabChange">
        <el-tab-pane :label="$t('message.all')" name="all">
          <MessageList
            :messages="messages"
            :loading="loading"
            @read="handleMarkAsRead"
            @view="handleViewMessage"
          />
        </el-tab-pane>
        <el-tab-pane :label="$t('message.system')" name="SYSTEM">
          <MessageList
            :messages="systemMessages"
            :loading="loading"
            @read="handleMarkAsRead"
            @view="handleViewMessage"
          />
        </el-tab-pane>
        <el-tab-pane :label="$t('message.personal')" name="PERSONAL">
          <MessageList
            :messages="personalMessages"
            :loading="loading"
            @read="handleMarkAsRead"
            @view="handleViewMessage"
          />
        </el-tab-pane>
        <el-tab-pane :label="$t('message.announcement')" name="ANNOUNCEMENT">
          <MessageList
            :messages="announcementMessages"
            :loading="loading"
            @read="handleMarkAsRead"
            @view="handleViewMessage"
          />
        </el-tab-pane>
      </el-tabs>

      <!-- 分頁 -->
      <el-pagination
        v-model:current-page="pagination.page"
        v-model:page-size="pagination.pageSize"
        :total="pagination.total"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleSizeChange"
        @current-change="handlePageChange"
        style="margin-top: 20px;"
      />
    </div>

    <!-- 訊息詳情對話框 -->
    <MessageDetailDialog
      v-model="showDetailDialog"
      :message="selectedMessage"
      @read="handleMarkAsRead"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { ElMessage } from 'element-plus';
import MessageList from './components/MessageList.vue';
import MessageDetailDialog from './components/MessageDetailDialog.vue';
import { getMessages, getMessagesByDate, markAsRead, getUnreadCount } from '@/api/message.api';
import type { MessageDto, MessageQueryDto } from '@/types/message';

const { t } = useI18n();

const selectedDate = ref<Date>(new Date());
const searchKeyword = ref<string>('');
const activeTab = ref<string>('all');
const loading = ref<boolean>(false);
const messages = ref<MessageDto[]>([]);
const selectedMessage = ref<MessageDto | null>(null);
const showDetailDialog = ref<boolean>(false);

const pagination = ref({
  page: 1,
  pageSize: 20,
  total: 0
});

const systemMessages = computed(() => 
  messages.value.filter(m => m.messageType === 'SYSTEM')
);

const personalMessages = computed(() => 
  messages.value.filter(m => m.messageType === 'PERSONAL')
);

const announcementMessages = computed(() => 
  messages.value.filter(m => m.messageType === 'ANNOUNCEMENT')
);

const loadMessages = async () => {
  loading.value = true;
  try {
    const query: MessageQueryDto = {
      page: pagination.value.page,
      pageSize: pagination.value.pageSize,
      messageType: activeTab.value !== 'all' ? activeTab.value : undefined,
      keyword: searchKeyword.value || undefined,
      startDate: selectedDate.value ? selectedDate.value.toISOString() : undefined
    };

    const response = await getMessages(query);
    messages.value = response.data.items;
    pagination.value.total = response.data.total;
  } catch (error) {
    ElMessage.error(t('message.loadError'));
  } finally {
    loading.value = false;
  }
};

const handleDatePick = (date: Date) => {
  selectedDate.value = date;
  loadMessages();
};

const handleSearch = () => {
  pagination.value.page = 1;
  loadMessages();
};

const handleTabChange = () => {
  pagination.value.page = 1;
  loadMessages();
};

const handleMarkAsRead = async (messageId: string) => {
  try {
    await markAsRead(messageId);
    await loadMessages();
    ElMessage.success(t('message.markAsReadSuccess'));
  } catch (error) {
    ElMessage.error(t('message.markAsReadError'));
  }
};

const handleViewMessage = (message: MessageDto) => {
  selectedMessage.value = message;
  showDetailDialog.value = true;
  if (!message.isRead) {
    handleMarkAsRead(message.messageId);
  }
};

const handleSizeChange = (size: number) => {
  pagination.value.pageSize = size;
  pagination.value.page = 1;
  loadMessages();
};

const handlePageChange = (page: number) => {
  pagination.value.page = page;
  loadMessages();
};

const hasMessageOnDate = (date: string): boolean => {
  // 檢查該日期是否有訊息
  return messages.value.some(m => {
    const msgDate = new Date(m.startDate).toISOString().split('T')[0];
    return msgDate === date;
  });
};

onMounted(() => {
  loadMessages();
});
</script>

<style scoped lang="scss">
.message-frame {
  padding: 20px;
  background-color: #f5f5f5;

  .calendar-section {
    margin-bottom: 20px;
    background: white;
    padding: 20px;
    border-radius: 4px;

    .calendar-cell {
      text-align: center;

      .has-message {
        color: #409eff;
        font-weight: bold;
      }
    }
  }

  .message-section {
    background: white;
    padding: 20px;
    border-radius: 4px;

    .message-header {
      display: flex;
      align-items: center;
      margin-bottom: 20px;

      h3 {
        margin: 0;
      }
    }
  }
}
</style>
```

### 4.2 UI 元件設計

#### 4.2.1 訊息列表元件 (`MessageList.vue`)

```vue
<template>
  <div class="message-list">
    <el-empty v-if="!loading && messages.length === 0" :description="$t('message.noMessages')" />
    <div v-else>
      <div
        v-for="message in messages"
        :key="message.messageId"
        class="message-item"
        :class="{ 'unread': !message.isRead, 'read': message.isRead }"
        @click="handleClick(message)"
      >
        <div class="message-header">
          <span class="message-title">{{ message.title }}</span>
          <el-tag
            :type="getPriorityType(message.priority)"
            size="small"
            style="margin-left: 10px;"
          >
            {{ $t(`message.priority.${message.priority}`) }}
          </el-tag>
          <el-tag
            :type="getTypeType(message.messageType)"
            size="small"
            style="margin-left: 10px;"
          >
            {{ $t(`message.type.${message.messageType}`) }}
          </el-tag>
        </div>
        <div class="message-content">
          {{ message.content?.substring(0, 100) }}...
        </div>
        <div class="message-footer">
          <span class="message-date">{{ formatDate(message.createdAt) }}</span>
          <el-button
            v-if="!message.isRead"
            type="primary"
            size="small"
            @click.stop="handleMarkAsRead(message.messageId)"
          >
            {{ $t('message.markAsRead') }}
          </el-button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { defineProps, defineEmits } from 'vue';
import { useI18n } from 'vue-i18n';
import type { MessageDto } from '@/types/message';

const props = defineProps<{
  messages: MessageDto[];
  loading: boolean;
}>();

const emit = defineEmits<{
  read: [messageId: string];
  view: [message: MessageDto];
}>();

const { t } = useI18n();

const handleClick = (message: MessageDto) => {
  emit('view', message);
};

const handleMarkAsRead = (messageId: string) => {
  emit('read', messageId);
};

const getPriorityType = (priority: string) => {
  switch (priority) {
    case 'HIGH':
      return 'danger';
    case 'NORMAL':
      return 'info';
    case 'LOW':
      return '';
    default:
      return 'info';
  }
};

const getTypeType = (type: string) => {
  switch (type) {
    case 'SYSTEM':
      return 'warning';
    case 'PERSONAL':
      return 'success';
    case 'ANNOUNCEMENT':
      return 'info';
    default:
      return 'info';
  }
};

const formatDate = (date: string) => {
  return new Date(date).toLocaleString();
};
</script>

<style scoped lang="scss">
.message-list {
  .message-item {
    padding: 15px;
    margin-bottom: 10px;
    border: 1px solid #e4e7ed;
    border-radius: 4px;
    cursor: pointer;
    transition: all 0.3s;

    &:hover {
      background-color: #f5f7fa;
      border-color: #409eff;
    }

    &.unread {
      background-color: #ecf5ff;
      border-left: 4px solid #409eff;
    }

    &.read {
      opacity: 0.7;
    }

    .message-header {
      display: flex;
      align-items: center;
      margin-bottom: 10px;

      .message-title {
        font-weight: bold;
        font-size: 16px;
      }
    }

    .message-content {
      color: #606266;
      margin-bottom: 10px;
    }

    .message-footer {
      display: flex;
      justify-content: space-between;
      align-items: center;

      .message-date {
        color: #909399;
        font-size: 12px;
      }
    }
  }
}
</style>
```

### 4.3 API 呼叫 (`message.api.ts`)

```typescript
import request from '@/utils/request';
import type { ApiResponse, PagedResult } from '@/types/common';
import type { MessageDto, MessageQueryDto, UnreadCountDto } from '@/types/message';

export const getMessages = (query: MessageQueryDto) => {
  return request.get<ApiResponse<PagedResult<MessageDto>>>('/api/v1/messages', {
    params: query
  });
};

export const getMessage = (messageId: string) => {
  return request.get<ApiResponse<MessageDto>>(`/api/v1/messages/${messageId}`);
};

export const markAsRead = (messageId: string) => {
  return request.post<ApiResponse>(`/api/v1/messages/${messageId}/read`);
};

export const batchMarkAsRead = (messageIds: string[]) => {
  return request.post<ApiResponse>('/api/v1/messages/batch-read', {
    messageIds
  });
};

export const getMessagesByDate = (date?: Date, startDate?: Date, endDate?: Date) => {
  return request.get<ApiResponse<MessageDto[]>>('/api/v1/messages/by-date', {
    params: {
      date: date?.toISOString(),
      startDate: startDate?.toISOString(),
      endDate: endDate?.toISOString()
    }
  });
};

export const getUnreadCount = () => {
  return request.get<ApiResponse<UnreadCountDto>>('/api/v1/messages/unread-count');
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立 SystemMessages 資料表
- [ ] 建立 UserMessageReads 資料表
- [ ] 建立 UserMessages 資料表
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (3天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 訊息顯示頁面開發
- [ ] 訊息列表元件開發
- [ ] 訊息詳情對話框開發
- [ ] 日曆元件整合
- [ ] 搜尋功能開發
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試
- [ ] 安全性測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 9天

---

## 六、注意事項

### 6.1 業務邏輯
- 訊息顯示需根據使用者權限過濾
- 系統訊息需在有效日期範圍內才顯示
- 個人訊息僅顯示給指定使用者
- 已讀狀態需即時更新
- 日曆需標記有訊息的日期

### 6.2 資料驗證
- 訊息標題不可為空
- 開始日期不可晚於結束日期
- 訊息類型必須為有效值

### 6.3 效能
- 大量訊息查詢必須使用分頁
- 日曆標記需使用快取機制
- 未讀數量需使用快取

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢訊息列表成功
- [ ] 查詢單筆訊息成功
- [ ] 標記訊息為已讀成功
- [ ] 批次標記訊息為已讀成功
- [ ] 查詢日期範圍內的訊息成功
- [ ] 查詢未讀訊息數量成功

### 7.2 整合測試
- [ ] 完整訊息查詢流程測試
- [ ] 訊息已讀狀態更新測試
- [ ] 日曆選擇日期查詢訊息測試
- [ ] 訊息搜尋功能測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試

---

## 八、參考資料

### 8.1 舊程式碼
- `IMS3/HANSHIN/IMS3/KERNEL/FrameMessage.aspx`
- `IMS3/HANSHIN/IMS3/KERNEL/FrameMessage.aspx.cs`

### 8.2 相關功能
- `FrameMenu-框架選單功能.md` - 框架選單功能
- `FrameMainMenu-框架主選單功能.md` - 框架主選單功能

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

