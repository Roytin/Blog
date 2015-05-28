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

            Get["/detail/{Link}"] = p =>
            {
                var article = blog.Articles.FirstOrDefault(a => a.Link == p.Link);
                if (article == null)
                {
                    return Response.AsRedirect("error");
                }
                return View["detail"].WithModel(new
                {
                    Author = blog.Author,
                    Article = article
                });
            };

            Get["/archive/{Date}"] = p => View["archive"].WithModel(new
            {
                Author = blog.Author,
                Articles = blog.Articles.Where(a=>a.CreateTime.Date == DateTime.Parse(p.Date))
            });

            Get["/category/{Category}"] = p => View["category"].WithModel(new
            {
                Author = blog.Author,
                Articles = blog.Articles.Where(a => a.Category == p.Category)
            });
        }
    }
}