using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using Dapper;

namespace Blog
{
    public class BlogEngine : IBlog
    {
        private readonly IDbConnection _dbConnection;
        private readonly object _syncLock = new object();
        private const string Salt = "Hehe";

        public static int C;
        public BlogEngine(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
            Interlocked.Increment(ref C);
        }
        
        public Author Login(string username, string password)
        {
            lock (_syncLock)
            {
                _dbConnection.Open();
                var author = _dbConnection.Query<Author>(
                    "select * from Author where UserName=@UserName and Password=MD5(@Password);", new
                    {
                        UserName = username,
                        Password = password + Salt
                    }).FirstOrDefault();
                _dbConnection.Close();
                return author;
            }
        }
        
        public Author Author
        {
            get
            {
                if (_author == null)
                {
                    _dbConnection.Open();
                    _author = _dbConnection.Query<Author>("select * from Author limit 1;").FirstOrDefault();
                    _dbConnection.Close();
                }
                return _author;
            }
        }

        private List<Article> _articles;
        private Author _author;

        public IEnumerable<Article> Articles
        {
            get
            {
                if (_articles == null)
                {
                    _dbConnection.Open();
                    _articles = _dbConnection.Query<Article>("select * from Article;").AsList();
                    _dbConnection.Close();
                }
                return _articles;
            }
        }

        public void AddArticle(Article article)
        {
            if (_articles.Any(a => a.Link == article.Link))
            {
                throw new Exception("有同样的Link存在");
            }
            article.CreateTime = DateTime.Now;
            article.LastUpdateTime = DateTime.Now;
            article.AuthorId = _author.Id;

            _dbConnection.Open();
            int n = _dbConnection.Execute("INSERT INTO Article(Title,Link,Content,Description,Category,Tags,CreateTime,LastUpdateTime,AuthorId) " +
                                           "Values(@Title,@Link,@Content,@Description,@Category,@Tags,@CreateTime,@LastUpdateTime,@AuthorId);", article);
            if (n > 0)
            {
                int id = _dbConnection.Query<int>("select Id from Article where Link = @Link", article).Single();
                article.Id = id;
                _articles.Add(article);
            }
            _dbConnection.Close();

        }

        public bool RemoveArticle(string link)
        {
            var article = _articles.FirstOrDefault(a => a.Link == link);
            if (article == null) 
                return false;

            _dbConnection.Open();
            _dbConnection.Execute("DELETE FROM Article WHERE Link=@Link;", article);
            _dbConnection.Close();
            return true;
        }

        public void UpdateArticle(Article temporary)
        {
            var article = _articles.FirstOrDefault(a => a.Id == temporary.Id);
            if (article == null)
                return;
            temporary.CreateTime = article.CreateTime;
            temporary.LastUpdateTime = DateTime.Now;
            temporary.AuthorId = Author.Id;
            _dbConnection.Open();
            int n = _dbConnection.Execute("UPDATE `Article` SET `Title`=@Title,`Content`=@Content,`Description`=@Description," +
                                  "`Category`=@Category," +
                                  "`Tags`=@Tags,`LastUpdateTime`=@LastUpdateTime WHERE `Id`=@Id;", temporary);
            if (n > 0)
            {
                _articles.Remove(article);
                _articles.Add(temporary);
                _articles.Sort();
            }
            _dbConnection.Close();
        }

        //public IEnumerable<Article> GetArticlesByLink(string link)
        //{
        //    lock (_syncLock)
        //    {
        //        _dbConnection.Open();
        //        var articles = _dbConnection.Query<Article>("select * from Article where Link=@Link",new
        //        {
        //            Link = link
        //        });
        //        _dbConnection.Close();
        //        return articles;
        //    }
        //}

        //public IEnumerable<Article> GetArticlesByAuthorId(int authorId)
        //{
        //    lock (_syncLock)
        //    {
        //        _dbConnection.Open();
        //        var articles = _dbConnection.Query<Article>("select * from Article where AuthorId=@AuthorId", new
        //        {
        //            AuthorId = authorId
        //        });
        //        _dbConnection.Close();
        //        return articles;
        //    }
        //}

        //public IEnumerable<Article> GetArticlesByLastUpdateDate(DateTime date)
        //{
        //    lock (_syncLock)
        //    {
        //        _dbConnection.Open();
        //        var articles = _dbConnection.Query<Article>("select * from Article where date(LastUpdateTime)=@Date",
        //            new
        //            {
        //                Date=date.ToString("yyyy-MM-dd")
        //            });
        //        _dbConnection.Close();
        //        return articles;
        //    }
        //}

        //public IEnumerable<Article> GetArticlesByTag(string tag)
        //{
        //    lock (_syncLock)
        //    {
        //        _dbConnection.Open();
        //        var articles = _dbConnection.Query<Article>("select * from Article where Find_In_Set(@Tag, Tags)",
        //            new 
        //            {
        //                Tag = tag
        //            });
        //        _dbConnection.Close();
        //        return articles;
        //    }
        //}

        //public IEnumerable<Article> GetArticlesByCategory(string category)
        //{
        //    lock (_syncLock)
        //    {
        //        _dbConnection.Open();
        //        var articles = _dbConnection.Query<Article>("select * from Article where Category=@Category", new
        //        {
        //            Category = category
        //        });
        //        _dbConnection.Close();
        //        return articles;
        //    }
        //}
    }
}