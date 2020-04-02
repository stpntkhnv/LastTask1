using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LastTask1.ViewModels;

namespace LastTask1.Models
{
    public class ItemContext : DbContext
    {
        public DbSet<Item> Items { get; set; }
        public ItemContext(DbContextOptions<ItemContext> options)
        : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
