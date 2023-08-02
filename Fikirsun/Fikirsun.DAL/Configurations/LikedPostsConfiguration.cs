using Fikirsun.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fikirsun.DAL.Configurations
{
    public class LikedPostsConfiguration : IEntityTypeConfiguration<LikedPost>
    {
        public void Configure(EntityTypeBuilder<LikedPost> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.user).WithMany(x => x.likedPosts).HasForeignKey(x => x.userId);
        }
    }
}
