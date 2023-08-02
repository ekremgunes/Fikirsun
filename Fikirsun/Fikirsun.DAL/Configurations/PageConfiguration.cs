using Fikirsun.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fikirsun.DAL.Configurations
{
    public class PageConfiguration : IEntityTypeConfiguration<Page>
    {
        public void Configure(EntityTypeBuilder<Page> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title).IsRequired();
            builder.Property(x => x.Content).IsRequired();
            builder.HasData(new Page()
            {
                Id = 1,
                Content = "Agreement lorem ipsum dolor sit a.",
                Title = "KVKK",
            });
        }
    }
}
