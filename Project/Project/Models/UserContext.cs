using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using Project.Models;

namespace Project.Models
{
    public class UserContext : DbContext
    {

        /// <summary>
        /// Модель класса Users
        /// Поля: int Id, string Name, string Surname, string Email,  byte[] Photo, string Information
        /// </summary>
        public DbSet<User> Users { get;set; }

        /// <summary>
        /// Модель класса Authorizations
        /// Поля: int Id, string Email, string Password, string Role
        /// </summary>
        public DbSet<Authorization> Authorizations { get; set; }

        /// <summary>
        /// Модель класса Technologies
        /// Поля:   int SId, string Name
        /// </summary>
        public DbSet<Technology> Technologies { get; set; }

        /// <summary>
        /// Модель класса UserTechnologies
        /// Поля: int Id, int SId, int Level
        /// </summary>
        public DbSet<UserTechnology> UserTechnologies { get; set; }

        /// <summary>
        /// Модель класса TechnologyCharacteristics
        /// Поля:  int Id, String Description, int SId
        /// </summary>
        public DbSet<TechnologyCharacteristic> TechnologyCharacteristics { get; set; }

        /// <summary>
        /// Модель класса Characteristic
        /// Поля: int Id, string Name
        /// </summary>
        public DbSet<Characteristic> Characteristics { get; set; }

        /// <summary>
        /// Модель класса TestLink
        /// Поля: int NId,  String Link, int Level, int SId
        /// </summary>
        public DbSet<TestLink> TestLinks { get; set; }

        /// <summary>
        /// Модель класса DataLink
        /// Поля: int NId,  String Link, int Level, int SId
        /// </summary>
        public DbSet<DataLink> DataLinks { get; set; }

        /// <summary>
        /// Модель класса Curator
        /// Поля: int Id, int SId
        /// </summary>
        public DbSet<Curator> Curators { get; set; }

        /// <summary>
        /// Модель класса Grade
        /// Поля: int NId,  int Id, int Level, int SId
        /// </summary>
        public DbSet<Grade> Grades { get; set; }

        /// <summary>
        /// Модель класса GradeName
        /// Поля:  int Id,  String Name
        /// </summary>
        public DbSet<GradeName> GradeNames { get; set; }


        /// <summary>
        /// Модель класса Tree
        /// Поля: int Id, int UserId, string Name, TreeType TreeType
        /// </summary>
        public DbSet<Tree> Trees { get; set; }

        /// <summary>
        /// Модель класса Node
        /// Поля: int Id, int TreeId, int ParentId, string Topic
        /// </summary>
        public DbSet<Node> Nodes { get; set; }

        /// <summary>
        /// Конструктор UserContext с аргументом DbContextOptions<UserContext>
        /// </summary>
        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Конструктор UserContext без аргументов
        /// </summary>
        public UserContext()
        {
            Database.EnsureCreated();
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured)
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=BarsInno");
        }


        /// <summary>
        /// Создание зависимостей
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // использование Fluent API для UserTechnology
            modelBuilder.Entity<UserTechnology>().HasKey(ut => new { ut.Id, ut.SId });

            modelBuilder.Entity<UserTechnology>()
             .HasOne(ut => ut.User)
             .WithMany(u => u.UserTechnologies)
             .HasForeignKey(ut => ut.Id);

            modelBuilder.Entity<UserTechnology>()
                .HasOne(ut => ut.Technology)
                .WithMany(t => t.UserTechnologies)
                .HasForeignKey(ut => ut.SId);


            // использование Fluent API для TechnologyCharacteristic
            modelBuilder.Entity<TechnologyCharacteristic>().HasKey(tc => new { tc.Id, tc.SId });

            modelBuilder.Entity<TechnologyCharacteristic>()
             .HasOne(tc => tc.Characteristic)
             .WithMany(c => c.TechnologyCharacteristics)
             .HasForeignKey(tc => tc.Id);

            modelBuilder.Entity<TechnologyCharacteristic>()
                .HasOne(tc => tc.Technology)
                .WithMany(t => t.TechnologyCharacteristics)
                .HasForeignKey(tc => tc.SId);
            //Curator
            modelBuilder.Entity<Curator>().HasKey(c => new { c.Id, c.SId });

            modelBuilder.Entity<Curator>()
             .HasOne(c => c.User)
             .WithMany(u => u.Curators)
             .HasForeignKey(c => c.Id);

            modelBuilder.Entity<Curator>()
                .HasOne(c => c.Technology)
                .WithMany(t => t.Curators)
                .HasForeignKey(c => c.SId);

            //Grade
            modelBuilder.Entity<Grade>()
            .HasOne(g => g.Technology)
            .WithMany(t => t.Grades);
            modelBuilder.Entity<Grade>().HasKey(g => new { g.Id, g.SId });

            //Data Link
            modelBuilder.Entity<DataLink>()
            .HasOne(dl => dl.Technology)
            .WithMany(t => t.DataLinks);

            //Test Link
            modelBuilder.Entity<TestLink>()
            .HasOne(tl => tl.Technology)
            .WithMany(t => t.TestLinks);


            modelBuilder.Entity<Tree>()
                .Property(p => p.Id).ForNpgsqlHasComment("Идентификатор дерева");
            modelBuilder.Entity<Tree>()
                .Property(p => p.UserId).ForNpgsqlHasComment("Идентификатор владельца дерева");
            modelBuilder.Entity<Tree>()
                .Property(p => p.Name).ForNpgsqlHasComment("Название дерева");
            modelBuilder.Entity<Tree>()
                .Property(p => p.TreeType).ForNpgsqlHasComment("Тип дерева");

            // Tree
            modelBuilder.Entity<Tree>()
             .HasOne(ut => ut.User)
             .WithMany(u => u.Trees)
             .HasForeignKey(ut => ut.UserId);

            modelBuilder.Entity<Node>()
                .Property(p => p.Id).ForNpgsqlHasComment("Идентификатор узла");
            modelBuilder.Entity<Node>()
                .Property(p => p.TreeId).ForNpgsqlHasComment("Идентификатор дерева которому принадлежит этот узел");
            modelBuilder.Entity<Node>()
                .Property(p => p.ParentId).ForNpgsqlHasComment("Идентификатор родительского узла");
            modelBuilder.Entity<Node>()
                .Property(p => p.Topic).ForNpgsqlHasComment("Текст узла");

            //Node
            modelBuilder.Entity<Node>()
             .HasMany(p => p.ChildNodes)
             .WithOne(p => p.ParentNode)
             .HasForeignKey(p => p.ParentId);

            modelBuilder.Entity<Node>()
             .HasOne(p => p.Tree)
             .WithMany(p => p.Nodes)
             .HasForeignKey(p => p.TreeId);
            
        }
    }
}