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
                    "select * from Author where UserName=@UserName and Password=MD5(@Password)", new
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
                _dbConnection.Open();
                var author = _dbConnection.Query<Author>("select * from Author limit 1").First();
                _dbConnection.Close();
                return author;
            }
        }

        private List<Article> _articles;

        public IEnumerable<Article> Articles
        {
            get
            {
                if (_articles == null)
                {
                    _dbConnection.Open();
                    _articles = _dbConnection.Query<Article>("select * from Article").AsList();
                    _dbConnection.Close();
                }
                return _articles;
            }
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