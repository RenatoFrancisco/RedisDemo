using Microsoft.EntityFrameworkCore;
using RedisDemo.Models;

namespace RedisDemo.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Country> Countries { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }
    }
}