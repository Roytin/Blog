using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Blog
{
    public interface IBlog
    {
        string Name { get; }
        string Description { get; }
        Author Login(string username, string password);

        Author Author { get; }

        IEnumerable<Article> Articles
        {
            get;
        }

        void AddArticle(Article article);

        bool RemoveArticle(string link);

        void UpdateArticle(Article temporary);

        IEnumerable<Info> GetCategory();

        IEnumerable<Info> GetArchive();
    }
}
