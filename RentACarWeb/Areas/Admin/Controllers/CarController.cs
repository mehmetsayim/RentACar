using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RentACarData;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Data;
using System.Globalization;

namespace RentACarWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrators, ProductManagers")]
    public class CarController : Controller
    {
        private readonly AppDbContext context;
        private readonly IConfiguration configuration;

        public CarController(
            AppDbContext context,
            IConfiguration configuration
            )
        {
            this.context = context;
            this.configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var model = await context.Cars.ToListAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await Dropdowns();

            return View(new Car { Enabled = true });
        }

        [HttpPost]
        public async Task<IActionResult> Create(Car model)
        {
            model.DateCreated = DateTime.UtcNow;
            model.Enabled = true;
            model.Price = decimal.Parse(model.PriceText);
            model.DiscountedPrice = model.DiscountedPriceText != null ? decimal.Parse(model.DiscountedPriceText) : null;


            model.Categories = await context
                .Categories
                .Where(p => model.CategoryIds.Any(q => q == p.Id)).ToListAsync();

            if (model.ImageFiles is not null)
            {
                foreach (var file in model.ImageFiles)
                {

                    var image = await Image.LoadAsync(file.OpenReadStream());
                    image.Mutate(p =>
                    {
                        p.Resize(
                            configuration.GetValue<int>("DefaultImageSize:Width"),
                            configuration.GetValue<int>("DefaultImageSize:Height"));
                    });

                    using var ms = new MemoryStream();
                    await image.SaveAsJpegAsync(ms);

                    model.CarImages.Add(new CarImage
                    {
                        DateCreated = DateTime.UtcNow,
                        Enabled = true,
                        Image = ms.ToArray()
                    });
                }
            }

            context.Cars.Add(model);
            try
            {
                await context.SaveChangesAsync();
                TempData["success"] = "Araç ekleme işlemi başarıyla tamamlanmıştır!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await Dropdowns();
                TempData["error"] = "Aynı isimli başka bir Araç bulunduğundan ekleme işlemi yapılamıyor!";
                return View(model);
            }

        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            await Dropdowns();
            var model = await context.Cars.FindAsync(id);
            model.PriceText = model.Price.ToString("f2", CultureInfo.CreateSpecificCulture("tr-TR"));
            model.DiscountedPriceText = model.DiscountedPrice.Value.ToString("f2", CultureInfo.CreateSpecificCulture("tr-TR"));
            model.CategoryIds = model.Categories.Select(c => c.Id).ToList();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Car model)
        {
            model.Price = decimal.Parse(model.PriceText);
            model.DiscountedPrice = model.DiscountedPriceText != null ? decimal.Parse(model.DiscountedPriceText) : null;

            //model.Categories = await context
            //    .Categories
            //    .Where(p => model.CategoryIds.Any(q => q == p.Id)).ToListAsync();

            if (model.ImageFiles is not null)
            {
                foreach (var file in model.ImageFiles)
                {

                    var image = await Image.LoadAsync(file.OpenReadStream());
                    image.Mutate(p =>
                    {
                        p.Resize(
                            configuration.GetValue<int>("DefaultImageSize:Width"),
                            configuration.GetValue<int>("DefaultImageSize:Height"));
                    });

                    using var ms = new MemoryStream();
                    await image.SaveAsJpegAsync(ms);

                    model.CarImages.Add(new CarImage
                    {
                        DateCreated = DateTime.UtcNow,
                        Enabled = true,
                        Image = ms.ToArray()
                    });
                }
            }
            foreach (var item in model.ImagesToDeleted)
            {
                var carImage = await context.CarImages.FindAsync(item);
                context.Entry(carImage).State = EntityState.Deleted;
            }



            context.Cars.Update(model);

            try
            {
                await context.SaveChangesAsync();
                TempData["success"] = "Araç güncelleme işlemi başarıyla tamamlanmıştır!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await Dropdowns();
                TempData["error"] = "Aynı isimli başka bir Araç bulunduğundan güncelleme işlemi yapılamıyor!";
                return View(model);
            }

        }


        [HttpGet]
        public async Task<IActionResult> Remove(Guid id)
        {
            var model = await context.Cars.FindAsync(id);
            context.Cars.Remove(model);
            try
            {
                await context.SaveChangesAsync();
                TempData["success"] = "Araç silme işlemi başarıyla tamamlanmıştır!";
            }
            catch (Exception)
            {
                TempData["error"] = $"{model.Name} isimli Araç bir ya da daha fazla kayıt ile ilişkili olduğu için silme işlemi tamamlanamıyor!";
            }
            return RedirectToAction(nameof(Index));

        }
         private async Task Dropdowns()
        {
            ViewBag.Categories = new SelectList(await context.Categories.Select(p => new { p.Id, p.Name, MainMenuName = p.MainMenu!.Name }).OrderBy(p => p.Name).ToListAsync(), "Id", "Name", null, "MainMenuName");
        }
    }
}
