using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperCommonScenarios.API.Models
{
    public class Blog
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Contents { get; set; }
        public List<BlogComment> BlogCommentList { get; set; } = new List<BlogComment>();
    }

    public class BlogComment
    {
        public int Id { get; set; }
        public int BlogId { get; set; }
        public string Comment { get; set; }
        public int? NumberOfStars { get; set; }
    }
}
