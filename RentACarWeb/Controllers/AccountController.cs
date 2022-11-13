using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NETCore.MailKit.Core;
using RentACarData;
using RentACarWeb.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace RentACarWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly IEmailService emailService;
        private readonly IWebHostEnvironment env;
        private readonly IConfiguration configuration;
        private readonly AppDbContext context;
        private readonly IShoppingCartService shoppingCartService;
        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            IWebHostEnvironment env,
            IConfiguration configuration,
            AppDbContext context,
            IShoppingCartService shoppingCartService
            )
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.emailService = emailService;
            this.env = env;
            this.configuration = configuration;
            this.context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel { RememberMe = true });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, true);
            if (result.Succeeded)
            {
                return Redirect(model.ReturnUrl ?? "/");
            }
            else
            {
                ModelState.AddModelError("", "Geçersiz kullanıcı girişi");
                return View(model);
            }
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }



        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel { });
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Name = model.Name,
                Gender = model.Gender,
                BirthDate = model.BirthDate,
                EmailConfirmed = false
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Members");
                await userManager.AddClaimAsync(user, new System.Security.Claims.Claim("FullName", user.Name));

                var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                var link = Url.Action(nameof(ConfirmEMail), "Account", new { id = user.Id, token = token }, Request.Scheme);
                var body = string.Format(
                    System.IO.File.ReadAllText(Path.Combine(env.WebRootPath, "templates", "EMailConfirmation.html")),
                    model.Name,
                    link);

                await emailService.SendAsync(
                    mailTo: model.UserName,
                    subject: "RentACar Eposta Doğrulama Mesajı",
                    message: body,
                    isHtml: true
                    );
                return RedirectToAction("Index", "Home");
            }
            else
            {
                result.Errors
                    .ToList()
                    .ForEach(p => ModelState.AddModelError("", p.Description));
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEMail(Guid id, string token)
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            if (user is not null)
            {
                var result = await userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    if (configuration.GetValue<bool>("AutoLogin"))
                        await signInManager.SignInAsync(user, isPersistent: false);
                    return View("EmailConfirmed");
                }
            }
            return View("InvalidConfirmation");

        }

        [HttpPost, Authorize]
        public async Task<IActionResult> Comment(Comment model)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var product = await context.Cars.FindAsync(model.CarId);
            if (product is null)
                return BadRequest();
            model.DateCreated = DateTime.Now;
            model.Enabled = false;
            model.ApplicationUserId = userId;
            await context.Comments.AddAsync(model);
            await context.SaveChangesAsync();
            return RedirectToRoute("product", new { id = model.CarId, name = product.Name.ToSafeUrlString() });
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> ShoppingCart()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var model = await context.Users.FindAsync(userId);
            return View(model);
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> Payment()
        {
            return View();
        }
        [HttpPost, Authorize]
        public async Task<IActionResult> Payment(PaymentViewModel model)
        {
            //Tahsilat yapıldıktan sonra.
            var user = await userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var order = new Order
            {
                ApplicationUserId = user.Id,
                DateCreated = DateTime.Now,
                DeliveryAddressId = Guid.Parse("d6085e1a-d965-47ca-b34f-3c2ccdf9e58d"),
                Enabled = true,
                Status = OrderStatus.New,
                OrderItems = user.ShoppingCartItems.Select(p => new OrderItem
                {
                    
                    Price = p.Car.Price,
                    CarId = p.CarId,
                    Quantity = p.Quantity,
                }).ToList()
            };
            await context.AddAsync(order);
            await context.SaveChangesAsync();
            await shoppingCartService.ClearCart();
            return View();
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> MyOrders()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var model = await context
                .Orders
                .Where(p => p.ApplicationUserId == userId)
                .OrderBy(p => p.Status)
                .ThenByDescending(p => p.DateCreated)
                .ToListAsync();
            return View(model);
        }
    }
}
