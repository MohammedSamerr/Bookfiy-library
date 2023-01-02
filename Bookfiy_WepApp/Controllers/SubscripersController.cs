using Bookfiy_WepApp.Core.Const;
using Bookfiy_WepApp.Core.Models;
using Bookfiy_WepApp.Core.ViewModels;
using Bookfiy_WepApp.Data;
using Bookfiy_WepApp.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixLabors.ImageSharp;
using System.Text.Encodings.Web;
using WhatsAppCloudApi;
using WhatsAppCloudApi.Services;
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
        private readonly IWhatsAppClient _whatsAppClient;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailBodyBuilder _emailBodyBuilder;
        private readonly IEmailSender _emailSender;
        public SubscripersController(ApplicationDbContext context, IMapper mapper, IimageService imageService, IDataProtectionProvider dataProtector, IWhatsAppClient whatsAppClient, IWebHostEnvironment webHostEnvironment, IEmailBodyBuilder emailBodyBuilder, IEmailSender emailSender)
        {
            _context = context;
            _mapper = mapper;
            _imageService = imageService;
            _dataProtector = dataProtector.CreateProtector("MySecureKy");
            _whatsAppClient = whatsAppClient;
            _webHostEnvironment = webHostEnvironment;
            _emailBodyBuilder = emailBodyBuilder;
            _emailSender = emailSender;
        }

        
        public IActionResult Index()
        {
            
            return View();
        }

        #region renewal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RenewSubscribtion(string sKey)
        {
            // Unprotect key
            var subscriberId = int.Parse(_dataProtector.Unprotect(sKey));

            var subscriber = await _context.Subscripers
                .Include(s => s.Subscriptions).SingleOrDefaultAsync(s => s.Id == subscriberId);


            if (subscriber is null)
                return NotFound();

            if (subscriber.IsBlackedListed)
                return BadRequest();


            var lastSubscribtion = subscriber.Subscriptions.Last();
            var startDate = lastSubscribtion.EndDate < DateTime.Today
                ? DateTime.Today 
                : lastSubscribtion.EndDate.AddDays(1);

            Subscription newSubscribtion = new ()
            {
                CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value,
                CreatedOn = DateTime.Now,
                StartDate = startDate,
                EndDate = startDate.AddYears(1)
            };
            subscriber.Subscriptions.Add(newSubscribtion);

            _context.SaveChanges();


            //TODO :: Send Message and Email

            //Send welcome email
            var placeholders = new Dictionary<string, string>()
            {
                { "imageUrl", "https://res.cloudinary.com/bookfiy/image/upload/v1672442893/icon-positive-vote-2_jcxdww_dps4xg.svg" },
                { "header", $"Welcome {subscriber.FirstName}" },
                { "body", "Your Subscription Renewed Successfully" }
            };

            var body = _emailBodyBuilder.GetEmailBody(EmailTempletes.notificationTemp, placeholders);

            await _emailSender.SendEmailAsync(
                subscriber.Email,
                "Welcome to Bookify", body);


            if (subscriber.HasWhatsApp)
            {
                var component = new List<WhatsAppComponent>()
            {
                new WhatsAppComponent
                {
                    Type = "body",
                    Parameters = new List<object>()
                    {
                        new WhatsAppTextParameter{Text = $"{subscriber.FirstName}"}
                    }
                }
                };
                var mobileNumber = _webHostEnvironment.IsDevelopment() ? "01151889942" : subscriber.MobileNumber;
                await _whatsAppClient.SendMessage($"2{mobileNumber}", WhatsAppLanguageCode.English_US, WhatsAppTemplates.WelcomeMessage, component);
            }

            //////////////////////

            var viewModel = _mapper.Map<SubscribtionViewModel>(newSubscribtion);

            return PartialView("_SubscriptionRow", viewModel);
        }
        #endregion

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
                .Include(s => s.Subscriptions)
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

            Subscription subscription = new ()
            {
                CreatedById = subscriber.CreatedById,
                CreatedOn = subscriber.CreatedOn,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddYears(1)
            };

            subscriber.Subscriptions.Add(subscription);
            _context.Add(subscriber);
            _context.SaveChanges();

            //Send welcome email
            var placeholders = new Dictionary<string, string>()
            {
                { "imageUrl", "https://res.cloudinary.com/bookfiy/image/upload/v1672442893/icon-positive-vote-2_jcxdww_dps4xg.svg" },
                { "header", $"Welcome {model.FirstName}" },
                { "body", "thanks for joining Bookify 🤩" }
            };

            var body = _emailBodyBuilder.GetEmailBody(EmailTempletes.notificationTemp, placeholders);

            await _emailSender.SendEmailAsync(
                model.Email,
                "Welcome to Bookify", body);


            if (model.HasWhatsApp)
            {
                var component = new List<WhatsAppComponent>()
            {
                new WhatsAppComponent
                {
                    Type = "body",
                    Parameters = new List<object>()
                    {
                        new WhatsAppTextParameter{Text = $"{model.FirstName}"}
                    }
                }
                };
                 var mobileNumber = _webHostEnvironment.IsDevelopment() ? "01151889942" : model.MobileNumber;
                 await _whatsAppClient.SendMessage($"2{mobileNumber}", WhatsAppLanguageCode.English_US, WhatsAppTemplates.WelcomeMessage, component);
            }


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

            var subscriberId = int.Parse(_dataProtector.Unprotect(model.Key!));

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
