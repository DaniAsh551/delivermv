using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Deliver.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Deliver.Data.Common;
using Deliver.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace Deliver.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly DeliverDbContext _db;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<RegisterModel> logger,
            DeliverDbContext db,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _db = db;
            _emailSender = emailSender;
            Islands = db.Islands;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public IQueryable<Island> Islands { get; set; }

        public class InputModel
        {
            [Required]
            [Phone]
            [Display(Name = "Phone")]
            [RegularExpression("[0-9]*", ErrorMessage = "Phone number is invalid.")]
            [StringLength(7, MinimumLength = 7)]
            public string Phone { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name ="Shop Name")]
            public string ShopName { get; set; }

            [Required]
            [Display(Name = "Payment Methods")]
            public PaymentMethod[] PaymentMethods { get; set; }

            [Required]
            [Display(Name = "Deliverable To")]
            public int[] SupportedIslands { get; set; }

            [Display(Name = "BML Account No")]
            [RegularExpression("[0-9]*", ErrorMessage = "BML Account No is invalid.")]
            [StringLength(13, MinimumLength = 13, ErrorMessage = "MIB Account No is invalid.")]
            public string BmlAccountNo { get; set; }
            [Display(Name = "MIB Account No")]
            [RegularExpression("[0-9]*", ErrorMessage = "MIB Account No is invalid.")]
            [StringLength(13, MinimumLength = 13, ErrorMessage = "MIB Account No is invalid.")]
            public string MibAccountNo { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (Input.PaymentMethods.Contains(PaymentMethod.BankTransferBml) && (Input.BmlAccountNo.IsEmpty() || Input.BmlAccountNo.Length != 13))
                ModelState.AddModelError("BmlAccountNo", "BML Account No is invalid.");
            if (Input.PaymentMethods.Contains(PaymentMethod.BankTransferMib) && (Input.MibAccountNo.IsEmpty() || Input.MibAccountNo.Length != 13))
                ModelState.AddModelError("MibAccountNo", "Mib Account No is invalid.");

            if (await _db.Users.AnyAsync(x => x.IsActive && x.PhoneNumber == Input.Phone))
                ModelState.AddModelError("Phone", $"There already is an account registered with {Input.Phone}");

            if (ModelState.IsValid)
            {
                Input.PaymentMethods = Input.PaymentMethods.OrderBy(x => (int)x).ToArray();
                var user = new User 
                {
                    Name = Input.ShopName,
                    PaymentMethods = Input.PaymentMethods
                    .Select(x => new UserPaymentMethod { PaymentMethod = x }).ToArray(),
                    UserName = Input.ShopName.Replace(' ', '_') + Input.Phone,
                    BmlAccount = Input.BmlAccountNo,
                    MibAccount = Input.MibAccountNo,
                    PhoneNumber = Input.Phone,
                    Islands = Input.SupportedIslands.Select(x => new IslandShop { IslandId = x })
                        .ToArray(),
                    IsActive = true,
                    UserType = UserType.Shop
                };

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);

                    /* if (_userManager.Options.SignIn.RequireConfirmedAccount)
                     {
                         return RedirectToPage("RegisterConfirmation", new { email = Input.Email });
                     }
                     else
                     {
                         await _signInManager.SignInAsync(user, isPersistent: false);
                         return LocalRedirect(returnUrl);
                     }*/
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
