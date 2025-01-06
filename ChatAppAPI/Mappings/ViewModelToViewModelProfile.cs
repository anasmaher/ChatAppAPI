using AutoMapper;
using ChatAppAPI.ViewModels.UserVMs;

namespace ChatAppAPI.Mappings
{
    public class ViewModelToViewModelProfile : Profile
    {
        public ViewModelToViewModelProfile()
        {
            CreateMap<RegisterVM, UserVM>();
        }
    }
}
