using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace etf.santorini.mp150608d
{
    public interface Player
    {
        void NextMove();
        void PickStartingFields();
        void MoveFigure();
        void BuildNewLevel();
    }
}
