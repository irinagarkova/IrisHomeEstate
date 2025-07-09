using HomeEstate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeEstate.Services.Core.Interfaces
{
    public class ApplicationUserService : IApplicationUserService
    {
        public Task CreateApplicationUserAsync(ApplicationUser applicationUser)
        {
            throw new NotImplementedException();
        }

        public Task DeleteApplicationUser(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser> GetApplicationUser(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<FavoriteProperty>> GetFavoritePropertiesAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser> UpdateApplicationUser(ApplicationUser applicationUser)
        {
            throw new NotImplementedException();
        }
    }
}
