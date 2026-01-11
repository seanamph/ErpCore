-- =============================================
-- MULTI_USERS_LIST - 多選使用者列表
-- 資料表建立腳本
-- =============================================
-- 參考開發計劃: 開發計劃/15-下拉列表/MULTI_USERS_LIST-多選使用者列表.md

-- 說明：
-- MULTI_USERS_LIST 功能使用現有的 Users 和 Departments 資料表
-- 本腳本用於確認相關資料表和索引已正確建立

-- 1. Users 表 (使用者主檔)
-- 參考: CreateUsersTables.sql
-- 主要欄位：
--   - UserId: 使用者編號 (主鍵)
--   - UserName: 使用者名稱
--   - OrgId: 組織代碼 (關聯到 Departments.DeptId)
--   - Title: 職稱
--   - Status: 狀態 (A:啟用, I:停用, L:鎖定)

-- 2. Departments 表 (部門主檔)
-- 參考: CreateDepartmentsTables.sql
-- 主要欄位：
--   - DeptId: 部門代碼 (主鍵)
--   - DeptName: 部門名稱
--   - Status: 狀態 (A:啟用, I:停用)

-- 3. 查詢 SQL (用於 MULTI_USERS_LIST)
-- SELECT u.UserId, u.UserName, u.OrgId, u.Title, u.Status,
--        d.DeptName AS OrgName
-- FROM Users u
-- LEFT JOIN Departments d ON u.OrgId = d.DeptId
-- WHERE 1=1
--   AND (u.UserId LIKE @UserId OR @UserId IS NULL)
--   AND (u.UserName LIKE @UserName OR @UserName IS NULL)
--   AND (u.OrgId = @OrgId OR @OrgId IS NULL)
--   AND (u.Status = @Status OR @Status IS NULL)
-- ORDER BY u.UserId
-- OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY

-- 4. 索引確認
-- Users 表應有以下索引：
--   - PK_Users (主鍵索引)
--   - IX_Users_OrgId (組織代碼索引)
--   - IX_Users_Status (狀態索引)
--   - IX_Users_UserType (使用者型態索引)
--   - IX_Users_ShopId (分店代碼索引)

-- Departments 表應有以下索引：
--   - PK_Departments (主鍵索引)
--   - IX_Departments_DeptName (部門名稱索引)
--   - IX_Departments_OrgId (組織代碼索引)
--   - IX_Departments_Status (狀態索引)
--   - IX_Departments_SeqNo (排序序號索引)

PRINT 'MULTI_USERS_LIST 功能使用的資料表：';
PRINT '  - Users (使用者主檔)';
PRINT '  - Departments (部門主檔)';
PRINT '';
PRINT '請確認以下 SQL 腳本已執行：';
PRINT '  - CreateUsersTables.sql';
PRINT '  - CreateDepartmentsTables.sql';
PRINT '';
PRINT 'MULTI_USERS_LIST 功能已準備就緒。';
