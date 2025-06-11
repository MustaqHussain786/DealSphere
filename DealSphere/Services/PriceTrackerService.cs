using DealSphere.Data;
using DealSphere.Models;
using Microsoft.Extensions.Hosting; // Add this
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class PriceTrackerService : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PriceTrackerService> _logger;
    private readonly PriceScraper _scraper;

    public PriceTrackerService(IServiceScopeFactory scopeFactory, ILogger<PriceTrackerService> logger, PriceScraper scraper)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _scraper = scraper;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await TrackPricesAsync(cancellationToken);
        // You can also schedule it to run daily using a Timer
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // Cleanup if needed
        return Task.CompletedTask;
    }

    private async Task TrackPricesAsync(CancellationToken token)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var deals = await db.Deals
            .Where(d => d.IsPublished && !string.IsNullOrEmpty(d.ProductUrl))
            .ToListAsync(token);

        foreach (var deal in deals)
        {
            var livePrice = await _scraper.FetchPriceFromUrlAsync(deal.ProductUrl);

            if (livePrice.HasValue)
            {
                var priceEntry = new PriceHistory
                {
                    DealId = deal.Id,
                    Price = livePrice.Value,
                    DateRecorded = DateTime.UtcNow,
                    Deal = deal
                };

                db.PriceHistories.Add(priceEntry);
                deal.DiscountPrice = livePrice.Value;
            }
        }

        await db.SaveChangesAsync(token);
        _logger.LogInformation("Tracked and updated prices for {Count} deals", deals.Count);
    }
}
