﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Tetris
{
    public class Board
    {
        private int Rows;
        private int Cols;
        private int Score;
        private int LinesFilled;
        private Tetramino currTetramino;
        private Label[,] BlockControls;
        static private Brush NoBrush = Brushes.Transparent;
        static private Brush SilverBrush = Brushes.Gray;

        public Board(Grid TetrisGrid)
        {
            Rows = TetrisGrid.RowDefinitions.Count;
            Cols = TetrisGrid.ColumnDefinitions.Count;
            Score = 0;
            LinesFilled = 0;
            BlockControls = new Label[Cols, Rows];

            for (int i = 0; i < Cols; i++)
            {
                for (int j = 0; j < Rows; j++)
                {
                    BlockControls[i, j] = new Label();
                    BlockControls[i, j].Background = NoBrush;
                    BlockControls[i, j].BorderBrush = SilverBrush;
                    BlockControls[i, j].BorderThickness = new Thickness(1, 1, 1, 1);
                    Grid.SetRow(BlockControls[i, j], j);
                    Grid.SetColumn(BlockControls[i, j], i);
                    TetrisGrid.Children.Add(BlockControls[i, j]);
                }
            }
            currTetramino = new Tetramino();
            currTetraminoDraw();
        }

        public int getScore()
        {
            return Score;
        }

        public int getLines()
        {
            return LinesFilled;
        }

        private void currTetraminoDraw()
        {
            Point Position = currTetramino.getCurrPosition();
            Point[] Shape = currTetramino.getCurrShape();
            Brush Color = currTetramino.getCurrColor();
            foreach (Point P in Shape)
            {
                BlockControls[(int)(P.X + Position.X) + ((Cols / 2) - 1),
                    (int)(P.Y + Position.Y) + 2].Background = Color;
            }
        }

        private void currTetraminoErase()
        {
            Point Position = currTetramino.getCurrPosition();
            Point[] Shape = currTetramino.getCurrShape();
            foreach (Point P in Shape)
            {
                BlockControls[(int)(P.X + Position.X) + ((Cols / 2) - 1),
                    (int)(P.Y + Position.Y) + 2].Background = NoBrush;
            }
        }

        private void CheckRows()
        {
            bool full;
            for (int i = Rows - 1; i > 0; i--)
            {
                full = true;
                for (int j = 0; j < Cols; j++)
                {
                    if (BlockControls[j, i].Background == NoBrush)
                    {
                        full = false;
                    }
                    if (full)
                    {
                        RemoveRow(i, j);
                        Score += 100;
                        LinesFilled += 1;
                    }
                }
            } 
        }

        private void RemoveRow(int row, int col)
        {
            for (int i = row; i > 2; i--)
            {
                for (int j = 0; j < col; j++)
                {
                    BlockControls[j, i].Background = NoBrush;
                }
            }
        }

        public void CurrentTetraminoMovLeft()
        {
            Point Position = currTetramino.getCurrPosition();
            Point[] Shape = currTetramino.getCurrShape();
            bool move = true;
            currTetraminoErase();
            foreach (Point P in Shape)
            {
                if(((int)(P.X + Position.X) + ((Cols/2)-1)-1) < 0)
                {
                    move = false;
                }
                else if (BlockControls[((int)(P.X + Position.X) + ((Cols / 2) - 1) - 1),
                    (int)(P.Y + Position.Y) +2].Background != NoBrush)
                {
                    move = false;
                }
            }
            if (move)
            {
                currTetramino.movLeft();
                currTetraminoDraw();
            }
            else
            {
                currTetraminoDraw();
            }
        }

        public void CurrentTetraminoMovRight()
        {
            Point Position = currTetramino.getCurrPosition();
            Point[] Shape = currTetramino.getCurrShape();
            bool move = true;
            foreach (Point P in Shape)
            {
                if (((int)(P.X + Position.X) + ((Cols / 2) - 1) + 1) >= Cols)
                {
                    move = false;
                }
                else if (BlockControls[((int)(P.X + Position.X) + ((Cols / 2) - 1) + 1),
                    (int)(P.Y + Position.Y) + 2].Background != NoBrush)
                {
                    move = false;
                }
            }
            if (move)
            {
                currTetramino.movRight();
                currTetraminoDraw();
            }
            else
            {
                currTetraminoDraw();
            }
        }

        public void CurrentTetraminoMovDown()
        {
            Point Position = currTetramino.getCurrPosition();
            Point[] Shape = currTetramino.getCurrShape();
            bool move = true;
            currTetraminoErase();
            foreach (Point P in Shape)
            {
                if (((int)(P.Y + Position.Y) + 2 + 1) >= Rows)
                {
                    move = false;
                }
                else if (BlockControls[((int)(P.X + Position.X) + ((Cols / 2) - 1)),
                    (int)(P.Y + Position.Y) + 2 + 1].Background != NoBrush)
                {
                    move = false;
                }
            }
            if (move)
            {
                currTetramino.movDown();
                currTetraminoDraw();
            }
            else
            {
                currTetraminoDraw();
                CheckRows();
                currTetramino = new Tetramino();
            }
        }

        public void CurrentTetraminoMovRotate()
        {
            Point Position = currTetramino.getCurrPosition();
            Point[] Shape = currTetramino.getCurrShape();
            Point[] S = new Point[4];
            bool move = true;
            Shape.CopyTo(S, 0);
            currTetraminoErase();
            for (int i = 0; i < S.Length; i++)
            {
                double x = S[i].X;
                S[i].X = S[i].Y * -1;
                S[i].Y = x;
                if (((int)((S[i].Y + Position.Y) + 2)) >= Rows)
                {
                    move = false;
                }
                else if (((int)(S[i].X + Position.X) + ((Cols / 2) - 1)) < 0)
                {
                        move = false;
                }
                else if (((int)(S[i].X + Position.X) + ((Cols/2)-1)) >= Cols)
                {
                    move = false;
                }
                else if (BlockControls[((int)(S[i].X + Position.X) + ((Cols/2)-1)),
                    (int)(S[i].Y + Position.Y) +2].Background != NoBrush)
                {
                    move = false;
                }
            }
            if (move)
            {
                currTetramino.movRotate();
                currTetraminoDraw();
            }
            else
            {
                currTetraminoDraw();
            }
        }
    }

    public class Tetramino
    {
        private Point currPosition;
        private Point[] currShape;
        private Brush currColor;
        private bool rotate;
        public Tetramino()
        {
            currPosition = new Point(0, 0);
            currColor = Brushes.Transparent;
            currShape = setRandomShape();
        }

        public Brush getCurrColor()
        {
            return currColor;
        }
        
        public Point getCurrPosition()
        {
            return currPosition;
        }

        public Point[] getCurrShape()
        {
            return currShape;
        }

        public void movLeft()
        {
            currPosition.X -= 1;
        }

        public void movRight()
        {
            currPosition.X += 1;
        }

        public void movDown()
        {
            currPosition.Y -= 1;
        }

        public void movRotate()
        {
            for (int i = 0; i < currShape.Length; i++)
            {
                double x = currShape[i].X;
                currShape[i].X = currShape[i].Y * -1;
                currShape[i].Y = x;
            }
        }

        private Point[] setRandomShape()
        {
            Random rand = new Random();
            switch (rand.Next() % 7)
            {
                case 0: // I
                    rotate = true;
                    currColor = Brushes.Cyan;
                    return new Point[]
                    {
                        new Point(0,0),
                        new Point(-1,0),
                        new Point(1,0),
                        new Point(2,0)
                    };
                case 1: // J
                    rotate = true;
                    currColor = Brushes.Blue;
                    return new Point[]
                    {
                        new Point(1,-1),
                        new Point(-1,0),
                        new Point(0,0),
                        new Point(1,0)
                    };
                case 2: // L
                    rotate = true;
                    currColor = Brushes.Orange;
                    return new Point[]
                    {
                        new Point(0,0),
                        new Point(-1,0),
                        new Point(1,0),
                        new Point(1,1)
                    };
                case 3: // O
                    rotate = false;
                    currColor = Brushes.Yellow;
                    return new Point[]
                    {
                        new Point(0,0),
                        new Point(0,1),
                        new Point(1,0),
                        new Point(1,1)
                    };
                case 4: // S
                    rotate = true;
                    currColor = Brushes.Green;
                    return new Point[]
                    {
                        new Point(1,1),
                        new Point(0,1),
                        new Point(0, 0),
                        new Point(-1,0)
                    };
                case 5: // T
                    rotate = true;
                    currColor = Brushes.Purple;
                    return new Point[]
                    {
                        new Point(0,0),
                        new Point(-1,0),
                        new Point(0,-1),
                        new Point(1,0)
                    };
                case 6: //Z 
                    rotate = true;
                    currColor = Brushes.Red;
                    return new Point[]
                    {
                        new Point(-1,0),
                        new Point(0,0),
                        new Point(0,-1),
                        new Point(1,-1)
                    };
                default:
                    return new Point[0];
            }
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer Timer;
        Board myBoard;
        public MainWindow()
        {
            InitializeComponent();
        }
        void MainWindow_Initialized(object sender, EventArgs e)
        {
            Timer = new DispatcherTimer();
            Timer.Tick += new EventHandler(Timer_Tick);
            Timer.Interval = new TimeSpan(0, 0, 0, 400);
            GameStart();
        }
        private void GameStart()
        {
            MainGrid.Children.Clear();
            myBoard = new Board(MainGrid);
            Timer.Start();
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            Score.Content = myBoard.getScore().ToString("00000000");
            Lines.Content = myBoard.getLines().ToString("00000000");
            myBoard.CurrentTetraminoMovDown();
        }

        private void GamePause()
        {
            if (Timer.IsEnabled) Timer.Stop();
            else Timer.Start();
        }

        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    if (Timer.IsEnabled) myBoard.CurrentTetraminoMovLeft();
                    break;
                case Key.Right:
                    if (Timer.IsEnabled) myBoard.CurrentTetraminoMovRight();
                    break;
                case Key.Down:
                    if (Timer.IsEnabled) myBoard.CurrentTetraminoMovDown();
                    break;
                case Key.Up:
                    if (Timer.IsEnabled) myBoard.CurrentTetraminoMovRotate();
                    break;
                case Key.F2:
                    GameStart();
                    break;
                case Key.F3:
                    GamePause();
                    break;
                default:
                    break;
            }
        }
    }
}
