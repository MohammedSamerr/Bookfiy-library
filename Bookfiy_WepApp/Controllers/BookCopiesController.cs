using AutoMapper;
using Bookfiy_WepApp.Core.Const;
using Bookfiy_WepApp.Core.Models;
using Bookfiy_WepApp.Data;
using Bookfiy_WepApp.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace Bookfiy_WepApp.Controllers
{
    [Authorize(Roles = AddRoles.Archive)]
    public class BookCopiesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public BookCopiesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleStatus(int id)
        {
            var books = _context.BookCopies.Find(id);

            if (books is null)
                return NotFound();

            books.IsDelete = !books.IsDelete;
            books.LastUpdateById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            books.LastUpdateOn = DateTime.Now;

            _context.SaveChanges();

            return Ok();
        }
        /*////////////////////////////////////////////////*/

        [Ajax_]
        public IActionResult Create(int bookId)
        {
            var book = _context.Books.Find(bookId);
            if (book is null)
                return NotFound();


            var viewModel = new BoooCopyFormViewModel
            {
                BookId = bookId,
                ShowRentalInput = book.IsAvailabbleForRent
            };
            return PartialView("Form", viewModel);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Create(BoooCopyFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var book = _context.Books.Find(model.BookId);

            if (book is null)
                return NotFound();

            var copy = new BookCopy
            {
                EdditionNumber = model.EdditionNumber,
                IsAvailabbleForRent = book.IsAvailabbleForRent ? model.IsAvailabbleForRent : false,
                CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value
        };

            book.Copies.Add(copy);
            _context.SaveChanges();

            var viewModel = _mapper.Map<BookCopyViewModel>(copy);
            return PartialView("_BookCopyRow", viewModel);
        }

        [Ajax_]
        public IActionResult Edit(int id)
        {
            var copy = _context.BookCopies.Include(b => b.Book).SingleOrDefault(c =>c.Id == id);

            if (copy is null)
                return NotFound();

            var viewModel = _mapper.Map<BoooCopyFormViewModel>(copy);
            viewModel.ShowRentalInput = copy.Book!.IsAvailabbleForRent;

            return PartialView("Form", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BoooCopyFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var copy = _context.BookCopies.Include(c => c.Book).SingleOrDefault(c => c.Id ==model.Id);

            if (copy is null)
                return NotFound();

            copy.EdditionNumber = model.EdditionNumber;
            copy.IsAvailabbleForRent = model.IsAvailabbleForRent;
            copy.LastUpdateOn = DateTime.Now;
            copy.LastUpdateById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            _context.SaveChanges();

            var viewModel = _mapper.Map<BookCopyViewModel>(copy);
            return PartialView("_BookCopyRow", viewModel);
        }

    }
}
