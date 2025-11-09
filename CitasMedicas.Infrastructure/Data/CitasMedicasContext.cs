using Microsoft.EntityFrameworkCore;
using CitasMedicas.Core.Entities;

namespace CitasMedicas.Infrastructure.Data
{
    public class CitasMedicasContext : DbContext
    {
        public CitasMedicasContext(DbContextOptions<CitasMedicasContext> options)
            : base(options)
        {
        }

        public DbSet<Medico> Medicos { get; set; } = null!;
        public DbSet<Paciente> Pacientes { get; set; } = null!;
        public DbSet<Cita> Citas { get; set; } = null!;
        public DbSet<Pago> Pagos { get; set; } = null!;
        public DbSet<Disponibilidad> Disponibilidades { get; set; } = null!;
        public DbSet<Enfermedad> Enfermedades { get; set; } = null!;
        public DbSet<PacienteEnfermedad> PacientesEnfermedades { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Disponibilidad>()
                .Property(d => d.CostoCita)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Paciente>()
                .Property(p => p.Saldo)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Pago>()
                .Property(p => p.Monto)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Cita>()
                .HasOne(c => c.Paciente)
                .WithMany()
                .HasForeignKey(c => c.PacienteId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Cita>()
        .HasOne(c => c.Medico)
        .WithMany()
        .HasForeignKey(c => c.MedicoId)
        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cita>()
                .HasOne(c => c.Disponibilidad)
                .WithMany()
                .HasForeignKey(c => c.DisponibilidadId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cita>()
                .HasOne(c => c.Pago)
                .WithOne(p => p.Cita)
                .HasForeignKey<Pago>(p => p.CitaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Medico>(entity =>
            {
                entity.ToTable("Medicos");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Especialidad).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Correo).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Correo).IsUnique(); 
            });
        }
    }
}
