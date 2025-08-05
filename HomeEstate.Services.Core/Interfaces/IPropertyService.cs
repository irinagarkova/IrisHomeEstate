using HomeEstate.Data.Models.Enum;
using HomeEstate.Services.Core.Dtos;
namespace HomeEstate.Services.Core.Interfaces
{
    public interface IPropertyService 
    {
        Task<PropertyDto> GetPropertyAsync(int id); //Details
        Task CreatePropertyAsync(PropertyDto property); 
        Task UpdatePropertyAsync(PropertyDto property); //Edit
        Task DeletePropertyAsync(int id);
        Task<ICollection<PropertyDto>> GetPropertiesByUserIdAsync(string userId);
        Task<Pagination<PropertyDto>> GetAllPropertiesAsync(int page, int pageSize);
        Task<ICollection<PropertyDto>> GetAllPropertiesAsync(SearchPropertyDto search); // Search

        // New methods for rent/sale functionality
        Task<ICollection<PropertyDto>> GetPropertiesByTypeAsync(PropertyListingType type);
        Task<ICollection<PropertyDto>> GetPropertiesForSaleAsync();
        Task<ICollection<PropertyDto>> GetPropertiesForRentAsync();
        Task<Pagination<PropertyDto>> SearchPropertiesAsync(PropertySearchDto searchCriteria);
        Task<PropertyStatisticsDto> GetUserPropertyStatisticsAsync(string userId);
		Task<IEnumerable<LocationDto>> GetAllLocations();

        Task<RentalStatisticsDto> GetRentalStatisticsAsync(string userId);
        Task<int> GetTotalPropertiesCount();
        Task<List<PropertyDto>> GetRecentProperties(int count);
        Task<ICollection<PropertyDto>> GetSimilarPropertiesAsync(int propertyId, int count);
    }

}

