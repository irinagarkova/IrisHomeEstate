using HomeEstate.Data;
using HomeEstate.Models;
using HomeEstate.Services.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeEstate.Services.Core
{
    public class PropertyService : IPropertyService
    {
        private readonly HomeEstateDbContext dbContext;

        public PropertyService(HomeEstateDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task CreatePropertyAsync(Property property)
        {
            var newProperty = new Property
            {
                Id = property.Id,
                Title = property.Title,
                Description = property.Description,
                Price = property.Price,
                Area = property.Area,
                Category = property.Category

            };
            await dbContext.AddAsync(newProperty);
            await dbContext.SaveChangesAsync();

        }

        public async Task DeletePropertyAsync(int id)
        {
            var property = await GetPropertyAsync(id);
            if(property != null)
            {
                property.IsDeleted = true;
                await dbContext.SaveChangesAsync();
            }
        }

        public async  Task<ICollection<Property>> GetAllPropertiesAsync()
        {
            var properties = await dbContext.Properties.ToListAsync();
            return properties;
        }

        public async Task<Property> GetPropertyAsync(int id)
        {
            var property = await dbContext.Properties.FirstOrDefaultAsync(p => p.Id == id);
            return property;
        }

        public async Task<Property> UpdatePropertyAsync(Property property)
        {
            var propertyToUpdate = await GetPropertyAsync(property.Id);
            if(propertyToUpdate !=null)
            {
                propertyToUpdate.Title = property.Title;
                propertyToUpdate.Description = property.Description;
                propertyToUpdate.Price = property.Price;
                propertyToUpdate.Area = property.Area;
                propertyToUpdate.CategoryId = property.CategoryId;
               
            }
            
            await dbContext.SaveChangesAsync();
            return propertyToUpdate;
        }
    }
}
