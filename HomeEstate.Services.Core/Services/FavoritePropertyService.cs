using AutoMapper;
using HomeEstate.Data;
using HomeEstate.Services.Core.Dtos;
using HomeEstate.Services.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeEstate.Services.Core.Services
{
	public class FavoritePropertyService : IFavoritePropertyService
	{
		private readonly HomeEstateDbContext dbContext;
		private readonly IMapper mapper;

		public FavoritePropertyService(HomeEstateDbContext dbContext, IMapper mapper)
		{
			this.dbContext = dbContext;
			this.mapper = mapper;
		}
		public Task AddPropertyToFavoriteAsync(FavoritePropertyDto property)
		{
			throw new NotImplementedException();
		}


		public Task RemovePropertyFromFavoriteAsync(PropertyDto property)
		{
			throw new NotImplementedException();
		}

		public async Task<ICollection<FavoritePropertyDto>> GetAllFavoritePropertiesAsync()
		{
			var prop = await dbContext.FavoriteProperties
				.Include(p => p.Property)
				.ThenInclude(p=> p.Images)
				.ToListAsync();

			var mapp = prop.Select(p => mapper.Map<FavoritePropertyDto>(p)).ToList();
			return mapp;
		}
	}
}
