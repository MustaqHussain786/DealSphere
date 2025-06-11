using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DealSphere.Models
{
    public class ProfileViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Profile Picture")]
        public IFormFile? ProfilePicture { get; set; }

        public string? ExistingProfilePicturePath { get; set; }

        public ProfileViewModel()
        {
            Name = string.Empty;
            Email = string.Empty;
        }
    }
}
