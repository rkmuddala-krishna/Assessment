using Microsoft.EntityFrameworkCore;
using DSO.OTP.API.Models;
using System;
using System.Collections.Generic;
namespace DSO.OTP.API.DB
{
    public partial class OTPDBContext : DbContext
    {

        public DbSet<OTP.API.Models.OTP> OTP { get; set; }

        public string DbPath { get; private set; }
        public OTPDBContext()
        {


        }
        public OTPDBContext(DbContextOptions<OTPDBContext> options)
                : base(options)
        {
        }
        //  protected override void OnConfiguring(DbContextOptionsBuilder options)
        //             => options.UseSqlite($"Data Source={DbPath}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // auto generated column
            modelBuilder.Entity<OTP.API.Models.OTP>()
                        .Property(f => f.ID)
                        .ValueGeneratedOnAdd();
            // Defualt datetime 
            //modelBuilder.Entity<OTP.API.Models.OTP>().Property(f => f.CreatedDate).HasDefaultValueSql("GETUTCDATE()");


            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }

}