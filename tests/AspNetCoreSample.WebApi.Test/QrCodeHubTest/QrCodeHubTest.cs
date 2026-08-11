using AspNetCoreSample.WebApi.Hubs;

using Microsoft.AspNetCore.SignalR;

namespace AspNetCoreSample.WebApi.Test;

public sealed class QrCodeHubTest
{
    [Test]
    [Category(nameof(QrCodeHubTest))]
    public async Task SendQRCodeData_CallsSendAsyncOnAllClients()
    {
        var hub = new QrCodeHub();
        var mockClients = new MockHubCallerClients();
        var mockCallerContext = new MockHubCallerContext();
        var mockGroupManager = new MockGroupManager();

        hub.Clients = mockClients;
        hub.Context = mockCallerContext;
        hub.Groups = mockGroupManager;

        await hub.SendQRCodeData("test-data");

        await Assert.That(mockClients.AllCalled).IsTrue();
        await Assert.That(mockClients.MethodName).IsEqualTo("ReceiveQRCodeData");
        await Assert.That(mockClients.Arg).IsEqualTo("test-data");
    }

    private sealed class MockHubCallerClients : IHubCallerClients
    {
        public bool AllCalled { get; set; }
        public string? MethodName { get; set; }
        public object? Arg { get; set; }

        public IClientProxy Caller => new MockClientProxy();
        public IClientProxy Others => new MockClientProxy();
        public IClientProxy All => new MockClientProxy(this);
        public IClientProxy AllExcept(IReadOnlyList<string> excludedConnectionIds) => new MockClientProxy();
        public IClientProxy Client(string connectionId) => new MockClientProxy();
        public IClientProxy Clients(IReadOnlyList<string> connectionIds) => new MockClientProxy();
        public IClientProxy Group(string groupName) => new MockClientProxy();
        public IClientProxy GroupExcept(string groupName, IReadOnlyList<string> excludedConnectionIds) => new MockClientProxy();
        public IClientProxy Groups(IReadOnlyList<string> groupNames) => new MockClientProxy();
        public IClientProxy OthersInGroup(string groupName) => new MockClientProxy();
        public IClientProxy User(string userId) => new MockClientProxy();
        public IClientProxy Users(IReadOnlyList<string> userIds) => new MockClientProxy();

        private sealed class MockClientProxy : IClientProxy
        {
            private readonly MockHubCallerClients? _owner;

            public MockClientProxy(MockHubCallerClients? owner = null)
            {
                _owner = owner;
            }

            public Task SendCoreAsync(string method, object?[] args, CancellationToken cancellationToken = default)
            {
                if (_owner != null)
                {
                    _owner.AllCalled = true;
                    _owner.MethodName = method;
                    _owner.Arg = args.Length > 0 ? args[0] : null;
                }
                return Task.CompletedTask;
            }
        }
    }

    private sealed class MockHubCallerContext : HubCallerContext
    {
        public override string ConnectionId => "test-connection";
        public override string? UserIdentifier => null;
        public override System.Security.Claims.ClaimsPrincipal? User => null;
        public override IDictionary<object, object?> Items => new Dictionary<object, object?>();
        public override Microsoft.AspNetCore.Http.Features.IFeatureCollection Features => new Microsoft.AspNetCore.Http.Features.FeatureCollection();
        public override CancellationToken ConnectionAborted => CancellationToken.None;

        public override void Abort() { }
    }

    private sealed class MockGroupManager : IGroupManager
    {
        public Task AddToGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task RemoveFromGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
