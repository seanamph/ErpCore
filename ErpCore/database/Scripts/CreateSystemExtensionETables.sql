-- =============================================
-- 系統擴展E類 (SYSPE00) - 資料庫腳本
-- 功能：員工資料維護 (SYSPE10-SYSPE11)、人事資料維護 (SYSPED0)
-- 建立日期：2025-01-09
-- =============================================

-- 1. 員工主檔表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Employees]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Employees] (
        [EmployeeId] NVARCHAR(50) NOT NULL PRIMARY KEY,
        [EmployeeName] NVARCHAR(100) NOT NULL,
        [IdNumber] NVARCHAR(20) NULL,
        [DepartmentId] NVARCHAR(50) NULL,
        [PositionId] NVARCHAR(50) NULL,
        [HireDate] DATETIME2 NULL,
        [ResignDate] DATETIME2 NULL,
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- A:在職, I:離職, L:留停
        [Email] NVARCHAR(100) NULL,
        [Phone] NVARCHAR(20) NULL,
        [Address] NVARCHAR(500) NULL,
        [BirthDate] DATETIME2 NULL,
        [Gender] NVARCHAR(10) NULL, -- M:男, F:女
        [Notes] NVARCHAR(500) NULL,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED ([EmployeeId] ASC)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Employees_DepartmentId] ON [dbo].[Employees] ([DepartmentId]);
    CREATE NONCLUSTERED INDEX [IX_Employees_Status] ON [dbo].[Employees] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_Employees_PositionId] ON [dbo].[Employees] ([PositionId]);
    CREATE NONCLUSTERED INDEX [IX_Employees_EmployeeName] ON [dbo].[Employees] ([EmployeeName]);
END
GO

-- 2. 人事主檔表
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Personnel]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Personnel] (
        [PersonnelId] NVARCHAR(50) NOT NULL PRIMARY KEY,
        [PersonnelName] NVARCHAR(100) NOT NULL,
        [IdNumber] NVARCHAR(20) NULL,
        [DepartmentId] NVARCHAR(50) NULL,
        [PositionId] NVARCHAR(50) NULL,
        [HireDate] DATETIME2 NULL,
        [ResignDate] DATETIME2 NULL,
        [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- A:在職, I:離職, L:留停
        [Email] NVARCHAR(100) NULL,
        [Phone] NVARCHAR(20) NULL,
        [Address] NVARCHAR(500) NULL,
        [BirthDate] DATETIME2 NULL,
        [Gender] NVARCHAR(10) NULL, -- M:男, F:女
        [Notes] NVARCHAR(500) NULL,
        [CreatedBy] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] NVARCHAR(50) NULL,
        [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_Personnel] PRIMARY KEY CLUSTERED ([PersonnelId] ASC)
    );

    -- 索引
    CREATE NONCLUSTERED INDEX [IX_Personnel_DepartmentId] ON [dbo].[Personnel] ([DepartmentId]);
    CREATE NONCLUSTERED INDEX [IX_Personnel_Status] ON [dbo].[Personnel] ([Status]);
    CREATE NONCLUSTERED INDEX [IX_Personnel_PositionId] ON [dbo].[Personnel] ([PositionId]);
    CREATE NONCLUSTERED INDEX [IX_Personnel_PersonnelName] ON [dbo].[Personnel] ([PersonnelName]);
END
GO

