using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using My_SocNet_Win.Classes;
using My_SocNet_Win.Classes.User;

namespace My_SocNet_Win.Pages;

[Authorize]
public class PrivacyModel : PageModel
{
    private readonly ILogger<PrivacyModel> _logger;
    private readonly SiteSettings _siteSettings;

    public ConcreteUser? CurrentUser { get; set; }

    public PrivacyModel(ILogger<PrivacyModel> logger, IOptions<SiteSettings> siteSettings)
    {
        _logger = logger;
        _siteSettings = siteSettings.Value;
    }


    public void OnGet()
    {
        var roleClaims = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        CurrentUser = new ConcreteUser
        {
            UserName = User.Identity.Name,
            Password = "",
            Roles = roleClaims

        };
        ViewData["Title"] = _siteSettings.Title;
        ViewData["Name"] = _siteSettings.Name;
        ViewData["Footer"] = _siteSettings.Footer;
        ViewData["Version"] = _siteSettings.Version;
    }
}

