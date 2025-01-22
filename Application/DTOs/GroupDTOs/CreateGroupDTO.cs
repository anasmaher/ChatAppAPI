namespace Application.DTOs.GroupDTOs
{
    public class CreateGroupDTO
    {
        public string Name { get; set; }

        public List<string> MembersIds { get; set; }
    }
}
