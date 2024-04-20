using Microsoft.AspNetCore.SignalR;

namespace AspNetCoreSample.WebApi.Hubs;

public class QrCodeHub : Hub
{
    public async Task SendQRCodeData(string data)
    {
        await Clients.All.SendAsync("ReceiveQRCodeData", data);
    }
}
