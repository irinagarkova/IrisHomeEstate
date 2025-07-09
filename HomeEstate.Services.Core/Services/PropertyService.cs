using AutoMapper;
using HomeEstate.Data;
using HomeEstate.Models;
using HomeEstate.Services.Core.Dtos;
using HomeEstate.Services.Core.Exceptions;
using HomeEstate.Services.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeEstate.Services.Core.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly HomeEstateDbContext dbContext;
        private readonly IMapper mapper;

        public PropertyService(HomeEstateDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task CreatePropertyAsync(PropertyDto property)
        {
            var newProperty = mapper.Map<Property>(property);
            await dbContext.AddAsync(newProperty);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeletePropertyAsync(int id)
        {
            var property = await getProperty(id);
            if (property != null)
            {
                property.IsDeleted = true;
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<ICollection<PropertyDto>> GetAllPropertiesAsync()
        {
            var properties = await dbContext.Properties.ToListAsync();
            var mappedProperties = properties.Select(p => mapper.Map<PropertyDto>(p)).ToList();

            return mappedProperties;
        }

        public async Task<PropertyDto> GetPropertyAsync(int id)
        {
            
            var property = await getProperty(id);
            var mappedProp = mapper.Map<PropertyDto>(property);

            return mappedProp;
        }
       private async Task<Property> getProperty(int id)
       {
            var property = await dbContext.Properties.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (property == null)
            {
                throw new NotFoundException("Property", id);
            }

            return property;

       }
        public async Task UpdatePropertyAsync(PropertyDto property)
        {
            var errors = new Dictionary<string, string>();
            if (property.Area <= 0)
            {
                errors.Add("Area", "Trqbva da e pone 1");
            }

            if (property.Title.Length < 5 || property.Title.Length > 20)
            {
                errors.Add("Tittle", "Trqbwa da e pone 5");
            }

            if (errors.Any())
            {
                throw new CustomValidationException("", errors);
            }

            var propertyToUpdate = await getProperty(property.Id);
            
            propertyToUpdate = mapper.Map<Property>(property);
            dbContext.Properties .Update(propertyToUpdate);
            await dbContext.SaveChangesAsync();
        }
    }
}
