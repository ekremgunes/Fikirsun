using Fikirsun.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fikirsun.DAL.Configurations
{
    public class SettingsConfiguration : IEntityTypeConfiguration<Settings>
    {
        public void Configure(EntityTypeBuilder<Settings> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.SiteSeo).IsRequired();
            builder.Property(x => x.Email).IsRequired();
            builder.Property(x => x.ExtraLink).IsRequired();
            builder.Property(x => x.SiteDescription).IsRequired();
            builder.Property(x => x.SiteLogo).IsRequired();
            builder.Property(x => x.SiteSeo).IsRequired();

            builder.Property(x => x.TwitterVisibility).HasDefaultValue(false).IsRequired();
            builder.Property(x => x.FacebookVisibility).HasDefaultValue(false).IsRequired();
            builder.Property(x => x.TwitterVisibility).HasDefaultValue(false).IsRequired();
            builder.Property(x => x.InstagramVisibility).HasDefaultValue(false).IsRequired();


            builder.HasData(new Settings
            {
                Email = "info@Fikirsun.com",
                Facebook = "facebook",
                ExtraLink = "github?",
                Instagram = "instagram",
                Youtube = "youtube",
                SiteDescription = "description",
                SiteLogo = "logo.png",
                SiteSeo = "Fikirsun",
                Twitter = "twitter link",
                Id = 1
            });
        }
    }
}
