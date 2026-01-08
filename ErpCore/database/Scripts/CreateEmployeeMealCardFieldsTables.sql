-- SYSL206/SYSL207 - 員餐卡管理：員餐卡欄位資料表
-- 建立日期：2025-01-08

-- 主要資料表: EmployeeMealCardFields (對應舊系統相關表)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EmployeeMealCardFields]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[EmployeeMealCardFields] (
        [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵
        [FieldId] NVARCHAR(50) NOT NULL, -- 欄位ID (FIELD_ID)
        [FieldName] NVARCHAR(200) NULL, -- 欄位名稱 (FIELD_NAME)
        [CardType] NVARCHAR(20) NULL, -- 卡片類型 (CARD_TYPE)
        [ActionType] NVARCHAR(20) NULL, -- 動作類型 (ACTION_TYPE)
        [OtherType] NVARCHAR(20) NULL, -- 其他類型 (OTHER_TYPE)
        [MustKeyinYn] NVARCHAR(1) NULL DEFAULT 'N', -- 必填標誌 (MUST_KEYIN_YN) Y:必填, N:非必填
        [ReadonlyYn] NVARCHAR(1) NULL DEFAULT 'N', -- 唯讀標誌 (READONLY_YN) Y:唯讀, N:可編輯
        [BtnEtekYn] NVARCHAR(1) NULL DEFAULT 'N', -- 按鈕標誌 (BTN_ETEK_YN) Y:顯示按鈕, N:不顯示
        [SeqNo] INT NULL DEFAULT 0, -- 排序序號
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 1:啟用, 0:停用
        [Notes] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
        CONSTRAINT [PK_EmployeeMealCardFields] PRIMARY KEY CLUSTERED ([TKey] ASC),
        CONSTRAINT [UQ_EmployeeMealCardFields_FieldId] UNIQUE ([FieldId])
    );
END

-- 索引
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EmployeeMealCardFields_CardType' AND object_id = OBJECT_ID('EmployeeMealCardFields'))
    CREATE NONCLUSTERED INDEX [IX_EmployeeMealCardFields_CardType] ON [dbo].[EmployeeMealCardFields] ([CardType]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EmployeeMealCardFields_ActionType' AND object_id = OBJECT_ID('EmployeeMealCardFields'))
    CREATE NONCLUSTERED INDEX [IX_EmployeeMealCardFields_ActionType] ON [dbo].[EmployeeMealCardFields] ([ActionType]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EmployeeMealCardFields_OtherType' AND object_id = OBJECT_ID('EmployeeMealCardFields'))
    CREATE NONCLUSTERED INDEX [IX_EmployeeMealCardFields_OtherType] ON [dbo].[EmployeeMealCardFields] ([OtherType]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EmployeeMealCardFields_Status' AND object_id = OBJECT_ID('EmployeeMealCardFields'))
    CREATE NONCLUSTERED INDEX [IX_EmployeeMealCardFields_Status] ON [dbo].[EmployeeMealCardFields] ([Status]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EmployeeMealCardFields_SeqNo' AND object_id = OBJECT_ID('EmployeeMealCardFields'))
    CREATE NONCLUSTERED INDEX [IX_EmployeeMealCardFields_SeqNo] ON [dbo].[EmployeeMealCardFields] ([SeqNo]);

-- 相關資料表: OtherType - 其他類型主檔
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OtherType]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[OtherType] (
        [OtherId] NVARCHAR(20) NOT NULL, -- 其他類型代碼
        [ActionId] NVARCHAR(20) NOT NULL, -- 動作類型代碼
        [OtherName] NVARCHAR(100) NOT NULL, -- 其他類型名稱
        [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_OtherType] PRIMARY KEY CLUSTERED ([OtherId], [ActionId])
    );
END

