using AutoMapper;
using HomeEstate.Models;
using HomeEstate.Services.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeEstate.Services.Core.Mappings
{
    public class PropertyMappingProfile : Profile
    {
        public PropertyMappingProfile()
        {
            // Property = source || PropertyDto = destination

            CreateMap<Property, PropertyDto>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(s => s.Category))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(s => s.Location))
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(s => s.Owner))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(s => s.Images))
                .ForMember(dest => dest.FavoriteCount, opt => opt.MapFrom(s => s.FavoriteProperties.Count))
                .ForMember(dest => dest.PetsAllowed, opt => opt.MapFrom(src => (bool?)src.PetsAllowed))
                .ForMember(dest => dest.IsFurnished, opt => opt.MapFrom(src => (bool?)src.IsFurnished))
                .ForMember(dest => dest.IsParking, opt => opt.MapFrom(src => (bool?)src.IsParking)).ReverseMap();

            CreateMap<FavoriteProperty, FavoritePropertyDto>()
                .ForMember(dest => dest.Property, opt => opt.MapFrom(s => s.Property))
                .ForMember(dest => dest.User, opt => opt.MapFrom(s => s.User))
                .ReverseMap();

            CreateMap<PropertyImage, PropertyImageDto>()
                .ForMember(dest => dest.PropertyId, opt => opt.MapFrom(s => s.Property.Id))
                .ReverseMap();

            CreateMap<Category, CategoryDto>().ReverseMap();

            CreateMap<Location, LocationDto>().ReverseMap();

            CreateMap<ApplicationUser, ApplicationUserDto>().ReverseMap();
            //.ForMember(dest => dest.Id, opt => opt.MapFrom(s => s.Id))
            //.ForMember(dest => dest.UserName, opt => opt.MapFrom(s => s.UserName))

            CreateMap<ApplicationUser, ApplicationUserWithRoleDto>();


        }


    }
}
