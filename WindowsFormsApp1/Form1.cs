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
using static WindowsFormsApp1.Geometry;

namespace WindowsFormsApp1
{
   
    
    

    public partial class Form1 : Form
    {
        string[]        mapData;
        Rectangle[][]   mapGraphics;
        List<Node> States = new List<Node>();
        Queue<Node> OpenNodes = new Queue<Node>();
        Queue<Node> ClosedNodes = new Queue<Node>();


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
        
        public Status GetTileInfo(int i, int j)
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
                case Keys.Enter:
                    {
                        States.Find(x =>
                        {
                            x.X == cube.getMapX(),
                            x.Y == cube.getMapY()
                            });
                        break;
                    }

            }
            var a = GetTileInfo(cube.getMapX(), cube.getMapY());
            if (a == Status.Dead||(cube.RedSideIsOnBottom()&&a==Status.Winner)) this.Close();
            else
            {
                graphics.Clear(Color.White);
                DrawMap();
                cube.DrawCube(graphics);
            }

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

            mapData = File.ReadAllLines("../../Новый текстовый документ.txt");
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
                    else if (mapData[i][j] == '1')
                    {
                        graphics.DrawRectangle(blackPen, mapGraphics[i][j]);
                        States.Add(new Node(i, j, Sides.Back));
                        States.Add(new Node(i, j, Sides.Bottom));
                        States.Add(new Node(i, j, Sides.Front));
                        States.Add(new Node(i, j, Sides.Left));
                        States.Add(new Node(i, j, Sides.Right));
                        States.Add(new Node(i, j, Sides.Top));
                    }
                    else graphics.FillRectangle(greenPen.Brush, mapGraphics[i][j]);
                }
            }

            cube = new Cube(1, 1);
            cube.DrawCube(graphics);
            
        }
    }
    
}
