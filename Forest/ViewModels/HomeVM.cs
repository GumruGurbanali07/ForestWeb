using Forest.Models;

namespace Forest.ViewModels
{
    public class HomeVM
    {
        public List<Article> Articles { get; set; }
        public Article FirstSlot { get; set; }
        public List<Article> AllSlot { get; set;}

        public List<KeyValuePair<string, int>> PopularCategories { get; set; }
        public string q { get; set; }
    }
}
