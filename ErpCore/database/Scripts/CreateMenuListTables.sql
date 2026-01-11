-- =============================================
-- MENU_LIST - 選單列表功能 SQL 腳本說明
-- 開發計劃文件：開發計劃/15-下拉列表/MENU_LIST-選單列表.md
-- =============================================

-- 說明：
-- MENU_LIST 功能需要 Menus 資料表來儲存選單資料
-- 此腳本說明 Menus 資料表的建立方式和相關設定

-- =============================================
-- Menus 資料表建立
-- =============================================
-- Menus 資料表用於儲存系統選單資料
-- 如果資料表已存在，請跳過建立步驟
-- 詳細的建立腳本請參考：CreateMenusTable.sql

-- 主要欄位說明：
--   MenuId          - 選單ID（主鍵）
--   MenuName        - 選單名稱
--   SystemId        - 系統ID（外鍵至 Systems 表）
--   ParentMenuId    - 父選單ID（外鍵至 Menus 表，支援階層結構）
--   SeqNo           - 排序序號
--   Icon            - 圖示
--   Url             - 連結網址
--   Status          - 狀態（1:啟用, 0:停用）
--   CreatedBy       - 建立者
--   CreatedAt       - 建立時間
--   UpdatedBy       - 更新者
--   UpdatedAt       - 更新時間

-- =============================================
-- 資料表關聯
-- =============================================
-- 1. Menus 與 Systems 的關聯
--    Menus.SystemId -> Systems.SystemId
--    一個系統可以有多個選單
--
-- 2. Menus 與 Menus 的關聯（父子選單）
--    Menus.ParentMenuId -> Menus.MenuId
--    支援選單階層結構，一個選單可以有多個子選單

-- =============================================
-- 索引說明
-- =============================================
-- 系統已建立以下索引以提升查詢效能：
--   1. IX_Menus_MenuId        - 選單ID索引（唯一）
--   2. IX_Menus_SystemId      - 系統ID索引
--   3. IX_Menus_ParentMenuId  - 父選單ID索引
--   4. IX_Menus_SeqNo         - 排序序號索引
--   5. IX_Menus_Status        - 狀態索引

-- =============================================
-- 範例資料
-- =============================================
-- 以下為範例資料，可根據實際需求調整：

-- INSERT INTO [dbo].[Menus] 
--     ([MenuId], [MenuName], [SystemId], [ParentMenuId], [SeqNo], [Icon], [Url], [Status], [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt])
-- VALUES
--     ('MENU001', '系統管理', 'SYS0000', NULL, 1, 'system', '/system', '1', 'SYSTEM', GETDATE(), 'SYSTEM', GETDATE()),
--     ('MENU002', '基本資料管理', 'SYS0000', NULL, 2, 'database', '/basic', '1', 'SYSTEM', GETDATE(), 'SYSTEM', GETDATE()),
--     ('MENU003', '使用者管理', 'SYS0000', 'MENU001', 1, 'user', '/system/user', '1', 'SYSTEM', GETDATE(), 'SYSTEM', GETDATE()),
--     ('MENU004', '角色管理', 'SYS0000', 'MENU001', 2, 'role', '/system/role', '1', 'SYSTEM', GETDATE(), 'SYSTEM', GETDATE());

-- =============================================
-- 查詢範例
-- =============================================
-- 1. 查詢所有啟用的選單
--    SELECT * FROM Menus WHERE Status = '1' ORDER BY SeqNo, MenuName;

-- 2. 查詢特定系統的選單
--    SELECT * FROM Menus WHERE SystemId = 'SYS0000' AND Status = '1' ORDER BY SeqNo, MenuName;

-- 3. 查詢選單及其子選單（階層查詢）
--    WITH MenuHierarchy AS (
--        SELECT MenuId, MenuName, ParentMenuId, SeqNo, 0 AS Level
--        FROM Menus
--        WHERE ParentMenuId IS NULL AND Status = '1'
--        UNION ALL
--        SELECT m.MenuId, m.MenuName, m.ParentMenuId, m.SeqNo, mh.Level + 1
--        FROM Menus m
--        INNER JOIN MenuHierarchy mh ON m.ParentMenuId = mh.MenuId
--        WHERE m.Status = '1'
--    )
--    SELECT * FROM MenuHierarchy ORDER BY Level, SeqNo, MenuName;

-- 4. 查詢選單選項（用於下拉選單）
--    SELECT MenuId AS Value, MenuName AS Label 
--    FROM Menus 
--    WHERE Status = '1' 
--    ORDER BY SeqNo, MenuName;

-- =============================================
-- API 端點說明
-- =============================================
-- GET  /api/v1/lists/menus
--     查詢選單列表（支援分頁、篩選、排序）
--     請求參數：
--       PageIndex  - 頁碼（預設：1）
--       PageSize   - 每頁筆數（預設：50）
--       SortField  - 排序欄位（預設：MenuName）
--       SortOrder  - 排序方向（ASC/DESC，預設：ASC）
--       MenuName   - 選單名稱（模糊查詢）
--       SystemId   - 系統ID（精確查詢）
--       Status     - 狀態（預設：1）
--     回應：PagedResult<MenuDto>

-- GET  /api/v1/lists/menus/{menuId}
--     查詢單筆選單
--     路徑參數：menuId - 選單ID
--     回應：MenuDto

-- GET  /api/v1/lists/menus/options
--     查詢選單選項（用於下拉選單）
--     請求參數：
--       systemId   - 系統ID（可選）
--       status     - 狀態（預設：1）
--     回應：IEnumerable<MenuOptionDto>

-- =============================================
-- 前端組件使用說明
-- =============================================
-- 1. MenuListDialog.vue - 選單選擇對話框組件
--    用於彈窗選擇選單
--    支援單選和多選模式
--    支援選單名稱和系統ID篩選
--    支援 returnFields 屬性，可指定回傳欄位
--    支援 returnControl 屬性，可回傳值到父視窗控制項

-- 2. MenuSelector.vue - 選單選擇器組件（可選）
--    用於表單中的選單選擇
--    整合 MenuListDialog 組件

-- 3. MenuList.vue - 選單列表頁面
--    獨立的選單列表查詢頁面
--    支援選單名稱、選單代碼、狀態篩選
--    顯示選單階層結構

-- =============================================
-- 注意事項
-- =============================================
-- 1. Menus 資料表必須先建立，請執行 CreateMenusTable.sql
-- 2. Systems 資料表必須先建立，因為 Menus 表有外鍵關聯
-- 3. MenuId 必須唯一，系統已建立唯一索引
-- 4. ParentMenuId 可以為 NULL（表示頂層選單）
-- 5. 刪除選單時需注意是否有子選單，建議使用軟刪除（將 Status 設為 '0'）
-- 6. 選單階層結構不建議超過 5 層，以免影響效能
-- 7. SeqNo 用於排序，建議使用整數，數字越小越靠前
-- 8. Status 欄位用於控制選單的啟用/停用，停用的選單不會在選單列表中顯示

-- =============================================
-- 測試建議
-- =============================================
-- 1. 測試選單列表查詢（分頁、篩選、排序）
-- 2. 測試選單階層結構查詢
-- 3. 測試選單選項查詢（用於下拉選單）
-- 4. 測試選單選擇對話框（單選、多選模式）
-- 5. 測試選單選擇器組件
-- 6. 測試選單列表頁面
-- 7. 測試選單資料的新增、修改、刪除（如果實作）
-- 8. 測試選單狀態變更（啟用/停用）
-- 9. 測試選單階層結構的建立和查詢
-- 10. 測試大量選單資料的查詢效能

-- =============================================
