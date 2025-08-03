using Microsoft.EntityFrameworkCore;
using ASPProject1.Web.Models;

namespace ASPProject1.Web.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<TodoTask> Tasks { get; set; }
    }
}
