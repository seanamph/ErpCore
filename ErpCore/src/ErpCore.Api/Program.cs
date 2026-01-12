using NLog;
using NLog.Web;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.Transfer;
using ErpCore.Infrastructure.Repositories.Inventory;
using ErpCore.Infrastructure.Repositories.StockAdjustment;
using ErpCore.Infrastructure.Repositories.InventoryCheck;
using ErpCore.Infrastructure.Repositories.Purchase;
using ErpCore.Infrastructure.Repositories.BasicData;
using ErpCore.Infrastructure.Repositories.Customer;
using ErpCore.Infrastructure.Repositories.SystemConfig;
using ErpCore.Infrastructure.Repositories.System;
using ErpCore.Infrastructure.Repositories.EInvoice;
using ErpCore.Infrastructure.Repositories.Recruitment;
using ErpCore.Infrastructure.Repositories.BusinessReport;
using ErpCore.Infrastructure.Repositories.AnalysisReport;
using ErpCore.Application.Services.Transfer;
using ErpCore.Application.Services.StockAdjustment;
using ErpCore.Application.Services.InventoryCheck;
using ErpCore.Application.Services.Purchase;
using ErpCore.Application.Services.Inventory;
using ErpCore.Application.Services.BasicData;
using ErpCore.Application.Services.Customer;
using ErpCore.Application.Services.SystemConfig;
using ErpCore.Application.Services.System;
using ErpCore.Application.Services.EInvoice;
using ErpCore.Application.Services.Recruitment;
using ErpCore.Application.Services.BusinessReport;
using ErpCore.Application.Services.AnalysisReport;
using ErpCore.Application.Services.SystemExtension;
using ErpCore.Infrastructure.Repositories.SystemExtension;
using ErpCore.Application.Services.DropdownList;
using ErpCore.Infrastructure.Repositories.DropdownList;
using ErpCore.Infrastructure.Repositories.OtherModule;
using ErpCore.Application.Services.OtherModule;
using ErpCore.Infrastructure.Repositories.Accounting;
using ErpCore.Application.Services.Accounting;
using ErpCore.Infrastructure.Repositories.TaxAccounting;
using ErpCore.Application.Services.TaxAccounting;
using ErpCore.Infrastructure.Repositories.Contract;
using ErpCore.Application.Services.Contract;
using ErpCore.Infrastructure.Repositories.Query;
using ErpCore.Application.Services.Query;
using ErpCore.Infrastructure.Repositories.Certificate;
using ErpCore.Application.Services.Certificate;
using ErpCore.Infrastructure.Repositories.Extension;
using ErpCore.Application.Services.Extension;
using ErpCore.Infrastructure.Repositories.OtherManagement;
using ErpCore.Application.Services.OtherManagement;
using ErpCore.Infrastructure.Repositories.InvoiceSales;
using ErpCore.Application.Services.InvoiceSales;
using ErpCore.Infrastructure.Repositories.InvoiceSalesB2B;
using ErpCore.Application.Services.InvoiceSalesB2B;
using ErpCore.Infrastructure.Repositories.SystemExtensionE;
using ErpCore.Application.Services.SystemExtensionE;
using ErpCore.Infrastructure.Repositories.SystemExtensionH;
using ErpCore.Application.Services.SystemExtensionH;
using ErpCore.Infrastructure.Repositories.Loyalty;
using ErpCore.Application.Services.Loyalty;
using ErpCore.Infrastructure.Repositories.SalesReport;
using ErpCore.Application.Services.SalesReport;
using ErpCore.Infrastructure.Repositories.StandardModule;
using ErpCore.Application.Services.StandardModule;
using ErpCore.Infrastructure.Repositories.CustomerCustom;
using ErpCore.Application.Services.CustomerCustom;
using ErpCore.Infrastructure.Repositories.CustomerCustomJgjn;
using ErpCore.Application.Services.CustomerCustomJgjn;
using ErpCore.Infrastructure.Repositories.CommunicationModule;
using ErpCore.Application.Services.CommunicationModule;
using ErpCore.Infrastructure.Repositories.ChartTools;
using ErpCore.Application.Services.ChartTools;
using ErpCore.Infrastructure.Repositories.InvoiceExtension;
using ErpCore.Application.Services.InvoiceExtension;
using ErpCore.Shared.Logging;

var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

