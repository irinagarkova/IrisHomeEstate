using HomeEstate.Services.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeEstate.Services.Core.Interfaces
{
	public interface IFavoritePropertyService
	{
		Task AddPropertyToFavoriteAsync(int id, string email);
		Task RemovePropertyFromFavoriteAsync(int id, string email);
		Task<int> GetFavoriteCountForPropertyAsync(int propertyId);
		Task<ICollection<FavoritePropertyDto>> GetAllFavoritePropertiesAsync(string email);


	}
}
