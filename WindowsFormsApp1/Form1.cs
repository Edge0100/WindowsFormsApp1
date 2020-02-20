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
        Queue<Node> OpenNodes = new Queue<Node>();
        Queue<Node> ClosedNodes = new Queue<Node>();
        Stack<Directions> resultMoves= new Stack<Directions>();

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
        
        private Status GetTileInfo(int i, int j)
        {
            switch (mapData[j][i])
            {
                case '1': return Status.Alive;
                case '2': return Status.Winner;
                default:  return Status.Dead;
            }
        }

        private bool AddNodeToOpenQueue(Node parent, Directions moveDirection)
        {
            var info = GetTileInfo(cube.X, cube.Y);

            if (info == Status.Alive)
            {
                if (
                        !(
                        OpenNodes.Any(x => x.X == cube.X && x.Y == cube.Y && x.RedSidePos == cube.RedSidePos) ||
                        ClosedNodes.Any(x => x.X == cube.X && x.Y == cube.Y && x.RedSidePos == cube.RedSidePos)
                         )
                      )
                {
                    OpenNodes.Enqueue(new Node(cube.X, cube.Y, cube.RedSidePos, parent, moveDirection));

                }
            }
            else if (info == Status.Winner && cube.RedSideIsOnBottom())
            {
                resultMoves.Push(moveDirection);
                Node curr = parent;

                do
                {
                    resultMoves.Push(curr.InitMove);
                    curr = curr.Parent;
                } while (curr.Parent!= null);
                return true;
            }
            return false;
        }
        private bool AI()
        {
            Node curr;
            bool end;
            while(OpenNodes.Count>0)
            {
                curr = OpenNodes.Dequeue();

                cube.X = curr.X;
                cube.Y = curr.Y;
                cube.RedSidePos = curr.RedSidePos;

                cube.MoveCube(Directions.West);
                if (AddNodeToOpenQueue(curr, Directions.West)) return true;
                cube.MoveCube(Directions.East);

                cube.MoveCube(Directions.North);
                if (AddNodeToOpenQueue(curr, Directions.North)) return true;
                cube.MoveCube(Directions.South);

                cube.MoveCube(Directions.East);
                if(AddNodeToOpenQueue(curr, Directions.East)) return true;
                cube.MoveCube(Directions.West);

                cube.MoveCube(Directions.South);
                if(AddNodeToOpenQueue(curr, Directions.South))return true;
                cube.MoveCube(Directions.North);



                ClosedNodes.Enqueue(curr);
            }
            return false;
        }

        private void CommandChainExecution()
        {
            do
            {
                cube.MoveCube(resultMoves.Pop());
                System.Threading.Thread.Sleep(1000);
                graphics.Clear(Color.White);
                DrawMap();
                cube.DrawCube(graphics);

            } while (resultMoves.Count > 0);
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
                        OpenNodes.Enqueue(new Node(cube.X, cube.Y, cube.RedSidePos,null));
                        if (AI()) 
                        { 
                            cube.X = ClosedNodes.Peek().X;
                            cube.Y = ClosedNodes.Peek().Y;
                            cube.RedSidePos = ClosedNodes.Peek().RedSidePos;
                            CommandChainExecution();
                                }
                        OpenNodes.Clear();
                        ClosedNodes.Clear();
                        resultMoves.Clear();
                        
                        break;
                    }

            }
            var a = GetTileInfo(cube.X, cube.Y);
            if (a == Status.Dead/*||(cube.RedSideIsOnBottom()&&a==Status.Winner)*/) this.Close();
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

            mapData = File.ReadAllLines("Новый текстовый документ.txt");
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

                    }
                    else graphics.FillRectangle(greenPen.Brush, mapGraphics[i][j]);
                }
            }

            cube = new Cube(1, 1);
            cube.DrawCube(graphics);
            
        }
    }
    
}
