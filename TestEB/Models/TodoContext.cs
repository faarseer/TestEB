using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TestEB.Models
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options)
            : base(options)
        {
        }

        public DbSet<TodoItem> TodoItems { get; set; }
        public DbSet<UserItem> UserItems { get; set; }
    }
}
