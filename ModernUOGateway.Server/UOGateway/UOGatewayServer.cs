using System.Net;
using System.Net.Sockets;

namespace ModernUOGateway.Server;

public class UOGatewayServer : IHostedService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly HashSet<TcpClient> _tcpClients = new();
    
    public UOGatewayServer(IHostApplicationLifetime hostApplicationLifetime)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _ = Task.Run(RunServer, cancellationToken);
        return Task.CompletedTask;
    }
    
    private async Task RunServer()
    {
        var stoppingToken = _hostApplicationLifetime.ApplicationStopping;
        var tcpListener = new TcpListener(IPAddress.Any, 2593);
        tcpListener.Start();
        
        while (!stoppingToken.IsCancellationRequested)
        {
            var client = await tcpListener.AcceptTcpClientAsync(stoppingToken);
            client.NoDelay = true;
            if (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            
            _tcpClients.Add(client);
            _ = Task.Run(() => HandleClientAsync(client, stoppingToken), stoppingToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var client in _tcpClients)
        {
            client.Client.Shutdown(SocketShutdown.Both);
            client.Client.Close(); // Calls dispose
        }

        _tcpClients.Clear();

        return Task.CompletedTask;
    }

    private async Task HandleClientAsync(TcpClient client, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }
        
        // Implement your logic to handle the TCP client
        // You can use _myService here as needed
    }
}