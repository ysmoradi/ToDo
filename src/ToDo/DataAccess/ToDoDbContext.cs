using Microsoft.EntityFrameworkCore;
using Prism.AppModel;
using Prism.Services;
using System;
using System.IO;
using ToDo.Model;

namespace ToDo.DataAccess
{
    public class ToDoDbContext : DbContext
    {
        private readonly IDeviceService _deviceService;

        public ToDoDbContext(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbFileName = "toDoDb-V1.db";

            if (_deviceService.RuntimePlatform != RuntimePlatform.UWP)
                dbFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), dbFileName);

            optionsBuilder.UseSqlite($"Filename={dbFileName}");

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ToDoItem>()
                .HasOne(toDo => toDo.Group)
                .WithMany(toDoGroup => toDoGroup.ToDoItems)
                .HasForeignKey(toDo => toDo.GroupId)
                .IsRequired(required: false)
                .OnDelete(DeleteBehavior.SetNull);

            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<ToDoItem> ToDoItems { get; set; }

        public virtual DbSet<ToDoGroup> ToDoGroups { get; set; }
    }
}
