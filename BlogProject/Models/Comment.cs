using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogProject.Models
{
    public class Comment
    {
        public int id { get; set; }
        public int postID { get; set;  }
        public string authorID { get; set; }
        public string body { get; set;  }
        public DateTimeOffset created { get; set;  }
        public DateTimeOffset? updated { get; set; }
        public string updateReason { get; set;  }

        public virtual BlogPost post { get; set; }
        public virtual ApplicationUser author { get; set; }
    }
}