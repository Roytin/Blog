using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;

namespace Blog.Modules
{
    public class AdminModule : NancyModule
    {
        public AdminModule(IBlog blog):base("/admin")
        {
            Get["/login/{UserName}/{Password}"] = p =>
            {
                Author author = blog.Login(p.UserName, p.Password);
                if (author == null)
                {
                    //username or password wrong
                    return "to login page";
                }
                Session["Author"] = author;
                return Response.AsRedirect("/admin/console");
            };

            Get["/console"] = p =>
            {
                var author = Session["Author"] as Author;
                if (author == null)
                {
                    //to login page
                    return "to login page";
                }

                return View["console"].WithModel(new
                {
                    Author = author
                });
            };
        }
    }
}