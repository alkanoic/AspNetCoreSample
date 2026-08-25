using Microsoft.AspNetCore.Http;

namespace AspNetCoreSample.Mvc.Test;

// テスト用のインメモリ ISession 実装。
public sealed class InMemorySession : ISession
{
    private readonly Dictionary<string, byte[]> _store = new();

    public bool IsAvailable => true;

    public string Id { get; } = Guid.NewGuid().ToString();

    public IEnumerable<string> Keys => _store.Keys;

    public void Clear() => _store.Clear();

    public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public void Remove(string key) => _store.Remove(key);

    public void Set(string key, byte[] value) => _store[key] = value;

    public bool TryGetValue(string key, out byte[] value) => _store.TryGetValue(key, out value!);
}
