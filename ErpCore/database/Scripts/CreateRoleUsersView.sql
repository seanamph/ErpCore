-- SYS0750 - 角色之使用者列表
-- 建立角色使用者查詢視圖

-- 如果視圖已存在，先刪除
IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[V_RoleUsers]'))
BEGIN
    DROP VIEW [dbo].[V_RoleUsers]
    PRINT '已刪除現有的 V_RoleUsers 視圖'
END
GO

-- 建立角色使用者視圖
-- 此視圖查詢指定角色下，哪些使用者擁有該角色
CREATE VIEW [dbo].[V_RoleUsers] AS
SELECT 
    R.RoleId AS ROLE_ID,
    R.RoleName AS ROLE_NAME,
    U.UserId AS USER_ID,
    U.UserName AS USER_NAME,
    U.UserType AS USER_TYPE,
    U.Status AS STATUS,
    U.Title AS TITLE,
    U.OrgId AS ORG_ID,
    O.OrgName AS ORG_NAME,
    UR.CreatedBy AS CREATED_BY,
    UR.CreatedAt AS CREATED_AT
FROM [dbo].[UserRoles] UR
INNER JOIN [dbo].[Roles] R ON UR.RoleId = R.RoleId
INNER JOIN [dbo].[Users] U ON UR.UserId = U.UserId
LEFT JOIN [dbo].[Organizations] O ON U.OrgId = O.OrgId
WHERE R.Status = '1'
GO

-- 建立索引以提升查詢效能（如果索引不存在）
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserRoles]') AND type in (N'U'))
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UserRoles_RoleId' AND object_id = OBJECT_ID('UserRoles'))
    BEGIN
        CREATE NONCLUSTERED INDEX [IX_UserRoles_RoleId] 
        ON [dbo].[UserRoles] ([RoleId])
        PRINT '已建立索引 IX_UserRoles_RoleId'
    END

    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UserRoles_UserId' AND object_id = OBJECT_ID('UserRoles'))
    BEGIN
        CREATE NONCLUSTERED INDEX [IX_UserRoles_UserId] 
        ON [dbo].[UserRoles] ([UserId])
        PRINT '已建立索引 IX_UserRoles_UserId'
    END
END
GO

PRINT 'V_RoleUsers 視圖建立成功'
GO
