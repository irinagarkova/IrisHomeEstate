using HomeEstate.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using static HomeEstate.HomeEstateCommon.EntityConstants.CategoryConstants;

namespace HomeEstate.Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder
                .HasKey(c => c.Id);

            builder
                .Property(c => c.Name)
                .IsRequired(true)
                .HasMaxLength(NameMaxLength);
            
            builder
                .HasMany<Property>()
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            
        }
    }
}
    
