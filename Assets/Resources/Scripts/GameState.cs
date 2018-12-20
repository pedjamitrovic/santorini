using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace etf.santorini.mp150608d
{
    public class GameState
    {
        public class GameField
        {
            public int FieldLevel { get; set; }
            public int PlayerFigure { get; set; }
            public GameField(int fieldLevel, int playerFigure)
            {
                FieldLevel = fieldLevel;
                PlayerFigure = playerFigure;
            }
            public GameField(GameField gf)
            {
                FieldLevel = gf.FieldLevel;
                PlayerFigure = gf.PlayerFigure;
            }
        }
        private readonly string[] positions = {
            "A1", "A2", "A3", "A4", "A5",
            "B1", "B2", "B3", "B4", "B5",
            "C1", "C2", "C3", "C4", "C5",
            "D1", "D2", "D3", "D4", "D5",
            "E1", "E2", "E3", "E4", "E5"
        };
        public Dictionary<string, GameField> table;
        public GameController gc;

        public GameState()
        {
            gc = GameObject.Find("GameController").GetComponent<GameController>();
            table = new Dictionary<string, GameField>();
            foreach (var position in positions)
            {
                table.Add(position, new GameField(0,0));
            }
        }
        public GameState(GameState gs)
        {
            gc = GameObject.Find("GameController").GetComponent<GameController>();
            table = new Dictionary<string, GameField>();
            foreach(var curr in gs.table)
            {
                table.Add(curr.Key, new GameField(curr.Value));
            }
        }
        public GameField this[string position]
        {
            get
            {
                return table[position];
            }
            set
            {
                table[position] = value;
            }
        }
        public List<Logger.GameMove> GetPossibleMoves(string currentPosition)
        {
            List<Logger.GameMove> possibleMoves = new List<Logger.GameMove>();
            string[] neighbours = gc.fields[currentPosition].GetComponent<Field>().neighbours;
            for (int i = 0; i < neighbours.Length; i++)
            {
                if (table[neighbours[i]].PlayerFigure == 0 && table[neighbours[i]].FieldLevel - 1 <= table[currentPosition].FieldLevel)
                {
                    Logger.GameMove gameMove = new Logger.GameMove();
                    gameMove.PreviousFigurePosition = currentPosition;
                    gameMove.NextFigurePosition = neighbours[i];
                    possibleMoves.Add(gameMove);
                }
            }
            return possibleMoves;
        }
        public List<Logger.GameMove> GetPossibleBuilds(Logger.GameMove gameMove)
        {
            List<Logger.GameMove> possibleBuilds = new List<Logger.GameMove>();
            string[] neighbours = gc.fields[gameMove.NextFigurePosition].GetComponent<Field>().neighbours;
            for (int i = 0; i < neighbours.Length; i++)
            {
                if ((table[neighbours[i]].PlayerFigure == 0 || neighbours[i] == gameMove.PreviousFigurePosition) && table[neighbours[i]].FieldLevel < 4)
                {
                    Logger.GameMove gameMoveWithBuild = new Logger.GameMove(gameMove);
                    gameMoveWithBuild.NewLevelBuildPosition = neighbours[i];
                    possibleBuilds.Add(gameMove);
                }
            }
            return possibleBuilds;
        }
        public List<Logger.GameMove> GetPossibleBuilds(List<Logger.GameMove> possibleMoves)
        {
            List<Logger.GameMove> possibleBuilds = new List<Logger.GameMove>();
            foreach(var possibleMove in possibleMoves)
            {
                string[] neighbours = gc.fields[possibleMove.NextFigurePosition].GetComponent<Field>().neighbours;
                for (int i = 0; i < neighbours.Length; i++)
                {
                    if ((table[neighbours[i]].PlayerFigure == 0 || neighbours[i] == possibleMove.PreviousFigurePosition) && table[neighbours[i]].FieldLevel < 4)
                    {
                        Logger.GameMove gameMoveWithBuild = new Logger.GameMove(possibleMove);
                        gameMoveWithBuild.NewLevelBuildPosition = neighbours[i];
                        possibleBuilds.Add(gameMoveWithBuild);
                    }
                }
            }
            return possibleBuilds;
        }
    }
}
