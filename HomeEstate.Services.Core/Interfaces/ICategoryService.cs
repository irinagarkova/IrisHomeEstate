using HomeEstate.Services.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeEstate.Services.Core.Interfaces
{
    public interface ICategoryService
    {
        Task<ICollection<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto> GetCategoryByIdAsync(int id);
        Task<CategoryDto> GetCategoryByNameAsync(string name);


    }
}
