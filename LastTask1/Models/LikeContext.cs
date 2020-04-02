using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LastTask1.Models
{
    public class LikeContext : DbContext
    {
        public DbSet<Like> Likes { get; set; }
        public LikeContext(DbContextOptions<LikeContext> options)
        : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
