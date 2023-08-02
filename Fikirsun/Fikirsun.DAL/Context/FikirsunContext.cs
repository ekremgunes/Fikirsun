using Fikirsun.DAL.Configurations;
using Fikirsun.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Fikirsun.DAL.Context
{
    public class FikirsunContext : IdentityDbContext<AppUser, AppRole, int,
        IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,
        IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<SpamWord> SpamWords { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Reply> Replies { get; set; }
        public DbSet<LikedComment> LikedComments { get; set; }
        public DbSet<LikedPost> LikedPosts { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public FikirsunContext(DbContextOptions<FikirsunContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>(b =>
            {
                b.HasMany(e => e.UserRoles)
                .WithOne(e => e.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
                b.Property(x => x.signDate).HasDefaultValueSql("getdate()");
                b.Property(x => x.profilePhoto).HasDefaultValue("defaultPP.png");
                b.Property(x => x.userSubName).HasDefaultValue("Fikirsun !");
            });

            modelBuilder.Entity<AppRole>(b =>
            {
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();
            });

            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new PostConfiguration());
            modelBuilder.ApplyConfiguration(new TagConfiguration());
            modelBuilder.ApplyConfiguration(new SettingsConfiguration());
            modelBuilder.ApplyConfiguration(new CommentConfiguration());
            modelBuilder.ApplyConfiguration(new ReplyConfiguration());
            modelBuilder.ApplyConfiguration(new PageConfiguration());
            modelBuilder.ApplyConfiguration(new SpamWordConfiguration());

        }

    }
}
