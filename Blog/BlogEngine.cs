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

        public string Name { get { return "Castle Babel"; } }
        public string Description { get { return @"那时，天下人的口音、言语，都是一样。 
他们往东边迁移的时候，在示拿地遇见一片平原，就住在那里。 
他们彼此商量说：“来吧！我们要作砖，把砖烧透了。”他们就拿砖当石头，又拿石漆当灰泥。 
他们说：“来吧！我们要建造一座城和一座塔，塔顶通天，为要传扬我们的名，免得我们分散在全地上。” 
耶和华降临，要看看世人所建造的城和塔。 
耶和华说：“看哪！他们成为一样的人民，都是一样的言语，如今既作起这事来，以后他们所要作的事，就没有不成就的了。 
我们下去，在那里变乱他们的口音，使他们的言语彼此不通。” 
于是，耶和华使他们从那里分散在全地上；他们就停工不造那城了。 
因为耶和华在那里变乱天下人的言语，使众人分散在全地上，所以那城名叫巴别(就是“变乱”的意思)。 "; } }

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
                    lock (_syncLock)
                    {
                        _dbConnection.Open();
                        _author = _dbConnection.Query<Author>("select * from Author limit 1;").FirstOrDefault();
                        _dbConnection.Close();
                    }
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
                    lock (_syncLock)
                    {
                        _dbConnection.Open();
                        _articles = _dbConnection.Query<Article>("select * from Article;").AsList();
                        _dbConnection.Close();
                        Refresh();
                    }
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

            lock (_syncLock)
            {
                _dbConnection.Open();
                int n =
                    _dbConnection.Execute(
                        "INSERT INTO Article(Title,Link,Content,Description,Category,Tags,CreateTime,LastUpdateTime,AuthorId) " +
                        "Values(@Title,@Link,@Content,@Description,@Category,@Tags,@CreateTime,@LastUpdateTime,@AuthorId);",
                        article);
                if (n > 0)
                {
                    int id = _dbConnection.Query<int>("select Id from Article where Link = @Link", article).Single();
                    article.Id = id;
                    _articles.Add(article);
                    Refresh();
                }
                _dbConnection.Close();
            }
        }

        public bool RemoveArticle(string link)
        {
            var article = _articles.FirstOrDefault(a => a.Link == link);
            if (article == null) 
                return false;

            lock (_syncLock)
            {
                _dbConnection.Open();
                if (_dbConnection.Execute("DELETE FROM Article WHERE Link=@Link;", article) > 0)
                {
                    _articles.Remove(article);
                    Refresh();
                }
                _dbConnection.Close();
            }
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
            lock (_syncLock)
            {
                _dbConnection.Open();
                int n =
                    _dbConnection.Execute(
                        "UPDATE `Article` SET `Title`=@Title,`Content`=@Content,`Description`=@Description,Link=@Link," +
                        "`Category`=@Category," +
                        "`Tags`=@Tags,`LastUpdateTime`=@LastUpdateTime WHERE `Id`=@Id;", temporary);
                if (n > 0)
                {
                    _articles.Remove(article);
                    _articles.Add(temporary);
                    Refresh();
                }
                _dbConnection.Close();
            }
        }

        private List<Info> _bufferInfoCategory;
        private List<Info> _bufferArchive;

        private void Refresh()
        {
            _articles.Sort();
            _bufferInfoCategory = _articles.GroupBy(a => a.Category).Select(g => new Info() { Key = g.Key, Count = g.Count() }).ToList();
            _bufferArchive = _articles.GroupBy(a => a.CreateTime.ToString("yyyy-MM")).Select(g => new Info() { Key = g.Key, Count = g.Count() }).ToList();
        }

        public IEnumerable<Info> GetCategory()
        {
            return _bufferInfoCategory;
        }

        public IEnumerable<Info> GetArchive()
        {
            return _bufferArchive;
        }
    }
}