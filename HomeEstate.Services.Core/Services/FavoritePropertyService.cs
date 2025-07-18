using AutoMapper;
using HomeEstate.Data;
using HomeEstate.Models;
using HomeEstate.Services.Core.Dtos;
using HomeEstate.Services.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeEstate.Services.Core.Services
{
    public class FavoritePropertyService : IFavoritePropertyService
    {
        private readonly HomeEstateDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IApplicationUserService applicationUserService;
        private readonly IPropertyService propertyService;

        public FavoritePropertyService(HomeEstateDbContext dbContext, IMapper mapper, IApplicationUserService applicationUserService, IPropertyService propertyService)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.applicationUserService = applicationUserService;
            this.propertyService = propertyService;
        }

        public async Task AddPropertyToFavoriteAsync(int id, string email)
        { 

            var prop = await propertyService.GetPropertyAsync(id);
            var user = await applicationUserService.GetApplicationUser(email);

            bool alreadyExists = await dbContext.FavoriteProperties
                .AnyAsync(fp => fp.PropertyId == prop.Id && fp.UserId == user.Id);

            if (alreadyExists)
            {
                throw new Exception("You already have this property in favorite");
               
            }

            var favProp = new FavoriteProperty
            {
                UserId = user.Id,
                PropertyId = prop.Id,
            };

            await dbContext.AddAsync(favProp);
            await dbContext.SaveChangesAsync();
        }

        public async Task<ICollection<FavoritePropertyDto>> GetAllFavoritePropertiesAsync(string email)
        {
            var prop = await dbContext.FavoriteProperties
                 .Where(p => p.User.Email == email)
                 .Include(p => p.Property)
                     .ThenInclude(p => p.Images)
                 .Include(p => p.Property.Location)
                 .Include(p => p.Property.Category)
                 .ToListAsync();
                
            var mapp = prop.Select(p => mapper.Map<FavoritePropertyDto>(p)).ToList();
            return mapp;
        }

        public async Task RemovePropertyFromFavoriteAsync(int id, string email)
        {
            var user = await applicationUserService.GetApplicationUser(email);
            var favoriteprop = await dbContext.FavoriteProperties.FirstOrDefaultAsync(fp => fp.PropertyId == id && fp.UserId == user.Id);

            if (favoriteprop != null)
            {
                dbContext.FavoriteProperties.Remove(favoriteprop);
                await dbContext.SaveChangesAsync();
            }

        }
    }
}
