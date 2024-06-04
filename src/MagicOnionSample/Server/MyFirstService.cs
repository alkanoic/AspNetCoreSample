using AspNetCoreSample.MagicOnion.Services;

using MagicOnion;
using MagicOnion.Server;

namespace AspNetCoreSample.MagicOnion;

public class MyFirstService : ServiceBase<IMyFirstService>, IMyFirstService
{
    // `UnaryResult<T>` allows the method to be treated as `async` method.
    public async UnaryResult<int> SumAsync(int x, int y)
    {
        Console.WriteLine($"Received:{x}, {y}");
        return x + y;
    }
}
