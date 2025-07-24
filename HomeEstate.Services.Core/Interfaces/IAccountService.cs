using HomeEstate.Services.Core.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeEstate.Services.Core.Interfaces
{
    public interface IAccountService
    {
        public Task<ApplicationUserDto> GetApplicationUserDetails(string email);

        public Task<ApplicationUserDto> UpdateApplicationUser(string email);
        public Task<ApplicationUserDto> CreateApplicationUser(ApplicationUserDto applicationUser);
        public Task DeleteApplicationUser(string email);




    }
}
