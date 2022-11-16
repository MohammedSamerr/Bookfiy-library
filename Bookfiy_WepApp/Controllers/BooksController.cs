using AutoMapper;
using Bookfiy_WepApp.Core.Const;
using Bookfiy_WepApp.Core.Models;
using Bookfiy_WepApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Bookfiy_WepApp.Controllers
{
    public class BooksController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment; // to get wwwroot path
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private List<string> _allowedExtensions = new() { ".jpg", ".jpeg", ".png" };
        private int _maxAllowedSize = 2097152;
        public BooksController(ApplicationDbContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _mapper = mapper;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {

            return View();
        }

        public IActionResult Create()
        {
            var viewModel = PopulateViewModel();
            return View("Form", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BookFormViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View("Form", PopulateViewModel(model));
            }

            var book = _mapper.Map<Book>(model);
            if(model.Image is not null)
            {
                var extension = Path.GetExtension(model.Image.FileName);

                if (!_allowedExtensions.Contains(extension))
                {
                        ModelState.AddModelError(nameof(model.Image), Error.NotAllowedExtensions);
                        return View("Form", PopulateViewModel(model));
                }
                if(model.Image.Length > _maxAllowedSize)
                {
                    ModelState.AddModelError(nameof(model.Image), Error.maxSize);
                    return View("Form", PopulateViewModel(model));
                }
                var imageName = $"{Guid.NewGuid()}{extension}";

                //_webHostEnvironment.WebRootPath = Path.GetDirectoryName(model.Image.FileName) ====> goes to wwwroot
                var path = Path.Combine($"{_webHostEnvironment.WebRootPath}/images/Books", imageName);
                //Move image to server
                using var stream = System.IO.File.Create(path);
                model.Image.CopyTo(stream);

                book.ImageURL = imageName;
            }
            foreach (var category in model.SelectedCategories)
            {
                book.Categories.Add(new Books_Categories
                {
                    CategoryId = category
                }); 
            }
            _context.Add(book);
            _context.SaveChanges();


            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            // select book with related data
            var book = _context.Books.Include(b => b.Categories).SingleOrDefault(b => b.Id == id);
            if(book is null)
                return NotFound();

            var model = _mapper.Map<BookFormViewModel>(book);
            var viewModel = PopulateViewModel(model);

            viewModel.SelectedCategories = book.Categories.Select(c => c.CategoryId).ToList();

            return View("Form", viewModel);


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BookFormViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View("Form", PopulateViewModel(model));
            }
            var book = _context.Books.Include(b => b.Categories).SingleOrDefault(b => b.Id == model.Id);
            if (book is null)
                return NotFound();

            if (model.Image is not null)
            {
                if (!string.IsNullOrEmpty(book.ImageURL))
                {
                    var oldImage = Path.Combine($"{_webHostEnvironment.WebRootPath}/images/Books", book.ImageURL);
                    if(System.IO.File.Exists(oldImage))
                        System.IO.File.Delete(oldImage);
                }
                var extension = Path.GetExtension(model.Image.FileName);

                if (!_allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(nameof(model.Image), Error.NotAllowedExtensions);
                    return View("Form", PopulateViewModel(model));
                }
                if (model.Image.Length > _maxAllowedSize)
                {
                    ModelState.AddModelError(nameof(model.Image), Error.maxSize);
                    return View("Form", PopulateViewModel(model));
                }
                var imageName = $"{Guid.NewGuid()}{extension}";

                //_webHostEnvironment.WebRootPath = Path.GetDirectoryName(model.Image.FileName) ====> goes to wwwroot
                var path = Path.Combine($"{_webHostEnvironment.WebRootPath}/images/Books", imageName);
                //Move image to server
                using var stream = System.IO.File.Create(path);
                model.Image.CopyTo(stream);

                model.ImageURL = imageName;
            }
            else if(model.Image is null && !string.IsNullOrEmpty(book.ImageURL))
                model.ImageURL = book.ImageURL;
            
            book = _mapper.Map(model, book);
            book.LastUpdateOn = DateTime.Now;

            foreach (var category in model.SelectedCategories)
            {
                book.Categories.Add(new Books_Categories
                {
                    CategoryId = category
                });
            }

            _context.SaveChanges();


            return RedirectToAction(nameof(Index));
        }
        public IActionResult AllowItem(BookFormViewModel model)
        {
            var book = _context.Books.SingleOrDefault(b => b.Title == model.Title && b.AuthorId == model.AuthorId);
            var isAllowed = book is null || book.Id.Equals(model.Id);

            return Json(isAllowed);
        }
        private BookFormViewModel PopulateViewModel(BookFormViewModel? model = null)
        {
            BookFormViewModel viewModel = model is null ? new BookFormViewModel() : model;

            var authors = _context.Authors.Where(a => !a.IsDelete).OrderBy(a => a.Name).ToList();
            var categories = _context.Categories.Where(a => !a.IsDelete).OrderBy(a => a.Name).ToList();
            viewModel.Author = _mapper.Map<IEnumerable<SelectListItem>>(authors);
            viewModel.Categories = _mapper.Map<IEnumerable<SelectListItem>>(categories);
            return viewModel;
        }

    }
}
