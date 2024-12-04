using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using My_SocNet_Win.Classes.Posts;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace My_SocNet_Win.Pages
{
    [Authorize]
    public class CreatePostModel : PageModel
    {
        private readonly IPostRepository<BasePost> _postRepository;

        [BindProperty]
        public BasePost Post { get; set; }

        [BindProperty]
        public List<IFormFile> Images { get; set; }

        public CreatePostModel(IPostRepository<BasePost> postRepository)
        {
            _postRepository = postRepository;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                // Handle the case where the user ID claim is not found
                ModelState.AddModelError(string.Empty, "User ID claim not found.");
                return Page();
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                // Handle the case where the user ID claim value is not a valid integer
                ModelState.AddModelError(string.Empty, "Invalid user ID claim value.");
                return Page();
            }

            Post.CreatorID = userId;
            Post.DateOfCreation = DateTime.UtcNow;
            Post.Dislikes = 0;
            Post.Likes = 0;

            // Initialize the Images property if it is null
            if (Post.Images == null)
            {
                Post.Images = new List<byte[]>();
            }

            if (Images != null && Images.Count > 0)
            {
                foreach (var formFile in Images)
                {
                    if (formFile.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await formFile.CopyToAsync(ms);
                            var fileBytes = ms.ToArray();
                            Post.Images.Add(fileBytes);
                        }
                    }
                }
            }

            await _postRepository.CreatePost(Post);

            return RedirectToPage("/Index");
        }
    }
}
