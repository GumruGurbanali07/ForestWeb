using Forest.Areas.Admin.Models.Articles;
using Forest.Data;
using Forest.Helper;
using Forest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace Forest.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    public class ArticleController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IWebHostEnvironment _env;



        public ArticleController(AppDbContext context, IHttpContextAccessor contextAccessor, IWebHostEnvironment env)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _env = env;
        }



        public IActionResult Index()
        {
            var articles = _context.Articles.Include(x => x.User).
                Include(x => x.ArticleTag).
                ThenInclude(x => x.Tag)
                .Where(x => x.IsDeleted == false)
                .Include(x => x.Category).ToList();
            return View(articles);
        }





        //public async Task<IActionResult> Create(List<int> tagIds, IFormFile Photo)
        public IActionResult Create()
        {
            CreateArticleDto m = new();
            m.Categories = _context.Category.ToList();
            m.Tags = _context.Tags.ToList();
            return View(m);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateArticleDto m)
        {
            m.Categories = _context.Category.ToList();
            m.Tags = _context.Tags.ToList();
            try
            {
                if(m.Photo == null)
                {
                    ModelState.AddModelError("Photo", "Shekil error");
                    return View(m);
                }

                if (m.tagIds == null)
                {
                    ModelState.AddModelError("tagIds", "tagIds error");
                    return View(m);
                }

                var userId = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                m.article.UserId = userId;
                m.article.CreatedDate = DateTime.Now;
                m.article.SeoUrl = m.article.Title.ReplaceInvalidChars();



                m.article.PhotoUrl = await m.Photo.SaveFileAsync(_env.WebRootPath);



                await _context.Articles.AddAsync(m.article);
                await _context.SaveChangesAsync();

                foreach (var item in m.tagIds)
                {
                    _context.ArticleTags.Add(new ArticleTag
                    {
                        ArticleId = m.article.Id,
                        TagId = item,
                    });
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(m);
            }
        }
        public IActionResult Edit(int id)
        {
            if (id == null) return NotFound();
            var articles = _context.Articles.FirstOrDefault(x => x.Id == id);
            if (articles == null) return NotFound();
            var categories = _context.Category.ToList();
            var tags = _context.Tags.ToList();
            ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");
            ViewData["Tags"] = tags;
            return View(articles);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Article article, List<int> tagIds, IFormFile Photo)
        {
            try
            {




                article.UpdatedDate = DateTime.Now;
                article.SeoUrl = SeoUrl.ReplaceInvalidChars(article.Title);


                if (Photo != null)
                {
                    article.PhotoUrl = await Photo.SaveFileAsync(_env.WebRootPath);
                }

                var art = _context.ArticleTags.Where(x => x.ArticleId == article.Id).ToList();
                _context.ArticleTags.RemoveRange(art);
                List<ArticleTag> articleTags = new();

                for (int i = 0; i < tagIds.Count; i++)
                {
                    ArticleTag articleTag = new()
                    {
                        ArticleId = article.Id,
                        TagId = tagIds[i],
                    };
                    articleTags.Add(articleTag);
                }
                await _context.ArticleTags.AddRangeAsync(articleTags);
                _context.Articles.Update(article);
                _context.SaveChanges();
                return RedirectToAction("Index");



            }
            catch (Exception ex)
            {

            }
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {

            var article = await _context.Articles.FirstOrDefaultAsync(x => x.Id == id);
            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}



