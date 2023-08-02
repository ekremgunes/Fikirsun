using Fikirsun.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fikirsun.DAL.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.categoryOrder).HasDefaultValueSql("100");
            builder.HasData(new Category()
            {
                Name = "Genel",
                categoryOrder = 1,
                posts = new List<Post>(),
                Id = 1
            });
        }
    }
}