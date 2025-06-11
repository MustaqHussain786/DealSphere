using System;
using DealSphere.Models;


namespace DealSphere.Models
{
    public class PriceHistory
    {
        public int Id { get; set; }

        public int DealId { get; set; }
        public required Deal Deal { get; set; }

        public decimal Price { get; set; }

        public DateTime DateRecorded { get; set; }
    }
}
