-- =============================================
-- SYSID_LIST - 系統ID列表
-- 資料表建立腳本
-- =============================================
-- 參考開發計劃: 開發計劃/15-下拉列表/SYSID_LIST-系統ID列表.md

-- 說明：
-- SYSID_LIST 功能使用現有的 Systems、Menus、Programs、Buttons 資料表
-- 本腳本用於確認相關資料表和索引已正確建立

-- 1. Systems 表 (系統主檔，對應舊系統 MNG_SYS)
-- 參考: CreateSystemsTable.sql
-- 主要欄位：
--   - SystemId: 系統ID (主鍵)
--   - SystemName: 系統名稱
--   - Status: 狀態 (1:啟用, 0:停用, NULL:啟用, A:啟用, I:停用)

-- 2. Menus 表 (選單主檔，對應舊系統 MNG_MENU)
-- 參考: CreateMenusTable.sql
-- 主要欄位：
--   - MenuId: 選單ID (主鍵)
--   - SystemId: 系統ID (關聯到 Systems.SystemId)
--   - Status: 狀態 (1:啟用, 0:停用, NULL:啟用)

-- 3. Programs 表 (作業主檔，對應舊系統 MNG_PROG)
-- 參考: CreateProgramsTable.sql
-- 主要欄位：
--   - ProgramId: 作業ID (主鍵)
--   - MenuId: 選單ID (關聯到 Menus.MenuId)
--   - Status: 狀態 (1:啟用, 0:停用)

-- 4. Buttons 表 (按鈕主檔，對應舊系統 MNG_BUTTON)
-- 參考: CreateButtonsTables.sql
-- 主要欄位：
--   - ButtonId: 按鈕ID (主鍵)
--   - ProgramId: 作業ID (關聯到 Programs.ProgramId)
--   - Status: 狀態 (1:啟用, 0:停用)

-- 5. 查詢 SQL (用於 SYSID_LIST)
-- 只顯示有選單、作業、按鈕的系統
-- SELECT DISTINCT s.SystemId, s.SystemName, s.Status
-- FROM Systems s
-- WHERE EXISTS (
--     SELECT 1 
--     FROM Menus M
--     INNER JOIN Programs P ON M.MenuId = P.MenuId
--     INNER JOIN Buttons B ON P.ProgramId = B.ProgramId
--     WHERE M.SystemId = s.SystemId
--       AND (M.Status = '1' OR M.Status IS NULL)
--       AND P.Status = '1'
--       AND B.Status = '1'
-- )
--   AND (s.SystemId LIKE @SystemId OR @SystemId IS NULL)
--   AND (s.SystemName LIKE @SystemName OR @SystemName IS NULL)
--   AND (s.Status = @Status OR s.Status IS NULL OR (@Status IS NULL AND (s.Status = '1' OR s.Status IS NULL)))
--   AND s.SystemId <> 'EIP0000'
--   AND (CASE WHEN @CurrentUser LIKE '%xcom%' THEN 1 ELSE 0 END = 1 
--        OR s.SystemId NOT IN ('CFG0000', 'XCOM000'))
-- ORDER BY s.SystemId

-- 6. 索引確認
-- Systems 表應有以下索引：
--   - PK_Systems (主鍵索引)
--   - IX_Systems_SystemName (系統名稱索引)
--   - IX_Systems_Status (狀態索引)

-- Menus 表應有以下索引：
--   - PK_Menus (主鍵索引)
--   - IX_Menus_SystemId (系統ID索引)
--   - IX_Menus_Status (狀態索引)

-- Programs 表應有以下索引：
--   - PK_Programs (主鍵索引)
--   - IX_Programs_MenuId (選單ID索引)
--   - IX_Programs_Status (狀態索引)

-- Buttons 表應有以下索引：
--   - PK_Buttons (主鍵索引)
--   - IX_Buttons_ProgramId (作業ID索引)
--   - IX_Buttons_Status (狀態索引)

-- 7. 系統排除邏輯
-- 預設排除以下系統：
--   - EIP0000 (所有使用者都排除)
--   - CFG0000 (除非是 xcom 使用者)
--   - XCOM000 (除非是 xcom 使用者)

PRINT 'SYSID_LIST 功能使用的資料表：';
PRINT '  - Systems (系統主檔)';
PRINT '  - Menus (選單主檔)';
PRINT '  - Programs (作業主檔)';
PRINT '  - Buttons (按鈕主檔)';
PRINT '';
PRINT '請確認以下 SQL 腳本已執行：';
PRINT '  - CreateSystemsTable.sql';
PRINT '  - CreateMenusTable.sql';
PRINT '  - CreateProgramsTable.sql';
PRINT '  - CreateButtonsTables.sql';
PRINT '';
PRINT 'SYSID_LIST 功能已準備就緒。';
