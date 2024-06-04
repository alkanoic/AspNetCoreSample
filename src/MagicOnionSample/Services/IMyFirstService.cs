using MagicOnion;

namespace AspNetCoreSample.MagicOnion.Services;

public interface IMyFirstService : IService<IMyFirstService>
{
    // The return type must be `UnaryResult<T>` or `UnaryResult`.
    UnaryResult<int> SumAsync(int x, int y);
}
