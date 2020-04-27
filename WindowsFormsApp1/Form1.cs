using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
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
        static string[] mapData;
        MyRect[][]   mapGraphics;
        MyRect prev;
        Node trail;

        Priority_Queue.StablePriorityQueue<Node> OpenNodes;
        List<Node> ClosedNodes = new List<Node>();
        Stack<Directions> resultMoves= new Stack<Directions>();

        System.Diagnostics.Stopwatch dt;
       static Node target, start;
        public 
        const int   xOffset = 20, 
                    yOffset = 20,
                    tileSize = 30;
        int         numOfFloorTiles=0,
                    nodesVisited=1,
                    mapChosen=3,
                    nodesMax=0;
        
        Graphics    graphics;
        Pen         blackPen;
        
        Pen         greenPen;
        Pen         bluePen;
        Pen         lightGreenPen;
        Pen         whitePen;


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
        
        internal static Status GetTileInfo(int i, int j)
        {
            if (i == target.X && j == target.Y)
            {
                return Status.Winner;
            }
            else
            if (i == start.X && j == start.Y)
            {
                return Status.Start;
            }
            else if (mapData[i][j] != 'A') return Status.Alive;
            else return Status.Dead;

                //switch (mapData[i][j])
                //{
                //    case '1': return Status.Alive;
                //    case '2': return Status.Winner;
                //    case '3': return Status.Start;
                //    //    case '4': return Status.Cens;
                //    default: return Status.Dead;
                //}
        }

        private bool IsConnected(Node firstNode, Geometry.Directions moveDirection)
        {
            byte[] bt1 = new byte[1];
            byte[] bt2 = new byte[1];

            bt1[0] = Convert.ToByte(mapData[firstNode.X][firstNode.Y]-'A');
            bt2[0] = Convert.ToByte(mapData[cube.X][cube.Y] - 'A');
            
            BitArray bitArray1 = new BitArray(bt1);
            BitArray bitArray2 = new BitArray(bt2);
            switch (moveDirection)
            {
                case Directions.West:
                    {
                        if (bitArray1[3] == true && bitArray2[1] == true) return true;
                        else return false;
                    }
                case Directions.North:
                    {
                        if (bitArray1[2] == true && bitArray2[0] == true) return true;
                        else return false;
                    }
                case Directions.East:
                    {
                        if (bitArray1[1] == true && bitArray2[3] == true) return true;
                        else return false;
                    }
                case Directions.South:
                    {
                        if (bitArray1[0] == true && bitArray2[2] == true) return true;
                        else return false;
                    }

                default:return false;
            }
            
        }
        private bool AddNodeToOpenQueueWidth(Node parent, Directions moveDirection)
        {
            var info = GetTileInfo(cube.X, cube.Y);
            
            if (IsConnected(parent, moveDirection))
            {
                if (info == Status.Alive || info == Status.Start || (info == Status.Cens && cube.RedSideIsOnBottom()))
                {
                    if (
                            !(
                            OpenNodes.Any(x => x.X == cube.X && x.Y == cube.Y && x.RedSidePos == cube.RedSidePos) ||
                            ClosedNodes.Any(x => x.X == cube.X && x.Y == cube.Y && x.RedSidePos == cube.RedSidePos)
                             )
                          )
                    {
                        OpenNodes.Enqueue(new Node(cube.X, cube.Y, cube.RedSidePos, parent, moveDirection), 1);

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
            }

            return false;
        }
        private bool AI()
        {
            Node curr;
            
            while(OpenNodes.Count>0)
            {
                nodesVisited++;
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

                if (nodesMax < OpenNodes.Count) nodesMax = OpenNodes.Count;

                ClosedNodes.Add(curr);
            }
            return false;
        }

        private void CommandChainExecution()
        {
            do
            {
                //cube.ClearCube(graphics);
                cube.MoveCube(resultMoves.Pop());
             
                cube.DrawCube(graphics);
                System.Threading.Thread.Sleep(1);

            } while (resultMoves.Count > 0);
        }
        private void callMessageBox(String method,long timeSpan, int pathLength, int nodeCount, int nodeMax)
        {
            string str = $"Метод поиска - {method}\r\n Длина пути - {pathLength}\r\nПосещено вершин - {nodeCount}\r\nВремя обхода  {timeSpan}\r\nМаксимальное число открытых вершин - {nodeMax}";
            File.AppendAllText($"../../maps/Map ({mapChosen})_log.txt", str);
            File.AppendAllText($"../../maps/Map ({mapChosen})_log.txt", "################\n\n");
            //MessageBox.Show(str, "Обход завершён", MessageBoxButtons.OK, MessageBoxIcon.Information);
            infoTextBox.Text = str;
           
            

        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.Left:
                    {
                        cube.ClearCube(graphics);
                        trail.X = cube.X;
                        trail.Y = cube.Y;
                        cube.MoveCube(Directions.West);
                        VibeCheck(trail, Directions.West, Directions.East);
                        break;
                    }
                case Keys.Up:
                    {
                        trail.X = cube.X;
                        trail.Y = cube.Y;
                        cube.ClearCube(graphics);
                        cube.MoveCube(Directions.North);
                        VibeCheck(trail, Directions.North, Directions.South);
                        break;
                    }
                case Keys.Right:
                    {
                        trail.X = cube.X;
                        trail.Y = cube.Y;
                        cube.ClearCube(graphics);
                        cube.MoveCube(Directions.East);
                        VibeCheck(trail, Directions.East, Directions.West);
                        break;
                    }
                case Keys.Down:
                    {
                        trail.X = cube.X;
                        trail.Y = cube.Y;
                        cube.ClearCube(graphics);
                        cube.MoveCube(Directions.South);
                        VibeCheck(trail, Directions.South, Directions.North);
                        break;
                    }
                case Keys.Enter:
                    {
                        
                        OpenNodes.Enqueue(new Node(cube.X, cube.Y, cube.RedSidePos,null),100000);
                        mapGraphics[start.X][start.Y].Fill(graphics, blackPen, whitePen.Brush);
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
                            
                            string path = "../../Полином.txt";
                            File.AppendAllText(path, $"Поиск в ширину от {DateTime.Now}\n");
                            for (int i=0;i<aasd;i++)
                            {
                                File.AppendAllText(path, $"x^{i}+");
                            }
                            File.AppendAllText(path, "\n");
                            CommandChainExecution();
                            callMessageBox($"Поиск в ширину от {DateTime.Now}",dtwidthpassed, aasd, nodesVisited, nodesMax);

                                }
                        OpenNodes.Clear();
                        ClosedNodes.Clear();
                        resultMoves.Clear();
                        GC.Collect();
                        //System.Threading.Thread.Sleep(5000);
                        nodesVisited= 1;
                        nodesMax = 0;
                        
                        break;
                    }
                
                case Keys.Space:
                    {
                        
                        OpenNodes.Enqueue(new Node(cube.X, cube.Y, cube.RedSidePos, null), 100000);
                        mapGraphics[start.X][start.Y].Fill(graphics, blackPen, whitePen.Brush);
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
                            string path = "../../Полином.txt";
                            File.AppendAllText(path, $"Поиск в ширину от {DateTime.Now}\n");
                            for (int i = 0; i < aasd; i++)
                            {
                                File.AppendAllText(path, $"x^{i}+");
                            }
                            File.AppendAllText(path, "\n");
                            CommandChainExecution();
                            callMessageBox("Эвристика по методу " + (currMethod == HeuristicsMethod.Manhattan ? "манхэттенсковго расстояния" : "эвкидовского расстояния между точками") + $" от {DateTime.Now}", dtpassed, aasd, nodesVisited, nodesMax) ;
                        }
                        OpenNodes.Clear();
                        ClosedNodes.Clear();
                        resultMoves.Clear();
                        GC.Collect();
                        //System.Threading.Thread.Sleep(5000);
                        nodesVisited = 1;
                        nodesMax = 0;
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
                        Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
               Screen.PrimaryScreen.Bounds.Height);
                        Graphics graphics = Graphics.FromImage(bitmap as Image);
                        graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
                        bitmap.Save($"../../data/Map ({mapChosen}) {DateTime.Now.ToString("dd MMMM yyyy HH-mm-ss")}.jpg", ImageFormat.Jpeg);
                        cube.ClearCube(graphics);
                        cube.MoveCube(start.X, start.Y, start.RedSidePos);
                        
                      
                        cube.DrawCube(graphics);
                        break;
                    }
                case Keys.S:
                    {
                        mapGraphics[start.X][start.Y].Fill(graphics, blackPen,whitePen.Brush);
                        start.X = cube.X;
                        start.Y = cube.Y;
                        start.RedSidePos = cube.RedSidePos;
                        mapGraphics[start.X][start.Y].Fill(graphics,blackPen,bluePen.Brush);
                        break;
                    }
                case Keys.T:
                    {
                        mapGraphics[target.X][target.Y].Fill(graphics, blackPen, whitePen.Brush);
                        target.X = cube.X;
                        target.Y = cube.Y;
                        mapGraphics[target.X][target.Y].Fill(graphics, blackPen, greenPen.Brush);
                        break;
                    }
                case Keys.NumPad1:
                    {
                        mapChosen = 1;
                        Map_Reload(mapChosen);
                        break;
                    }
                case Keys.NumPad2:
                    {
                        mapChosen = 2;
                        Map_Reload(mapChosen);
                        break;
                    }
                case Keys.NumPad3:
                    {
                        mapChosen = 3;
                        Map_Reload(mapChosen);
                        break;
                    }
                case Keys.NumPad4:
                    {
                        mapChosen = 4;
                        Map_Reload(mapChosen);
                        break;
                    }
                case Keys.NumPad5:
                    {
                        mapChosen = 5;
                        Map_Reload(mapChosen);
                        break;
                    }
                case Keys.NumPad6:
                    {
                        mapChosen = 6;
                        Map_Reload(mapChosen);
                        break;
                    }
                case Keys.NumPad7:
                    {
                        mapChosen = 7;
                        Map_Reload(mapChosen);
                        break;
                    }
                case Keys.NumPad8:
                    {
                        mapChosen = 8;
                        Map_Reload(mapChosen);
                        break;
                    }
                case Keys.NumPad9:
                    {
                        mapChosen = 9;
                        Map_Reload(mapChosen);
                        break;
                    }
            }
            
          

        }
        private void VibeCheck(Node trail, Directions direction, Directions stepBack)
        {
            var a = GetTileInfo(cube.X, cube.Y);
            if (a == Status.Dead || (!cube.RedSideIsOnBottom() && a == Status.Cens)||!IsConnected(trail,direction))
            {

                cube.MoveCube(stepBack);


                
            }
            cube.DrawCube(graphics);
        }
        private bool AStar()
        {
            Node curr;
            while (OpenNodes.Count>0)
            {
                nodesVisited++;
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

                if (nodesMax < OpenNodes.Count) nodesMax = OpenNodes.Count;

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

            if (h >= 0 && h <= (numOfFloorTiles - curr.Gx) * 0.7) return h;
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
            
            if (IsConnected(parent, moveDirection))
            {
                if (info == Status.Alive || info == Status.Start || (info == Status.Cens && cube.RedSideIsOnBottom()))
                {


                    if (
                            !((OpenNodes.Any(x => x.X == cube.X && x.Y == cube.Y && x.RedSidePos == cube.RedSidePos))
                             ||
                            (ClosedNodes.Any(x => x.X == cube.X && x.Y == cube.Y && x.RedSidePos == cube.RedSidePos))
                             )
                          )
                    {
                        var a = new Node(cube.X, cube.Y, cube.RedSidePos, parent, moveDirection);
                        a.Fx = fx;
                        a.Gx = parent.Gx + 1;
                        OpenNodes.Enqueue(a, a.Fx);


                    }
                    else if (OpenNodes.Any(x => x.X == cube.X && x.Y == cube.Y && x.RedSidePos == cube.RedSidePos && x.Fx > fx))
                    {
                        var old = OpenNodes.First(x => x.X == cube.X && x.Y == cube.Y && x.RedSidePos == cube.RedSidePos);
                        old.Fx = fx;
                        old.Gx = parent.Gx + 1;
                        old.Parent = parent;

                    }
                    else if (ClosedNodes.Any(x => x.X == cube.X && x.Y == cube.Y && x.RedSidePos == cube.RedSidePos && x.Fx > fx))
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
            }
            return false;
        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            
        }

       
        private void Map_Reload(int mapID)
        {
            start = null;
            graphics.Clear(Color.White);
            OpenNodes.Clear();
            ClosedNodes.Clear();
            resultMoves.Clear();
            nodesMax = 0;
            numOfFloorTiles = 0;
            nodesVisited = 1;

            GC.Collect();
            string path = $"../../maps/Map ({mapChosen}).txt";
            mapData = File.ReadAllLines(path);
            Map_Modifier();

            mapGraphics = new MyRect[mapData.Length][];

            for (int i = 0; i < mapData.Length; i++)
            {
                
                mapGraphics[i] = new MyRect[mapData[i].Length];

                for (int j = 0; j < mapData[i].Length; j++)
                {
                    mapGraphics[i][j] = new MyRect(xOffset + j * tileSize, yOffset + i * tileSize, tileSize - 5,mapData[i][j]);

                    if (mapData[i][j] == 'A') {
                        mapGraphics[i][j].Fill(graphics, blackPen,blackPen.Brush);
                    }
                    else 
                    {
                        mapGraphics[i][j].Draw(graphics,blackPen);
                        numOfFloorTiles++;

                    }
                     if(i==1&& j==1)
                    {
                        mapGraphics[i][j].Fill(graphics,blackPen,bluePen.Brush);
                        cube = new Cube(i, j);
                        cube.RedSidePos = Sides.Front;
                        start = new Node(cube.X, cube.Y, cube.RedSidePos);
                        numOfFloorTiles++;
                    }
                    //else if (mapData[i][j] == '4')
                    //{
                    //    graphics.FillMyRect(lightGreenPen.Brush, mapGraphics[i][j]);
                        
                    //    numOfFloorTiles++;
                    //}
                    if (i==15&&j==15)
                    {
                        numOfFloorTiles++;
                        target = new Node(i, j, Sides.Bottom);
                        mapGraphics[i][j].Fill(graphics,blackPen,greenPen.Brush);
                    }
                }
            }
            File.WriteAllLines(path, mapData);
            OpenNodes = new Priority_Queue.StablePriorityQueue<Node>(mapData.Length * mapData[0].Length * 6);
            
            
            if (start == null)
            {
                cube = new Cube(21, 29);
                cube.RedSidePos = Sides.Front;
                start = new Node(cube.X, cube.Y, cube.RedSidePos);
            }
            cube.DrawCube(graphics);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            prev = new MyRect();
            trail = new Node(0,0, Sides.Back);
            prev.Width = tileSize;
            dt = new System.Diagnostics.Stopwatch();
            Text = currMethod.ToString();
            blackPen = new Pen(Color.Black);
            
            greenPen = new Pen(Color.Green);
            bluePen = new Pen(Color.Blue);
            lightGreenPen = new Pen(Color.LightGreen);
            whitePen = new Pen(Color.White);
            graphics = CreateGraphics();
            mapData = File.ReadAllLines("../../maps/Map (6).txt");
            //mapData = File.ReadAllLines("../../Drew.txt");
            //mapData = File.ReadAllLines("../../Новый текстовый документ.txt");
            mapGraphics = new MyRect[mapData.Length][];

            for (int i=0; i<mapData.Length;i++)
            {
                mapData[i] = mapData[i].Replace(" ","");
                mapGraphics[i] = new MyRect[mapData[i].Length];

               for(int j=0; j<mapData[i].Length;j++)
                {
                    mapGraphics[i][j] = new MyRect(xOffset + j * tileSize, yOffset + i * tileSize, tileSize - 5,mapData[i][j]);
                    if (mapData[i][j] == 'A')
                    {
                        mapGraphics[i][j].Fill(graphics, blackPen, blackPen.Brush);
                    }
                    else
                    {
                        mapGraphics[i][j].Draw(graphics, blackPen);
                        numOfFloorTiles++;

                    }
                    if (i == 21 && j == 29)
                    {
                        mapGraphics[i][j].Fill(graphics, blackPen, bluePen.Brush);
                        cube = new Cube(i, j);
                        cube.RedSidePos = Sides.Front;
                        start = new Node(cube.X, cube.Y, cube.RedSidePos);
                        numOfFloorTiles++;
                    }
                    //else if (mapData[i][j] == '4')
                    //{
                    //    graphics.FillMyRect(lightGreenPen.Brush, mapGraphics[i][j]);

                    //    numOfFloorTiles++;
                    //}
                    if (i == 1 && j == 14)
                    {
                        numOfFloorTiles++;
                        target = new Node(i, j, Sides.Bottom);
                        mapGraphics[i][j].Fill(graphics, blackPen, greenPen.Brush);
                    }
                }
            }
            OpenNodes = new Priority_Queue.StablePriorityQueue<Node>(mapData.Length * mapData[0].Length*6);
            cube = new Cube(start.X, start.Y);
            cube.RedSidePos = Sides.Front;
            
            cube.DrawCube(graphics);
            
        }
        private void Map_Modifier()
        {
            if (mapData[0][13] != 'A')
            {
                int newLength = mapData[0].Length + 2;
                string[] newMapData = new string[mapData.Length + 2];
                newMapData[0] = newMapData[newMapData.Length - 1] = new string('A', newLength);
                for (int i = 0; i < mapData.Length; i++)
                {
                    newMapData[i + 1] = 'A' + mapData[i] + 'A';
                }
                mapData = newMapData;
            }
            
        }
    }
    
}
