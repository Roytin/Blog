using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Blog
{
    public interface IBlog
    {
        Author GetAuthor(string username, string password);

        Author GetOwner();

        IEnumerable<Article> GetArticles();

        IEnumerable<Article> GetArticlesById(int articleId);

        IEnumerable<Article> GetArticlesByAuthorId(int authorId);

        IEnumerable<Article> GetArticlesByLastUpdateDate(DateTime date);

        IEnumerable<Article> GetArticlesByTag(string tag);

        IEnumerable<Article> GetArticlesByCategory(string category);
    }

    /// <summary>
    /// 作者
    /// </summary>
    public class Author
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string NickName { get; set; }
        public string LastLoginIP { get; set; }
        public DateTime LastLoginTime { get; set; }
        public DateTime CreateTime { get; set; }

        public string Resume { get; set; }

        public DateTime Birthday { get; set; }

        public int Age
        {
            get { return DateTime.Today.Year - Birthday.Year; }
        }
    }

    /// <summary>
    /// 文章
    /// </summary>
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public string Content { get; set; }

        public string Tags { get; set; }

        public string Description { get; set; }

        public DateTime LastUpdateTime { get; set; }

        public string Category { get; set; }

        public string Link { get; set; }

        public int AuthorId { get; set; }
    }
}
