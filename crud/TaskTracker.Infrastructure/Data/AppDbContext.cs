using Microsoft.EntityFrameworkCore;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<User> Users => Set<User>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureUsers(modelBuilder);
        ConfigureProjects(modelBuilder);
        ConfigureProjectMembers(modelBuilder);
        ConfigureTasks(modelBuilder);
    }

    private static void ConfigureUsers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.HasKey(user => user.Id);

            entity.Property(user => user.Id)
                .HasColumnName("id")
                .ValueGeneratedNever();

            entity.Property(user => user.Login)
                .HasColumnName("login")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(user => user.Email)
                .HasColumnName("email")
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(user => user.Password)
                .HasColumnName("password")
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(user => user.Name)
                .HasColumnName("name")
                .HasMaxLength(150)
                .IsRequired();

            entity.HasIndex(user => user.Login)
                .IsUnique();

            entity.HasIndex(user => user.Email)
                .IsUnique();
        });
    }

    private static void ConfigureProjects(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>(entity =>
        {
            entity.ToTable("projects");

            entity.HasKey(project => project.Id);

            entity.Property(project => project.Id)
                .HasColumnName("id")
                .ValueGeneratedNever();

            entity.Property(project => project.Name)
                .HasColumnName("name")
                .HasMaxLength(200)
                .IsRequired();
        });
    }

    private static void ConfigureProjectMembers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProjectMember>(entity =>
        {
            entity.ToTable("project_members");

            entity.HasKey(member => new 
            {
                member.UserId,
                member.ProjectId
            });

            entity.Property(member => member.UserId)
                .HasColumnName("user_id");

            entity.Property(member => member.ProjectId)
                .HasColumnName("project_id");

            entity.Property(member => member.Role)
                .HasColumnName("role")
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            entity.HasOne(member => member.User)
                .WithMany(user => user.ProjectMembers)
                .HasForeignKey(member => member.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(member => member.Project)
                .WithMany(project => project.Members)
                .HasForeignKey(member => member.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureTasks(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.ToTable("tasks");

            entity.HasKey(task => task.Id);

            entity.Property(task => task.Id)
                .HasColumnName("id")
                .ValueGeneratedNever();

            entity.Property(task => task.ProjectId)
                .HasColumnName("project_id")
                .IsRequired();

            entity.Property(task => task.Title)
                .HasColumnName("title")
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(task => task.Description)
                .HasColumnName("description")
                .HasMaxLength(2000);

            entity.Property(task => task.Deadline)
                .HasColumnName("deadline");

            entity.Property(task => task.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(task => task.AssignedUserId)
                .HasColumnName("assigned_user_id");

            entity.Property(task => task.AuthorId)
                .HasColumnName("author_id")
                .IsRequired();

            entity.Property(task => task.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            entity.Property(task => task.UpdateAt)
                .HasColumnName("updated_at");

            entity.HasOne(task => task.Project)
                .WithMany(project => project.Tasks)
                .HasForeignKey(task => task.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(task => task.Author)
                .WithMany(user => user.AuthoredTasks)
                .HasForeignKey(task => task.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(task => task.AssignedUser)
                .WithMany(user => user.AssignedTasks)
                .HasForeignKey(task => task.AssignedUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}