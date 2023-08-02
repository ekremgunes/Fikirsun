using Fikirsun.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fikirsun.DAL.Configurations
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.postTitle).IsRequired().HasMaxLength(250);
            builder.Property(x => x.viewCount).HasDefaultValueSql("1");
            builder.Property(x => x.createdDate).HasDefaultValueSql("getdate()");
            builder.Property(x => x.postContent).IsRequired();
            builder.Property(x => x.likeCount).HasDefaultValueSql("0");
            builder.Property(x => x.isEdited).HasDefaultValue(false);
            builder.Property(x => x.isBanned).HasDefaultValue(false);

            builder.HasOne(x => x.user).WithMany(x => x.posts).HasForeignKey(x => x.userId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.category).WithMany(x => x.posts).HasForeignKey(x => x.categoryId);
        }
    }
}
