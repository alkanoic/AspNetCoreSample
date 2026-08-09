using AspNetCoreSample.WebApi.Hubs;
using AspNetCoreSample.WebApi.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AspNetCoreSample.WebApi.Test;

public sealed class QrCodeControllerTest
{
    [Fact]
    [Trait("Category", nameof(QrCodeControllerTest))]
    public async Task PostSendsQRCodeDataToAllClients()
    {
        var mockClientProxy = new MockClientProxy();
        var mockHubClients = new MockHubClients { AllProxy = mockClientProxy };
        var mockHubContext = new MockHubContext<QrCodeHub>(mockHubClients);

        var controller = new QrCodeController(mockHubContext);
        var model = new QrCodeRequest { QrCode = "test-qr-data" };

        var result = await controller.Post(model);

        Assert.IsType<OkResult>(result);
        Assert.True(mockClientProxy.SendCoreAsyncCalled);
        Assert.Equal("ReceiveQRCodeData", mockClientProxy.Method);
        Assert.Single(mockClientProxy.Args);
        Assert.Equal("test-qr-data", mockClientProxy.Args[0]);
    }

    private sealed class MockClientProxy : IClientProxy
    {
        public bool SendCoreAsyncCalled { get; set; }
        public string? Method { get; set; }
        public object?[]? Args { get; set; }

        public Task SendCoreAsync(string method, object?[] args, CancellationToken cancellationToken = default)
        {
            SendCoreAsyncCalled = true;
            Method = method;
            Args = args;
            return Task.CompletedTask;
        }
    }

    private sealed class MockHubClients : IHubClients
    {
        public IClientProxy AllProxy { get; set; } = null!;

        public IClientProxy All => AllProxy;
        public IClientProxy AllExcept(IReadOnlyList<string> excludedConnectionIds) => new MockClientProxy();
        public IClientProxy Client(string connectionId) => new MockClientProxy();
        public IClientProxy Clients(IReadOnlyList<string> connectionIds) => new MockClientProxy();
        public IClientProxy Group(string groupName) => new MockClientProxy();
        public IClientProxy GroupExcept(string groupName, IReadOnlyList<string> excludedConnectionIds) => new MockClientProxy();
        public IClientProxy Groups(IReadOnlyList<string> groupNames) => new MockClientProxy();
        public IClientProxy User(string userId) => new MockClientProxy();
        public IClientProxy Users(IReadOnlyList<string> userIds) => new MockClientProxy();
    }

    private sealed class MockHubContext<THub> : IHubContext<THub> where THub : Hub
    {
        public IHubClients Clients { get; }
        public IGroupManager Groups { get; } = new MockGroupManager();

        public MockHubContext(IHubClients clients)
        {
            Clients = clients;
        }
    }

    private sealed class MockGroupManager : IGroupManager
    {
        public Task AddToGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task RemoveFromGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
