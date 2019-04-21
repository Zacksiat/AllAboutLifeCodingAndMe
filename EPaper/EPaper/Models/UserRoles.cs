using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPaper.Models
{
    public class UserRoles
    {
        public ApplicationUser Users { get; set; }
        public IdentityRole Roles { get; set; }
        
    }
}
