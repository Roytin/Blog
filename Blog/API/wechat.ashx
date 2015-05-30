<%@ WebHandler Language="C#" Class="wechat" %>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

public class wechat : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        if (HttpContext.Current.Request.HttpMethod.ToUpper() != "POST")
        {
            string echoString = context.Request.QueryString["echostr"];
            string signature = context.Request.QueryString["signature"];
            string timestamp = context.Request.QueryString["timestamp"];
            string nonce = context.Request.QueryString["nonce"];
            //Console.WriteLine("echostring:{0},signature:{1},timestamp:{2},nonce:{3}", echoString, signature, timestamp, nonce);
            var strs = new List<string>()
            {
                "haogege",timestamp,nonce
            };
            strs.Sort();
            string comein = string.Join("", strs);
            byte[] cleanBytes = Encoding.Default.GetBytes(comein);
            byte[] hashedBytes = SHA1.Create().ComputeHash(cleanBytes);
            string cominStr = BitConverter.ToString(hashedBytes).Replace("-", "");
            if (String.Compare(cominStr, signature, StringComparison.OrdinalIgnoreCase) == 0)
            {
                context.Response.Write(echoString);
                context.Response.End();
            }
            else
            {
                context.Response.Write("wrong");
                context.Response.End();
            }
        }
        else
        {
            using (Stream stream = context.Request.InputStream)
            {
                var postBytes = new Byte[stream.Length];
                stream.Read(postBytes, 0, (Int32)stream.Length);
                string postString = Encoding.UTF8.GetString(postBytes);
                context.Response.Write(Handle(postString));
            }
        }
    }

    private bool Same(byte[] a, byte[] b)
    {
        if (a.Length == b.Length)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

    private string Handle(string postStr)
    {
        var help = new MessageHelp();
        return help.ReturnMessage(postStr);
    }

}