namespace ChatAppAPI.ViewModels.UserVMs
{
    public class UpdateUserVM
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public IFormFile? Photo { get; set; }
    }
}
