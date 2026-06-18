using Microsoft.EntityFrameworkCore;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.ToTable("tasks");

            entity.HasKey(task => task.Id);

            entity.Property(task => task.Id)
                .HasColumnName("id")
                .ValueGeneratedNever();

            entity.Property(task => task.Title)
                .HasColumnName("title")
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(task => task.Description)
                .HasColumnName("description")
                .HasMaxLength(2000);

            entity.Property(task => task.IsCompleted)
                .HasColumnName("is_completed")
                .IsRequired();

            entity.Property(task => task.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            entity.Property(task => task.UpdateAt)
                .HasColumnName("updated_at");
        });
    }
}