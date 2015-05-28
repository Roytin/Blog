using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog
{
    /// <summary>
    /// 作者
    /// </summary>
    [Serializable]
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
}