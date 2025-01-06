using Microsoft.AspNetCore.Http;

namespace Application.DTOs.UserDTOs
{
    public class RegisterDTO
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string ConfirmedPassword { get; set; }

        public IFormFile Photo { get; set; }
    }
}
