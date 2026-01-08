-- SYSL510 - 加班發放管理：加班發放資料表
-- 建立日期：2025-01-09

-- 主要資料表: OvertimePayments (加班發放主檔)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OvertimePayments]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[OvertimePayments] (
        [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [PaymentNo] NVARCHAR(50) NOT NULL, -- 發放單號
        [PaymentDate] DATETIME2 NOT NULL, -- 發放日期
        [EmployeeId] NVARCHAR(50) NOT NULL, -- 員工編號
        [EmployeeName] NVARCHAR(100) NULL, -- 員工姓名
        [DepartmentId] NVARCHAR(50) NULL, -- 部門編號
        [OvertimeHours] DECIMAL(18,2) NOT NULL DEFAULT 0, -- 加班時數
        [OvertimeAmount] DECIMAL(18,2) NOT NULL DEFAULT 0, -- 加班金額
        [StartDate] DATETIME2 NOT NULL, -- 開始日期
        [EndDate] DATETIME2 NOT NULL, -- 結束日期
        [Status] NVARCHAR(20) NOT NULL DEFAULT 'Draft', -- 狀態 (Draft/Submitted/Approved/Rejected)
        [ApprovedBy] NVARCHAR(50) NULL, -- 審核者
        [ApprovedAt] DATETIME2 NULL, -- 審核時間
        [Notes] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
        CONSTRAINT [UQ_OvertimePayments_PaymentNo] UNIQUE ([PaymentNo])
    );
END

-- 索引
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_OvertimePayments_PaymentNo' AND object_id = OBJECT_ID('OvertimePayments'))
    CREATE NONCLUSTERED INDEX [IX_OvertimePayments_PaymentNo] ON [dbo].[OvertimePayments] ([PaymentNo]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_OvertimePayments_EmployeeId' AND object_id = OBJECT_ID('OvertimePayments'))
    CREATE NONCLUSTERED INDEX [IX_OvertimePayments_EmployeeId] ON [dbo].[OvertimePayments] ([EmployeeId]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_OvertimePayments_PaymentDate' AND object_id = OBJECT_ID('OvertimePayments'))
    CREATE NONCLUSTERED INDEX [IX_OvertimePayments_PaymentDate] ON [dbo].[OvertimePayments] ([PaymentDate]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_OvertimePayments_Status' AND object_id = OBJECT_ID('OvertimePayments'))
    CREATE NONCLUSTERED INDEX [IX_OvertimePayments_Status] ON [dbo].[OvertimePayments] ([Status]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_OvertimePayments_DepartmentId' AND object_id = OBJECT_ID('OvertimePayments'))
    CREATE NONCLUSTERED INDEX [IX_OvertimePayments_DepartmentId] ON [dbo].[OvertimePayments] ([DepartmentId]);

