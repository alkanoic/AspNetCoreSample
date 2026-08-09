using System.Net;
using System.Net.Sockets;

namespace AspNetCoreSample.Mvc.Test;

public static class AvailablePort
{
    public static int GetAvailablePort()
    {
        // OS にエフェメラルポートを予約させることで check-then-use の競合を避ける
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        return ((IPEndPoint)listener.LocalEndpoint).Port;
    }

    public static int GetAvailablePort(int startPort)
    {
        // 一時的にソケットを bind して空きポートを確認する
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
                // 使用中のポートはスキップする
            }
        }

        return -1;
    }
}
