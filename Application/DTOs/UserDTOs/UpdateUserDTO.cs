using Microsoft.AspNetCore.Http;

namespace Application.DTOs.UserDTOs
{
    public class UpdateUserDTO
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public IFormFile Photo { get; set; }
    }
}
