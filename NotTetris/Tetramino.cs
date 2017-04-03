using System;
using System.Windows;
using System.Windows.Media;

namespace NotTetris
{
    public class Tetramino
    {

        private Point currPosition;
        private Point[] currShape;
        private Brush currColor;
        private bool rotate;
        private string Name;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"> 
        /// i is the shape to spawn
        /// 0. I
        /// 1. J
        /// 2. L
        /// 3. [ ]
        /// 4. S
        /// 5. T
        /// 6. Z
        /// </param>
        public Tetramino(int i)
        {
            currPosition = new Point(0,0);
            currColor = Brushes.Transparent;
            currShape = setShape(i);
        }

        public string getName()
        {
            return Name;
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
             currPosition.Y += 1;
        }

        public void movRotate()
        {
            if (rotate)
            {
                for (int i = 0; i < currShape.Length; i++)
                {
                    double x = currShape[i].X;
                    currShape[i].X = currShape[i].Y*-1;
                    currShape[i].Y = x; 
                }
            }
        }

        private Point[] setShape(int i)
        {

            switch (i)
            {
                
                case 0: // I
                rotate = true;
                currColor = Brushes.Cyan;
                    Name = "I";
                return new Point[]
                    {
                        new Point(0,0), 
                        new Point(-1,0), 
                        new Point(1,0), 
                        new Point(2,0) 
                    };
 
                case 1: //J
                rotate = true;
                currColor = Brushes.Blue;
                    Name = "J";
                return new Point[]
                    {
                        new Point(1,-1), 
                        new Point(-1,0), 
                        new Point(0,0), 
                        new Point(1,0) 
                    };

                case 2: //L
                rotate = true;
                currColor = Brushes.Orange;
                    Name = "L";
                    return new Point[]
                    {
                        new Point(0,0), 
                        new Point(-1,0), 
                        new Point(1,0), 
                        new Point(0,-1) 
                    };

                case 3: // [ ]
                rotate = false;
                currColor = Brushes.Yellow;
                    Name = "[ ]";
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
                    Name = "S";
                    return new Point[]
                    {
                        new Point(0,0), 
                        new Point(-1,0), 
                        new Point(0,-1), 
                        new Point(1,0) 
                    };

                case 5: // T
                rotate = true;
                    currColor = Brushes.Purple;
                    Name = "T";
                    return new Point[]
                    {
                        new Point(0,0), 
                        new Point(-1,0), 
                        new Point(0,-1), 
                        new Point(1,0) 
                    };

                case 6: //z
                rotate = true;
                currColor = Brushes.Red;
                    Name = "Z";
                    return new Point[]
                    {
                        new Point(0,0), 
                        new Point(-1,0), 
                        new Point(0,1), 
                        new Point(1,1) 
                    };
                    

                default:
                    return null; 
                     
            }
        }
    }
}
