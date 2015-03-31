using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Printer
{
    class TimeLapse
    {
        private string mPath = "timelapse";
        private bool mEnd = false;
        private bool mPause = false;
        private int mDelay = 60;
        private int mWidth = 100;
        private int mHeight = 100;
        private Point mStart = new Point(0, 0);
        private int mFps = 1;

        public TimeLapse()
        {
            if (!Directory.Exists(mPath))
            {
                Directory.CreateDirectory(mPath);
            }
        }

        public int Delay
        {
            set
            {
                if (value > 0 && value < 1000)
                {
                    mDelay = value;
                }
            }
        }

        public int Width
        {
            set { mWidth = value; }
        }
        public int Height
        {
            set { mHeight = value; }
        }
        public Point StartLocation
        {
            set
            {
                mStart.X = value.X;
                mStart.Y = value.Y;
            }
        }
        public Bitmap screenCap()
        {
            return screenCap(mStart, mWidth, mHeight);
        }

        public Bitmap screenCap(Point start, int width, int height)
        {
            Bitmap bmpScreenCapture = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bmpScreenCapture);
            g.CopyFromScreen(start.X, start.Y, 0, 0, bmpScreenCapture.Size, CopyPixelOperation.SourceCopy);
            g.Dispose();
            return bmpScreenCapture;
        }

        public void startTimeLapse(string fileName)
        {
            mEnd = false;
            new Thread(new ThreadStart(() =>
            {
                int delay = mDelay;
                using (VideoWriter vW = new VideoWriter(mPath + @"\" + fileName, mFps, mWidth, mHeight, true))
                {
                    vW.WriteFrame<Bgr, byte>(new Image<Bgr, Byte>(screenCap()));
                    while (!mEnd)
                    {
                        for (int i = 0; i < delay; i++)
                        {
                            Thread.Sleep(1000);
                            if (mEnd) break;
                            if (mPause) Thread.Sleep(100);
                        }
                        vW.WriteFrame<Bgr, byte>(new Image<Bgr, Byte>(screenCap()));
                    }
                    vW.WriteFrame<Bgr, byte>(new Image<Bgr, Byte>(screenCap()));
                }
            })).Start();
        }

        public void stop()
        {
            mEnd = true;
        }

        public void togglePause()
        {
            mPause = !mPause;
        }
    }
}
