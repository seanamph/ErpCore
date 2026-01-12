import { createRouter, createWebHistory } from 'vue-router'

const routes = [
  {
    path: '/',
    redirect: '/transfer/receipt'
  },
  {
    path: '/transfer',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'receipt',
        name: 'TransferReceipt',
        component: () => import('../views/Transfer/Receiving.vue'),
        meta: { title: '調撥單驗收作業 (SYSW352)' }
      },
      {
        path: 'return',
        name: 'TransferReturn',
        component: () => import('../views/Transfer/Return.vue'),
        meta: { title: '調撥單驗退作業 (SYSW362)' }
      },
      {
        path: 'shortage',
        name: 'TransferShortage',
        component: () => import('../views/Transfer/Shortage.vue'),
        meta: { title: '調撥短溢維護作業 (SYSW384)' }
      }
    ]
  },
  {
    path: '/stock-adjustment',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'inventory',
        name: 'StockAdjustment',
        component: () => import('../views/StockAdjustment/InventoryAdjustment.vue'),
        meta: { title: '庫存調整作業 (SYSW490)' }
      }
    ]
  },
  {
    path: '/inventory-check',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'stocktaking',
        name: 'InventoryCheck',
        component: () => import('../views/InventoryCheck/StocktakingPlan.vue'),
        meta: { title: '盤點維護作業 (SYSW53M)' }
      }
    ]
  },
  {
    path: '/inventory',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'supplier-goods',
        name: 'SupplierGoods',
        component: () => import('../views/Inventory/SupplierGoods.vue'),
        meta: { title: '供應商商品資料維護 (SYSW110)' }
      },
      {
        path: 'product-goods-id',
        name: 'ProductGoodsId',
        component: () => import('../views/Inventory/ProductGoodsId.vue'),
        meta: { title: '商品進銷碼維護作業 (SYSW137)' }
      },
      {
        path: 'price-change',
        name: 'PriceChange',
        component: () => import('../views/Inventory/PriceChange.vue'),
        meta: { title: '商品永久變價作業 (SYSW150)' }
      }
    ]
  },
  {
    path: '/purchase',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'order',
        name: 'PurchaseOrder',
        component: () => import('../views/Purchase/PurchaseOrder.vue'),
        meta: { title: '訂退貨申請作業 (SYSW315)' }
      },
      {
        path: 'order-return',
        name: 'PurchaseOrderReturn',
        component: () => import('../views/Purchase/PurchaseOrderReturn.vue'),
        meta: { title: '訂退貨申請作業 (SYSW316)' }
      },
      {
        path: 'receipt',
        name: 'PurchaseReceipt',
        component: () => import('../views/Purchase/PurchaseReceipt.vue'),
        meta: { title: '採購單驗收作業 (SYSW324)' }
      },
      {
        path: 'receipt-v2',
        name: 'PurchaseReceiptV2',
        component: () => import('../views/Purchase/PurchaseReceiptV2.vue'),
        meta: { title: '採購單驗收作業 (SYSW336)' }
      },
      {
        path: 'settled-adjustment',
        name: 'SettledPurchaseReceiptAdjustment',
        component: () => import('../views/Purchase/SettledPurchaseReceiptAdjustment.vue'),
        meta: { title: '已日結採購單驗收調整作業 (SYSW333)' }
      },
      {
        path: 'closed-return-adjustment',
        name: 'ClosedReturnAdjustment',
        component: () => import('../views/Purchase/ClosedReturnAdjustment.vue'),
        meta: { title: '已日結退貨單驗退調整作業 (SYSW530)' }
      },
      {
        path: 'closed-return-adjustment-v2',
        name: 'ClosedReturnAdjustmentV2',
        component: () => import('../views/Purchase/ClosedReturnAdjustmentV2.vue'),
        meta: { title: '已日結退貨單驗退調整作業 (SYSW337)' }
      },
      {
        path: 'on-site-order',
        name: 'OnSitePurchaseOrder',
        component: () => import('../views/Purchase/OnSitePurchaseOrder.vue'),
        meta: { title: '現場打單作業 (SYSW322)' }
      }
    ]
  },
  {
    path: '/customer',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'maintenance',
        name: 'CustomerMaintenance',
        component: () => import('../views/Customer/CustomerMaintenance.vue'),
        meta: { title: '客戶基本資料維護作業 (CUS5110)' }
      },
      {
        path: 'query',
        name: 'CustomerQuery',
        component: () => import('../views/Customer/CustomerQuery.vue'),
        meta: { title: '客戶查詢作業 (CUS5120)' }
      },
      {
        path: 'report',
        name: 'CustomerReport',
        component: () => import('../views/Customer/CustomerReport.vue'),
        meta: { title: '客戶報表 (CUS5130)' }
      }
    ]
  },
  {
    path: '/einvoice',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'process',
        name: 'EInvoiceProcess',
        component: () => import('../views/EInvoice/EInvoiceProcess.vue'),
        meta: { title: '電子發票處理作業 (ECA3010)' }
      },
      {
        path: 'query',
        name: 'EInvoiceQuery',
        component: () => import('../views/EInvoice/EInvoiceQuery.vue'),
        meta: { title: '電子發票查詢作業 (ECA3020)' }
      },
      {
        path: 'upload',
        name: 'EInvoiceUpload',
        component: () => import('../views/EInvoice/EInvoiceUpload.vue'),
        meta: { title: '電子發票上傳作業 (ECA3030)' }
      },
      {
        path: 'upload-2050',
        name: 'ECA2050Upload',
        component: () => import('../views/EInvoice/ECA2050Upload.vue'),
        meta: { title: '電子發票上傳作業 (ECA2050)' }
      },
      {
        path: 'report',
        name: 'EInvoiceReport',
        component: () => import('../views/EInvoice/EInvoiceReport.vue'),
        meta: { title: '電子發票報表查詢 (ECA3040)' }
      }
    ]
  },
  {
    path: '/business-report',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'meal-cards',
        name: 'EmployeeMealCardList',
        component: () => import('../views/BusinessReport/EmployeeMealCardList.vue'),
        meta: { title: '業務報表查詢作業 (SYSL130)' }
      }
    ]
  },
  {
    path: '/analysis-report',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'query/:reportId?',
        name: 'AnalysisReportQuery',
        component: () => import('../views/AnalysisReport/AnalysisReportQuery.vue'),
        meta: { title: '進銷存分析報表 (SYSA1000)' }
      },
      {
        path: 'consumable-label-print',
        name: 'ConsumableLabelPrint',
        component: () => import('../views/AnalysisReport/ConsumableLabelPrint.vue'),
        meta: { title: '耗材標籤列印作業 (SYSA254)' }
      },
      {
        path: 'consumable-report',
        name: 'ConsumableReportQuery',
        component: () => import('../views/AnalysisReport/ConsumableReportQuery.vue'),
        meta: { title: '耗材管理報表 (SYSA255)' }
      }
    ]
  },
  {
    path: '/system',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'permissions',
        name: 'Permissions',
        component: () => import('../views/System/Permissions.vue'),
        meta: { title: '權限管理 (SYS0310, SYS0320, SYS0330, SYS0340)' }
      },
      {
        path: 'role-permissions',
        name: 'SYS0310Query',
        component: () => import('../views/System/SYS0310Query.vue'),
        meta: { title: '角色系統權限設定 (SYS0310)' }
      },
      {
        path: 'user-permissions',
        name: 'SYS0320Query',
        component: () => import('../views/System/SYS0320Query.vue'),
        meta: { title: '使用者系統權限設定 (SYS0320)' }
      },
      {
        path: 'role-field-permissions',
        name: 'SYS0330Query',
        component: () => import('../views/System/SYS0330Query.vue'),
        meta: { title: '角色欄位權限設定 (SYS0330)' }
      },
      {
        path: 'system-config',
        name: 'SystemConfig',
        component: () => import('../views/System/SystemConfig.vue'),
        meta: { title: '系統設定 (CFG0410, CFG0420, CFG0430, CFG0440)' }
      },
      {
        path: 'change-logs',
        name: 'ChangeLogs',
        component: () => import('../views/System/ChangeLogs.vue'),
        meta: { title: '異動記錄查詢 (SYS0610-SYS0660)' }
      },
      {
        path: 'users',
        name: 'SYS0110Query',
        component: () => import('../views/System/SYS0110Query.vue'),
        meta: { title: '使用者基本資料維護 (SYS0110)' }
      },
      {
        path: 'users-browse',
        name: 'SYS0120Query',
        component: () => import('../views/System/SYS0120Query.vue'),
        meta: { title: '使用者資料瀏覽 (SYS0120)' }
      },
      {
        path: 'users-query',
        name: 'SYS0910Query',
        component: () => import('../views/System/SYS0910Query.vue'),
        meta: { title: '使用者查詢功能 (SYS0910)' }
      },
      {
        path: 'user-agents',
        name: 'SYS0117Query',
        component: () => import('../views/System/SYS0117Query.vue'),
        meta: { title: '使用者權限代理 (SYS0117)' }
      },
      {
        path: 'user-schedules',
        name: 'SYS0116Query',
        component: () => import('../views/System/SYS0116Query.vue'),
        meta: { title: '排程修改使用者基本資料 (SYS0116)' }
      },
      {
        path: 'login-log-query',
        name: 'SYS0760Query',
        component: () => import('../views/System/SYS0760Query.vue'),
        meta: { title: '使用者異常登入報表 (SYS0760)' }
      },
      {
        path: 'user-change-logs',
        name: 'SYS0610Query',
        component: () => import('../views/System/SYS0610Query.vue'),
        meta: { title: '使用者基本資料異動查詢 (SYS0610)' }
      },
      {
        path: 'role-change-logs',
        name: 'SYS0620Query',
        component: () => import('../views/System/SYS0620Query.vue'),
        meta: { title: '角色基本資料異動查詢 (SYS0620)' }
      },
      {
        path: 'user-role-change-logs',
        name: 'SYS0630Query',
        component: () => import('../views/System/SYS0630Query.vue'),
        meta: { title: '使用者角色對應設定異動查詢 (SYS0630)' }
      },
      {
        path: 'system-permission-change-logs',
        name: 'SYS0640Query',
        component: () => import('../views/System/SYS0640Query.vue'),
        meta: { title: '系統權限異動記錄查詢 (SYS0640)' }
      },
      {
        path: 'controllable-field-change-logs',
        name: 'SYS0650Query',
        component: () => import('../views/System/SYS0650Query.vue'),
        meta: { title: '可管控欄位異動記錄查詢 (SYS0650)' }
      },
      {
        path: 'other-change-logs',
        name: 'SYS0660Query',
        component: () => import('../views/System/SYS0660Query.vue'),
        meta: { title: '其他異動記錄查詢 (SYS0660)' }
      },
      {
        path: 'system-permission-list',
        name: 'SYS0710Query',
        component: () => import('../views/System/SYS0710Query.vue'),
        meta: { title: '系統權限列表 (SYS0710)' }
      },
      {
        path: 'user-account-policy',
        name: 'SYS0130Query',
        component: () => import('../views/System/SYS0130Query.vue'),
        meta: { title: '使用者帳戶原則管理 (SYS0130)' }
      },
      {
        path: 'user-roles',
        name: 'SYS0220Query',
        component: () => import('../views/System/SYS0220Query.vue'),
        meta: { title: '使用者之角色設定維護 (SYS0220)' }
      },
      {
        path: 'role-users',
        name: 'SYS0230Query',
        component: () => import('../views/System/SYS0230Query.vue'),
        meta: { title: '角色之使用者設定維護 (SYS0230)' }
      },
      {
        path: 'role-copy',
        name: 'SYS0240Query',
        component: () => import('../views/System/SYS0240Query.vue'),
        meta: { title: '角色複製 (SYS0240)' }
      },
      {
        path: 'program-user-permissions',
        name: 'SYS0720Query',
        component: () => import('../views/System/ProgramUserPermissionReport.vue'),
        meta: { title: '作業權限之使用者列表 (SYS0720)' }
      },
      {
        path: 'role-system-permissions',
        name: 'SYS0731Query',
        component: () => import('../views/System/RoleSystemPermissionListReport.vue'),
        meta: { title: '角色系統權限列表 (SYS0731)' }
      },
      {
        path: 'program-role-permissions',
        name: 'SYS0740Query',
        component: () => import('../views/System/ProgramRolePermissionListReport.vue'),
        meta: { title: '作業權限之角色列表 (SYS0740)' }
      },
      {
        path: 'role-user-list',
        name: 'SYS0750Query',
        component: () => import('../views/System/RoleUserListReport.vue'),
        meta: { title: '角色之使用者列表 (SYS0750)' }
      },
      {
        path: 'role-users',
        name: 'SYS0750Query',
        component: () => import('../views/System/RoleUserListReport.vue'),
        meta: { title: '角色之使用者列表 (SYS0750)' }
      },
      {
        path: 'user-abnormal-login',
        name: 'SYS0760Query',
        component: () => import('../views/System/UserAbnormalLoginReport.vue'),
        meta: { title: '使用者異常登入報表 (SYS0760)' }
      },
      {
        path: 'button-logs',
        name: 'SYS0790Query',
        component: () => import('../views/System/ButtonLogReport.vue'),
        meta: { title: '按鈕操作記錄查詢 (SYS0790)' }
      },
      {
        path: 'system-program-buttons',
        name: 'SYS0810Query',
        component: () => import('../views/System/SystemProgramButtonReport.vue'),
        meta: { title: '系統作業與功能列表查詢 (SYS0810)' }
      }
    ]
  },
  {
    path: '/pos',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'transaction',
        name: 'PosTransaction',
        component: () => import('../views/Pos/Transaction.vue'),
        meta: { title: 'POS交易查詢' }
      },
      {
        path: 'report',
        name: 'PosReport',
        component: () => import('../views/Pos/Report.vue'),
        meta: { title: 'POS報表查詢' }
      },
      {
        path: 'sync',
        name: 'PosSync',
        component: () => import('../views/Pos/Sync.vue'),
        meta: { title: 'POS資料同步作業' }
      }
    ]
  },
  {
    path: '/tools',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'file-upload',
        name: 'FileUpload',
        component: () => import('../views/Tools/FileUpload.vue'),
        meta: { title: '檔案上傳工具' }
      },
      {
        path: 'barcode',
        name: 'Barcode',
        component: () => import('../views/Tools/Barcode.vue'),
        meta: { title: '條碼處理工具' }
      },
      {
        path: 'html2pdf',
        name: 'Html2Pdf',
        component: () => import('../views/Tools/Html2Pdf.vue'),
        meta: { title: 'HTML轉PDF工具' }
      }
    ]
  },
  {
    path: '/kiosk',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'report',
        name: 'KioskReport',
        component: () => import('../views/Kiosk/Report.vue'),
        meta: { title: 'Kiosk報表查詢' }
      },
      {
        path: 'process',
        name: 'KioskProcess',
        component: () => import('../views/Kiosk/Process.vue'),
        meta: { title: 'Kiosk資料處理作業' }
      }
    ]
  },
  {
    path: '/dropdown-list',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'address',
        name: 'AddressList',
        component: () => import('../views/DropdownList/AddressList.vue'),
        meta: { title: '地址列表 (ADDR_CITY_LIST, ADDR_ZONE_LIST)' }
      },
      {
        path: 'date',
        name: 'DateList',
        component: () => import('../views/DropdownList/DateList.vue'),
        meta: { title: '日期列表 (DATE_LIST)' }
      },
      {
        path: 'menu',
        name: 'MenuList',
        component: () => import('../views/DropdownList/MenuList.vue'),
        meta: { title: '選單列表 (MENU_LIST)' }
      },
      {
        path: 'multi-select',
        name: 'MultiSelectList',
        component: () => import('../views/DropdownList/MultiSelectList.vue'),
        meta: { title: '多選列表 (MULTI_AREA_LIST, MULTI_SHOP_LIST, MULTI_USERS_LIST)' }
      },
      {
        path: 'system',
        name: 'SystemList',
        component: () => import('../views/DropdownList/SystemList.vue'),
        meta: { title: '系統列表 (SYSID_LIST, USER_LIST)' }
      }
    ]
  },
  {
    path: '/communication',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'auto-process-mail',
        name: 'AutoProcessMail',
        component: () => import('../views/Communication/AutoProcessMail.vue'),
        meta: { title: '自動處理郵件作業 (AutoProcessMail)' }
      },
      {
        path: 'encode-data',
        name: 'EncodeData',
        component: () => import('../views/Communication/EncodeData.vue'),
        meta: { title: '資料編碼作業 (EncodeData)' }
      },
      {
        path: 'mail-sms',
        name: 'MailSms',
        component: () => import('../views/Communication/MailSms.vue'),
        meta: { title: '郵件簡訊發送作業 (SYS5000)' }
      }
    ]
  },
  {
    path: '/ui-component',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'data-maintenance',
        name: 'DataMaintenance',
        component: () => import('../views/UiComponent/DataMaintenance.vue'),
        meta: { title: '資料維護UI組件 (IMS30系列)' }
      },
      {
        path: 'query-report',
        name: 'QueryReport',
        component: () => import('../views/UiComponent/QueryReport.vue'),
        meta: { title: 'UI組件查詢與報表' }
      }
    ]
  },
  {
    path: '/report-extension',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'module-o',
        name: 'ReportModuleO',
        component: () => import('../views/ReportExtension/ModuleO.vue'),
        meta: { title: '報表模組O' }
      },
      {
        path: 'module-n',
        name: 'ReportModuleN',
        component: () => import('../views/ReportExtension/ModuleN.vue'),
        meta: { title: '報表模組N' }
      },
      {
        path: 'module-wp',
        name: 'ReportModuleWP',
        component: () => import('../views/ReportExtension/ModuleWP.vue'),
        meta: { title: '報表模組WP' }
      },
      {
        path: 'module-7',
        name: 'ReportModule7',
        component: () => import('../views/ReportExtension/Module7.vue'),
        meta: { title: '報表模組7 (SYS7000)' }
      },
      {
        path: 'print',
        name: 'ReportPrint',
        component: () => import('../views/ReportExtension/Print.vue'),
        meta: { title: '報表列印作業 (SYS7B10-SYS7B40)' }
      },
      {
        path: 'statistics',
        name: 'ReportStatistics',
        component: () => import('../views/ReportExtension/Statistics.vue'),
        meta: { title: '報表統計作業 (SYS7C10, SYS7C30)' }
      }
    ]
  },
  {
    path: '/core',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'user-management',
        name: 'UserManagement',
        component: () => import('../views/Core/UserManagement.vue'),
        meta: { title: '使用者管理' }
      },
      {
        path: 'framework',
        name: 'Framework',
        component: () => import('../views/Core/Framework.vue'),
        meta: { title: '框架功能' }
      },
      {
        path: 'tools',
        name: 'Tools',
        component: () => import('../views/Core/Tools.vue'),
        meta: { title: '工具功能' }
      },
      {
        path: 'system-function',
        name: 'SystemFunction',
        component: () => import('../views/Core/SystemFunction.vue'),
        meta: { title: '系統功能' }
      }
    ]
  },
  {
    path: '/other-module',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'crp-report',
        name: 'CrpReport',
        component: () => import('../views/OtherModule/CrpReport.vue'),
        meta: { title: 'CRP報表模組' }
      },
      {
        path: 'eip-integration',
        name: 'EipIntegration',
        component: () => import('../views/OtherModule/EipIntegration.vue'),
        meta: { title: 'EIP系統整合 (IMS2EIP)' }
      },
      {
        path: 'lab-test',
        name: 'LabTest',
        component: () => import('../views/OtherModule/LabTest.vue'),
        meta: { title: '實驗室測試功能 (Lab)' }
      }
    ]
  },
  {
    path: '/human-resource',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'personnel',
        name: 'Personnel',
        component: () => import('../views/HumanResource/Personnel.vue'),
        meta: { title: '人事管理 (SYSH110)' }
      },
      {
        path: 'payroll',
        name: 'Payroll',
        component: () => import('../views/HumanResource/Payroll.vue'),
        meta: { title: '薪資管理 (SYSH210)' }
      },
      {
        path: 'attendance',
        name: 'Attendance',
        component: () => import('../views/HumanResource/Attendance.vue'),
        meta: { title: '考勤管理' }
      }
    ]
  },
  {
    path: '/accounting',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'account-subject',
        name: 'AccountSubject',
        component: () => import('../views/Accounting/AccountSubject.vue'),
        meta: { title: '會計科目維護 (SYSN110)' }
      },
      {
        path: 'accounting',
        name: 'Accounting',
        component: () => import('../views/Accounting/Accounting.vue'),
        meta: { title: '會計管理 (SYSN120-SYSN154)' }
      },
      {
        path: 'financial-transaction',
        name: 'FinancialTransaction',
        component: () => import('../views/Accounting/FinancialTransaction.vue'),
        meta: { title: '財務交易 (SYSN210-SYSN213)' }
      },
      {
        path: 'asset',
        name: 'Asset',
        component: () => import('../views/Accounting/Asset.vue'),
        meta: { title: '資產管理 (SYSN310-SYSN311)' }
      },
      {
        path: 'financial-report',
        name: 'FinancialReport',
        component: () => import('../views/Accounting/FinancialReport.vue'),
        meta: { title: '財務報表 (SYSN510-SYSN540)' }
      },
      {
        path: 'other-financial',
        name: 'OtherFinancial',
        component: () => import('../views/Accounting/OtherFinancial.vue'),
        meta: { title: '其他財務功能 (SYSN610-SYSN910)' }
      }
    ]
  },
  {
    path: '/tax-accounting',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'accounting-subject',
        name: 'TaxAccountingSubject',
        component: () => import('../views/TaxAccounting/AccountingSubject.vue'),
        meta: { title: '會計科目維護 (SYST111-SYST11A)' }
      },
      {
        path: 'accounting-voucher',
        name: 'AccountingVoucher',
        component: () => import('../views/TaxAccounting/AccountingVoucher.vue'),
        meta: { title: '會計憑證管理 (SYST121-SYST12B)' }
      },
      {
        path: 'accounting-book',
        name: 'AccountingBook',
        component: () => import('../views/TaxAccounting/AccountingBook.vue'),
        meta: { title: '會計帳簿管理 (SYST131-SYST134)' }
      },
      {
        path: 'invoice-data',
        name: 'InvoiceData',
        component: () => import('../views/TaxAccounting/InvoiceData.vue'),
        meta: { title: '發票資料維護 (SYST211-SYST212)' }
      },
      {
        path: 'transaction-data',
        name: 'TransactionData',
        component: () => import('../views/TaxAccounting/TransactionData.vue'),
        meta: { title: '交易資料處理 (SYST221, SYST311-SYST352)' }
      },
      {
        path: 'tax-report',
        name: 'TaxReport',
        component: () => import('../views/TaxAccounting/TaxReport.vue'),
        meta: { title: '稅務報表查詢 (SYST411-SYST452)' }
      },
      {
        path: 'tax-report-print',
        name: 'TaxReportPrint',
        component: () => import('../views/TaxAccounting/TaxReportPrint.vue'),
        meta: { title: '稅務報表列印 (SYST510-SYST530)' }
      },
      {
        path: 'voucher-audit',
        name: 'VoucherAudit',
        component: () => import('../views/TaxAccounting/VoucherAudit.vue'),
        meta: { title: '暫存傳票審核作業 (SYSTA00-SYSTA70)' }
      },
      {
        path: 'voucher-import',
        name: 'VoucherImport',
        component: () => import('../views/TaxAccounting/VoucherImport.vue'),
        meta: { title: '傳票轉入作業 (SYST002-SYST003)' }
      }
    ]
  },
  {
    path: '/procurement',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'procurement',
        name: 'Procurement',
        component: () => import('../views/Procurement/Procurement.vue'),
        meta: { title: '採購管理 (SYSP110-SYSP190)' }
      },
      {
        path: 'supplier',
        name: 'ProcurementSupplier',
        component: () => import('../views/Procurement/Supplier.vue'),
        meta: { title: '供應商管理 (SYSP210-SYSP260)' }
      },
      {
        path: 'payment',
        name: 'ProcurementPayment',
        component: () => import('../views/Procurement/Payment/index.vue'),
        meta: { title: '付款管理 (SYSP271-SYSP2B0)' }
      },
      {
        path: 'bank-management',
        name: 'ProcurementBankManagement',
        component: () => import('../views/Procurement/BankManagement/index.vue'),
        meta: { title: '銀行管理' }
      },
      {
        path: 'procurement-report',
        name: 'ProcurementReport',
        component: () => import('../views/Procurement/ProcurementReport/index.vue'),
        meta: { title: '採購報表查詢 (SYSP410-SYSP4I0)' }
      },
      {
        path: 'procurement-other',
        name: 'ProcurementOther',
        component: () => import('../views/Procurement/ProcurementOther/index.vue'),
        meta: { title: '採購其他功能 (SYSP510-SYSP530)' }
      }
    ]
  },
  {
    path: '/contract',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'contract-data',
        name: 'ContractData',
        component: () => import('../views/Contract/ContractData/index.vue'),
        meta: { title: '合同資料維護 (SYSF110-SYSF140)' }
      },
      {
        path: 'contract-process',
        name: 'ContractProcess',
        component: () => import('../views/Contract/ContractProcess/index.vue'),
        meta: { title: '合同處理作業 (SYSF210-SYSF220)' }
      },
      {
        path: 'contract-extension',
        name: 'ContractExtension',
        component: () => import('../views/Contract/ContractExtension/index.vue'),
        meta: { title: '合同擴展維護 (SYSF350-SYSF540)' }
      },
      {
        path: 'cms-contract',
        name: 'CmsContract',
        component: () => import('../views/Contract/CmsContract/index.vue'),
        meta: { title: 'CMS合同維護 (CMS2310-CMS2320)' }
      }
    ]
  },
  {
    path: '/lease',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'lease-data',
        name: 'LeaseData',
        component: () => import('../views/Lease/LeaseData/index.vue'),
        meta: { title: '租賃資料維護 (SYS8110-SYS8220)' }
      },
      {
        path: 'lease-extension',
        name: 'LeaseExtension',
        component: () => import('../views/Lease/LeaseExtension/index.vue'),
        meta: { title: '租賃擴展維護 (SYS8A10-SYS8A45)' }
      },
      {
        path: 'lease-process',
        name: 'LeaseProcess',
        component: () => import('../views/Lease/LeaseProcess/index.vue'),
        meta: { title: '租賃處理作業 (SYS8B50-SYS8B90)' }
      }
    ]
  },
  {
    path: '/lease-syse',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'lease-syse-data',
        name: 'LeaseSYSEData',
        component: () => import('../views/LeaseSYSE/LeaseSYSEData/index.vue'),
        meta: { title: '租賃資料維護 (SYSE110-SYSE140)' }
      },
      {
        path: 'lease-syse-extension',
        name: 'LeaseSYSEExtension',
        component: () => import('../views/LeaseSYSE/LeaseSYSEExtension/index.vue'),
        meta: { title: '租賃擴展維護 (SYSE210-SYSE230)' }
      },
      {
        path: 'lease-syse-fee',
        name: 'LeaseSYSEFee',
        component: () => import('../views/LeaseSYSE/LeaseSYSEFee/index.vue'),
        meta: { title: '費用資料維護 (SYSE310-SYSE430)' }
      }
    ]
  },
  {
    path: '/lease-sysm',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'lease-sysm-data',
        name: 'LeaseSYSMData',
        component: () => import('../views/LeaseSYSM/LeaseSYSMData/index.vue'),
        meta: { title: '租賃資料維護 (SYSM111-SYSM138)' }
      },
      {
        path: 'lease-sysm-report',
        name: 'LeaseSYSMReport',
        component: () => import('../views/LeaseSYSM/LeaseSYSMReport/index.vue'),
        meta: { title: '租賃報表查詢 (SYSM141-SYSM144)' }
      }
    ]
  },
  {
    path: '/sales',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'sales-data',
        name: 'SalesData',
        component: () => import('../views/Sales/SalesData/index.vue'),
        meta: { title: '銷售資料維護 (SYSD110-SYSD140)' }
      },
      {
        path: 'sales-process',
        name: 'SalesProcess',
        component: () => import('../views/Sales/SalesProcess/index.vue'),
        meta: { title: '銷售處理作業 (SYSD210-SYSD230)' }
      },
      {
        path: 'sales-report',
        name: 'SalesReport',
        component: () => import('../views/Sales/SalesReport/index.vue'),
        meta: { title: '銷售報表查詢 (SYSD310-SYSD430)' }
      }
    ]
  },
  {
    path: '/energy',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'energy-base',
        name: 'EnergyBase',
        component: () => import('../views/Energy/EnergyBase/index.vue'),
        meta: { title: '能源基礎維護 (SYSO100-SYSO130)' }
      },
      {
        path: 'energy-process',
        name: 'EnergyProcess',
        component: () => import('../views/Energy/EnergyProcess/index.vue'),
        meta: { title: '能源處理作業 (SYSO310)' }
      },
      {
        path: 'energy-extension',
        name: 'EnergyExtension',
        component: () => import('../views/Energy/EnergyExtension/index.vue'),
        meta: { title: '能源擴展維護 (SYSOU10-SYSOU33)' }
      }
    ]
  },
  {
    path: '/report-management',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'receiving-base',
        name: 'ReceivingBase',
        component: () => import('../views/ReportManagement/ReceivingBase/index.vue'),
        meta: { title: '收款基礎功能 (SYSR110-SYSR120)' }
      },
      {
        path: 'receiving-process',
        name: 'ReceivingProcess',
        component: () => import('../views/ReportManagement/ReceivingProcess/index.vue'),
        meta: { title: '收款處理功能 (SYSR210-SYSR240)' }
      },
      {
        path: 'receiving-extension',
        name: 'ReceivingExtension',
        component: () => import('../views/ReportManagement/ReceivingExtension/index.vue'),
        meta: { title: '收款擴展功能 (SYSR310-SYSR450)' }
      },
      {
        path: 'receiving-other',
        name: 'ReceivingOther',
        component: () => import('../views/ReportManagement/ReceivingOther/index.vue'),
        meta: { title: '收款其他功能 (SYSR510-SYSR570)' }
      }
    ]
  },
  {
    path: '/certificate',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'certificate-data',
        name: 'CertificateData',
        component: () => import('../views/Certificate/CertificateData/index.vue'),
        meta: { title: '憑證資料維護 (SYSK110-SYSK150)' }
      },
      {
        path: 'certificate-process',
        name: 'CertificateProcess',
        component: () => import('../views/Certificate/CertificateProcess/index.vue'),
        meta: { title: '憑證處理作業 (SYSK210-SYSK230)' }
      },
      {
        path: 'certificate-report',
        name: 'CertificateReport',
        component: () => import('../views/Certificate/CertificateReport/index.vue'),
        meta: { title: '憑證報表查詢 (SYSK310-SYSK500)' }
      }
    ]
  },
  {
    path: '/extension',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'index',
        name: 'Extension',
        component: () => import('../views/Extension/index.vue'),
        meta: { title: '擴展功能維護 (SYS9000)' }
      }
    ]
  },
  {
    path: '/query',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'query',
        name: 'Query',
        component: () => import('../views/Query/Query/index.vue'),
        meta: { title: '查詢功能維護 (SYSQ000)' }
      },
      {
        path: 'quality-base',
        name: 'QualityBase',
        component: () => import('../views/Query/QualityBase/index.vue'),
        meta: { title: '質量管理基礎功能 (SYSQ110-SYSQ120)' }
      },
      {
        path: 'quality-process',
        name: 'QualityProcess',
        component: () => import('../views/Query/QualityProcess/index.vue'),
        meta: { title: '質量管理處理功能 (SYSQ210-SYSQ250)' }
      },
      {
        path: 'quality-report',
        name: 'QualityReport',
        component: () => import('../views/Query/QualityReport/index.vue'),
        meta: { title: '質量管理報表功能 (SYSQ310-SYSQ340)' }
      }
    ]
  },
  {
    path: '/other-management',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'system-s',
        name: 'SystemS',
        component: () => import('../views/OtherManagement/SystemS/index.vue'),
        meta: { title: 'S系統功能維護 (SYSS000)' }
      },
      {
        path: 'system-u',
        name: 'SystemU',
        component: () => import('../views/OtherManagement/SystemU/index.vue'),
        meta: { title: 'U系統功能維護 (SYSU000)' }
      },
      {
        path: 'system-v',
        name: 'SystemV',
        component: () => import('../views/OtherManagement/SystemV/index.vue'),
        meta: { title: 'V系統功能維護 (SYSV000)' }
      },
      {
        path: 'system-j',
        name: 'SystemJ',
        component: () => import('../views/OtherManagement/SystemJ/index.vue'),
        meta: { title: 'J系統功能維護 (SYSJ000)' }
      }
    ]
  },
  {
    path: '/customer-invoice',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'customer-data',
        name: 'CustomerData',
        component: () => import('../views/CustomerInvoice/CustomerData/index.vue'),
        meta: { title: '客戶資料維護 (SYS2000)' }
      },
      {
        path: 'invoice-print',
        name: 'InvoicePrint',
        component: () => import('../views/CustomerInvoice/InvoicePrint/index.vue'),
        meta: { title: '發票列印作業 (SYS2000)' }
      },
      {
        path: 'mail-fax',
        name: 'MailFax',
        component: () => import('../views/CustomerInvoice/MailFax/index.vue'),
        meta: { title: '郵件傳真作業 (SYS2000)' }
      },
      {
        path: 'ledger-data',
        name: 'LedgerData',
        component: () => import('../views/CustomerInvoice/LedgerData/index.vue'),
        meta: { title: '總帳資料維護 (SYS2000)' }
      }
    ]
  },
  {
    path: '/invoice-sales',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'invoice-data',
        name: 'InvoiceSalesInvoiceData',
        component: () => import('../views/InvoiceSales/InvoiceData/index.vue'),
        meta: { title: '發票資料維護 (SYSG110-SYSG190)' }
      },
      {
        path: 'invoice-print',
        name: 'InvoiceSalesInvoicePrint',
        component: () => import('../views/InvoiceSales/InvoicePrint/index.vue'),
        meta: { title: '電子發票列印 (SYSG210-SYSG2B0)' }
      },
      {
        path: 'sales-query',
        name: 'InvoiceSalesSalesQuery',
        component: () => import('../views/InvoiceSales/SalesQuery/index.vue'),
        meta: { title: '銷售查詢作業 (SYSG510-SYSG5D0)' }
      },
      {
        path: 'sales-report-query',
        name: 'InvoiceSalesSalesReportQuery',
        component: () => import('../views/InvoiceSales/SalesReportQuery/index.vue'),
        meta: { title: '報表查詢作業 (SYSG610-SYSG640)' }
      },
      {
        path: 'sales-report-print',
        name: 'InvoiceSalesSalesReportPrint',
        component: () => import('../views/InvoiceSales/SalesReportPrint/index.vue'),
        meta: { title: '報表列印作業 (SYSG710-SYSG7I0)' }
      }
    ]
  },
  {
    path: '/invoice-sales-b2b',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'b2b-invoice-data',
        name: 'B2BInvoiceData',
        component: () => import('../views/InvoiceSalesB2B/B2BInvoiceData/index.vue'),
        meta: { title: 'B2B發票資料維護' }
      },
      {
        path: 'b2b-invoice-print',
        name: 'B2BInvoicePrint',
        component: () => import('../views/InvoiceSalesB2B/B2BInvoicePrint/index.vue'),
        meta: { title: 'B2B電子發票列印' }
      },
      {
        path: 'b2b-sales-data',
        name: 'B2BSalesData',
        component: () => import('../views/InvoiceSalesB2B/B2BSalesData/index.vue'),
        meta: { title: 'B2B銷售資料維護' }
      },
      {
        path: 'b2b-sales-query',
        name: 'B2BSalesQuery',
        component: () => import('../views/InvoiceSalesB2B/B2BSalesQuery/index.vue'),
        meta: { title: 'B2B銷售查詢作業' }
      }
    ]
  },
  {
    path: '/system-extension-e',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'employee-data',
        name: 'EmployeeData',
        component: () => import('../views/SystemExtensionE/EmployeeData/index.vue'),
        meta: { title: '員工資料維護 (SYSPE10-SYSPE11)' }
      },
      {
        path: 'personnel-data',
        name: 'PersonnelData',
        component: () => import('../views/SystemExtensionE/PersonnelData/index.vue'),
        meta: { title: '人事資料維護 (SYSPED0)' }
      }
    ]
  },
  {
    path: '/system-extension-h',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'personnel-batch',
        name: 'PersonnelBatch',
        component: () => import('../views/SystemExtensionH/PersonnelBatch/index.vue'),
        meta: { title: '人事批量新增 (SYSH3D0_FMI)' }
      },
      {
        path: 'system-extension-ph',
        name: 'SystemExtensionPH',
        component: () => import('../views/SystemExtensionH/SystemExtensionPH/index.vue'),
        meta: { title: '感應卡維護作業 (SYSPH00)' }
      }
    ]
  },
  {
    path: '/loyalty',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'init',
        name: 'LoyaltyInit',
        component: () => import('../views/Loyalty/LoyaltyInit/index.vue'),
        meta: { title: '忠誠度系統初始化 (WEBLOYALTYINI)' }
      },
      {
        path: 'maintenance',
        name: 'LoyaltyMaintenance',
        component: () => import('../views/Loyalty/LoyaltyMaintenance/index.vue'),
        meta: { title: '忠誠度系統維護 (LPS)' }
      }
    ]
  },
  {
    path: '/chart-tools',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'chart',
        name: 'Chart',
        component: () => import('../views/ChartTools/Chart/index.vue'),
        meta: { title: '圖表功能 (SYS1000)' }
      },
      {
        path: 'tools',
        name: 'Tools',
        component: () => import('../views/ChartTools/Tools/index.vue'),
        meta: { title: '工具功能' }
      }
    ]
  },
  {
    path: '/system-exit',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: '',
        name: 'SystemExit',
        component: () => import('../views/SystemExit/index.vue'),
        meta: { title: '系統退出 (SYS9999)' }
      }
    ]
  },
  {
    path: '/customer-custom',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'cus3000',
        name: 'Cus3000',
        component: () => import('../views/CustomerCustom/Cus3000/index.vue'),
        meta: { title: 'CUS3000 客戶定制模組' }
      },
      {
        path: 'cus3000-eskyland',
        name: 'Cus3000Eskyland',
        component: () => import('../views/CustomerCustom/Cus3000Eskyland/index.vue'),
        meta: { title: 'CUS3000.ESKYLAND 客戶定制模組' }
      },
      {
        path: 'cus5000-eskyland',
        name: 'Cus5000Eskyland',
        component: () => import('../views/CustomerCustom/Cus5000Eskyland/index.vue'),
        meta: { title: 'CUS5000.ESKYLAND 客戶定制模組' }
      },
      {
        path: 'cus-backup',
        name: 'CusBackup',
        component: () => import('../views/CustomerCustom/CusBackup/index.vue'),
        meta: { title: 'CUSBACKUP 客戶定制模組' }
      },
      {
        path: 'cus-cts',
        name: 'CusCts',
        component: () => import('../views/CustomerCustom/CusCts/index.vue'),
        meta: { title: 'CUSCTS 客戶定制模組' }
      },
      {
        path: 'cus-hanshin',
        name: 'CusHanshin',
        component: () => import('../views/CustomerCustom/CusHanshin/index.vue'),
        meta: { title: 'CUSHANSHIN 客戶定制模組' }
      },
      {
        path: 'sys8000-eskyland',
        name: 'Sys8000Eskyland',
        component: () => import('../views/CustomerCustom/Sys8000Eskyland/index.vue'),
        meta: { title: 'SYS8000.ESKYLAND 客戶定制模組' }
      }
    ]
  },
  {
    path: '/standard-module',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'std3000',
        name: 'Std3000',
        component: () => import('../views/StandardModule/Std3000/index.vue'),
        meta: { title: 'STD3000 標準模組 (SYS3620)' }
      },
      {
        path: 'std5000',
        name: 'Std5000',
        component: () => import('../views/StandardModule/Std5000/index.vue'),
        meta: { title: 'STD5000 標準模組' }
      }
    ]
  },
  {
    path: '/mir-module',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'mirh000',
        name: 'MirH000',
        component: () => import('../views/MirModule/MirH000/index.vue'),
        meta: { title: 'MIRH000 系列模組' }
      },
      {
        path: 'mirv000',
        name: 'MirV000',
        component: () => import('../views/MirModule/MirV000/index.vue'),
        meta: { title: 'MIRV000 系列模組' }
      },
      {
        path: 'mirw000',
        name: 'MirW000',
        component: () => import('../views/MirModule/MirW000/index.vue'),
        meta: { title: 'MIRW000 系列模組' }
      }
    ]
  },
  {
    path: '/msh-module',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'msh3000',
        name: 'Msh3000',
        component: () => import('../views/MshModule/Msh3000/index.vue'),
        meta: { title: 'MSH3000 模組' }
      }
    ]
  },
  {
    path: '/sap-integration',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'trans-sap',
        name: 'TransSap',
        component: () => import('../views/SapIntegration/TransSap/index.vue'),
        meta: { title: 'TransSAP 整合模組' }
      }
    ]
  },
  {
    path: '/universal-module',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'univ000',
        name: 'Univ000',
        component: () => import('../views/UniversalModule/Univ000/index.vue'),
        meta: { title: 'UNIV000 通用模組' }
      }
    ]
  },
  {
    path: '/customer-custom-jgjn',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'sys-cust-jgjn',
        name: 'SysCustJgjn',
        component: () => import('../views/CustomerCustomJgjn/SysCustJgjn/index.vue'),
        meta: { title: 'SYSCUST_JGJN 客戶定制JGJN模組' }
      }
    ]
  },
  {
    path: '/communication-module',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'xcom',
        name: 'Xcom',
        component: () => import('../views/CommunicationModule/Xcom/index.vue'),
        meta: { title: 'XCOM000 通訊模組' }
      },
      {
        path: 'xcom-msg',
        name: 'XcomMsg',
        component: () => import('../views/CommunicationModule/XcomMsg/index.vue'),
        meta: { title: 'XCOMMSG 錯誤訊息處理' }
      }
    ]
  },
  {
    path: '/invoice-extension',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: '',
        name: 'InvoiceExtension',
        component: () => import('../views/InvoiceExtension/index.vue'),
        meta: { title: '電子發票擴展功能' }
      }
    ]
  },
  {
    path: '/sales-report',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'sales-report-module',
        name: 'SalesReportModule',
        component: () => import('../views/SalesReport/SalesReportModule/index.vue'),
        meta: { title: '銷售報表模組 (SYS1000)' }
      },
      {
        path: 'crystal-report',
        name: 'CrystalReport',
        component: () => import('../views/SalesReport/CrystalReport/index.vue'),
        meta: { title: 'Crystal Reports 報表功能 (SYS1360)' }
      }
    ]
  },
  {
    path: '/recruitment',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'tenant-locations',
        name: 'TenantLocationList',
        component: () => import('../views/Recruitment/TenantLocationList.vue'),
        meta: { title: '招商其他功能 - 租戶位置維護 (SYSC999)' }
      }
    ]
  },
  {
    path: '/master-data',
    component: () => import('../views/Layout.vue'),
    children: [
      {
        path: 'product-category',
        name: 'ProductCategory',
        component: () => import('../views/BasicData/ProductCategory.vue'),
        meta: { title: '商品分類資料維護作業 (SYSB110)' }
      },
      {
        path: 'departments',
        name: 'Departments',
        component: () => import('../views/BasicData/Departments.vue'),
        meta: { title: '部別資料維護作業 (SYSWB40)' }
      },
      {
        path: 'parameters',
        name: 'Parameters',
        component: () => import('../views/BasicData/Parameters.vue'),
        meta: { title: '參數資料設定維護作業 (SYSBC40)' }
      },
      {
        path: 'banks',
        name: 'Banks',
        component: () => import('../views/BasicData/Banks.vue'),
        meta: { title: '銀行基本資料維護作業 (SYSBC20)' }
      },
      {
        path: 'regions',
        name: 'Regions',
        component: () => import('../views/BasicData/Regions.vue'),
        meta: { title: '地區設定 (SYSBC30)' }
      },
      {
        path: 'warehouses',
        name: 'Warehouses',
        component: () => import('../views/BasicData/Warehouses.vue'),
        meta: { title: '庫別資料維護作業 (SYSWB60)' }
      },
      {
        path: 'groups',
        name: 'Groups',
        component: () => import('../views/BasicData/Groups.vue'),
        meta: { title: '組別資料維護作業 (SYSWB70)' }
      },
      {
        path: 'areas',
        name: 'Areas',
        component: () => import('../views/BasicData/Areas.vue'),
        meta: { title: '區域基本資料維護作業 (SYSB450)' }
      },
      {
        path: 'vendors',
        name: 'Vendors',
        component: () => import('../views/BasicData/Vendors.vue'),
        meta: { title: '廠/客基本資料維護作業 (SYSB206)' }
      }
    ]
  }
]

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
})

export default router

