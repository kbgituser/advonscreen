using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Dal.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace AdvScreen.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModelPhone : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly IEmailSender _emailSender;

        public LoginModelPhone(SignInManager<ApplicationUser> signInManager, 
            ILogger<LoginModel> logger,
            UserManager<ApplicationUser> userManager
            , IEmailSender emailSender
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [Phone]
            [Display(Name = "Телефон")]
            [DataType(DataType.PhoneNumber)]
            [RegularExpression(@"^\(?([0-9]{1})\)?[-. ]?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Номер телефона введен некорректно!")]

            public string Phone { get; set; }

            [Display(Name = "Запомнить меня?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                
                ApplicationUser user = await _userManager.FindByNameAsync(Input.Phone);

                if (user != null)
                {
                    // проверяем, подтвержден ли email
                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        return RedirectToPage("./LoginEmail", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                    }
                    else
                    {
                        var authProps = new AuthenticationProperties
                        {
                            IsPersistent = Input.RememberMe,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(2)
                        };
                        await _signInManager.SignInAsync(user, authProps);
                        //await _signInManager.SignInAsync(user, true);
                        return LocalRedirect(returnUrl);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Данный номер незарегистрирован в системе!");
                    return Page();
                }
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError(string.Empty, "Ошибка!");
            return Page();
        }
    }
}
