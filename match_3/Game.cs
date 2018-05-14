using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using match_3.Annotations;

namespace match_3
{
    public sealed class Game : INotifyPropertyChanged
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public int Points
        {
            get => _points;
            set
            {
                _points = value;
                OnPropertyChanged();
            }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public int Countdown
        {
            get => _countdown;
            set
            {
                _countdown = value;
                OnPropertyChanged();
            }
        }

        public Game(Action<Tile> registerTile, Action<Tile> unregisterTile, Action<Tile> dropAnimation)
        {
            FillBoard(registerTile);
            DeleteAndDropTiles(dropAnimation, registerTile, unregisterTile);
            _gameTimer = new DispatcherTimer(
                new TimeSpan(0, 0, 1), DispatcherPriority.Normal,
                delegate
                {
                    Countdown -= 1;
                    if (Countdown == 0)
                    {
                        Switcher.Switch(new GameOver(Points));
                    }
                }, Application.Current.Dispatcher);
        }

        public void RemoveMatches(Action<Tile> deleteAnimation)
        {
            _lastMatches = CheckMatches();
            Points += _lastMatches.Count;
            foreach (var match in _lastMatches)
            {
                deleteAnimation(match);
            }
        }

        private readonly Tile[,] _board = new Tile[16, 8];

        private readonly Color[] _colors =
            {Colors.Red, Colors.Green, Colors.Blue, Colors.LightYellow, Colors.RosyBrown};

        public void FillBoard(Action<Tile> registerTileCallback)
        {
            var r = new Random();
            for (var i = 0; i < 8; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    if (_board[i, j] != null) continue;
                    _board[i, j] = new Tile(i - 8, j, _colors[r.Next(_colors.Length)]);
                    registerTileCallback(_board[i, j]);
                }
            }
        }

        private List<Tile> _lastMatches = new List<Tile>();
        private int _points;
        private int _countdown = 60;
        private readonly DispatcherTimer _gameTimer;

        public void TrySwapTiles(
            Tile first, Tile second, Action<Tile, Tile> successAnimCallback,
            Action<Tile, Tile> failAnimCallback)
        {
            if (Math.Abs(first.Top - second.Top) + Math.Abs(first.Left - second.Left) > 1)
            {
                return;
            }

            Utility.Swap(
                ref _board[first.Top + 8, first.Left],
                ref _board[second.Top + 8, second.Left]);
            _lastMatches = CheckMatches();
            if (_lastMatches.Count > 0)
            {
                first.SwapCoordinates(ref second);
                successAnimCallback(first, second);
            }
            else
            {
                Utility.Swap(
                    ref _board[first.Top + 8, first.Left],
                    ref _board[second.Top + 8, second.Left]);
                failAnimCallback(first, second);
            }
        }

        private void DeleteMatches(Action<Tile> unregisterTile)
        {
            foreach (var match in _lastMatches)
            {
                unregisterTile(_board[match.Top + 8, match.Left]);
                _board[match.Top + 8, match.Left] = null;
            }
        }

        public void DeleteAndDropTiles(
            Action<Tile> tileDropAnimation, Action<Tile> registerTile,
            Action<Tile> unregisterTile)
        {
            DeleteMatches(unregisterTile);
            var dropLengths = new int[8];
            for (int i = 16 - 1; i >= 0; i--)
            {
                for (var j = 0; j < 8; j++)
                {
                    if (_board[i, j] == null)
                    {
                        dropLengths[j]++;
                    }
                    else if (dropLengths[j] != 0)
                    {
                        if (_board[i + dropLengths[j], j] != null)
                        {
                            throw new InvalidOperationException(
                                "It is not null where tile is dropped");
                        }

                        Utility.Swap(ref _board[i, j], ref _board[i + dropLengths[j], j]);
                        _board[i + dropLengths[j], j].Top = i + dropLengths[j] - 8;
                        tileDropAnimation(_board[i + dropLengths[j], j]);
                    }
                }
            }

            FillBoard(registerTile);
        }

        private List<Tile> CheckMatches()
        {
            var delete = new bool[16, 8];
            for (var i = 8; i < 16; i++)
            {
                var matches = 1;
                var color = _board[i, 0].Color;
                for (var j = 1; j < 8; j++)
                {
                    if (_board[i, j].Color == color)
                    {
                        ++matches;
                    }
                    else
                    {
                        if (matches >= 3)
                        {
                            for (var k = 1; k < matches + 1; k++)
                            {
                                delete[i, j - k] = true;
                            }
                        }

                        color = _board[i, j].Color;
                        matches = 1;
                    }
                }

                if (matches < 3) continue;
                for (var k = 1; k < matches + 1; k++)
                {
                    delete[i, 8 - k] = true;
                }
            }

            for (var i = 0; i < 8; i++)
            {
                var matches = 1;
                var color = _board[8, i].Color;
                for (var j = 9; j < 16; j++)
                {
                    if (_board[j, i].Color == color)
                    {
                        ++matches;
                    }
                    else
                    {
                        if (matches >= 3)
                        {
                            for (var k = 1; k < matches + 1; k++)
                            {
                                delete[j - k, i] = true;
                            }
                        }

                        color = _board[j, i].Color;
                        matches = 1;
                    }
                }
                
                if (matches < 3) continue;
                for (var k = 1; k < matches + 1; k++)
                {
                    delete[16 - k, i] = true;
                }
            }

            var result = new List<Tile>();
            for (var i = 8; i < 16; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    if (delete[i, j])
                    {
                        result.Add(_board[i, j]);
                    }
                }
            }

            return result;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged(
            [CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}