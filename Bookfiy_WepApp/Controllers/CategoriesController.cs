using Bookfiy_WepApp.Core.Models;

using Bookfiy_WepApp.Data;
using Bookfiy_WepApp.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bookfiy_WepApp.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.Categories.AsNoTracking().ToList());
        }

        [HttpGet]
        [Ajax_]
        public IActionResult Create()
        {

            return PartialView("_Form");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Create(CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var category = new Category
            {
                Name = model.Name,
            };
            _context.Categories.Add(category);
            _context.SaveChanges();

            
            return PartialView("_CategoryRow" , category);
        }

        [HttpGet]
        [Ajax_]
        public IActionResult Edit(int id)
        {
            var category = _context.Categories.Find(id);

            if (category is null)
                return NotFound();

            var viewModel = new CategoryFormViewModel
            {
                Id = id,
                Name = category.Name
            };

            return PartialView("_Form", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var category = _context.Categories.Find(model.Id);

            if (category is null)
                return NotFound();

            category.Name = model.Name;
            category.LastUpdateOn = DateTime.Now;

            _context.SaveChanges();



            return PartialView("_CategoryRow", category);
        }
        [HttpPost]
       

        public IActionResult ToggleStatus(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
                return NotFound();

            category.IsDelete = !category.IsDelete;
            category.LastUpdateOn = DateTime.Now;
            _context.SaveChanges();
            return Ok(category.LastUpdateOn.ToString());
        }
    }
}
