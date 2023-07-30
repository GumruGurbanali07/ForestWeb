using Forest.Models;

namespace Forest.Models
{
    public class Category : BaseEntity
    {
        public string CategoryName { get; set; }
        public List<Article>? Article { get; set; }

    }
}
