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
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace My_SocNet_Win.Pages
{
    public class SignInUPModel : PageModel
    {
        private readonly ILogger<SignInUPModel> _logger;
        private readonly SiteSettings _siteSettings;
        private readonly IUserRepository<BaseUsers> _userRepository;
        private readonly IDistributedCache _cache;

        public SignInUPModel(ILogger<SignInUPModel> logger, IOptions<SiteSettings> siteSettings,
                                 IUserRepository<BaseUsers> userRepository, IDistributedCache cache) 
        {
            _logger = logger;
            _siteSettings = siteSettings.Value;
            _userRepository = userRepository;
            _cache = cache;
        }
        
        public ConcreteUser? CurrentUser { get; set; }

        public void OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                var claimsCacheKey = $"claims_{User.Identity.Name}";
                var cachedClaims = _cache.GetString(claimsCacheKey);

                if (cachedClaims != null)
                {
                    var claims = JsonSerializer.Deserialize<List<Claim>>(cachedClaims);
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    HttpContext.User = new ClaimsPrincipal(claimsIdentity);
                    Redirect("/Index");
                }
                else
                {
                    CurrentUser = new ConcreteUser
                    {
                        UserName = User.Identity.Name,
                        Password = string.Empty // Set an appropriate default or fetched password here
                    };
                    Redirect("/Index");
                }
            }
            ViewData["SiteName"] = _siteSettings.Name;
            ViewData["Version"] = _siteSettings.Version;
        }

        public async Task<IActionResult> OnPostLogIn(string logemail, string logpass)
        {
            var cacheKey = $"user_{logemail}";
            var cachedUser = await _cache.GetStringAsync(cacheKey);

            if (cachedUser != null)
            {
                CurrentUser = JsonSerializer.Deserialize<ConcreteUser>(cachedUser);
            }
            else
            {
                var dbUser = await _userRepository.GetUserByEmailAndPasswordAsync(logemail, logpass);
                if (dbUser != null)
                {
                    CurrentUser = UserConverter.ConvertToBaseUser(dbUser);
                    var serializedUser = JsonSerializer.Serialize(CurrentUser);
                    await _cache.SetStringAsync(cacheKey, serializedUser, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(3) // Set user cache expiration time (3h in this case)
                    });
                }
            }

            if (CurrentUser != null)
            {
                #region Claims (save Data in Cookie)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, CurrentUser.UserName),
                    new Claim(ClaimTypes.Email, CurrentUser.Email)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                // Cache claims data
                var claimsCacheKey = $"claims_{logemail}";
                var serializedClaims = JsonSerializer.Serialize(claims);
                await _cache.SetStringAsync(claimsCacheKey, serializedClaims, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(3) // Set claims cache expiration time (3h in this case)
                });
                #endregion

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