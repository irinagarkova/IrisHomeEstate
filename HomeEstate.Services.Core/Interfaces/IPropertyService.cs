using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeEstate.Models;
using HomeEstate.Services.Core.Dtos;
namespace HomeEstate.Services.Core.Interfaces
{
    public interface IPropertyService 
    {
        Task<PropertyDto> GetPropertyAsync(int id); //Details
        Task CreatePropertyAsync(PropertyDto property); 
        Task UpdatePropertyAsync(PropertyDto property); //Edit
        Task DeletePropertyAsync(int id);
        Task<ICollection<PropertyDto>> GetAllPropertiesAsync();


    }
}
