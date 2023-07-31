using Forest.Data;
using Forest.Helper;
using Forest.Models;
using Microsoft.AspNetCore.Mvc;
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

        //[HttpGet]
        //public async Task<IActionResult> Index(string Empsearch)
        //{
        //    ViewData["Getarticlenames"] = Empsearch;
        //    var empquery = from x in _context.Articles select x;
        //    if (!String.IsNullOrEmpty(Empsearch))
        //    {
        //        empquery = empquery.Where(x => x.ArticleTag.Contains(Empsearch));
        //    }
        //    return View(await empquery.AsNoTracking().ToListAsync());
        //}

        public async Task<IActionResult> Create(List<int> tagIds, IFormFile Photo)
        {
            var categories = _context.Category.ToList();
            var tags = _context.Tags.ToList();
            ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");
            ViewData["Tags"] = tags;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Article article, List<int> tagIds, IFormFile Photo)
        {
            try
            {
                var categories = await _context.Category.ToListAsync();
                var tags = await _context.Tags.ToListAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");
                ViewData["Tags"] = tags;


                var userId = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                article.UserId = userId;
                article.CreatedDate = DateTime.Now;
                article.SeoUrl = article.Title.ReplaceInvalidChars();

           

                article.PhotoUrl = await Photo.SaveFileAsync(_env.WebRootPath);

               

                await _context.Articles.AddAsync(article);
                await _context.SaveChangesAsync();

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
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
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

        //public async Task<IActionResult> Delete(int id)
        //{
        //    if (id == null) return NotFound();

        //    if (article == null) return NotFound();
        //    return View();
        //}
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



