using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace etf.santorini.mp150608d
{
    public class LoadGameBot : Player
    {
        public string id;
        private GameController gameController;
        public Player realPlayer;
        private Logger.FigureStartingPosition playerFigures;
        private List<Logger.GameMove> moves;
        private int curr;

        // Update is called once per frame
        void Update()
        {

        }

        public LoadGameBot(string id, Player realPlayer, Logger.FigureStartingPosition playerFigures, List<Logger.GameMove> moves)
        {
            this.id = id;
            this.realPlayer = realPlayer;
            this.playerFigures = playerFigures;
            this.moves = moves;
            curr = 0;
            gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        }

        public Task CalculateNextMove()
        {
            gameController.UI.onTurnText.text = "ON TURN: " + id;
            gameController.UI.nextMoveText.text = "CALCULATING...";
            if (curr >= moves.Count)
            {
                gameController.RemoveLoadGameBot(this);
                return realPlayer.CalculateNextMove();
            }
            else return Task.CompletedTask;
        }

        public Task SelectFigure(SemaphoreSlim semaphore)
        {
            var task = Task.Run(() => { semaphore.Wait(); });
            gameController.UI.onTurnText.text = "ON TURN: " + id;
            gameController.UI.nextMoveText.text = "PICK FIGURE";

            gameController.FetchFigure(moves[curr].PreviousFigurePosition).GetComponent<PlayerFigure>().Pick();

            return task;
        }

        public Task MoveFigure(SemaphoreSlim semaphore)
        {
            var task = Task.Run(() => { semaphore.Wait(); });
            gameController.UI.onTurnText.text = "ON TURN: " + id;
            gameController.UI.nextMoveText.text = "MOVE FIGURE";

            gameController.fields[moves[curr].NextFigurePosition].GetComponent<Field>().Pick();

            return task;
        }

        public Task BuildNewLevel(SemaphoreSlim semaphore)
        {
            var task = Task.Run(() => { semaphore.Wait(); });
            gameController.UI.onTurnText.text = "ON TURN: " + id;
            gameController.UI.nextMoveText.text = "PICK A FIELD TO UPGRADE LEVEL";

            gameController.fields[moves[curr].NewLevelBuildPosition].GetComponent<Field>().Pick();

            curr++;

            return task;
        }

        public Task PickStartingField(SemaphoreSlim semaphore)
        {
            var task = Task.Run(() => { semaphore.Wait(); });
            gameController.UI.onTurnText.text = "ON TURN: " + id;
            gameController.UI.nextMoveText.text = "PICK STARTING POSITION";

            gameController.fields[playerFigures.First ?? playerFigures.Second].GetComponent<Field>().Pick();

            playerFigures.First = null;

            return task;
        }
        public string Id()
        {
            return id;
        }
    }
}