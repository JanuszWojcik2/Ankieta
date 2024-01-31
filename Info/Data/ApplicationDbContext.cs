using Info.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Info.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Category>? Categories { get; set; }
        public DbSet<Text>? Texts { get; set; }
        public DbSet<Opinion>? Opinions { get; set; }
        public DbSet<AppUser>? AppUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            base.OnModelCreating(modelbuilder);

            modelbuilder.Entity<Category>()
                .HasMany(c => c.Texts)
                .WithOne(t => t.Category);

            modelbuilder.Entity<Text>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Texts);

            modelbuilder.Entity<Text>()
                .HasMany(t => t.Opinions)
                .WithOne(o => o.Text);

            modelbuilder.Entity<Opinion>()
                .HasOne(o => o.Text)
                .WithMany(t => t.Opinions)
                .OnDelete(DeleteBehavior.Restrict);

            modelbuilder.Entity<AppUser>()
                .HasMany(u => u.Texts)
                .WithOne(t => t.User);

            modelbuilder.Entity<Text>()
                .HasOne(u => u.User)
                .WithMany(u => u.Texts);

            modelbuilder.Entity<AppUser>()
                .HasMany(u => u.Opinions)
                .WithOne(o => o.User);

            modelbuilder.Entity<Opinion>()
                .HasOne(o => o.User)
                .WithMany(u => u.Opinions);
        }
    }
}
