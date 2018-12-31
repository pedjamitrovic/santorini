using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace etf.santorini.mp150608d
{
    public class AdvancedBot : Player
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
        public Logger.GameMove nextGameMove;
        public int depth;

        // Update is called once per frame
        void Update()
        {

        }

        public AdvancedBot(string id, int depth)
        {
            this.id = id;
            this.depth = depth;
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
            gameController.UI.nextMoveText.text = "PICK FIGURE";

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
            while (!gameController.fields[randomPosition].GetComponent<Field>().enabled);

            gameController.fields[randomPosition].GetComponent<Field>().Pick();

            return task;
        }

        public string Id()
        {
            return id;
        }
        public void CalculateMove()
        {
            float? bestMoveValue = 0;
            nextGameMove = AlphaBeta(gameController.gameState, true, out bestMoveValue, float.NegativeInfinity, float.PositiveInfinity, depth);
        }
        public Logger.GameMove AlphaBeta(GameState currentState, bool maximizingPlayer, out float? est, float? alpha, float? beta, int maxLevel = 3, int current = 0)
        {
            Logger.GameMove bestMove = null;
            if (current < maxLevel - 1)
            {
                float? currentEst = null;
                if (maximizingPlayer) currentEst = float.NegativeInfinity;
                else if (!maximizingPlayer) currentEst = float.PositiveInfinity;
                foreach (var move in currentState.GetPossibleGameMoves())
                {
                    currentState.MakeMove(move);

                    float? retEst = null;
                    AlphaBeta(currentState, !maximizingPlayer, out retEst, alpha, beta, maxLevel, current + 1);

                    if (current == 0)
                    {
                        gameController.UI.AddPossibleMove(move, retEst ?? 0f);
                    }

                    currentState.UndoMove(move);
                    if (maximizingPlayer && retEst > currentEst)
                    {
                        currentEst = retEst;
                        bestMove = move;
                        alpha = (alpha >= retEst ? alpha : retEst);
                        if (alpha >= beta) break;
                    }
                    else if (!maximizingPlayer && retEst < currentEst)
                    {
                        currentEst = retEst;
                        bestMove = move;
                        beta = (beta <= retEst ? beta : retEst);
                        if (alpha >= beta) break;
                    }
                }
                est = currentEst;
                return bestMove;
            }
            else
            {
                float? currentEst = null;
                if (maximizingPlayer) currentEst = float.NegativeInfinity;
                else if (!maximizingPlayer) currentEst = float.PositiveInfinity;
                foreach (var move in currentState.GetPossibleGameMoves())
                {
                    var retEst = Estimate(currentState, move);
                    if (maximizingPlayer && retEst > currentEst)
                    {
                        currentEst = retEst;
                        bestMove = move;
                    }
                    else if (!maximizingPlayer && retEst < currentEst)
                    {
                        currentEst = retEst;
                        bestMove = move;
                    }
                }
                est = currentEst;
                return bestMove;
            }
        }
        float Estimate(GameState state, Logger.GameMove move)
        {
            int fieldLevel = -1;
            string fieldLevelPosition = "";
            float m = state.table[move.NextFigurePosition].FieldLevel;
            float l = 0;
            if (state.table[move.NewLevelBuildPosition].FieldLevel < 4)
            {
                l = (state.table[move.NewLevelBuildPosition].FieldLevel + 1) * (state.Distance(state.notOnTurn, move.NewLevelBuildPosition) - state.Distance(state.onTurn, move.NewLevelBuildPosition));
            }
            else
            {
                if (state.Distance(state.notOnTurn, move.NewLevelBuildPosition, out fieldLevel, out fieldLevelPosition) == 1)
                {
                    l = (state.table[move.NewLevelBuildPosition].FieldLevel + 1) * (fieldLevel - state.table[move.NextFigurePosition].FieldLevel);
                }
            }
            state.Distance(state.onTurn, move.NewLevelBuildPosition, out fieldLevel, out fieldLevelPosition);
            float qme = fieldLevel * state.Distance(state.notOnTurn, fieldLevelPosition);
            state.Distance(state.notOnTurn, move.NewLevelBuildPosition, out fieldLevel, out fieldLevelPosition);
            float qop = fieldLevel * state.Distance(state.onTurn, fieldLevelPosition);

            float q = qme - qop;
            float f = m + l + q;
            return f;
        }
    }
}