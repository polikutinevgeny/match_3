using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Controls;

namespace match_3
{
    public class Game
    {
        public Game(Action<Tile> deleteAnimation, Action<Tile> registerTile)
        {
            FillBoard(registerTile);
            RemoveMatches(deleteAnimation);
        }

        public void RemoveMatches(Action<Tile> deleteAnimation)
        {
            lastMatches = CheckMatches();
            foreach (var match in lastMatches)
            {
                deleteAnimation(match);
//                match.Shape.StrokeThickness = 5.0;
            }
        }

        public Tile[,] Board = new Tile[16, 8];

        private Color[] _colors =
            {Colors.Red, Colors.Green, Colors.Blue, Colors.LightYellow, Colors.RosyBrown};

        public void FillBoard(Action<Tile> registerTileCallback)
        {
            var r = new Random();
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (Board[i, j] == null)
                    {
                        Board[i, j] = new Tile(i - 8, j, _colors[r.Next(_colors.Length)]);
                        registerTileCallback(Board[i, j]);
                    }
                }
            }
        }

        private List<Tile> lastMatches;

        public void TrySwapTiles(
            Tile first, Tile second, Action<Tile, Tile> successAnimCallback,
            Action<Tile, Tile> failAnimCallback)
        {
            if (Math.Abs(first.Top - second.Top) + Math.Abs(first.Left - second.Left) > 1)
            {
                return;
            }
            Utility.Swap(
                ref Board[first.Top + 8, first.Left], ref Board[second.Top + 8, second.Left]);
            lastMatches = CheckMatches();
            if (lastMatches.Count > 0)
            {
                first.SwapCoordinates(ref second);
                successAnimCallback(first, second);
            }
            else
            {
                Utility.Swap(
                    ref Board[first.Top + 8, first.Left], ref Board[second.Top + 8, second.Left]);
                failAnimCallback(first, second);
            }
        }

        public void DeleteMatches(Action<Tile> unregisterTile)
        {
            foreach (var match in lastMatches)
            {
                unregisterTile(Board[match.Top + 8, match.Left]);
                Board[match.Top + 8, match.Left] = null;
            }
        }

        public void DeleteAndDropTiles(Action<Tile> tileDropAnimation, Action<Tile> registerTile, Action<Tile> unregisterTile)
        {
            DeleteMatches(unregisterTile);
            int[] dropLengths = new int[8];
            for (int i = 16 - 1; i >= 0; i--)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (Board[i, j] == null)
                    {
                        dropLengths[j]++;
                    }
                    else if (dropLengths[j] != 0)
                    {
                        if (Board[i + dropLengths[j], j] != null)
                        {
                            throw new InvalidOperationException(
                                "It is not null where tile is dropped");
                        }

                        Utility.Swap(ref Board[i, j], ref Board[i + dropLengths[j], j]);
                        Board[i + dropLengths[j], j].Top = i + dropLengths[j] - 8;
                        tileDropAnimation(Board[i + dropLengths[j], j]);
                    }
                }
            }
            FillBoard(registerTile);
        }

        public List<Tile> CheckMatches()
        {
            bool[,] delete = new bool[16, 8];
            for (int i = 8; i < 16; i++)
            {
                int matches = 1;
                var color = Board[i, 0].Color;
                for (int j = 1; j < 8; j++)
                {
                    if (Board[i, j].Color == color)
                    {
                        ++matches;
                    }
                    else
                    {
                        if (matches >= 3)
                        {
                            for (int k = 1; k < matches + 1; k++)
                            {
                                delete[i, j - k] = true;
                            }
                        }

                        color = Board[i, j].Color;
                        matches = 1;
                    }
                }
                if (matches >= 3)
                {
                    for (int k = 1; k < matches + 1; k++)
                    {
                        delete[i, 8 - k] = true;
                    }
                }
            }

            for (int i = 0; i < 8; i++)
            {
                int matches = 1;
                var color = Board[8, i].Color;
                for (int j = 9; j < 16; j++)
                {
                    if (Board[j, i].Color == color)
                    {
                        ++matches;
                    }
                    else
                    {
                        if (matches >= 3)
                        {
                            for (int k = 1; k < matches + 1; k++)
                            {
                                delete[j - k, i] = true;
                            }
                        }

                        color = Board[j, i].Color;
                        matches = 1;
                    }
                }

                if (matches >= 3)
                {
                    for (int k = 1; k < matches + 1; k++)
                    {
                        delete[16 - k, i] = true;
                    }
                }
            }

            List<Tile> result = new List<Tile>();
            for (int i = 8; i < 16; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (delete[i, j])
                    {
                        result.Add(Board[i, j]);
                    }
                }
            }

            return result;
        }
    }
}