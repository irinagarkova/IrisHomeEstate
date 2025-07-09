using AutoMapper;
using HomeEstate.Services.Core.Dtos;
using HomeEstate.Web.Models;

namespace HomeEstate.Web.Mappings
{
    public class PropertyWebMappingProfile : Profile
    {
        public PropertyWebMappingProfile()
        {
            // Property = source || PropertyDto = destination
            // var souce = Property()
            // var destination = new PropertyDto();
            CreateMap<PropertyDto, PropertyViewModel>()
                // destination.Images = soruce.Images
                //.ForMember(dest => dest.Images, opt => opt.MapFrom(p => p.Images))
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.Location, opt => opt.Ignore())
                .ForMember(dest => dest.Owner, opt => opt.Ignore());


            CreateMap<AddAndUpdatePropertyViewModel, PropertyDto>().ReverseMap();
        }
    }
}
