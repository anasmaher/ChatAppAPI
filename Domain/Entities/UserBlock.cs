namespace Domain.Entities
{
    public class UserBlock
    {
        public string UserId { get; set; }
        public AppUser User { get; set; }

        public string BlockedUserId { get; set; }
        public AppUser BlockedUser { get; set; }
    }
}
