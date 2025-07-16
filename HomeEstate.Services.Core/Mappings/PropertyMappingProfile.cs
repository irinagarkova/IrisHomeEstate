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
            // var souce = Property()
            // var destination = new PropertyDto();
            CreateMap<Property, PropertyDto>()
                // destination.Images = soruce.Images
                //.ForMember(dest => dest.Images, opt => opt.MapFrom(p => p.Images))
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.Location, opt => opt.Ignore())
                .ForMember(dest => dest.Owner, opt => opt.Ignore())
                .ForMember(dest => dest.Images, opt=> opt.MapFrom(s=>s.Images))
                .ReverseMap();

            CreateMap<FavoriteProperty,FavoritePropertyDto>()
                .ForMember(dest => dest.Property, opt => opt.MapFrom(s=> s.Property))
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<PropertyImage, PropertyImageDto>().ReverseMap();
        }   

    }
}
