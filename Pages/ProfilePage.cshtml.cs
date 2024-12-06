using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using My_SocNet_Win.Classes;
using My_SocNet_Win.Classes.Converters;
using My_SocNet_Win.Classes.User;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace My_SocNet_Win.Pages
{
    [Authorize]
    public class ProfilePageModel : PageModel
    {
        private readonly SiteSettings _siteSettings;
        private readonly IUserRepository<BaseUsers> _userRepository;
        private readonly IDistributedCache _cache;

        public ConcreteUser? CurrentUser { get; set; }
        public ProfilePageModel(IOptions<SiteSettings> siteSettings, IUserRepository<BaseUsers> userRepository, IDistributedCache cache)
        {
            _siteSettings = siteSettings.Value;
            _userRepository = userRepository;
            _cache = cache;
        }

        public async Task OnGet()
        {
            var userId = User.FindFirstValue("UserId");
            var dbUser = await _userRepository.GetUserByIdAsync(Int32.Parse(userId));
            CurrentUser = UserConverter.ConvertToBaseUser(dbUser);
            ViewData["SiteName"] = _siteSettings.Name;
            ViewData["Version"] = _siteSettings.Version;
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync();
            return RedirectToPage("/Index");
        }
        public async Task<IActionResult> OnPostCancelAsync()
        {
            return await Task.FromResult(RedirectToPage());
        }
    }
}
