using AutoMapper;
using Bookfiy_WepApp.Core.Models;

namespace Bookfiy_WepApp.Core.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //category
            CreateMap<Category , CategoryViewModel>();
            CreateMap<CategoryFormViewModel , Category>().ReverseMap();
        }
    }
}
