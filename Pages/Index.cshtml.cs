using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using My_SocNet_Win.Classes;
using My_SocNet_Win.Classes.DB;
using My_SocNet_Win.Classes.User;

namespace My_SocNet_Win.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly SiteSettings _siteSettings;

    public IndexModel(ILogger<IndexModel> logger, IOptions<SiteSettings> siteSettings)
    {
        _logger = logger;
        _siteSettings = siteSettings.Value;
    }

    public void OnGet()
    {
        ViewData["Title"] = _siteSettings.Title;
        ViewData["Name"] = _siteSettings.Name;
        ViewData["Footer"] = _siteSettings.Footer;
        ViewData["Version"] = _siteSettings.Version;
    }
}