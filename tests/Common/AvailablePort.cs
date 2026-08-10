using System.Net;
using System.Net.Sockets;

namespace AspNetCoreSample.Test.Common;

public static class AvailablePort
{
    public static int GetAvailablePort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        return ((IPEndPoint)listener.LocalEndpoint).Port;
    }

    public static int GetAvailablePort(int startPort)
    {
        for (var port = startPort; port <= 65535; port++)
        {
            try
            {
                using var listener = new TcpListener(IPAddress.Loopback, port);
                listener.Start();
                return ((IPEndPoint)listener.LocalEndpoint).Port;
            }
            catch (SocketException)
            {
            }
        }

        return -1;
    }
}
