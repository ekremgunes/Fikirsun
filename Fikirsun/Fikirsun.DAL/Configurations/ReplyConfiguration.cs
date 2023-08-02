using Fikirsun.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fikirsun.DAL.Configurations
{
    public class ReplyConfiguration : IEntityTypeConfiguration<Reply>
    {
        public void Configure(EntityTypeBuilder<Reply> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Content).IsRequired().HasMaxLength(500);
            builder.Property(x => x.createdDate).HasDefaultValueSql("getdate()");
            builder.Property(x => x.isBanned).HasDefaultValue(false);

            builder.HasOne(x => x.user).WithMany(x => x.replies).HasForeignKey(x => x.userId).OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.comment).WithMany(x => x.replies).HasForeignKey(x => x.commentId);
        }
    }
}
