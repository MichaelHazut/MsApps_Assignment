using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp.DataAccessLayer
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public bool CanConnect()
        {
            try
            {
                return this.Database.CanConnect();
            }
            catch
            {
                return false;
            }
        }
    }
}
