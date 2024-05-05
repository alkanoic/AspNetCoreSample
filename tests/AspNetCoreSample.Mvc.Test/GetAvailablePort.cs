using System.Net.NetworkInformation;

namespace AspNetCoreSample.Mvc.Test;

public static class AvailablePort
{
    public static int GetAvailablePort()
    {
        Random random = new Random();
        int randomPortNumber = random.Next(5000, 65000);
        return GetAvailablePort(randomPortNumber);
    }

    public static int GetAvailablePort(int startPort)
    {
        var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();

        var connections = ipGlobalProperties.GetActiveTcpConnections().Select(x => x.LocalEndPoint);
        var tcpListeners = ipGlobalProperties.GetActiveTcpListeners();
        var udpListeners = ipGlobalProperties.GetActiveUdpListeners();

        var activePorts = new HashSet<int>(connections
            .Concat(tcpListeners)
            .Concat(udpListeners)
            .Where(x => x.Port >= startPort)
            .Select(x => x.Port));

        for (var port = startPort; port <= 65535; port++)
        {
            if (!activePorts.Contains(port))
                return port;
        }

        return -1;
    }
}
