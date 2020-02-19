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
        Sides redSide;
        const int xOffset = 5, yOffset = 5, cubeSize = 20;
        int x, y;
        public Cube(int startMapX, int startMapY)
        {
            x = startMapX;
            y = startMapY;

            cubeSide = new Rectangle();

            cubeSide.X = Form1.xOffset + xOffset + y * Form1.tileSize;
            cubeSide.Y = Form1.yOffset + yOffset + x * Form1.tileSize;

            cubeSide.Width = cubeSide.Height = cubeSize;

            redSide = Sides.Bottom;

            transparentRedPen = new Pen(Color.FromArgb(127, 127, 0, 0));
            redPen = new Pen(Color.Red);
            redPen.Width = 4;
            blackPen = new Pen(Color.Black);
        }
        public Sides GetRedSidePos()
        {
            return redSide;
        }
        public int getMapX()
        {
            return x;
        }
        public int getMapY()
        {
            return y;
        }
        public bool RedSideIsOnBottom()
        {
            return redSide == Sides.Bottom;
        }
        public void DrawCube(Graphics graphics)
        {
            int half = cubeSize / 2;
            switch (redSide)
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
                        x--;
                        break;
                    }
                case Directions.North:
                    {
                        y--;
                        break;
                    }
                case Directions.East:
                    {
                        x++;
                        break;
                    }
                case Directions.South:
                    {
                        y++;
                        break;
                    }
            }
            cubeSide.X = Form1.xOffset + xOffset + x * Form1.tileSize;
            cubeSide.Y = Form1.yOffset + yOffset + y * Form1.tileSize;
            switch (redSide)
            {
                case Sides.Top:
                    {
                        switch (direction)
                        {
                            case Directions.West:
                                {
                                    redSide = Sides.Left;
                                    break;
                                }
                            case Directions.North:
                                {
                                    redSide = Sides.Front;
                                    break;
                                }
                            case Directions.East:
                                {
                                    redSide = Sides.Right;
                                    break;
                                }
                            case Directions.South:
                                {
                                    redSide = Sides.Back;
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
                                    redSide = Sides.Right;
                                    break;
                                }
                            case Directions.North:
                                {
                                    redSide = Sides.Back;
                                    break;
                                }
                            case Directions.East:
                                {
                                    redSide = Sides.Left;
                                    break;
                                }
                            case Directions.South:
                                {
                                    redSide = Sides.Front;
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
                                    redSide = Sides.Bottom;
                                    break;
                                }

                            case Directions.East:
                                {
                                    redSide = Sides.Top;
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
                                    redSide = Sides.Bottom;
                                    break;
                                }

                            case Directions.South:
                                {
                                    redSide = Sides.Top;
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
                                    redSide = Sides.Bottom;
                                    break;
                                }

                            case Directions.West:
                                {
                                    redSide = Sides.Top;
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
                                    redSide = Sides.Bottom;
                                    break;
                                }

                            case Directions.North:
                                {
                                    redSide = Sides.Top;
                                    break;
                                }

                        }
                        break;
                    }
            }

        }


    }
}
