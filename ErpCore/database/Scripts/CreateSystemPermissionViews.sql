-- SYS0710 - 系統權限列表
-- 建立系統權限查詢視圖

-- 如果視圖已存在，先刪除
IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[V_UserButtonRoleButton]'))
BEGIN
    DROP VIEW [dbo].[V_UserButtonRoleButton]
    PRINT '已刪除現有的 V_UserButtonRoleButton 視圖'
END
GO

-- 建立使用者按鈕權限視圖
-- 此視圖整合使用者直接權限和角色權限
CREATE VIEW [dbo].[V_UserButtonRoleButton] AS
SELECT DISTINCT
    U.UserId AS USER_ID,
    U.UserName AS USER_NAME,
    R.RoleId AS ROLE_ID,
    R.RoleName AS ROLE_NAME,
    CS.SystemId AS SYS_ID,
    CS.SystemName AS SYS_NAME,
    CSS.SubSystemId AS MENU_ID,
    CSS.SubSystemName AS MENU_NAME,
    CP.ProgramId AS PG_ID,
    CP.ProgramName AS PG_ID_NAME,
    CB.ButtonId AS BUTTON_ID,
    CB.ButtonName AS BTN_NAME,
    CB.PageId AS PAGE_ID
FROM [dbo].[ConfigButtons] CB
INNER JOIN [dbo].[ConfigPrograms] CP ON CB.ProgramId = CP.ProgramId
INNER JOIN [dbo].[ConfigSystems] CS ON CP.SystemId = CS.SystemId
LEFT JOIN [dbo].[ConfigSubSystems] CSS ON CP.SubSystemId = CSS.SubSystemId
LEFT JOIN [dbo].[RoleButtons] RB ON CB.ButtonId = RB.ButtonId
LEFT JOIN [dbo].[UserButtons] UB ON CB.ButtonId = UB.ButtonId
LEFT JOIN [dbo].[Users] U ON (UB.UserId = U.UserId OR EXISTS (
    SELECT 1 FROM [dbo].[UserRoles] UR 
    INNER JOIN [dbo].[Roles] R2 ON UR.RoleId = R2.RoleId
    WHERE UR.UserId = U.UserId AND RB.RoleId = R2.RoleId
))
LEFT JOIN [dbo].[Roles] R ON RB.RoleId = R.RoleId
WHERE CB.Status = '1' 
  AND CP.Status = 'A'
  AND CS.Status = 'A'
  AND (UB.UserId IS NOT NULL OR RB.RoleId IS NOT NULL)
GO

PRINT 'V_UserButtonRoleButton 視圖建立成功'
GO
