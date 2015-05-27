using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;

namespace Blog.Modules
{
    public class HomeModule:NancyModule
    {
        public HomeModule(IBlog blog)
        {
            Get["/"] = p => View["index"].WithModel(new
            {
                Author = blog.Author,
                Articles = blog.Articles,
                Times = BlogEngine.C
            });
        }
    }
}