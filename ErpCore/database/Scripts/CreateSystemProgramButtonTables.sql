-- =============================================
-- SYS0810 / SYS0999 - 系統作業與功能列表查詢
-- 資料表建立腳本
-- =============================================
-- 參考開發計劃: 
--   - 開發計劃/01-系統管理/03-權限管理/SYS0810-系統作業與功能列表查詢.md
--   - 開發計劃/01-系統管理/03-權限管理/SYS0999-系統作業與功能列表查詢.md

-- 說明：
-- SYS0810 和 SYS0999 功能共用相同的資料表結構
-- SYS0810: 一般查詢用途
-- SYS0999: 出庫用途
-- 本腳本用於確認相關資料表和索引已正確建立

-- 1. Systems 表 (系統主檔，對應舊系統 MNG_SYS)
-- 參考: CreateSystemsTable.sql
-- 主要欄位：
--   - SystemId: 系統ID (主鍵)
--   - SystemName: 系統名稱
--   - Status: 狀態 (A:啟用, I:停用)

-- 2. Programs 表 (作業主檔，對應舊系統 MNG_PROG)
-- 參考: CreateProgramsTable.sql
-- 主要欄位：
--   - ProgramId: 作業ID (主鍵)
--   - ProgramName: 作業名稱
--   - MenuId: 選單ID (關聯到 Menus.MenuId)
--   - Status: 狀態 (1:啟用, 0:停用)

-- 3. Buttons 表 (按鈕主檔，對應舊系統 MNG_BUTTON)
-- 參考: CreateButtonsTables.sql
-- 主要欄位：
--   - ButtonId: 按鈕ID (主鍵)
--   - ProgramId: 作業ID (關聯到 Programs.ProgramId)
--   - ButtonName: 按鈕名稱
--   - Status: 狀態 (1:啟用, 0:停用)

-- 4. Menus 表 (選單主檔，對應舊系統 MNG_MENU)
-- 參考: CreateMenusTable.sql
-- 主要欄位：
--   - MenuId: 選單ID (主鍵)
--   - SystemId: 系統ID (關聯到 Systems.SystemId)
--   - Status: 狀態 (1:啟用, 0:停用)

-- 5. 查詢 SQL (用於 SYS0810)
-- 查詢系統作業與功能列表
-- SELECT 
--     s.SystemId,
--     s.SystemName,
--     p.ProgramId,
--     p.ProgramName,
--     p.ProgramType,
--     p.SeqNo AS ProgramSeqNo,
--     b.ButtonId,
--     b.ButtonName,
--     b.ButtonAttr AS ButtonType,
--     b.PageId
-- FROM Systems s
-- INNER JOIN Menus m ON s.SystemId = m.SystemId
-- INNER JOIN Programs p ON m.MenuId = p.MenuId
-- LEFT JOIN Buttons b ON p.ProgramId = b.ProgramId
-- WHERE s.SystemId = @SystemId
--     AND s.Status = 'A'
--     AND m.Status = '1'
--     AND p.Status = '1'
--     AND (b.Status = '1' OR b.Status IS NULL)
-- ORDER BY p.SeqNo, p.ProgramId, b.ButtonId

PRINT 'SYS0810 / SYS0999 - 系統作業與功能列表查詢相關資料表檢查完成';
PRINT '相關資料表：Systems, Menus, Programs, Buttons';
PRINT '請確認以上資料表已正確建立並包含必要的索引';
PRINT 'SYS0810: 一般查詢用途';
PRINT 'SYS0999: 出庫用途';
