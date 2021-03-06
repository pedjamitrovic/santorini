﻿using System;
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
        private static readonly string[] positions = {
            "A1", "A2", "A3", "A4", "A5",
            "B1", "B2", "B3", "B4", "B5",
            "C1", "C2", "C3", "C4", "C5",
            "D1", "D2", "D3", "D4", "D5",
            "E1", "E2", "E3", "E4", "E5"
        };
        public static Dictionary<string, string[]> neighbours;
        public Dictionary<string, GameField> table;
        public GameController gc;
        public int onTurn;
        public int notOnTurn;

        static GameState()
        {
            neighbours = new Dictionary<string, string[]>();
            neighbours.Add("A1", new string[] { "A2", "B1", "B2" });
            neighbours.Add("A2", new string[] { "A1", "A3", "B1", "B2", "B3" });
            neighbours.Add("A3", new string[] { "A2", "A4", "B2", "B3", "B4" });
            neighbours.Add("A4", new string[] { "A3", "A5", "B3", "B4", "B5" });
            neighbours.Add("A5", new string[] { "A4", "B4", "B5" });
            neighbours.Add("B1", new string[] { "A1", "A2", "B2", "C1", "C2" });
            neighbours.Add("B2", new string[] { "A1", "A2", "A3", "B1", "B3", "C1", "C2", "C3" });
            neighbours.Add("B3", new string[] { "A2", "A3", "A4", "B2", "B4", "C2", "C3", "C4" });
            neighbours.Add("B4", new string[] { "A3", "A4", "A5", "B3", "B5", "C3", "C4", "C5" });
            neighbours.Add("B5", new string[] { "A4", "A5", "B4", "C4", "C5" });
            neighbours.Add("C1", new string[] { "B1", "B2", "C2", "D1", "D2" });
            neighbours.Add("C2", new string[] { "B1", "B2", "B3", "C1", "C3", "D1", "D2", "D3" });
            neighbours.Add("C3", new string[] { "B2", "B3", "B4", "C2", "C4", "D2", "D3", "D4" });
            neighbours.Add("C4", new string[] { "B3", "B4", "B5", "C3", "C5", "D3", "D4", "D5" });
            neighbours.Add("C5", new string[] { "B4", "B5", "C4", "D4", "D5" });
            neighbours.Add("D1", new string[] { "C1", "C2", "D2", "E1", "E2" });
            neighbours.Add("D2", new string[] { "C1", "C2", "C3", "D1", "D3", "E1", "E2", "E3" });
            neighbours.Add("D3", new string[] { "C2", "C3", "C4", "D2", "D4", "E2", "E3", "E4" });
            neighbours.Add("D4", new string[] { "C3", "C4", "C5", "D3", "D5", "E3", "E4", "E5" });
            neighbours.Add("D5", new string[] { "C4", "C5", "D4", "E4", "E5" });
            neighbours.Add("E1", new string[] { "D1", "D2", "E2" });
            neighbours.Add("E2", new string[] { "D1", "D2", "D3", "E1", "E3" });
            neighbours.Add("E3", new string[] { "D2", "D3", "D4", "E2", "E4" });
            neighbours.Add("E4", new string[] { "D3", "D4", "D5", "E3", "E5" });
            neighbours.Add("E5", new string[] { "D4", "D5", "E4" });
        }

        public GameState()
        {
            gc = GameObject.Find("GameController").GetComponent<GameController>();
            table = new Dictionary<string, GameField>();
            foreach (var position in positions)
            {
                table.Add(position, new GameField(0, 0));
            }
            onTurn = (gc.onTurn == gc.first ? -1 : -2);
            notOnTurn = (onTurn == -1 ? -2 : -1);
        }
        public GameState(GameState gs, int onTurn)
        {
            gc = GameObject.Find("GameController").GetComponent<GameController>();
            table = new Dictionary<string, GameField>();
            foreach (var curr in gs.table)
            {
                table.Add(curr.Key, new GameField(curr.Value));
            }
            this.onTurn = (onTurn == -1 ? -1 : -2);
            this.notOnTurn = (onTurn == -1 ? -2 : -1);
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
        public float Distance(int player, string field)
        {
            float min = float.PositiveInfinity;
            foreach (var position in GetPlayerPositions(player))
            {
                float curr = Math.Max(Math.Abs(field[0] - position[0]), Math.Abs(field[1] - position[1]));
                if (curr < min) min = curr;
            }
            return min;
        }
        public float Distance(int player, string field, out int fieldLevel, out string fieldLevelPosition)
        {
            fieldLevel = -1;
            fieldLevelPosition = "";
            float min = float.PositiveInfinity;
            foreach (var position in GetPlayerPositions(player))
            {
                float curr = Math.Max(Math.Abs(field[0] - position[0]), Math.Abs(field[1] - position[1]));
                if (curr < min)
                {
                    min = curr;
                    fieldLevel = table[position].FieldLevel;
                    fieldLevelPosition = position;
                }
                if (curr == min && fieldLevel < table[position].FieldLevel)
                {
                    fieldLevel = table[position].FieldLevel;
                    fieldLevelPosition = position;
                }
            }
            return min;
        }
        public void MakeMove(Logger.GameMove move)
        {
            table[move.NextFigurePosition].PlayerFigure = table[move.PreviousFigurePosition].PlayerFigure;
            table[move.PreviousFigurePosition].PlayerFigure = 0;
            table[move.NewLevelBuildPosition].FieldLevel++;
            ChangeTurns();
        }
        public void UndoMove(Logger.GameMove move)
        {
            table[move.PreviousFigurePosition].PlayerFigure = table[move.NextFigurePosition].PlayerFigure;
            table[move.NextFigurePosition].PlayerFigure = 0;
            table[move.NewLevelBuildPosition].FieldLevel--;
            ChangeTurns();
        }
        public void ChangeTurns()
        {
            var tmp = onTurn;
            onTurn = notOnTurn;
            notOnTurn = tmp;
        }
        public List<Logger.GameMove> GetPossibleMoves(string currentPosition)
        {
            List<Logger.GameMove> possibleMoves = new List<Logger.GameMove>();
            string[] neighbours = GameState.neighbours[currentPosition];
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
            string[] neighbours = GameState.neighbours[gameMove.NextFigurePosition];
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
            foreach (var possibleMove in possibleMoves)
            {
                string[] neighbours = GameState.neighbours[possibleMove.NextFigurePosition];
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
        public List<Logger.GameMove> GetPossibleGameMoves()
        {
            var playerPositions = GetCurrentPlayerPositions();
            var possibleMoves = GetPossibleMoves(playerPositions[0]);
            possibleMoves.AddRange(GetPossibleMoves(playerPositions[1]));
            return GetPossibleBuilds(possibleMoves);
        }
        public List<string> GetCurrentPlayerPositions()
        {
            List<string> positions = new List<string>();
            foreach (var field in table)
            {
                if (field.Value.PlayerFigure == onTurn) positions.Add(field.Key);
            }
            return positions;
        }
        public List<string> GetPlayerPositions(int player)
        {
            List<string> positions = new List<string>();
            foreach (var field in table)
            {
                if (field.Value.PlayerFigure == player) positions.Add(field.Key);
            }
            return positions;
        }
    }
}
