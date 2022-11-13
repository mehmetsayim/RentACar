using Microsoft.AspNetCore.Mvc;
using RentACarData;

namespace RentACarWeb.Components
{
    public class MainMenuBarViewComponent : ViewComponent
    {
        private readonly AppDbContext context;

        public MainMenuBarViewComponent(
            AppDbContext context
            )
        {
            this.context = context;
        }

        public IViewComponentResult Invoke()
        {
            var model = context.MainMenus.Where(p => p.Enabled).ToList();
            return View(model);
        }
    }
}
