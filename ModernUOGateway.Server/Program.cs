namespace ModernUOGateway.Server;

public class Program
{
    public static Task Main(string[] args)
    {
        WebApplicationServer.BuildWebApplicationServer(args).Run();
    }
}