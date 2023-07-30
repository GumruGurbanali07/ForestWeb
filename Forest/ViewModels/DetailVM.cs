using Forest.Models;

namespace Forest.ViewModels
{
    public class DetailVM
    {
        public Article Article { get; set; }
        public Article NextArticle { get; set; }
        public Article PrevArticle { get; set; }
        public List<Article> SimilarArticle { get; set; }
        public List<Article> PopularArticle { get; set; }
        public List<ArticleComment> ArticleComments { get; set; }
    }
}
