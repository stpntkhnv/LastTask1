using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LastTask1.Models
{
    public class TagContext : DbContext
    {
        public DbSet<Tag> Tags { get; set; }
        public TagContext(DbContextOptions<TagContext> options)
        : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
