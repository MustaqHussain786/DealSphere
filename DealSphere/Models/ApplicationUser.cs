using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace DealSphere.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; } = string.Empty;

        public string? ProfilePictureFileName { get; set; }

        [NotMapped]
        public List<string> Roles { get; set; } = new();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        public string ProfilePicturePath =>
      string.IsNullOrEmpty(ProfilePictureFileName)
      ? "/images/default-profile.png"
      : $"/uploads/profile_pics/{ProfilePictureFileName}";
    }
}
