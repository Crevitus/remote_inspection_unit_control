using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace remote_inspection_unit_control
{
    interface IDataHandler
    {
        void dataHandler(byte[] data);
    }
}
