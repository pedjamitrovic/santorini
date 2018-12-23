using System.Threading;
using System.Threading.Tasks;

namespace etf.santorini.mp150608d
{
    public interface Player
    {
        Task CalculateNextMove();
        Task PickStartingField(SemaphoreSlim semaphore);
        Task SelectFigure(SemaphoreSlim semaphore);
        Task MoveFigure(SemaphoreSlim semaphore);
        Task BuildNewLevel(SemaphoreSlim semaphore);
        string Id();
    }
}
