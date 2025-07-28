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
                .ForMember(dest => dest.Images, opt => opt.MapFrom(p => p.Images))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(s => s.Category))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(l => l.Location))
                .ForMember(dest => dest.Owner, opt => opt.Ignore());

            //class 1 prop int Id  prop Address Address class2  2 prop int Id prop Address
            CreateMap<AddAndUpdatePropertyViewModel, PropertyDto>()
                 .ForMember(dest => dest.Images, opt => opt.Ignore()).ReverseMap();

            CreateMap<PropertyDto, DetailsViewModel>();

            CreateMap<FavoritePropertyDto, FavoritePropertyViewModel>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(x => x.Property.Price))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(x => x.Property.Location != null ? x.Property.Location.Address : "N/A"))  
                .ForMember(dest => dest.PropertyId, opt => opt.MapFrom(x => x.Property.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(x => x.Property.Title))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(x => x.Property.Images.Any() ? x.Property.Images.First().ImageUrl : ""))
                .ReverseMap();

            CreateMap<AddAndUpdatePropertyViewModel, PropertyDto>()
               .ForMember(dest => dest.Images, opt => opt.Ignore())
               .ForMember(dest => dest.MonthlyRent, opt => opt.MapFrom(src => src.MonthlyRent))
               .ForMember(dest => dest.SecurityDeposit, opt => opt.MapFrom(src => src.SecurityDeposit))
               .ForMember(dest => dest.MinimumLeasePeriod, opt => opt.MapFrom(src => src.MinimumLeasePeriod))
               .ForMember(dest => dest.AvailableFrom, opt => opt.MapFrom(src => src.AvailableFrom))
               .ForMember(dest => dest.PetsAllowed, opt => opt.MapFrom(src => src.PetsAllowed))
               .ForMember(dest => dest.IsFurnished, opt => opt.MapFrom(src => src.IsFurnished))
               .ForMember(dest => dest.IsParking, opt => opt.MapFrom(src => src.IsParking))
               .ForMember(dest => dest.PropertyType, opt => opt.MapFrom(src => src.PropertyType))
               .ReverseMap();

            CreateMap<RegisterViewModel, ApplicationUser>()
             .ForMember(dest => dest.UserName, opt => opt.MapFrom(x => x.Email));

            CreateMap<SearchViewModel, SearchPropertyDto>().ReverseMap();
        }
    }
}
