using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.Cookies;
using Nancy.ModelBinding;

namespace Blog.Modules
{
    public class AdminModule : NancyModule
    {
        public AdminModule(IBlog blog):base("/admin")
        {
            Get["/"] = p =>
            {
                var author = CheckLogin(blog);
                if (author == null)
                    return View["login"];

                return View["admin"].WithModel(new
                {
                    Author = author,
                    Articles = blog.Articles,
                });
            };

            Get["/login/{UserName}/{Password}"] = p =>
            {
                Author author = blog.Login(p.UserName, p.Password);
                if (author == null)
                {
                    //username or password wrong
                    return View["login"];
                }

                //login success
                Session["Author"] = author;
                return Response.AsRedirect("/admin/");//.WithCookie(new NancyCookie("xsp-mz", p.UserName)).WithCookie(new NancyCookie("xsp-mm", p.Password));
            };

            Get["/edit/{Link}"] = p =>
            {
                //check session
                if (CheckLogin(blog) == null)
                    return View["login"];

                if (p.Link == "new")
                {
                    //添加新
                    return View["edit"].WithModel(new
                    {
                        Action = "add"
                    });
                }
                
                //update
                var article = blog.Articles.FirstOrDefault(a => a.Link == p.Link);
                if (article == null)
                {
                    return View["error"];
                }

                return View["edit"].WithModel(new
                {
                    Action = "edit",
                    Article = article,
                });
            };

            Post["/edit/{Link}"] = p =>
            {
                //check session
                if (CheckLogin(blog) == null)
                    return View["login"];

                var article = this.Bind<Article>();
                if (p.Link == "new")
                {
                    //添加新
                    blog.AddArticle(article);
                }
                else
                {
                    //update
                    blog.UpdateArticle(article);
                }

                return Response.AsRedirect("/admin/");
            };
        }

        private Author CheckLogin(IBlog blog)
        {
            var author = Session["Author"] as Author;
            if (author == null)
            {
                //string mz;
                //if (Request.Cookies.TryGetValue("xsp-mz", out mz))
                //{
                //    string mm;
                //    if (Request.Cookies.TryGetValue("xsp-mz", out mm))
                //    {
                //        author = blog.Login(mz, mm);
                //    }
                //}
            }
            return author;
        }
    }
}