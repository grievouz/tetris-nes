using System;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Tetris.Controls;

namespace Tetris
{
    internal class Game
    {
        private MediaPlayer _backPlayer = new MediaPlayer();
        private Board _myBoard;
        private int _tickSpeed = 400;

        private DispatcherTimer _timer;
        public TetrisGame TetrisGame;

        public Game(TetrisGame tetrisGame)
        {
            _backPlayer.Open(new Uri("sounds/background.wav", UriKind.Relative));
            _backPlayer.Volume = 0.3;
            _backPlayer.MediaEnded += backPlayer_Ended;
            _backPlayer.Play();

            TetrisGame = tetrisGame;
            _timer = new DispatcherTimer();
            _timer.Tick += GameTick;
            _timer.Interval = new TimeSpan(0, 0, 0, 0, _tickSpeed);
            GameStart();
        }

        public void SetTickSpeed(int milliseconds)
        {
            _tickSpeed = milliseconds;
        }

        public void GameStart()
        {
            TetrisGame.GameGrid.Children.Clear();
            _myBoard = new Board(TetrisGame.GameGrid, this);
            _timer.Start();
        }

        private void GameTick(object sender, EventArgs e)
        {
            TetrisGame.ScoreLabel.Content = _myBoard.GetScore().ToString("000000");
            TetrisGame.TopLabel.Content = (int.Parse(TetrisGame.TopLabel.Content.ToString()) > _myBoard.GetScore() ? TetrisGame.TopLabel.Content : _myBoard.GetScore().ToString("000000"));
            TetrisGame.LinesLabel.Content = _myBoard.GetLines().ToString("00");
            _myBoard.CurrTetraminoMovDown();
            if (CheckLevelUp())
                LevelUp(_myBoard.GetLines() % 1 + 1);
        }

        private bool CheckLevelUp()
        {
            var level = int.Parse(TetrisGame.LevelLabel.Content.ToString());

            if (level != _myBoard.GetLines() % 1)
            {
                TetrisGame.LevelLabel.Content = (level + 1).ToString("0000000");
                return true;
            }

            return false;
        }

        private void LevelUp(int currlevel)
        {
            _timer = new DispatcherTimer();
            _timer.Tick += GameTick;
            _timer.Interval = new TimeSpan(0, 0, 0, 0, _tickSpeed / (5 + currlevel));
        }

        private void GameToggle()
        {
            if (_timer.IsEnabled)
                _timer.Stop();
            else
                _timer.Start();
        }

