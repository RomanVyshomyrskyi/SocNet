using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using My_SocNet_Win.Classes;
using My_SocNet_Win.Classes.Converters;
using My_SocNet_Win.Classes.User;

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

        public void OnGet()
        {
            var userId = User.FindFirstValue("UserId");
            var dbUser = _userRepository.GetUserByIdAsync(userId).Result;
            CurrentUser = UserConverter.ConvertToBaseUser(dbUser);
            ViewData["SiteName"] = _siteSettings.Name;
            ViewData["Version"] = _siteSettings.Version;   
        }
    }
}
