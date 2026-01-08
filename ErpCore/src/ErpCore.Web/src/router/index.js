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
  }
]

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
})

export default router

