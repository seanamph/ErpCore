-- SYS0720 - 作業權限之使用者列表
-- 建立作業使用者權限查詢視圖

-- 如果視圖已存在，先刪除
IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[V_ProgramUserPermission]'))
BEGIN
    DROP VIEW [dbo].[V_ProgramUserPermission]
    PRINT '已刪除現有的 V_ProgramUserPermission 視圖'
END
GO

-- 建立作業使用者權限視圖
-- 此視圖查詢指定作業下，哪些使用者擁有該作業的權限，以及這些使用者擁有的按鈕權限列表
-- 只查詢使用者直接權限（不包含角色權限）
CREATE VIEW [dbo].[V_ProgramUserPermission] AS
SELECT DISTINCT
    CP.ProgramId AS PROG_ID,
    CP.ProgramName AS PROG_NAME,
    U.UserId AS USER_ID,
    U.UserName AS USER_NAME,
    CB.ButtonId AS BUTTON_ID,
    CB.ButtonName AS BUTTON_NAME,
    CB.PageId AS PAGE_ID
FROM [dbo].[ConfigPrograms] CP
INNER JOIN [dbo].[ConfigButtons] CB ON CP.ProgramId = CB.ProgramId
INNER JOIN [dbo].[UserButtons] UB ON CB.ButtonId = UB.ButtonId
INNER JOIN [dbo].[Users] U ON UB.UserId = U.UserId
WHERE CB.Status = '1' 
  AND CP.Status = 'A'
  AND U.Status = '1'
GO

-- 建立索引以提升查詢效能
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UserButtons_UserId_ButtonId' AND object_id = OBJECT_ID('UserButtons'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_UserButtons_UserId_ButtonId] 
    ON [dbo].[UserButtons] ([UserId], [ButtonId])
    PRINT '已建立索引 IX_UserButtons_UserId_ButtonId'
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ConfigButtons_ProgramId_Status' AND object_id = OBJECT_ID('ConfigButtons'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_ConfigButtons_ProgramId_Status] 
    ON [dbo].[ConfigButtons] ([ProgramId], [Status])
    PRINT '已建立索引 IX_ConfigButtons_ProgramId_Status'
END
GO

PRINT 'V_ProgramUserPermission 視圖建立成功'
GO
