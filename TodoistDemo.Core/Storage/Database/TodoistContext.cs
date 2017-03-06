using Microsoft.EntityFrameworkCore;

namespace TodoistDemo.Core.Storage.Database
{
    public class TodoistContext: DbContext
    {
        public DbSet<DbItem> Items { get; set; }

        public DbSet<DbUser> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=todoist.db");
        }
    }
}