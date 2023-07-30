using Forest.Data;
using Forest.Models;
using Microsoft.AspNetCore.Mvc;

namespace Forest.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var categories = _context.Category.ToList();
            return View(categories);
        }
        [HttpGet]
        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(category);
                }
                category.CreatedDate = DateTime.Now;
                _context.Category.Add(category);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }

        }


        public IActionResult Detail(int? id)
        {
            if (id == null) return NotFound();
            var categoryName = _context.Category.FirstOrDefault(x => x.Id == id);
            if (categoryName == null) return NotFound();
            return View(categoryName);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();
            var categoryName = _context.Category.FirstOrDefault(x => x.Id == id);
            if (categoryName == null) return NotFound();
            return View(categoryName);
        }

        [HttpPost]
        public IActionResult Delete(Category category)
        {
            category.DeletedTime = DateTime.Now;
            _context.Category.Remove(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();
            var tagName = _context.Category.FirstOrDefault(x => x.Id == id);
            if (tagName == null) return NotFound();
            return View(tagName);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(category);
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        category.UpdatedDate = DateTime.Now;
                        _context.Category.Update(category);
                        _context.SaveChanges();
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }
    }
}

