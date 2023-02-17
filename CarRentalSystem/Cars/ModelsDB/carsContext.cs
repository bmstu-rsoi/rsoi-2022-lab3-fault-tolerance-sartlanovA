using Microsoft.EntityFrameworkCore;

namespace Cars.ModelsDB
{
    public partial class CarsContext : DbContext
    {
        public CarsContext()
        {
        }

        public CarsContext(DbContextOptions<CarsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Car> Cars { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=cars;Username=postgres;Password=22rfrnec");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Car>(entity =>
            {
                entity.ToTable("cars");

                entity.HasIndex(e => e.CarUid, "cars_car_uid_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Availability).HasColumnName("availability");

                entity.Property(e => e.Brand)
                    .HasMaxLength(80)
                    .HasColumnName("brand");

                entity.Property(e => e.CarUid).HasColumnName("car_uid");

                entity.Property(e => e.Model)
                    .HasMaxLength(80)
                    .HasColumnName("model");

                entity.Property(e => e.Power).HasColumnName("power");

                entity.Property(e => e.Price).HasColumnName("price");

                entity.Property(e => e.RegistrationNumber)
                    .HasMaxLength(20)
                    .HasColumnName("registration_number");

                entity.Property(e => e.Type)
                    .HasMaxLength(20)
                    .HasColumnName("type");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
