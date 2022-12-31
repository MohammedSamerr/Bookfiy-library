using AutoMapper;
using Bookfiy_WepApp.Core.Models;
using Bookfiy_WepApp.Core.ViewModels;
using Bookfiy_WepApp.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using Bookfiy_WepApp.Services;
using Bookfiy_WepApp.Core.Const;

namespace Bookfiy_WepApp.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly IEmailBodyBuilder _emailBodyBuilder;
        private readonly IMapper _mapper;
        public UsersController(UserManager<ApplicationUser> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager, IEmailSender emailSender, IWebHostEnvironment webHostEnvironment, IEmailBodyBuilder emailBodyBuilder)
        {
            _userManager = userManager;
            _mapper = mapper;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _emailBodyBuilder = emailBodyBuilder;
        }

        public async Task<IActionResult> Index()
        {
            
            var users = await _userManager.Users.ToListAsync();
            var viewModel  = _mapper.Map<IEnumerable<UsersViewModel>>(users);
            return View(viewModel);
        }

        [HttpGet]
        [Ajax_]
        public async Task<IActionResult> Create()
        {
            var viewModel = new UserFormViewModel
            {
                Roles = await _roleManager.Roles.Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Name

                }).ToListAsync()
            };
            return PartialView("_Form", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserFormViewModel model)
        {
            if(!ModelState.IsValid)
                return BadRequest();

            ApplicationUser user = new()
            {
                FullName = model.Fullname,
                UserName = model.UserName,
                Email = model.Email,
                CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRolesAsync(user, model.SelectedRoles);

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                     "/Account/ConfirmEmail",
                     pageHandler: null,
                     values: new { area = "Identity", userId = user.Id, code },
                     protocol: Request.Scheme);

                var placeholders = new Dictionary<string, string>()
                {
                    { "imageUrl", "https://res.cloudinary.com/bookfiy/image/upload/v1672442893/icon-positive-vote-1_rdexez_bvulzr.svg" },
                    { "header", $"Hey {user.FullName}, thanks for joining us!" },
                    { "body", "please confirm your email" },
                    { "url", $"{HtmlEncoder.Default.Encode(callbackUrl!)}" },
                    { "linkTitle", "Active Account!" }
                };

                var body = _emailBodyBuilder.GetEmailBody(EmailTempletes.emailTemp, placeholders);

                await _emailSender.SendEmailAsync(user.Email, "Confirm your email", body);


                var viewModel = _mapper.Map<UsersViewModel>(user);
                return PartialView("_UsersRow", viewModel);
            }

            return BadRequest(string.Join(',',result.Errors.Select(e => e.Description)));
        }
        public async Task<IActionResult> AllowUser(UserFormViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            var isAllowed = user is null || user.Id.Equals(model.Id);
            return Json(isAllowed);
        }

        public async Task<IActionResult> AllowEmail(UserFormViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            var isAllowed = user is null || user.Id.Equals(model.Id);
            return Json(isAllowed);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (id is null)
                return NotFound();

            user.IsDelete = !user.IsDelete;
            user.LastUpdateById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            user.LastUpdateOn = DateTime.Now;
            await _userManager.UpdateAsync(user);
            return Ok(user.LastUpdateOn.ToString());
        }

        [HttpGet]
        [Ajax_]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return NotFound();

            var viewModel = new ResetPasswordFormViewModel { Id = user.Id };
            return PartialView("_ResetPassword", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordFormViewModel model )
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user is null)
                return NotFound();
            var currentPass = user.PasswordHash;
            await _userManager.RemovePasswordAsync(user);
            var result = await _userManager.AddPasswordAsync(user, model.Password);
            if (result.Succeeded)
            {
                user.LastUpdateOn = DateTime.Now;
                user.LastUpdateById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
                await _userManager.UpdateAsync(user);
                var viewModel = _mapper.Map<UsersViewModel>(user);
                return PartialView("_UsersRow", viewModel);
            }

            user.PasswordHash = currentPass;
            await _userManager.UpdateAsync(user);

            return BadRequest(string.Join(',', result.Errors.Select(e => e.Description)));
        }

        [HttpGet]
        [Ajax_]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
                return NotFound();

            var viewModel = _mapper.Map<UserFormViewModel>(user);

            viewModel.SelectedRoles = await _userManager.GetRolesAsync(user);
            viewModel.Roles = await _roleManager.Roles
                                .Select(r => new SelectListItem
                                {
                                    Text = r.Name,
                                    Value = r.Name
                                })
                                .ToListAsync();

            return PartialView("_Form", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await _userManager.FindByIdAsync(model.Id);

            if (user is null)
                return NotFound();

            user = _mapper.Map(model, user);
            user.LastUpdateById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            user.LastUpdateOn = DateTime.Now;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                var currentRoles = await _userManager.GetRolesAsync(user);

                var rolesUpdated = !currentRoles.SequenceEqual(model.SelectedRoles);

                if (rolesUpdated)
                {
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    await _userManager.AddToRolesAsync(user, model.SelectedRoles);
                }

                var viewModel = _mapper.Map<UsersViewModel>(user);
                return PartialView("_UsersRow", viewModel);
            }

            return BadRequest(string.Join(',', result.Errors.Select(e => e.Description)));
        }
    }
}
