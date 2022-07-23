﻿using ASC.Api.Core.Extensions;

using Microsoft.Extensions.Hosting.WindowsServices;

var options = new WebApplicationOptions
{
    Args = args,
    ContentRootPath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : default
};

var builder = WebApplication.CreateBuilder(options);

builder.Host.UseWindowsService();
builder.Host.UseSystemd();
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.WebHost.ConfigureKestrel((hostingContext, serverOptions) =>
{
    var kestrelConfig = hostingContext.Configuration.GetSection("Kestrel");

    if (!kestrelConfig.Exists()) return;

    var unixSocket = kestrelConfig.GetValue<string>("ListenUnixSocket");

    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
    {
        if (!string.IsNullOrWhiteSpace(unixSocket))
        {
            unixSocket = string.Format(unixSocket, hostingContext.HostingEnvironment.ApplicationName.Replace("ASC.", "").Replace(".", ""));

            serverOptions.ListenUnixSocket(unixSocket);
        }
    }
});

builder.Host.ConfigureAppConfiguration((hostContext, config) =>
{
    var builded = config.Build();
    var path = builded["pathToConf"];
    if (!Path.IsPathRooted(path))
    {
        path = Path.GetFullPath(CrossPlatform.PathCombine(hostContext.HostingEnvironment.ContentRootPath, path));
    }

    config.SetBasePath(path);
    var env = hostContext.Configuration.GetValue("ENVIRONMENT", "Production");

    config
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{env}.json", true)
        .AddJsonFile("storage.json")
        .AddJsonFile($"storage.{env}.json", true)
        .AddJsonFile("mail.json")
        .AddJsonFile($"mail.{env}.json", true)
        .AddJsonFile("elastic.json")
        .AddJsonFile($"elastic.{env}.json", true)
        .AddEnvironmentVariables()
        .AddCommandLine(args)
        .AddInMemoryCollection(new Dictionary<string, string>
            {
                {"pathToConf", path }
            }
        );
});

builder.Host.ConfigureServices((hostContext, services) =>
{
    services.AddHttpContextAccessor();
    services.AddMemoryCache();
    services.AddHttpClient();
    var diHelper = new DIHelper(services);
    diHelper.TryAdd<FactoryIndexerMailMail>();
    diHelper.TryAdd<FactoryIndexerMailContact>();
    diHelper.TryAdd(typeof(ICacheNotify<>), typeof(KafkaCacheNotify<>));
    services.AddSingleton(new ConsoleParser(args));
    diHelper.TryAdd<AggregatorServiceLauncher>();
    diHelper.TryAdd<AggregatorServiceScope>();
    services.AddDistributedTaskQueue();
    services.AddAutoMapper(Assembly.GetAssembly(typeof(MappingProfile)));
    services.AddHostedService<AggregatorServiceLauncher>();
    services.Configure<HostOptions>(opts => opts.ShutdownTimeout = TimeSpan.FromSeconds(15));

    var serviceProvider = services.BuildServiceProvider();
    var logger = serviceProvider.GetService<ILogger<CrmLinkEngine>>();
    services.AddSingleton(typeof(ILogger), logger);
});

builder.Host.ConfigureNLogLogging();

var startup = new BaseWorkerStartup(builder.Configuration);

startup.ConfigureServices(builder.Services);

builder.Host.ConfigureContainer<ContainerBuilder>((context, builder) =>
{
    builder.Register(context.Configuration, false, false, "search.json");
});

var app = builder.Build();

startup.Configure(app);

await app.RunAsync();