        public void KeyHandler(KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Left) && Keyboard.IsKeyDown(Key.Down))
            {
                _myBoard.CurrTetraminoMovLeft();
                _myBoard.CurrTetraminoMovDown();
            }
            else if (Keyboard.IsKeyDown(Key.Right) && Keyboard.IsKeyDown(Key.Down))
            {
                _myBoard.CurrTetraminoMovRight();
                _myBoard.CurrTetraminoMovDown();
            }
            else if (Keyboard.IsKeyDown(Key.Up) && Keyboard.IsKeyDown(Key.Down))
            {
                _myBoard.CurrTetraminoMovRotate();
                _myBoard.CurrTetraminoMovDown();
            }
            else
            {
                switch (e.Key)
                {
                    case Key.Left:
                        if (_timer.IsEnabled) _myBoard.CurrTetraminoMovLeft();
                        break;
                    case Key.Right:
                        if (_timer.IsEnabled) _myBoard.CurrTetraminoMovRight();
                        break;
                    case Key.Down:
                        if (_timer.IsEnabled) _myBoard.CurrTetraminoMovDown();
                        break;
                    case Key.Up:
                        if (_timer.IsEnabled) _myBoard.CurrTetraminoMovRotate();
                        break;
                    case Key.S:
                        GameStart();
                        break;
                    case Key.P:
                        GameToggle();
                        break;
                }
            }

        }

        private void backPlayer_Ended(object sender, EventArgs e)
        {
            _backPlayer.Open(new Uri("sounds/background.wav", UriKind.Relative));
            _backPlayer.Play();
        }
    }

    internal class Board
    {
        private static readonly Brush _noBrush = Brushes.Transparent;
        private readonly Label[,] _blockControls;
        private readonly int _cols;

        private Tetramino _currTetramino;
        private int _linesFilled;
        private readonly Game _myGame;
        private readonly int _rows;
        private int _score;

        public void CheckDeath()
        {
            foreach(var pos in _currTetramino.GetCurrShape())
            {
                int x = (int)pos.X + (int)_currTetramino.GetCurrPosition().X + 4;
                int y = (int)pos.Y + (int)_currTetramino.GetCurrPosition().Y + 2;

                if (!Equals(_blockControls[x, y].Background, _noBrush))
                    _myGame.GameStart();
            }
        }

        public Board(Grid tetrisGrid, Game myGame)
        {
            _myGame = myGame;
            _rows = tetrisGrid.RowDefinitions.Count;
            _cols = tetrisGrid.ColumnDefinitions.Count;
            _score = 0;
            _linesFilled = 0;

            _blockControls = new Label[_cols, _rows];

            for (var i = 0; i < _cols; i++)
            for (var j = 0; j < _rows; j++)
            {
                _blockControls[i, j] = new Label();
                _blockControls[i, j].Background = _noBrush;
                _blockControls[i, j].BorderBrush = Brushes.Black;
                _blockControls[i, j].BorderThickness = new Thickness(1, 1, 1, 1);
                Grid.SetRow(_blockControls[i, j], j);
                Grid.SetColumn(_blockControls[i, j], i);
                tetrisGrid.Children.Add(_blockControls[i, j]);
            }

            _currTetramino = new Tetramino();
            CheckDeath();

            CurrTetraminoDraw();
        }

        public int GetScore()
        {
            return _score;
        }

        public int GetLines()
        {
            return _linesFilled;
        }

        private void CurrTetraminoDraw()
        {
            var position = _currTetramino.GetCurrPosition();
            var shape = _currTetramino.GetCurrShape();

            foreach (var s in shape)
                _blockControls[(int) (s.X + position.X) + (_cols / 2 - 1), (int) (s.Y + position.Y) + 2].Background =
                    _currTetramino.GetCurrColor();
        }

        private void CurrTetraminoErase()
        {
            var position = _currTetramino.GetCurrPosition();
            var shape = _currTetramino.GetCurrShape();

            foreach (var S in shape)
                _blockControls[(int) (S.X + position.X) + (_cols / 2 - 1), (int) (S.Y + position.Y) + 2].Background =
                    _noBrush;
        }

        private void CheckRows()
        {
            bool full;
            var holder = 0;
            for (var i = _rows - 1; i > 0; i--)
            {
                full = true;
                for (var j = 0; j < _cols; j++)
                    if (_blockControls[j, i].Background.Equals(_noBrush))
                        full = false;

                if (full)
                {
                    holder++;
                    RemoveRow(i);
                    if (holder == 1)
                        _score += 50 * int.Parse(_myGame.TetrisGame.LevelLabel.Content.ToString() + 1);
                    if (holder == 2)
                        _score += 150 * int.Parse(_myGame.TetrisGame.LevelLabel.Content.ToString() + 1);
                    if (holder == 3)
                        _score += 350 * int.Parse(_myGame.TetrisGame.LevelLabel.Content.ToString() + 1);
                    if (holder >= 4)
                        _score += 1000 * int.Parse(_myGame.TetrisGame.LevelLabel.Content.ToString() + 1);
                    i++;
                }
            }
            _linesFilled += holder;
        }

        private void RemoveRow(int row)
        {
            for (var i = row; i > 2; i--)
            for (var j = 0; j < _cols; j++)
                _blockControls[j, i].Background = _blockControls[j, i - 1].Background;
        }

        public void CurrTetraminoMovLeft()
        {
            var position = _currTetramino.GetCurrPosition();
            var shape = _currTetramino.GetCurrShape();
            var move = true;

            CurrTetraminoErase();

            foreach (var s in shape)
                if ((int) (s.X + position.X) + (_cols / 2 - 1) - 1 < 0)
                    move = false;
                else if (!_blockControls[(int) (s.X + position.X) + (_cols / 2 - 1) - 1, (int) (s.Y + position.Y) + 2]
                    .Background.Equals(_noBrush))
                    move = false;

            if (move)
            {
                MoveDirectionSound();
                _currTetramino.MovLeft();
                CurrTetraminoDraw();
            }
            else
            {
                CurrTetraminoDraw();
            }
        }

        public void CurrTetraminoMovRight()
        {
            var position = _currTetramino.GetCurrPosition();
            var shape = _currTetramino.GetCurrShape();
            var move = true;

            CurrTetraminoErase();

            foreach (var s in shape)
                if ((int) (s.X + position.X) + (_cols / 2 - 1) + 1 >= _cols)
                    move = false;
                else if (!_blockControls[(int) (s.X + position.X) + (_cols / 2 - 1) + 1, (int) (s.Y + position.Y) + 2]
                    .Background.Equals(_noBrush))
                    move = false;

            if (move)
            {
                MoveDirectionSound();
                _currTetramino.MovRight();
                CurrTetraminoDraw();
            }
            else
            {
                CurrTetraminoDraw();
            }
        }

        public void CurrTetraminoMovDown()
        {
            var position = _currTetramino.GetCurrPosition();
            var shape = _currTetramino.GetCurrShape();
            var move = true;

            CurrTetraminoErase();

            foreach (var s in shape)
                if ((int) (s.Y + position.Y) + 2 + 1 >= _rows)
                    move = false;
                else if (!_blockControls[(int) (s.X + position.X) + (_cols / 2 - 1), (int) (s.Y + position.Y) + 2 + 1]
                    .Background.Equals(_noBrush))
                    move = false;

            if (move)
            {
                _currTetramino.MovDown();
                CurrTetraminoDraw();
            }
            else
            {
                CurrTetraminoDraw();
                CheckRows();
                _currTetramino = new Tetramino();
                CheckDeath();
            }
        }

        private void MoveDirectionSound()
        {
            Stream str = Properties.Resources.move;
            new SoundPlayer(str).Play();
        }

        private void MoveRotateSound()
        {
            Stream str = Properties.Resources.rotate;
            new SoundPlayer(str).Play();
        }

        public void CurrTetraminoMovRotate()
        {
            var position = _currTetramino.GetCurrPosition();
            var shape = _currTetramino.GetCurrShape();
            var s = new Point[4];
            var move = true;
            shape.CopyTo(s, 0);

            CurrTetraminoErase();

            for (var i = 0; i < s.Length; i++)
            {
                var x = s[i].X;
                s[i].X = s[i].Y * -1;
                s[i].Y = x;

                if ((int) (s[i].Y + position.Y + 2) >= _rows)
                    move = false;
                else if ((int) (s[i].X + position.X) + (_cols / 2 - 1) < 0)
                    move = false;
                else if ((int) (s[i].X + position.X) + (_cols / 2 - 1) >= _rows)
                    move = false;
                else if ((int) (s[i].X + position.X) + (_cols / 2 - 1) >= 10)
                    move = false;
                else if (!_blockControls[(int) (s[i].X + position.X) + (_cols / 2 - 1), (int) (s[i].Y + position.Y) + 2]
                    .Background.Equals(_noBrush))
                    move = false;
            }

            if (move)
            {
                MoveRotateSound();
                _currTetramino.MovRotate();
                CurrTetraminoDraw();
            }
            else
            {
                CurrTetraminoDraw();
            }
        }
    }

    internal class Tetramino
    {
        private Brush _currColor;
        private Point _currPosition;
        private readonly Point[] _currShape;
        private bool _rotate;


        public Tetramino()
        {
            _currPosition = new Point(0, 2);
            _currColor = Brushes.Transparent;
            _currShape = SetRandomShape();
        }

        public Brush GetCurrColor()
        {
            return _currColor;
        }

        public Point GetCurrPosition()
        {
            return _currPosition;
        }

        public Point[] GetCurrShape()
        {
            return _currShape;
        }

        public void MovLeft()
        {
            _currPosition.X -= 1;
        }

        public void MovRight()
        {
            _currPosition.X += 1;
        }

        public void MovDown()
        {
            _currPosition.Y += 1;
        }

        public void MovRotate()
        {
            if (_rotate)
                for (var i = 0; i < _currShape.Length; i++)
                {
                    var x = _currShape[i].X;
                    _currShape[i].X = _currShape[i].Y * -1;
                    _currShape[i].Y = x;
                }
        }

        private Random _rnd = new Random();

        private Point[] SetRandomShape()
        {

            switch (_rnd.Next() % 7)
            {
                case 0: // I
                    _rotate = true;
                    _currColor = Brushes.Cyan;
                    return new[]
                    {
                        new Point(0, 0),
                        new Point(-1, 0),
                        new Point(1, 0),
                        new Point(2, 0)
                    };

                case 1: // J
                    _rotate = true;
                    _currColor = Brushes.Blue;
                    return new[]
                    {
                        new Point(1, 0),
                        new Point(-1, 1),
                        new Point(0, 1),
                        new Point(1, 1)
                    };

                case 2: // T
                    _rotate = true;
                    _currColor = Brushes.Purple;
                    return new[]
                    {
                        new Point(0, 0),
                        new Point(-1, 0),
                        new Point(0, -1),
                        new Point(1, 0)
                    };

                case 3: // L
                    _rotate = true;
                    _currColor = Brushes.Orange;
                    return new[]
                    {
                        new Point(0, -1),
                        new Point(-1, -1),
                        new Point(1, -1),
                        new Point(1, 0)
                    };

                case 4: // S
                    _rotate = true;
                    _currColor = Brushes.Green;
                    return new[]
                    {
                        new Point(0, -1),
                        new Point(0, 0),
                        new Point(1, -1),
                        new Point(-1, 0)
                    };

                case 5: // Z
                    _rotate = true;
                    _currColor = Brushes.Red;
                    return new[]
                    {
                        new Point(0, -1),
                        new Point(-1, -1),
                        new Point(0, 0),
                        new Point(1, 0)
                    };

                case 6: // O
                    _rotate = false;
                    _currColor = Brushes.Yellow;
                    return new[]
                    {
                        new Point(0, -1),
                        new Point(0, 0),
                        new Point(1, -1),
                        new Point(1, 0)
                    };

                default:
                    return null;
            }
        }
    }
}