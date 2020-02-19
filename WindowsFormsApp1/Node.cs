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

        public Geometry.Sides RedSidePos { get; set; }
        public Node Parent { get; set; }

        public Node(int x, int y, Geometry.Sides redSidePos)
        {
            X = x;
            Y = y;
            RedSidePos = redSidePos;
        }
    }
}
