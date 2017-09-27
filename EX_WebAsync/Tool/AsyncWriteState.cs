using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace EX_WebAsync.Tool
{
    class AsyncWriteState
    {
        public int WriteCountOnce { get; set; }
        public int Offset { get; set; }
        public byte[] Buffer { get; set; }
        public ManualResetEvent WaitHandle { get; set; }
        public FileStream FS { get; set; }
    }
}
