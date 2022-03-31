﻿var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSystemd();
builder.Host.UseWindowsService();
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
    var buided = config.Build();
    var path = buided["pathToConf"];
    if (!Path.IsPathRooted(path))
    {
        path = Path.GetFullPath(CrossPlatform.PathCombine(hostContext.HostingEnvironment.ContentRootPath, path));
    }

    config.SetBasePath(path);
    var env = hostContext.Configuration.GetValue("ENVIRONMENT", "Production");
    config
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{env}.json", true)
        .AddJsonFile("mail.json")
        .AddJsonFile($"mail.{env}.json")
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
    diHelper.TryAdd<WatchdogLauncher>();
    services.AddHostedService<WatchdogLauncher>();
    diHelper.TryAdd(typeof(ICacheNotify<>), typeof(KafkaCache<>));
    services.AddSingleton(new ConsoleParser(args));
    services.Configure<HostOptions>(opts => opts.ShutdownTimeout = TimeSpan.FromSeconds(15));
});

builder.Host.ConfigureContainer<ContainerBuilder>((context, builder) =>
{
    builder.Register(context.Configuration, false, false);
});

var startup = new BaseWorkerStartup(builder.Configuration);

startup.ConfigureServices(builder.Services);

var app = builder.Build();

startup.Configure(app);

await app.RunAsync();
