using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace remote_inspection_unit_control
{
    class Pipe
    {
        private Point mPos;
        private bool[] mDir;
        public Pipe(Point p, bool[] dir)
        {
            mPos = p;
            mDir = dir;
        }

        public Point Position
        {
            get
            {
                return mPos;
            }
        }

        public bool[] Direction
        {
            get
            {
                return mDir;
            }
        }

    }
}
