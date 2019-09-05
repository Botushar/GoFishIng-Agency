namespace GoFishIng.Web.Areas.Identity.Pages.Account
{
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Security.Claims;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;
    using GoFishIng.Data;
    using GoFishIng.Data.Models;
    using GoFishIng.Services;
    using GoFishIng.Web.Areas.Store.ViewModels;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    [AllowAnonymous]
#pragma warning disable SA1649 // File name should match first type name
    public class RegisterModel : PageModel
#pragma warning restore SA1649 // File name should match first type name
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<RegisterModel> logger;
        private readonly IEmailSender emailSender;
        private readonly ICartsService cartsServices;
        private readonly ITripsService tripsServices;
        private readonly ApplicationDbContext db;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ICartsService cartsServices,
            ITripsService tripsServices,
            ApplicationDbContext db)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.emailSender = emailSender;
            this.cartsServices = cartsServices;
            this.db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public void OnGet(string returnUrl = null)
        {
            this.ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            //CreateCartViewModel model = new CreateCartViewModel();
            returnUrl = returnUrl ?? this.Url.Content("~/");
            if (this.ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = this.Input.Email, Email = this.Input.Email};
                var result = await this.userManager.CreateAsync(user, this.Input.Password);
                //var experimental = user.Id;
                this.cartsServices.CreateCart(user.Id);
                //var searchedId = this.db.Carts.Include(u => u.User).FirstOrDefault(c => c.UserId == user.Id);
                //user.CartId = searchedId.Id;
                //this.db.Users.Update(user);
                this.db.SaveChanges();

                //ClaimsPrincipal principal = this.HttpContext.User as ClaimsPrincipal;
                //var id = this.userManager.GetUserId();


                if (result.Succeeded)
                {
                    this.logger.LogInformation("User created a new account with password.");

                    var code = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = this.Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = code },
                        protocol: this.Request.Scheme);

                    await this.emailSender.SendEmailAsync(
                        this.Input.Email,
                        "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    await this.signInManager.SignInAsync(user, isPersistent: false);

                    return this.LocalRedirect(returnUrl);
                }

                foreach (var error in result.Errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }

                
            }

            // If we got this far, something failed, redisplay form
            return this.Page();
        }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public string CartId { get; set; }
        }

        //private ApplicationUser GetCurrentUserAsync()
        //{
        //    userManager.GetUserAsync(HttpContext.User);

        //    var user = GetCurrentUserAsync();

        //    var userId = user?.Id;

        //    return userId;
        //}


    }
}
