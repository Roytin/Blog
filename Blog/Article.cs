using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog
{

    /// <summary>
    /// 文章
    /// </summary>
    public class Article : IComparable<Article>
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public string Content { get; set; }

        public string Tags { get; set; }

        public string Description { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime LastUpdateTime { get; set; }

        public string Category { get; set; }

        public string Link { get; set; }

        public int AuthorId { get; set; }

        public int CompareTo(Article other)
        {
            return other.CreateTime.CompareTo(CreateTime);
        }
    }
}