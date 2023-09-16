using Forest.Data;
using Forest.DTOs;
using Forest.Helper;
using Forest.Models;
using Forest.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace Forest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public HomeController(ILogger<HomeController> logger, AppDbContext context, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _context = context;
            _contextAccessor = contextAccessor;
        }



        public IActionResult Index(int pg = 1)
        {
            const int pageSize = 9;
            if (pg < 1)
            {
                pg = 1;
            }

            int articleCount = _context.Articles.Count();

            var pager = new Pager(articleCount, pg, pageSize);

            int artSkip = (pg - 1) * pageSize;

            var articles = _context.Articles
                .Include(x => x.User)
                .Include(x => x.Category)
                .OrderByDescending(x => x.CreatedDate)
                .Skip(artSkip)
                .Take(pager.PageSize)
                .ToList();
            var firstArticle = _context.Articles
                 .Include(x => x.Category).Include(x => x.User)
                 .OrderByDescending(x => x.Id)
                 .FirstOrDefault();
            var popularCategories = _context.Articles.GroupBy(article => article.Category.CategoryName).AsEnumerable().Select(group => new KeyValuePair<string, int>(group.Key, group.Count())).OrderByDescending(pair => pair.Value).Take(3).ToList();


            HomeVM homeVM = new()
            {
                Articles = articles,
                FirstSlot = firstArticle,
                PopularCategories = popularCategories,

            };
            ViewBag.Pager = pager;

            var recentPosts = _context.Articles
               .Include(x => x.User)
               .Include(x => x.Category)
               .OrderByDescending(x => x.CreatedDate)
               .Take(5)
               .ToList();

            ViewBag.RecentPosts = recentPosts;

            return View(homeVM);
        }
        public async Task<IActionResult> Detail(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.CurrentUser = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            }

            if (id == null) return NotFound();
            var article = _context.Articles.Include(x => x.Category).Include(x => x.User).Include(x => x.ArticleTag).ThenInclude(x => x.Tag).SingleOrDefault(x => x.Id == id);

            var cookie = _contextAccessor.HttpContext.Request.Cookies[$"Views"];
            string[] findCookie = { "" };

            if (cookie != null)
            {
                findCookie = cookie.Split('-').ToArray();
            }

            if (!findCookie.Contains(article.Id.ToString()))
            {
                Response.Cookies.Append($"Views", $"{cookie}-{article.Id}",
                    new CookieOptions
                    {
                        Secure = true,
                        HttpOnly = true,
                        Expires = DateTime.Now.AddYears(1),
                    });

                article.ViewCount += 1;
                _context.Articles.Update(article);
                await _context.SaveChangesAsync();
            }









            if (article == null) return NotFound();
            var popart = _context.Articles.OrderByDescending(x => x.ViewCount).Take(3).ToList();
            var comments = _context.ArticleComments.Include(x => x.User).Where(x => x.ArticleId == article.Id).ToList();
            var nextArticle = _context.Articles.FirstOrDefault(x => x.Id < id);
            var prevArticle = _context.Articles.FirstOrDefault(x => x.Id > id);
            var similarArticle = _context.Articles.Include(x => x.Category).OrderByDescending(x => x.Id).Where(x => x.CategoryId == article.CategoryId && x.Id != article.Id).Take(2).ToList();

            var popularCategories = _context.Articles.GroupBy(article => article.Category.CategoryName).AsEnumerable().Select(group => new KeyValuePair<string, int>(group.Key, group.Count())).OrderByDescending(pair => pair.Value).Take(3).ToList();
            foreach (var comment in comments)
            {
                ViewData[$"PublishDateAgo_{comment.Id}"] = CommentTime.GetTimeAgo(comment.PublishDate);
            }



            DetailVM detailVM = new()
            {
                Article = article,
                PopularArticle = popart,
                ArticleComments = comments,
                NextArticle = nextArticle,
                PrevArticle = prevArticle,
                SimilarArticle = similarArticle,
                PopularCategories= popularCategories,

            };
            return View(detailVM);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(AddCommentDto m)
        {
            if (m.Id != null && m.CommentText != null)
            {
                var UserId = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                await _context.ArticleComments.AddAsync(new ArticleComment
                {
                    ArticleId = m.Id,
                    Comment = m.CommentText,
                    PublishDate = DateTime.Now,
                    UserId = UserId,
                });
                await _context.SaveChangesAsync();
                return RedirectToAction("Detail", "Home", new { Id = m.Id });
            }
            return RedirectToAction("Detail", "Home", new { Id = m.Id });
        }

        //[HttpPost]
        //public async Task<IActionResult> AddComment(ArticleComment articleComment, int articleId)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        articleComment.UserId = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

        //        _context.ArticleComments.Add(articleComment);
        //        await _context.SaveChangesAsync();

        //        return RedirectToAction("Index", "Home", new { Id = articleId });
        //    }

        //    // If ModelState is not valid, return to the "AddComment" view with validation errors
        //    ViewData["articleId"] = articleId;
        //    return View("AddComment", articleComment);
        //}



        [HttpPost]
        public async Task<IActionResult> DeleteComment(int commentId, int articleId)
        {
            var artCom = _context.ArticleComments.FirstOrDefault(x => x.Id == commentId);
            _context.ArticleComments.Remove(artCom);
            await _context.SaveChangesAsync();
            return RedirectToAction("Detail", "Home", new { Id = articleId });
        }

        public IActionResult Search(string q, int pg = 1)
        {

            const int pageSize = 9;
            if (pg < 1)
            {
                pg = 1;
            }

            //var model = _context.Articles.AsQueryable(); 
            //model.Where(x => x.Title.Contains(q)).ToList();
            int articleCount = _context.Articles.Where(x => x.Title.Contains(q)).Count();

            var pager = new Pager(articleCount, pg, pageSize);

            int artSkip = (pg - 1) * pageSize;

            var articles = _context.Articles.Where(x => x.Title.Contains(q))
                .Include(x => x.User)
                .Include(x => x.Category)
                .OrderByDescending(x => x.CreatedDate)
                .Skip(artSkip)
                .Take(pager.PageSize)
                .ToList();
            var firstArticle = _context.Articles
                 .Include(x => x.Category).Include(x => x.User)
                 .OrderByDescending(x => x.Id)
                 .FirstOrDefault();
            var popularCategories = _context.Articles.GroupBy(article => article.Category.CategoryName).AsEnumerable().Select(group => new KeyValuePair<string, int>(group.Key, group.Count())).OrderByDescending(pair => pair.Value).Take(3).ToList();


            HomeVM homeVM = new()
            {
                Articles = articles,
                FirstSlot = firstArticle,
                PopularCategories = popularCategories,
                q = q
            };
            ViewBag.Pager = pager;

            var recentPosts = _context.Articles
               .Include(x => x.User)
               .Include(x => x.Category)
               .OrderByDescending(x => x.CreatedDate)
               .Take(5)
               .ToList();

            ViewBag.RecentPosts = recentPosts;

            return View(homeVM);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

        public IActionResult Privacy()
        {
            return View();
        }

       
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}