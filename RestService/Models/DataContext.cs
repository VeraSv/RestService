using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RestApi.Models
{
    public class DataContext : DbContext
    {
        public DbSet<DataModel> Data { get; set; }
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
