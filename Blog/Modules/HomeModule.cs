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
            Get["/"] = p =>
            {
                var author = blog.GetOwner();

                return View["index"].WithModel(new
                {
                    Author = author,
                    Articles = blog.GetArticlesByAuthorId(author.Id)
                });
            };
        }
    }
}