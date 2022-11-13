using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RentACarData;

namespace RentACarWeb.Components
{
    public class UserBarViewComponent : ViewComponent
    {
        private readonly UserManager<ApplicationUser> userManager;

        public UserBarViewComponent(
            UserManager<ApplicationUser> userManager
            )
        {
            this.userManager = userManager;
        }

        [Authorize]
        public IViewComponentResult Invoke()
        {
            var userName = User.Identity.Name;
            ApplicationUser model = null;
            if (userName is not null)
                model = userManager.FindByNameAsync(userName).Result;
            return View(model);
        }
    }
}
