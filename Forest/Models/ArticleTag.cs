using Forest.Models;

namespace Forest.Models
{
    public class ArticleTag
    {
        //coxun coxa elaqeleri
        public int Id { get; set; }
        public int ArticleId { get; set; }
        public Article Article { get; set; }

        public int TagId { get; set; }
        public Tag Tag { get; set; }

    }
}
