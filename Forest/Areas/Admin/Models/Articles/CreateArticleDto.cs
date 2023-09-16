using Forest.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Forest.Areas.Admin.Models.Articles
{
    public class CreateArticleDto
    {
        public Article article { get; set; }
        [Required(ErrorMessage = "Choose tag")]
        public List<int> tagIds { get; set; }
        [Required(ErrorMessage = "Choose photo")]
        public IFormFile Photo { get; set; }
        public IEnumerable<Category> Categories { get; set; } 
        public IEnumerable<Tag> Tags { get; set; }
    }
}
