//namespace SFA.DAS.RoATPService.Application.Api.StartupConfiguration;

//public class Startup
//{
//    private const string ServiceName = "SFA.DAS.RoATPService";
//    private const string Version = "1.0";
//    private readonly IHostingEnvironment _env;
//    private readonly ILogger<Startup> _logger;

//    private IConfiguration ApplicationConfiguration { get; }

//    public Startup(IHostingEnvironment env, IConfiguration config, ILogger<Startup> logger)
//    {
//        _env = env;
//        _logger = logger;
//        _logger.LogInformation("In startup constructor.  Before GetConfig");
//        Configuration = ConfigurationService
//            .GetConfig(config["EnvironmentName"], config["ConfigurationStorageConnectionString"], Version, ServiceName).Result;
//        ApplicationConfiguration = config;
//        _logger.LogInformation("In startup constructor.  After GetConfig");
//    }

//    public IWebConfiguration Configuration { get; }

//    public void ConfigureServices(IServiceCollection services)
//    {
//        try
//        {
//            services.AddAuthentication(Configuration);

//            services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });

//            services.Configure<RequestLocalizationOptions>(options =>
//            {
//                options.DefaultRequestCulture = new RequestCulture("en-GB");
//                options.SupportedCultures = new List<CultureInfo> { new CultureInfo("en-GB") };
//                options.SupportedUICultures = new List<CultureInfo> { new CultureInfo("en-GB") };
//                options.RequestCultureProviders.Clear();
//            });

//            var auditLogSettings = new RegisterAuditLogSettings();
//            ApplicationConfiguration.Bind("RegisterAuditLogSettings", auditLogSettings);
//            services.AddSingleton(auditLogSettings);

//            services.AddHealthChecks();
//            IMvcBuilder mvcBuilder;
//            if (_env.IsDevelopment())
//                mvcBuilder = services.AddMvc(opt => { opt.Filters.Add(new AllowAnonymousFilter()); });
//            else
//                mvcBuilder = services.AddMvc();

//            mvcBuilder
//                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix,
//                    opts => { opts.ResourcesPath = "Resources"; })
//                .AddDataAnnotationsLocalization()
//                .AddControllersAsServices()
//                .AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining<Startup>());

//            services.AddSwaggerGen(c =>
//            {
//                c.SwaggerDoc("v1", new Info { Title = "SFA.DAS.RoATPService.Application.Api", Version = "v1" });

//                if (_env.IsDevelopment())
//                {
//                    var basePath = AppContext.BaseDirectory;
//                    var xmlPath = Path.Combine(basePath, "SFA.DAS.RoATPService.Application.Api.xml");
//                    c.IncludeXmlComments(xmlPath);
//                }
//            });

//            ConfigureDependencyInjection(services);

//            if (_env.IsDevelopment())
//            {
//                // TestDataService.AddTestData(serviceProvider.GetService<AssessorDbContext>());
//            }
//        }
//        catch (Exception e)
//        {
//            _logger.LogError(e, "Error during Startup Configure Services");
//            throw;
//        }
//    }

//    private void ConfigureDependencyInjection(IServiceCollection services)
//    {
//        services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
//        services.AddTransient(x => Configuration);
//        services.AddTransient<IDownloadRegisterRepository, DownloadRegisterRepository>();
//        services.AddTransient<IFatDataExportRepository, FatDataExportRepository>();
//        services.AddTransient<ILookupDataRepository, LookupDataRepository>();
//        services.AddTransient<IOrganisationRepository, OrganisationRepository>();
//        services.AddTransient<IDuplicateCheckRepository, DuplicateCheckRepository>();
//        services.AddTransient<ICreateOrganisationRepository, CreateOrganisationRepository>();
//        services.AddTransient<IOrganisationSearchRepository, OrganisationSearchRepository>();
//        services.AddTransient<IUpdateOrganisationRepository, UpdateOrganisationRepository>();
//        services.AddTransient<IOrganisationCategoryValidator, OrganisationCategoryValidator>();
//        services.AddTransient<IDataTableHelper, DataTableHelper>();
//        services.AddTransient<ICacheHelper, CacheHelper>();
//        services.AddTransient<IProviderTypeValidator, ProviderTypeValidator>();
//        services.AddTransient<IOrganisationSearchValidator, OrganisationSearchValidator>();
//        services.AddTransient<IOrganisationValidator, OrganisationValidator>();
//        services.AddTransient<IOrganisationSearchValidator, OrganisationSearchValidator>();
//        services.AddTransient<IMapCreateOrganisationRequestToCommand, MapCreateOrganisationRequestToCommand>();
//        services.AddTransient<ITextSanitiser, TextSanitiser>();
//        services.AddHttpClient<IUkrlpApiClient, UkrlpApiClient>();
//        services.AddTransient<IAuditLogService, AuditLogService>();
//        services.AddTransient<IOrganisationStatusManager, OrganisationStatusManager>();
//        services.AddTransient<IUkrlpSoapSerializer, UkrlpSoapSerializer>();
//        services.AddTransient<IEventsRepository, EventsRepository>();

//        services.AddTransient<IDbConnectionHelper, DbConnectionHelper>();
//        services.AddTransient<IAzureClientCredentialHelper, AzureClientCredentialHelper>();
//        services.AddTransient<IFatDataExportService, FatDataExportService>();

//        services.AddMediatR(typeof(GetProviderTypesHandler).GetTypeInfo().Assembly);
//    }


//    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
//    {
//        try
//        {
//            MappingStartup.AddMappings();

//            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

//            app.UseSwagger()
//                .UseSwaggerUI(c =>
//                {
//                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SFA.DAS.RoATPService.Application.Api v1");
//                    c.RoutePrefix = string.Empty;
//                })
//                .UseAuthentication();

//            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

//            app.UseRequestLocalization();
//            app.UseHealthChecks("/health");

//            app.UseMvc();
//        }
//        catch (Exception e)
//        {
//            _logger.LogError(e, "Error during Startup Configure");
//            throw;
//        }

//    }
//}
