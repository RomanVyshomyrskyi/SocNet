using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using My_SocNet_Win.Classes;
using My_SocNet_Win.Classes.DB;
using My_SocNet_Win.Classes.User;

namespace My_SocNet_Win.Pages
{
    public class SignInUPModel : PageModel
    {
        private readonly ILogger<SignInUPModel> _logger;
        private readonly SiteSettings _siteSettings;
        private IUserRepository<BaseUsers> _userRepository;
        public SignInUPModel(ILogger<SignInUPModel> logger, IOptions<SiteSettings> siteSettings, IUserRepository<BaseUsers> userRepository)
        {
            _logger = logger;
            _siteSettings = siteSettings.Value;
            _userRepository = userRepository;
        }
        
        public BaseUsers? CurrentUser { get; set; }
        public void OnGet()
        {
            ViewData["SiteName"] = _siteSettings.Name;
            ViewData["Version"] = _siteSettings.Version;
        }
        public async Task<IActionResult> OnPostLogIn(string logemail, string logpass)
        {
            CurrentUser = await _userRepository.GetUserByEmailAndPasswordAsync(logemail, logpass);
            if (CurrentUser != null)
            {
                TempData.Put("CurrentUser", CurrentUser);
                return RedirectToPage("/Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }
        }
        public IActionResult OnPostSignUp(string logname, string logemail, string logpass)
        {
            return Redirect("/Index");
        }
    }
}
