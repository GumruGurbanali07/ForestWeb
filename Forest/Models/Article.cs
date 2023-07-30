using Forest.Models;
using System.ComponentModel;

namespace Forest.Models
{
    public class Article : BaseEntity
    {

        public string Title { get; set; }
        public string Content { get; set; }

        public int ViewCount { get; set; }


        //bir userin cox article ola biler
        public string UserId { get; set; } //sql ucun 
        public User User { get; set; }// C# ucundu 
        public string SeoUrl { get; set; }
        public string PhotoUrl { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public List<ArticleTag> ArticleTag { get; set; }


    }
}
