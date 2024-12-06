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

        public async Task<IActionResult> OnPostUpdateLikesDislikes([FromBody] List<LikeDislikeUpdate> updates)
        {
            foreach (var update in updates)
            {
                var post = await _postRepository.GetPost(update.PostId);
                if (post != null)
                {
                    if (update.Type == "like")
                    {
                        post.Likes += update.Delta;
                    }
                    else if (update.Type == "dislike")
                    {
                        post.Dislikes += update.Delta;
                    }
                    await _postRepository.UpdatePost(post);
                }
            }
            return new JsonResult(new { success = true });
        }

        public class LikeDislikeUpdate
        {
            public int PostId { get; set; }
            public string Type { get; set; }
            public int Delta { get; set; }
        }
    }
}