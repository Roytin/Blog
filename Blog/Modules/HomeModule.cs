﻿using System;
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
                Title = blog.Name,
                Categories = blog.GetCategory(),
                Archives = blog.GetArchive(),
                Description = blog.Description
            });

            Get["/detail/{Link}"] = p =>
            {
                var article = blog.Articles.FirstOrDefault(a => a.Link == p.Link);
                if (article == null)
                {
                    return Response.AsRedirect("/error");
                }
                return View["detail"].WithModel(new
                {
                    Author = blog.Author,
                    Article = article,
                    Title = article.Title + " - " + blog.Author.NickName + " - " + blog.Name,
                    Content = article.Content,
                    Categories = blog.GetCategory(),
                    Archives = blog.GetArchive(),
                });
            };

            Get["/archive/{Date}"] = p => {
                DateTime date = DateTime.Parse(p.Date);
                return View["archive"].WithModel(new
                {
                    Author = blog.Author,
                    Articles =
                        blog.Articles.Where(
                            a =>
                                a.CreateTime.Year == date.Year &&
                                a.CreateTime.Month == date.Month),
                    Title = "档案" + p.Date + " - " + blog.Name,
                    Categories = blog.GetCategory(),
                    Archives = blog.GetArchive(),
                });
            };

            Get["/category/{Category}"] = p => View["category"].WithModel(new
            {
                Author = blog.Author,
                Articles = blog.Articles.Where(a => a.Category == p.Category),
                Title = "分类" + p.Category + " - " + blog.Name,
                Categories = blog.GetCategory(),
                Archives = blog.GetArchive(),
            });

            Get["/error"] = p => View["error"];
        }
    }
}