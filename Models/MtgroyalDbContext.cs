using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MTGRoyal.Models;

public partial class MtgroyalDbContext : DbContext
{
    public MtgroyalDbContext()
    {
    }

    public MtgroyalDbContext(DbContextOptions<MtgroyalDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Carta> Cartas { get; set; }

    public virtual DbSet<Colore> Colores { get; set; }

    public virtual DbSet<Rareza> Rarezas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=tcp:mtgroyal-server.database.windows.net,1433;Initial Catalog=MTGRoyalDB;Persist Security Info=False;User ID=MTGRoyalAdmin;Password=Ureni_77;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Carta>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cartas__3214EC079FB39A6A");

            entity.Property(e => e.Coleccion).HasMaxLength(100);
            entity.Property(e => e.ImagenUrl).HasColumnName("ImagenURL");
            entity.Property(e => e.Nombre).HasMaxLength(150);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Tipo).HasMaxLength(100);

            entity.HasOne(d => d.Rareza).WithMany(p => p.Carta)
                .HasForeignKey(d => d.RarezaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cartas_Rarezas");

            entity.HasMany(d => d.Colors).WithMany(p => p.Carta)
                .UsingEntity<Dictionary<string, object>>(
                    "CartaColore",
                    r => r.HasOne<Colore>().WithMany()
                        .HasForeignKey("ColorId")
                        .HasConstraintName("FK__CartaColo__Color__75A278F5"),
                    l => l.HasOne<Carta>().WithMany()
                        .HasForeignKey("CartaId")
                        .HasConstraintName("FK__CartaColo__Carta__74AE54BC"),
                    j =>
                    {
                        j.HasKey("CartaId", "ColorId").HasName("PK__CartaCol__1FD951DF6D580AC1");
                        j.ToTable("CartaColores");
                    });
        });

        modelBuilder.Entity<Colore>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Colores__3214EC07687422F2");

            entity.HasIndex(e => e.Nombre, "UQ__Colores__75E3EFCF9732394C").IsUnique();

            entity.Property(e => e.Nombre).HasMaxLength(50);
        });

        modelBuilder.Entity<Rareza>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Rarezas__3214EC076409824F");

            entity.HasIndex(e => e.Nombre, "UQ__Rarezas__75E3EFCF851792BB").IsUnique();

            entity.Property(e => e.Nombre).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
