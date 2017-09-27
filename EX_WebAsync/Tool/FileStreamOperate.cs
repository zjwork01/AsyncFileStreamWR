using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
 * 文件流操作
 * Eric
 * 2017-09-25 
 * */
namespace EX_WebAsync.Tool
{
    /// <summary>
    /// 异步读写操作
    /// </summary>
    class FileStreamOperate
    {
        #region 写文件

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="filepath">文件地址</param>
        /// <param name="msg">数据</param>
        public void Write(string filepath, string msg)
        {
            byte[] dataForWrite = Encoding.Default.GetBytes(msg);
            using (FileStream fileStream = new FileStream(filepath, FileMode.Append, FileAccess.Write))
            {
                int offset = 0;
                //每次写入32字节
                int writeCountOnce = dataForWrite.Length < (1 << 5) ? dataForWrite.Length : (1 << 5);
                //构造回调函数需要的状态
                AsyncWriteState state = new AsyncWriteState()
                {
                    WriteCountOnce = writeCountOnce,
                    Offset = offset,
                    Buffer = dataForWrite,
                    WaitHandle = new System.Threading.ManualResetEvent(false),
                    FS = fileStream
                };
                //异步写操作
                fileStream.BeginWrite(dataForWrite, offset, writeCountOnce, new AsyncCallback(AsyncWriteCallBack), state);
                state.WaitHandle.WaitOne();
            }
            Console.WriteLine("Done");
            Console.ReadKey();
        }

        private void AsyncWriteCallBack(IAsyncResult result)
        {
            AsyncWriteState state = result.AsyncState as AsyncWriteState;
            try
            {
                state.FS.EndWrite(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Write Error:" + ex.ToString());
                state.WaitHandle.Set();
                return;
            }
            if ((state.Offset + state.WriteCountOnce) < state.Buffer.Length)
            {
                state.Offset += state.WriteCountOnce;
                state.WriteCountOnce = state.WriteCountOnce < (state.Buffer.Length - state.Offset) ? state.WriteCountOnce : (state.Buffer.Length - state.Offset);
                state.FS.BeginWrite(state.Buffer, state.Offset, state.WriteCountOnce, new AsyncCallback(AsyncWriteCallBack), state);
            }
            else
            {
                state.WaitHandle.Set();
            }
        }

        #endregion

        #region 读文件并显示

        //读取到的信息
        public StringBuilder DataForRead = new StringBuilder();
        private const Int32 bufferSize = 1024;
        /// <summary>
        /// 异步读出数据
        /// </summary>
        /// <param name="filepath">文件地址</param>
        public  void Read(string filepath)
        {
            using (FileStream stream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[bufferSize];
                AsyncReadState state = new AsyncReadState()
                {
                    FS = stream,
                    Buffer = buffer,
                    Even = new System.Threading.ManualResetEvent(false)
                };
                //异步读操作
                stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(AsyncReadCallBack), state);
                //阻塞当前线程直到读取完毕发出信号
                state.Even.WaitOne();
            }
        }

        private void AsyncReadCallBack(IAsyncResult result)
        {
            AsyncReadState state = result.AsyncState as AsyncReadState;
            int readCon = state.FS.EndRead(result);
            //判断是否读到内容
            if (readCon > 0)
            {
                byte[] buffer;
                if (readCon == bufferSize)
                {
                    buffer = state.Buffer;
                }
                else
                {
                    buffer = new byte[readCon];
                    Array.Copy(state.Buffer, 0, buffer, 0, readCon);
                }
                //输出读取的内容
                DataForRead.Append(Encoding.Default.GetString(buffer));
            }
            if (readCon < bufferSize)
            {
                //终止当前线程
                state.Even.Set();
            }
            else
            {
                Array.Clear(state.Buffer, 0, bufferSize);
                //再次执行异步读取数据
                state.FS.BeginRead(state.Buffer, 0, bufferSize, new AsyncCallback(AsyncReadCallBack), state);
            }
        }

        #endregion
    }
}
