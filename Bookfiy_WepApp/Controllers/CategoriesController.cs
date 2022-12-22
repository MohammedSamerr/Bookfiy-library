using AutoMapper;
using Bookfiy_WepApp.Core.Const;
using Bookfiy_WepApp.Core.Mapping;
using Bookfiy_WepApp.Core.Models;

using Bookfiy_WepApp.Data;
using Bookfiy_WepApp.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Bookfiy_WepApp.Controllers
{
    [Authorize(Roles = AddRoles.Archive)]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public CategoriesController(ApplicationDbContext context , IMapper mapper)
        {
            _mapper = mapper ;
            _context = context;
        }

        public IActionResult Index()
        {
            var categories = _context.Categories.AsNoTracking().ToList(); 
            var voewModel = _mapper.Map<IEnumerable<CategoryViewModel>>(categories);
            return View(voewModel);
        }

        [HttpGet]
        [Ajax_]
        public  IActionResult Create()
        {

            return PartialView("_Form");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Create(CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var category = _mapper.Map<Category>(model);
            category.CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            _context.Categories.Add(category);
            _context.SaveChanges();

            var viewModel = _mapper.Map<CategoryViewModel>(category);
            return PartialView("_CategoryRow" , viewModel);
        }

        [HttpGet]
        [Ajax_]
        public IActionResult Edit(int id)
        {
            var category = _context.Categories.Find(id);

            if (category is null)
                return NotFound();

            var viewModel = _mapper.Map<CategoryFormViewModel>(category);

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
            category.LastUpdateById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            _context.SaveChanges();

            var viewModel = _mapper.Map<CategoryViewModel>(category);
            return PartialView("_CategoryRow", viewModel);
        }

        [HttpPost]
        public IActionResult ToggleStatus(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
                return NotFound();

            category.IsDelete = !category.IsDelete;
            category.LastUpdateOn = DateTime.Now;
            category.LastUpdateById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            _context.SaveChanges();
            return Ok(category.LastUpdateOn.ToString());
        }


        public IActionResult AllowItem(CategoryFormViewModel model)
        {
            //select item from database by name
            //we ensure that data is uniqe so we will use singleordefault
            var category = _context.Categories.SingleOrDefault(c => c.Name ==model.Name);
            var isAllowed = category is null || category.Id.Equals(model.Id);
            return Json(isAllowed);
        }
    }
}
