using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
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
        private System.Windows.Controls.Label[,] SpielRaster;
        static private Brush NoBrush = Brushes.Transparent;
        static private Brush SilverBrush = Brushes.Gray;

        // Konstruktor
        public Board(Grid TetrisGrid)
        {
            Rows = TetrisGrid.RowDefinitions.Count;
            Cols = TetrisGrid.ColumnDefinitions.Count;
            Score = 0;
            LinesFilled = 0;
            SpielRaster = new System.Windows.Controls.Label[Cols, Rows];

            for (int i = 0; i < Cols; i++)
            {
                for (int j = 0; j < Rows; j++)
                {
                    SpielRaster[i, j] = new System.Windows.Controls.Label();
                    SpielRaster[i, j].Background = NoBrush;
                    SpielRaster[i, j].BorderBrush = SilverBrush;
                    SpielRaster[i, j].BorderThickness = new Thickness(1, 1, 1, 1);
                    Grid.SetRow(SpielRaster[i, j], j);
                    Grid.SetColumn(SpielRaster[i, j], i);
                    TetrisGrid.Children.Add(SpielRaster[i, j]);
                }
            }
            currTetramino = new Tetramino();
            currTetraminoDraw();
        }

        // Getter
        public int getScore()
        {
            return Score;
        }

        public int getLines()
        {
            return LinesFilled;
        }

        // Methoden
        private void currTetraminoDraw()
        {
            Point Position = currTetramino.getCurrPosition();
            Point[] Shape = currTetramino.getCurrShape();
            Brush Color = currTetramino.getCurrColor();
            foreach (Point P in Shape)
            {
                SpielRaster[(int)(P.X + Position.X) + ((Cols / 2) - 1),
                    (int)(P.Y + Position.Y) + 2].Background = Color;
            }
        }

        private void currTetraminoErase()
        {
            Point Position = currTetramino.getCurrPosition();
            Point[] Shape = currTetramino.getCurrShape();
            foreach (Point P in Shape)
            {
                SpielRaster[(int)(P.X + Position.X) + ((Cols / 2) - 1),
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
                    if (SpielRaster[j, i].Background == NoBrush)
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
                    SpielRaster[j, i].Background = NoBrush;
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
                if (((int)(P.X + Position.X) + ((Cols / 2) - 1) - 1) < 0)
                {
                    move = false;
                }
                else if (SpielRaster[((int)(P.X + Position.X) + ((Cols / 2) - 1) - 1),
                    (int)(P.Y + Position.Y) + 2].Background != NoBrush)
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
            currTetraminoErase();
            foreach (Point P in Shape)
            {
                if (((int)(P.X + Position.X) + ((Cols / 2) - 1) + 1) >= Cols)
                {
                    move = false;
                }
                else if (SpielRaster[((int)(P.X + Position.X) + ((Cols / 2) - 1) + 1),
                    (int)(P.Y + Position.Y) + 4].Background != NoBrush)
                {
                    //System.Windows.MessageBox.Show("Aktueller Punkt X =" + P.X.ToString() + "\nAktuelle Position des Steins:" + Position.X.ToString());
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
                else if (SpielRaster[((int)(P.X + Position.X) + ((Cols / 2) - 1)),
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
                else if (((int)(S[i].X + Position.X) + ((Cols / 2) - 1)) >= Cols)
                {
                    move = false;
                }
                else if (SpielRaster[((int)(S[i].X + Position.X) + ((Cols / 2) - 1)),
                    (int)(S[i].Y + Position.Y) + 2].Background != NoBrush)
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
}
