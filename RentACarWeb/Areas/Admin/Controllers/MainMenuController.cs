using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentACarData;

namespace RentACarWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrators, ProductManagers")]
    public class MainMenuController : Controller
    {

        private readonly AppDbContext context;

        public MainMenuController(
            AppDbContext context
            )
        {
            this.context = context;
        }
        public async Task<IActionResult> Index()
        {
            var model = await context.MainMenus.ToListAsync();
            return View(model);
        }
        

        [HttpGet]
        public IActionResult Create()
        {
            return View(new MainMenu { Enabled = true });
        }

        [HttpPost]
        public async Task<IActionResult> Create(MainMenu model)
        {
            model.DateCreated = DateTime.UtcNow;
            model.Enabled = true;

            context.MainMenus.Add(model);
            try
            {
                await context.SaveChangesAsync();
                TempData["success"] = "Menü ekleme işlemi başarıyla tamamlanmıştır!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                TempData["error"] = "Aynı isimli başka bir menü bulunduğundan ekleme işlemi yapılamıyor!";
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["error"] = "Beklenmedik hata, daha sonra tekrar deneyiniz!";
                return View(model);
            }

        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await context.MainMenus.FindAsync(id);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(MainMenu model)
        {

            context.MainMenus.Update(model);
            try
            {
                await context.SaveChangesAsync();
                TempData["success"] = "Menü güncelleme işlemi başarıyla tamamlanmıştır!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["error"] = "Aynı isimli başka bir menü bulunduğundan güncelleme işlemi yapılamıyor!";
                return View(model);
            }

        }


        [HttpGet]
        public async Task<IActionResult> Remove(Guid id)
        {
            var model = await context.MainMenus.FindAsync(id);
            context.MainMenus.Remove(model);
            try
            {
                await context.SaveChangesAsync();
                TempData["success"] = "Menü silme işlemi başarıyla tamamlanmıştır!";
            }
            catch (Exception)
            {
                TempData["error"] = $"{model.Name} isimli menü bir ya da daha fazla kayıt ile ilişkili olduğu için silme işlemi tamamlanamıyor!";
            }
            return RedirectToAction(nameof(Index));

        }
    }
}
