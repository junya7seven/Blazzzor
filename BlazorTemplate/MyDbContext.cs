using Entities.Models;
using Infrasrtucture.Data;
using Microsoft.EntityFrameworkCore;
using BlazorTemplate.Models;

namespace BlazorTemplate
{
    public class MyDbContext : ApplicationDbContext<ApplicationUser>
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }
    }


    
}
