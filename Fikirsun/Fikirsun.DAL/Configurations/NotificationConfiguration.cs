using Fikirsun.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fikirsun.DAL.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.NotifyType).IsRequired().HasDefaultValue(1);
            builder.Property(x => x.isRead).HasDefaultValue(false);
            builder.Property(x => x.Content).IsRequired();
            builder.Property(x => x.signDate).HasDefaultValueSql("getdate()");
            builder.HasOne(x => x.user).WithMany(x => x.notifications).HasForeignKey(x => x.userId);
        }
    }
}
