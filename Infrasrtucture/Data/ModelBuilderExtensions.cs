﻿using Microsoft.EntityFrameworkCore;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Models;

namespace Infrasrtucture.Data
{
    public static class ModelBuilderExtensions
    {
        public static void ApplyDataBaseExtension(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);

                entity.Property(u => u.UserId)
                   .ValueGeneratedOnAdd()
                   .IsRequired();

                entity.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(128);

                entity.HasIndex(u => u.UserName)
                .IsUnique();

                entity.Property(u => u.NormalUserName)
                .HasComputedColumnSql("LOWER([UserName])");

                entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(128);

                entity.HasIndex(u => u.Email)
                .IsUnique();


                entity.Property(u => u.NormalEmail)
                .HasComputedColumnSql("LOWER([Email])");

                entity.Property(u => u.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            });


            modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany()
                .HasForeignKey(ur => ur.RoleId);
        }
    }
}
