using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalR.Hubs;
using SignalR.Models;

namespace SignalR.Controllers
{
    public class ProductController(IHubContext<ProductHub> hub) : Controller
    {
        ShopifyEntity context = new ();
        private readonly IHubContext<ProductHub> _hub = hub;

        public IActionResult Index()
        {
            return View(context.Products.ToList());
        }

        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        public IActionResult New(Product product)
        {
            context.Products.Add(product);
            context.SaveChanges();

            TempData["NewProductName"] = product.Name;

            return RedirectToAction("Index");
        }
    }
}
