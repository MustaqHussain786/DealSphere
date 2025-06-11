using System.Diagnostics;
using DealSphere.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DealSphere.Data; // ⬅️ Make sure this matches your namespace
using System.Linq;

namespace DealSphere.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(string search, string category, string tab = "Trending", int page = 1)
        {
            var deals = _context.Deals.Where(d => d.IsPublished);

            if (!string.IsNullOrWhiteSpace(search))
                deals = deals.Where(d => d.Title.Contains(search) || d.Store.Contains(search));

            if (!string.IsNullOrWhiteSpace(category))
                deals = deals.Where(d => d.Category == category);

            switch (tab)
            {
                case "TopDiscounts":
                    deals = deals.OrderByDescending(d => (d.ActualPrice - d.DiscountPrice) / d.ActualPrice);
                    break;
                case "NewDeals":
                    deals = deals.OrderByDescending(d => d.CreatedAt);
                    break;
                default: // Trending
                    deals = deals.OrderByDescending(d => d.Rating);
                    break;
            }

            var list = deals.Skip((page - 1) * 6).Take(6).ToList();

            ViewBag.Search = search;
            ViewBag.Categories = _context.Deals.Select(d => d.Category).Distinct().ToList();
            ViewBag.Tab = tab;
            ViewBag.TotalPages = (int)Math.Ceiling(deals.Count() / 6.0);
            ViewBag.Page = page;

            return View(list);
        }




        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        


    }
}
