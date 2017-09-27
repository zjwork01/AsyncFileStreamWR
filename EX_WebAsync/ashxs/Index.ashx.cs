using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EX_WebAsync.Tool;
using System.Web.Script.Serialization;

namespace EX_WebAsync.ashxs
{
    /// <summary>
    /// Index 的摘要说明
    /// </summary>
    public class Index : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string type = getData(context, "type");
            switch (type)
            {
                case "read":
                    string rfilepath = getData(context, "filepath");
                    Read(context,rfilepath);
                    break;
                case "write":
                    string wfilepath = getData(context, "filepath");
                    string wdata = getData(context, "data");
                    Write(context, wfilepath, wdata);
                    break;
            }
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="context"></param>
        /// <param name="filepath"></param>
        /// <param name="data"></param>
        private void Write(HttpContext context, string filepath, string data)
        {
            try
            {
                FileStreamOperate writeoperate = new FileStreamOperate();
                writeoperate.Write(filepath, data);
                var msg = new { result = "success" };
                context.Response.Write(ObjectToJSON(msg));
            }
            catch(Exception ex)
            {
                Log.WriteLog(LogType.Error, "写入文件出错，原因：" + ex.ToString());
                var msg = new { result = "fail" };
                context.Response.Write(ObjectToJSON(msg));
            }
        }

        private void Read(HttpContext context,string filepath)
        {
            try
            {
                FileStreamOperate readOperate = new FileStreamOperate();
                readOperate.Read(filepath);
                var msg = new { result = "success", version = readOperate.DataForRead.ToString() };
                context.Response.Write(ObjectToJSON(msg));
            }
            catch(Exception ex)
            {
                Log.WriteLog(LogType.Error, "读取文件出错，原因：" + ex.ToString());
                var msg = new { result = "fail" };
                context.Response.Write(ObjectToJSON(msg));
            }
        }

        private string ObjectToJSON(object data)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string result = serializer.Serialize(data);
            return result;
        }

        private string getData(HttpContext context,string key)
        {
            string data = string.Empty;
            try
            {
                if (context.Request[key] != null && !string.IsNullOrEmpty(context.Request[key].ToString()))
                {
                    data = context.Request[key].ToString();
                }
            }
            catch { data = ""; }

            return data;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}