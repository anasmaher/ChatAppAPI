namespace Utilities
{
    public static class UserHelpers
    {
        public static (string UserId1, string UserId2) GetOrderedUserIds(string userIdA, string userIdB)
        {
            if (string.Compare(userIdA, userIdB, StringComparison.Ordinal) < 0)
                return (userIdA, userIdB);
            
            else
                return (userIdB, userIdA);
        }
    }
}
