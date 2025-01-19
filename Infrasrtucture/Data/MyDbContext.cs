using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Models;
using Microsoft.EntityFrameworkCore;


namespace Infrasrtucture.Data
{
    public class MyDbContext : ApplicationDbContext<ApplicationUser>
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }
    }
}
