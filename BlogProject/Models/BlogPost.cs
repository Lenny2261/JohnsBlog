using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlogProject.Models
{
    public class BlogPost
    {

        public BlogPost()
        {
            this.comments = new HashSet<Comment>();
        }

        public int id { get; set; }
        public string authorID { get; set; }
        public string abstractBody { get; set; }
        public int categoryID { get; set; }
        public DateTimeOffset  created { get; set; }
        public DateTimeOffset? updated { get; set; }
        public string title { get; set; }
        public string slug { get; set; }

        [AllowHtml]
        public string body { get; set; }
        public string mediaURL { get; set; }
        public bool published { get; set; }

        public virtual ICollection<Comment> comments { get; set; }
        public virtual ApplicationUser author { get; set; }
        public virtual Category category { get; set; }
    }
}