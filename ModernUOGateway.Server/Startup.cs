namespace ModernUOGateway.Server;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Register framework services
        services.AddControllers();

        // Register your custom services
        // services.AddSingleton<IMyService, MyService>();
        services.AddSingleton<UOGatewayServer>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
    }
}