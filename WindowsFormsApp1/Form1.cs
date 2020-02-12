using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
   public enum Directions
    {
        West,
        North,
        East,
        South
    }
    public enum Status
    {
        Dead,
        Alive,
        Winner
    }
    
    

    public partial class Form1 : Form
    {
        string[]        mapData;
        Rectangle[][]   mapGraphics;

        public 
        const int   xOffset = 20, 
                    yOffset = 20,
                    tileSize = 30;
        
        Graphics    graphics;
        Pen         blackPen;
        
        Pen         greenPen;

        Cube        cube;
        public Form1()
        {
            InitializeComponent();
        }
        
        public Status getTileInfo(int i, int j)
        {
            switch (mapData[j][i])
            {
                case '1': return Status.Alive;
                case '2': return Status.Winner;
                default:  return Status.Dead;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.Left:
                    {
                        cube.MoveCube(Directions.West);
                        break;
                    }
                case Keys.Up:
                    {
                        cube.MoveCube(Directions.North);
                        break;
                    }
                case Keys.Right:
                    {
                        cube.MoveCube(Directions.East);
                        break;
                    }
                case Keys.Down:
                    {
                        cube.MoveCube(Directions.South);
                        break;
                    }
            }
            if (getTileInfo(cube.getMapX(), cube.getMapY()) == Status.Dead) this.Close();
            graphics.Clear(Color.White);
            DrawMap();
            cube.DrawCube(graphics);

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void DrawMap()
        {
            for (int i = 0; i < mapData.Length; i++)
            {
                for (int j = 0; j < mapData[i].Length; j++)
                {
                    if (mapData[i][j] == '0') continue;
                    else if (mapData[i][j] == '1') graphics.DrawRectangle(blackPen, mapGraphics[i][j]);
                    else graphics.FillRectangle(greenPen.Brush, mapGraphics[i][j]);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            blackPen = new Pen(Color.Black);
            
            greenPen = new Pen(Color.Green);

            graphics = CreateGraphics();

            mapData = File.ReadAllLines("C:/Users/admin/Desktop/WindowsFormsApp1/WindowsFormsApp1/Новый текстовый документ.txt");
            mapGraphics = new Rectangle[mapData.Length][];

            for (int i=0; i<mapData.Length;i++)
            {
                mapData[i] = mapData[i].Replace(" ","");
                mapGraphics[i] = new Rectangle[mapData[i].Length];

               for(int j=0; j<mapData[i].Length;j++)
                {
                    mapGraphics[i][j].X = xOffset + j * tileSize;
                    mapGraphics[i][j].Y = yOffset + i * tileSize;
                    mapGraphics[i][j].Width = mapGraphics[i][j].Height = tileSize;
                    if (mapData[i][j] == '0') continue;
                    else if (mapData[i][j] == '1') graphics.DrawRectangle(blackPen, mapGraphics[i][j]);
                    else graphics.FillRectangle(greenPen.Brush, mapGraphics[i][j]);
                }
            }

            cube = new Cube(1, 1);
            cube.DrawCube(graphics);
            
        }
    }
    public class Cube
    {
        enum Sides
        {
            Left,
            Right,
            Front,
            Back,
            Top,
            Bottom
        }

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
        public int getMapX()
        {
            return x;
        }
        public int getMapY()
        {
            return y;
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
