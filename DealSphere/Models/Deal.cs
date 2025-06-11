using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DealSphere.Models
{
    public class Deal
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; }= string.Empty;

        [Display(Name = "Actual Price")]
        public decimal ActualPrice { get; set; }

        [Display(Name = "Discount Price")]
        public decimal DiscountPrice { get; set; }

        public float Rating { get; set; }

        [Required]
        public string Category { get; set; }=string.Empty;

        [Required]
        public string Subcategory { get; set; }=string.Empty;

        [Display(Name = "Product URL")]
        public string ProductUrl { get; set; } = string.Empty;

        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; } =string.Empty;

        public string Store { get; set; } = string.Empty;

        [Display(Name = "Other Store Product URL")]
        public string OtherStoreProductUrl { get; set; } =string.Empty ;

        [Display(Name = "Other Store Name")]
        public string OtherStoreName { get; set; } =string.Empty;

        [Display(Name = "Other Store Price")]
        public decimal OtherStorePrice { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsPublished { get; set; }

        public ICollection<PriceHistory> PriceHistories { get; set; } = new List<PriceHistory>();





    }
}
