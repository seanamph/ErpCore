-- 人力資源管理類資料表 (SYSH000)
-- 包含：人事管理、薪資管理、考勤管理

-- ============================================
-- 1. 人事管理 - 員工基本資料表 (Employees)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Employees]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Employees] (
        [EmployeeId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 員工編號
        [EmployeeName] NVARCHAR(100) NOT NULL, -- 員工姓名
        [IdNumber] NVARCHAR(20) NULL, -- 身分證字號
        [DepartmentId] NVARCHAR(50) NULL, -- 部門代碼
        [PositionId] NVARCHAR(50) NULL, -- 職位代碼
        [HireDate] DATETIME2 NULL, -- 到職日期
        [ResignDate] DATETIME2 NULL, -- 離職日期
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 員工狀態 A:在職, I:離職, L:留停
        [Email] NVARCHAR(100) NULL, -- 電子郵件
        [Phone] NVARCHAR(20) NULL, -- 電話
        [Address] NVARCHAR(500) NULL, -- 地址
        [BirthDate] DATETIME2 NULL, -- 出生日期
        [Gender] NVARCHAR(10) NULL, -- 性別 M:男, F:女
        [Notes] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
        CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED ([EmployeeId] ASC)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Employees_DepartmentId] ON [dbo].[Employees] ([DepartmentId]);
    CREATE NONCLUSTERED INDEX [IX_Employees_Status] ON [dbo].[Employees] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_Employees_PositionId] ON [dbo].[Employees] ([PositionId]);
    CREATE NONCLUSTERED INDEX [IX_Employees_EmployeeName] ON [dbo].[Employees] ([EmployeeName]);
    
    PRINT 'Employees 表建立成功';
END
ELSE
BEGIN
    PRINT 'Employees 表已存在';
END
GO

-- ============================================
-- 2. 薪資管理 - 薪資資料表 (Payrolls)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payrolls]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Payrolls] (
        [PayrollId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 薪資編號
        [EmployeeId] NVARCHAR(50) NOT NULL, -- 員工編號
        [PayrollYear] INT NOT NULL, -- 薪資年度
        [PayrollMonth] INT NOT NULL, -- 薪資月份
        [BaseSalary] DECIMAL(18, 2) NULL DEFAULT 0, -- 基本薪資
        [Allowance] DECIMAL(18, 2) NULL DEFAULT 0, -- 津貼
        [Bonus] DECIMAL(18, 2) NULL DEFAULT 0, -- 獎金
        [Deduction] DECIMAL(18, 2) NULL DEFAULT 0, -- 扣款
        [TotalSalary] DECIMAL(18, 2) NULL DEFAULT 0, -- 總薪資
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'D', -- 狀態 D:草稿, C:已確認, P:已發放
        [PayDate] DATETIME2 NULL, -- 發放日期
        [Notes] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
        CONSTRAINT [PK_Payrolls] PRIMARY KEY CLUSTERED ([PayrollId] ASC)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Payrolls_EmployeeId] ON [dbo].[Payrolls] ([EmployeeId]);
    CREATE NONCLUSTERED INDEX [IX_Payrolls_YearMonth] ON [dbo].[Payrolls] ([PayrollYear], [PayrollMonth]);
    CREATE NONCLUSTERED INDEX [IX_Payrolls_Status] ON [dbo].[Payrolls] ([Status]);
    
    -- 外鍵
    ALTER TABLE [dbo].[Payrolls]
    ADD CONSTRAINT [FK_Payrolls_Employees] FOREIGN KEY ([EmployeeId])
    REFERENCES [dbo].[Employees] ([EmployeeId]);
    
    PRINT 'Payrolls 表建立成功';
END
ELSE
BEGIN
    PRINT 'Payrolls 表已存在';
END
GO

-- ============================================
-- 3. 考勤管理 - 考勤資料表 (Attendances)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Attendances]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Attendances] (
        [AttendanceId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 考勤編號
        [EmployeeId] NVARCHAR(50) NOT NULL, -- 員工編號
        [AttendanceDate] DATETIME2 NOT NULL, -- 考勤日期
        [CheckInTime] DATETIME2 NULL, -- 上班時間
        [CheckOutTime] DATETIME2 NULL, -- 下班時間
        [WorkHours] DECIMAL(5, 2) NULL DEFAULT 0, -- 工作時數
        [OvertimeHours] DECIMAL(5, 2) NULL DEFAULT 0, -- 加班時數
        [LeaveType] NVARCHAR(20) NULL, -- 請假類型
        [LeaveHours] DECIMAL(5, 2) NULL DEFAULT 0, -- 請假時數
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'N', -- 狀態 N:正常, L:請假, A:曠職, O:加班
        [Notes] NVARCHAR(500) NULL, -- 備註
        [CreatedBy] NVARCHAR(50) NULL, -- 建立者
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
        [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
        CONSTRAINT [PK_Attendances] PRIMARY KEY CLUSTERED ([AttendanceId] ASC)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Attendances_EmployeeId] ON [dbo].[Attendances] ([EmployeeId]);
    CREATE NONCLUSTERED INDEX [IX_Attendances_AttendanceDate] ON [dbo].[Attendances] ([AttendanceDate]);
    CREATE NONCLUSTERED INDEX [IX_Attendances_Status] ON [dbo].[Attendances] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_Attendances_EmployeeDate] ON [dbo].[Attendances] ([EmployeeId], [AttendanceDate]);
    
    -- 外鍵
    ALTER TABLE [dbo].[Attendances]
    ADD CONSTRAINT [FK_Attendances_Employees] FOREIGN KEY ([EmployeeId])
    REFERENCES [dbo].[Employees] ([EmployeeId]);
    
    PRINT 'Attendances 表建立成功';
END
ELSE
BEGIN
    PRINT 'Attendances 表已存在';
END
GO

