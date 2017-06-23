using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace OneStopMinh.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool ConfirmedEmail { get; set; }
    }
}
