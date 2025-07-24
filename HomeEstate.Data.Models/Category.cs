using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HomeEstate.Models
{
    public class Category
    {
       
        [Comment("Category identifier")]

        public int Id { get; set; }

        [Comment("Category name")]
        public string Name { get; set; } = null!;
     
    }
}