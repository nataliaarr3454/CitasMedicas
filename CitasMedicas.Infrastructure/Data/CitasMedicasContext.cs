using CitasMedicas.Core.Entities;
using CitasMedicas.Core.Enums;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

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
        public DbSet<Security> Securities { get; set; } = null!;

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

            // Relaciones de Cita
            modelBuilder.Entity<Cita>()
                .HasOne(c => c.Paciente)
                .WithMany(p => p.Citas)
                .HasForeignKey(c => c.PacienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cita>()
                .HasOne(c => c.Medico)
                .WithMany(m => m.Citas)
                .HasForeignKey(c => c.MedicoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cita>()
                .HasOne(c => c.Disponibilidad)
                .WithMany(d => d.Citas)
                .HasForeignKey(c => c.DisponibilidadId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cita>()
                .HasOne(c => c.Pago)
                .WithOne(p => p.Cita)
                .HasForeignKey<Pago>(p => p.CitaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configurar Medico
            modelBuilder.Entity<Medico>(entity =>
            {
                entity.ToTable("Medicos");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.Especialidad)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.Correo)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.HasIndex(e => e.Correo).IsUnique();

                // Relación con Disponibilidades
                entity.HasMany(m => m.Disponibilidades)
                    .WithOne(d => d.Medico)
                    .HasForeignKey(d => d.MedicoId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configurar Paciente
            modelBuilder.Entity<Paciente>(entity =>
            {
                entity.ToTable("Pacientes");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.Apellido)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.Correo)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.HasIndex(e => e.Correo).IsUnique();
                entity.Property(e => e.Telefono)
                    .IsRequired()
                    .HasMaxLength(15);
                entity.Property(e => e.Direccion)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            // Configurar Security
            modelBuilder.Entity<Security>(entity =>
            {
                entity.ToTable("Security");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Login)
                    .IsRequired()
                    .HasMaxLength(50)
                .IsUnicode(false);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(15)
                    .HasConversion(
                        v => v.ToString(),
                        v => (RoleType)Enum.Parse(typeof(RoleType), v)
                    );
                entity.HasIndex(e => e.Login).IsUnique();
            });

            // Configurar Disponibilidad
            modelBuilder.Entity<Disponibilidad>(entity =>
            {
                entity.ToTable("Disponibilidades");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasDefaultValue("Disponible");
                entity.Property(e => e.HoraInicio)
                    .IsRequired();
                entity.Property(e => e.HoraFin)
                    .IsRequired();
            });

            // Configurar Pago
            modelBuilder.Entity<Pago>(entity =>
            {
                entity.ToTable("Pagos");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EstadoPago)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasDefaultValue("Pendiente");
                entity.Property(e => e.FechaPago)
                    .HasDefaultValueSql("GETDATE()");
            });
        }
    }
}
