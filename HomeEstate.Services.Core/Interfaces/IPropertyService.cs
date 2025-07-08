using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeEstate.Models;
namespace HomeEstate.Services.Core.Interfaces
{
    public interface IPropertyService 
    {
        Task<Property> GetPropertyAsync(int id); //Details
        Task CreatePropertyAsync(Property property); 
        Task<Property> UpdatePropertyAsync(Property property); //Edit
        Task DeletePropertyAsync(int id);
        Task<ICollection<Property>> GetAllPropertiesAsync();


    }
}
