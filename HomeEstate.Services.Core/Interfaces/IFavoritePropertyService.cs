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
		Task AddPropertyToFavoriteAsync(FavoritePropertyDto property);
		Task RemovePropertyFromFavoriteAsync(PropertyDto property);
		Task<ICollection<FavoritePropertyDto>> GetAllFavoritePropertiesAsync();


	}
}
