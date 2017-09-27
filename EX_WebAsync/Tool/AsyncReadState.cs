using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EX_WebAsync.Tool
{
    class AsyncReadState
    {
        public FileStream FS { get; set; }
        public byte[] Buffer { get; set; }
        public ManualResetEvent Even { get; set; }
    }
}
