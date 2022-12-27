using AutoMapper;
using Bookfiy_WepApp.Core.Const;
using Bookfiy_WepApp.Core.Models;
using Bookfiy_WepApp.Core.ViewModels;
using Bookfiy_WepApp.Data;
using Bookfiy_WepApp.Filters;
using Bookfiy_WepApp.Services;
using Bookfiy_WepApp.settings;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NuGet.Packaging.Signing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;
using static System.Reflection.Metadata.BlobBuilder;
using Image = SixLabors.ImageSharp.Image;

namespace Bookfiy_WepApp.Controllers
{
    [Authorize(Roles = AddRoles.Archive)]
    public class BooksController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment; // to get wwwroot path
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;
        private List<string> _allowedExtensions = new() { ".jpg", ".jpeg", ".png" };
        private int _maxAllowedSize = 2097152;
        private readonly IimageService _imageService;
        public BooksController(ApplicationDbContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment
            , IOptions<CloudinarySettings> cloudinary
            , IimageService imageService)

        {
            _mapper = mapper;
            _context = context;
            _webHostEnvironment = webHostEnvironment;

            Account account = new()
            {
                Cloud = cloudinary.Value.CloudName,
                ApiKey = cloudinary.Value.APIKey,
                ApiSecret = cloudinary.Value.APISecret
            };

            _cloudinary = new Cloudinary(account);
            _imageService = imageService;
        }
        public IActionResult Index()
        {

            return View();
        }
        
        [HttpPost]
        public IActionResult GetBooks()
        {
            //Request from datatable comes on form
            var skip = int.Parse(Request.Form["start"]);
            var pageSize = int.Parse(Request.Form["length"]);

            var searchValue = Request.Form["search[value]"];

            var sortColumnIndex = Request.Form["order[0][column]"];
            var sortColumn = Request.Form[$"columns[{sortColumnIndex}][name]"];
            var sortColumnDirection = Request.Form["order[0][dir]"];

            IQueryable<Book> book = _context.Books.Include(a=>a.Author).Include(c => c.Categories)
                 .ThenInclude(c => c.Category);

            if (!string.IsNullOrEmpty(searchValue))
                book = book.Where(b => b.Title.Contains(searchValue) || b.Author!.Name.Contains(searchValue));

            book = book.OrderBy($"{sortColumn} {sortColumnDirection}");

            var data = book.Skip(skip).Take(pageSize).ToList();
            var mappedDate = _mapper.Map<ICollection<BookViewModel>>(data);
            var recordsTotal = book.Count();

            var jsonData = new { recordsFiltered = recordsTotal, recordsTotal, data= mappedDate };
            return Ok(jsonData);
        }

