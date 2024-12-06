using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using My_SocNet_Win.Classes;
using My_SocNet_Win.Classes.Posts;
using My_SocNet_Win.Classes.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace My_SocNet_Win.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly SiteSettings _siteSettings;
        private readonly IPostRepository<BasePost> _postRepository;
        private readonly IUserRepository<BaseUsers> _userRepository;
        public ConcreteUser? CurrentUser { get; set; }
        public List<BasePost> RecentPosts { get; set; } = new List<BasePost>();
        public Dictionary<int, string> UserNames { get; set; } = new Dictionary<int, string>();

        public IndexModel(ILogger<IndexModel> logger, IOptions<SiteSettings> siteSettings, IPostRepository<BasePost> postRepository, IUserRepository<BaseUsers> userRepository)
        {
            _logger = logger;
            _siteSettings = siteSettings.Value;
            _postRepository = postRepository;
            _userRepository = userRepository;
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

            try
            {
                RecentPosts = await _postRepository.GetPosts(15);

                foreach (var post in RecentPosts)
                {
                    if (post != null && !UserNames.ContainsKey(post.CreatorID))
                    {
                        var userName = await _userRepository.GetUserNameByIdAsync(post.CreatorID);
                        UserNames[post.CreatorID] = userName ?? "Unknown";
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent posts");
            }
        }

        public async Task<IActionResult> OnPostUpdateLikesDislikes(int PostId, string Type, int Delta)
        {
            var post = await _postRepository.GetPost(PostId);
            if (post != null)
            {
                if (Type == "like")
                {
                    post.Likes += Delta;
                    if (Delta > 0 && post.Dislikes > 0)
                    {
                        post.Dislikes -= 1;
                    }
                }
                else if (Type == "dislike")
                {
                    post.Dislikes += Delta;
                    if (Delta > 0 && post.Likes > 0)
                    {
                        post.Likes -= 1;
                    }
                }
                await _postRepository.UpdatePost(post);
            }
            return new JsonResult(new { success = true });
        }
    }
}