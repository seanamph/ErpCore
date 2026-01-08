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
    builder.Services.AddScoped<IUserAgentRepository, UserAgentRepository>();
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
    builder.Services.AddScoped<IEInvoiceRepository, EInvoiceRepository>();
    builder.Services.AddScoped<IProspectMasterRepository, ProspectMasterRepository>();
    builder.Services.AddScoped<IProspectRepository, ProspectRepository>();
    builder.Services.AddScoped<IInterviewRepository, InterviewRepository>();
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
    builder.Services.AddScoped<ISystemExtensionRepository, SystemExtensionRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.SystemExtension.ISystemExtensionReportRepository, ErpCore.Infrastructure.Repositories.SystemExtension.SystemExtensionReportRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.Kiosk.IKioskTransactionRepository, ErpCore.Infrastructure.Repositories.Kiosk.KioskTransactionRepository>();
    builder.Services.AddScoped<ICityRepository, CityRepository>();
    builder.Services.AddScoped<IZoneRepository, ZoneRepository>();
    builder.Services.AddScoped<IMenuRepository, MenuRepository>();
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
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.Procurement.ISupplierRepository, ErpCore.Infrastructure.Repositories.Procurement.SupplierRepository>();
    builder.Services.AddScoped<IContractRepository, ContractRepository>();
    builder.Services.AddScoped<ICashParamsRepository, CashParamsRepository>();
    builder.Services.AddScoped<IPcKeepRepository, PcKeepRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.ReportManagement.IArItemsRepository, ErpCore.Infrastructure.Repositories.ReportManagement.ArItemsRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.ReportManagement.IAccountsReceivableRepository, ErpCore.Infrastructure.Repositories.ReportManagement.AccountsReceivableRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.ReportManagement.IReceiptVoucherTransferRepository, ErpCore.Infrastructure.Repositories.ReportManagement.ReceiptVoucherTransferRepository>();
    builder.Services.AddScoped<ErpCore.Infrastructure.Repositories.ReportManagement.IDepositsRepository, ErpCore.Infrastructure.Repositories.ReportManagement.DepositsRepository>();
    builder.Services.AddScoped<IVoucherRepository, VoucherRepository>();
    builder.Services.AddScoped<IVoucherDetailRepository, VoucherDetailRepository>();
    builder.Services.AddScoped<IExtensionFunctionRepository, ExtensionFunctionRepository>();

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
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IUserAgentService, UserAgentService>();
    builder.Services.AddScoped<IRoleService, RoleService>();
    builder.Services.AddScoped<IUserRoleService, UserRoleService>();
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
    builder.Services.AddScoped<IPermissionCategoryReportService, PermissionCategoryReportService>();
    builder.Services.AddScoped<ISystemProgramButtonService, SystemProgramButtonService>();
    builder.Services.AddScoped<IButtonLogService, ButtonLogService>();
    builder.Services.AddScoped<IEInvoiceService, EInvoiceService>();
    builder.Services.AddScoped<IProspectMasterService, ProspectMasterService>();
    builder.Services.AddScoped<IProspectService, ProspectService>();
    builder.Services.AddScoped<IInterviewService, InterviewService>();
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
    builder.Services.AddScoped<ISystemExtensionService, SystemExtensionService>();
    builder.Services.AddScoped<ErpCore.Application.Services.SystemExtension.ISystemExtensionReportService, ErpCore.Application.Services.SystemExtension.SystemExtensionReportService>();
    builder.Services.AddScoped<ErpCore.Application.Services.Kiosk.IKioskReportService, ErpCore.Application.Services.Kiosk.KioskReportService>();
    builder.Services.AddScoped<ErpCore.Application.Services.Kiosk.IKioskProcessService, ErpCore.Application.Services.Kiosk.KioskProcessService>();
    builder.Services.AddScoped<ICityService, CityService>();
    builder.Services.AddScoped<IZoneService, ZoneService>();
    builder.Services.AddScoped<IDateService, DateService>();
    builder.Services.AddScoped<IMenuService, MenuService>();
    builder.Services.AddScoped<IMultiSelectListService, MultiSelectListService>();
    builder.Services.AddScoped<ISystemListService, SystemListService>();
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
    builder.Services.AddScoped<ITaxAccountingSubjectService, TaxAccountingSubjectService>();
    builder.Services.AddScoped<IVoucherTypeService, VoucherTypeService>();
    builder.Services.AddScoped<ICommonVoucherService, CommonVoucherService>();
    builder.Services.AddScoped<ICashFlowLargeTypeService, CashFlowLargeTypeService>();
    builder.Services.AddScoped<ICashFlowMediumTypeService, CashFlowMediumTypeService>();
    builder.Services.AddScoped<ICashFlowSubjectTypeService, CashFlowSubjectTypeService>();
    builder.Services.AddScoped<ICashFlowSubTotalService, CashFlowSubTotalService>();
    builder.Services.AddScoped<ErpCore.Application.Services.Procurement.ISupplierService, ErpCore.Application.Services.Procurement.SupplierService>();
    builder.Services.AddScoped<IContractService, ContractService>();
    builder.Services.AddScoped<ICashParamsService, CashParamsService>();
    builder.Services.AddScoped<IPcKeepService, PcKeepService>();
    builder.Services.AddScoped<IContractProcessRepository, ContractProcessRepository>();
    builder.Services.AddScoped<IContractProcessService, ContractProcessService>();
    builder.Services.AddScoped<ErpCore.Application.Services.ReportManagement.IArItemsService, ErpCore.Application.Services.ReportManagement.ArItemsService>();
    builder.Services.AddScoped<ErpCore.Application.Services.ReportManagement.IAccountsReceivableService, ErpCore.Application.Services.ReportManagement.AccountsReceivableService>();
    builder.Services.AddScoped<ErpCore.Application.Services.ReportManagement.IReceivingExtensionService, ErpCore.Application.Services.ReportManagement.ReceivingExtensionService>();
    builder.Services.AddScoped<ErpCore.Application.Services.ReportManagement.IReceivingOtherService, ErpCore.Application.Services.ReportManagement.ReceivingOtherService>();
    builder.Services.AddScoped<IVoucherService, VoucherService>();
    builder.Services.AddScoped<IExtensionFunctionService, ExtensionFunctionService>();

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

