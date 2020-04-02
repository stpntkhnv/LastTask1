using LastTask1.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LastTask1.ViewModels
{
    public class AdminPanelViewModel
    {
        public User User { get; set; }
        public List<User> Users { get; set; }
        public List<IdentityRole> AllRoles { get; set; }
        public IList<string> UserRoles { get; set; }
    }
}
