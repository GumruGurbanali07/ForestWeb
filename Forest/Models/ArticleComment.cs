namespace Forest.Models
{
    public class ArticleComment
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public string UserId { get; set; }
        public DateTime PublishDate { get; set; }
        public User User { get; set; }
        public int ArticleId { get; set; }
        public Article Article { get; set; }
    }
}
