using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace etf.santorini.mp150608d
{
    class RandomBot : Player
    {
        public string id;
        private GameController gameController;
        private System.Random rnd;
        private readonly string[] positions = {
            "A1", "A2", "A3", "A4", "A5",
            "B1", "B2", "B3", "B4", "B5",
            "C1", "C2", "C3", "C4", "C5",
            "D1", "D2", "D3", "D4", "D5",
            "E1", "E2", "E3", "E4", "E5"
        };
        private Logger.GameMove nextGameMove;

        // Update is called once per frame
        void Update()
        {
        }

        public RandomBot(string id)
        {
            this.id = id;
            gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
            rnd = new System.Random();
        }

        public Task CalculateNextMove()
        {
            var task = Task.Run(() => { CalculateMove(); });
            gameController.UI.onTurnText.text = "ON TURN: " + id;
            gameController.UI.nextMoveText.text = "CALCULATING...";

            return task;
        }

        public Task SelectFigure(SemaphoreSlim semaphore)
        {
            var task = Task.Run(() => { semaphore.Wait(); });
            gameController.UI.onTurnText.text = "ON TURN: " + id;
            gameController.UI.nextMoveText.text = "SELECT FIGURE";
            
            gameController.FetchFigure(nextGameMove.PreviousFigurePosition).GetComponent<PlayerFigure>().Pick();

            return task;
        }

        public Task MoveFigure(SemaphoreSlim semaphore)
        {
            var task = Task.Run(() => { semaphore.Wait(); });
            gameController.UI.onTurnText.text = "ON TURN: " + id;
            gameController.UI.nextMoveText.text = "MOVE FIGURE";
            
            gameController.fields[nextGameMove.NextFigurePosition].GetComponent<Field>().Pick();

            return task;
        }

        public Task BuildNewLevel(SemaphoreSlim semaphore)
        {
            var task = Task.Run(() => { semaphore.Wait(); });
            gameController.UI.onTurnText.text = "ON TURN: " + id;
            gameController.UI.nextMoveText.text = "PICK A FIELD TO UPGRADE LEVEL";
            
            gameController.fields[nextGameMove.NewLevelBuildPosition].GetComponent<Field>().Pick();

            return task;
        }

        public Task PickStartingField(SemaphoreSlim semaphore)
        {
            var task = Task.Run(() => { semaphore.Wait(); });
            gameController.UI.onTurnText.text = "ON TURN: " + id;
            gameController.UI.nextMoveText.text = "PICK STARTING POSITION";

            string randomPosition;
            do
            {
                randomPosition = positions[rnd.Next(positions.Length)];
            }
            while (gameController.gameState[randomPosition].PlayerFigure != 0);

            gameController.fields[randomPosition].GetComponent<Field>().Pick();

            return task;
        }

        public void CalculateMove()
        {
            PlayerFigure randomFigure;
            do
            {
                randomFigure = gameController.FetchMyFigure(this, rnd.Next(2)).GetComponent<PlayerFigure>();
            }
            while (!randomFigure.enabled);

            var possibleMoves = gameController.gameState.GetPossibleMoves(randomFigure.position);

            var possibleBuilds = gameController.gameState.GetPossibleBuilds(possibleMoves);

            int random = rnd.Next(possibleBuilds.Count);
            nextGameMove = possibleBuilds[random];
        }

        public string Id()
        {
            return id;
        }
    }
}
