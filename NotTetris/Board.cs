using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NotTetris
{
    public class Board
    {
        private int Rows;
        private int Cols;
        private int Score;
        private int LinesFilled;
        private Tetramino currTetramino;
        private Label[,] BlockControls;
        private bool gameOver = false; 

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
                    BlockControls[i,j].BorderThickness = new Thickness(1);
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
            Point position = currTetramino.getCurrPosition();
            Point[] Shape = currTetramino.getCurrShape();
            Brush Color = currTetramino.getCurrColor(); 
            
            foreach (Point S in Shape)
            {
                BlockControls[(int) (S.X + position.X) + ((Cols/2) - 1),
                              (int) (S.Y + position.Y) + 2].Background = Color;
            }
        }

        private void currTretaminoErase()
        {
            Point position = currTetramino.getCurrPosition();
            Point[] Shape = currTetramino.getCurrShape();

            foreach (Point S in Shape)
            {
                BlockControls[(int)(S.X + position.X) + ((Cols / 2) - 1),
                              (int)(S.Y + position.Y) + 2].Background = NoBrush;
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
                }

                if (full)
                {
                    RemoveRow(i);
                    Score += 100;
                    LinesFilled += 1;
                }
            }
        }

        private void RemoveRow(int row)
        {
            for (int i = row; i > 0; i--)
            {
                for (int j = 0; j < Cols; j++)
                {
                    BlockControls[j, i].Background = BlockControls[j, i - 1].Background; 
                }
            }
        }

        public void CurrTetraminoMovLeft()
        {
            Point Position = currTetramino.getCurrPosition();
            Point[] Shape = currTetramino.getCurrShape();
            bool move = true; 

            currTretaminoErase();

            foreach (Point S in Shape)
            {
                if (((int) (S.X + Position.X) + ((Cols/2) - 1) - 1) < 0)
                {
                    move = false; 
                }

                else if (BlockControls[((int)(S.X + Position.X) + ((Cols / 2) - 1) - 1),
                    (int)(S.Y + Position.Y) + 2].Background != NoBrush)
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

        public void CurrTetraminoMovRight()
        {
            Point Position = currTetramino.getCurrPosition();
            Point[] Shape = currTetramino.getCurrShape();
            bool move = true;

            currTretaminoErase();

            foreach (Point S in Shape)
            {
                if (((int)(S.X + Position.X) + ((Cols / 2) - 1) + 1) >= Cols)
                {
                    move = false;
                }

                else if (BlockControls[((int)(S.X + Position.X) + ((Cols / 2) - 1) + 1),
                    (int)(S.Y + Position.Y) + 2].Background != NoBrush)
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

        public void CurrTetraminoMovDown()
        {
            Point Position = currTetramino.getCurrPosition();
            Point[] Shape = currTetramino.getCurrShape();
            bool move = true; 

            currTretaminoErase();

            foreach (Point S in Shape)
            {
                if (((int)(S.Y + Position.Y) + 2 + 1) >= Rows)
                {
                    move = false;
                }

                else if (BlockControls[((int)(S.X + Position.X) + ((Cols / 2) - 1)),
                    (int)(S.Y + Position.Y) + 2 + 1].Background != NoBrush)
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

                if ((int) Position.Y == 0)
                {
                    gameOver = true;
                }

                else
                {
                    currTetramino = new Tetramino();
                }
                
            }
        }

        public void CurrTetraminoMovRotate()
        {
            Point Position = currTetramino.getCurrPosition();
            Point[] S = new Point[4];
            Point[] Shape = currTetramino.getCurrShape();
            bool move = true; 
            Shape.CopyTo(S, 0);
            currTretaminoErase();

            for (int i = 0; i < S.Length; i++)
            {
                double x = S[i].X;
                S[i].X = S[i].Y*-1;
                S[i].Y = x;

                if (((int)((S[i].Y + Position.Y) + 2)) >= Rows)
                {
                    move = false; 
                }

                else if (((int) (S[i].X + Position.X) + ((Cols/2) - 1)) < 0)
                {
                    move = false; 
                }

                else if (((int) (S[i].X + Position.X) + ((Cols/2) - 1)) >= Rows)
                {
                    move = false; 
                }

                else if (BlockControls[((int) (S[i].X + Position.X) + ((Cols/2) - 1)),
                                       (int) (S[i].Y + Position.Y) + 2].Background != NoBrush)
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

        public bool GameOver()
        {
            return gameOver; 
        }

    }
}
