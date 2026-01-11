-- =============================================
-- DATE_LIST - 日期列表功能 SQL 腳本說明
-- 開發計劃文件：開發計劃/15-下拉列表/DATE_LIST-日期列表.md
-- =============================================

-- 說明：
-- DATE_LIST 功能為前端 UI 組件，不需要建立資料表
-- 但需要讀取系統參數設定（Parameters 資料表）中的日期格式設定
-- 此腳本說明相關參數的設定方式

-- =============================================
-- 系統參數設定（Parameters 資料表）
-- =============================================
-- DATE_LIST 功能需要從 Parameters 資料表讀取以下參數：
--   1. DATE_FORMAT - 日期格式（預設：yyyy/MM/dd）
--   2. TIME_FORMAT - 時間格式（預設：HH:mm:ss）

-- 如果 Parameters 資料表不存在，請先建立該資料表
-- 如果參數不存在，系統會使用預設值

-- 範例：設定日期格式參數
-- INSERT INTO [dbo].[Parameters] ([ParameterCode], [ParameterName], [ParameterValue], [ParameterType], [Description], [Status], [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt])
-- VALUES 
--     ('DATE_FORMAT', 'FORMAT', 'yyyy/MM/dd', 'STRING', '系統日期格式', '1', 'SYSTEM', GETDATE(), 'SYSTEM', GETDATE()),
--     ('TIME_FORMAT', 'FORMAT', 'HH:mm:ss', 'STRING', '系統時間格式', '1', 'SYSTEM', GETDATE(), 'SYSTEM', GETDATE());

-- 或者使用 UPDATE 更新現有參數：
-- UPDATE [dbo].[Parameters]
-- SET [ParameterValue] = 'yyyy/MM/dd',
--     [UpdatedBy] = 'SYSTEM',
--     [UpdatedAt] = GETDATE()
-- WHERE [ParameterCode] = 'DATE_FORMAT' AND [ParameterName] = 'FORMAT';

-- UPDATE [dbo].[Parameters]
-- SET [ParameterValue] = 'HH:mm:ss',
--     [UpdatedBy] = 'SYSTEM',
--     [UpdatedAt] = GETDATE()
-- WHERE [ParameterCode] = 'TIME_FORMAT' AND [ParameterName] = 'FORMAT';

-- =============================================
-- 支援的日期格式
-- =============================================
-- 系統支援以下日期格式（.NET DateTime 格式）：
--   yyyy/MM/dd      - 2024/01/01
--   yyyy-MM-dd      - 2024-01-01
--   yyyyMMdd        - 20240101
--   MM/dd/yyyy      - 01/01/2024
--   dd/MM/yyyy      - 01/01/2024
--   其他 .NET DateTime 支援的格式

-- =============================================
-- 日期驗證規則
-- =============================================
-- 1. 年份不能小於 1582 年（格里曆開始年份）
-- 2. 日期必須有效（如 2 月 30 日無效）
-- 3. 支援閏年判斷
-- 4. 日期字串必須符合指定的格式

-- =============================================
-- 前端組件使用說明
-- =============================================
-- 1. DatePicker.vue - 日期選擇器組件
--    用於表單中的日期輸入
--    支援 date, datetime, daterange 類型
--    自動從系統參數讀取日期格式

-- 2. DatePickerDialog.vue - 日期選擇對話框組件
--    用於彈窗選擇日期
--    支援 returnControl 屬性，可回傳值到父視窗控制項

-- 3. DateList.vue - 日期選擇頁面
--    獨立的日期選擇頁面
--    支援快速選擇（今天、昨天、明天、本月第一天、本月最後一天）
--    顯示日期資訊（星期、年份、月份）

-- =============================================
-- API 端點說明
-- =============================================
-- GET  /api/v1/system/date-format
--     取得系統日期格式設定
--     回應：{ dateFormat: "yyyy/MM/dd", timeFormat: "HH:mm:ss", dateTimeFormat: "yyyy/MM/dd HH:mm:ss" }

-- POST /api/v1/system/validate-date
--     驗證日期格式
--     請求：{ dateString: "2024/01/01", dateFormat: "yyyy/MM/dd" }
--     回應：{ isValid: true, parsedDate: "2024-01-01T00:00:00Z", errorMessage: null }

-- POST /api/v1/system/parse-date
--     解析日期字串
--     請求：{ dateString: "2024/01/01", dateFormat: "yyyy/MM/dd" }
--     回應：DateTime? (解析後的日期，如果解析失敗則為 null)

-- =============================================
-- 注意事項
-- =============================================
-- 1. 此功能不需要建立資料表，只需要確保 Parameters 資料表存在
-- 2. 如果 Parameters 資料表中沒有日期格式參數，系統會使用預設值
-- 3. 日期格式參數必須符合 .NET DateTime 格式規範
-- 4. 前端組件會自動從系統參數讀取日期格式設定
-- 5. 日期驗證會在後端和前端同時進行，確保資料正確性

-- =============================================
-- 測試建議
-- =============================================
-- 1. 測試不同日期格式的解析和驗證
-- 2. 測試年份小於 1582 年的日期（應該被拒絕）
-- 3. 測試無效日期（如 2 月 30 日）的處理
-- 4. 測試閏年日期的處理
-- 5. 測試多語言顯示（月份、星期）
-- 6. 測試快速選擇功能
-- 7. 測試日期選擇器組件在不同表單中的使用

-- =============================================
