using VozniPark.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace VozniPark.Data
{
    public class ApplicationDbContext : IdentityDbContext<Korisnik>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 

           
            modelBuilder.Entity<Servis>()
                .Property(s => s.Cijena)
                .HasPrecision(18, 2);

            
            modelBuilder.Entity<Servis>()
                .HasOne(s => s.Vozilo)          
                .WithMany(v => v.Servisi)       
                .HasForeignKey(s => s.VoziloId) 
                .OnDelete(DeleteBehavior.Cascade); 
        }

        
        public DbSet<Vozilo> Vozila { get; set; }
        public DbSet<Servis> Servisi { get; set; }
    }
}