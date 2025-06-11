using DealSphere.Data;
using DealSphere.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Globalization;

namespace DealSphere.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DealController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DealController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LIST + FILTER
        public IActionResult Index(string search, string status)
        {
            var deals = _context.Deals.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                deals = deals.Where(d => d.Title.ToLower().Contains(search.ToLower()));
            }

            if (!string.IsNullOrEmpty(status))
            {
                if (status == "Published")
                    deals = deals.Where(d => d.IsPublished);
                else if (status == "Unpublished")
                    deals = deals.Where(d => !d.IsPublished);
            }

            ViewBag.Search = search;
            ViewBag.Status = status;

            return View("~/Views/Admin/ManageDeals.cshtml", deals.ToList());

        }

        // CREATE
        public IActionResult Create() => View("~/Views/Deal/Create.cshtml");

        [HttpPost]
        public async Task<IActionResult> Create(Deal deal)
        {
            if (ModelState.IsValid)
            {
                deal.CreatedAt = DateTime.UtcNow;
                _context.Deals.Add(deal);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Deal created successfully!";
                return RedirectToAction("Index");
            }

            return View("~/Views/Deal/Create.cshtml", deal);
        }

        // UPLOAD EXCEL
        public IActionResult UploadExcel() => View("~/Views/Deal/UploadExcel.cshtml");

        [HttpPost]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("", "Please select a valid Excel file.");
                return View("~/Views/Deal/UploadExcel.cshtml");
            }

            var deals = new List<Deal>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null || worksheet.Dimension == null)
                    {
                        ModelState.AddModelError("", "Worksheet not found or is empty.");
                        return View("~/Views/Deal/UploadExcel.cshtml");
                    }

                    int rowCount = worksheet.Dimension.End.Row;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        try
                        {
                            var title = worksheet.Cells[row, 1].Text?.Trim();
                            if (string.IsNullOrWhiteSpace(title)) continue;

                            var description = worksheet.Cells[row, 2].Text?.Trim();
                            var actualPriceText = worksheet.Cells[row, 3].Text?.Trim();
                            var discountPriceText = worksheet.Cells[row, 4].Text?.Trim();
                            var ratingText = worksheet.Cells[row, 5].Text?.Trim();
                            var category = worksheet.Cells[row, 6].Text?.Trim();
                            var subcategory = worksheet.Cells[row, 7].Text?.Trim();
                            var productUrl = worksheet.Cells[row, 8].Text?.Trim();
                            var imageUrl = worksheet.Cells[row, 9].Text?.Trim();
                            var store = worksheet.Cells[row, 10].Text?.Trim();
                            var otherStoreProductUrl = worksheet.Cells[row, 11].Text?.Trim();
                            var otherStoreName = worksheet.Cells[row, 12].Text?.Trim();
                            var otherStorePriceText = worksheet.Cells[row, 13].Text?.Trim();

                            decimal actualPrice = 0;
                            decimal discountPrice = 0;
                            float rating = 0;
                            decimal otherStorePrice = 0;

                            bool isValid =
                                decimal.TryParse(actualPriceText, NumberStyles.Any, CultureInfo.InvariantCulture, out actualPrice) &&
                                decimal.TryParse(discountPriceText, NumberStyles.Any, CultureInfo.InvariantCulture, out discountPrice) &&
                                float.TryParse(ratingText, NumberStyles.Any, CultureInfo.InvariantCulture, out rating) &&
                                decimal.TryParse(otherStorePriceText, NumberStyles.Any, CultureInfo.InvariantCulture, out otherStorePrice);

                            if (!isValid) continue;

                            deals.Add(new Deal
                            {
                                Title = title,
                                Description = description,
                                ActualPrice = actualPrice,
                                DiscountPrice = discountPrice,
                                Rating = rating,
                                Category = category,
                                Subcategory = subcategory,
                                ProductUrl = productUrl,
                                ImageUrl = imageUrl,
                                Store = store,
                                OtherStoreProductUrl = otherStoreProductUrl,
                                OtherStoreName = otherStoreName,
                                OtherStorePrice = otherStorePrice,
                                CreatedAt = DateTime.UtcNow
                            });
                        }
                        catch { continue; }
                    }
                }
            }

            if (!deals.Any())
            {
                ModelState.AddModelError("", "No valid rows found in Excel file.");
                return View("~/Views/Deal/UploadExcel.cshtml");
            }

            TempData["DealsJson"] = JsonConvert.SerializeObject(deals);
            return RedirectToAction("PreviewDeals");
        }

        // PREVIEW DEALS
        public IActionResult PreviewDeals()
        {
            var dealsJson = TempData["DealsJson"] as string;
            if (string.IsNullOrEmpty(dealsJson)) return RedirectToAction("UploadExcel");

            TempData.Keep("DealsJson");
            var deals = JsonConvert.DeserializeObject<List<Deal>>(dealsJson);
            return View("~/Views/Deal/PreviewDeals.cshtml", deals);
        }

        // CONFIRM UPLOAD
        [HttpPost]
        public async Task<IActionResult> ConfirmUpload()
        {
            var dealsJson = TempData["DealsJson"] as string;
            if (string.IsNullOrEmpty(dealsJson)) return RedirectToAction("UploadExcel");

            var deals = JsonConvert.DeserializeObject<List<Deal>>(dealsJson);
            foreach (var deal in deals)
            {
                deal.CreatedAt = DateTime.UtcNow;
            }

            _context.Deals.AddRange(deals);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"{deals.Count} deals successfully uploaded.";
            return RedirectToAction("UploadExcel");
        }

        // EDIT
        public async Task<IActionResult> Edit(int id)
        {
            var deal = await _context.Deals.FindAsync(id);
            if (deal == null) return NotFound();
            return View("~/Views/Deal/Edit.cshtml", deal);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Deal deal)
        {
            if (id != deal.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                _context.Update(deal);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Deal updated successfully!";
                return RedirectToAction("Index");
            }

            return View("~/Views/Deal/Edit.cshtml", deal);
        }

        // DELETE
        public async Task<IActionResult> Delete(int id)
        {
            var deal = await _context.Deals.FindAsync(id);
            if (deal == null) return NotFound();
            return View("~/Views/Deal/Delete.cshtml", deal);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deal = await _context.Deals.FindAsync(id);
            if (deal == null) return NotFound();

            _context.Deals.Remove(deal);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Deal deleted successfully.";
            return RedirectToAction("Index");
        }

        // DETAILS
        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            var deal = _context.Deals
                .Include(d => d.PriceHistories)
                .FirstOrDefault(d => d.Id == id && d.IsPublished);

            if (deal == null) return NotFound();

            deal.PriceHistories = deal.PriceHistories
                .OrderBy(ph => ph.DateRecorded)
                .ToList();

            return View("~/Views/Deal/Details.cshtml", deal);
        }

        // TOGGLE PUBLISH
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TogglePublish(int id)
        {
            var deal = _context.Deals.FirstOrDefault(d => d.Id == id);
            if (deal == null) return NotFound();

            deal.IsPublished = !deal.IsPublished;
            _context.SaveChanges();

            TempData["Success"] = $"Deal {(deal.IsPublished ? "published" : "unpublished")} successfully.";
            return RedirectToAction("Index");
        }

        // BULK TOGGLE PUBLISH
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BulkTogglePublish(List<int> selectedIds, string actionType)
        {
            if (selectedIds == null || !selectedIds.Any())
            {
                TempData["Error"] = "Please select at least one deal.";
                return RedirectToAction("Index");
            }

            var deals = _context.Deals.Where(d => selectedIds.Contains(d.Id)).ToList();
            foreach (var deal in deals)
            {
                deal.IsPublished = (actionType == "Publish");
            }

            _context.SaveChanges();
            TempData["Success"] = $"Successfully {(actionType == "Publish" ? "published" : "unpublished")} {deals.Count} deal(s).";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BulkDelete(List<int> selectedIds)
        {
            if (selectedIds == null || !selectedIds.Any())
            {
                TempData["Error"] = "Please select at least one deal to delete.";
                return RedirectToAction("Index");
            }

            var deals = _context.Deals.Where(d => selectedIds.Contains(d.Id)).ToList();
            _context.Deals.RemoveRange(deals);
            _context.SaveChanges();

            TempData["Success"] = $"{deals.Count} deal(s) deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
