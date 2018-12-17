using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace etf.santorini.mp150608d
{
    public interface Player
    {
        Task PickStartingField(SemaphoreSlim semaphore);
        Task SelectFigure(SemaphoreSlim semaphore);
        Task MoveFigure(SemaphoreSlim semaphore);
        Task BuildNewLevel(SemaphoreSlim semaphore);
        string Id();
    }
}
