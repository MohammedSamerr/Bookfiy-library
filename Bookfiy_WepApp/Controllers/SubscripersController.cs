using Bookfiy_WepApp.Core.Const;
using Bookfiy_WepApp.Core.Models;
using Bookfiy_WepApp.Data;
using Bookfiy_WepApp.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixLabors.ImageSharp;
using Image = SixLabors.ImageSharp.Image;

namespace Bookfiy_WepApp.Controllers
{
    [Authorize(Roles = AddRoles.Reception)]
    public class SubscripersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IimageService _imageService;
        public SubscripersController(ApplicationDbContext context, IMapper mapper, IimageService imageService)
        {
            _context = context;
            _mapper = mapper;
            _imageService = imageService;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Create
        public IActionResult Create()
        {
            return View("_Form", PopulateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubscriperFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("_Form", PopulateViewModel(model));

            var subscriber = _mapper.Map<Subscriper>(model);

            if (model.Image is not null)
            {
                var imageName = $"{Guid.NewGuid()}{Path.GetExtension(model.Image.FileName)}";

                var result = await _imageService.UploadAsync(model.Image, imageName, "/images/Subscriber", hasThumbnail: true);

                if (!result.isUploded)
                {
                    ModelState.AddModelError("Image", result.errorMessage!);
                    return View("_Form", PopulateViewModel(model));

                }
                subscriber.ImageUrl = $"/images/Subscriber/{imageName}";
                subscriber.ImageThumbnailUrl = $"/images/Subscriber/thumb/{imageName}";
            }
            subscriber.CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            _context.Add(subscriber);
            _context.SaveChanges();
            return RedirectToAction(nameof(Details), new { id = subscriber.Id });

        }
        #endregion

        #region Edit
        public IActionResult Edit(int id)
        {
            var subscriber = _context.Subscripers.SingleOrDefault(s => s.Id == id);
            if (subscriber is null)
                return NotFound();

            var model = _mapper.Map<SubscriperFormViewModel>(subscriber);
            return View("_Form",PopulateViewModel(model));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubscriperFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("_Form",PopulateViewModel(model));

            var subscriper = _context.Subscripers.Find(model.Id);

            if (subscriper is null)
                return NotFound();

            if(model.Image is not null)
            {
                if (!string.IsNullOrEmpty(model.ImageUrl))
                    _imageService.Delete(subscriper.ImageUrl, subscriper.ImageThumbnailUrl);

                var imageName = $"{Guid.NewGuid()}{Path.GetExtension(model.Image.FileName)}";
                var (isUploded, errorMessage) = await _imageService.UploadAsync(model.Image, imageName, "/images/Subscriber", hasThumbnail: true);
                if (!isUploded)
                {
                    ModelState.AddModelError("Image", errorMessage!);
                    return View("_Form", PopulateViewModel(model));
                }
                model.ImageUrl = $"/images/Subscriber/{imageName}";
                model.ImageThumbnailUrl = $"/images/Subscriber/thumb/{imageName}";
            }
            else if (!string.IsNullOrEmpty(subscriper.ImageUrl))
            {
                model.ImageUrl = subscriper.ImageUrl;
                model.ImageThumbnailUrl = subscriper.ImageThumbnailUrl;
            }

            subscriper = _mapper.Map(model,subscriper);
            subscriper.CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            subscriper.LastUpdateOn = DateTime.Now;
            _context.SaveChanges();
            return RedirectToAction(nameof(Details), new { id = subscriper.Id });
        }
        #endregion

        #region Search

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> search(SearchFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(nameof(Index), model);

            var subscriber = await _context.Subscripers
                .SingleOrDefaultAsync(s => s.MobileNumber == model.Value ||
                s.NantionalId == model.Value || s.Email == model.Value && !s.IsDelete);

            var viewModel = _mapper.Map<SubscriberSearchViewModel>(subscriber);

            return PartialView("_Result", viewModel); 
        }
        #endregion

        #region Details
        public async Task<IActionResult> Details(int id)
        {
            var subscriber = await _context.Subscripers.Include(g=>g.Governorate).Include(a=>a.Area).SingleOrDefaultAsync(s => s.Id == id);

            if (subscriber is null)
                return BadRequest();

            var viewModel = _mapper.Map<SubscriperViewModel>(subscriber);
            return View(viewModel);
        }
        #endregion

        #region Handel Selcet List
        [Ajax_]
        public IActionResult GetArea(int governoateId)
        {
            var areas = _context.Areas.Where(a => a.GovernorateId == governoateId && !a.IsDelete)
                .OrderBy(g => g.Name).ToList();

            return Ok(_mapper.Map<IEnumerable<SelectListItem>>(areas));
        }
        public SubscriperFormViewModel PopulateViewModel(SubscriperFormViewModel? model = null)
        {
            SubscriperFormViewModel viewModel = model is null ? new SubscriperFormViewModel() : model;
            var governorate = _context.Governorates.Where(a => !a.IsDelete).OrderBy(a => a.Name).ToList();
            viewModel.Governorates = _mapper.Map<IEnumerable<SelectListItem>>(governorate);

            if (model?.GovernorateId > 0)
            {
                var areas = _context.Areas
                    .Where(g => g.GovernorateId == model.GovernorateId && !g.IsDelete)
                    .OrderBy(n => n.Name).ToList();
                viewModel.Areas = _mapper.Map<IEnumerable<SelectListItem>>(areas);
            }
            return viewModel;
        }
        #endregion

        #region validation
        public IActionResult AllowNationalId(SubscriperFormViewModel model)
        {
            var subscriper = _context.Subscripers.SingleOrDefault(s => s.NantionalId == model.NantionalId);
            var isAllowed = subscriper is null || subscriper.Id.Equals(model.Id);
            return Json(isAllowed);
        }
        public IActionResult AllowMobileNumber(SubscriperFormViewModel model)
        {
            var subscriper = _context.Subscripers.SingleOrDefault(s => s.MobileNumber == model.MobileNumber);
            var isAllowed = subscriper is null || subscriper.Id.Equals(model.Id);
            return Json(isAllowed);
        }

        public IActionResult AllowEmail(SubscriperFormViewModel model)
        {
            var subscriper = _context.Subscripers.SingleOrDefault(s => s.Email == model.Email);
            var isAllowed = subscriper is null || subscriper.Id.Equals(model.Id);
            return Json(isAllowed);
        }
        #endregion


    }
}
