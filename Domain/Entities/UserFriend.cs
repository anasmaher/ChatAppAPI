namespace Domain.Entities
{
    public class UserFriend
    {
        public string UserId { get; set; }
        public AppUser User { get; set; }

        public string FriendId { get; set; }
        public AppUser Friend { get; set; }
    }
}
