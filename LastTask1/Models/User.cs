using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace LastTask1.Models
{
    public class User : IdentityUser
    {
        
        public int nItems { get; set; }

        public int nComments { get; set; }
        public int nLikes { get; set; }
        public int nCollections { get; set; }
        public string Role { get; set; }
    }
}