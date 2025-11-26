using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using PipeSelectorApp.Models;

namespace PipeSelectorApp;

public class PipesDbContext : DbContext
{
    public DbSet<PipeNomenclature> PipeNomenclature { get; set; } = null!;
    public DbSet<PipeStrengthClass> PipeStrengthClass { get; set; } = null!;
    public DbSet<SteelMark> steelMark { get; set; } = null!;
    public DbSet<Product_type> Product_type { get; set; } = null!;
    public DbSet<PipeThread> PipeThread { get; set; } = null!;
    public DbSet<Section> Sections { get; set; } = null!;
    public DbSet<NomenclatureByStrengthClass> NomenclatureByStrengthClass { get; set; } = null!;
    public DbSet<Product_Nomenclature> Product_Nomenclature { get; set; } = null!;
    public DbSet<Product> Product { get; set; } = null!;
    public DbSet<Productivity> Productivity { get; set; } = null!;
    public DbSet<MainInfo> MainInfo { get; set; } = null!;
    public DbSet<PipesPerHourMap> PipesPerHourMap { get; set; } = null!;
    public DbSet<ShiftTask> ShiftTasks { get; set; } = null!;

    public PipesDbContext(DbContextOptions<PipesDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Явное сопоставление имён таблиц
        modelBuilder.Entity<PipeNomenclature>().ToTable("PipeNomenclature");
        modelBuilder.Entity<PipeStrengthClass>().ToTable("PipeStrengthClass");
        modelBuilder.Entity<SteelMark>().ToTable("SteelMark");
        modelBuilder.Entity<Product_type>().ToTable("Product_type");
        modelBuilder.Entity<PipeThread>().ToTable("PipeThread");
        modelBuilder.Entity<Section>().ToTable("Sections");
        modelBuilder.Entity<NomenclatureByStrengthClass>().ToTable("NomenclatureByStrengthClass");
        modelBuilder.Entity<Product_Nomenclature>().ToTable("Product_Nomenclature");
        modelBuilder.Entity<Product>().ToTable("Product");
        modelBuilder.Entity<Productivity>().ToTable("Productivity");
        modelBuilder.Entity<MainInfo>().ToTable("MainInfo");
        modelBuilder.Entity<PipesPerHourMap>().ToTable("PipesPerHourMap");
        modelBuilder.Entity<ShiftTask>().ToTable("ShiftTask");

        // Конфигурация связей
        modelBuilder.Entity<NomenclatureByStrengthClass>()
            .HasKey(n => n.id);
        modelBuilder.Entity<NomenclatureByStrengthClass>()
            .HasOne(n => n.PipeNomenclature)
            .WithMany()
            .HasForeignKey(n => n.PipeNom_id)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<NomenclatureByStrengthClass>()
            .HasOne(n => n.StrengthClass)
            .WithMany()
            .HasForeignKey(n => n.StrengthClass_id);

        modelBuilder.Entity<Product_Nomenclature>()
            .HasKey(p => p.id);
        modelBuilder.Entity<Product_Nomenclature>()
            .HasOne(p => p.PipeNomenclature)
            .WithMany()
            .HasForeignKey(p => p.PipeNom_id)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Product_Nomenclature>()
            .HasOne(p => p.StrengthClass)
            .WithMany()
            .HasForeignKey(p => p.StrClass_id);

        modelBuilder.Entity<Product>()
            .HasKey(p => p.id);
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Product_Nomenclature)
            .WithMany()
            .HasForeignKey(p => p.ProductNomencl_id)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Product>()
            .HasOne(p => p.PipeThread)
            .WithMany()
            .HasForeignKey(p => p.Thread_id);

        modelBuilder.Entity<Productivity>()
            .HasKey(p => p.Id);
        modelBuilder.Entity<Productivity>()
            .HasOne(p => p.Product)
            .WithMany()
            .HasForeignKey(p => p.Product_id)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Productivity>()
            .HasOne(p => p.Section)
            .WithMany()
            .HasForeignKey(p => p.Section_id);

        modelBuilder.Entity<MainInfo>()
            .HasKey(m => m.id);
        modelBuilder.Entity<MainInfo>()
            .HasOne(m => m.PipeNomenclature)
            .WithMany()
            .HasForeignKey(m => m.PipeNom_id)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<MainInfo>()
            .HasOne(m => m.NomenclatureByStrengthClass)
            .WithMany()
            .HasForeignKey(m => m.NomByStrClass_id);
        modelBuilder.Entity<MainInfo>()
            .HasOne(m => m.Productivity)
            .WithMany()
            .HasForeignKey(m => m.Prod_id);
        modelBuilder.Entity<MainInfo>()
            .HasOne(m => m.ProductType)
            .WithMany()
            .HasForeignKey(m => m.Type_id);
        modelBuilder.Entity<MainInfo>()
            .HasOne(m => m.SteelMark)
            .WithMany()
            .HasForeignKey(m => m.Steel_id);

        modelBuilder.Entity<PipesPerHourMap>()
            .HasKey(p => p.Id);
        modelBuilder.Entity<PipesPerHourMap>()
            .HasOne(p => p.PipeNomenclature)
            .WithMany()
            .HasForeignKey(p => p.PipeNom_id);
        modelBuilder.Entity<PipesPerHourMap>()
            .HasOne(p => p.Section)
            .WithMany()
            .HasForeignKey(p => p.Section_id)
            .OnDelete(DeleteBehavior.Cascade);

        // Настройка связи ShiftTask -> Productivity
        modelBuilder.Entity<ShiftTask>()
            .HasOne(st => st.Productivity)
            .WithMany()
            .HasForeignKey(st => st.productivity_id)
            .OnDelete(DeleteBehavior.SetNull); // Изменено с Restrict на SetNull для согласованности

        // Настройка связи ShiftTask -> Section
        modelBuilder.Entity<ShiftTask>()
            .HasOne(st => st.Section)
            .WithMany()
            .HasForeignKey(st => st.Section_id)
            .OnDelete(DeleteBehavior.NoAction);

        // Уникальный индекс на Section_id
        modelBuilder.Entity<ShiftTask>()
            .HasIndex(st => st.Section_id)
            .IsUnique();

    }
}