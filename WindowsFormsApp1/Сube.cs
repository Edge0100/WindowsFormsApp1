using System.Drawing;
using static WindowsFormsApp1.Geometry;

namespace WindowsFormsApp1
{
    public class Cube
    {
        

        Rectangle cubeSide;
        Pen blackPen;
        Pen transparentRedPen;
        Pen redPen;
        Pen whitePen;
        Pen greenPen;
        Pen bluePen;
        Pen lightGreenPen;
        const int xOffset = 5, yOffset = 5, cubeSize = 20;
        
        public Cube(int startMapX, int startMapY)
        {
            X = startMapX;
            Y = startMapY;

            cubeSide = new Rectangle();
            
            cubeSide.X = Form1.xOffset + xOffset + Y * Form1.tileSize;
            cubeSide.Y = Form1.yOffset + yOffset + X * Form1.tileSize;

            cubeSide.Width = cubeSide.Height = cubeSize;

            

            RedSidePos = Sides.Bottom;

            transparentRedPen = new Pen(Color.FromArgb(127, 127, 0, 0));
            redPen = new Pen(Color.Red);
            redPen.Width = 4;
            blackPen = new Pen(Color.Black);
            whitePen = new Pen(Color.White);
            greenPen = new Pen(Color.Green);
            bluePen = new Pen(Color.Blue);
            lightGreenPen = new Pen(Color.LightGreen);
        }
        public int Gx { get; set; }
        public Sides RedSidePos
        {
            get;set;
        }
        public int X
        {
            get;set;
        }
        public int Y
        {
            get;set;
        }
        public bool RedSideIsOnBottom()
        {
            return RedSidePos == Sides.Bottom;
        }
        public void ClearCube(Graphics graphics)
        {
            switch (Form1.GetTileInfo(X,Y))
            {
                case Status.Alive:
                    graphics.FillRectangle(whitePen.Brush, cubeSide.X - 1, cubeSide.Y - 1, cubeSide.Width + 2, cubeSide.Height + 2);
                    break;

                case Status.Cens:
                    graphics.FillRectangle(lightGreenPen.Brush, cubeSide.X - 1, cubeSide.Y - 1, cubeSide.Width + 2, cubeSide.Height + 2);
                    break;
                case Status.Start:
                    graphics.FillRectangle(bluePen.Brush, cubeSide.X - 1, cubeSide.Y - 1, cubeSide.Width + 2, cubeSide.Height + 2);
                    break;
                case Status.Winner:
                    graphics.FillRectangle(greenPen.Brush, cubeSide.X - 1, cubeSide.Y - 1, cubeSide.Width + 2, cubeSide.Height + 2);
                    break;
            }
            
            
        }
        public void ClearCube(Graphics graphics, Pen pen)
        {

            graphics.FillRectangle(pen.Brush, cubeSide.X - 1, cubeSide.Y - 1, cubeSide.Width + 2, cubeSide.Height + 2);

        }
        public void DrawCube(Graphics graphics)
        {
            int half = cubeSize / 2;
            switch (RedSidePos)
            {
                case Sides.Top:
                    {
                        graphics.FillRectangle(redPen.Brush, cubeSide);

                        break;
                    }
                case Sides.Bottom:
                    {
                        graphics.FillRectangle(transparentRedPen.Brush, cubeSide);
                        break;
                    }
                case Sides.Left:
                    {
                        graphics.DrawRectangle(blackPen, cubeSide);
                        graphics.DrawLine(redPen, cubeSide.X, cubeSide.Y + half, cubeSide.X + half, cubeSide.Y + half);
                        break;
                    }
                case Sides.Front:
                    {
                        graphics.DrawRectangle(blackPen, cubeSide);
                        graphics.DrawLine(redPen, cubeSide.X + half, cubeSide.Y, cubeSide.X + half, cubeSide.Y + half);
                        break;
                    }
                case Sides.Right:
                    {
                        graphics.DrawRectangle(blackPen, cubeSide);
                        graphics.DrawLine(redPen, cubeSide.X + half, cubeSide.Y + half, cubeSide.X + 2 * half, cubeSide.Y + half);
                        break;
                    }
                case Sides.Back:
                    {
                        graphics.DrawRectangle(blackPen, cubeSide);
                        graphics.DrawLine(redPen, cubeSide.X + half, cubeSide.Y + half, cubeSide.X + half, cubeSide.Y + 2 * half);
                        break;
                    }
            }
        }
        public void MoveCube(Directions direction)
        {
            
            switch (direction)
            {
                case Directions.West:
                    {
                        Y--;
                        break;
                    }
                case Directions.North:
                    {
                        X--;
                        break;
                    }
                case Directions.East:
                    {
                        Y++;
                        break;
                    }
                case Directions.South:
                    {
                        X++;
                        break;
                    }
                
            }
            cubeSide.X = Form1.xOffset + xOffset + Y * Form1.tileSize;
            cubeSide.Y = Form1.yOffset + yOffset + X * Form1.tileSize;
            switch (RedSidePos)
            {
                case Sides.Top:
                    {
                        switch (direction)
                        {
                            case Directions.West:
                                {
                                    RedSidePos = Sides.Left;
                                    break;
                                }
                            case Directions.North:
                                {
                                    RedSidePos = Sides.Front;
                                    break;
                                }
                            case Directions.East:
                                {
                                    RedSidePos = Sides.Right;
                                    break;
                                }
                            case Directions.South:
                                {
                                    RedSidePos = Sides.Back;
                                    break;
                                }
                        }

                        break;
                    }
                case Sides.Bottom:
                    {
                        switch (direction)
                        {
                            case Directions.West:
                                {
                                    RedSidePos = Sides.Right;
                                    break;
                                }
                            case Directions.North:
                                {
                                    RedSidePos = Sides.Back;
                                    break;
                                }
                            case Directions.East:
                                {
                                    RedSidePos = Sides.Left;
                                    break;
                                }
                            case Directions.South:
                                {
                                    RedSidePos = Sides.Front;
                                    break;
                                }
                        }
                        break;
                    }
                case Sides.Left:
                    {
                        switch (direction)
                        {
                            case Directions.West:
                                {
                                    RedSidePos = Sides.Bottom;
                                    break;
                                }

                            case Directions.East:
                                {
                                    RedSidePos = Sides.Top;
                                    break;
                                }

                        }
                        break;
                    }
                case Sides.Front:
                    {
                        switch (direction)
                        {
                            case Directions.North:
                                {
                                    RedSidePos = Sides.Bottom;
                                    break;
                                }

                            case Directions.South:
                                {
                                    RedSidePos = Sides.Top;
                                    break;
                                }

                        }
                        break;
                    }
                case Sides.Right:
                    {
                        switch (direction)
                        {
                            case Directions.East:
                                {
                                    RedSidePos = Sides.Bottom;
                                    break;
                                }

                            case Directions.West:
                                {
                                    RedSidePos = Sides.Top;
                                    break;
                                }

                        }
                        break;
                    }
                case Sides.Back:
                    {
                        switch (direction)
                        {
                            case Directions.South:
                                {
                                    RedSidePos = Sides.Bottom;
                                    break;
                                }

                            case Directions.North:
                                {
                                    RedSidePos = Sides.Top;
                                    break;
                                }

                        }
                        break;
                    }
            }

        }
        public void MoveCube(int x, int y, Sides redSidePos)
        {
            X = x;
            Y = y;
            RedSidePos = redSidePos;
            cubeSide.X = Form1.xOffset + xOffset + Y * Form1.tileSize;
            cubeSide.Y = Form1.yOffset + yOffset + X * Form1.tileSize;
        }

    }
}
