using Application.DTOs.UserDTOs;

namespace Application.DTOs.GroupDTOs
{
    public class GroupDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<UserDTO> Members { get; set; }
    }
}
