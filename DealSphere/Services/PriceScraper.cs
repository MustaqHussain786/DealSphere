using HtmlAgilityPack;
using System.Globalization;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

public class PriceScraper
{
    private readonly ILogger<PriceScraper> _logger;

    public PriceScraper(ILogger<PriceScraper> logger)
    {
        _logger = logger;
    }

    public async Task<decimal?> FetchPriceFromUrlAsync(string url)
    {
        try
        {
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Adjust this XPath if needed for other websites
            var priceNode = doc.DocumentNode.SelectSingleNode("//span[@class='a-price-whole']"); // Amazon

            if (priceNode != null)
            {
                var priceText = priceNode.InnerText.Replace("₹", "").Replace(",", "").Trim();
                if (decimal.TryParse(priceText, NumberStyles.Any, CultureInfo.InvariantCulture, out var price))
                {
                    return price;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Error fetching price from {Url}: {Message}", url, ex.Message);
        }

        return null;
    }
}
