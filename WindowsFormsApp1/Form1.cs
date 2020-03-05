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
        Rectangle prev;
        
        Priority_Queue.StablePriorityQueue<Node> OpenNodes;
        List<Node> ClosedNodes = new List<Node>();
        Stack<Directions> resultMoves= new Stack<Directions>();

        System.Diagnostics.Stopwatch dt;
        Node target, start;
        public 
        const int   xOffset = 20, 
                    yOffset = 20,
                    tileSize = 30;
        int         numOfFloorTiles=0,
                    nodesVisited=1;
        
        Graphics    graphics;
        Pen         blackPen;
        
        Pen         greenPen;



        Cube        cube;

        enum HeuristicsMethod
        {
            Manhattan,
            Euclid
        }

        HeuristicsMethod currMethod=HeuristicsMethod.Manhattan;
        
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

        private bool AddNodeToOpenQueueWidth(Node parent, Directions moveDirection)
        {
            var info = GetTileInfo(cube.X, cube.Y);
            nodesVisited++;
            if (info == Status.Alive)
            {
                if (
                        !(
                        OpenNodes.Any(x => x.X == cube.X && x.Y == cube.Y && x.RedSidePos == cube.RedSidePos) ||
                        ClosedNodes.Any(x => x.X == cube.X && x.Y == cube.Y && x.RedSidePos == cube.RedSidePos)
                         )
                      )
                {
                    OpenNodes.Enqueue(new Node(cube.X, cube.Y, cube.RedSidePos, parent, moveDirection),1);

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
            
            while(OpenNodes.Count>0)
            {
                curr = OpenNodes.Dequeue();

                cube.X = curr.X;
                cube.Y = curr.Y;
                cube.RedSidePos = curr.RedSidePos;

                cube.MoveCube(Directions.West);
                if (AddNodeToOpenQueueWidth(curr, Directions.West)) return true;
                cube.MoveCube(Directions.East);

                cube.MoveCube(Directions.North);
                if (AddNodeToOpenQueueWidth(curr, Directions.North)) return true;
                cube.MoveCube(Directions.South);

                cube.MoveCube(Directions.East);
                if(AddNodeToOpenQueueWidth(curr, Directions.East)) return true;
                cube.MoveCube(Directions.West);

                cube.MoveCube(Directions.South);
                if(AddNodeToOpenQueueWidth(curr, Directions.South))return true;
                cube.MoveCube(Directions.North);



                ClosedNodes.Add(curr);
            }
            return false;
        }

        private void CommandChainExecution()
        {
            do
            {
                cube.ClearCube(graphics);
                cube.MoveCube(resultMoves.Pop());
             
                cube.DrawCube(graphics);
                System.Threading.Thread.Sleep(300);

            } while (resultMoves.Count > 0);
        }
        private void callMessageBox(String method,long timeSpan, int pathLength, int nodeCount)
        {
            MessageBox.Show($"Метод поиска - {method}\n Длина пути - {pathLength}\nПосещено вершин - {nodeCount}\nВремя обхода  {timeSpan}", "Обход завершён", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.Left:
                    {
                        cube.ClearCube(graphics);
                        cube.MoveCube(Directions.West);
                        break;
                    }
                case Keys.Up:
                    {
                        cube.ClearCube(graphics);
                        cube.MoveCube(Directions.North);
                        break;
                    }
                case Keys.Right:
                    {
                        cube.ClearCube(graphics);
                        cube.MoveCube(Directions.East);
                        break;
                    }
                case Keys.Down:
                    {
                        cube.ClearCube(graphics);
                        cube.MoveCube(Directions.South);
                        break;
                    }
                case Keys.Enter:
                    {
                        
                        OpenNodes.Enqueue(new Node(cube.X, cube.Y, cube.RedSidePos,null),100000);
                        start = OpenNodes.First;
                         dt.Start();
                        if (AI()) 
                        {
                            dt.Stop();
                            var dtwidthpassed = dt.ElapsedMilliseconds;
                            dt.Reset();
                            //cube.ClearCube(graphics);
                            cube.MoveCube(ClosedNodes[0].X, ClosedNodes[0].Y, ClosedNodes[0].RedSidePos);
                            var aasd = resultMoves.Count;
                            String date = DateTime.Now.ToString().Replace(':','-');
                            string path = "../../Полином.txt";
                            for (int i=0;i<aasd;i++)
                            {
                                File.AppendAllText(path, $"x^{i}+");
                            }
                            File.AppendAllText(path, "\n");
                            CommandChainExecution();
                            callMessageBox("Поиск в ширину",dtwidthpassed, aasd, nodesVisited);

                                }
                        OpenNodes.Clear();
                        ClosedNodes.Clear();
                        resultMoves.Clear();
                        GC.Collect();
                        //System.Threading.Thread.Sleep(5000);
                        nodesVisited= 1;
                        
                        break;
                    }
                case Keys.Space:
                    {
                        
                        OpenNodes.Enqueue(new Node(cube.X, cube.Y, cube.RedSidePos, null), 100000);
                        start = OpenNodes.First;
                        OpenNodes.First.Gx = 0;
                        dt.Start();
                        if (AStar())
                        {
                            dt.Stop();
                            var dtpassed = dt.ElapsedMilliseconds;
                            dt.Reset();
                            cube.MoveCube(ClosedNodes[0].X, ClosedNodes[0].Y, ClosedNodes[0].RedSidePos);
                            var aasd = resultMoves.Count;
                            CommandChainExecution();
                            callMessageBox("Эвристика по методу "+ (currMethod==HeuristicsMethod.Manhattan?"манхэттенсковго расстояния":"эвкидовского расстояния между точками"),dtpassed, aasd, nodesVisited);
                        }
                        OpenNodes.Clear();
                        ClosedNodes.Clear();
                        resultMoves.Clear();
                        GC.Collect();
                        //System.Threading.Thread.Sleep(5000);
                        nodesVisited = 1;
                        break;
                    }
                case Keys.D1:
                    {
                        currMethod = HeuristicsMethod.Manhattan;
                        Text = currMethod.ToString();
                        break;
                    }
                case Keys.D2:
                    {
                        currMethod = HeuristicsMethod.Euclid;
                        Text = currMethod.ToString();
                        break;
                    }
                case Keys.Escape:
                    {
                        cube.ClearCube(graphics, greenPen);
                        cube.MoveCube(start.X, start.Y, start.RedSidePos);
                        
                      ;
                        cube.DrawCube(graphics);
                        break;
                    }
            }
            var a = GetTileInfo(cube.X, cube.Y);
            if (a == Status.Dead/*||(cube.RedSideIsOnBottom()&&a==Status.Winner)*/) this.Close();
            else
            {
                
             
                cube.DrawCube(graphics);
            }

        }
        private bool AStar()
        {
            Node curr;
            while (OpenNodes.Count>0)
            {
                curr = OpenNodes.Dequeue();

                cube.X = curr.X;
                cube.Y = curr.Y;
                cube.RedSidePos = curr.RedSidePos;
                cube.Gx=curr.Gx;

                cube.MoveCube(Directions.West);
                if (AddNodeToOpenQueueAStar(curr, Directions.West)) return true;
                cube.MoveCube(Directions.East);

                cube.MoveCube(Directions.North);
                if (AddNodeToOpenQueueAStar(curr, Directions.North)) return true;
                cube.MoveCube(Directions.South);

                cube.MoveCube(Directions.East);
                if (AddNodeToOpenQueueAStar(curr, Directions.East)) return true;
                cube.MoveCube(Directions.West);

                cube.MoveCube(Directions.South);
                if (AddNodeToOpenQueueAStar(curr, Directions.South)) return true;
                cube.MoveCube(Directions.North);



                ClosedNodes.Add(curr);
            }
            return false;
        }
        private float Heuristic(Cube curr)
        {
            float h;
            int dx=curr.X-target.X, dy=curr.Y-target.Y;
            if(currMethod==HeuristicsMethod.Manhattan)
                h= Math.Abs(dx)+Math.Abs(dy);
            else
                h= (float)Math.Sqrt(dx*dx + dy*dy);

            if (h >= 0 /*&& h <= (numOfFloorTiles - curr.Gx)*0.7*/) return h;
            else
            {
                this.Close();
                return 0;
            }
        }
        
        private bool AddNodeToOpenQueueAStar(Node parent, Directions moveDirection)
        {
            var info = GetTileInfo(cube.X, cube.Y);
            var fx = cube.Gx+Heuristic(cube);
            nodesVisited++;

            if (info == Status.Alive)
            {
    
                
                if (
                        !((OpenNodes.Any(x => x.X == cube.X && x.Y == cube.Y && x.RedSidePos == cube.RedSidePos))
                         ||
                        (ClosedNodes.Any(x => x.X == cube.X && x.Y == cube.Y && x.RedSidePos == cube.RedSidePos))
                         )
                      )
                {
                    var a = new Node(cube.X, cube.Y, cube.RedSidePos, parent, moveDirection);
                    a.Fx=fx;
                    a.Gx = parent.Gx + 1;
                    OpenNodes.Enqueue(a,a.Fx);
                    

                }
                else if (OpenNodes.Any(x => x.X == cube.X && x.Y == cube.Y && x.RedSidePos == cube.RedSidePos && x.Fx > fx))
                {
                    var old = OpenNodes.First(x => x.X == cube.X && x.Y == cube.Y && x.RedSidePos == cube.RedSidePos);
                    old.Fx = fx;
                    old.Gx=parent.Gx+1;
                    old.Parent = parent;
                    
                }
                else if (ClosedNodes.Any(x => x.X == cube.X && x.Y == cube.Y && x.RedSidePos == cube.RedSidePos&& x.Fx > fx))
                {
                    var old = ClosedNodes.First(x => x.X == cube.X && x.Y == cube.Y && x.RedSidePos == cube.RedSidePos);
                    old.Fx = fx;
                    old.Gx = parent.Gx + 1;
                    old.Parent = parent;
                    ClosedNodes.Remove(old);
                    OpenNodes.Enqueue(old, old.Fx);

                    

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
                } while (curr.Parent != null);
                return true;
            }
            return false;
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
                    if (mapData[i][j] == '*') continue;
                    else if (mapData[i][j] == '1') graphics.DrawRectangle(blackPen, mapGraphics[i][j]);
                    else graphics.FillRectangle(greenPen.Brush, mapGraphics[i][j]);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            prev = new Rectangle();
            prev.Height=prev.Width = tileSize;
            dt = new System.Diagnostics.Stopwatch();
            Text = currMethod.ToString();
            blackPen = new Pen(Color.Black);
            
            greenPen = new Pen(Color.Green);

            graphics = CreateGraphics();
            mapData = File.ReadAllLines("../../big map.txt");
            //mapData = File.ReadAllLines("../../Drew.txt");
            //mapData = File.ReadAllLines("../../Новый текстовый документ.txt");
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
                    if (mapData[i][j] == '*') continue;
                    else if (mapData[i][j] == '1')
                    {
                        graphics.DrawRectangle(blackPen, mapGraphics[i][j]);
                        numOfFloorTiles++;

                    }
                    else
                    {
                        numOfFloorTiles++;
                        target = new Node(i, j, Sides.Bottom);
                        graphics.FillRectangle(greenPen.Brush, mapGraphics[i][j]);
                    }
                }
            }
            OpenNodes = new Priority_Queue.StablePriorityQueue<Node>(mapData.Length * mapData[0].Length*6);
            cube = new Cube(1, 1);
            cube.RedSidePos = Sides.Front;
            start = new Node(cube.X, cube.Y, cube.RedSidePos);
            cube.DrawCube(graphics);
            
        }
    }
    
}
