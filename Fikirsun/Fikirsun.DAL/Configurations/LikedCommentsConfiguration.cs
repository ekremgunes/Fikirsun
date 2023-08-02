using Fikirsun.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Fikirsun.DAL.Configurations
{
    public class LikedCommentsConfiguration : IEntityTypeConfiguration<LikedComment>
    {
        public void Configure(EntityTypeBuilder<LikedComment> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.user).WithMany(x => x.likedComments).HasForeignKey(x => x.userId);
        }
    }
}
