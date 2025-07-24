using AutoMapper;
using HomeEstate.Data;
using HomeEstate.Models;
using HomeEstate.Services.Core.Dtos;
using HomeEstate.Services.Core.Exceptions;
using HomeEstate.Services.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeEstate.Services.Core.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IMapper mapper;
        private readonly HomeEstateDbContext dbContext;

        public CategoryService(HomeEstateDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }


        public async Task<ICollection<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await dbContext.Categories
                .OrderBy(c => c.Id)
                .ToListAsync();

            var mapped = categories.Select(c=> mapper.Map<CategoryDto>(c)).ToList();
            return mapped;
           
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(int id)
        {
            var category = await dbContext.Categories
                .FirstOrDefaultAsync(c =>c.Id == id);

            if (category != null)
            {
                throw new NotFoundException("Category", id);
            }

            return mapper.Map<CategoryDto>(category);

        }

        public async Task<CategoryDto> GetCategoryByNameAsync(string name)
        {
            var category = await dbContext.Categories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());

            if (category == null)
                throw new NotFoundException("Category", name);

            return mapper.Map<CategoryDto>(category);
        }
    }

}

