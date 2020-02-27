using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class Node
    {
        public int X { get; set; }
        public int Y { get; set; }
        
        public string GetKey()
        {
            return $"X{X}Y{Y}R{RedSidePos}";
        }

        public float Fx { get; set; }
        public int Gx { get; set; }

        public Geometry.Sides RedSidePos { get; set; }

        public Geometry.Directions InitMove { get; set; }

        public Node Parent { get; set; }

        public Node(int x, int y, Geometry.Sides redSidePos)
        {
            X = x;
            Y = y;
            RedSidePos = redSidePos;
        }
        public Node(int x, int y, Geometry.Sides redSidePos, Node parent)
        {
            X = x;
            Y = y;
            RedSidePos = redSidePos;
            Parent = parent;
        }
        public Node(int x, int y, Geometry.Sides redSidePos, Node parent, Geometry.Directions initMove)
        {
            X = x;
            Y = y;
            RedSidePos = redSidePos;
            Parent = parent;
            InitMove = initMove;
        }

    }
}
