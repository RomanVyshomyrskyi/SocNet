using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using My_SocNet_Win.Classes;

namespace My_SocNet_Win.Pages;

public class IndexModel : PageModel
{
    IServiceProvider _serviceProvider;
    private readonly ILogger<IndexModel> _logger;
    private readonly SiteSettings _siteSettings;
    public IndexModel(ILogger<IndexModel> logger, IOptions<SiteSettings> siteSettings, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _siteSettings = siteSettings.Value;
        _serviceProvider = serviceProvider;
    }

    public void OnGet()
    {
        DatabaseConfigurator.EnsureAdminUserExists(_serviceProvider);
        ViewData["Title"] = _siteSettings.Title;
        ViewData["Name"] = _siteSettings.Name;
        ViewData["Footer"] = _siteSettings.Footer;
        ViewData["Version"] = _siteSettings.Version;
    }
}
