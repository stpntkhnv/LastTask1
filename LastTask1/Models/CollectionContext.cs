using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LastTask1.Models
{
    public class CollectionContext : DbContext
    {
        public DbSet<Collection> Collections { get; set; }
        public CollectionContext(DbContextOptions<CollectionContext> options)
        : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
