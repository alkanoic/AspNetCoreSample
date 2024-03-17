using AspNetCoreSample.WebApi.Hubs;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AspNetCoreSample.WebApi;

[Route("api/[controller]")]
[ApiController]
public class QrCodeController : ControllerBase
{
    private readonly IHubContext<QrCodeHub> _hubContext;

    public QrCodeController(IHubContext<QrCodeHub> hubContext)
    {
        _hubContext = hubContext;
    }

    [HttpPost]
    public async Task<IActionResult> Post(Models.QrCodeRequest model)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveQRCodeData", model.QrCode);
        return Ok();
    }
}
