using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Dapper;

namespace Blog
{
    public class BlogEngine : IBlog
    {
        private readonly IDbConnection _dbConnection;
        private readonly object _syncLock = new object();
        private const string Salt = "Hehe";

        public BlogEngine(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public Author GetAuthor(string username, string password)
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

        public Author GetOwner()
        {
            lock (_syncLock)
            {
                _dbConnection.Open();
                var author = _dbConnection.Query<Author>("select * from Author limit 1").FirstOrDefault();
                _dbConnection.Close();
                return author;
            }
        }

        public IEnumerable<Article> GetArticles()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Article> GetArticlesById(int articleId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Article> GetArticlesByAuthorId(int authorId)
        {
            return null;
        }

        public IEnumerable<Article> GetArticlesByLastUpdateDate(DateTime date)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Article> GetArticlesByTag(string tag)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Article> GetArticlesByCategory(string category)
        {
            throw new NotImplementedException();
        }
    }
}