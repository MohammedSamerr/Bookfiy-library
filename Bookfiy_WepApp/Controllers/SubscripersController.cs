using Bookfiy_WepApp.Core.Const;
using Bookfiy_WepApp.Core.Models;
using Bookfiy_WepApp.Core.ViewModels;
using Bookfiy_WepApp.Data;
using Bookfiy_WepApp.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
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
        private readonly IDataProtector _dataProtector;
        private readonly IMapper _mapper;
        private readonly IimageService _imageService;
        public SubscripersController(ApplicationDbContext context, IMapper mapper, IimageService imageService, IDataProtectionProvider dataProtector)
        {
            _context = context;
            _mapper = mapper;
            _imageService = imageService;
            _dataProtector = dataProtector.CreateProtector("MySecureKy");
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Search

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Search(SearchFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var subscriber = _context.Subscripers
                            .SingleOrDefault(s =>
                                    s.Email == model.Value
                                || s.NantionalId == model.Value
                                || s.MobileNumber == model.Value);

            var viewModel = _mapper.Map<SubscriberSearchViewModel>(subscriber);

            if (subscriber is not null)
                viewModel.Key = _dataProtector.Protect(subscriber.Id.ToString());

            return PartialView("_Result", viewModel);
        }
        #endregion

        #region Details
        public IActionResult Details(string id)
        {
            var subscriberId = int.Parse(_dataProtector.Unprotect(id));

            var subscriber = _context.Subscripers
                .Include(s => s.Governorate)
                .Include(s => s.Area)
                .SingleOrDefault(s => s.Id == subscriberId);

            if (subscriber is null)
                return NotFound();

            var viewModel = _mapper.Map<SubscriperViewModel>(subscriber);
            viewModel.Key = id;

            return View(viewModel);
        }
        #endregion

        #region Create
        public IActionResult Create()
        {
            var viewModel = PopulateViewModel();
            return View("_Form", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubscriperFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("_Form", PopulateViewModel(model));

            var subscriber = _mapper.Map<Subscriper>(model);

            var imageName = $"{Guid.NewGuid()}{Path.GetExtension(model.Image!.FileName)}";
            var imagePath = "/images/Subscriber";

            var (isUploaded, errorMessage) = await _imageService.UploadAsync(model.Image, imageName, imagePath, hasThumbnail: true);

            if (!isUploaded)
            {
                ModelState.AddModelError("Image", errorMessage!);
                return View("_Form", PopulateViewModel(model));
            }

            subscriber.ImageUrl = $"{imagePath}/{imageName}";
            subscriber.ImageThumbnailUrl = $"{imagePath}/thumb/{imageName}";
            subscriber.CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            _context.Add(subscriber);
            _context.SaveChanges();

            //TODO: Send welcome email

            var subscriberId = _dataProtector.Protect(subscriber.Id.ToString());

            return RedirectToAction(nameof(Details), new { id = subscriberId });
        }
        #endregion

        #region Edit
        public IActionResult Edit(string id)
        {
            var subsciberId = int.Parse(_dataProtector.Unprotect(id));
            var subscriber = _context.Subscripers.SingleOrDefault(s => s.Id == subsciberId);
            if (subscriber is null)
                return NotFound();

            var model = _mapper.Map<SubscriperFormViewModel>(subscriber);

            var viewModel = PopulateViewModel(model);
            viewModel.Key = id;
            return View("_Form", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubscriperFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("_Form",PopulateViewModel(model));

            var subscriberId = int.Parse(_dataProtector.Unprotect(model.Key));

            var subscriper = _context.Subscripers.Find(subscriberId);

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
            return RedirectToAction(nameof(Details), new { id = model.Key });
        }
        #endregion

        #region Handel Selcet List
        [Ajax_]
        public IActionResult GetArea(int governoateId)
        {
            var areas = _context.Areas
                    .Where(a => a.GovernorateId == governoateId && !a.IsDelete)
                    .OrderBy(g => g.Name)
                    .ToList();

            return Ok(_mapper.Map<IEnumerable<SelectListItem>>(areas));
        }
        public SubscriperFormViewModel PopulateViewModel(SubscriperFormViewModel? model = null)
        {
            SubscriperFormViewModel viewModel = model is null ? new SubscriperFormViewModel() : model;

            var governorates = _context.Governorates.Where(a => !a.IsDelete).OrderBy(a => a.Name).ToList();
            viewModel.Governorates = _mapper.Map<IEnumerable<SelectListItem>>(governorates);

            if (model?.GovernorateId > 0)
            {
                var areas = _context.Areas
                    .Where(a => a.GovernorateId == model.GovernorateId && !a.IsDelete)
                    .OrderBy(a => a.Name)
                    .ToList();

                viewModel.Areas = _mapper.Map<IEnumerable<SelectListItem>>(areas);
            }

            return viewModel;
        }
        #endregion

        #region validation
        public IActionResult AllowNationalId(SubscriperFormViewModel model)
        {
            var subscriberId = 0;
            if(!string.IsNullOrEmpty(model.Key))
                subscriberId = int.Parse(_dataProtector.Unprotect(model.Key));

            var subscriper = _context.Subscripers.SingleOrDefault(s => s.NantionalId == model.NantionalId);
            
            var isAllowed = subscriper is null || subscriper.Id.Equals(subscriberId);
            return Json(isAllowed);
        }
        public IActionResult AllowMobileNumber(SubscriperFormViewModel model)
        {
            var subscriberId = 0;
            if (!string.IsNullOrEmpty(model.Key))
                subscriberId = int.Parse(_dataProtector.Unprotect(model.Key));

            var subscriper = _context.Subscripers.SingleOrDefault(s => s.MobileNumber == model.MobileNumber);
            var isAllowed = subscriper is null || subscriper.Id.Equals(subscriberId);
            return Json(isAllowed);
        }
        public IActionResult AllowEmail(SubscriperFormViewModel model)
        {
            var subscriberId = 0;
            if (!string.IsNullOrEmpty(model.Key))
                subscriberId = int.Parse(_dataProtector.Unprotect(model.Key));

            var subscriper = _context.Subscripers.SingleOrDefault(s => s.Email == model.Email);
            var isAllowed = subscriper is null || subscriper.Id.Equals(subscriberId);
            return Json(isAllowed);
        }
        #endregion


    }
}
