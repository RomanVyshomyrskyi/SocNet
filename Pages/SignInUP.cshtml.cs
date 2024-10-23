using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using My_SocNet_Win.Classes;
using My_SocNet_Win.Classes.Converters;
using My_SocNet_Win.Classes.DB;
using My_SocNet_Win.Classes.User;
using System.Security.Claims;

namespace My_SocNet_Win.Pages
{
    public class SignInUPModel : PageModel
    {
        private readonly ILogger<SignInUPModel> _logger;
        private readonly SiteSettings _siteSettings;
        private readonly IUserRepository<BaseUsers> _userRepository;

        public SignInUPModel(ILogger<SignInUPModel> logger, IOptions<SiteSettings> siteSettings, IUserRepository<BaseUsers> userRepository)
        {
            _logger = logger;
            _siteSettings = siteSettings.Value;
            _userRepository = userRepository;
        }
        
        public ConcreteUser? CurrentUser { get; set; }

        public void OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                CurrentUser = new ConcreteUser
                {
                    UserName = User.Identity.Name,
                    Password = string.Empty // Set an appropriate default or fetched password here
                };
                Redirect("/Index");
            }
            ViewData["SiteName"] = _siteSettings.Name;
            ViewData["Version"] = _siteSettings.Version;
        }

        public async Task<IActionResult> OnPostLogIn(string logemail, string logpass)
        {
            var dbUser = await _userRepository.GetUserByEmailAndPasswordAsync(logemail, logpass);
            if (dbUser != null)
            {
                CurrentUser = UserConverter.ConvertToBaseUser(dbUser);
                #region Claims (save Data in Cookie)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, CurrentUser.UserName),
                    new Claim(ClaimTypes.Email, CurrentUser.Email),
                    
                };
                foreach (var role in CurrentUser.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
                foreach (var friend in CurrentUser.Friends)
                {
                    claims.Add(new Claim("Friend", friend.ToString()));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true
                };
                #endregion
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                return Redirect("/Index");
            }
            return Page();
        }

        public IActionResult OnPostSignUp(string logname, string logemail, string logpass)
        {
            // Implement user registration logic here
            return Redirect("/Index");
        }
    }
}