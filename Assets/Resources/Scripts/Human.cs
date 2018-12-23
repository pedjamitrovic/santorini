using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace etf.santorini.mp150608d
{
    class Human : Player
    {
        public string id;
        private GameController gameController;

        public Human(string id)
        {
            this.id = id;
            gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        }

        public Task CalculateNextMove()
        {
            var task = Task.Run(() => { });
            return task;
        }

        public Task SelectFigure(SemaphoreSlim semaphore)
        {
            var task = Task.Run(() => { semaphore.Wait(); });
            gameController.UI.onTurnText.text = "ON TURN: " + id;
            gameController.UI.nextMoveText.text = "SELECT FIGURE";
            return task;
        }

        public Task MoveFigure(SemaphoreSlim semaphore)
        {
            var task = Task.Run(() => { semaphore.Wait(); });
            gameController.UI.onTurnText.text = "ON TURN: " + id;
            gameController.UI.nextMoveText.text = "MOVE FIGURE";
            return task;
        }

        public Task BuildNewLevel(SemaphoreSlim semaphore)
        {
            var task = Task.Run(() => { semaphore.Wait(); });
            gameController.UI.onTurnText.text = "ON TURN: " + id;
            gameController.UI.nextMoveText.text = "PICK A FIELD TO UPGRADE LEVEL";
            return task;
        }

        public Task PickStartingField(SemaphoreSlim semaphore)
        {
            var task = Task.Run(() => { semaphore.Wait(); });
            gameController.UI.onTurnText.text = "ON TURN: " + id;
            gameController.UI.nextMoveText.text = "PICK STARTING POSITION";
            return task;
        }

        public string Id()
        {
            return id;
        }
    }
}
