﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace etf.santorini.mp150608d
{
    class SimpleBot : Player
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

        public SimpleBot(string id)
        {
            this.id = id;
            gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
            rnd = new System.Random();
        }

        public Task SelectFigure(SemaphoreSlim semaphore)
        {
            var task = Task.Run(() => { semaphore.Wait(); });
            gameController.UI.onTurnText.text = "ON TURN: " + id;
            gameController.UI.nextMoveText.text = "SELECT FIGURE";

            CalculateMove();
            
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
            List<GameObject> myFigures = new List<GameObject>
            {
                gameController.FetchMyFigure(this, 0),
                gameController.FetchMyFigure(this, 1)
            };
            List<GameObject> opponentFigures = new List<GameObject>();
            foreach (GameObject fig in gameController.playerFigures)
            {
                if (fig != myFigures[0] && fig != myFigures[1])
                {
                    opponentFigures.Add(fig);
                }
            }
            var possibleMoves = gameController.gameState.GetPossibleMoves(myFigures[0].GetComponent<PlayerFigure>().position);
            possibleMoves.AddRange(gameController.gameState.GetPossibleMoves(myFigures[1].GetComponent<PlayerFigure>().position));

            var possibleBuilds = gameController.gameState.GetPossibleBuilds(possibleMoves);


            if (possibleBuilds.Count == 0)
            {
                throw new System.Exception();
            }

            float currFunctionValue = 0, maxFunctionValue = float.NegativeInfinity;
            Logger.GameMove bestGameMove = new Logger.GameMove();
            foreach (Logger.GameMove currGameMove in possibleBuilds)
            {
                float m = gameController.gameState[currGameMove.NextFigurePosition].FieldLevel;
                float l = gameController.gameState[currGameMove.NewLevelBuildPosition].FieldLevel + 1;
                float myDistance = 0, opponentDistance = 0;
                foreach (var myFigure in myFigures)
                {
                    myDistance += Vector3.Distance(myFigure.transform.position, gameController.fields[currGameMove.NewLevelBuildPosition].transform.position);
                }
                foreach (var opponentFigure in opponentFigures)
                {
                    opponentDistance += Vector3.Distance(opponentFigure.transform.position, gameController.fields[currGameMove.NewLevelBuildPosition].transform.position);
                }
                l = l * (myDistance - opponentDistance);
                currFunctionValue = m + l;
                if (currFunctionValue >= maxFunctionValue)
                {
                    maxFunctionValue = currFunctionValue;
                    bestGameMove = new Logger.GameMove(currGameMove);
                }
            }
            nextGameMove = bestGameMove;
        }
    }
}