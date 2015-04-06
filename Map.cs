using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace remote_inspection_unit_control
{
    class Map
    {
        private Bitmap mImage;
        private Pen mPen = new Pen(Color.Red, 5);//line color and size
        private Color mBackgroundColor = Color.FromArgb(47, 90, 127);
        bool mLeft = false, mRight = false, mForward = false, mBack = false;
        int mSize = 30;
        public Map(Bitmap b)
        {
            mImage = b;
        }

        public int PenSize
        {
            set
            {
                mPen = new Pen(Color.Black, value);
            }
        }

        public void refresh()
        {
            mLeft = false; 
            mRight = false; 
            mForward = false; 
            mBack = false;
        }

        public bool Left
        {
            set { mLeft = value; }
        }
        public bool Right
        {
            set
            {
                mRight = value;
            }
        }

        public bool Forward
        {
            set
            {
                mForward = value;
            }
        }
        public bool Back
        {
            set
            {
                mBack = value;
            }
        }

        public void draw()
        {
            using (Graphics g = Graphics.FromImage(mImage))
            {
                g.Clear(mBackgroundColor);
                if (mLeft)
                {
                    g.DrawEllipse(mPen, mImage.Width / 2, mImage.Height * 0.05f, mSize/2, mSize/2);
                }
                if (mRight)
                {
                    g.DrawEllipse(mPen, mImage.Width / 2, mImage.Height * 0.90f, mSize/2, mSize/2);
                }
                if (mBack)
                {
                    g.DrawEllipse(mPen, mImage.Width * 0.05f, mImage.Height / 2, mSize/2, mSize/2);
                }
                if (mForward)
                {
                    g.DrawEllipse(mPen, mImage.Width * 0.95f, mImage.Height / 2, mSize/2, mSize/2);
                }
            }
        }
        //g.DrawLine(mPen, new Point(x + perSizeX / 2, (y - perSizeY / 2) - 1), new Point(x + perSizeX / 2, (y + perSizeY / 2) + 1));
    }
}
