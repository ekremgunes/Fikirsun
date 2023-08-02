using Fikirsun.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fikirsun.DAL.Configurations
{
    public class SpamWordConfiguration : IEntityTypeConfiguration<SpamWord>
    {
        public void Configure(EntityTypeBuilder<SpamWord> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasAnnotation("MinLength", 2);
        }
    }
}
