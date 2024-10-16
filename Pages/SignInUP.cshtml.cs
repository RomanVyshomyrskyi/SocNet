using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using My_SocNet_Win.Classes;

namespace My_SocNet_Win.Pages
{
    public class SignInUPModel : PageModel
    {
        private readonly ILogger<SignInUPModel> _logger;
        private readonly SiteSettings _siteSettings;
        public SignInUPModel(ILogger<SignInUPModel> logger, IOptions<SiteSettings> siteSettings)
        {
            _logger = logger;
            _siteSettings = siteSettings.Value;
        }
        public void OnGet()
        {
            ViewData["SiteName"] = _siteSettings.Name;
            ViewData["Version"] = _siteSettings.Version;
        }
        public IActionResult OnPostLogIn(string logemail, string logpass)
        {
            return RedirectToPage("/Index");
        }
        public IActionResult OnPostSignUp(string logname, string logemail, string logpass)
        {
            return Redirect("/Index");
        }
    }
}
