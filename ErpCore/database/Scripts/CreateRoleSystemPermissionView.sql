-- SYS0731 - 角色系統權限列表
-- 建立角色系統權限查詢視圖

-- 如果視圖已存在，先刪除
IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[V_RoleSystemPermission]'))
BEGIN
    DROP VIEW [dbo].[V_RoleSystemPermission]
    PRINT '已刪除現有的 V_RoleSystemPermission 視圖'
END
GO

-- 建立角色系統權限視圖
-- 此視圖查詢指定角色的系統權限列表，包含系統、子系統（選單）、作業及按鈕權限的完整結構
-- 只查詢角色直接權限（不包含使用者權限）
CREATE VIEW [dbo].[V_RoleSystemPermission] AS
SELECT DISTINCT
    R.RoleId AS ROLE_ID,
    R.RoleName AS ROLE_NAME,
    CS.SystemId AS SYS_ID,
    CS.SystemName AS SYS_NAME,
    CSS.SubSystemId AS MENU_ID,
    CSS.SubSystemName AS MENU_NAME,
    CP.ProgramId AS PROG_ID,
    CP.ProgramName AS PROG_NAME,
    CB.ButtonId AS BUTTON_ID,
    CB.ButtonName AS BUTTON_NAME,
    CB.PageId AS PAGE_ID
FROM [dbo].[RoleButtons] RB
INNER JOIN [dbo].[Roles] R ON RB.RoleId = R.RoleId
INNER JOIN [dbo].[ConfigButtons] CB ON RB.ButtonId = CB.ButtonId
INNER JOIN [dbo].[ConfigPrograms] CP ON CB.ProgramId = CP.ProgramId
LEFT JOIN [dbo].[ConfigSubSystems] CSS ON CP.SubSystemId = CSS.SubSystemId
INNER JOIN [dbo].[ConfigSystems] CS ON CP.SystemId = CS.SystemId
WHERE RB.RoleId IS NOT NULL
  AND RB.UserId IS NULL
  AND CB.Status = '1' 
  AND CP.Status = 'A'
  AND CS.Status = 'A'
  AND (CSS.SubSystemId IS NULL OR CSS.Status = 'A')
GO

-- 建立索引以提升查詢效能
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RoleButtons_RoleId_ButtonId' AND object_id = OBJECT_ID('RoleButtons'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_RoleButtons_RoleId_ButtonId] 
    ON [dbo].[RoleButtons] ([RoleId], [ButtonId])
    WHERE [UserId] IS NULL
    PRINT '已建立索引 IX_RoleButtons_RoleId_ButtonId'
END
GO

PRINT 'V_RoleSystemPermission 視圖建立成功'
GO