try
{
    logger.Info("應用程式啟動");

    var builder = WebApplication.CreateBuilder(args);

    // 設定 NLog
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();

    // 加入服務
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // 設定 CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    // 註冊資料庫連線工廠
    builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

    // 註冊 Repository
    builder.Services.AddScoped<ITransferReturnRepository, TransferReturnRepository>();
    builder.Services.AddScoped<ITransferReceiptRepository, TransferReceiptRepository>();
    builder.Services.AddScoped<ITransferShortageRepository, TransferShortageRepository>();
    builder.Services.AddScoped<IStockRepository, StockRepository>();
    builder.Services.AddScoped<ITransferOrderRepository, TransferOrderRepository>();
    builder.Services.AddScoped<IStockAdjustmentRepository, StockAdjustmentRepository>();
    builder.Services.AddScoped<IStocktakingPlanRepository, StocktakingPlanRepository>();
    builder.Services.AddScoped<IPurchaseReceiptRepository, PurchaseReceiptRepository>();
    builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
    builder.Services.AddScoped<ISupplierGoodsRepository, SupplierGoodsRepository>();
    builder.Services.AddScoped<IProductGoodsIdRepository, ProductGoodsIdRepository>();
    builder.Services.AddScoped<IPriceChangeRepository, PriceChangeRepository>();
    builder.Services.AddScoped<IPopPrintRepository, PopPrintRepository>();
    builder.Services.AddScoped<ITextFileProcessLogRepository, TextFileProcessLogRepository>();
    builder.Services.AddScoped<ITextFileProcessDetailRepository, TextFileProcessDetailRepository>();
    builder.Services.AddScoped<IParameterRepository, ParameterRepository>();
    builder.Services.AddScoped<IRegionRepository, RegionRepository>();
    builder.Services.AddScoped<IBankRepository, BankRepository>();
    builder.Services.AddScoped<IAreaRepository, AreaRepository>();
    builder.Services.AddScoped<IVendorRepository, VendorRepository>();
    builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
    builder.Services.AddScoped<IGroupRepository, GroupRepository>();
    builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
    builder.Services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
    builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
    builder.Services.AddScoped<IConfigSystemRepository, ConfigSystemRepository>();
    builder.Services.AddScoped<IConfigSubSystemRepository, ConfigSubSystemRepository>();
    builder.Services.AddScoped<IConfigProgramRepository, ConfigProgramRepository>();
    builder.Services.AddScoped<IConfigButtonRepository, ConfigButtonRepository>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IUserScheduleRepository, UserScheduleRepository>();
    builder.Services.AddScoped<IUserAgentRepository, UserAgentRepository>();
    builder.Services.AddScoped<IUserBusinessTypeRepository, UserBusinessTypeRepository>();
    builder.Services.AddScoped<IUserWarehouseAreaRepository, UserWarehouseAreaRepository>();
    builder.Services.AddScoped<IUserStoreRepository, UserStoreRepository>();
    builder.Services.AddScoped<IUserShopRepository, UserShopRepository>();
    builder.Services.AddScoped<IUserVendorRepository, UserVendorRepository>();
    builder.Services.AddScoped<IUserDepartmentRepository, UserDepartmentRepository>();
    builder.Services.AddScoped<IUserButtonRepository, UserButtonRepository>();
    builder.Services.AddScoped<IRoleRepository, RoleRepository>();
    builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
    builder.Services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
    builder.Services.AddScoped<IUserPermissionRepository, UserPermissionRepository>();
    builder.Services.AddScoped<IRoleFieldPermissionRepository, RoleFieldPermissionRepository>();
    builder.Services.AddScoped<IUserFieldPermissionRepository, UserFieldPermissionRepository>();
    builder.Services.AddScoped<IItemCorrespondRepository, ItemCorrespondRepository>();
    builder.Services.AddScoped<IItemPermissionRepository, ItemPermissionRepository>();
    builder.Services.AddScoped<IControllableFieldRepository, ControllableFieldRepository>();
    builder.Services.AddScoped<IChangeLogRepository, ChangeLogRepository>();
    builder.Services.AddScoped<IButtonLogRepository, ButtonLogRepository>();
    builder.Services.AddScoped<ILoginLogRepository, LoginLogRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.System.IButtonRepository, ErpCore.Infrastructure.Repositories.System.ButtonRepository>();
    builder.Services.AddScoped<IEInvoiceRepository, EInvoiceRepository>();
    builder.Services.AddScoped<IProspectMasterRepository, ProspectMasterRepository>();
    builder.Services.AddScoped<IProspectRepository, ProspectRepository>();
    builder.Services.AddScoped<IInterviewRepository, InterviewRepository>();
    builder.Services.AddScoped<ITenantLocationRepository, TenantLocationRepository>();
    builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
    builder.Services.AddScoped<IEmployeeMealCardRepository, EmployeeMealCardRepository>();
    builder.Services.AddScoped<IBusinessReportRepository, BusinessReportRepository>();
    builder.Services.AddScoped<IBusinessReportManagementRepository, BusinessReportManagementRepository>();
    builder.Services.AddScoped<IBusinessReportPrintRepository, BusinessReportPrintRepository>();
    builder.Services.AddScoped<IBusinessReportPrintDetailRepository, BusinessReportPrintDetailRepository>();
    builder.Services.AddScoped<IBusinessReportPrintLogRepository, BusinessReportPrintLogRepository>();
    builder.Services.AddScoped<IEmployeeMealCardFieldRepository, EmployeeMealCardFieldRepository>();
    builder.Services.AddScoped<IEmployeeMealCardReportRepository, EmployeeMealCardReportRepository>();
    builder.Services.AddScoped<IReturnCardRepository, ReturnCardRepository>();
    builder.Services.AddScoped<IReturnCardReportRepository, ReturnCardReportRepository>();
    builder.Services.AddScoped<IOvertimePaymentRepository, OvertimePaymentRepository>();
    builder.Services.AddScoped<IOvertimePaymentReportRepository, OvertimePaymentReportRepository>();
    builder.Services.AddScoped<IAnalysisReportRepository, AnalysisReportRepository>();
    builder.Services.AddScoped<IConsumablePrintRepository, ConsumablePrintRepository>();
    builder.Services.AddScoped<ISystemExtensionRepository, SystemExtensionRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.SystemExtension.ISystemExtensionReportRepository, ErpCore.Infrastructure.Repositories.SystemExtension.SystemExtensionReportRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.Kiosk.IKioskTransactionRepository, ErpCore.Infrastructure.Repositories.Kiosk.KioskTransactionRepository>();
    builder.Services.AddScoped<ICityRepository, CityRepository>();
    builder.Services.AddScoped<IZoneRepository, ZoneRepository>();
    builder.Services.AddScoped<IMenuRepository, MenuRepository>();
    builder.Services.AddScoped<IProgramRepository, ProgramRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.Tools.IFileUploadRepository, ErpCore.Infrastructure.Repositories.Tools.FileUploadRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.Tools.IPdfConversionLogRepository, ErpCore.Infrastructure.Repositories.Tools.PdfConversionLogRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.Communication.IEmailLogRepository, ErpCore.Infrastructure.Repositories.Communication.EmailLogRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.Communication.IEmailAttachmentRepository, ErpCore.Infrastructure.Repositories.Communication.EmailAttachmentRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.Communication.ISmsLogRepository, ErpCore.Infrastructure.Repositories.Communication.SmsLogRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.Communication.IEmailQueueRepository, ErpCore.Infrastructure.Repositories.Communication.EmailQueueRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.Communication.IEncodeLogRepository, ErpCore.Infrastructure.Repositories.Communication.EncodeLogRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.UiComponent.IUIComponentRepository, ErpCore.Infrastructure.Repositories.UiComponent.UIComponentRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.ReportExtension.IReportQueryRepository, ErpCore.Infrastructure.Repositories.ReportExtension.ReportQueryRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.ReportExtension.IReportPrintLogRepository, ErpCore.Infrastructure.Repositories.ReportExtension.ReportPrintLogRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.ReportExtension.IReportStatisticRepository, ErpCore.Infrastructure.Repositories.ReportExtension.ReportStatisticRepository>();
    builder.Services.AddScoped<ICrpReportRepository, CrpReportRepository>();
    builder.Services.AddScoped<IEipIntegrationRepository, EipIntegrationRepository>();
    builder.Services.AddScoped<ILabTestRepository, LabTestRepository>();
    builder.Services.AddScoped<IAccountSubjectRepository, AccountSubjectRepository>();
    builder.Services.AddScoped<ITaxAccountingSubjectRepository, TaxAccountingSubjectRepository>();
    builder.Services.AddScoped<IVoucherTypeRepository, VoucherTypeRepository>();
    builder.Services.AddScoped<ICommonVoucherRepository, CommonVoucherRepository>();
    builder.Services.AddScoped<ICashFlowLargeTypeRepository, CashFlowLargeTypeRepository>();
    builder.Services.AddScoped<ICashFlowMediumTypeRepository, CashFlowMediumTypeRepository>();
    builder.Services.AddScoped<ICashFlowSubjectTypeRepository, CashFlowSubjectTypeRepository>();
    builder.Services.AddScoped<ICashFlowSubTotalRepository, CashFlowSubTotalRepository>();
    builder.Services.AddScoped<IInvoiceDataRepository, InvoiceDataRepository>();
    builder.Services.AddScoped<ITransactionDataRepository, TransactionDataRepository>();
    builder.Services.AddScoped<ITaxReportRepository, TaxReportRepository>();
    builder.Services.AddScoped<ITaxReportPrintRepository, TaxReportPrintRepository>();
    builder.Services.AddScoped<IVoucherAuditRepository, VoucherAuditRepository>();
    builder.Services.AddScoped<IVoucherImportRepository, VoucherImportRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.Procurement.ISupplierRepository, ErpCore.Infrastructure.Repositories.Procurement.SupplierRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.Procurement.IPaymentRepository, ErpCore.Infrastructure.Repositories.Procurement.PaymentRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.Procurement.IBankAccountRepository, ErpCore.Infrastructure.Repositories.Procurement.BankAccountRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.Procurement.IProcurementOtherRepository, ErpCore.Infrastructure.Repositories.Procurement.ProcurementOtherRepository>();
    builder.Services.AddScoped<IContractRepository, ContractRepository>();
    builder.Services.AddScoped<ICashParamsRepository, CashParamsRepository>();
    builder.Services.AddScoped<IPcKeepRepository, PcKeepRepository>();
    builder.Services.AddScoped<IPcCashRepository, PcCashRepository>();
    builder.Services.AddScoped<IPcCashRequestRepository, PcCashRequestRepository>();
    builder.Services.AddScoped<IPcCashTransferRepository, PcCashTransferRepository>();
    builder.Services.AddScoped<IPcCashInventoryRepository, PcCashInventoryRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.Query.IVoucherAuditRepository, ErpCore.Infrastructure.Repositories.Query.VoucherAuditRepository>();
    builder.Services.AddScoped<IQueryFunctionRepository, QueryFunctionRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.ReportManagement.IArItemsRepository, ErpCore.Infrastructure.Repositories.ReportManagement.ArItemsRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.ReportManagement.IAccountsReceivableRepository, ErpCore.Infrastructure.Repositories.ReportManagement.AccountsReceivableRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.ReportManagement.IReceiptVoucherTransferRepository, ErpCore.Infrastructure.Repositories.ReportManagement.ReceiptVoucherTransferRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.ReportManagement.IDepositsRepository, ErpCore.Infrastructure.Repositories.ReportManagement.DepositsRepository>();
    builder.Services.AddScoped<IVoucherRepository, VoucherRepository>();
    builder.Services.AddScoped<IVoucherDetailRepository, VoucherDetailRepository>();
    builder.Services.AddScoped<IExtensionFunctionRepository, ExtensionFunctionRepository>();
    builder.Services.AddScoped<ISYSSFunctionRepository, SYSSFunctionRepository>();
    builder.Services.AddScoped<ISYSUFunctionRepository, SYSUFunctionRepository>();
    builder.Services.AddScoped<ISYSVFunctionRepository, SYSVFunctionRepository>();
    builder.Services.AddScoped<ISYSJFunctionRepository, SYSJFunctionRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.CustomerInvoice.ICustomerDataRepository, ErpCore.Infrastructure.Repositories.CustomerInvoice.CustomerDataRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.CustomerInvoice.IInvoicePrintRepository, ErpCore.Infrastructure.Repositories.CustomerInvoice.InvoicePrintRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.CustomerInvoice.IMailFaxRepository, ErpCore.Infrastructure.Repositories.CustomerInvoice.MailFaxRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.CustomerInvoice.ILedgerDataRepository, ErpCore.Infrastructure.Repositories.CustomerInvoice.LedgerDataRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.StoreMember.IStoreRepository, ErpCore.Infrastructure.Repositories.StoreMember.StoreRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.StoreMember.IMemberRepository, ErpCore.Infrastructure.Repositories.StoreMember.MemberRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.StoreMember.IPromotionRepository, ErpCore.Infrastructure.Repositories.StoreMember.PromotionRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.StoreFloor.IShopFloorRepository, ErpCore.Infrastructure.Repositories.StoreFloor.ShopFloorRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.StoreFloor.IFloorRepository, ErpCore.Infrastructure.Repositories.StoreFloor.FloorRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.StoreFloor.ITypeCodeRepository, ErpCore.Infrastructure.Repositories.StoreFloor.TypeCodeRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.StoreFloor.IPosTerminalRepository, ErpCore.Infrastructure.Repositories.StoreFloor.PosTerminalRepository>();
    builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
    builder.Services.AddScoped<IElectronicInvoiceRepository, ElectronicInvoiceRepository>();
    builder.Services.AddScoped<IElectronicInvoicePrintSettingRepository, ElectronicInvoicePrintSettingRepository>();
    builder.Services.AddScoped<ISalesOrderRepository, SalesOrderRepository>();
    builder.Services.AddScoped<ISalesOrderDetailRepository, SalesOrderDetailRepository>();
    builder.Services.AddScoped<ISalesOrderQueryRepository, SalesOrderQueryRepository>();
    builder.Services.AddScoped<ISalesReportQueryRepository, SalesReportQueryRepository>();
    builder.Services.AddScoped<IReportTemplateRepository, ReportTemplateRepository>();
    builder.Services.AddScoped<IReportPrintLogRepository, ReportPrintLogRepository>();
    // B2B InvoiceSales
    builder.Services.AddScoped<IB2BInvoiceRepository, B2BInvoiceRepository>();
    builder.Services.AddScoped<IB2BElectronicInvoiceRepository, B2BElectronicInvoiceRepository>();
    builder.Services.AddScoped<IB2BElectronicInvoicePrintSettingRepository, B2BElectronicInvoicePrintSettingRepository>();
    builder.Services.AddScoped<IB2BSalesOrderRepository, B2BSalesOrderRepository>();
    builder.Services.AddScoped<IB2BSalesOrderDetailRepository, B2BSalesOrderDetailRepository>();
    builder.Services.AddScoped<IB2BSalesOrderQueryRepository, B2BSalesOrderQueryRepository>();
    // SystemExtensionE
    builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
    builder.Services.AddScoped<IPersonnelRepository, PersonnelRepository>();
    // SystemExtensionH
    builder.Services.AddScoped<IPersonnelImportLogRepository, PersonnelImportLogRepository>();
    builder.Services.AddScoped<IPersonnelImportDetailRepository, PersonnelImportDetailRepository>();
    builder.Services.AddScoped<IEmpCardRepository, EmpCardRepository>();
    // Loyalty
    builder.Services.AddScoped<ILoyaltySystemConfigRepository, LoyaltySystemConfigRepository>();
    builder.Services.AddScoped<ILoyaltySystemInitLogRepository, LoyaltySystemInitLogRepository>();
    builder.Services.AddScoped<ILoyaltyPointTransactionRepository, LoyaltyPointTransactionRepository>();
    builder.Services.AddScoped<ILoyaltyMemberRepository, LoyaltyMemberRepository>();
    // CommunicationModule
    builder.Services.AddScoped<ISystemCommunicationRepository, SystemCommunicationRepository>();
    builder.Services.AddScoped<IXComSystemParamRepository, XComSystemParamRepository>();
    builder.Services.AddScoped<IErrorMessageRepository, ErrorMessageRepository>();
    // ChartTools
    builder.Services.AddScoped<IChartConfigRepository, ChartConfigRepository>();
    // InvoiceExtension
    builder.Services.AddScoped<IEInvoiceExtensionRepository, EInvoiceExtensionRepository>();

    // 註冊 Service
    builder.Services.AddScoped<ITransferReturnService, TransferReturnService>();
    builder.Services.AddScoped<ITransferReceiptService, TransferReceiptService>();
    builder.Services.AddScoped<ITransferShortageService, TransferShortageService>();
    builder.Services.AddScoped<IStockAdjustmentService, StockAdjustmentService>();
    builder.Services.AddScoped<IStocktakingPlanService, StocktakingPlanService>();
    builder.Services.AddScoped<IPurchaseReceiptService, PurchaseReceiptService>();
    builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
    builder.Services.AddScoped<IOnSitePurchaseOrderService, OnSitePurchaseOrderService>();
    builder.Services.AddScoped<ISupplierGoodsService, SupplierGoodsService>();
    builder.Services.AddScoped<IProductGoodsIdService, ProductGoodsIdService>();
    builder.Services.AddScoped<IPriceChangeService, PriceChangeService>();
    builder.Services.AddScoped<IPopPrintService, PopPrintService>();
    builder.Services.AddScoped<ITextFileService, TextFileService>();
    builder.Services.AddScoped<ErpCore.Application.Services.Inventory.IProductService, ErpCore.Application.Services.Inventory.ProductService>();
    builder.Services.AddScoped<ErpCore.Application.Services.BasicData.IShopService, ErpCore.Application.Services.BasicData.ShopService>();
    builder.Services.AddScoped<IParameterService, ParameterService>();
    builder.Services.AddScoped<IRegionService, RegionService>();
    builder.Services.AddScoped<IBankService, BankService>();
    builder.Services.AddScoped<IAreaService, AreaService>();
    builder.Services.AddScoped<IVendorService, VendorService>();
    builder.Services.AddScoped<IDepartmentService, DepartmentService>();
    builder.Services.AddScoped<IGroupService, GroupService>();
    builder.Services.AddScoped<IWarehouseService, WarehouseService>();
    builder.Services.AddScoped<IProductCategoryService, ProductCategoryService>();
    builder.Services.AddScoped<ICustomerService, CustomerService>();
    builder.Services.AddScoped<IConfigSystemService, ConfigSystemService>();
    builder.Services.AddScoped<IConfigSubSystemService, ConfigSubSystemService>();
    builder.Services.AddScoped<IConfigProgramService, ConfigProgramService>();
    builder.Services.AddScoped<IConfigButtonService, ConfigButtonService>();
    builder.Services.AddScoped<ErpCore.Application.Services.System.ISystemService, ErpCore.Application.Services.System.SystemService>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.System.ISystemRepository, ErpCore.Infrastructure.Repositories.System.SystemRepository>();
    builder.Services.AddScoped<ErpCore.Application.Services.System.IMenuService, ErpCore.Application.Services.System.MenuService>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.System.IMenuRepository, ErpCore.Infrastructure.Repositories.System.MenuRepository>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IUserScheduleService, UserScheduleService>();
    builder.Services.AddHostedService<UserScheduleBackgroundService>();
    builder.Services.AddScoped<IActiveDirectoryService, ActiveDirectoryService>();
    builder.Services.AddScoped<IUserAgentService, UserAgentService>();
    builder.Services.AddScoped<IRoleService, RoleService>();
    builder.Services.AddScoped<IUserRoleService, UserRoleService>();
    builder.Services.AddScoped<IRoleUserAssignmentService, RoleUserAssignmentService>();
    builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();
    builder.Services.AddScoped<IUserPermissionService, UserPermissionService>();
    builder.Services.AddScoped<IRoleFieldPermissionService, RoleFieldPermissionService>();
    builder.Services.AddScoped<IUserFieldPermissionService, UserFieldPermissionService>();
    builder.Services.AddScoped<IItemCorrespondService, ItemCorrespondService>();
    builder.Services.AddScoped<IItemPermissionService, ItemPermissionService>();
    builder.Services.AddScoped<IControllableFieldService, ControllableFieldService>();
    builder.Services.AddScoped<IChangeLogService, ChangeLogService>();
    builder.Services.AddScoped<ISystemPermissionService, SystemPermissionService>();
    builder.Services.AddScoped<IProgramUserPermissionService, ProgramUserPermissionService>();
    builder.Services.AddScoped<IRoleSystemPermissionService, RoleSystemPermissionService>();
    builder.Services.AddScoped<IProgramRolePermissionService, ProgramRolePermissionService>();
    builder.Services.AddScoped<IRoleUserService, RoleUserService>();
    builder.Services.AddScoped<IRoleUserService, RoleUserService>();
    builder.Services.AddScoped<IPermissionCategoryReportService, PermissionCategoryReportService>();
    builder.Services.AddScoped<ISystemProgramButtonService, SystemProgramButtonService>();
    builder.Services.AddScoped<IButtonLogService, ButtonLogService>();
    builder.Services.AddScoped<ILoginLogService, LoginLogService>();
    builder.Services.AddScoped<ErpCore.Application.Services.System.IButtonService, ErpCore.Application.Services.System.ButtonService>();
    builder.Services.AddScoped<IEInvoiceService, EInvoiceService>();
    builder.Services.AddScoped<IProspectMasterService, ProspectMasterService>();
    builder.Services.AddScoped<IProspectService, ProspectService>();
    builder.Services.AddScoped<IInterviewService, InterviewService>();
    builder.Services.AddScoped<IBusinessOtherService, BusinessOtherService>();
    builder.Services.AddScoped<IEmployeeMealCardService, EmployeeMealCardService>();
    builder.Services.AddScoped<IBusinessReportService, BusinessReportService>();
    builder.Services.AddScoped<IBusinessReportManagementService, BusinessReportManagementService>();
    builder.Services.AddScoped<IBusinessReportPrintService, BusinessReportPrintService>();
    builder.Services.AddScoped<IBusinessReportPrintDetailService, BusinessReportPrintDetailService>();
    builder.Services.AddScoped<IBusinessReportPrintLogService, BusinessReportPrintLogService>();
    builder.Services.AddScoped<IEmployeeMealCardFieldService, EmployeeMealCardFieldService>();
    builder.Services.AddScoped<IEmployeeMealCardReportService, EmployeeMealCardReportService>();
    builder.Services.AddScoped<IReturnCardService, ReturnCardService>();
    builder.Services.AddScoped<IReturnCardReportService, ReturnCardReportService>();
    builder.Services.AddScoped<IOvertimePaymentService, OvertimePaymentService>();
    builder.Services.AddScoped<IOvertimePaymentReportService, OvertimePaymentReportService>();
    builder.Services.AddScoped<IAnalysisReportService, AnalysisReportService>();
    builder.Services.AddScoped<IConsumablePrintService, ConsumablePrintService>();
    builder.Services.AddScoped<ISystemExtensionService, SystemExtensionService>();
    builder.Services.AddScoped<ErpCore.Application.Services.SystemExtension.ISystemExtensionReportService, ErpCore.Application.Services.SystemExtension.SystemExtensionReportService>();
    builder.Services.AddScoped<ErpCore.Application.Services.Kiosk.IKioskReportService, ErpCore.Application.Services.Kiosk.KioskReportService>();
    builder.Services.AddScoped<ErpCore.Application.Services.Kiosk.IKioskProcessService, ErpCore.Application.Services.Kiosk.KioskProcessService>();
    builder.Services.AddScoped<ICityService, CityService>();
    builder.Services.AddScoped<IZoneService, ZoneService>();
    builder.Services.AddScoped<IDateService, DateService>();
    builder.Services.AddScoped<IMenuService, MenuService>();
    builder.Services.AddScoped<IProgramService, ProgramService>();
    builder.Services.AddScoped<IMultiSelectListService, MultiSelectListService>();
    builder.Services.AddScoped<ISystemListService, SystemListService>();
    builder.Services.AddScoped<IUserListService, UserListService>();
    builder.Services.AddScoped<ErpCore.Application.Services.Tools.IFileUploadService, ErpCore.Application.Services.Tools.FileUploadService>();
    builder.Services.AddScoped<ErpCore.Application.Services.Tools.IBarcodeService, ErpCore.Application.Services.Tools.BarcodeService>();
    builder.Services.AddScoped<ErpCore.Application.Services.Tools.IHtml2PdfService, ErpCore.Application.Services.Tools.Html2PdfService>();
    builder.Services.AddScoped<ErpCore.Application.Services.Communication.IEmailService, ErpCore.Application.Services.Communication.EmailService>();
    builder.Services.AddScoped<ErpCore.Application.Services.Communication.ISmsService, ErpCore.Application.Services.Communication.SmsService>();
    builder.Services.AddScoped<ErpCore.Application.Services.Communication.IEmailQueueService, ErpCore.Application.Services.Communication.EmailQueueService>();
    builder.Services.AddScoped<ErpCore.Application.Services.Communication.IEncodeService, ErpCore.Application.Services.Communication.EncodeService>();
    builder.Services.AddScoped<ErpCore.Application.Services.UiComponent.IDataMaintenanceComponentService, ErpCore.Application.Services.UiComponent.DataMaintenanceComponentService>();
    builder.Services.AddScoped<ErpCore.Application.Services.UiComponent.IUiComponentQueryService, ErpCore.Application.Services.UiComponent.UiComponentQueryService>();
    builder.Services.AddScoped<ErpCore.Application.Services.ReportExtension.IReportModule7Service, ErpCore.Application.Services.ReportExtension.ReportModule7Service>();
    builder.Services.AddScoped<ErpCore.Application.Services.ReportExtension.IReportPrintService, ErpCore.Application.Services.ReportExtension.ReportPrintService>();
    builder.Services.AddScoped<ErpCore.Application.Services.ReportExtension.IReportStatisticsService, ErpCore.Application.Services.ReportExtension.ReportStatisticsService>();
    builder.Services.AddScoped<ErpCore.Application.Services.Core.IUserManagementService, ErpCore.Application.Services.Core.UserManagementService>();
    builder.Services.AddScoped<ErpCore.Application.Services.Core.IFrameworkService, ErpCore.Application.Services.Core.FrameworkService>();
    builder.Services.AddScoped<ICrpReportService, CrpReportService>();
    builder.Services.AddScoped<IEipIntegrationService, EipIntegrationService>();
    builder.Services.AddScoped<ILabTestService, LabTestService>();
    builder.Services.AddScoped<IEmployeeService, EmployeeService>();
    builder.Services.AddScoped<IAccountSubjectService, AccountSubjectService>();
    builder.Services.AddScoped<IFinancialReportService, FinancialReportService>();
    builder.Services.AddScoped<ITaxAccountingSubjectService, TaxAccountingSubjectService>();
    builder.Services.AddScoped<IVoucherTypeService, VoucherTypeService>();
    builder.Services.AddScoped<ICommonVoucherService, CommonVoucherService>();
    builder.Services.AddScoped<ICashFlowLargeTypeService, CashFlowLargeTypeService>();
    builder.Services.AddScoped<ICashFlowMediumTypeService, CashFlowMediumTypeService>();
    builder.Services.AddScoped<ICashFlowSubjectTypeService, CashFlowSubjectTypeService>();
    builder.Services.AddScoped<ICashFlowSubTotalService, CashFlowSubTotalService>();
    builder.Services.AddScoped<IInvoiceDataService, InvoiceDataService>();
    builder.Services.AddScoped<ITransactionDataService, TransactionDataService>();
    builder.Services.AddScoped<ITaxReportService, TaxReportService>();
    builder.Services.AddScoped<ITaxReportPrintService, TaxReportPrintService>();
    builder.Services.AddScoped<IVoucherAuditService, VoucherAuditService>();
    builder.Services.AddScoped<IVoucherImportService, VoucherImportService>();
    builder.Services.AddScoped<ErpCore.Application.Services.Procurement.ISupplierService, ErpCore.Application.Services.Procurement.SupplierService>();
    builder.Services.AddScoped<ErpCore.Application.Services.Procurement.IPaymentService, ErpCore.Application.Services.Procurement.PaymentService>();
    builder.Services.AddScoped<ErpCore.Application.Services.Procurement.IBankAccountService, ErpCore.Application.Services.Procurement.BankAccountService>();
    builder.Services.AddScoped<ErpCore.Application.Services.Procurement.IProcurementReportService, ErpCore.Application.Services.Procurement.ProcurementReportService>();
    builder.Services.AddScoped<ErpCore.Application.Services.Procurement.IProcurementOtherService, ErpCore.Application.Services.Procurement.ProcurementOtherService>();
    builder.Services.AddScoped<IContractService, ContractService>();
    builder.Services.AddScoped<ICashParamsService, CashParamsService>();
    builder.Services.AddScoped<IPcKeepService, PcKeepService>();
    builder.Services.AddScoped<IPcCashService, PcCashService>();
    builder.Services.AddScoped<IPcCashRequestService, PcCashRequestService>();
    builder.Services.AddScoped<IPcCashTransferService, PcCashTransferService>();
    builder.Services.AddScoped<IPcCashInventoryService, PcCashInventoryService>();
    builder.Services.AddScoped<ErpCore.Application.Services.Query.IVoucherAuditService, ErpCore.Application.Services.Query.VoucherAuditService>();
    builder.Services.AddScoped<IQueryFunctionService, QueryFunctionService>();
    builder.Services.AddScoped<IContractProcessRepository, ContractProcessRepository>();
    builder.Services.AddScoped<IContractProcessService, ContractProcessService>();
    builder.Services.AddScoped<ErpCore.Application.Services.ReportManagement.IArItemsService, ErpCore.Application.Services.ReportManagement.ArItemsService>();
    builder.Services.AddScoped<ErpCore.Application.Services.ReportManagement.IAccountsReceivableService, ErpCore.Application.Services.ReportManagement.AccountsReceivableService>();
    builder.Services.AddScoped<ErpCore.Application.Services.ReportManagement.IReceivingExtensionService, ErpCore.Application.Services.ReportManagement.ReceivingExtensionService>();
    builder.Services.AddScoped<ErpCore.Application.Services.ReportManagement.IReceivingOtherService, ErpCore.Application.Services.ReportManagement.ReceivingOtherService>();
    builder.Services.AddScoped<IVoucherService, VoucherService>();
    builder.Services.AddScoped<IExtensionFunctionService, ExtensionFunctionService>();
    builder.Services.AddScoped<ISYSSFunctionService, SYSSFunctionService>();
    builder.Services.AddScoped<ISYSUFunctionService, SYSUFunctionService>();
    builder.Services.AddScoped<ISYSVFunctionService, SYSVFunctionService>();
    builder.Services.AddScoped<ISYSJFunctionService, SYSJFunctionService>();
    builder.Services.AddScoped<ErpCore.Application.Services.CustomerInvoice.ICustomerDataService, ErpCore.Application.Services.CustomerInvoice.CustomerDataService>();
    builder.Services.AddScoped<ErpCore.Application.Services.CustomerInvoice.IInvoicePrintService, ErpCore.Application.Services.CustomerInvoice.InvoicePrintService>();
    builder.Services.AddScoped<ErpCore.Application.Services.CustomerInvoice.IMailFaxService, ErpCore.Application.Services.CustomerInvoice.MailFaxService>();
    builder.Services.AddScoped<ErpCore.Application.Services.CustomerInvoice.ILedgerDataService, ErpCore.Application.Services.CustomerInvoice.LedgerDataService>();
    builder.Services.AddScoped<ErpCore.Application.Services.StoreMember.IStoreService, ErpCore.Application.Services.StoreMember.StoreService>();
    builder.Services.AddScoped<ErpCore.Application.Services.StoreMember.IMemberService, ErpCore.Application.Services.StoreMember.MemberService>();
    builder.Services.AddScoped<ErpCore.Application.Services.StoreMember.IStoreQueryService, ErpCore.Application.Services.StoreMember.StoreQueryService>();
    builder.Services.AddScoped<ErpCore.Application.Services.StoreMember.IMemberQueryService, ErpCore.Application.Services.StoreMember.MemberQueryService>();
    builder.Services.AddScoped<ErpCore.Application.Services.StoreMember.IPromotionService, ErpCore.Application.Services.StoreMember.PromotionService>();
    builder.Services.AddScoped<ErpCore.Application.Services.StoreMember.IStoreReportService, ErpCore.Application.Services.StoreMember.StoreReportService>();
    builder.Services.AddScoped<ErpCore.Application.Services.StoreFloor.IShopFloorService, ErpCore.Application.Services.StoreFloor.ShopFloorService>();
    builder.Services.AddScoped<ErpCore.Application.Services.StoreFloor.IShopFloorQueryService, ErpCore.Application.Services.StoreFloor.ShopFloorQueryService>();
    builder.Services.AddScoped<ErpCore.Application.Services.StoreFloor.IFloorService, ErpCore.Application.Services.StoreFloor.FloorService>();
    builder.Services.AddScoped<ErpCore.Application.Services.StoreFloor.IFloorQueryService, ErpCore.Application.Services.StoreFloor.FloorQueryService>();
    builder.Services.AddScoped<ErpCore.Application.Services.StoreFloor.ITypeCodeService, ErpCore.Application.Services.StoreFloor.TypeCodeService>();
    builder.Services.AddScoped<ErpCore.Application.Services.StoreFloor.ITypeCodeQueryService, ErpCore.Application.Services.StoreFloor.TypeCodeQueryService>();
    builder.Services.AddScoped<ErpCore.Application.Services.StoreFloor.IPosTerminalService, ErpCore.Application.Services.StoreFloor.PosTerminalService>();
    builder.Services.AddScoped<ErpCore.Application.Services.StoreFloor.IPosTerminalQueryService, ErpCore.Application.Services.StoreFloor.PosTerminalQueryService>();
    builder.Services.AddScoped<IInvoiceService, InvoiceService>();
    builder.Services.AddScoped<IElectronicInvoiceService, ElectronicInvoiceService>();
    builder.Services.AddScoped<IElectronicInvoicePrintSettingService, ElectronicInvoicePrintSettingService>();
    builder.Services.AddScoped<ISalesOrderService, SalesOrderService>();
    builder.Services.AddScoped<ISalesOrderQueryService, SalesOrderQueryService>();
    builder.Services.AddScoped<ISalesReportQueryService, SalesReportQueryService>();
    builder.Services.AddScoped<IReportPrintService, ReportPrintService>();
    // B2B InvoiceSales Services
    builder.Services.AddScoped<IB2BInvoiceService, B2BInvoiceService>();
    builder.Services.AddScoped<IB2BElectronicInvoiceService, B2BElectronicInvoiceService>();
    builder.Services.AddScoped<IB2BElectronicInvoicePrintSettingService, B2BElectronicInvoicePrintSettingService>();
    builder.Services.AddScoped<IB2BSalesOrderService, B2BSalesOrderService>();
    builder.Services.AddScoped<IB2BSalesOrderQueryService, B2BSalesOrderQueryService>();
    // SystemExtensionH Services
    builder.Services.AddScoped<IPersonnelBatchService, PersonnelBatchService>();
    builder.Services.AddScoped<ISystemExtensionPHService, SystemExtensionPHService>();
    // Loyalty Services
    builder.Services.AddScoped<ILoyaltyInitService, LoyaltyInitService>();
    builder.Services.AddScoped<ILoyaltyMaintenanceService, LoyaltyMaintenanceService>();
    // CommunicationModule Services
    builder.Services.AddScoped<ISystemCommunicationService, SystemCommunicationService>();
    builder.Services.AddScoped<IXComSystemParamService, XComSystemParamService>();
    builder.Services.AddScoped<IErrorMessageService, ErrorMessageService>();
    // ChartTools Services
    builder.Services.AddScoped<IChartConfigService, ChartConfigService>();
    // InvoiceExtension Services
    builder.Services.AddScoped<IEInvoiceExtensionService, EInvoiceExtensionService>();
    
    // Energy Repositories
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.Energy.IEnergyBaseRepository, ErpCore.Infrastructure.Repositories.Energy.EnergyBaseRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.Energy.IEnergyProcessRepository, ErpCore.Infrastructure.Repositories.Energy.EnergyProcessRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.Energy.IEnergyExtensionRepository, ErpCore.Infrastructure.Repositories.Energy.EnergyExtensionRepository>();
    
    // SalesReport Repositories
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.SalesReport.ISalesReportRepository, ErpCore.Infrastructure.Repositories.SalesReport.SalesReportRepository>();
    
    // Energy Services
    builder.Services.AddScoped<ErpCore.Application.Services.Energy.IEnergyBaseService, ErpCore.Application.Services.Energy.EnergyBaseService>();
    builder.Services.AddScoped<ErpCore.Application.Services.Energy.IEnergyProcessService, ErpCore.Application.Services.Energy.EnergyProcessService>();
    builder.Services.AddScoped<ErpCore.Application.Services.Energy.IEnergyExtensionService, ErpCore.Application.Services.Energy.EnergyExtensionService>();
    
    // SalesReport Services
    builder.Services.AddScoped<ErpCore.Application.Services.SalesReport.ISalesReportService, ErpCore.Application.Services.SalesReport.SalesReportService>();
    
    // StandardModule Repositories
    builder.Services.AddScoped<IStd3000DataRepository, Std3000DataRepository>();
    builder.Services.AddScoped<IStd5000BaseDataRepository, Std5000BaseDataRepository>();
    builder.Services.AddScoped<IStd5000MemberRepository, Std5000MemberRepository>();
    builder.Services.AddScoped<IStd5000MemberPointRepository, Std5000MemberPointRepository>();
    builder.Services.AddScoped<IStd5000TransactionRepository, Std5000TransactionRepository>();
    builder.Services.AddScoped<IStd5000TransactionDetailRepository, Std5000TransactionDetailRepository>();
    
    // StandardModule Services
    builder.Services.AddScoped<IStd3000DataService, Std3000DataService>();
    builder.Services.AddScoped<IStd5000BaseDataService, Std5000BaseDataService>();
    builder.Services.AddScoped<IStd5000MemberService, Std5000MemberService>();
    builder.Services.AddScoped<IStd5000TransactionService, Std5000TransactionService>();
    
    // Lease Repositories
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.Lease.ILeaseRepository, ErpCore.Infrastructure.Repositories.Lease.LeaseRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.Lease.ILeaseExtensionRepository, ErpCore.Infrastructure.Repositories.Lease.LeaseExtensionRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.Lease.ILeaseProcessRepository, ErpCore.Infrastructure.Repositories.Lease.LeaseProcessRepository>();
    
    // Lease Services
    builder.Services.AddScoped<ErpCore.Application.Services.Lease.ILeaseService, ErpCore.Application.Services.Lease.LeaseService>();
    builder.Services.AddScoped<ErpCore.Application.Services.Lease.ILeaseExtensionService, ErpCore.Application.Services.Lease.LeaseExtensionService>();
    builder.Services.AddScoped<ErpCore.Application.Services.Lease.ILeaseProcessService, ErpCore.Application.Services.Lease.LeaseProcessService>();

    // CustomerCustom Repositories
    builder.Services.AddScoped<ICus3000MemberRepository, Cus3000MemberRepository>();
    builder.Services.AddScoped<ICus3000PromotionRepository, Cus3000PromotionRepository>();
    builder.Services.AddScoped<ICus3000ActivityRepository, Cus3000ActivityRepository>();
    // Cus3000Eskyland Repositories
    builder.Services.AddScoped<ICus3000EskylandMemberRepository, Cus3000EskylandMemberRepository>();

    // CustomerCustom Services
    builder.Services.AddScoped<ICus3000MemberService, Cus3000MemberService>();
    builder.Services.AddScoped<ICus3000PromotionService, Cus3000PromotionService>();
    builder.Services.AddScoped<ICus3000ActivityService, Cus3000ActivityService>();
    // Cus3000Eskyland Services
    builder.Services.AddScoped<ICus3000EskylandMemberService, Cus3000EskylandMemberService>();

    // CustomerCustomJgjn Repositories
    builder.Services.AddScoped<IJgjNDataRepository, JgjNDataRepository>();
    builder.Services.AddScoped<IJgjNCustomerRepository, JgjNCustomerRepository>();
    builder.Services.AddScoped<IJgjNInvoiceRepository, JgjNInvoiceRepository>();

    // CustomerCustomJgjn Services
    builder.Services.AddScoped<IJgjNDataService, JgjNDataService>();
    builder.Services.AddScoped<IJgjNCustomerService, JgjNCustomerService>();
    builder.Services.AddScoped<IJgjNInvoiceService, JgjNInvoiceService>();

    // MirModule Repositories
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.MirModule.IMirH000PersonnelRepository, ErpCore.Infrastructure.Repositories.MirModule.MirH000PersonnelRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.MirModule.IMirH000SalaryRepository, ErpCore.Infrastructure.Repositories.MirModule.MirH000SalaryRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.MirModule.IMirV000DataRepository, ErpCore.Infrastructure.Repositories.MirModule.MirV000DataRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.MirModule.IMirW000DataRepository, ErpCore.Infrastructure.Repositories.MirModule.MirW000DataRepository>();

    // MirModule Services
    builder.Services.AddScoped<ErpCore.Application.Services.MirModule.IMirH000PersonnelService, ErpCore.Application.Services.MirModule.MirH000PersonnelService>();
    builder.Services.AddScoped<ErpCore.Application.Services.MirModule.IMirH000SalaryService, ErpCore.Application.Services.MirModule.MirH000SalaryService>();
    builder.Services.AddScoped<ErpCore.Application.Services.MirModule.IMirV000DataService, ErpCore.Application.Services.MirModule.MirV000DataService>();
    builder.Services.AddScoped<ErpCore.Application.Services.MirModule.IMirW000DataService, ErpCore.Application.Services.MirModule.MirW000DataService>();

    // MshModule Repositories
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.MshModule.IMsh3000DataRepository, ErpCore.Infrastructure.Repositories.MshModule.Msh3000DataRepository>();

    // MshModule Services
    builder.Services.AddScoped<ErpCore.Application.Services.MshModule.IMsh3000DataService, ErpCore.Application.Services.MshModule.Msh3000DataService>();

    // SapIntegration Repositories
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.SapIntegration.ITransSapRepository, ErpCore.Infrastructure.Repositories.SapIntegration.TransSapRepository>();

    // SapIntegration Services
    builder.Services.AddScoped<ErpCore.Application.Services.SapIntegration.ITransSapService, ErpCore.Application.Services.SapIntegration.TransSapService>();

    // UniversalModule Repositories
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.UniversalModule.IUniv000Repository, ErpCore.Infrastructure.Repositories.UniversalModule.Univ000Repository>();

    // UniversalModule Services
    builder.Services.AddScoped<ErpCore.Application.Services.UniversalModule.IUniv000Service, ErpCore.Application.Services.UniversalModule.Univ000Service>();

    // 註冊日誌服務
    builder.Services.AddSingleton<ILoggerService>(provider => new LoggerService("Default"));

    // 註冊 HTTP 上下文存取器
    builder.Services.AddHttpContextAccessor();

    // 註冊使用者上下文服務
    builder.Services.AddScoped<ErpCore.Shared.Common.IUserContext, ErpCore.Shared.Common.UserContext>();

    // 註冊匯出工具服務
    builder.Services.AddScoped<ErpCore.Shared.Common.ExportHelper>();

    // 註冊檔案儲存服務
    builder.Services.AddScoped<ErpCore.Infrastructure.Services.FileStorage.IFileStorageService, ErpCore.Infrastructure.Services.FileStorage.LocalFileStorageService>();

    // 註冊通知服務
    builder.Services.AddScoped<ErpCore.Infrastructure.Services.Notification.INotificationService, ErpCore.Infrastructure.Services.Notification.SmsNotificationService>();

    // 註冊快取服務
    builder.Services.AddMemoryCache();
    builder.Services.AddScoped<ErpCore.Infrastructure.Caching.ICacheService, ErpCore.Infrastructure.Caching.MemoryCacheService>();
    // 如需使用 Redis 或 DistributedCache，可取消註解以下行並安裝對應套件
    // builder.Services.AddStackExchangeRedisCache(options => { options.Configuration = "localhost:6379"; });
    // builder.Services.AddScoped<ErpCore.Infrastructure.Caching.ICacheService, ErpCore.Infrastructure.Caching.RedisCacheService>();
    // builder.Services.AddScoped<ErpCore.Infrastructure.Caching.ICacheService, ErpCore.Infrastructure.Caching.DistributedCacheService>();

    // 註冊身份驗證服務
    builder.Services.AddScoped<ErpCore.Infrastructure.Authentication.JwtTokenService>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Authentication.PasswordHasher>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Authentication.TokenValidator>();

    // 註冊授權服務
    builder.Services.AddScoped<ErpCore.Infrastructure.Authorization.PermissionChecker>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Authorization.RoleBasedAuthorizationHandler>();

    // 註冊背景工作服務
    builder.Services.AddScoped<ErpCore.Infrastructure.BackgroundJobs.IBackgroundJobService, ErpCore.Infrastructure.BackgroundJobs.HangfireBackgroundJobService>();
    // 如需使用 Quartz，可取消註解以下行並安裝對應套件
    // builder.Services.AddScoped<ErpCore.Infrastructure.BackgroundJobs.IBackgroundJobService, ErpCore.Infrastructure.BackgroundJobs.QuartzBackgroundJobService>();

    // 註冊訊息佇列服務
    builder.Services.AddScoped<ErpCore.Infrastructure.Messaging.IMessageQueueService, ErpCore.Infrastructure.Messaging.RabbitMqService>();
    // 如需使用 Azure Service Bus，可取消註解以下行並安裝對應套件
    // builder.Services.AddScoped<ErpCore.Infrastructure.Messaging.IMessageQueueService, ErpCore.Infrastructure.Messaging.AzureServiceBusService>();

    var app = builder.Build();

    // 設定 HTTP 請求管道
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseCors("AllowAll");
    app.UseAuthorization();
    app.MapControllers();

    logger.Info("應用程式啟動完成");
    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "應用程式啟動失敗");
    throw;
}
finally
{
    LogManager.Shutdown();
}

