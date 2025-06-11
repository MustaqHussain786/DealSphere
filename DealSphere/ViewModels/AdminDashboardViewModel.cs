namespace DealSphere.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalDeals { get; set; }
        public int TotalUsers { get; set; }
        public int TotalAdmins { get; set; }
        public int PublishedDeals { get; set; }
        public int UnPublishedDeals { get; set; }

        public List<DealSphere.Models.Deal> RecentDeals { get; set; } = new();
    }
}
