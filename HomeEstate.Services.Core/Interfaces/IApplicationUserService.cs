using HomeEstate.Models;
using HomeEstate.Services.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeEstate.Services.Core.Interfaces
{
    public interface IApplicationUserService
    {
        Task<ApplicationUser> GetApplicationUser(string email); 
        Task<ApplicationUser> UpdateApplicationUser (ApplicationUserDto applicationUser); 
        Task DeleteApplicationUser (int id);

    }
}
