using AutoMapper;
using HomeEstate.Data;
using HomeEstate.Models;
using HomeEstate.Services.Core.Dtos;
using HomeEstate.Services.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace HomeEstate.Services.Core.Services
{
    public class ApplicationUserService : IApplicationUserService
    {
        private readonly HomeEstateDbContext dbContext;
        private readonly IMapper mapper;

        public ApplicationUserService(HomeEstateDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<ApplicationUser> GetApplicationUser(string email)
        {
            var user = await dbContext.ApplicationUser
                .Include(p=>p.FavoriteProperties)
                .ThenInclude(x=>x.Property)
                .ThenInclude(x=>x.Images)
                .FirstOrDefaultAsync(x=> x.Email == email);
            if(user != null)
            {
                return user; 
            }
            throw new Exception();
        }
        public async Task<ApplicationUser> UpdateApplicationUser(ApplicationUserDto applicationUser)
        {
            var userFromDb = await dbContext.ApplicationUser.FindAsync(applicationUser.Id);
            if (userFromDb == null)
            {
                throw new Exception($"User with ID {applicationUser.Id} not found.");
            }

            mapper.Map(applicationUser, userFromDb);

            await dbContext.SaveChangesAsync();

            return userFromDb;
        }

        public async Task DeleteApplicationUser(int id)
        {
            var user = await dbContext.ApplicationUser.FindAsync(id);
            if (user == null)
            {
                throw new Exception($"User with ID {id} not found.");
            }

            dbContext.ApplicationUser.Remove(user);
            await dbContext.SaveChangesAsync();
        }

    }
}
