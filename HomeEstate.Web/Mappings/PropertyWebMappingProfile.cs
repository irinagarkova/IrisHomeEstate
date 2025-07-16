using AutoMapper;
using HomeEstate.Models;
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

            //class 1 prop int Id  prop Address Address class2  2 prop int Id prop Address
            CreateMap<AddAndUpdatePropertyViewModel, PropertyDto>().ReverseMap();

            CreateMap<FavoritePropertyDto, FavoritePropertyViewModel>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(x=> x.Property.Price))
                //.ForMember(dest => dest.Address, opt => opt.MapFrom(x =>x.Property.Location.Address))
                .ForMember(dest => dest.PropertyId, opt=> opt.MapFrom(x=>x.Property.Id))
                .ForMember(dest =>dest.Title, opt=> opt.MapFrom(x=>x.Property.Title))
                .ForMember(dest => dest.ImageUrl, opt=> opt.MapFrom(x=>x.Property.Images.First().ImageUrl))
                .ReverseMap();

            CreateMap<RegisterViewModel, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(x => x.Email));
               
        }
    }
}
