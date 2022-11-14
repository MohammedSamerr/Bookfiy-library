using AutoMapper;
using Bookfiy_WepApp.Core.Models;
using Bookfiy_WepApp.Data;
using Bookfiy_WepApp.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bookfiy_WepApp.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AuthorsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var authors = _context.Authors.AsNoTracking().ToList();

            var viewModel = _mapper.Map<IEnumerable<AuthorViewModel>>(authors);

            return View(viewModel);
        }

        [HttpGet]
        [Ajax_]
        public IActionResult Create()
        {
            return PartialView("_Form");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AuthorFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var author = _mapper.Map<Author>(model);
            _context.Add(author);
            _context.SaveChanges();

            var viewModel = _mapper.Map<AuthorViewModel>(author);

            return PartialView("_AuthorRow", viewModel);
        }

        [HttpGet]
        [Ajax_]
        public IActionResult Edit(int id)
        {
            var author = _context.Authors.Find(id);

            if (author is null)
                return NotFound();

            var viewModel = _mapper.Map<AuthorFormViewModel>(author);

            return PartialView("_Form", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(AuthorFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var author = _context.Authors.Find(model.Id);

            if (author is null)
                return NotFound();

            author = _mapper.Map(model, author);
            author.LastUpdateOn = DateTime.Now;

            _context.SaveChanges();

            var viewModel = _mapper.Map<AuthorViewModel>(author);

            return PartialView("_AuthorRow", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleStatus(int id)
        {
            var author = _context.Authors.Find(id);

            if (author is null)
                return NotFound();

            author.IsDelete = !author.IsDelete;
            author.LastUpdateOn = DateTime.Now;

            _context.SaveChanges();

            return Ok(author.LastUpdateOn.ToString());
        }

        public IActionResult AllowItem(AuthorFormViewModel model)
        {
            var author = _context.Authors.SingleOrDefault(c => c.Name == model.Name);
            var isAllowed = author is null || author.Id.Equals(model.Id);

            return Json(isAllowed);
        }
    }
}
