using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace etf.santorini.mp150608d
{
    class Human: Player
    {
        public GameController gameController;

        public Human(GameController gameController)
        {
            this.gameController = gameController;
        }

        public void NextMove()
        {
            MoveFigure();
            BuildNewLevel();
        }

        public void MoveFigure()
        {
            gameController.nextMoveText.text = "MOVE FIGURE";
        }

        public void BuildNewLevel()
        {
            gameController.nextMoveText.text = "PICK A FIELD TO UPGRADE LEVEL";
        }

        public void PickStartingFields()
        {
            gameController.nextMoveText.text = "PICK STARTING POSITION";
        }
    }
}
