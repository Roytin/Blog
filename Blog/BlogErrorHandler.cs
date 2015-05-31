using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using Nancy;
using Nancy.ErrorHandling;
using Nancy.IO;

namespace Blog
{
    public class BlogErrorHandler : IStatusCodeHandler
    {
        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
        {
            return (int)statusCode == 404;
        }

        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            context.Response.ContentType = "text/html";
            context.Response.Contents = s =>
            {
                using (var writer = new StreamWriter(new UnclosableStreamWrapper(s), Encoding.UTF8))
                {
                    writer.Write(Resource.ResourceManager.GetString("error"));
                }
            };
        }
    }
}