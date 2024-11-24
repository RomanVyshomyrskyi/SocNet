using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using My_SocNet_Win.Classes;
using My_SocNet_Win.Classes.Posts;
using My_SocNet_Win.Classes.User;

namespace My_SocNet_Win.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly SiteSettings _siteSettings;
        private readonly IPostRepository<BasePost> _postRepository;
        public ConcreteUser? CurrentUser { get; set; }
        public List<BasePost> RecentPosts { get; set; } = new List<BasePost>();

        public IndexModel(ILogger<IndexModel> logger, IOptions<SiteSettings> siteSettings, IPostRepository<BasePost> postRepository)
        {
            _logger = logger;
            _siteSettings = siteSettings.Value;
            _postRepository = postRepository;
        }

        public async Task OnGet()
        {
            #region Obligatory for all pages
            var roleClaims = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            CurrentUser = new ConcreteUser
            {
                UserName = User.Identity?.Name ?? "Unknown",
                Password = "",
                Roles = roleClaims
            };

            ViewData["Title"] = _siteSettings.Title;
            ViewData["Name"] = _siteSettings.Name;
            ViewData["Footer"] = _siteSettings.Footer;
            ViewData["Version"] = _siteSettings.Version;
            #endregion
            try{
                RecentPosts = await _postRepository.GetPosts(15);
            }catch(Exception ex){
                _logger.LogError(ex, "Error getting recent posts");
            }
        }
    }
}