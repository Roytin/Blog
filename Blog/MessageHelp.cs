using System;
using System.Net;
using System.Web;
using System.IO;
using System.Text;
using System.Xml;

public class MessageHelp
{
    //返回消息
    public string ReturnMessage(string postStr)
    {
        string responseContent = "";
        var xmldoc = new XmlDocument();
        xmldoc.Load(new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(postStr)));
        XmlNode msgType = xmldoc.SelectSingleNode("/xml/MsgType");
        if (msgType != null)
        {
            switch (msgType.InnerText)
            {
                case "event":
                    responseContent = EventHandle(xmldoc);//事件处理
                    break;
                case "text":
                    responseContent = TextHandle(xmldoc);//接受文本消息处理
                    break;
                default:
                    break;
            }
        }
        return responseContent;
    }
    //事件
    public string EventHandle(XmlDocument xmldoc)
    {
        string responseContent = "";
        XmlNode Event = xmldoc.SelectSingleNode("/xml/Event");
        XmlNode eventKey = xmldoc.SelectSingleNode("/xml/EventKey");
        XmlNode toUserName = xmldoc.SelectSingleNode("/xml/ToUserName");
        XmlNode fromUserName = xmldoc.SelectSingleNode("/xml/FromUserName");
        if (Event != null)
        {
            if(Event.InnerText.Equals("subscribe"))
            {
                responseContent = string.Format(ReplyType.Message_Text,
                  fromUserName.InnerText,
                  toUserName.InnerText,
                  DateTime.Now.Ticks,
                  "Welcome to CastleInfinite！\r\n我是您的助手喵喵酱\r\n(ฅ>ω<*ฅ)\r\n无限城游戏世界正在紧锣密鼓地开发ing...<a href=\"http://ifchen.com\">点击进入官网</a>\r\n您现在可以随意和我聊天哦~");
            }
            //菜单单击事件
            else if (Event.InnerText.Equals("CLICK"))
            {
                if (eventKey.InnerText.Equals("click_one"))//click_one
                {
                    responseContent = string.Format(ReplyType.Message_Text,
                        fromUserName.InnerText,
                        toUserName.InnerText,
                        DateTime.Now.Ticks,
                        "你点击的是click_one");
                }
                else if (eventKey.InnerText.Equals("click_two"))//click_two
                {
                    responseContent = string.Format(ReplyType.Message_News_Main,
                        fromUserName.InnerText,
                        toUserName.InnerText,
                        DateTime.Now.Ticks,
                        "2",
                         string.Format(ReplyType.Message_News_Item, "我要寄件", "",
                         "http://www.soso.com/orderPlace.jpg",
                         "http://www.soso.com/") +
                         string.Format(ReplyType.Message_News_Item, "订单管理", "",
                         "http://www.soso.com/orderManage.jpg",
                         "http://www.soso.com/"));
                }
                else if (eventKey.InnerText.Equals("click_three"))//click_three
                {
                    responseContent = string.Format(ReplyType.Message_News_Main,
                        fromUserName.InnerText,
                        toUserName.InnerText,
                        DateTime.Now.Ticks,
                        "1",
                         string.Format(ReplyType.Message_News_Item, "标题", "摘要",
                         "http://www.soso.com/jieshao.jpg",
                         "http://www.soso.com/"));
                }
            }
        }
        return responseContent;
    }

    //接受文本消息
    public string TextHandle(XmlDocument xmldoc)
    {
        string responseContent = "";
        XmlNode toUserName = xmldoc.SelectSingleNode("/xml/ToUserName");
        XmlNode fromUserName = xmldoc.SelectSingleNode("/xml/FromUserName");
        XmlNode content = xmldoc.SelectSingleNode("/xml/Content");
        if (content != null)
        {
            string resStr = "喵喵酱不认识...(┳Д┳)";
            var request = WebRequest.Create("http://api.mrtimo.com/Simsimi.ashx?parm=" + content.InnerText);
            request.Timeout = 1500;
            try
            {
                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        if (stream != null)
                        {
                            byte[] data = new byte[1024];
                            int length = stream.Read(data, 0, data.Length);
                            resStr = Encoding.UTF8.GetString(data, 0, length);
                            stream.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                resStr = "想了半天还是不认识..(ρε；)";
            }
            responseContent = string.Format(ReplyType.Message_Text,
                fromUserName.InnerText,
                toUserName.InnerText,
                DateTime.Now.Ticks,
                resStr);
        }
        return responseContent;
    }

    //写入日志
    public void WriteLog(string text)
    {
        StreamWriter sw = new StreamWriter(".\\log.txt", true);
        sw.WriteLine(text);
        sw.Close();//写入
    }
}

//回复类型
public class ReplyType
{
    /// <summary>
    /// 普通文本消息
    /// </summary>
    public static string Message_Text
    {
        get
        {
            return @"<xml>
                            <ToUserName><![CDATA[{0}]]></ToUserName>
                            <FromUserName><![CDATA[{1}]]></FromUserName>
                            <CreateTime>{2}</CreateTime>
                            <MsgType><![CDATA[text]]></MsgType>
                            <Content><![CDATA[{3}]]></Content>
                            </xml>";
        }
    }

    /// <summary>
    /// 图文消息主体
    /// </summary>
    public static string Message_News_Main
    {
        get
        {
            return @"<xml>
                            <ToUserName><![CDATA[{0}]]></ToUserName>
                            <FromUserName><![CDATA[{1}]]></FromUserName>
                            <CreateTime>{2}</CreateTime>
                            <MsgType><![CDATA[news]]></MsgType>
                            <ArticleCount>{3}</ArticleCount>
                            <Articles>
                            {4}
                            </Articles>
                            </xml> ";
        }
    }

    /// <summary>
    /// 图文消息项
    /// </summary>
    public static string Message_News_Item
    {
        get
        {
            return @"<item>
                            <Title><![CDATA[{0}]]></Title> 
                            <Description><![CDATA[{1}]]></Description>
                            <PicUrl><![CDATA[{2}]]></PicUrl>
                            <Url><![CDATA[{3}]]></Url>
                            </item>";
        }
    }
}