        public IActionResult Details(int id)
        {
            var book = _context.Books.Include(a => a.Author)
                 .Include(bc => bc.Copies)
                 .Include(c => c.Categories)
                 .ThenInclude(c => c.Category)
                 .SingleOrDefault(b => b.Id == id);
            if (book is null)
                return NotFound();

            var viewModel = _mapper.Map<BookViewModel>(book);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleStatus(int id)
        {
            var books = _context.Books.Find(id);

            if (books is null)
                return NotFound();

            books.IsDelete = !books.IsDelete;
            books.LastUpdateOn = DateTime.Now;
            books.LastUpdateById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            _context.SaveChanges();

            return Ok();
        }


        public IActionResult Create()
        {
            var viewModel = PopulateViewModel();
            return View("Form", viewModel);
        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookFormViewModel model)
        {

            if (!ModelState.IsValid)
                return View("Form", PopulateViewModel(model));

            var book = _mapper.Map<Book>(model);

            if (model.Images is not null)
            {
                var imageName = $"{Guid.NewGuid()}{Path.GetExtension(model.Images.FileName)}";

                var result = await _imageService.UploadAsync(model.Images, imageName, "/images/Books",hasThumbnail: true);

                if (result.isUploded) { 
                book.ImageURL = $"/images/Books/{imageName}";
                book.ImageThumnailURL = $"/images/Books/thumb/{imageName}";
                }
                else
                {
                    ModelState.AddModelError(nameof(Image), result.errorMessage!);
                    return View("Form",PopulateViewModel(model));
                }
                //using var straem = model.Image.OpenReadStream();

                //var imageParams = new ImageUploadParams
                //{
                //    File = new FileDescription(imageName, straem),
                //    UseFilename = true
                //};

                //var result = await _cloudinary.UploadAsync(imageParams);

                //book.ImageUrl = result.SecureUrl.ToString();
                //book.ImageThumbnailUrl = GetThumbnailUrl(book.ImageUrl);
                //book.ImagePublicId = result.PublicId;
            }
            book.CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            foreach (var category in model.SelectedCategories)
                book.Categories.Add(new Books_Categories { CategoryId = category });

            _context.Add(book);
            _context.SaveChanges();

            return RedirectToAction(nameof(Details), new { id = book.Id });
        }

        public IActionResult Edit(int id)
        {
            // select book with related data
            var book = _context.Books.Include(b => b.Categories).SingleOrDefault(b => b.Id == id);
            if (book is null)
                return NotFound();

            var model = _mapper.Map<BookFormViewModel>(book);
            var viewModel = PopulateViewModel(model);

            viewModel.SelectedCategories = book.Categories.Select(c => c.CategoryId).ToList();

            return View("Form", viewModel);


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BookFormViewModel model)
        {


            if (!ModelState.IsValid)
            {
                return View("Form", PopulateViewModel(model));
            }
            var book = _context.Books.Include(b => b.Categories).Include(c=>c.Copies).SingleOrDefault(b => b.Id == model.Id);


            if (book is null)
                return NotFound();

            //string imagePublicId = null;

            if (model.Images is not null)
            {
                if (!string.IsNullOrEmpty(book.ImageURL))
                {
                    _imageService.Delete(book.ImageURL, book.ImageThumnailURL);

                    //await _cloudinary.DeleteResourcesAsync(book.ImagePublicId);
                }
                var imageName = $"{Guid.NewGuid()}{Path.GetExtension(model.Images.FileName)}";

                var result = await _imageService.UploadAsync(model.Images, imageName, "/images/Books", hasThumbnail: true);

                if (!result.isUploded)
                {
                    ModelState.AddModelError(nameof(Image), result.errorMessage!);
                    return View("Form", PopulateViewModel(model));
                }
                
                model.ImageURL = $"/images/Books/{imageName}";
                model.ImageThumbnailUrl = $"/images/Books/thumb/{imageName}";

                //using var stream = model.Image.OpenReadStream();
                //var imageParams = new ImageUploadParams
                //{
                //    File = new FileDescription(imageName, stream),
                //    //to uplode my image name 
                //    UseFilename = true
                //    //options
                //    //Transformation = new Transformation().Height(300).Width(500).Radius("max").Gravity("face").Crop("fill")

                //};

                //var result = await _cloudinary.UploadAsync(imageParams);
                //model.ImageURL = result.SecureUrl.ToString();
                //imagePublicId = result.PublicId;

            }
            else if (!string.IsNullOrEmpty(book.ImageURL))
            {
                model.ImageURL = book.ImageURL;
                model.ImageThumbnailUrl = book.ImageThumnailURL;
            }

            book = _mapper.Map(model, book);
            book.LastUpdateOn = DateTime.Now;
            book.LastUpdateById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            //book.ImageThumnailURL = GetThumbnailUrl(book.ImageURL!);
            //book.ImagePublicId = imagePublicId;
            
            foreach (var category in model.SelectedCategories)
            {
                book.Categories.Add(new Books_Categories
                {
                    CategoryId = category
                });
            }
            if (!model.IsAvailabbleForRent)
            {
                foreach (var copy in book.Copies)
                {
                    copy.IsAvailabbleForRent = false;
                }
            }
            _context.SaveChanges();


            return RedirectToAction(nameof(Details), new { id = book.Id });
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

        private string GetThumbnailUrl(string url)
        {

            var separator = "image/upload/";
            var urlParts = url.Split(separator);

            var thumbnailUrl = $"{urlParts[0]}{separator}c_thumb,w_200,g_face/{urlParts[1]}";

            return thumbnailUrl;
        }

    }
}
