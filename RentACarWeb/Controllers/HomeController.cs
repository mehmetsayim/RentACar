using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NETCore.MailKit.Core;
using RentACarData;
using RentACarWeb.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace RentACarWeb.Controllers
{
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly IEmailService emailService;
        private readonly IConfiguration configuration;
        private readonly AppDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IShoppingCartService shoppingCartService;

        public HomeController(
            ILogger<HomeController> logger,
            IEmailService emailService,
            IConfiguration configuration,
            AppDbContext context,
            UserManager<ApplicationUser> userManager,
            IShoppingCartService shoppingCartService
            )
        {
            _logger = logger;
            this.emailService = emailService;
            this.configuration = configuration;
            this.context = context;
            this.userManager = userManager;
            this.shoppingCartService = shoppingCartService;
        }

        [ResponseCache(Duration = 86400)]
        public async Task<IActionResult> Index()
        {
            ViewBag.Featured = await context.Cars.OrderBy(p => p.DiscountedPrice).Take(4).ToListAsync();
            ViewBag.BestSeller = await context.Cars.OrderByDescending(p => p.OrderItems.Count).Take(4).ToListAsync();
            if (User.Identity.IsAuthenticated)
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                var categories = user.Orders.SelectMany(p => p.OrderItems).SelectMany(p => p.Car.Categories).Select(p => p.Id).Distinct();
                ViewBag.ShowCase = await context.Cars.OrderBy(p => p.Id).Take(12).ToListAsync();
            }
            else
            {
                ViewBag.ShowCase = await context.Cars.OrderBy(p => p.Id).Take(12).ToListAsync();
            }
            return View();
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult ContactUs()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ContactUs(ContactUsViewModel model)
        {
            await emailService.SendAsync(
                configuration.GetValue<string>("EMailSettings:SenderEmail"),
                $"Ziyaretçi Mesajı ({model.Name})",
                $"Gönderen: \t{model.Name}\nTel: \t\t{model.PhoneNumber ?? "Tel. Belirtilmemiş"}\nE-Posta: \t{model.EMail}\nMesaj:\n------\n{model.Message}"
                );
            TempData["messageSent"] = true;
            return View(new ContactUsViewModel());
        }

        [HttpGet]
        public async Task<IActionResult> Category(Guid id, int? page)
        {
            var model = await context.Categories.FindAsync(id);
            ViewBag.Page = page;
            return View(model);
        }  

        [HttpGet]

        public IActionResult Car()
        {
            List<Car> model = context.Cars.ToList();
            return View(model);
        }

        public async Task<IActionResult> Detay(Guid id, int? page)
        {
            var model = await context.Cars.FindAsync(id);
            ViewBag.Page = page;
            return View(model);
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> AddToCart(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var product = await context.Cars.FindAsync(id);
            var shoppingCartItem = new ShoppingCartItem
            {
                ApplicationUserId = userId,
                Quantity = 1,
                CarId = id,
                DateCreated = DateTime.Now,
                Enabled = true,
            };
            await context.ShoppingCartItems.AddAsync(shoppingCartItem);
            await context.SaveChangesAsync();
            TempData["addedToCart"] = true;
            return Redirect(Request.Headers["Referer"].ToString());
        }


        [HttpGet, Authorize]
        public async Task<IActionResult> RemoveFromCart(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var item = await context.
                ShoppingCartItems.
                OrderBy(p => p.DateCreated).
                LastOrDefaultAsync(p => p.ApplicationUserId == userId && p.CarId == id);

            if (item is null)
            {
                return BadRequest();
            }
            context.ShoppingCartItems.Remove(item);
            await context.SaveChangesAsync();
            return RedirectToAction("ShoppingCart", "Account");
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> RemoveAllFromCart(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var items = await context.ShoppingCartItems.Where(p => p.ApplicationUserId == userId && p.CarId == id).ToListAsync();
            if (items is null)
            {
                return BadRequest();
            }
            context.ShoppingCartItems.RemoveRange(items);
            await context.SaveChangesAsync();
            return RedirectToAction("ShoppingCart", "Account");
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> ClearCart()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var items = await context.ShoppingCartItems.Where(p => p.ApplicationUserId == userId).ToListAsync();
            if (items is null)
            {
                return BadRequest();
            }
            context.ShoppingCartItems.RemoveRange(items);
            await context.SaveChangesAsync();
            return RedirectToAction("ShoppingCart", "Account");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}