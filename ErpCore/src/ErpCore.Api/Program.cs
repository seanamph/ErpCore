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
    builder.Services.AddScoped<IEmployeeMealCardRepository, EmployeeMealCardRepository>();
    builder.Services.AddScoped<IBusinessReportRepository, BusinessReportRepository>();
    builder.Services.AddScoped<IBusinessReportManagementRepository, BusinessReportManagementRepository>();

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

    // 註冊日誌服務
    builder.Services.AddSingleton<ILoggerService>(provider => new LoggerService("Default"));

    // 註冊 HTTP 上下文存取器
    builder.Services.AddHttpContextAccessor();

    // 註冊使用者上下文服務
    builder.Services.AddScoped<ErpCore.Shared.Common.IUserContext, ErpCore.Shared.Common.UserContext>();

    // 註冊匯出工具服務
    builder.Services.AddScoped<ErpCore.Shared.Common.ExportHelper>();

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

