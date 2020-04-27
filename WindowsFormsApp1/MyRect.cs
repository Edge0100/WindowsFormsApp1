using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections;

namespace WindowsFormsApp1
{
    class MyRect
    {
        private Rectangle Rect;

        int tileDesign;
        public MyRect()
        {
            Rect = new Rectangle();
        }
        public MyRect(int x, int y, int width, char tileDesign)
        {

            Rect = new Rectangle(x, y, width, width);
            this.tileDesign = (tileDesign - 'A');
        }

        public int X
        {
            get
            {
                return Rect.X;
            }
            set
            {
                Rect.X = value;
            }
        }
        public int Y
        {
            get
            {
                return Rect.Y;
            }
            set
            {
                Rect.Y = value;
            }
        }
        public int Width
        {
            get
            {
                return Rect.Width;
            }
            set
            {
                Rect.Width = value;
            }
        }
        public void Draw(Graphics graphics, Pen blackPen)
        {
            blackPen.Width = 2;
            byte[] bt = new byte[1];
            bt[0] = Convert.ToByte(tileDesign);
            BitArray bitArray = new BitArray(bt);
            try { if (bitArray[0] == false) graphics.DrawLine(blackPen, Rect.X, Rect.Y + Rect.Width, Rect.X + Rect.Width, Rect.Y + Rect.Width); } catch { }
            try
            {
                if (bitArray[1] == false) graphics.DrawLine(blackPen, Rect.X + Rect.Width, Rect.Y+1, Rect.X + Rect.Width, Rect.Y+1 + Rect.Width);
            }
            catch { }
            try { if (bitArray[2] == false) graphics.DrawLine(blackPen, Rect.X, Rect.Y, Rect.X + Rect.Width, Rect.Y); } catch { }
            try { if (bitArray[3] == false) graphics.DrawLine(blackPen, Rect.X, Rect.Y, Rect.X, Rect.Y + Rect.Width); } catch { }
        }
        public void Fill(Graphics graphics, Pen blackPen, Brush fillBrush)
        {
            blackPen.Width = 2;
            graphics.FillRectangle(fillBrush, Rect);
            byte[] bt = new byte[1];
            bt[0] = Convert.ToByte(tileDesign);
            BitArray bitArray = new BitArray(bt);
            try
            {
                if (bitArray[0] == false) graphics.DrawLine(blackPen, Rect.X, Rect.Y + Rect.Width, Rect.X + Rect.Width, Rect.Y + Rect.Width);
            }
            catch { }
            try { if (bitArray[1] == false) graphics.DrawLine(blackPen, Rect.X + Rect.Width, Rect.Y+1, Rect.X + Rect.Width, Rect.Y+1 + Rect.Width); } catch { }
            try { if (bitArray[2] == false) graphics.DrawLine(blackPen, Rect.X, Rect.Y, Rect.X + Rect.Width, Rect.Y); } catch { }
            try { if (bitArray[3] == false) graphics.DrawLine(blackPen, Rect.X, Rect.Y, Rect.X, Rect.Y + Rect.Width); } catch { }
        }
    }



}

