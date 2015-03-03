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
        private Point mStart = new Point(0, 0);
        private Bitmap mImage;
        private double mSize = 15;

        public Map(Bitmap b)
        {
            mImage = b;
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
                return mSize;
            }
            set
            {
                if (value > 0)
                {
                    mSize = value;
                }
            }
        }

        public void add(Point p, bool[] dir)
        {
            mMapData.Add(new Pipe(p, dir));
        }

        public void draw()
        {
            double perSizeX = mImage.Width / mSize;
            double perSizeY = mImage.Height / mSize;
            using (Graphics g = Graphics.FromImage(mImage))
            {
                g.Clear(Color.White);
            }
            foreach (Pipe p in mMapData)
            {
                if (p.Position.X - mStart.X >= 0 && p.Position.X - mStart.X < mSize &&
                    p.Position.Y - mStart.Y >= 0 && p.Position.Y - mStart.Y < mSize)
                {
                    double x = (p.Position.X + 1 - mStart.X) * perSizeX - perSizeX / 2;
                    double y = (p.Position.Y + 1 - mStart.Y) * perSizeY - perSizeY / 2;
                    for (int i = 0; i < perSizeX / 2; i++)
                    {
                        int tempY1 = (int)(y + perSizeY / 2) - 1;
                        int tempY2 = (int)(y - perSizeY / 2);
                        int tempX1 = (int)(x + i);
                        int tempX2 = (int)(x - i);
                        if (p.Direction[0])
                        {
                            mImage.SetPixel(tempX1, tempY2, Color.Black);//top
                            mImage.SetPixel(tempX2, tempY2, Color.Black);
                        }
                        if (p.Direction[1])
                        {
                            mImage.SetPixel(tempX1, tempY1, Color.Black);//bottom
                            mImage.SetPixel(tempX2, tempY1, Color.Black);
                        }
                    }
                    for (int i = 0; i < perSizeY / 2; i++)
                    {
                        int tempX1 = (int)(x + perSizeX / 2) - 1;
                        int tempX2 = (int)(x - perSizeX / 2);
                        int tempY1 = (int)(y + i);
                        int tempY2 = (int)(y - i);
                        if (p.Direction[2])
                        {
                            mImage.SetPixel(tempX2, tempY1, Color.Black);//left
                            mImage.SetPixel(tempX2, tempY2, Color.Black);
                        }
                        if (p.Direction[3])
                        {
                            mImage.SetPixel(tempX1, tempY1, Color.Black);//right
                            mImage.SetPixel(tempX1, tempY2, Color.Black);
                        }
                        
                    }
                }
            }
        }
    }
}
