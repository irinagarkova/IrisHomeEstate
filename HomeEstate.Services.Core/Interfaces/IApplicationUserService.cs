using HomeEstate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeEstate.Services.Core.Interfaces
{
    public interface IApplicationUserService
    {
        Task<ApplicationUser> GetApplicationUser(int id); //Details
        Task CreateApplicationUserAsync(ApplicationUser applicationUser);
        Task<ApplicationUser> UpdateApplicationUser (ApplicationUser applicationUser); //Edit
        Task DeleteApplicationUser (int id);
        Task<ICollection<FavoriteProperty>> GetFavoritePropertiesAsync(int id);

        //images
        

    }
}
