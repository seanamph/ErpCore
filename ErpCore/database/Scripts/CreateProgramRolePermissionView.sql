-- SYS0740 - 作業權限之角色列表
-- 建立作業角色權限查詢視圖

-- 如果視圖已存在，先刪除
IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[V_ProgramRolePermission]'))
BEGIN
    DROP VIEW [dbo].[V_ProgramRolePermission]
    PRINT '已刪除現有的 V_ProgramRolePermission 視圖'
END
GO

-- 建立作業角色權限視圖
-- 此視圖查詢指定作業下，哪些角色擁有該作業的權限，以及這些角色擁有的按鈕權限列表
-- 只查詢角色直接權限（不包含使用者權限）
CREATE VIEW [dbo].[V_ProgramRolePermission] AS
SELECT DISTINCT
    CP.ProgramId AS PROG_ID,
    CP.ProgramName AS PROG_NAME,
    R.RoleId AS ROLE_ID,
    R.RoleName AS ROLE_NAME,
    CB.ButtonId AS BUTTON_ID,
    CB.ButtonName AS BUTTON_NAME,
    CB.PageId AS PAGE_ID
FROM [dbo].[ConfigPrograms] CP
INNER JOIN [dbo].[ConfigButtons] CB ON CP.ProgramId = CB.ProgramId
INNER JOIN [dbo].[RoleButtons] RB ON CB.ButtonId = RB.ButtonId
INNER JOIN [dbo].[Roles] R ON RB.RoleId = R.RoleId
WHERE RB.RoleId IS NOT NULL
  AND RB.UserId IS NULL
  AND CB.Status = '1' 
  AND CP.Status = 'A'
  AND R.Status = '1'
GO

-- 建立索引以提升查詢效能
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RoleButtons_RoleId_ButtonId_Program' AND object_id = OBJECT_ID('RoleButtons'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_RoleButtons_RoleId_ButtonId_Program] 
    ON [dbo].[RoleButtons] ([RoleId], [ButtonId])
    WHERE [UserId] IS NULL
    PRINT '已建立索引 IX_RoleButtons_RoleId_ButtonId_Program'
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ConfigButtons_ProgramId_Status_Role' AND object_id = OBJECT_ID('ConfigButtons'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_ConfigButtons_ProgramId_Status_Role] 
    ON [dbo].[ConfigButtons] ([ProgramId], [Status])
    PRINT '已建立索引 IX_ConfigButtons_ProgramId_Status_Role'
END
GO

PRINT 'V_ProgramRolePermission 視圖建立成功'
GO
