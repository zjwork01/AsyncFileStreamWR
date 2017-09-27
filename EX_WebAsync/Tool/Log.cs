using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.IO;
using System.Text;
using EX_WebAsync.Tool;

/**
 * 用于日志的读写
 * 赵进
 * 2017-09-27
 * */
namespace EX_WebAsync.Tool
{
    /// <summary>
    /// 日志类，用于日志操作
    /// </summary>
    public class Log
    {
        //获取日志文件的地址
        private static string LogPath = ConfigurationManager.AppSettings["LogPath"].ToString() + "Log-" + DateTime.Now.ToString("yyyy-MM-dd")+".txt";
        
        /// <summary>
        /// 写入日志文件
        /// </summary>
        /// <param name="type">日志类型</param>
        /// <param name="msg">日志内容</param>
        public static void WriteLog(LogType type, string msg)
        {
            if (!File.Exists(LogPath))
            {
                File.Create(LogPath).Close();
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(type.ToString()+"     ");
            sb.Append(DateTime.Now.ToString("hh:mm:ss")+"       ");
            sb.Append(msg+@"\r\h");
            FileStreamOperate operate = new FileStreamOperate();
            operate.Write(LogPath, sb.ToString());
        }
    }

    /// <summary>
    /// 日志类型 Trace,Error,Warning,SQL
    /// </summary>
    public enum LogType
    {
        Trace,
        Warning,
        Error,
        SQL
    }
}