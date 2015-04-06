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
        private List<Pipe> mMapData = new List<Pipe>();
        private Point mStart = new Point(0, 0);//top left of map
        private Bitmap mImage;
        private double mZoom = 10;//zoom
        private Pen mPen = new Pen(Color.Black, 5);//line color and size
        private Color mBackgroundColor = Color.FromArgb(47, 90, 127);

        public Map(Bitmap b)
        {
            mImage = b;
            mStart.X = mStart.X - (int)(mZoom / 2);
            mStart.Y = mStart.Y - (int)(mZoom / 2);
        }

        public int PenSize
        {
            set
            {
                mPen = new Pen(Color.Black, value);
            }
        }
        
        public Point StartLoc
        {
            get
            {
                return mStart;
            }
            set
            {
                mStart = value;
            }
        }

        public double Size
        {
            get
            {
                return mZoom;
            }
            set
            {
                if (value > 0)
                {
                    mZoom = value;
                }
            }
        }

        public void add(Point p, bool[] dir)
        {
            mMapData.Add(new Pipe(p, dir));
            //centre map on added pipe when end of edge
            if (p.X < mStart.X)
            {
                mStart.X = mStart.X - (mStart.X - p.X) - (int)(mZoom / 2);//west
            }
            else if (p.Y < mStart.Y)
            {
                mStart.Y = mStart.Y - (mStart.Y - p.Y) - (int)(mZoom / 2);//north
            }
            else if (p.X >= mZoom + mStart.X)
            {
                mStart.X = mStart.X - (mStart.X - p.X) - (int)(mZoom / 2);//east
            }
            else if (p.Y >= mZoom + mStart.Y)
            {
                mStart.Y = mStart.Y - (mStart.Y - p.Y) - (int)(mZoom / 2);//south
            }
        }

        public void draw()
        {
            int perSizeX = (int)Math.Ceiling(mImage.Width / mZoom);
            int perSizeY = (int)Math.Ceiling(mImage.Height / mZoom);
            using (Graphics g = Graphics.FromImage(mImage))
            {
                g.Clear(mBackgroundColor);
            }
            foreach (Pipe p in mMapData)
            {
                if (p.Position.X - mStart.X >= 0 && p.Position.X - mStart.X < mZoom &&
                    p.Position.Y - mStart.Y >= 0 && p.Position.Y - mStart.Y < mZoom)
                {
                    int x = (int)Math.Ceiling((double)(p.Position.X + 1 - mStart.X) * perSizeX - perSizeX / 2);
                    int y = (int)Math.Ceiling((double)(p.Position.Y + 1 - mStart.Y) * perSizeY - perSizeY / 2);
                    using (Graphics g = Graphics.FromImage(mImage))
                    {
                        if (p.Direction[0])
                        {
                            g.DrawLine(mPen, new Point((x - perSizeX / 2) - 1, y - perSizeY / 2), new Point((x + perSizeX / 2) + 1, y - perSizeY / 2));//top
                        }
                        if (p.Direction[1])
                        {
                            g.DrawLine(mPen, new Point((x - perSizeX / 2) - 1, y + perSizeY / 2), new Point((x + perSizeX / 2) + 1, y + perSizeY / 2));//bottom
                        }
                        if (p.Direction[2])
                        {
                            g.DrawLine(mPen, new Point(x - perSizeX / 2, (y - perSizeY / 2) - 1), new Point(x - perSizeX / 2, (y + perSizeY / 2) + 1));//left
                        }
                        if (p.Direction[3])
                        {
                            g.DrawLine(mPen, new Point(x + perSizeX / 2, (y - perSizeY / 2) - 1), new Point(x + perSizeX / 2, (y + perSizeY / 2) + 1));//right
                        }
                    }
                    //mImage.SetPixel((int)x, (int)y, Color.Red);
                }
            }
        }
    }
}
