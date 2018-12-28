using System;
using System.Collections.Generic;
using System.IO;

namespace etf.santorini.mp150608d
{
    public class Logger
    {
        public class FigureStartingPosition
        {
            public string First { get; set; }
            public string Second { get; set; }
            public new string ToString()
            {
                return First + " " + Second;
            }
            public static FigureStartingPosition FromString(string figureStartingPosition)
            {
                string[] values = figureStartingPosition.Split(' ');
                if (values.Length < 2) throw new FormatException("Provided string: " + figureStartingPosition + " is not a valid format of FigureStartingPosition.");
                return new FigureStartingPosition { First = values[0], Second = values[1] };
            }
        }
        public class GameMove
        {
            public string PreviousFigurePosition { get; set; }
            public string NextFigurePosition { get; set; }
            public string NewLevelBuildPosition { get; set; }
            public GameMove() { }
            public GameMove(GameMove gm)
            {
                PreviousFigurePosition = gm.PreviousFigurePosition;
                NextFigurePosition = gm.NextFigurePosition;
                NewLevelBuildPosition = gm.NewLevelBuildPosition;
            }
            public new string ToString()
            {
                return PreviousFigurePosition + " " + NextFigurePosition + " " + NewLevelBuildPosition;
            }
            public static GameMove FromString(string gameMove)
            {
                string[] values = gameMove.Split(' ');
                if (values.Length < 3) throw new FormatException("Provided string: " + gameMove + " is not a valid format of GameMove.");
                return new GameMove { PreviousFigurePosition = values[0], NextFigurePosition = values[1], NewLevelBuildPosition = values[2] };
            }
            public static bool operator ==(GameMove obj1, GameMove obj2)
            {
                return (obj1.PreviousFigurePosition == obj2.PreviousFigurePosition
                            && obj1.NextFigurePosition == obj2.NextFigurePosition
                            && obj1.NewLevelBuildPosition == obj2.NewLevelBuildPosition);
            }
            public static bool operator !=(GameMove obj1, GameMove obj2)
            {
                return !(obj1.PreviousFigurePosition == obj2.PreviousFigurePosition
                            && obj1.NextFigurePosition == obj2.NextFigurePosition
                            && obj1.NewLevelBuildPosition == obj2.NewLevelBuildPosition);
            }
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }
                if (ReferenceEquals(this, obj))
                {
                    return true;
                }
                return obj.GetType() == GetType() && Equals((GameMove)obj);
            }
            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = PreviousFigurePosition.GetHashCode();
                    hashCode = (hashCode * 397) ^ NextFigurePosition.GetHashCode();
                    hashCode = (hashCode * 397) ^ NewLevelBuildPosition.GetHashCode();
                    return hashCode;
                }
            }
        }

        public Player first;
        public FigureStartingPosition firstplayerFSP;
        public List<GameMove> firstPlayerGM;
        public Player second;
        public FigureStartingPosition secondplayerFSP;
        public List<GameMove> secondPlayerGM;
        public string fileName;

        public Logger(Player first, Player second, string fileName)
        {
            this.first = first;
            this.second = second;
            this.fileName = fileName;
            this.firstPlayerGM = new List<GameMove>();
            this.secondPlayerGM = new List<GameMove>();
        }

        public void LogStartingPosition(Player current, string firstFigurePosition, string secondFigurePosition)
        {
            FigureStartingPosition fsp = new FigureStartingPosition { First = firstFigurePosition, Second = secondFigurePosition };
            if (current == first)
            {
                firstplayerFSP = fsp;
            }
            else if (current == second)
            {
                secondplayerFSP = fsp;
            }
        }

        public void LogGameMove(Player current, string previousFigurePosition, string nextFigurePosition, string newLevelBuildPosition)
        {
            GameMove gm = new GameMove { PreviousFigurePosition = previousFigurePosition, NextFigurePosition = nextFigurePosition, NewLevelBuildPosition = newLevelBuildPosition };
            if (current == first)
            {
                firstPlayerGM.Add(gm);
            }
            else if (current == second)
            {
                secondPlayerGM.Add(gm);
            }
        }

        public void AlterLastBuildPosition(Player current, string newLevelBuildPosition)
        {
            if (current == first)
            {
                firstPlayerGM[firstPlayerGM.Count - 1].NewLevelBuildPosition = newLevelBuildPosition;
            }
            else if (current == second)
            {
                secondPlayerGM[secondPlayerGM.Count - 1].NewLevelBuildPosition = newLevelBuildPosition;
            }
        }

        public void Invalidate()
        {
            firstplayerFSP = secondplayerFSP = null;
            firstPlayerGM.Clear();
            secondPlayerGM.Clear();
        }

        public void WriteToFile()
        {
            using (StreamWriter sw = File.CreateText(fileName))
            {
                sw.WriteLine(firstplayerFSP.ToString());
                sw.WriteLine(secondplayerFSP.ToString());
                for (int i = 0; i < firstPlayerGM.Count; i++)
                {
                    sw.WriteLine(firstPlayerGM[i].ToString());
                    if (i < secondPlayerGM.Count) sw.WriteLine(secondPlayerGM[i].ToString());
                }
            }
        }

        public void ReadFromFile()
        {
            try
            {
                using (StreamReader sr = File.OpenText(fileName))
                {
                    string line;
                    line = sr.ReadLine();
                    firstplayerFSP = FigureStartingPosition.FromString(line);
                    line = sr.ReadLine();
                    secondplayerFSP = FigureStartingPosition.FromString(line);
                    bool evenLine = false;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (evenLine)
                        {
                            secondPlayerGM.Add(GameMove.FromString(line));
                        }
                        else
                        {
                            firstPlayerGM.Add(GameMove.FromString(line));
                        }
                        evenLine = !evenLine;
                    }
                }
            }
            catch (Exception e) { e.ToString(); }
        }

        public void Clean()
        {
            firstPlayerGM = new List<GameMove>();
            firstplayerFSP = null;
            secondPlayerGM = new List<GameMove>();
            secondplayerFSP = null;
        }

        public void SetPlayers(Player first, Player second)
        {
            this.first = first;
            this.second = second;
        } 
    }
}
