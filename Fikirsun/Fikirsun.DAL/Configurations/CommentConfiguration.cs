using Fikirsun.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fikirsun.DAL.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Content).IsRequired().HasMaxLength(1500);
            builder.Property(x => x.likeCount).HasDefaultValueSql("0");
            builder.Property(x => x.createdDate).HasDefaultValueSql("getdate()");
            builder.Property(x => x.isAnswer).HasDefaultValue(false);
            builder.Property(x => x.isBanned).HasDefaultValue(false);
            builder.HasOne(x => x.Post).WithMany(x => x.comments).HasForeignKey(x => x.postId);
            builder.HasOne(x => x.user).WithMany(x => x.comments).HasForeignKey(x => x.userId).OnDelete(DeleteBehavior.SetNull);

        }
    }
}
