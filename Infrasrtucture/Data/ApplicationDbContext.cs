using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Entities.Models;

namespace Infrasrtucture.Data
{
    public class ApplicationDbContext<TUser> : DbContext
        where TUser : User
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<TUser> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Role> Roles { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ModelBuilderExtensions.ApplyDataBaseExtension(modelBuilder);
        }
    }
}
