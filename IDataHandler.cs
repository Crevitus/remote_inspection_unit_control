using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace remote_inspection_unit_control
{
    interface IDataHandler
    {
        void dataHandler(byte[] data);
    }
}
