using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhotoFilePath { get; set; } = "wwwroot/uploads/photos/defaultPhoto.png";

        public int TokenVersion { get; set; } = 1;

        public string? RefreshToken { get; set; }

        public DateTime RefreshTokenExpiryTime { get; set; }

        public virtual ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();

    }
}
